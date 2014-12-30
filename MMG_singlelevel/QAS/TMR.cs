using System;
using Wnlib;
using System.Collections;
using System.Text;
using WSD_TypeLib;
using OntoSem;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace QAS
{
	public enum status
	{
		HasOccured,
		HasNotOccured,
		NotAvailable,
	}
	public enum Cor
	{
		For,
		And,
		Or,
		Nor,
		But,
		Yet,
		So
	}
	[Serializable]
	public class Frame
	{
		public Concept Predicate;
		public ArrayList Reason=new ArrayList();
		public ArrayList ExpectedResult;
		public ArrayList UnExpectedResult;
		public ArrayList Before=new ArrayList();
		public ArrayList Concurrent=new ArrayList();
		public ArrayList After=new ArrayList();
		public status st;
		public ArrayList CorFrames = new ArrayList();
		public bool Positive;
		public bool IsConcept=false;
		public bool IsPRP = false;
		public ParseNode PRP;
		#region ICloneable Members

		public Frame Copy()
		{
			// TODO:  Add Frame.Clone implementation
			Frame f=new Frame();
			f.Predicate=this.Predicate.Copy();
			f.Reason=this.Reason;
			f.ExpectedResult=this.ExpectedResult;
			f.UnExpectedResult=this.UnExpectedResult;
			f.Before=this.Before;
			f.Concurrent=this.Concurrent;
			f.After=this.After;
			f.st=this.st;
			f.CorFrames=this.CorFrames;
			f.Positive=this.Positive;
			f.IsConcept=this.IsConcept;
			f.PRP=this.PRP;
			f.IsPRP=this.IsPRP;
			return f;
		}

		#endregion
	}

	public class CorFrame
	{
		public CorFrame(ArrayList Frames ,Cor c)
		{
			C = c;
			F = Frames;
		}
		public Cor C;
		public ArrayList F;
	}
	public class TMR
	{
		public Hashtable PrepositionFiller;
		public Hashtable WordToConcept;
		public Hashtable ADJAndADVFiller;
		ParseNode PRDSNode;
		public Hashtable Concepts = new Hashtable();
		Stack Frames=new Stack();
		Frame CurrFrame;
		ArrayList CurrFrames=new ArrayList();
		ArrayList CurrentSubject=new ArrayList();
		Stack Subjects=new Stack();
		public ArrayList MainFrames=new ArrayList();
		public ArrayList CurrNoun=new ArrayList();
		Stack Nouns=new Stack();
		public Ontology Onto;
		bool Continue = true;
		ArrayList Discourse=new ArrayList();
		ArrayList ParseTrees=new ArrayList();
		bool IsCsFrames=false;
		ArrayList CSFrames=new ArrayList();
		public bool IsPassive = false;
		ArrayList OutputFrs = new ArrayList();

		public string Output(string tabstrr)
		{
			string Out = "";
			foreach(Frame F in MainFrames)
			{
				OutputFrs.Add(1);
				OutputFrs.Add(F);
				OutputFrame(3,F,"");
				strinout.Add("--------------------------------------------------------------------------------------");
			}
			return Out;
		}

		public string OutputFrame(int Depth,Frame  F,string tabstr)
		{
			string Out ="";
			if(Depth == 0)
			{
				strinout.Add( tabstr+ "Concept Name: "+F.Predicate.Name+" ("+F.Predicate.Id.ToLower().Replace("_"," ")+")");
				return Out;
			}
			else
			{
				if(!F.IsConcept)
					strinout.Add( tabstr+ "Pedicate Name: "+F.Predicate.Name+" ("+F.Predicate.Id.ToLower().Replace("_"," ")+")");
				else
					strinout.Add( tabstr+ "Concept Name: "+F.Predicate.Name+" ("+F.Predicate.Id.ToLower().Replace("_"," ")+")");
				foreach(Property P in F.Predicate.FullProperties)
				{
					if(P.IsModified)
					{
						strinout.Add(tabstr+P.Name);
						strinout.Add(tabstr+OutputFiller(Depth,F,P.Fillers[0],tabstr));		
					}
				}
				return Out;
			}
		}

		public string OutputFiller(int Depth,Frame f,Filler  F,string tabstrr)
		{
			Depth -- ;
			string O = "";
			if(F.IsScalar)
			{
				O += tabstrr+F.ScalarFiller+"\n";
				
				if(F.Property.Name == "CARDINALITY" && int.Parse( (string)F.ScalarFiller)>1)
				{
					tabstrr += "\t";
					if(F.Frames == null )
					{
						//System.Windows.Forms.MessageBox.Show("Empty cardinality");
						return O;
					}
					else
					{
						/*Frame FR = new Frame();
						foreach(Concept d in F.Frames)
						{
							FR.Predicate = d;
							FR.IsConcept = true;
							O += tabstrr+OutputFrame(4,FR,tabstrr)+"\n";
						}*/
						return O;
					}
				}
				else
					return O;
			}
			else
			{
				tabstrr += "\t";
				foreach(Frame d in F.Frames)
						O += tabstrr+OutputFrame(Depth,d,tabstrr)+"\n";
				return O;
			}			
		}


		public ArrayList strinout= new ArrayList();

		public void Init(ArrayList ps,ArrayList Words,ArrayList ds,bool show)
		{
			ParseTrees=ps;
			Discourse=ds;
			LoadADVAndADJFillers();
			LoadPrepositionFillers();
			LoadWordToConcept();
			foreach(ParseTree tree in ParseTrees)
			{
				DisabiguteTree(tree,tree.Root);
				CurrFrame = null;
				CurrFrames.Clear();
				CurrNoun = new ArrayList();
				CurrentSubject = new ArrayList();
			}
			if(show)
			{
				TMROutput t = new TMROutput();
				this.Output("");
				t.textBox1.Lines = (string [])strinout.ToArray(typeof(string));
				t.textBox1.Select(0,0);
				t.Show();
			}
		}


		public void LoadPrepositionFillers()
		{
			this.PrepositionFiller = new Hashtable();
			ArrayList ar = new ArrayList();
			ar.Add(false);//Subject
			ar.Add("PART-OF-OBJECT");
			this.PrepositionFiller.Add("OF", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("DURATION");
			this.PrepositionFiller.Add("DURING", ar);

			ar = new ArrayList();
			ar.Add(true);
			ar.Add("PATH");
			this.PrepositionFiller.Add("THROUGHOUT", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("DESTINATION");
			this.PrepositionFiller.Add("TO", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("SOURCE");
			this.PrepositionFiller.Add("FROM", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("INSTRUMENT");
			this.PrepositionFiller.Add("THROUGH", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("DESTINATION");
			ar.Add(true);//Predecate
			ar.Add("LOCATION");
			this.PrepositionFiller.Add("INTO", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("LOCATION");
			this.PrepositionFiller.Add("IN", ar);

			ar = new ArrayList();
			ar.Add(true);//Predecate
			ar.Add("REASON");
			this.PrepositionFiller.Add("DUE_TO", ar);
		}

		public Concept GetNPPConcept(ParseTree tree,ParseNode node)
		{
			string word = WSD.GetWordString(tree,node);
			word.ToUpper();
			switch(word)
			{
				case"I":
					return (Concept)Onto["HUMAN"].Copy();
				case"HE":
					Concept C = (Concept)Onto["HUMAN"].Copy();
					C.Properties["GENDER"].Fillers[0] = new Filler(C.Properties["GENDER"],Modifier.VALUE,"MALE",null,true);
					return C;
				case"SHE":
					C = (Concept)Onto["HUMAN"].Copy();
					C.Properties["GENDER"].Fillers[0] = new Filler(C.Properties["GENDER"],Modifier.VALUE,"FEMALE",null,true);
					return C;
				case"IT":
				case"ITS":
					return (Concept)Onto["OBJECT"].Copy();
				case"YOU":
					return (Concept)Onto["HUMAN"].Copy();
				case"WE":
					return (Concept)Onto["HUMAN"].Copy();
				case"THEIR":
				case"THEY":
					return (Concept)Onto["HUMAN"].Copy();
				default:
					return null;
			}
            
           
		}

		public void LoadADVAndADJFillers()
		{
			this.ADJAndADVFiller = new Hashtable();

			this.ADJAndADVFiller.Add("SIZE", new PRPERTYVALUE("VOLUME",""));

			//this.ADJAndADVFiller.Add("CLENCHED", new PRPERTYVALUE("VOLUME","<0.3"));

			this.ADJAndADVFiller.Add("STRONG", new PRPERTYVALUE("STRENGTH", ">0.5"));

			this.ADJAndADVFiller.Add("POWERFUL", new PRPERTYVALUE("STRENGTH", ">0.7"));

			this.ADJAndADVFiller.Add("LIFETIME", new PRPERTYVALUE("AGE", "<> 0 130"));

			this.ADJAndADVFiller.Add("FOUR", new PRPERTYVALUE("CARDINALITY", "4"));

			this.ADJAndADVFiller.Add("TWO", new PRPERTYVALUE("CARDINALITY", "2"));

			this.ADJAndADVFiller.Add("UPPER", new PRPERTYVALUE("LOCATION", "UPPER PART"));

			this.ADJAndADVFiller.Add("LOWER", new PRPERTYVALUE("LOCATION", "LOWER PART"));

			this.ADJAndADVFiller.Add("POOR", new PRPERTYVALUE("STATE-OF-REPAIR", "<0.2"));

			this.ADJAndADVFiller.Add("RICH", new PRPERTYVALUE("STATE-OF-REPAIR", ">0.8"));

			this.ADJAndADVFiller.Add("BOTTOM", new PRPERTYVALUE("LOCATION", "LOWER PART"));

			this.ADJAndADVFiller.Add("LEFT", new PRPERTYVALUE("LOCATION", "LEFT PART"));

			this.ADJAndADVFiller.Add("RIGHT", new PRPERTYVALUE("LOCATION", "RIGHT PART"));

			this.ADJAndADVFiller.Add("INCREASED", new PRPERTYVALUE("STRENGTH", ">0.5"));

			this.ADJAndADVFiller.Add("DECREASED", new PRPERTYVALUE("STRENGTH", "<0.5"));
		}
		public class PRPERTYVALUE
		{
			public PRPERTYVALUE(string prop, string _value)
			{
				this.PropertyName = prop;
				this.Value = _value;
			}
			public string PropertyName;
			public string Value;
		}
		public Concept GetConcept(string word)
		{
			return ((Concept)((Concept)this.WordToConcept[word.ToUpper()]).Copy());
		}
		public PRPERTYVALUE GetADModification(string word)
		{
			try
			{
				return (PRPERTYVALUE)this.ADJAndADVFiller[word.ToUpper()];
			}
			catch
			{
				return null;
			}
		}
		public ArrayList GetPRSModification(string word)
		{
			return (ArrayList)this.PrepositionFiller[word.ToUpper()];
		}
		public void LoadWordToConcept()
		{
			this.WordToConcept = new Hashtable();
			this.WordToConcept.Add("HEART", Onto["HEART"]);
			this.WordToConcept.Add("BE", Onto["BE"]);
			this.WordToConcept.Add("ORGAN", Onto["MUSCLE-ORGAN"]);
			this.WordToConcept.Add("CIRCULATORY_SYSTEM", Onto["ORGAN-SYSTEM"]);
			this.WordToConcept.Add("PUMP", Onto["PUMP-EVENT"]);
			this.WordToConcept.Add("PUMPS", Onto["PUMP-EVENT"]);
			this.WordToConcept.Add("PUMP-V", Onto["PUMP-EVENT"]);
			this.WordToConcept.Add("PUMPS-V", Onto["PUMP-EVENT"]);
			this.WordToConcept.Add("PUMPED", Onto["PUMP-EVENT"]);
			this.WordToConcept.Add("BLOOD", Onto["BLOOD"]);
			this.WordToConcept.Add("BODY", Onto["BODY"]);
			this.WordToConcept.Add("SIZE", Onto["SIZE"]);
			Concept c = Onto["HAND"];
			c.FullProperties["VOLUME"].Fillers[0] = new Filler(c.FullProperties["VOLUME"],Modifier.UNKNOWN,"<0.3",null,true);
			c.FullProperties["VOLUME"].IsModified=true;
			this.WordToConcept.Add("CLENCHED_FIST",c);
			this.WordToConcept.Add("CARDIAC_MUSCLE", Onto["CARDIAC-MUSCLE-TISSUE"]);
			this.WordToConcept.Add("CONSTITUTE", Onto["FORM-EVENT"]);
			this.WordToConcept.Add("LIFETIME", Onto["AGE"]);
			this.WordToConcept.Add("CONSTITUTES", Onto["FORM-EVENT"]);
			this.WordToConcept.Add("CONTRACT", Onto["CONTRACTION-EVENT"]);
			this.WordToConcept.Add("CONTRACTS", Onto["CONTRACTION-EVENT"]);
			this.WordToConcept.Add("CONSTRICTS", Onto["CONTRACTION-EVENT"]);
			this.WordToConcept.Add("RELAXES", Onto["RELAX-MUSCLE"]);
			this.WordToConcept.Add("RELAX", Onto["RELAX-MUSCLE"]);
			this.WordToConcept.Add("PERSON", Onto["HUMAN"]);
			this.WordToConcept.Add("A_PERSON", Onto["HUMAN"]);
			this.WordToConcept.Add("CHAMBER", Onto["ORGAN-CHAMBER"]);
			this.WordToConcept.Add("CHAMBERS", Onto["ORGAN-CHAMBER"]);
			this.WordToConcept.Add("COLLECT", Onto["COLLECT-EVENT"]);
			this.WordToConcept.Add("COLLECTS", Onto["COLLECT-EVENT"]);
			this.WordToConcept.Add("COME", Onto["COME"]);
			this.WordToConcept.Add("COME_TO", Onto["COME"]);
			this.WordToConcept.Add("COMES", Onto["COME"]);
			this.WordToConcept.Add("COMING", Onto["COME"]);
			this.WordToConcept.Add("COMING_TO", Onto["COME"]);
			this.WordToConcept.Add("ATRIUM", Onto["CARDIAC-MUSCLE-OF-ATRIUM"]);
			this.WordToConcept.Add("ATRIUMS", Onto["CARDIAC-MUSCLE-OF-ATRIUM"]);
			this.WordToConcept.Add("DELIVER", Onto["DELIVER"]);
			this.WordToConcept.Add("DELIVERS", Onto["DELIVER"]);
			this.WordToConcept.Add("CONTRACTION", Onto["CONTRACTION-EVENT"]);
			this.WordToConcept.Add("RELAXATION", Onto["RELAXATION-EVENT"]);
			this.WordToConcept.Add("CONTRACTIONS", Onto["CONTRACTION-EVENT"]);
			this.WordToConcept.Add("RECEIVE", Onto["TAKE"]);
			this.WordToConcept.Add("RECEIVES", Onto["TAKE"]);
			this.WordToConcept.Add("REGION", Onto["REGION"]);
			this.WordToConcept.Add("REGIONS", Onto["REGION"]);
			this.WordToConcept.Add("LUNG", Onto["LUNG"]);
			this.WordToConcept.Add("LUNGS", Onto["LUNG"]);
			this.WordToConcept.Add("OXYGEN", Onto["OXYGEN"]);
			this.WordToConcept.Add("ABSORB", Onto["ABSORB"]);
			this.WordToConcept.Add("ABSORBED", Onto["ABSORB"]);
			this.WordToConcept.Add("ABSORBING", Onto["ABSORB"]);
			this.WordToConcept.Add("SYSTOLE", new Concept(Onto, "CONTRACT-MUSCLE"));
			this.WordToConcept.Add("DIASTOLE", new Concept(Onto, "RELAX-MUSCLE"));
			this.WordToConcept.Add("VENTRICLE", Onto["CARDIAC-MUSCLE-OF-VENTRICLE"]);
			this.WordToConcept.Add("VENTRICLES", Onto["CARDIAC-MUSCLE-OF-VENTRICLE"]);
			this.WordToConcept.Add("LEFT_VENTRICLE", Onto["CARDIAC-MUSCLE-OF-VENTRICLE"]);
			this.WordToConcept.Add("RIGHT_VENTRICLE", Onto["CARDIAC-MUSCLE-OF-VENTRICLE"]);
			this.WordToConcept.Add("FORCE", Onto["MOVE-AWAY"]);
			this.WordToConcept.Add("FORCING", Onto["MOVE-AWAY"]);
			this.WordToConcept.Add("FORCES", Onto["MOVE-AWAY"]);
			this.WordToConcept.Add("ARTERIES", Onto["ARTERY"]);
			this.WordToConcept.Add("ATRIA", Onto["CARDIAC-MUSCLE-OF-ATRIUM"]);
			this.WordToConcept.Add("EMPTIES", Onto["REMOVE"]);
			this.WordToConcept.Add("AORTA", Onto["TUNICA-INTIMA-OF-AORTA"]);
			this.WordToConcept.Add("PULMONARY_ARTERY", Onto["ARTERY"]);
			this.WordToConcept.Add("SYSTOLIC_PRESSURE", Onto["SYSTOLIC-PRESSURE"]);
			this.WordToConcept.Add("PRESSURE", Onto["PRESSURE"]);
			this.WordToConcept.Add("LET", Onto["PRODUCE"]);
			this.WordToConcept.Add("ROOM", Onto["SPACE"]);
			this.WordToConcept.Add("ACCEPT", Onto["TAKE"]);
			this.WordToConcept.Add("ACCEPTS", Onto["TAKE"]);
			this.WordToConcept.Add("DIASTOLIC_PRESSURE", Onto["DIASTOLIC-PRESSURE"]);
			this.WordToConcept.Add("MAKE", Onto["PRODUCE"]);

		}

		public Frame SetPRS(ParseTree tree,ParseNode node,ArrayList CurrNoun)
		{
			string word  = WSD.GetWordString(tree,(ParseNode)node.Children[0]);
			ArrayList Modification = GetPRSModification(word);
			ArrayList arr = DisabiguteTree(tree,(ParseNode)node.Children[1]);
			if(Modification==null)
				return null;
			for(int i = 0;i<Modification.Count ; i++)
			{
				if(word== "DURING")
				{
					i++;
					string prop = (string)Modification[i];
					foreach(Frame f in CurrFrames)
					{
						f.Predicate.FullProperties[prop].Fillers[0] = new Filler(f.Predicate.FullProperties[prop],Modifier.UNKNOWN,"<> 0 130",null,true);
						f.Predicate.FullProperties[prop].IsModified=true;
					}
				}
				else if(word== "DUE_TO")
				{
					if(CurrFrame!=null)
						CurrFrame.Reason.AddRange(arr);
				}
				else
				{
					bool b = (bool)Modification[i];
					i++;
					string prop = (string)Modification[i];
					if(b)
					{
						if(!IsCsFrames)
						{
							foreach(Frame f in CurrFrames)
							{
								f.Predicate.FullProperties[prop].Fillers[0] = new Filler(f.Predicate.Properties[prop],Modifier.UNKNOWN,null,arr,false);
								f.Predicate.FullProperties[prop].IsModified=true;
							}
						}
						else
						{
							foreach(Frame f in CSFrames)
							{
								f.Predicate.FullProperties[prop].Fillers[0] = new Filler(f.Predicate.Properties[prop],Modifier.UNKNOWN,null,arr,false);
								f.Predicate.FullProperties[prop].IsModified=true;
							}
						}
					}
					else
					{
						if(CurrentSubject.Count!=0)
						{
							foreach(Frame f in CurrentSubject)
							{
								Property p=f.Predicate.FullProperties[prop];
								if(p!=null)
								{
									p.Fillers[0] = new Filler(f.Predicate.Properties[prop],Modifier.UNKNOWN,null,arr,false);	
									p.IsModified=true;
								}
							}
							foreach(Concept f in CurrNoun)
							{
								Property p=f.FullProperties[prop];
								if(p!=null)
								{
									p.Fillers[0] = new Filler(f.FullProperties[prop],Modifier.UNKNOWN,null,arr,false);	
									p.IsModified=true;
								}
							}
						}
						else
						{
							foreach(Concept f in CurrNoun)
							{
								Property p=f.FullProperties[prop];
								if(p!=null)
								{
									p.Fillers[0] = new Filler(f.FullProperties[prop],Modifier.UNKNOWN,null,arr,false);	
									p.IsModified=true;
								}
							}
						}
					}
				}
			}
			return null;
		}

		public Frame SetProperty(ParseTree tree,ParseNode node,Concept CurrNoun)
		{
			string word  = WSD.GetWordString(tree,node);
			PRPERTYVALUE Modification = GetADModification(word);
			if(Modification==null)
				return null;
			string prop = (string)Modification.PropertyName;
			Concept f= CurrNoun;
			Property p=f.FullProperties[prop];
			if(p!=null)
			{
				if(!p.IsModified)
				{
					p.Fillers[0] = new Filler(p,Modifier.UNKNOWN,Modification.Value,null,true);	
					p.IsModified=true;
				}
				else
				{
					Concept NewCon = Onto[CurrNoun.Name].Copy();
					Property Newp=NewCon.FullProperties[prop];
					Newp.Fillers[0] = new Filler(Newp,Modifier.UNKNOWN,Modification.Value,null,true);	
					Newp.IsModified=true;
					Concepts.Add(NewCon.Name+"#1",NewCon);
					this.CurrNoun[0]=NewCon;
				}
			}
			return null;
		}

		public Cor GetCor(ParseTree tree,ParseNode node)
		{
			string word = (string)WSD.GetWordString(tree,node);
			switch(word.ToUpper())
			{
				case"AND":
					return Cor.And;
				case"BUT":
					return Cor.But;
				case"FOR":
					return Cor.For;
				case"NOR":
					return Cor.Nor;
				case"OR":
					return Cor.Or;
				case"SO":
					return Cor.So;
				case"YET":
					return Cor.Yet;
				default:
					throw new Exception("Invalid Cor string");
			}
		}
		#region Get_PRDS

		public ArrayList GetPRDS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="PRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="PRDS1")
				return GetPRDS1(tree,n);
			else if(n.Goal=="PRDS2")
				return GetPRDS2(tree,n);
			else
				return null;
		}
		
		public ArrayList GetPRDS1(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="PRD"&&n3.Goal=="PRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PRD"&&n3.Goal=="PRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			
			else if(n.Goal=="PRDS2"&&n3.Goal=="PRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetPRDS2(tree,n);
				temp1= GetPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PRDS2"&&n3.Goal=="PRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetPRDS2(tree,n);
				temp1= GetPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));			
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PRD"&&n3.Goal=="PRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= GetPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}

		public ArrayList GetPRDS2(ParseTree tree,ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
			{
				ArrayList TempFrames1 = GetPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.Or;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
			{
				ArrayList TempFrames1 = GetPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));				
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "NTH")
			{
				ArrayList TempFrames1 = GetPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
				{
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
					f.Positive=false;
				}
				foreach(Frame f in TempFrames2)
				{
					f.CorFrames.Add(new CorFrame(TempFrames1,c));
					f.Positive=false;
				}
				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else 
				return null;
		}

		public ArrayList GetPRDS3(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="PRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="PRDS4")
				return GetPRDS4(tree,n);
			else
				return null;
		}
		public ArrayList GetPRDS4(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="PRD"&&n3.Goal=="PRDS4")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetPRDS4(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PRD"&&n3.Goal=="PRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}
		#endregion

		#region GET_IPRDS

		public ArrayList GetIPRDS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="IPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="IPRDS1")
				return GetIPRDS1(tree,n);
			else if(n.Goal=="IPRDS2")
				return GetIPRDS2(tree,n);
			else
				return null;
		}
		
		public ArrayList GetIPRDS1(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="IPRD"&&n3.Goal=="IPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetIPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="IPRD"&&n3.Goal=="IPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			
			else if(n.Goal=="IPRDS2"&&n3.Goal=="IPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetIPRDS2(tree,n);
				temp1= GetIPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="IPRDS2"&&n3.Goal=="IPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetIPRDS2(tree,n);
				temp1= GetIPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));			
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="IPRD"&&n3.Goal=="IPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= GetIPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}

		public ArrayList GetIPRDS2(ParseTree tree,ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
			{
				ArrayList TempFrames1 = GetIPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetIPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.Or;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
			{
				ArrayList TempFrames1 = GetIPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetIPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));				
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "NTH")
			{
				ArrayList TempFrames1 = GetIPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetIPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
				{
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
					f.Positive=false;
				}
				foreach(Frame f in TempFrames2)
				{
					f.CorFrames.Add(new CorFrame(TempFrames1,c));
					f.Positive=false;
				}
				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else 
				return null;
		}

		public ArrayList GetIPRDS3(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="IPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="IPRDS4")
				return GetIPRDS4(tree,n);
			else
				return null;
		}
		public ArrayList GetIPRDS4(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="IPRD"&&n3.Goal=="IPRDS4")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetIPRDS4(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="IPRD"&&n3.Goal=="IPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}
		#endregion

		#region Get_GPRDS

		public ArrayList GetGPRDS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="GPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="GPRDS1")
				return GetGPRDS1(tree,n);
			else if(n.Goal=="GPRDS2")
				return GetGPRDS2(tree,n);
			else
				return null;
		}
		
		public ArrayList GetGPRDS1(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="GPRD"&&n3.Goal=="GPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetGPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="GPRD"&&n3.Goal=="GPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			
			else if(n.Goal=="GPRDS2"&&n3.Goal=="GPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetGPRDS2(tree,n);
				temp1= GetGPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="GPRDS2"&&n3.Goal=="GPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetGPRDS2(tree,n);
				temp1= GetGPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));			
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="GPRD"&&n3.Goal=="GPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= GetGPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}

		public ArrayList GetGPRDS2(ParseTree tree,ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
			{
				ArrayList TempFrames1 = GetGPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetGPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.Or;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
			{
				ArrayList TempFrames1 = GetGPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetGPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));				
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "NTH")
			{
				ArrayList TempFrames1 = GetGPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetGPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
				{
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
					f.Positive=false;
				}
				foreach(Frame f in TempFrames2)
				{
					f.CorFrames.Add(new CorFrame(TempFrames1,c));
					f.Positive=false;
				}
				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else 
				return null;
		}

		public ArrayList GetGPRDS3(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="GPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="GPRDS4")
				return GetGPRDS4(tree,n);
			else
				return null;
		}
		public ArrayList GetGPRDS4(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="GPRD"&&n3.Goal=="GPRDS4")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetGPRDS4(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="GPRD"&&n3.Goal=="GPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}
		#endregion

		#region Get_SPRDS

		public ArrayList GetSPRDS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="SPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="SPRDS1")
				return GetSPRDS1(tree,n);
			else if(n.Goal=="SPRDS2")
				return GetSPRDS2(tree,n);
			else
				return null;
		}
		
		public ArrayList GetSPRDS1(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="SPRD"&&n3.Goal=="SPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetSPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="SPRD"&&n3.Goal=="SPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			
			else if(n.Goal=="SPRDS2"&&n3.Goal=="SPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetSPRDS2(tree,n);
				temp1= GetSPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="SPRDS2"&&n3.Goal=="SPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetSPRDS2(tree,n);
				temp1= GetSPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));			
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="SPRD"&&n3.Goal=="SPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= GetSPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}

		public ArrayList GetSPRDS2(ParseTree tree,ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
			{
				ArrayList TempFrames1 = GetSPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetSPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.Or;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
			{
				ArrayList TempFrames1 = GetSPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetSPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));				
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "NTH")
			{
				ArrayList TempFrames1 = GetSPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetSPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
				{
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
					f.Positive=false;
				}
				foreach(Frame f in TempFrames2)
				{
					f.CorFrames.Add(new CorFrame(TempFrames1,c));
					f.Positive=false;
				}
				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else 
				return null;
		}

		public ArrayList GetSPRDS3(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="SPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="SPRDS4")
				return GetSPRDS4(tree,n);
			else
				return null;
		}
		public ArrayList GetSPRDS4(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="SPRD"&&n3.Goal=="SPRDS4")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetSPRDS4(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="SPRD"&&n3.Goal=="SPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}
		#endregion

		#region Get_PPRDS

		public ArrayList GetPPRDS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="PPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="PPRDS1")
				return GetPPRDS1(tree,n);
			else if(n.Goal=="PPRDS2")
				return GetPPRDS2(tree,n);
			else
				return null;
		}
		
		public ArrayList GetPPRDS1(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="PPRD"&&n3.Goal=="PPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetPPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PPRD"&&n3.Goal=="PPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			
			else if(n.Goal=="PPRDS2"&&n3.Goal=="PPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetPPRDS2(tree,n);
				temp1= GetPPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PPRDS2"&&n3.Goal=="PPRDS1")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= GetPPRDS2(tree,n);
				temp1= GetPPRDS1(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));			
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PPRD"&&n3.Goal=="PPRDS2")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= GetPPRDS2(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}

		public ArrayList GetPPRDS2(ParseTree tree,ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
			{
				ArrayList TempFrames1 = GetPPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.Or;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
			{
				ArrayList TempFrames1 = GetPPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
					f.CorFrames.Add(new CorFrame(TempFrames2,c));				
				foreach(Frame f in TempFrames2)
					f.CorFrames.Add(new CorFrame(TempFrames1,c));

				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else if(((ParseNode)node.Children[0]).Goal == "NTH")
			{
				ArrayList TempFrames1 = GetPPRDS3(tree,(ParseNode)node.Children[1]);
				ArrayList TempFrames2 = GetPPRDS3(tree,(ParseNode)node.Children[3]);
				Cor c = Cor.And;
				foreach(Frame f in TempFrames1)
				{
					f.CorFrames.Add(new CorFrame(TempFrames2,c));
					f.Positive=false;
				}
				foreach(Frame f in TempFrames2)
				{
					f.CorFrames.Add(new CorFrame(TempFrames1,c));
					f.Positive=false;
				}
				ArrayList arr=new ArrayList();
				arr.AddRange(TempFrames1);
				arr.AddRange(TempFrames2);
				return arr;
			}
			else 
				return null;
		}

		public ArrayList GetPPRDS3(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="PPRD")
				return DisabiguteTree(tree,n);
			else if(n.Goal=="PPRDS4")
				return GetPPRDS4(tree,n);
			else
				return null;
		}
		public ArrayList GetPPRDS4(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			ParseNode n2=(ParseNode)node.Children[1];
			ParseNode n3=(ParseNode)node.Children[2];
			if(n.Goal=="PPRD"&&n3.Goal=="PPRDS4")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1=GetPPRDS4(tree,n3);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,((CorFrame)((Frame)temp1[0]).CorFrames[((Frame)temp1[0]).CorFrames.Count-1]).C));
				temp1.AddRange(temp);
				return temp1;
			}
			else if(n.Goal=="PPRD"&&n3.Goal=="PPRD")
			{
				ArrayList temp=new ArrayList();
				ArrayList temp1=new ArrayList();
				temp= DisabiguteTree(tree,n);
				temp1= DisabiguteTree(tree,n3);
				Cor c=GetCor(tree,n2);
				foreach(Frame f in temp)
					f.CorFrames.Add(new CorFrame(temp1,c));
				foreach(Frame f in temp1)
					f.CorFrames.Add(new CorFrame(temp,c));
				temp1.AddRange(temp);
				return temp1;
			}
			else
				return null;
		}
		#endregion

		public ArrayList GetSubjects(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(n.Goal == "SSBJ")
				return DisabiguteTree(tree,n);
			else
				return null;
		}
		public ArrayList GetObjects(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(n.Goal == "SOBJ")
				return DisabiguteTree(tree,n);
			else
				return null;
		}
		public ArrayList GetBADJS(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(node.Children.Count==1)
			{
				if(n.Goal == "BADJ")
					return DisabiguteTree(tree,n);
				else
					return null;
			}
			else if(node.Children.Count==2)
			{
				ParseNode n1 =(ParseNode) node.Children[1];
				ArrayList a=new ArrayList();
				a=DisabiguteTree(tree,n);
				if(a==null)
					a=new ArrayList();
				GetBADJS(tree,n1);
				return null;
			}	
			else
				return null;
		}

		public ArrayList GetCMPAVS(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(node.Children.Count==1)
			{
				if(n.Goal == "CMPAV")
					return DisabiguteTree(tree,n);
				else
					return null;
			}
			else
			{
				return null;
			}/*
			else if(node.Children.Count==2)
			{
				ParseNode n1 =(ParseNode) node.Children[1];
				ArrayList a=new ArrayList();
				a=DisabiguteTree(tree,n);
				if(a==null)
					a=new ArrayList();
				DisabiguteTree(tree,n1);
				return null;
			}	
			else
				return null;*/
		}
		public ArrayList GetFADJS(ParseTree tree,ParseNode node)
		{
			ParseNode n = (ParseNode) node.Children[0];
			if(n.Goal == "FADJ")
				return DisabiguteTree(tree,n);
			else
				return null;
		}
		public ArrayList GetPRPS(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(n.Goal == "PRP")
				return DisabiguteTree(tree,n);
			else
				return null;
		}

		public ArrayList GetLPRPS(ParseTree tree,ParseNode node)
		{
			ArrayList Frames=new ArrayList();
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="PRP")
				return DisabiguteTree(tree,n);
			else
				return null;
		}

		public ArrayList RepresentSpecialVerbs(ParseTree tree,ParseNode node,string verb)
		{
			ParseNode n0=(ParseNode)node.Children[0];
			if(verb=="BE")
			{
				//all the cases in the text is predicate only
				Frame F=new Frame();
				F.Predicate=GetConcept(verb);
				F.Predicate.Id="is";
				CurrFrames.Add(F);
				if(CurrFrame!=null)
					Frames.Push(CurrFrame);
				CurrFrame=F;
				ArrayList a=new ArrayList();
				a.Add(F);
				if(node.Children.Count==2)
					DisabiguteTree(tree,(ParseNode)node.Children[1]);
				return a;
			}
			else if(verb=="HAVE")
			{
				//the only case now is to make it PartOF
				if(node.Children.Count==1)
				{
					Frame F=new Frame();
					F.Predicate=GetConcept(verb);
					F.Predicate.Id="has";
					CurrFrames.Add(F);
					if(CurrFrame!=null)
						Frames.Push(CurrFrame);
					CurrFrame=F;
					ArrayList a=new ArrayList();
					a.Add(F);
					return a;
				}
				else if(n0.Goal=="V")
				{
					ArrayList Objs=DisabiguteTree(tree,(ParseNode)node.Children[1]);
					foreach(Frame f in CurrentSubject)
					{
						Property p= f.Predicate.FullProperties["HAS-OBJECT-AS-PART"];
						if(p==null)
							return null;
						p.Fillers[0]=new Filler(p,Modifier.SEM,null,Objs,false);
						p.IsModified=true;
					}
					foreach(Frame f in Objs)
					{
						if(f.IsConcept)
						{
							Property p=f.Predicate.FullProperties["PART-OF-OBJECT"];
							if(p==null)
								return null;
							p.Fillers[0]=new Filler(p,Modifier.SEM,null,CurrentSubject,false);
							p.IsModified=true;
						}
					}
					return null;
				}
				else
					return null;
			}
			else
				return null;
		}
		
		public ArrayList GetADVS(ParseTree tree,ParseNode node)
		{
			ParseNode n =(ParseNode) node.Children[0];
			if(node.Children.Count==1)
			{
				if(n.Goal == "ADV")
					return DisabiguteTree(tree,n);
				else
					return null;
			}
			else if(node.Children.Count==2)
			{
				ParseNode n1 =(ParseNode) node.Children[1];
				ArrayList a=new ArrayList();
				a=DisabiguteTree(tree,n);
				if(a==null)
					a=new ArrayList();
				GetBADJS(tree,n1);
				return null;
			}	
			else
				return null;
		}

		public void RepresentADVS(ParseTree tree,ParseNode node)
		{
			GetADVS(tree,node);
		}

		public ArrayList DisabiguteTree(ParseTree tree,ParseNode node)
		{
			switch(node.Goal)
			{
				case"S":
				{
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CS")
							CSFrames=DisabiguteTree(tree,n);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="ABPH")
						{
							ArrayList TempFrames=new ArrayList();
							TempFrames=DisabiguteTree(tree,n);
							foreach(Frame f in CSFrames)
							{
								f.Before.AddRange(TempFrames);
								f.Reason.AddRange(TempFrames);
							}
							foreach(Frame f in TempFrames)
							{
								f.After.AddRange(CSFrames);
								f.st = status.HasOccured;
							}
							MainFrames.AddRange(TempFrames);
						}
						else if(n.Goal=="PRPHS")
						{
							ArrayList PRPHSFrames=new ArrayList();
							PRPHSFrames=DisabiguteTree(tree,n);
							foreach(Frame f in PRPHSFrames)
							{
								f.ExpectedResult.AddRange(CSFrames);
								f.After.AddRange(CSFrames);
								f.st=((Frame)CSFrames[0]).st;
							}
							foreach(Frame f in CSFrames)
							{
								f.Reason.AddRange(PRPHSFrames);
								f.Before.AddRange(PRPHSFrames);
							}
							MainFrames.AddRange(PRPHSFrames);
						}
						else if(n.Goal=="ADVS")
						{
							IsCsFrames=true;
							RepresentADVS(tree,n);
							IsCsFrames=false;
						}
						else if(n.Goal=="NPF")
						{
							ArrayList TempFrames=new ArrayList();
							TempFrames=DisabiguteTree(tree,n);
							foreach(Frame f in CSFrames)
							{
								f.Before.AddRange(TempFrames);
								f.Reason.AddRange(TempFrames);
							}
							foreach(Frame f in TempFrames)
							{
								f.After.AddRange(CSFrames);
								f.st = status.HasOccured;
							}
							MainFrames.AddRange(TempFrames);
						}
					MainFrames.AddRange(CSFrames);

				}break;
				case"CS":
				{
					if(node.Children.Count==1)
						return DisabiguteTree(tree,(ParseNode)node.Children[0]);
					else
					{
						if(((ParseNode)node.Children[0]).Goal == "DS" ||((ParseNode)node.Children[0]).Goal == "IMS")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[0]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[2]);
							Cor c = GetCor(tree,(ParseNode)node.Children[1]);
							foreach(Frame f in TempFrames1)
							{
								if(((ParseNode)node.Children[0]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames2,c));
								if(c==Cor.But)
									f.UnExpectedResult.AddRange(TempFrames2);
							}
							foreach(Frame f in TempFrames2)
							{
								if(((ParseNode)node.Children[0]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames1,c));
							}
							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal == "ETH" || ((ParseNode)node.Children[0]).Goal == "WTH")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[1]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[3]);
							Cor c = Cor.Or;
							foreach(Frame f in TempFrames1)
							{
								if(((ParseNode)node.Children[1]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames2,c));
							}
							foreach(Frame f in TempFrames2)
							{
								if(((ParseNode)node.Children[1]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames1,c));
							}
							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal == "BTH" || ((ParseNode)node.Children[0]).Goal == "NOTN")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[1]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[3]);
							Cor c = Cor.And;
							foreach(Frame f in TempFrames1)
							{
								if(((ParseNode)node.Children[1]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames2,c));
							}
							foreach(Frame f in TempFrames2)
							{
								if(((ParseNode)node.Children[1]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames1,c));
							}
							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal == "NTH")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[1]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[3]);
							Cor c = Cor.And;
							foreach(Frame f in TempFrames1)
							{
								if(((ParseNode)node.Children[1]).Goal != "DS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames2,c));
								f.Positive=false;
							}
							foreach(Frame f in TempFrames2)
							{
								if(((ParseNode)node.Children[1]).Goal == "IMS")
									f.st= status.HasNotOccured;
								f.CorFrames.Add(new CorFrame(TempFrames1,c));
								f.Positive=false;
							}
							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal=="THE")
						{
							ArrayList TempFrames1 = new ArrayList();
							ArrayList TempFrames2 = new ArrayList();
							foreach(ParseNode n in node.Children)
							{
								if(n.Goal=="DS")
									TempFrames1=DisabiguteTree(tree,n);
								if(n.Goal=="DS"&&TempFrames2.Count!=0)
									TempFrames2=DisabiguteTree(tree,n);
							}
							foreach(Frame f in TempFrames1)
								f.ExpectedResult.AddRange(TempFrames2);
							foreach(Frame f in TempFrames2)
								f.Reason.AddRange(TempFrames1);

							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal=="SS")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[0]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[2]);
							switch((WSD.GetWordString(tree,(ParseNode)node.Children[1]).ToLower()))
							{
								case"before":
									foreach(Frame f in TempFrames1)
										f.After.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Before.AddRange(TempFrames1);
									break;
								case"after":
									foreach(Frame f in TempFrames2)
										f.After.AddRange(TempFrames1);
									foreach(Frame f in TempFrames1)
										f.Before.AddRange(TempFrames2);
									break;
								case"as soon as":
								case"if only":
								case"if":
								case"in case":
								case"now that":
								case"inasmuch":
								case"inasmuch as":
								case"whenever":
								case"as":
								case"because":
								case"providing":
								case"providing that":
								case"provided":
								case"provided that":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"when":
								{
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
								}break;
								case"while":
								case"whereas":
								case"since":
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;	
								case"till":
								case"until":
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Positive=!f.Positive;
									}
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"even though":
								case"although":
									foreach(Frame f in TempFrames2)
										f.UnExpectedResult.AddRange(TempFrames1);
									break;
								case"as if":
									foreach(Frame f in TempFrames2)
										f.st=status.HasNotOccured;
									break;
								case"as long as":
								{
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Concurrent.AddRange(TempFrames2);
									}
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Concurrent.AddRange(TempFrames1);
									}
								}break;
								case"in order that":
								case"so that":
									// from the ontology
									
									break;
								case"lest":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=!f.Positive;
									}
									// ontology
									break;
								case"unless":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=false;
									}
									break;
								case"where":
									foreach(Frame f in TempFrames1)
										f.Predicate.Properties["LOCATION"].Fillers[0]=new Filler(f.Predicate.Properties["LOCATION"],Modifier.SEM,null,TempFrames2,false);
									break;
								case"whether":
									foreach(Frame f in TempFrames2)
										f.st=status.NotAvailable;
									break;
								default:
								{
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;
								}
							}

						}
						else if(((ParseNode)node.Children[0]).Goal=="SOR")
						{
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[1]);
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[3]);
//							Cor c = Cor.And;
//							foreach(Frame f in TempFrames1)
//							{
//								if(((ParseNode)node.Children[1]).Goal != "DS")
//									f.st= status.HasNotOccured;
//								f.CorFrames.Add(new CorFrame(TempFrames2,c));
//								f.Positive=false;
//							}
//							foreach(Frame f in TempFrames2)
//							{
//								if(((ParseNode)node.Children[1]).Goal == "IMS")
//									f.st= status.HasNotOccured;
//								f.CorFrames.Add(new CorFrame(TempFrames1,c));
//								f.Positive=false;
//							}
							switch((WSD.GetWordString(tree,(ParseNode)node.Children[1]).ToLower()))
							{
								case"before":
									foreach(Frame f in TempFrames1)
										f.After.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Before.AddRange(TempFrames1);
									break;
								case"after":
									foreach(Frame f in TempFrames2)
										f.After.AddRange(TempFrames1);
									foreach(Frame f in TempFrames1)
										f.Before.AddRange(TempFrames2);
									break;
								case"as soon as":
								case"if only":
								case"if":
								case"in case":
								case"now that":
								case"inasmuch":
								case"inasmuch as":
								case"whenever":
								case"as":
								case"because":
								case"providing":
								case"providing that":
								case"provided":
								case"provided that":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"when":
								{
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Before.AddRange(TempFrames2);
									}
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.After.AddRange(TempFrames1);
									}
								}break;
								case"while":
								case"whereas":
								case"since":
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;	
								case"till":
								case"until":
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Positive=!f.Positive;
									}
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"even though":
								case"although":
									foreach(Frame f in TempFrames2)
										f.UnExpectedResult.AddRange(TempFrames1);
									break;
								case"as if":
									foreach(Frame f in TempFrames2)
										f.st=status.HasNotOccured;
									break;
								case"as long as":
								{
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Concurrent.AddRange(TempFrames2);
									}
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Concurrent.AddRange(TempFrames1);
									}
								}break;
								case"in order that":
								case"so that":
									// from the ontology
									
									break;
								case"lest":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=!f.Positive;
									}
									// ontology
									break;
								case"unless":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=false;
									}
									break;
								case"where":
									foreach(Frame f in TempFrames1)
										f.Predicate.Properties["LOCATION"].Fillers[0]=new Filler(f.Predicate.Properties["LOCATION"],Modifier.SEM,null,TempFrames2,false);
									break;
								case"whether":
									foreach(Frame f in TempFrames2)
										f.st=status.NotAvailable;
									break;
								default:
								{
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;
								}
							}
							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal=="THE")
						{
							ArrayList TempFrames1 = new ArrayList();
							ArrayList TempFrames2 = new ArrayList();
							foreach(ParseNode n in node.Children)
							{
								if(n.Goal=="DS")
									TempFrames1=DisabiguteTree(tree,n);
								if(n.Goal=="DS"&&TempFrames2.Count!=0)
									TempFrames2=DisabiguteTree(tree,n);
							}
							foreach(Frame f in TempFrames1)
								f.ExpectedResult.AddRange(TempFrames2);
							foreach(Frame f in TempFrames2)
								f.Reason.AddRange(TempFrames1);

							ArrayList arr=new ArrayList();
							arr.AddRange(TempFrames1);
							arr.AddRange(TempFrames2);
							return arr;
						}
						else if(((ParseNode)node.Children[0]).Goal=="SS")
						{
							ArrayList TempFrames1 = DisabiguteTree(tree,(ParseNode)node.Children[0]);
							ArrayList TempFrames2 = DisabiguteTree(tree,(ParseNode)node.Children[2]);
							switch(WSD.GetWordString(tree,(ParseNode)node.Children[1]).ToLower())
							{
								case"before":
									foreach(Frame f in TempFrames1)
										f.After.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Before.AddRange(TempFrames1);
									break;
								case"after":
									foreach(Frame f in TempFrames2)
										f.After.AddRange(TempFrames1);
									foreach(Frame f in TempFrames1)
										f.Before.AddRange(TempFrames2);
									break;
								case"as soon as":
								case"if only":
								case"if":
								case"in case":
								case"now that":
								case"inasmuch":
								case"inasmuch as":
								case"whenever":
								case"as":
								case"because":
								case"providing":
								case"providing that":
								case"provided":
								case"provided that":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"when":
								{
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Before.AddRange(TempFrames2);
									}
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.After.AddRange(TempFrames1);
									}
								}break;
								case"while":
								case"whereas":
								case"since":
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;	
								case"till":
								case"until":
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Positive=!f.Positive;
									}
									foreach(Frame f in TempFrames2)
										f.ExpectedResult.AddRange(TempFrames1);
									break;
								case"even though":
								case"although":
									foreach(Frame f in TempFrames2)
										f.UnExpectedResult.AddRange(TempFrames1);
									break;
								case"as if":
									foreach(Frame f in TempFrames2)
										f.st=status.HasNotOccured;
									break;
								case"as long as":
									foreach(Frame f in TempFrames1)
									{
										f.Reason.AddRange(TempFrames2);
										f.Concurrent.AddRange(TempFrames2);
									}
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Concurrent.AddRange(TempFrames1);
									}
									break;
								case"in order that":
								case"so that":
									// from the ontology
									
									break;
								case"lest":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=!f.Positive;
									}
									// ontology
									break;
								case"unless":
									foreach(Frame f in TempFrames1)
										f.Reason.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
									{
										f.ExpectedResult.AddRange(TempFrames1);
										f.Positive=false;
									}
									break;
								case"where":
									foreach(Frame f in TempFrames1)
										f.Predicate.Properties["LOCATION"]=((Frame)TempFrames2[0]).Predicate.Properties["LOCATION"];
									break;
								case"whether":
									foreach(Frame f in TempFrames2)
										f.st=status.NotAvailable;
									break;
								default:
									foreach(Frame f in TempFrames1)
										f.Concurrent.AddRange(TempFrames2);
									foreach(Frame f in TempFrames2)
										f.Concurrent.AddRange(TempFrames1);
									break;
							}
						}

					}
				}break;
				case"SS":
				{
					return DisabiguteTree(tree,(ParseNode)node.Children[0]);
				}
				case"DS":
				{
					this.PRDSNode = (ParseNode)node.Children[1];
					ArrayList SbjW=GetSubjects(tree,(ParseNode)node.Children[0]);
					this.Subjects.Push(CurrentSubject);
					CurrentSubject=SbjW;
					if(this.Continue)
					{
						ArrayList PRDS=GetPRDS(tree,(ParseNode)node.Children[1]);
						if(PRDS==null)
						{
							this.Continue = true;
							CurrentSubject = (ArrayList)Subjects.Pop();
							return SbjW;
						}
						foreach(Frame f in PRDS)
							if(!f.IsConcept)
							{
								if(!IsPassive)
								{
									f.Predicate.FullProperties["AGENT"].Fillers[0]=new Filler(f.Predicate.Properties["AGENT"],Modifier.SEM,null,CurrentSubject,false);
									f.Predicate.FullProperties["AGENT"].IsModified=true;
								}
								else
								{
									IsPassive = false;
									f.Predicate.FullProperties["THEME"].Fillers[0]=new Filler(f.Predicate.Properties["THEME"],Modifier.SEM,null,CurrentSubject,false);
									f.Predicate.FullProperties["THEME"].IsModified=true;
								}
							}
						CurrentSubject = (ArrayList)Subjects.Pop();
						return PRDS;
					}
					else
					{
						this.Continue = true;
						CurrentSubject = (ArrayList)Subjects.Pop();
						return SbjW;
					}
					/*ArrayList PRDW=new ArrayList();
					foreach(ParseNode n in PRDS)
					{
						PRDW=Get_main_PRD(tree,n);
						if(PRDW.Count<2)
							DisabiguteWords(PRDW,SbjW);
						else
						{
							DisabiguteWords(PRDW);
							if(((WordSense)SbjW[0]).Node.Senses==null)
								DisabiguteWords(PRDW.GetRange(0,1),SbjW);
						}
					}
					foreach(ParseNode n in PRDS)
						DisabiguteTree(tree,n);
					DisabiguteTree(tree,(ParseNode)node.Children[0]);*/
				}
				case"ABPH":
				{
					/*ArrayList APHS=Get_main_ABPH(tree,node);
					if(!node.visited)
						DisabiguteWords(APHS);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="AVJ")
							DisabiguteWords(APHS.GetRange(0,1),Get_main_AVJ(tree,n));						
					if(((ParseNode)node.Children[0]).Goal=="NP")
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
					DisabiguteTree(tree,(ParseNode)node.Children[node.Children.Count-1]);*/
				}break;

				case"CMPS":
				{
					ArrayList arr=new ArrayList();
					foreach(ParseNode n in node.Children)
					{
						ArrayList a= DisabiguteTree(tree,n);
						if(a!=null)
							arr.AddRange(a);
					}
					return arr;
				}
				case"CMP":
				{
					ParseNode n= (ParseNode)node.Children[0];
					switch(n.Goal)
					{
						case"OBJ":
							ArrayList Obj=GetObjects(tree,n);
							string s= "THEME";
							if(CurrFrame!=null)
							{
								foreach(Frame f in CurrFrames)
								{
									Property p=f.Predicate.FullProperties[s];
									if(!p.IsModified)
									{
										p.IsModified=true;
										p.Fillers[0]=new Filler(p,Modifier.SEM,null,Obj,false);
									}
								}
								Property pro=CurrFrame.Predicate.FullProperties[s];
								if(!pro.IsModified)
								{
									pro.IsModified=true;
									pro.Fillers[0]=new Filler(pro,Modifier.SEM,null,Obj,false);
								}
								return null;
							}
							else
								return Obj;
						case"LPRPS":
							GetLPRPS(tree,n);
							return null;
						case"CMPAVJ":
							ParseNode nn = (ParseNode)n.Children[0];
							if(nn.Goal == "CMPAVS")
							{
								return GetCMPAVS(tree,nn);
							}
							else
							{}
							break;
						case"INFPH":
						{
							if(n.Children.Count==3)
							{
								Frame F=new Frame();
								ParseNode n1=(ParseNode)n.Children[1];
								ParseNode n2=(ParseNode)n.Children[2];
								string w=WSD.GetWordString(tree,n1);
								if(w.ToLower()=="pump" || w.ToLower()=="pumps")
									w+="-v";
								F.Predicate=GetConcept(w);
								F.Predicate.Id="to "+w;
								CurrFrames.Add(F);
								if(CurrFrame!=null)
									Frames.Push(CurrFrame);
								CurrFrame = F;
								DisabiguteTree(tree,n2);
								if(Frames.Count!=0)
									CurrFrame=(Frame)this.Frames.Pop();
								Property p= CurrFrame.Predicate.FullProperties["PURPOSE"];
								if(p!=null)
								{
									ArrayList arr=new ArrayList();
									arr.Add(F);
									p.Fillers[0]=new Filler(p,Modifier.SEM,null,arr,false);
									p.IsModified=true;
								}
							}
						}break;
					}
				}break;
				case"CMPAVJ":break;
				case"SSBJ":
				{
					ParseNode n = (ParseNode)node.Children[0];
					switch(n.Goal)
					{
						case "NP":
							return DisabiguteTree(tree,n);

					}
				}break;
				case"SOBJ":
				{
					ParseNode n = (ParseNode)node.Children[0];
					switch(n.Goal)
					{
						case "NPC":
							return DisabiguteTree(tree,n);

					}
				}break;
				case"FADJ":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal=="PRPS")
						GetPRPS(tree,n);
					else if(n.Goal=="PRPH")
						return DisabiguteTree(tree,n);

				}
					break;
				case"ADV":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal=="PRP")
						return DisabiguteTree(tree,n);

				}break;
				case"IPRDS":
				case"GPRDS":
				case"SPRDS":
				case"PPRDS":
				case"PRPHS":
				case"CMPAVS":
				case"CMPAJS":
				case"LPRPS":
				case"ADVS":
				case"PRPS":
				case"OBJ":
				{
					return GetObjects(tree,node);
				}
				
				case"PRDS1":
				case"IPRDS1":
				case"GPRDS1":
				case"SPRDS1":
				case"PPRDS1":
				case"PRPHS1":
				case"BADJS1":
				case"FADJS1":
				case"CMPAVS1":
				case"CMPAJS1":
				case"LPRPS1":
				case"ADVS1":
				case"PRPS1":
				case"OBJ1":
				case"SBJ1":

				case"PRDS2":
				case"IPRDS2":
				case"GPRDS2":
				case"SPRDS2":
				case"PPRDS2":
				case"PRPHS2":
				case"BADJS2":
				case"FADJS2":
				case"CMPAVS2":
				case"CMPAJS2":
				case"LPRPS2":
				case"ADVS2":
				case"PRPS2":
				case"OBJ2":
				case"SBJ2":

				case"PRDS3":
				case"IPRDS3":
				case"GPRDS3":
				case"SPRDS3":
				case"PPRDS3":
				case"PRPHS3":
				case"BADJS3":
				case"FADJS3":
				case"CMPAVS3":
				case"CMPAJS3":
				case"LPRPS3":
				case"ADVS3":
				case"PRPS3":
				case"OBJ3":
				case"SBJ3":

				case"PRDS4":
				case"IPRDS4":
				case"GPRDS4":
				case"SPRDS4":
				case"PPRDS4":
				case"PRPHS4":
				case"BADJS4":
				case"FADJS4":
				case"CMPAVS4":
				case"CMPAJS4":
				case"LPRPS4":
				case"ADVS4":
				case"PRPS4":
				case"OBJ4":
				case"SBJ4":
				{
					foreach(ParseNode n in node.Children)
						if(n.Children!=null)
							DisabiguteTree(tree,n);
					break;
				}
				case"ADVC":
				case"PRP":
				{
					ArrayList arr=new ArrayList();
					arr.Add(SetPRS(tree,node,CurrNoun));
					return arr;
				}
				case"NC":
				{
					/*	ParseNode n=(ParseNode)node.Children[0];
						if(n.Goal=="SOR")
							DisabiguteTree(tree,(ParseNode)node.Children[1]);
						else
						{
							if(!node.visited)
							{
								ArrayList SbjW=Get_main_RPN(tree,(ParseNode)node.Children[0]);
								ArrayList PRDW=Get_main_PRD(tree,(ParseNode)node.Children[1]);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}
							DisabiguteTree(tree,(ParseNode)node.Children[0]);
							DisabiguteTree(tree,(ParseNode)node.Children[1]);
						}*/
				}break;
				case"ADJC":
				{
					/*	if(!node.visited)
						{
							ArrayList SbjW=Get_main_RPN(tree,(ParseNode)node.Children[0]);
							ParseNode n=(ParseNode)node.Children[1];
							if(n.Goal=="PRD")
							{
								ArrayList PRDW=Get_main_PRD(tree,n);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}
							else
							{
								ArrayList PRDW=Get_main_DS(tree,n);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}
						}
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
						DisabiguteTree(tree,(ParseNode)node.Children[1]);
						*/
				}break;
				case"RPN":
				{
					DisabiguteTree(tree,(ParseNode)node.Children[node.Children.Count-1]);
				}break;
				case"DRPN":
				{
					/*	if(node.Children.Count==1)
							return ;
						if(!node.visited)
						{
							ArrayList SbjW=Get_main_RPN(tree,(ParseNode)node.Children[0]);
							ParseNode n=(ParseNode)node.Children[1];
							ArrayList PRDW=Get_main_NP(tree,n);
							DisabiguteWords(PRDW,SbjW);
						}
						DisabiguteTree(tree,(ParseNode)node.Children[1]);
						*/
				}break;
				case"NNC":
				{
					ArrayList arr=new ArrayList();
					string word=WSD.GetWordString(tree,(ParseNode)node.Children[0]).ToUpper();
					Concept Con=GetConcept(word);
					Concept Instance;
					ArrayList a = new ArrayList();
					if(!Concepts.Contains(Con.Name))
					{
						Instance = (Concept)Con.Copy();
						Concepts.Add(Con.Name,Instance);
					}
					else
						Instance = (Concept)Concepts[Con.Name];Frame f=new Frame();
					f.Predicate=Con;
					f.IsConcept=true;
					arr.Add(f);
					return arr;

				}
				case"NN":
				{
					ParseNode n = (ParseNode)node.Children[0];
					switch(n.Goal)
					{
						case "N":
							Concept Noun=GetConcept(WSD.GetWordString(tree,n));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n,Noun);
							n.concept=Noun;
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
							/*string word  = WSD.GetWordString(tree,n);
							Concept Con=GetConcept(word);
							Concept Instance;
							ArrayList a = new ArrayList();
							if(!Concepts.Contains(Con.Name))
							{
								Instance = (Concept)Con.Copy();
								Concepts.Add(Con.Name,Instance);
							}
							else
								Instance = (Concept)Concepts[Con.Name];
							Frame f=new Frame();
							f.Predicate=Instance;
							f.IsConcept=true;
							a.Add(f);
							return a;*/

					}
				}break;
				case"PRPH":
				{
					if(node.Children.Count==2)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						if(n1.Goal=="VING"&&n2.Goal=="CMPS")
						{
							string word = WSD.GetWordString(tree,n1);
							WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo(word,true,true);
							word=((string)wordinfo.texts[0]).ToUpper();
							Frame F=new Frame();
							switch(word)
							{
								case"BE":
									return RepresentSpecialVerbs(tree,node,word);
								case"HAVE":
									return RepresentSpecialVerbs(tree,node,word);
								default:
									string w=WSD.GetWordString(tree,n1);
									if(w.ToLower()=="pump" || w.ToLower()=="pumps")
										w+="-v";
									F.Predicate=GetConcept(w);
									F.Predicate.Id=WSD.GetWordString(tree,n1);
									if(CurrFrame!=null)
										Frames.Push(CurrFrame);
									CurrFrame = F;
									break;
							}
							DisabiguteTree(tree,n2);
							if(Frames.Count!=0)
								CurrFrame=(Frame)Frames.Pop();
							ArrayList a=new ArrayList();
							a.Add(F);
							return a;
						}
					}

				}break;
				case"BADJ":
				{
					SetProperty(tree,(ParseNode)node.Children[0],(Concept)CurrNoun[0]);
					return null;
				}
				case"INFPH":
				{
					/*	if(node.Children.Count==2)
							return;
						ArrayList r=Get_main_INFPH(tree,node);
						ArrayList s=new ArrayList();
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS")
							{
								ArrayList Cmps=Parser.VerbSense.GetCMPList(n);
								foreach(ParseNode cmp in Cmps)
								{
									ParseNode nn=(ParseNode)cmp.Children[0];
									if(nn.Goal=="LPRPS")
										DisabiguteWords(r,Get_main_LPRPS(tree,nn));
									else if(nn.Goal=="CMPAVJ")
									{
										ParseNode ch=(ParseNode)nn.Children[0];
										if(ch.Goal=="CMPAVS")
											DisabiguteWords(r,Get_main_CMPAVS(tree,ch));
										else
											DisabiguteWords(r,Get_main_CMPAJS(tree,ch));
									}
									else if(nn.Goal=="OBJ")
										DisabiguteWords(r,Get_main_OBJ(tree,nn));
									else
										DisabiguteWords(r,Get_main_INFPH(tree,nn));
								}
							}
							else if(n.Goal=="ADVS")
							{
								s.AddRange(Get_main_ADVS(tree,n));
								DisabiguteWords(r,s);
							}
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS"||n.Goal=="ADVS")
								DisabiguteTree(tree,n);
								*/
				}break;
				case"INFPO":
				{
					/*	if(node.Children.Count==1)
							return;
						ArrayList r=Get_main_INFPH(tree,node);
						ArrayList s=new ArrayList();
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS")
							{
								ArrayList Cmps=Parser.VerbSense.GetCMPList(n);
								foreach(ParseNode cmp in Cmps)
								{
									ParseNode nn=(ParseNode)cmp.Children[0];
									if(nn.Goal=="LPRPS")
										DisabiguteWords(r,Get_main_LPRPS(tree,nn));
									else if(nn.Goal=="CMPAVJ")
									{
										ParseNode ch=(ParseNode)nn.Children[0];
										if(ch.Goal=="CMPAVS")
											DisabiguteWords(r,Get_main_CMPAVS(tree,ch));
										else
											DisabiguteWords(r,Get_main_CMPAJS(tree,ch));
									}
									else if(nn.Goal=="OBJ")
										DisabiguteWords(r,Get_main_OBJ(tree,nn));
									else
										DisabiguteWords(r,Get_main_INFPH(tree,nn));
								}
							}
							else if(n.Goal=="ADVS")
							{
								s.AddRange(Get_main_ADVS(tree,n));
								DisabiguteWords(r,s);
							}
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS"||n.Goal=="ADVS")
								DisabiguteTree(tree,n);
								*/
				}break;
				case"NPADJ":break;
				case"NPF":break;
				case"NPC":
				{
					if(node.Children.Count==4)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						ParseNode n3=(ParseNode)node.Children[2];
						ParseNode n4=(ParseNode)node.Children[3];
						if(n1.Goal=="ARC"&&n2.Goal=="AVJ"&&n3.Goal=="NNC"&&n4.Goal=="FAVJ")
						{
							Concept Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n3.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p!=null && p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n2);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							if(WSD.GetWordString(tree,n3)=="ORGAN")
								Noun.Id="MUSCLAR ORGAN";
							else
							{
								Noun.Id=SetId(tree,n3,Noun);
								n3.concept=Noun;
							}
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n4);
							if(Nouns.Count!=0)
								CurrNoun=(ArrayList)Nouns.Pop();
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
					}
					else if(node.Children.Count==3)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						ParseNode n3=(ParseNode)node.Children[2];
						Concept Noun;
						if(n1.Goal=="ARC"&&n2.Goal=="NNC"&&n3.Goal=="FAVJ")
						{
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n2.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n2,Noun);
							n2.concept=Noun;
							ArrayList arr = DisabiguteTree(tree,n3);
							if(arr!=null)
							{
								foreach(Frame fr in arr)
								{
									if(!fr.IsConcept)
									{
										Frame fram=new Frame();
										fram.Predicate=Noun;
										fram.IsConcept=true;
										ArrayList An=new ArrayList();
										An.Add(fram);
										fr.Predicate.FullProperties["AGENT"].Fillers[0]=new Filler(fr.Predicate.FullProperties["AGENT"],Modifier.SEM,null,An,false);
										fr.Predicate.FullProperties["AGENT"].IsModified=true;
										MainFrames.Add(fr);
									}

								}
							}
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							if(Nouns.Count!=0)
								CurrNoun=(ArrayList)Nouns.Pop();
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n1.Goal=="ARC"&&n2.Goal=="AVJ"&&n3.Goal=="NNC")
						{
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n3.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n2);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n3,Noun);
							n3.concept=Noun;
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							if(Nouns.Count > 0)
								CurrNoun = (ArrayList)Nouns.Pop();
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						
					}
					else if(node.Children.Count==2)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						if(n1.Goal=="ARC"&&n2.Goal=="NNC")
						{
							Concept Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n2.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n2,Noun);
							n2.concept=Noun;
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n1.Goal=="CNP"&&n2.Goal=="NNC")
						{
							ArrayList a = DisabiguteTree(tree,n2);
							string coname = WSD.GetWordString(tree,n1);
							string proname = WSD.GetWordString(tree,n2);
							coname = coname.Remove(coname.Length-2,2);
							Concept concC = GetConcept(coname); 
							Concept concP = GetConcept(proname);
							Property P = concC.FullProperties[concP.Name];
							P.IsModified=true;
							if(P != null)
							{
								ArrayList aa = new ArrayList();
								aa.Add(new Frame().Predicate = concC);
								return null;
							}
							else
								return null;
						}
						else if(n1.Goal=="AVJ"&&n2.Goal=="NNC")
						{
							Concept Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n2.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n1);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n2,Noun);
							n2.concept=Noun;
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							if(Nouns.Count > 0)
								CurrNoun = (ArrayList)Nouns.Pop();
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n1.Goal=="PPJ"&&n2.Goal=="NNC")
						{
							ArrayList DisRef=new ArrayList();
							bool b=false;
							foreach(ArrayList arr in Discourse)
							{
								foreach(DiscourseEntry de in arr)
								{
									if(de.Node==n1)
									{
										DisRef=arr;
										b=true;
										break;
									}
								}
								if(b)
									break;
							}
							Concept c=new Concept();
							b=false;
							foreach(DiscourseEntry de in DisRef)
							{
								if(!(de.Node.Goal=="NPP"||de.Node.Goal=="PPJ"||
									de.Node.Goal=="REPC"||de.Node.Goal=="DEMP"||
									de.Node.Goal=="OPP"||de.Node.Goal=="RXPN"||
									de.Node.Goal=="EACHO"||de.Node.Goal=="ONEAN")
									)
								{
									c=de.Node.concept;
									if(c==null)
										c=de.Node.Parent.concept;
									if(c==null)
										b=false;
									else
										b=true;
								}
							}
							if(!b)
								c=GetNPPConcept(tree,n1);
							Concept Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n2.Children[0]));
							Concept con=null;
							if(Concepts.Contains(Noun.Name))
							{
								con=(Concept)Concepts[Noun.Name];
								Property p= con.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();
									
									
									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									con=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							Property p1=c.FullProperties["HAS-OBJECT-AS-PART"];
							if(p1!=null)
							{
								ArrayList arr=new ArrayList();
								Frame fr=new Frame();
								fr.Predicate=Noun;
								fr.IsConcept=true;
								arr.Add(fr);
								p1.Fillers[0]=new Filler(p1,Modifier.SEM,null,arr,false);
								p1.IsModified=true;
							}
							Noun.Id=SetId(tree,n2,Noun);
							n2.concept=Noun;
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						return null;
					}
					else if(node.Children.Count==1)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						if(n1.Goal=="NNC")
						{
							Concept Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n1.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n1,Noun);
							n1.concept=Noun;
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n1.Goal=="OPP")
						{
							ArrayList DisRef=new ArrayList();
							bool b=false;
							foreach(ArrayList arr in Discourse)
							{
								foreach(DiscourseEntry de in arr)
								{
									if(de.Node==n1)
									{
										DisRef=arr;
										b=true;
										break;
									}
								}
								if(b)
									break;
							}
							Concept c=new Concept();
							foreach(DiscourseEntry de in DisRef)
							{
								if(!(de.Node.Goal=="NPP"||de.Node.Goal=="PPJ"||
									de.Node.Goal=="REPC"||de.Node.Goal=="DEMP"||
									de.Node.Goal=="OPP"||de.Node.Goal=="RXPN"||
									de.Node.Goal=="EACHO"||de.Node.Goal=="ONEAN"))
								{
									c=de.Node.concept;
									if(c==null)
										c=((ParseNode)de.Node.Children[0]).concept;
									if(c==null)
										b=false;
									else
										b=true;
									break;
								}
								else
									c=null;
							}
							if(c==null)
								c=GetNPPConcept(tree,n1);
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=c;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
					}

				}break;
				case"NP":
				{
					ParseNode n0 = (ParseNode)node.Children[0];
					if(node.Children.Count==1)
					{
						if(n0.Goal=="NPP")
						{
							ArrayList DisRef=new ArrayList();
							bool b=false;
							foreach(ArrayList arr in Discourse)
							{
								foreach(DiscourseEntry de in arr)
								{
									if(de.Node==n0)
									{
										DisRef=arr;
										b=true;
										break;
									}
								}
								if(b)
									break;
							}
							Concept c=null;
							foreach(DiscourseEntry de in DisRef)
							{
								if(!(de.Node.Goal=="NPP"||de.Node.Goal=="PPJ"||
									de.Node.Goal=="REPC"||de.Node.Goal=="DEMP"||
									de.Node.Goal=="OPP"||de.Node.Goal=="RXPN"||
									de.Node.Goal=="EACHO"||de.Node.Goal=="ONEAN"))
								{
									c=de.Node.concept;
									if(c==null)
										c=((ParseNode)de.Node.Children[0]).concept;
									if(c==null)
										b=false;
									else
										b=true;
									break;
								}
								else
									c=null;
							}
							if(c==null)
								c=GetNPPConcept(tree,n0);
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=c;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n0.Goal=="NN")
						{
							Concept Noun=new Concept();
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n0.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n0,Noun);
							n0.concept=Noun;
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
					}
					if(node.Children.Count==2)
					{
						if(n0.Goal=="AVJ")
						{
							Concept Noun=new Concept();
							ParseNode n1 = (ParseNode)node.Children[1];
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n1.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n0);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							Noun.Id=SetId(tree,n1,Noun);
							n1.concept=Noun;
							if(Nouns.Count > 0)
								CurrNoun = (ArrayList)Nouns.Pop();
							ArrayList a=new ArrayList();
							Frame f=new Frame();
							f.Predicate=Noun;
							f.IsConcept= true;
							a.Add(f);
							return a;
						}
						else if(n0.Goal=="ARC")
						{	
							ParseNode n1 = (ParseNode)node.Children[1];
							switch(n1.Goal)
							{
								case "NN":
									return DisabiguteTree(tree,n1);
							}						
						}
						else if(n0.Goal=="PPJ")
						{	
							ArrayList DisRef=new ArrayList();
							bool b=false;
							foreach(ArrayList arr in Discourse)
							{
								foreach(DiscourseEntry de in arr)
								{
									if(de.Node==n0)
									{
										DisRef=arr;
										b=true;
										break;
									}
								}
								if(b)
									break;
							}
							Concept c=null;
							ParseNode n1 = (ParseNode)node.Children[1];
							b=false;
							foreach(DiscourseEntry de in DisRef)
							{
								if(!(de.Node.Goal=="NPP"||de.Node.Goal=="PPJ"||
									de.Node.Goal=="REPC"||de.Node.Goal=="DEMP"||
									de.Node.Goal=="OPP"||de.Node.Goal=="RXPN"||
									de.Node.Goal=="EACHO"||de.Node.Goal=="ONEAN"))
								{
									c=de.Node.concept;
									if(c==null)
										c=((ParseNode)de.Node.Children[0]).concept;
									if(c==null)
										b=false;
									else
										b=true;
									break;
									
								}
							}
							if(!b)
								c=GetNPPConcept(tree,n0);
							ArrayList a;
							switch(n1.Goal)
							{
								case "NN":
								{
									a = DisabiguteTree(tree,n1);
									Property p = c.FullProperties[GetADModification(((Frame)a[0]).Predicate.Name).PropertyName];
									if(p != null)
									{
										ArrayList Pred = GetPRDS(tree,PRDSNode);
										if(((Frame)Pred[0]).Predicate.Name == "BE")
										{
											Filler f = ((Frame)Pred[0]).Predicate.FullProperties["THEME"].Fillers[0];
											Frame fr = (Frame)f.Frames[0];
											if(fr.IsConcept)
											{
												Property po= fr.Predicate.FullProperties[GetADModification(((Frame)a[0]).Predicate.Name).PropertyName];
												if(po!=null)
												{
													this.Continue = false;
													p.Fillers[0] = po.Fillers[0];
													p.IsModified=true;
													Frame F = new Frame();
													F.Predicate = c;
													F.IsConcept = true;
													ArrayList d  = new ArrayList();
													d.Add(F);
													return d;
												}
												else
												{
													this.Continue = false;
													p.Fillers[0] = new Filler(p,Modifier.UNKNOWN,null,f.Frames,false);
													Frame F = new Frame();
													F.Predicate = c;
													F.IsConcept = true;
													ArrayList d  = new ArrayList();
													d.Add(F);
													return d;
												}
											}
											else
											{
												/*Handle Non concept frame */
												return null;
											}
										}
										else
										{
											/*Wait*/
											return null;
										}
									}
									else
									{
										/*Create predicate Own in MainFrames And return a Frame of NN */
										return null;
									}
								}
								
							}

						}
					}
					if(node.Children.Count==3)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						ParseNode n3=(ParseNode)node.Children[2];
						Concept Noun=new Concept();
						if(n1.Goal=="ARC"&&n2.Goal=="AVJ"&&n3.Goal=="NN")
						{
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n3.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n2);
							if(c!=null)
							{
								bool b=true;
								foreach(Property p in Noun.FullProperties)
								{
									Query q=new Query();
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											Query q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							if(Nouns.Count!=0)
								CurrNoun=(ArrayList)Nouns.Pop();
						}
						Noun.Id=SetId(tree,n3,Noun);
						n3.concept=Noun;
						ArrayList a=new ArrayList();
						Frame f=new Frame();
						f.Predicate=Noun;
						f.IsConcept= true;
						a.Add(f);
						return a;
					}
					if(node.Children.Count==4)
					{
						ParseNode n1=(ParseNode)node.Children[0];
						ParseNode n2=(ParseNode)node.Children[1];
						ParseNode n3=(ParseNode)node.Children[2];
						ParseNode n4=(ParseNode)node.Children[3];
						Concept Noun=new Concept();
						if(n1.Goal=="ARC"&&n2.Goal=="AVJ"&&n3.Goal=="NN"&&n4.Goal=="FAVJ")
						{
							Noun=GetConcept(WSD.GetWordString(tree,(ParseNode)n3.Children[0]));
							Concept c=null;
							if(Concepts.Contains(Noun.Name))
							{
								c=(Concept)Concepts[Noun.Name];
								Property p= c.FullProperties["CARDINALITY"];
								if(p.IsModified)
								{
									Noun=((Concept)Concepts[Noun.Name]).Copy();

									/**/Property Pn = Noun.FullProperties["CARDINALITY"];
									/**/Pn.Fillers[0] = new Filler(Pn,Modifier.SEM,"1",null,true);
									/**/Pn.IsModified = false;


									if(p.Fillers[0].Frames==null)
										p.Fillers[0].Frames=new ArrayList();
									p.Fillers[0].Frames.Add(Noun);
									c=null;
								}
							}
							else
								Concepts.Add(Noun.Name,Noun);
							if(CurrNoun.Count!=0)
								Nouns.Push(CurrNoun);
							CurrNoun=new ArrayList();
							CurrNoun.Add(Noun);
							DisabiguteTree(tree,n2);
							if(c!=null)
							{
								bool b=true;
								Query q=new Query();
								foreach(Property p in Noun.FullProperties)
								{
									if(p.IsModified)
										if(!q.ChechForProperty(c.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
									Noun=c;
								else
								{
									if(c.FullProperties["SUB"]==null)
										c.FullProperties.Add("SUB",new Property(c,"SUB",null));
									bool ba=false;
									foreach(Filler fl in c.FullProperties["SUB"].Fillers)
									{
										ba=false;
										foreach(Property p in Noun.FullProperties)
										{
											ba=true;
											q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										c.FullProperties["SUB"].Fillers.Add(new Filler(c.FullProperties["SUB"],Modifier.SEM,Noun,null,false));
									if(Noun.FullProperties["SUPER"]==null)
										Noun.FullProperties.Add("SUPER",new Property(Noun,"SUPER",null));
									ba=false;
									foreach(Filler fl in Noun.FullProperties["SUPER"].Fillers)
									{
										ba=false;
										foreach(Property p in c.FullProperties)
										{
											ba=true;
											q=new Query();
											if(p.IsModified)
												if(!q.ChechForProperty(fl.ConceptFiller.FullProperties[p.Name],p))
												{
													ba=false;
													break;
												}
										}
										if(ba)
											break;
									}
									if(!ba)
										Noun.FullProperties["SUPER"].Fillers.Add(new Filler(Noun.FullProperties["SUPER"],Modifier.SEM,Noun,null,false));
									
								}
							}
							DisabiguteTree(tree,n4);
							if(Nouns.Count!=0)
								CurrNoun=(ArrayList)Nouns.Pop();
						}
						Noun.Id=SetId(tree,n3,Noun);
						n3.concept=Noun;
						ArrayList a=new ArrayList();
						Frame f=new Frame();
						f.Predicate=Noun;
						f.IsConcept= true;
						a.Add(f);
						return a;
					}
				}break;
				case"AVJ":
				{
					if(node.Children.Count==1)
						GetBADJS(tree,(ParseNode)node.Children[0]);
					else
						GetBADJS(tree,(ParseNode)node.Children[1]);
					return null;
				}
				case"CMPAJ":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal=="CMADJ")
						DisabiguteTree(tree,n);
					else if(n.Goal=="CPADJ")
					{
						SetProperty(tree,n,(Concept)CurrNoun[0]);
						return null;
					}
				}break;
				case"CMPAV":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal=="PADV")
					{
						PRPERTYVALUE Adv = GetADModification(WSD.GetWordString(tree,n));
						if(Adv != null)
						{
							foreach(Frame f in CurrFrames)
							{
								f.Predicate.Properties[Adv.PropertyName].Fillers[0] = new Filler(f.Predicate.Properties[Adv.PropertyName],Modifier.SEM,Adv.Value,null,true);
								f.Predicate.Properties[Adv.PropertyName].IsModified=true;
							}
						}
						return null;
					}
				}break;
				case"FAVJ":
				{
					if(node.Children.Count==1)
						return GetFADJS(tree,(ParseNode)node.Children[0]);
				}break;
				case"IVP":
				case"GVP":
				case"PVP":
				case"SVP":
				case"VP":
				{
					/*	ArrayList r=Get_main_VP(tree,node);
						ArrayList s=new ArrayList();
						foreach(ParseNode n in node.Children)
							if(n.Goal=="ADVS")
								s.AddRange(Get_main_ADVS(tree,n));
						DisabiguteWords(r,s);
						foreach(ParseNode n in node.Children)
							if(n.Goal=="ADVS")
								DisabiguteTree(tree,n);
								*/
				}break;
				case"IPRD":
				case"PPRD":
				case"GPRD":
				case"SPRD":
				case"PRD":
				{
					ParseNode n = (ParseNode)node.Children[0];
					switch(n.Goal)
					{
						case"VPSP":
						case"VINF":
						case "V":
						{
							string word = WSD.GetWordString(tree,n);
							WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo(word,true,true);
							word=((string)wordinfo.texts[0]).ToUpper();
							Frame F=new Frame();
							switch(word)
							{
								case"BE":
									return RepresentSpecialVerbs(tree,node,word);
								case"HAVE":
									return RepresentSpecialVerbs(tree,node,word);
								default:
									string w=WSD.GetWordString(tree,n);
									if(w.ToLower()=="pump" || w.ToLower()=="pumps")
										w+="-v";
									F.Predicate=GetConcept(w);
									F.Predicate.Id=WSD.GetWordString(tree,n);
									CurrFrames.Add(F);
									if(CurrFrame!=null)
										Frames.Push(CurrFrame);
									CurrFrame = F;
									break;
							}
							
							if(node.Children.Count>1)
							{
								ParseNode n1 = (ParseNode)node.Children[1];
								DisabiguteTree(tree,n1);
							}
							if(Frames.Count!=0)
								CurrFrame=(Frame)this.Frames.Pop();
							ArrayList a=new ArrayList();
							a.Add(F);
							return a;
						}
						case"PADV":
						{
							// the verb frame
							ParseNode n1=(ParseNode)node.Children[1];
							string word = WSD.GetWordString(tree,n1);
							WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo(word,true,true);
							word=((string)wordinfo.texts[0]).ToUpper();
							Frame F=new Frame();
							switch(word)
							{
								case"BE":
									return RepresentSpecialVerbs(tree,node,word);
								case"HAVE":
									return RepresentSpecialVerbs(tree,node,word);
								default:
									string w=WSD.GetWordString(tree,n1);
									if(w.ToLower()=="pump" || w.ToLower()=="pumps")
										w+="-V";
									F.Predicate=GetConcept(w);
									F.Predicate.Id=WSD.GetWordString(tree,n1);
									CurrFrames.Add(F);
									if(CurrFrame!=null)
										Frames.Push(CurrFrame);
									CurrFrame = F;
									break;
							}
							// the Adverb Modification
							string adv=WSD.GetWordString(tree,n);
							PRPERTYVALUE Adv=GetADModification(adv);
							if(Adv != null)
								F.Predicate.Properties[Adv.PropertyName].Fillers[0] = new Filler(F.Predicate.Properties[Adv.PropertyName],Modifier.SEM,Adv.Value,null,true);
							//
							if(node.Children.Count>2)
								DisabiguteTree(tree,(ParseNode)node.Children[2]);
							if(Frames.Count!=0)
								CurrFrame=(Frame)this.Frames.Pop();
							ArrayList a=new ArrayList();
							a.Add(F);
							return a;
						}
						case"IVP":
						case"PVP":
						case"GVP":
						case"SVP":
						case"VP":
						{
							if(n.Children.Count==2)
							{
								ParseNode n1=(ParseNode)n.Children[0];
								ParseNode n2=(ParseNode)n.Children[1];
								if(n1.Goal=="BE1"&&n2.Goal=="VPSP")
								{
									string word = WSD.GetWordString(tree,n2);
									WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo(word,true,true);
									word=((string)wordinfo.texts[0]).ToUpper();
									Frame F=new Frame();
									string w=WSD.GetWordString(tree,n2);
									if(w.ToLower()=="pump" || w.ToLower()=="pumps")
										w+="-V";
									F.Predicate=GetConcept(w);
									F.Predicate.Id=WSD.GetWordString(tree,n2);
									CurrFrames.Add(F);
									if(CurrFrame!=null)
										Frames.Push(CurrFrame);
									CurrFrame = F;
									IsPassive = true;
									if(node.Children.Count>1)
									{
										ParseNode n3 = (ParseNode)node.Children[1];
										DisabiguteTree(tree,n3);
									}
									if(Frames.Count!=0)
										CurrFrame=(Frame)this.Frames.Pop();
									ArrayList a=new ArrayList();
									a.Add(F);
									return a;
								}
							}
						}break;
					}
				}break;
				case"CMADJ":
				{
					/*	ArrayList r=Get_main_CMADJ(tree,node);
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS")
							{
								ArrayList Cmps=Parser.VerbSense.GetCMPList(n);
								foreach(ParseNode cmp in Cmps)
								{
									ParseNode nn=(ParseNode)cmp.Children[0];
									if(nn.Goal=="LPRPS")
										DisabiguteWords(r,Get_main_LPRPS(tree,nn));
									else if(nn.Goal=="CMPAVJ")
									{
										ParseNode ch=(ParseNode)nn.Children[0];
										if(ch.Goal=="CMPAVS")
											DisabiguteWords(r,Get_main_CMPAVS(tree,ch));
										else
											DisabiguteWords(r,Get_main_CMPAJS(tree,ch));
									}
									else if(nn.Goal=="OBJ")
										DisabiguteWords(r,Get_main_OBJ(tree,nn));
									else
										DisabiguteWords(r,Get_main_INFPH(tree,nn));
								}
							}
							else if(n.Goal=="PRD")
								DisabiguteWords(r,Get_main_PRD(tree,n));
							else if(n.Goal=="SBJ")
								DisabiguteWords(r,Get_main_SBJ(tree,n));
						foreach(ParseNode n in node.Children)
							if(n.Goal=="CMPS"||n.Goal=="PRD"||n.Goal=="SBJ"||n.Goal=="NPADJ")
								DisabiguteTree(tree,n);*/
				}break;
				case"CMADV":
				{
					/*					ArrayList r=Get_main_CMADV(tree,node);
										foreach(ParseNode n in node.Children)
											if(n.Goal=="CMPS")
											{
												ArrayList Cmps=Parser.VerbSense.GetCMPList(n);
												foreach(ParseNode cmp in Cmps)
												{
													ParseNode nn=(ParseNode)cmp.Children[0];
													if(nn.Goal=="LPRPS")
														DisabiguteWords(r,Get_main_LPRPS(tree,nn));
													else if(nn.Goal=="CMPAVJ")
													{
														ParseNode ch=(ParseNode)nn.Children[0];
														if(ch.Goal=="CMPAVS")
															DisabiguteWords(r,Get_main_CMPAVS(tree,ch));
														else
															DisabiguteWords(r,Get_main_CMPAJS(tree,ch));
													}
													else if(nn.Goal=="OBJ")
														DisabiguteWords(r,Get_main_OBJ(tree,nn));
													else
														DisabiguteWords(r,Get_main_INFPH(tree,nn));
												}
											}
											else if(n.Goal=="PRD")
												DisabiguteWords(r,Get_main_PRD(tree,n));
											else if(n.Goal=="SBJ")
												DisabiguteWords(r,Get_main_SBJ(tree,n));
										foreach(ParseNode n in node.Children)
											if(n.Goal=="CMPS"||n.Goal=="PRD"||n.Goal=="SBJ")
												DisabiguteTree(tree,n);
												*/
				}break;
				case"INS":
				{
					/*	int i=0;
						if(((ParseNode)node.Children[0]).Goal=="QSW2")
							i=1;
						if(node.Children.Count==2)
						{
							DisabiguteTree(tree,(ParseNode)node.Children[0]);
							return;
						}
						else if(((ParseNode)node.Children[0]).Goal=="WHO")
						{
							ArrayList PRDS=GetPRDS(tree,(ParseNode)node.Children[1]);
							ArrayList SbjW=Get_main_RPN(tree,(ParseNode)node.Children[0]);
							ArrayList PRDW=new ArrayList();
							foreach(ParseNode n in PRDS)
							{
								PRDW=Get_main_PRD(tree,n);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}
						}
						else if(node.Children.Count==6)
						{
							ArrayList PRDS=GetPRDS(tree,(ParseNode)node.Children[4]);
							ArrayList SbjW=Get_main_NP(tree,(ParseNode)node.Children[1]);
							SbjW.AddRange(Get_main_SBJ(tree,(ParseNode)node.Children[3]));
							ArrayList PRDW=new ArrayList();
							foreach(ParseNode n in PRDS)
							{
								PRDW=Get_main_PRD(tree,n);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}						
						}
						else if(((ParseNode)node.Children[0]).Goal=="QSW1")
						{
							ArrayList PRDS=GetPRDS(tree,(ParseNode)node.Children[2]);
							ArrayList SbjW=Get_main_NP(tree,(ParseNode)node.Children[1]);
							ArrayList PRDW=new ArrayList();
							foreach(ParseNode n in PRDS)
							{
								PRDW=Get_main_PRD(tree,n);
								if(PRDW.Count<2)
									DisabiguteWords(PRDW,SbjW);
								else
								{
									DisabiguteWords(PRDW);
									if(((WordSense)SbjW[0]).Node.Senses==null)
										DisabiguteWords(PRDW.GetRange(0,1),SbjW);
								}
							}						
						}
						else if(node.Children.Count==3+i)
						{
							ArrayList PRDW=new ArrayList();
							PRDW.Add(new WordSense ("BE","V",1,(ParseNode)node.Children[i]));
							ArrayList SbjW=Get_main_SBJ(tree,(ParseNode)node.Children[1+i]);
							DisabiguteWords(PRDW,SbjW);
						}
						else if(((ParseNode)node.Children[2+i]).Goal=="CMPAJ")
						{
							ArrayList PRDW=Get_main_SBJ(tree,(ParseNode)node.Children[1+i]);
							ArrayList SbjW=Get_main_CMPAJ(tree,(ParseNode)node.Children[2+i]);
							DisabiguteWords(PRDW,SbjW);
						}
						else if(((ParseNode)node.Children[2+i]).Goal=="LPRPS")
						{
							ArrayList PRDW=Get_main_SBJ(tree,(ParseNode)node.Children[1+i]);
							ArrayList SbjW=Get_main_LPRPS(tree,(ParseNode)node.Children[2+i]);
							DisabiguteWords(PRDW,SbjW);
						}
						else
						{
							ArrayList PRDW=Get_main_PRD(tree,(ParseNode)node.Children[2+i]);
							ArrayList SbjW=Get_main_SBJ(tree,(ParseNode)node.Children[1+i]);
							if(PRDW.Count<2)
								DisabiguteWords(PRDW,SbjW);
							else
							{
								DisabiguteWords(PRDW);
								if(((WordSense)SbjW[0]).Node.Senses==null)
									DisabiguteWords(PRDW.GetRange(0,1),SbjW);
							}
						}
						foreach(ParseNode n in node.Children)
							if(n.Children!=null)
								DisabiguteTree(tree,n);*/
				}break;
				default:
					return null;
			}
			return null;
		}
		
		string SetId(ParseTree tree,ParseNode n,Concept Noun)
		{
			if(Noun.FullProperties["CARDINALITY"]!=null && 
				Noun.FullProperties["CARDINALITY"].IsModified)
			{
				string str=Noun.FullProperties["CARDINALITY"].Fillers[0].ScalarFiller;
				switch(str)
				{
					case"1":
						str="one";break;
					case"2":
						str="two";break;
					case"3":
						str="three";break;
					case"4":
						str="four";break;
					case"5":
						str="five";break;
					case"6":
						str="six";break;
					case"7":
						str="seven";break;
					case"8":
						str="eight";break;
					case"9":
						str="nine";break;
				}
				return str+" "+WSD.GetWordString(tree,n);
			}
			return WSD.GetWordString(tree,n);
		}

	}
}