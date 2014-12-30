using System;
using System.Collections.Generic;
using System.Text;
using mmTMR;

namespace MindMapMeaningRepresentation
{
    /// <summary>
    /// a class that calculate weights based on relation type
    /// </summary>
    public abstract class WeightAssigner
    {
        protected MindMapTMR _mindMapTMR;

        public MindMapTMR MindMapTMR
        {
            get { return _mindMapTMR; }
        }
        public WeightAssigner(MindMapTMR mindMaapTMR)
        {
            _mindMapTMR = mindMaapTMR;
            //LoadRelationWeights();
        }

        public abstract double WeighNounFrame(int nounFrameIndex);
        public abstract double WeighVerbFrame(int verbFrameIndex);

        public abstract List<double> Weights_NounFrame();
        public abstract List<double> Weights_VerbFrame();


        //common data
        protected Dictionary<CaseRole, double> caseRoleWeights = new Dictionary<CaseRole, double>();
        protected Dictionary<TemporalRelationType, double> temporalRelationWeights = new Dictionary<TemporalRelationType, double>();
        protected Dictionary<DomainRelationType, double> domainRelationWeights = new Dictionary<DomainRelationType, double>();

        protected void LoadRelationWeights() //here is where we alter the weights
        {
            caseRoleWeights.Clear();
            domainRelationWeights.Clear();
            temporalRelationWeights.Clear();

            #region CaseRole Weights

            caseRoleWeights.Add(CaseRole.Accompanier, 70);
            caseRoleWeights.Add(CaseRole.Action, 60);
            caseRoleWeights.Add(CaseRole.Agent, 100);
            caseRoleWeights.Add(CaseRole.among, 40);
            caseRoleWeights.Add(CaseRole.Beneficiary, 90);
            caseRoleWeights.Add(CaseRole.Cotheme, 80);
            caseRoleWeights.Add(CaseRole.Destination, 50);
            caseRoleWeights.Add(CaseRole.example, 60);
            caseRoleWeights.Add(CaseRole.Instrument, 60);
            caseRoleWeights.Add(CaseRole.Theme, 90);
            caseRoleWeights.Add(CaseRole.Source, 50);
            caseRoleWeights.Add(CaseRole.Path, 100);
            caseRoleWeights.Add(CaseRole.location, 20);
            caseRoleWeights.Add(CaseRole.time, 20);
            caseRoleWeights.Add(CaseRole.purpose, 60);
            caseRoleWeights.Add(CaseRole.reason, 60);
            caseRoleWeights.Add(CaseRole.OwnerOf, 50);
            caseRoleWeights.Add(CaseRole.Possession, 50);
            caseRoleWeights.Add(CaseRole.unknown, 50);
            caseRoleWeights.Add(CaseRole.use, 50);
            caseRoleWeights.Add(CaseRole.focus, 50);
            caseRoleWeights.Add(CaseRole.Means, 50);
            caseRoleWeights.Add(CaseRole.Under, 50);

            #endregion
            #region Temporal Relation Weights
            temporalRelationWeights.Add(TemporalRelationType.After, 30);
            temporalRelationWeights.Add(TemporalRelationType.Before, 30);
            temporalRelationWeights.Add(TemporalRelationType.Concurrent, 30);
            #endregion
            #region Domain Relation Weights
            domainRelationWeights.Add(DomainRelationType.ExpectedResult, 50);
            domainRelationWeights.Add(DomainRelationType.UnExpectedResult, 40);
            domainRelationWeights.Add(DomainRelationType.Reason, 60);
            domainRelationWeights.Add(DomainRelationType.How, 50);
            domainRelationWeights.Add(DomainRelationType.place, 20);
            #endregion
        }

        

        public List<double> GetNounFrameWeights()
        {
            return Weights_NounFrame();
        }

        public List<double> GetVerbFrameWeights()
        {
            return Weights_VerbFrame();
        }
    }
}
