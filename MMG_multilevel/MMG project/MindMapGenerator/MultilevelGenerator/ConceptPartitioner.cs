using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurMindMapOntology;
using MindMapMeaningRepresentation;
using SyntacticAnalyzer;
using mmTMR;

namespace MultilevelGenerator
{
    public class ConceptPartitioner
    {
        
        Dictionary<MindMapConcept, List<Frame>> CurrentConcepts;
        Dictionary<Frame, int> FrameDistance;
        Dictionary<Frame, List<Frame>> FrameGroup;
        Dictionary<Frame, bool> FrameMarked;
        List<Frame> Frames;

        public ConceptPartitioner(List<Frame> Frames)
        {
            CurrentConcepts = new Dictionary<MindMapConcept, List<Frame>>();
            FrameGroup = new Dictionary<Frame, List<Frame>>();
            FrameMarked = new Dictionary<Frame, bool>();
            this.Frames = Frames;

            foreach (Frame f in Frames)
            {
                FrameMarked.Add(f, false);
                if (CurrentConcepts.ContainsKey(f.Concept) == true)
                {
                    CurrentConcepts[f.Concept].Add(f);
                    foreach (Frame frame in CurrentConcepts[f.Concept])
                    {
                        FrameMarked[frame] = true;
                    }
                }
                else
                {
                    List<Frame> list = new List<Frame>();
                    list.Add(f);
                    CurrentConcepts.Add(f.Concept, list);
                }
            }
        }

        public Dictionary<MindMapConcept, List<Frame>> PartitionConcepts()
        {
            if (CurrentConcepts.Count > 1)
            {
                FrameDistance = new Dictionary<Frame, int>();

                foreach (Frame f1 in this.Frames)
                {
                    List<Frame> group = new List<Frame>();
                    group.Add(f1);
                    int Minimum = 2;
                    foreach (Frame f2 in this.Frames)
                    {
                        if (f1 != f2)
                        {
                            DistanceInfo info = MindMapConcept.Distance(f1.Concept, f2.Concept);
                            if (info.Distance < Minimum)
                            {
                                Minimum = info.Distance;
                                group.Clear();
                                group.Add(f1);
                                group.Add(f2);
                            }
                            else if (info.Distance == Minimum)
                            {
                                group.Add(f2);
                            }
                        }
                    }

                    FrameGroup.Add(f1, group);
                    FrameDistance.Add(f1, Minimum);

                }

                while (AllMarked() == false)
                {
                    List<Frame> list = getSmallestDistanceGroup();
                    List<MindMapConcept> concepts = new List<MindMapConcept>();
                    foreach (Frame f in list)
                    {
                        concepts.Add(f.Concept);
                    }

                    if (list.Count > 1)
                    {
                        MindMapConcept ParentConcept = getNearestParent(concepts);

                        List<Frame> frames = new List<Frame>();
                        foreach (Frame f in list)
                        {
                            FrameMarked[f] = true;
                            frames.Add(f);
                            CurrentConcepts.Remove(f.Concept);
                        }
                        CurrentConcepts.Add(ParentConcept, frames);
                    }
                    else
                    {
                        foreach (Frame f in list)
                            FrameMarked[f] = true;
                    }
                }
            }
            return CurrentConcepts;

        }

        private bool AllMarked()
        {
            foreach (Frame f in this.Frames)
            {
                if (this.FrameMarked[f] == false)
                    return false;
            }

            return true;
        }

        private List<Frame> getSmallestDistanceGroup()
        {
            int Minimum = int.MaxValue;
            List<Frame> list = new List<Frame>();

            foreach (Frame f in this.Frames)
            {
                if (this.FrameMarked[f] == false)
                {
                    if (this.FrameDistance[f] < Minimum)
                    {
                        list.Clear();
                        Minimum = this.FrameDistance[f];
                        foreach (Frame _f in this.FrameGroup[f])
                        {
                            if (this.FrameMarked[_f] == false)
                                list.Add(_f);
                        }
                    }
                }
            }
            return list;
        }

        private MindMapConcept getNearestParent(List<MindMapConcept> list)
        {
            MindMapConcept Concept = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                DistanceInfo info = MindMapConcept.Distance(Concept, list[i + 1]);
                Concept = info.Parent;
            }

            return Concept;
        }

    }
}
