using System;
using System.Collections.Generic;
using System.Text;
using mmTMR;

namespace MindMapMeaningRepresentation
{
    public class DirectRelationBasedTMRWeighter2 :WeightAssigner
    {
        public DirectRelationBasedTMRWeighter2(MindMapTMR mindMapTMR):base(mindMapTMR)
        {
            LoadRelationWeights();
            LoadRelationFactors();
        }

        private Dictionary<CaseRole, double> caseRoleFactors = new Dictionary<CaseRole, double>();
        private Dictionary<TemporalRelationType, double> temporalRelationFactors = new Dictionary<TemporalRelationType, double>();
        private Dictionary<DomainRelationType, double> domainRelationFactors = new Dictionary<DomainRelationType, double>();

        protected void LoadRelationFactors() //here is where we alter the Factors
        {
            caseRoleFactors.Clear();
            temporalRelationFactors.Clear();
            domainRelationFactors.Clear();

            #region CaseRole Factors
            caseRoleFactors.Add(CaseRole.Agent, 2.0);
            caseRoleFactors.Add(CaseRole.Theme, 1.50);
            caseRoleFactors.Add(CaseRole.Beneficiary, 1.50);
            caseRoleFactors.Add(CaseRole.Cotheme, 1.00);
            caseRoleFactors.Add(CaseRole.Accompanier, 0.80);
            caseRoleFactors.Add(CaseRole.Instrument, 0.60);
            caseRoleFactors.Add(CaseRole.Source, 0.80);
            caseRoleFactors.Add(CaseRole.Destination, 0.80);
            caseRoleFactors.Add(CaseRole.Path, 0.50);
            caseRoleFactors.Add(CaseRole.location, 0.80);
            caseRoleFactors.Add(CaseRole.time, 0.80);
            caseRoleFactors.Add(CaseRole.purpose, 0.60);
            caseRoleFactors.Add(CaseRole.reason, 0.60);
            caseRoleFactors.Add(CaseRole.OwnerOf, 0.50);
            caseRoleFactors.Add(CaseRole.Possession, 0.50);
            caseRoleFactors.Add(CaseRole.unknown, 0.50);
            caseRoleFactors.Add(CaseRole.use, 0.60);
            caseRoleFactors.Add(CaseRole.focus, 0.60);
            caseRoleFactors.Add(CaseRole.Action, 0.60);
            caseRoleFactors.Add(CaseRole.among, 0.40);
            caseRoleFactors.Add(CaseRole.Means, 0.50);
            caseRoleFactors.Add(CaseRole.Under, 0.50);
            caseRoleFactors.Add(CaseRole.example, 0.60);

            #endregion
            #region Temporal Relation Factors
            temporalRelationFactors.Add(TemporalRelationType.After, 0.30);
            temporalRelationFactors.Add(TemporalRelationType.Before, 0.30);
            temporalRelationFactors.Add(TemporalRelationType.Concurrent, 0.40);
            #endregion
            #region Domain Relation Factors
            domainRelationFactors.Add(DomainRelationType.ExpectedResult, 0.60);
            domainRelationFactors.Add(DomainRelationType.UnExpectedResult, 0.80);
            domainRelationFactors.Add(DomainRelationType.Reason, 0.80);
            domainRelationFactors.Add(DomainRelationType.How, 0.60);
            domainRelationFactors.Add(DomainRelationType.Completion, 0.60);
            domainRelationFactors.Add(DomainRelationType.place, 0.80);
            #endregion
        }

        public override double WeighNounFrame(int nounFrameIndex)
        {
            LoadRelationWeights();
            LoadRelationFactors();

            NounFrame NF = _mindMapTMR.Nounframes[nounFrameIndex];
            //relations of noun frames ---> CaseRoles
            Dictionary<CaseRole, List<VerbFrame>> Associatedactions = MindMapTMR.GetNounFrameAssociatedactions(nounFrameIndex);
            double Weight = 0;

            //Array caseRoleArr = Enum.GetValues(typeof(CaseRole));

            int len = Enum.GetNames(typeof(CaseRole)).Length;

            for (int i = 0; i < Enum.GetNames(typeof(CaseRole)).Length; i++)
            {
                //Enum.GetValues(CaseRole)
                if (Associatedactions.ContainsKey((CaseRole)i))
                {
                    List<VerbFrame> ListofVF = Associatedactions[(CaseRole)i];
                    int count = ListofVF.Count;
                    if (count != 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            Weight += caseRoleWeights[(CaseRole)i];
                        }
                    }
                }
            }

            return Weight;
        }

        public override double WeighVerbFrame(int verbFrameIndex)
        {
            List<double> NFWeights = GetNounFrameWeights();

            double weight = 0;

            foreach (CaseRole cr in this._mindMapTMR.VerbFrames[verbFrameIndex].CaseRoles.Keys)
            {
                List<NounFrame> nfs = this._mindMapTMR.VerbFrames[verbFrameIndex].CaseRoles[cr];
                foreach (NounFrame NF in nfs)
                {
                    weight += NFWeights[this._mindMapTMR.Nounframes.IndexOf(NF)] * caseRoleFactors[cr];
                }
            }

            foreach (TemporalRelationType trt in this._mindMapTMR.VerbFrames[verbFrameIndex].TemporalRelations.Keys)
            {
                List<VerbFrame> vfs = this._mindMapTMR.VerbFrames[verbFrameIndex].TemporalRelations[trt];
                foreach (VerbFrame VF in vfs)
                {
                    //weight += temporalRelationWeights[trt];
                    weight += VFWeights[this._mindMapTMR.VerbFrames.IndexOf(VF)] * temporalRelationFactors[trt];
                }
            }

            foreach (DomainRelationType drt in this._mindMapTMR.VerbFrames[verbFrameIndex].DomainRelations.Keys)
            {
                List<VerbFrame> vfs = this._mindMapTMR.VerbFrames[verbFrameIndex].DomainRelations[drt];
                foreach (VerbFrame VF in vfs)
                {
                    //weight += domainRelationWeights[drt];
                    weight += VFWeights[this._mindMapTMR.VerbFrames.IndexOf(VF)] * domainRelationFactors[drt];
                }
            }
            return weight;

        }

        public override List<double> Weights_NounFrame()
        {
            int Len = _mindMapTMR.Nounframes.Count;
            List<double> NounFrameWeights = new List<double>(Len);
            for (int i = 0; i < Len; i++)
            {
                NounFrameWeights.Add(WeighNounFrame(i));
            }
            return NounFrameWeights;
        }

        protected List<double> VFWeights;
        public override List<double> Weights_VerbFrame()
        {
            VFWeights = new List<double>();
            VFWeights = LoadInitialVounFrameWeights();
            double avgError;
            double threshold = 0;
            do
            {
                List<double> oldVFWeights = new List<double>(VFWeights);
                VFWeights = GetVFWeights();
                avgError = CalcAvgError(oldVFWeights, VFWeights);
            } while (avgError > threshold);
            return VFWeights;
        }
        List<double> IterativeError = new List<double>();

        private double CalcAvgError(List<double> oldVFWeights, List<double> VFWeights)
        {
            List<double> difference = new List<double>();
            double DiffSum = 0;
            for (int i = 0; i < oldVFWeights.Count; i++)
            {
                DiffSum += VFWeights[i] - oldVFWeights[i];
            }
            double error = DiffSum / oldVFWeights.Count;
            IterativeError.Add(error);
            return error;
        }


        public List<double> GetVFWeights()
        {
            int Len = _mindMapTMR.VerbFrames.Count;
            List<double> VerbFrameWeights = new List<double>(Len);
            for (int i = 0; i < Len; i++)
            {
                VerbFrameWeights.Add(WeighVerbFrame(i));
            }
            return VerbFrameWeights;
        }

        private List<double> LoadInitialVounFrameWeights()
        {
            int Len = _mindMapTMR.VerbFrames.Count;
            List<double> InitialVFWeights = new List<double>(Len);
            for (int i = 0; i < Len; i++)
            {
                InitialVFWeights.Add(0);
            }
            return InitialVFWeights;
        }
    }
}
