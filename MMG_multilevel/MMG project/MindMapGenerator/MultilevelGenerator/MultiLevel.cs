using System;
using System.Collections.Generic;
using System.Text;
using OurMindMapOntology;
using SyntacticAnalyzer;
using mmTMR;
using MindMapMeaningRepresentation;



namespace MultilevelGenerator
{
    
    public enum FrameStatus
    {
        Hidden,
        Grouped,
        Present
    }

    public class MultiLevel
    {
        public Dictionary<NounFrame, Dictionary<MindMapConcept, List<Frame>>> MainNounFrames_VerbFrames;
        public Dictionary<Frame, List<Dictionary<MindMapConcept, List<Frame>>>> VerbFrames_NounFrames;
        public Dictionary<Frame, Frame> NewFrame_OriginalFrame;
        MindMapTMR TMR;
        List<double> NounFramesWeights;
        List<double> VerbFramesWeights;
        public List<NounFrame> MainNounFrames;
        public List<FrameStatus> FramesStatus;



        public MultiLevel(MindMapTMR TMR)
        {
            this.TMR = TMR;

            //Weight Assigning..
            DirectRelationBasedTMRWeighter2 drbw = new DirectRelationBasedTMRWeighter2(this.TMR);
            this.NounFramesWeights = drbw.GetNounFrameWeights();
            this.VerbFramesWeights = drbw.GetVerbFrameWeights();

        }

        public MindMapTMR Run()
        {
            MainNounFrames_VerbFrames = new Dictionary<NounFrame, Dictionary<MindMapConcept, List<Frame>>>();
            VerbFrames_NounFrames = new Dictionary<Frame,List<Dictionary<MindMapConcept,List<Frame>>>>();
            NewFrame_OriginalFrame = new Dictionary<Frame, Frame>();

            //new TMR..
            MindMapTMR NewTMR = new MindMapTMR();

            //Initializing el status beta3 kol frame..
            this.FramesStatus = new List<FrameStatus>();
            for (int i = 0; i < this.TMR.Nounframes.Count + this.TMR.VerbFrames.Count; i++)
            {
                this.FramesStatus.Add(FrameStatus.Hidden);
            }

            //Weight-based partitioning of the nounframes..
            List<Frame> Nounframes = new List<Frame>();
            foreach (Frame f in this.TMR.Nounframes)
            {
                Nounframes.Add(f);
            }
            WeightBasedPartitioner wbp1 = new WeightBasedPartitioner(Nounframes, this.NounFramesWeights);
            List<List<Frame>> Clusters = wbp1.Partition();

            //getting maximum centroid  cluster
            List<int> dummy = getMaximumCentroidCluster(wbp1);
            this.MainNounFrames = new List<NounFrame>();

            foreach (int i in dummy)
            {
                this.MainNounFrames.Add((NounFrame)wbp1.Frames[i]);
                NewTMR.Nounframes.Add((NounFrame)wbp1.Frames[i]);
                this.NewFrame_OriginalFrame.Add((NounFrame)wbp1.Frames[i], wbp1.Frames[i]);
                this.FramesStatus[this.TMR.Nounframes.IndexOf((NounFrame)wbp1.Frames[i])] = FrameStatus.Present;
            }

            foreach (NounFrame main_nf1 in this.MainNounFrames)
            {
                foreach (NounFrame main_nf2 in this.MainNounFrames)
                {
                    if (main_nf1 != main_nf2)
                    {
                        List<List<Frame>> paths = new List<List<Frame>>();
                        List<List<CaseRole>> relations = new List<List<CaseRole>>();
                        GetRelations((Frame)main_nf1,(Frame)main_nf2,out paths,out relations);

                        foreach (List<Frame> path in paths)
                        {
                            foreach (Frame f in path)
                            {
                                if (f is NounFrame)
                                {
                                    if (this.FramesStatus[this.TMR.Nounframes.IndexOf((NounFrame)f)] == FrameStatus.Hidden)
                                    {
                                        NewTMR.Nounframes.Add((NounFrame)f);
                                        this.FramesStatus[this.TMR.Nounframes.IndexOf((NounFrame)f)] = FrameStatus.Present;
                                    }
                                }
                                else
                                {
                                    if (this.FramesStatus[this.TMR.Nounframes.Count + this.TMR.VerbFrames.IndexOf((VerbFrame)f)] == FrameStatus.Hidden)
                                    {
                                        NewTMR.VerbFrames.Add((VerbFrame)f);
                                        this.FramesStatus[this.TMR.Nounframes.Count + this.TMR.VerbFrames.IndexOf((VerbFrame)f)] = FrameStatus.Present;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < this.MainNounFrames.Count; i++)
            {
                List<VerbFrame> nf_verbframes = new List<VerbFrame>();
                List<double> nf_verbframes_weights = new List<double>();

                Dictionary<CaseRole, List<VerbFrame>> AssociatedActions = this.TMR.GetNounFrameAssociatedactions(this.TMR.Nounframes.IndexOf(this.MainNounFrames[i]));
                foreach (CaseRole cr in AssociatedActions.Keys)
                {
                    foreach (VerbFrame vf in AssociatedActions[cr])
                    {
                        if (FramesStatus[this.TMR.VerbFrames.IndexOf(vf) + this.TMR.Nounframes.Count] == FrameStatus.Hidden && nf_verbframes.Contains(vf) == false)
                        {
                            nf_verbframes.Add(vf);
                            nf_verbframes_weights.Add(this.VerbFramesWeights[this.TMR.VerbFrames.IndexOf(vf)]);
                        }
                    }
                }

                List<Frame> Verbframes = new List<Frame>();
                foreach (Frame f in nf_verbframes)
                {
                    Verbframes.Add(f);
                }

                Dictionary<MindMapConcept, List<Frame>> concept_VerbFrames = new Dictionary<MindMapConcept, List<Frame>>();
                Dictionary<MindMapConcept, List<Frame>> concept_NounFrames = new Dictionary<MindMapConcept, List<Frame>>();

                concept_VerbFrames = groupFrames(Verbframes);
                this.MainNounFrames_VerbFrames.Add(this.MainNounFrames[i], concept_VerbFrames);

                foreach (MindMapConcept c in concept_VerbFrames.Keys)
                {
                    if (concept_VerbFrames[c].Count > 1)
                    {
                        List<NounFrame> nounframe = new List<NounFrame>();
                        nounframe.Add(this.MainNounFrames[i]);
                        NewTMR.VerbFrames.Add(new VerbFrame(c.Text, c));
                        NewTMR.VerbFrames[NewTMR.VerbFrames.Count - 1].CaseRoles.Add(CaseRole.Agent, nounframe);
                    }
                    else
                    {
                        VerbFrame original_verbframe = (VerbFrame)concept_VerbFrames[c][0];
                        VerbFrame verbFrame = cloneVerbFrame(original_verbframe);

                        int index = this.TMR.VerbFrames.IndexOf(original_verbframe);
                        foreach (CaseRole cr in verbFrame.CaseRoles.Keys)
                        {
                            List<Frame> NounFrames = new List<Frame>();
                            foreach (NounFrame nf in verbFrame.CaseRoles[cr])
                            {
                                if(this.MainNounFrames.Contains(nf) == false)
                                    NounFrames.Add(nf);
                            }
                            if (NounFrames.Count > 5)
                            {
                                concept_NounFrames = groupFrames(NounFrames);
                                if (this.VerbFrames_NounFrames.ContainsKey(concept_VerbFrames[c][0]) == false)
                                {
                                    List<Dictionary<MindMapConcept, List<Frame>>> list = new List<Dictionary<MindMapConcept, List<Frame>>>();
                                    list.Add(concept_NounFrames);
                                    this.VerbFrames_NounFrames.Add(concept_VerbFrames[c][0], list);
                                }
                                else
                                {
                                    this.VerbFrames_NounFrames[concept_VerbFrames[c][0]].Add(concept_NounFrames);
                                }

                                this.FramesStatus[this.TMR.VerbFrames.IndexOf(verbFrame) + this.TMR.Nounframes.Count] = FrameStatus.Present;

                                foreach (MindMapConcept c_nfs in concept_NounFrames.Keys)
                                {
                                    if (concept_NounFrames[c_nfs].Count > 1)
                                    {
                                        NounFrame newNounFrame = new NounFrame(c_nfs.Text, c_nfs);
                                        NewTMR.Nounframes.Add(newNounFrame);
                                        verbFrame.CaseRoles[cr].Add(newNounFrame);

                                        foreach (Frame frame in concept_NounFrames[c_nfs])
                                        {
                                            this.FramesStatus[this.TMR.Nounframes.IndexOf((NounFrame)frame)] = FrameStatus.Grouped;
                                            verbFrame.CaseRoles[cr].Remove((NounFrame)frame);
                                        }
                                    }

                                    else
                                    {
                                        this.FramesStatus[this.TMR.Nounframes.IndexOf((NounFrame)concept_NounFrames[c_nfs][0])] = FrameStatus.Present;
                                    }
                                }
                            }
                        }
                        NewTMR.VerbFrames.Add(verbFrame);
                        NewFrame_OriginalFrame.Add(verbFrame, this.TMR.VerbFrames[index]);
                    }
                }
            }

            for (int k = 0; k < NewTMR.VerbFrames.Count; k++)
            {
                VerbFrame vf  =  NewTMR.VerbFrames[k];
                foreach (TemporalRelationType trt in vf.TemporalRelations.Keys)
                {
                    for (int i = 0; i < vf.TemporalRelations[trt].Count; i++)
                    {
                        if (NewTMR.VerbFrames.Contains(vf.TemporalRelations[trt][i]) == false)
                        {
                            NewTMR.VerbFrames.Add(vf.TemporalRelations[trt][i]);
                        }
                    }
                }
            }

            for (int k = 0; k < NewTMR.VerbFrames.Count; k++)
            {
                VerbFrame vf = NewTMR.VerbFrames[k];
                foreach (DomainRelationType drt in vf.DomainRelations.Keys)
                {
                    for (int i = 0; i < vf.DomainRelations[drt].Count; i++)
                    {
                        if (NewTMR.VerbFrames.Contains(vf.DomainRelations[drt][i]) == false)
                        {
                            NewTMR.VerbFrames.Add(vf.DomainRelations[drt][i]);
                        }
                    }
                }
            }

            foreach (VerbFrame vf in NewTMR.VerbFrames)
            {
                foreach (CaseRole cr in vf.CaseRoles.Keys)
                {
                    for (int i = 0; i < vf.CaseRoles[cr].Count; i++)
                    {
                        if (NewTMR.Nounframes.Contains(vf.CaseRoles[cr][i]) == false)
                        {
                            NewTMR.Nounframes.Add(vf.CaseRoles[cr][i]);
                        }
                    }
                }
            }

            foreach (VerbFrame vf in NewTMR.VerbFrames)
            {
                foreach (DomainRelationType drt in vf.DomainRelations_n.Keys)
                {
                    for (int i = 0; i < vf.DomainRelations_n[drt].Count; i++)
                    {
                        if (NewTMR.Nounframes.Contains(vf.DomainRelations_n[drt][i]) == false)
                        {
                            NewTMR.Nounframes.Add(vf.DomainRelations_n[drt][i]);
                        }
                    }
                }
            }

            foreach (VerbFrame vf in NewTMR.VerbFrames)
            {
                foreach (TemporalRelationType trt in vf.TemporalRelations_n.Keys)
                {
                    for (int i = 0; i < vf.TemporalRelations_n[trt].Count; i++)
                    {
                        if (NewTMR.Nounframes.Contains(vf.TemporalRelations_n[trt][i]) == false)
                        {
                            NewTMR.Nounframes.Add(vf.TemporalRelations_n[trt][i]);
                        }
                    }
                }
            }

            List<NounFrame> dummylist = new List<NounFrame>();
            foreach (NounFrame nf in NewTMR.Nounframes)
            {
                foreach (CaseRole cr in nf.Ownerof.Keys)
                {
                    for (int i = 0; i < nf.Ownerof[cr].Count; i++)
                    {
                        if (TMR.Nounframes.Contains(nf.Ownerof[cr][i]) == true && NewTMR.Nounframes.Contains(nf.Ownerof[cr][i]) == false && this.FramesStatus[this.TMR.Nounframes.IndexOf(nf.Ownerof[cr][i])] != FrameStatus.Grouped)
                        {
                            dummylist.Add(nf.Ownerof[cr][i]);
                        }
                    }
                }
            }

            foreach (NounFrame nf in TMR.Nounframes)
            {
                foreach (CaseRole cr in nf.Ownerof.Keys)
                {
                    foreach (NounFrame nf2 in nf.Ownerof[cr])
                    {
                        if (NewTMR.Nounframes.Contains(nf2) == true && NewTMR.Nounframes.Contains(nf) == false)
                            NewTMR.Nounframes.Add(nf);
                    }
                }
            }

            

            foreach (NounFrame nf in dummylist)
            {
                if(NewTMR.Nounframes.Contains(nf) == false)
                    NewTMR.Nounframes.Add(nf);
            }

            

            return NewTMR;
        }

        private VerbFrame cloneVerbFrame(VerbFrame original_verbframe)
        {
            VerbFrame verbFrame = new VerbFrame(original_verbframe.VerbName, original_verbframe.Concept);
            verbFrame.VerbNegation = original_verbframe.VerbNegation;

            foreach (CaseRole cr in original_verbframe.CaseRoles.Keys)
            {
                foreach (NounFrame nf in original_verbframe.CaseRoles[cr])
                {
                    if (verbFrame.CaseRoles.ContainsKey(cr) == false)
                    {
                        List<NounFrame> dummy2 = new List<NounFrame>();
                        dummy2.Add(nf);
                        verbFrame.CaseRoles.Add(cr, dummy2);
                    }
                    else
                    {
                        verbFrame.CaseRoles[cr].Add(nf);
                    }
                }
            }

            foreach (TemporalRelationType trt in original_verbframe.TemporalRelations.Keys)
            {
                foreach (VerbFrame vf in original_verbframe.TemporalRelations[trt])
                {
                    if (verbFrame.TemporalRelations.ContainsKey(trt) == false)
                    {
                        List<VerbFrame> dummy2 = new List<VerbFrame>();
                        dummy2.Add(vf);
                        verbFrame.TemporalRelations.Add(trt, dummy2);
                    }
                    else
                    {
                        verbFrame.TemporalRelations[trt].Add(vf);
                    }
                }   
            }

            foreach (DomainRelationType drt in original_verbframe.DomainRelations.Keys)
            {
                foreach (VerbFrame vf in original_verbframe.DomainRelations[drt])
                {
                    if (verbFrame.DomainRelations.ContainsKey(drt) == false)
                    {
                        List<VerbFrame> dummy2 = new List<VerbFrame>();
                        dummy2.Add(vf);
                        verbFrame.DomainRelations.Add(drt, dummy2);
                    }
                    else
                    {
                        verbFrame.DomainRelations[drt].Add(vf);
                    }
                }
            }

            foreach (DomainRelationType drt in original_verbframe.DomainRelations_n.Keys)
            {
                foreach (NounFrame nf in original_verbframe.DomainRelations_n[drt])
                {
                    if (verbFrame.DomainRelations_n.ContainsKey(drt) == false)
                    {
                        List<NounFrame> dummy2 = new List<NounFrame>();
                        dummy2.Add(nf);
                        verbFrame.DomainRelations_n.Add(drt, dummy2);
                    }
                    else
                    {
                        verbFrame.DomainRelations_n[drt].Add(nf);
                    }
                }
            }

            foreach (TemporalRelationType trt in original_verbframe.TemporalRelations_n.Keys)
            {
                foreach (NounFrame nf in original_verbframe.TemporalRelations_n[trt])
                {
                    if (verbFrame.TemporalRelations_n.ContainsKey(trt) == false)
                    {
                        List<NounFrame> dummy2 = new List<NounFrame>();
                        dummy2.Add(nf);
                        verbFrame.TemporalRelations_n.Add(trt, dummy2);
                    }
                    else
                    {
                        verbFrame.TemporalRelations_n[trt].Add(nf);
                    }
                }
            }

            return verbFrame;
        }


        private void getShortertPathtoMainNounFrames(NounFrame nf, out List<Frame> Path, out List<CaseRole> Relations)
        {
            Path = null;
            Relations = null;
            int ShortestPathDistance = int.MaxValue;
            List<List<Frame>> Paths;
            List<List<CaseRole>> PathsRelations; 

            foreach (NounFrame mainnounframe in this.MainNounFrames)
            {
                checkedRelations.Clear();
                GetRelations(nf, mainnounframe, out Paths, out PathsRelations);

                for (int i = 0; i < Paths.Count; i++)
                {
                    if (Paths[i].Count < ShortestPathDistance)
                    {
                        Path = Paths[i];
                        Relations = PathsRelations[i];
                        ShortestPathDistance = Paths[i].Count;
                    }
                }
            }
        }

        private void Normalize_Centroids(WeightBasedPartitioner wbp)
        {
            double minimum = double.MaxValue;
            for (int i = 0; i < wbp.KCentroids[wbp.optimalK].Count; i++)
            {

                if (wbp.KCentroids[wbp.optimalK][i] < minimum)
                {
                    minimum = wbp.KCentroids[wbp.optimalK][i];
                }
            }

            double maximum = 0;

            for (int i = 0; i < wbp.KCentroids[wbp.optimalK].Count; i++)
            {
                wbp.KCentroids[wbp.optimalK][i] -= minimum;
                if (wbp.KCentroids[wbp.optimalK][i] > maximum)
                {
                    maximum = wbp.KCentroids[wbp.optimalK][i];
                }
            }

            if (maximum != 0)
            {
                for (int i = 0; i < wbp.KCentroids[wbp.optimalK].Count; i++)
                {
                    wbp.KCentroids[wbp.optimalK][i] = wbp.KCentroids[wbp.optimalK][i] / maximum;
                }
            }
        }

        Dictionary<Frame, List<Frame>> checkedRelations = new Dictionary<Frame, List<Frame>>();
        private void GetRelations(Frame f1, Frame f2, out List<List<Frame>> frames, out List<List<CaseRole>> relations)
        {
            List<List<Frame>> frames_rest;
            List<List<CaseRole>> relations_rest;

            frames = new List<List<Frame>>();
            relations = new List<List<CaseRole>>();
            Dictionary<CaseRole, List<Frame>> AssociatedActions = new Dictionary<CaseRole,List<Frame>>();

            if (f1 is NounFrame)
            {
                Dictionary<CaseRole, List<VerbFrame>>  N_AssociatedActions = this.TMR.GetNounFrameAssociatedactions(this.TMR.Nounframes.IndexOf((NounFrame)f1));
                foreach (CaseRole cr in N_AssociatedActions.Keys)
                {
                    AssociatedActions.Add(cr, new List<Frame>());
                    foreach (Frame frame in N_AssociatedActions[cr])
                    {
                        AssociatedActions[cr].Add(frame);
                    }
                }
            }

            else
            {
                Dictionary<CaseRole, List<NounFrame>>  V_AssociatedActions = this.TMR.VerbFrames[this.TMR.VerbFrames.IndexOf((VerbFrame)f1)].CaseRoles;
                foreach (CaseRole cr in V_AssociatedActions.Keys)
                {
                    AssociatedActions.Add(cr, new List<Frame>());
                    foreach (Frame frame in V_AssociatedActions[cr])
                    {
                        AssociatedActions[cr].Add(frame);
                    }
                }
            }

            foreach (CaseRole cr in AssociatedActions.Keys)
            {
                foreach (Frame f in AssociatedActions[cr])
                {
                    if (f == f2)
                    {
                        List<Frame> frames_list = new List<Frame>();
                        frames_list.Add(f1);
                        frames_list.Add(f2);

                        frames.Add(frames_list);

                        List<CaseRole> relations_list = new List<CaseRole>();
                        relations_list.Add(cr);

                        relations.Add(relations_list);
                        return;
                    }

                    else
                    {

                        if (checkedRelations.ContainsKey(f1) == false)
                        {
                            List<Frame> list = new List<Frame>();
                            list.Add(f);
                            checkedRelations.Add(f1, list);
                        }

                        else
                        {
                            if (checkedRelations[f1].Contains(f) == false)
                            {
                                checkedRelations[f1].Add(f);
                            }
                        }

                        if (checkedRelations.ContainsKey(f) == false)
                        {
                            List<Frame> list = new List<Frame>();
                            list.Add(f1);
                            checkedRelations.Add(f, list);
                            GetRelations(f, f2, out frames_rest, out relations_rest);
                            for (int i = 0; i < frames_rest.Count; i++)
                            {
                                frames_rest[i].Insert(0, f1);
                                frames.Add(frames_rest[i]);
                                relations_rest[i].Insert(0, cr);
                                relations.Add(relations_rest[i]);
                            }
                        }

                        else
                        {
                            if (checkedRelations[f].Contains(f1) == false)
                            {
                                checkedRelations[f].Add(f1);
                                GetRelations(f, f2, out frames_rest, out relations_rest);
                                for (int i = 0; i < frames_rest.Count; i++)
                                {
                                    frames_rest[i].Insert(0, f1);
                                    frames.Add(frames_rest[i]);
                                    relations_rest[i].Insert(0, cr);
                                    relations.Add(relations_rest[i]);

                                }
                            }
                            
                        }
   
                    }
                    
                }
            }
        }

        
        private Dictionary<MindMapConcept,List<Frame>> groupFrames(List<Frame> Frames)
        {
            Dictionary<MindMapConcept,List<Frame>> concept_frames;
            ConceptPartitioner cp = new ConceptPartitioner(Frames);
            concept_frames = cp.PartitionConcepts();
            return concept_frames;
        }

        private List<int> getMaximumCentroidCluster(WeightBasedPartitioner wbp)
        {
            double Maximum = 0;
            int index = -1;
            for (int i = 0; i < wbp.KCentroids[wbp.optimalK].Count; i++)
            {
                if (wbp.KCentroids[wbp.optimalK][i] >= Maximum)
                {
                    Maximum = wbp.KCentroids[wbp.optimalK][i];
                    index = i;
                }
            }

            return wbp.KGroups[wbp.optimalK][index];
        }

        private int getMaximumCentroidIndex(WeightBasedPartitioner wbp)
        {
            double Maximum = 0;
            int index = -1;
            for (int i = 0; i < wbp.KCentroids[wbp.optimalK].Count; i++)
            {
                if (wbp.KCentroids[wbp.optimalK][i] >= Maximum)
                {
                    Maximum = wbp.KCentroids[wbp.optimalK][i];
                    index = i;
                }
            }
            return index;
        }

        private int getHighestWeightedFrame(List<Frame> list)
        {
            double Maximum = 0;
            int index = -1;

            for(int i=0;i<list.Count;i++)
            {
                if (list[i] is NounFrame)
                {
                    if (this.NounFramesWeights[this.TMR.Nounframes.IndexOf((NounFrame)list[i])] > Maximum)
                    {
                        Maximum = this.NounFramesWeights[this.TMR.Nounframes.IndexOf((NounFrame)list[i])];
                        index = i;
                    }
                }
            }

            return index;
        }

       
    }
}
