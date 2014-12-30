using System;
using System.Collections.Generic;
using System.Text;
using mmTMR;

namespace MindMapMeaningRepresentation
{
    public class DirectRelationBasedTMRWeighter1 : WeightAssigner
    {
        public DirectRelationBasedTMRWeighter1(MindMapTMR mindMapTMR):base(mindMapTMR)
        {
            LoadRelationWeights();
        }

        public override double WeighNounFrame(int nounFrameIndex)
        {
            NounFrame NF = _mindMapTMR.Nounframes[nounFrameIndex];
            //relations of noun frames ---> CaseRoles
            Dictionary<CaseRole, List<VerbFrame>> Associatedactions = MindMapTMR.GetNounFrameAssociatedactions(nounFrameIndex);
            double Weight = 0;

            Array caseRoleArr = Enum.GetValues(typeof(CaseRole));

            for (int i = 0; i < Enum.GetNames(typeof(CaseRole)).Length; i++)
            {
                //Enum.GetValues(CaseRole)
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

            return Weight;
        }

        public override double WeighVerbFrame(int verbFrameIndex)
        {
            VerbFrame VF = _mindMapTMR.VerbFrames[verbFrameIndex];
            //relations of verb frames ---> CaseRoles, Domain Relations, Temporal Relations
            //CaseRoles stored in VF.CaseRoles (dictionary)
            //Temporal Relations stored in VF.TemporalRelations (dictionary)
            //Domain Relations stored in VF.DomainRelations (dictionary)
            double Weight = 0;

            for (int i = 0; i < Enum.GetNames(typeof(CaseRole)).Length; i++)//CaseRoles
            {
                List<NounFrame> ListofNF = VF.CaseRoles[(CaseRole)i];
                int count = ListofNF.Count;
                if (count != 0)
                {
                    for (int j = 0; j < count; j++)
                    {
                        Weight += caseRoleWeights[(CaseRole)i];
                    }
                }
            }
            for (int i = 0; i < Enum.GetNames(typeof(DomainRelationType)).Length; i++)//Domain Relations
            {
                List<VerbFrame> ListofVF = VF.DomainRelations[(DomainRelationType)i];
                int count = ListofVF.Count;
                if (count != 0)
                {
                    for (int j = 0; j < count; j++)
                    {
                        Weight += domainRelationWeights[(DomainRelationType)i];
                    }
                }
            }
            for (int i = 0; i < Enum.GetNames(typeof(TemporalRelationType)).Length; i++)//Temporal Relations
            {
                List<VerbFrame> ListofVF = VF.TemporalRelations[(TemporalRelationType)i];
                int count = ListofVF.Count;
                if (count != 0)
                {
                    for (int j = 0; j < count; j++)
                    {
                        Weight += temporalRelationWeights[(TemporalRelationType)i];
                    }
                }
            }

            return Weight;
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

        public override List<double> Weights_VerbFrame()
        {
            int Len = _mindMapTMR.VerbFrames.Count;
            List<double> VerbFrameWeights = new List<double>(Len);
            for (int i = 0; i < Len; i++)
            {
                VerbFrameWeights.Add(WeighVerbFrame(i));
            }
            return VerbFrameWeights;
        }
    }
}
