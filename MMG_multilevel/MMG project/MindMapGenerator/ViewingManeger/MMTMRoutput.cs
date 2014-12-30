using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using System.Windows;
using System.Windows.Forms;
using mmTMR;
using System.Drawing;
//using System.Collections.Generic;
using SyntacticAnalyzer;
using ViewingManeger;


namespace MindMapViewingManagement 
{
    public class MMTMRoutput:DrawingManager 
    {
        
        public MindMapTMR _TMR;
        NewGoogleSearch GImSearch { get; set; }
        Dictionary<VerbFrame, TMRVerbFrameEntity> _dicVerbFrame= new Dictionary<VerbFrame,TMRVerbFrameEntity>();
        Dictionary<NounFrame, TMRNounFrameEntity> _dicNounFrame= new Dictionary<NounFrame,TMRNounFrameEntity>();
        public MMTMRoutput(Control cntrl, MindMapTMR TMR, NewGoogleSearch gImSearch)
            : base(cntrl)
        {
            GImSearch = gImSearch;
            _TMR = TMR;
            AddEntities();    
            //4each 3al noun frames w n create entities w n defha w 2a3redha 
            //verb frames 
        }
        void AddEntities()
        {
            foreach (NounFrame NF in _TMR.Nounframes)
            {
                TMRNounFrameEntity nfe = new TMRNounFrameEntity(0, 0, NF,GImSearch);
                this.Add(nfe);
                _dicNounFrame.Add(NF, nfe);
            }
            foreach (NounFrame NF in _TMR.Nounframes)
            {
                TMRNounFrameEntity NF_entity = _dicNounFrame[NF];
                if (NF.Adjective != null)
                {
                    for (int i = 0; i < NF.Adjective.Count; i++)
                    {
                        ParseNode adj = NF.Adjective[i];
                        MM_RectangleWithText AdjEntity = new MM_RectangleWithText(0, 0, 30, 25, adj.Text, "");
                        Add(AdjEntity);
                        
                        Add(new MM_LineWithText(NF_entity, AdjEntity, NF.Adjective_fillerType[i]==""?"Adj":NF.Adjective_fillerType[i]));
                        
                    }
                }

            }
            foreach (VerbFrame VF in _TMR.VerbFrames)
            {
                TMRVerbFrameEntity VF_entity = new TMRVerbFrameEntity(0, 0, VF);
                Add(VF_entity);
                _dicVerbFrame.Add(VF, VF_entity);

                foreach (CaseRole cr in VF.CaseRoles.Keys)
                {
                    List<NounFrame> nfs = VF.CaseRoles[cr];
                    foreach (NounFrame NF in nfs)
                    {
                        TMRNounFrameEntity nfe = _dicNounFrame[NF];
                        Add(new MM_LineWithText(VF_entity, nfe, cr.ToString()));
                    }
                }
               
            }
            foreach (VerbFrame VF in _TMR.VerbFrames)
            {
                TMRVerbFrameEntity VF_entity = _dicVerbFrame[VF];
                foreach (DomainRelationType drt in VF.DomainRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.DomainRelations[drt];
                    foreach (VerbFrame vf in vfl)
                    {
                        TMRVerbFrameEntity vfe = _dicVerbFrame[vf];
                        Add(new MM_LineWithText(VF_entity, vfe, drt.ToString()));
                    }


                }

                foreach (DomainRelationType drt in VF.DomainRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.DomainRelations_n[drt];
                    foreach (NounFrame nf in nfl)
                    {
                        TMRNounFrameEntity nfe = _dicNounFrame[nf];
                        Add(new MM_LineWithText(VF_entity, nfe, drt.ToString()));
                    }


                }
                
                foreach (TemporalRelationType trt in VF.TemporalRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.TemporalRelations[trt];
                    foreach (VerbFrame vf in vfl)
                    {
                        TMRVerbFrameEntity vfe = _dicVerbFrame[vf];
                        Add(new MM_LineWithText(VF_entity, vfe, trt.ToString()));
                    }


                }
                foreach (TemporalRelationType trt in VF.TemporalRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.TemporalRelations_n[trt];
                    foreach (NounFrame nf in nfl)
                    {
                        TMRNounFrameEntity nfe = _dicNounFrame[nf];
                        Add(new MM_LineWithText(VF_entity, nfe, trt.ToString()));
                    }


                }
                //aspects
                if (VF.Aspect.Duration.ActionTime != null)
                {
                    string time = VF.Aspect.Duration.ActionTime;
                    TMRVerbFrameEntity vfe = _dicVerbFrame[VF];
                    MM_RectangleWithText timeEntity = new MM_RectangleWithText(0, 0, 30, 25, time,"");
                    Add(timeEntity);
                    Add(new MM_LineWithText(VF_entity, timeEntity, "time"));
                }
            }
         
         
            
               

        }
        

    }
}
