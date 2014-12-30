using System;
using Wnlib;
using System.Collections;
using System.Text;
using WSD_TypeLib;
using OntoSem;

namespace QAS
{
	public class Query
	{
		#region NOT_NEEDED
		private ArrayList Get_main_SBJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="SSBJ")
			{
				MainOfObjs.AddRange(Get_main_SSBJ(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_SBJ(tree,n));
			return MainOfObjs;
		}
		
		private ArrayList Get_main_SSBJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			((ParseNode)node.Children[0]).visited=true;
			switch (((ParseNode)node.Children[0]).Goal)
			{
				case "NP":
					return Get_main_NP(tree,(ParseNode)node.Children[0]);
				default:
					throw new Exception("Invalid SSBJ");
			}
		}
		private ArrayList Get_main_NP(ParseTree tree, ParseNode node)
		{
			ParseNode fch=(ParseNode)node.Children[0];
			if(fch.Goal=="NP"||fch.Goal=="NPC")
				return Get_main_NP(tree,fch);
			else
			{
				foreach(ParseNode n in node.Children)
					if(n.Goal=="NNC"||n.Goal=="NN")
						return Get_main_NN(tree,n);
				return new ArrayList();
			}
		}
		private ArrayList Get_main_NN(ParseTree tree, ParseNode n)
		{
			n.visited=true;
			ParseNode term=(ParseNode)n.Children[0];
			if(term.Goal=="ADVS")
				term=(ParseNode)n.Children[1];
			string s=WSD.GetWordString(tree,term);
			switch(term.Goal)
			{
				case "PPN" :
					return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,term)});
				case "VING" :
					return new ArrayList( new WordSense [] {new WordSense(s,"V",-1,term)});
				default :
					return new ArrayList( new WordSense [] {new WordSense(s,"N",-1,term)});
			}
		}
		private ArrayList Get_main_DS(ParseTree tree, ParseNode node)
		{
			ArrayList DS =new ArrayList();
			ArrayList PRDs=GetPRDS(tree,(ParseNode)node.Children[1]);
			foreach(ParseNode n in PRDs)
			{
				ParseNode nn=(ParseNode)n.Children[0];
				if(nn.Goal=="PADV")
					nn=(ParseNode)n.Children[1];
				if(nn.Goal=="V")
				{
					string s=WSD.GetWordString(tree,nn);
					DS.Add(new WordSense(s,"V",-1,nn));
				}
				else
				{
					string s=WSD.GetWordString(tree,(ParseNode)nn.Children[nn.Children.Count-1]);
					DS.Add(new WordSense(s,"V",-1,(ParseNode)nn.Children[nn.Children.Count-1]));
				}
			}
			return DS;
		}
		private ParseNode Get_first_CMPS(ParseTree tree, ParseNode node)
		{
			ArrayList PRDs=GetPRDS(tree,(ParseNode)node.Children[1]);
			ParseNode ps=(ParseNode)((ParseNode)PRDs[0]).Children[((ParseNode)PRDs[0]).Children.Count-1];
			if(ps.Goal=="CMPS")
				return ps;
			else
				return null;
		}
		public static ArrayList GetPRDS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRD"||node.Goal=="IPRD"||
				node.Goal=="PPRD"||node.Goal=="GPRD"||node.Goal=="SPRD")
			{
				MainOfObjs.Add(node);
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(GetPRDS(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_main_CMPS(ParseTree tree, ParseNode node)
		{
			//eslam
			node.visited=true;
			ArrayList Mains = new ArrayList();
			ParseNode ch=(ParseNode)node.Children[0];
			ch=(ParseNode)ch.Children[0];
			ch.visited=true;
			switch (ch.Goal)
			{
				case"OBJ":
					Mains.AddRange(Get_main_OBJ(tree,ch));
					break;
				case"INFPO":
				case"INFPH":
					Mains.AddRange(Get_main_INFPH(tree,ch));
					break;
				case"CMPAVJ":
					Mains.AddRange(Get_main_CMPAVJ(tree,ch));
					break;
				case"LPRPS":
					Mains.AddRange(Get_main_LPRPS(tree,ch));
					break;
				default:
					break;
			}

			return Mains;
		}
		private ArrayList Get_main_LPRPS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRP")
			{
				MainOfObjs.Add(node);
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_LPRPS(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_main_OBJ(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="SOBJ")
			{
				MainOfObjs.AddRange(Get_main_SOBJ(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_OBJ(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_main_SOBJ(ParseTree tree,ParseNode node)
		{
			switch (((ParseNode)node.Children[0]).Goal)
			{
				case "NPC":
					return Get_main_NP(tree,(ParseNode)node.Children[0]);
				default:
					throw new Exception("Invalid SOBJ");
			}
		}
		private ArrayList Get_main_INFPH(ParseTree tree, ParseNode node)
		{
			ArrayList V=new ArrayList();
			ParseNode vnode=(ParseNode)node.Children[node.Children.Count-1];
			if(vnode.Goal=="VINF")
			{
				string str=WSD.GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			else
			{
				vnode=(ParseNode)node.Children[node.Children.Count-2];
				string str=WSD.GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			return V;
		}
		private ArrayList Get_main_CMPAVJ(ParseTree tree, ParseNode node)
		{
			if(((ParseNode)node.Children[0]).Goal=="CMPAJS")
				return Get_main_CMPAJS(tree,(ParseNode)node.Children[0]);
			else
				return new ArrayList();
		}
		private ArrayList Get_main_CMPAJS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="CMPAJ")
			{
				MainOfObjs.AddRange(Get_main_CMPAJ(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_CMPAJS(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_main_CMPAJ(ParseTree tree, ParseNode node)
		{
			ArrayList Mians = new ArrayList();
			switch(((ParseNode)node.Children[0]).Goal)
			{
				case"CADJ":
				case"CPADJ":
					string s=WSD.GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"A",-1,(ParseNode)node.Children[0]));
					return Mians;
				case"CMADJ":
					return Get_main_CMADJ(tree,((ParseNode)node.Children[0]));
				case"VPSP":
					s=WSD.GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"V",-1,(ParseNode)node.Children[0]));
					return Mians;
				default:
					throw new Exception("Invalid CMPAJ");
			}
		}
		private ArrayList Get_main_CMADJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs=new ArrayList();
			foreach(ParseNode n in node.Children)
				if(n.Goal=="PADJ" || n.Goal=="CADJ" || n.Goal=="SADJ")
					MainOfObjs.Add( new WordSense(WSD.GetWordString(tree,n),"A",-1,n));
				else if(n.Goal=="NPADJ")
					MainOfObjs.AddRange(Get_ADJ_NP(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_ADJ_NP(ParseTree tree,ParseNode node)
		{
			node.visited=true;
			foreach(ParseNode n in node.Children)
				if(n.Goal=="AVJ")
					return Get_main_AVJ(tree,n);
			return new ArrayList();
		}
		private ArrayList Get_main_AVJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			if(node.Children.Count > 1)
				return Get_main_BADJS(tree,(ParseNode)node.Children[1]);
			else
				return Get_main_BADJS(tree,(ParseNode)node.Children[0]);
		}
		private ArrayList Get_main_BADJS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="BADJ")
			{
				MainOfObjs.AddRange(Get_main_BADJ(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_BADJS(tree,n));
			return MainOfObjs;
		}
		private ArrayList Get_main_BADJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ParseNode Child = (ParseNode)node.Children[0];
			string s=WSD.GetWordString(tree,Child);
			switch(Child.Goal)
			{
				case "BPADJ" :
				case "CADJ" :
				case "SADJ" :
					return (new ArrayList( new WordSense [] {new WordSense(s,"A",-1,Child)}));
				case "VPSP" :
				case "VING" :
					return (new ArrayList( new WordSense [] {new WordSense(s,"V",-1,Child)}));
			}
			throw new Exception("Invalid BADJ");
		}
		#endregion
		public ParseTree Question;
		public ArrayList Frames;
		public Hashtable Concepts;
		public Ontology Onto;
		public TMR tmr=new TMR();
		public void Begin()
		{
			frmSyntaxGenerator form=new frmSyntaxGenerator();
			tmr.Onto=Onto;
			tmr.Init(new ArrayList(),null,null,false);
			ParseNode Snode=(ParseNode)Question.Root.Children[0];
			ParseNode Qnode=(ParseNode)Snode.Children[0];
			if(Qnode.Children.Count==2)		//case DS ya bahyem
			{
				ParseNode DSNode=(ParseNode)Qnode.Children[0];
				ArrayList arr = tmr.DisabiguteTree(Question,DSNode);
				bool b=CompareFrames(Frames,arr);
				form.Begin(null,b,AnswerType.Boolean);
				
				// problem with diffrent words in the sentence
			}
			else if(Qnode.Children.Count==3) // who PRDS ?
			{
				ParseNode PRDNode=(ParseNode)Qnode.Children[1];
				ArrayList arr = tmr.GetPRDS(Question,PRDNode);
				ArrayList AgentConcepts=new ArrayList();
				if(arr==null)
				{
					//case verb have
					tmr = new TMR();
					tmr.Onto = Onto;
					tmr.Init(new ArrayList(),null,new ArrayList(),false);
					arr=tmr.DisabiguteTree(Question,(ParseNode)((ParseNode)PRDNode.Children[0]).Children[1]);
					Concept r;
					r = CpmareBatekh(this.Concepts,arr);
					if(r == null)
					{
						form.Begin(null,null,AnswerType.NoAnswer);
						return;
					}
					else
					{
						form.Begin(null,r,AnswerType.Concept);
					}
				}
				else
				{
					if(!tmr.IsPassive)
						AgentConcepts.AddRange(GetAgentOf(Frames,arr));
					else
						AgentConcepts.AddRange(GetThemeOf(Frames,arr));
					if(AgentConcepts.Count==0)
					{
						form.Begin(null,null,AnswerType.NoAnswer);
						return;
					}
				}
				ArrayList c2=new ArrayList();
				foreach(Concept coc in AgentConcepts)
				{
					Frame frm=new Frame();
					frm.Predicate=coc;
					frm.IsConcept=true;
					foreach(Frame x in arr)
					{
						Frame h=new Frame();
						h=x.Copy();
						h.Predicate.FullProperties["AGENT"].Fillers[0].Frames=new ArrayList();
						h.Predicate.FullProperties["AGENT"].Fillers[0].Frames.Add(frm);
						h.Predicate.FullProperties["AGENT"].IsModified=true;
						c2.Add(h);
					}
				}
				foreach(Frame f in Frames)
				{
					if(f.Predicate.Name=="BE")
					{
						Property PA=f.Predicate.FullProperties["AGENT"];
						Property PT=f.Predicate.FullProperties["THEME"];
						ArrayList Agents=PA.Fillers[0].Frames;
						ArrayList Themes=PT.Fillers[0].Frames;
						foreach(Frame fa in Agents)
						{
							foreach(Concept c in AgentConcepts)
							{
								if(fa.Predicate==c)
								{
									c2.Add(f);
								}
							}
						}
						foreach(Frame fa in Themes)
						{
							foreach(Concept c in AgentConcepts)
							{
								if(fa.Predicate==c)
								{
									c2.Add(f);
								}
							}
						}
					}
				}
				form.Begin(null,c2,AnswerType.List);
				//the output is an arraylist
			}
			else if(Qnode.Children.Count==6)
			{
				ParseNode NPNode=(ParseNode)Qnode.Children[1];
				ParseNode SbjNode=(ParseNode)Qnode.Children[3];
				ParseNode IPRDSNode=(ParseNode)Qnode.Children[4];
				ArrayList arr = tmr.GetIPRDS(Question,IPRDSNode);
				tmr=new TMR();
				tmr.Onto=Onto;
				tmr.Init(new ArrayList(),null,null,false);
				ArrayList Sbjs=tmr.GetSubjects(Question,SbjNode);
				tmr=new TMR();
				tmr.Onto=Onto;
				tmr.Init(new ArrayList(),null,null,false);
				ArrayList Nps=tmr.DisabiguteTree(Question,NPNode);
				ArrayList c=new ArrayList();
				ArrayList Framesd=new ArrayList();
				foreach(Frame f in Frames)
					foreach(Frame fa in arr)
						if(f.Predicate.Name==fa.Predicate.Name)
							Framesd.Add(f);
				foreach(Frame f in Framesd)
				{
					if(
						CompareFrames(f.Predicate.FullProperties["AGENT"].Fillers[0].Frames,Sbjs)

						&&

						CompareFrames(f.Predicate.FullProperties["THEME"].Fillers[0].Frames,Nps)
						)
					{
						foreach(Frame fa in arr)
						{
							Frame x=new Frame();
							x=fa.Copy();
							x.Predicate.FullProperties["AGENT"].Fillers[0].Frames=new ArrayList();
							x.Predicate.FullProperties["AGENT"].Fillers[0].Frames=Sbjs;
							x.Predicate.FullProperties["THEME"].Fillers[0].Frames=new ArrayList();
							x.Predicate.FullProperties["THEME"].Fillers[0].Frames=f.Predicate.FullProperties["THEME"].Fillers[0].Frames;
							c.Add(x);
						}
					}
					else if(((Frame)f.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"]!=null
						&&((Frame)f.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"].IsModified)
					{
						if(
							CompareFrames(((Frame)f.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"].Fillers[0].Frames,Sbjs)

							&&

							CompareFrames(f.Predicate.FullProperties["THEME"].Fillers[0].Frames,Nps)
							)
						{
							foreach(Frame fa in arr)
							{
								Frame x=new Frame();
								x=fa.Copy();
								x.Predicate.FullProperties["AGENT"].Fillers[0].Frames=new ArrayList();
								x.Predicate.FullProperties["AGENT"].Fillers[0].Frames=Sbjs;
								x.Predicate.FullProperties["THEME"].Fillers[0].Frames=new ArrayList();
								x.Predicate.FullProperties["THEME"].Fillers[0].Frames=f.Predicate.FullProperties["THEME"].Fillers[0].Frames;
								c.Add(x);
							}
						}
					}
				}
				if(c.Count != 0)
					form.Begin(null,c,AnswerType.List);
				else 
					form.Begin(null,null,AnswerType.NoAnswer);
			}
			else if(Qnode.Children.Count==4)
			{
				ParseNode n0 = (ParseNode)Qnode.Children[0];
				ParseNode n1 = (ParseNode)Qnode.Children[1];
				ParseNode n2 = (ParseNode)Qnode.Children[2];
				if((n0.Goal=="BE2" && n2.Goal == "GPRDS" || 
					n0.Goal=="HAV1" && n2.Goal == "PPRDS" ||
					n0.Goal=="WIL1" && n2.Goal == "SPRDS" || 
					n0.Goal=="XV" && n2.Goal == "IPRDS" || 
					n0.Goal=="DO2" && n2.Goal == "IPRDS") 
					&& n1.Goal=="SBJ")//there is a problem in the parse tree
				{
					bool Passive=false;
					ArrayList arr = null;
					switch(n2.Goal)
					{
						case"GPRDS":
						{
							arr = tmr.GetGPRDS(Question,n2);
							if(((ParseNode)n2.Children[0]).Goal!="GVP")
								Passive=true;
						}break;
						case"PPRDS":
							arr = tmr.GetPPRDS(Question,n2);
							break;
						case"SPRDS":
							arr = tmr.GetSPRDS(Question,n2);
							break;
						case"IPRDS":
							arr = tmr.GetIPRDS(Question,n2);
							break;
					}
					tmr=new TMR();
					tmr.Onto=Onto;
					tmr.Init(new ArrayList(),null,null,false);
					tmr.IsPassive=Passive;
					ArrayList Sbjs=tmr.GetSubjects(Question,n1);
					ArrayList Framesd=new ArrayList();
					foreach(Frame f in Frames)
						foreach(Frame fa in arr)
							if(f.Predicate.Name==fa.Predicate.Name)
							{
								if(!tmr.IsPassive)
								{
									fa.Predicate.FullProperties["AGENT"].Fillers[0].Frames = Sbjs;
									fa.Predicate.FullProperties["AGENT"].IsModified = true;
									if(CompareTwoFrames(f,fa))
									{
										form.Begin(null,true,AnswerType.Boolean);
										return ;
									}
								}
								else
								{
									fa.Predicate.FullProperties["THEME"].Fillers[0].Frames = Sbjs;
									fa.Predicate.FullProperties["THEME"].IsModified = true;
									if(CompareTwoFrames(f,fa))
									{
										form.Begin(null,true,AnswerType.Boolean);
										return ;
									}
								}
							}
							
							
					form.Begin(null,null,AnswerType.NoAnswer);
				}
				else if(n1.Goal=="NP"&&n2.Goal=="PRDS")
				{
					ArrayList Sbj=Get_main_NP(Question,n1);
					ArrayList arr = tmr.GetPRDS(Question,n2);
					ArrayList Agents= GetAgentOf(Frames,arr);
					Concept con=tmr.GetConcept(((WordSense)Sbj[0]).Word);
					ArrayList result = new ArrayList();
					bool unique=true;
					Concept cx=null;
					foreach(Concept c in Agents)
						if(con.Name==c.Name)
						{
							if(cx==null)
								cx=c;
							unique&=c==cx;
							Frame F = new Frame();
							F = ((Frame)arr[0]).Copy();
							F.Predicate.FullProperties["AGENT"].Fillers[0].Frames =  new ArrayList();
							Frame hfx=new Frame();
							hfx.Predicate=c;
							F.Predicate.FullProperties["AGENT"].Fillers[0].Frames.Add(hfx);
							F.Predicate.FullProperties["AGENT"].IsModified=true;
							result.Add(F);
						}
					if(unique)
						result.Add(cx);
					if(result.Count == 0)
						form.Begin(null,null,AnswerType.NoAnswer,unique,true);
					else if(!unique)
						form.Begin(null,result,AnswerType.List,unique,true);
					else
						form.Begin(null,result,AnswerType.MixedList,unique,true);
				}
				else if(n0.Goal=="QSW2"&&n1.Goal=="BE2"&&n2.Goal=="SBJ")
				{
					ArrayList Sbjs=tmr.GetSubjects(Question,n2);
					Frame Sbjf=(Frame)Sbjs[0];
					ArrayList c2=new ArrayList();
					if(Concepts.Contains(Sbjf.Predicate.Name))
					{
						Concept c=(Concept)Concepts[Sbjf.Predicate.Name];
						if(c.FullProperties["SUB"] != null)
						{
							bool b=true;
							foreach(Filler fl in c.FullProperties["SUB"].Fillers)
							{
								Concept sub=fl.ConceptFiller;
								b=true;
								foreach(Property p in Sbjf.Predicate.FullProperties)
								{
									if(p.IsModified)
									{
										if(!ChechForProperty(c.FullProperties[p.Name],p))
										{
											if(!ChechForProperty(sub.FullProperties[p.Name],p))
											{
												b=false;
												break;
											}
										}
									}
								}
								if(b)
									c2.Add(sub);			
							}
						}
						else
						{
							bool b=true;
							foreach(Property p in Sbjf.Predicate.FullProperties)
							{
								if(p.IsModified)
								{
									if(!ChechForProperty(c.FullProperties[p.Name],p))
									{
										b=false;
										break;
									}
								}   
							}
							if(b)
								c2.Add(c);
						}
						if(c.FullProperties["CARDINALITY"] != null&&c.FullProperties["CARDINALITY"].IsModified)
						{
							bool b=false;
							foreach(Concept r in c.FullProperties["CARDINALITY"].Fillers[0].Frames)
							{
								b=true;
								foreach(Property p in Sbjf.Predicate.FullProperties)
								{
									if(p.IsModified)
									{
										if(!ChechForProperty(r.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
									}
								}
								if(b)
									c2.Add(r);
							}
						}
					}
					ArrayList frar = new ArrayList();
					ArrayList temp=new ArrayList();
					foreach(Frame f in Frames)
					{
						if(f.Predicate.Name=="BE")
						{
							Property PA=f.Predicate.FullProperties["AGENT"];
							Property PT=f.Predicate.FullProperties["THEME"];
							ArrayList Agents=PA.Fillers[0].Frames;
							ArrayList Themes=PT.Fillers[0].Frames;
							foreach(Frame fa in Agents)
							{
								foreach(Concept c in c2)
									if(fa.Predicate==c && f.Predicate.Name=="BE")
									{
										temp.Add(f);
										frar.Add(c);
									}
							}

							foreach(Frame fa in Themes)
							{
								foreach(Concept c in c2)
									if(fa.Predicate==c && f.Predicate.Name=="BE")
									{
										temp.Add(f);
										frar.Add(c);
									}
							}
						}
					}
					c2.InsertRange(0,temp);
					switch(WSD.GetWordString(Question,n0))
					{
						case"WHAT":
						case"WHICH":
						case"WHO":
							if(temp.Count==0)
								form.Begin(null,c2,AnswerType.ConceptList);
							else
								form.Begin(null,c2,AnswerType.MixedList);
							break;
						case"WHERE":
							foreach(Concept con in c2)
							{
								if(con.FullProperties["LOCATION"].IsModified)
								{
									form.Begin(null,con.FullProperties["LOCATION"],AnswerType.Property);
									/*Get the location of predicates containing this concept
									 *using frar  */
									break;
								}
							}
							break;
						default:
							form.Begin(null,null,AnswerType.NoAnswer);
							break;
					}
				}
				else if(n0.Goal=="BE2"&&n1.Goal=="SBJ"&&n2.Goal=="CMPAJ")
				{
					ArrayList nouns=tmr.GetSubjects(Question,n1);
					tmr.CurrNoun.Clear();
					tmr.CurrNoun.Add(((Frame)nouns[0]).Predicate);
					tmr.DisabiguteTree(Question,n2);
					Concept sbj=(Concept)tmr.CurrNoun[0];
					if(Concepts.Contains(sbj.Name))
					{
						Concept c=(Concept)Concepts[sbj.Name];
						if(c.FullProperties["SUB"] != null)
						{
							bool b=true;
							foreach(Filler fl in c.FullProperties["SUB"].Fillers)
							{
								Concept sub=fl.ConceptFiller;
								b=true;
								foreach(Property p in sbj.FullProperties)
								{
									if(p.IsModified)
									{
										if(!ChechForProperty(c.FullProperties[p.Name],p))
										{
											if(!ChechForProperty(sub.FullProperties[p.Name],p))
											{
												b=false;
												break;
											}
										}
									}
								}
								if(b)
								{
									form.Begin(null,b,AnswerType.Boolean);
									break;
								}								
							}
							if(!b)
								form.Begin(null,b,AnswerType.Boolean);
						}
						else if(c.FullProperties["CARDINALITY"] != null&&c.FullProperties["CARDINALITY"].IsModified)
						{
							bool b=false;
							foreach(Concept r in c.FullProperties["CARDINALITY"].Fillers[0].Frames)
							{
								b=true;
								foreach(Property p in sbj.FullProperties)
								{
									if(p.IsModified)
									{
										if(!ChechForProperty(r.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
									}
								}
								if(b)
								{
									form.Begin(null,b,AnswerType.Boolean);
									return;
								}
							}
							form.Begin(null,b,AnswerType.Boolean);
						}
						else
						{
							bool b=true;
							foreach(Property p in sbj.FullProperties)
							{
								if(p.IsModified)
								{
									if(!ChechForProperty(c.FullProperties[p.Name],p))
									{
										b=false;
										break;
									}
								}   
							}
							if(b)
								form.Begin(null,b,AnswerType.Boolean);
							if(!b)
								form.Begin(null,b,AnswerType.Boolean);
						}
					}
				}
				else if(n0.Goal=="BE2"&&n1.Goal=="SBJ"&&n2.Goal=="LPRPS")
				{
					ArrayList arrSbj = tmr.GetSubjects(Question,n1);
					Concept consbj = ((Frame)arrSbj[0]).Predicate;
					ArrayList PrpFrames=new ArrayList();
					ParseNode nprp=(ParseNode)n2.Children[0];
					SetPRSSpecial(Question,nprp,consbj);
					Frame FR = null;
					Concept fragent = null;
					Concept frtheme = null;
					Frame result = null;
					bool b=false;
					foreach(Frame f in Frames)
					{
						if(f.Predicate.FullProperties["AGENT"] != null&&f.Predicate.FullProperties["AGENT"].Fillers != null && f.Predicate.FullProperties["AGENT"].Fillers.Count>0 &&
							f.Predicate.FullProperties["AGENT"].Fillers[0].Frames != null && f.Predicate.FullProperties["AGENT"].Fillers[0].Frames.Count >0)
						{
							FR = (Frame)f.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0];
							fragent = FR.Predicate;
							if(fragent.Name==consbj.Name)
							{
								b=true;
								foreach(Property p in consbj.FullProperties)
								{
									if(p.IsModified)
										if(!ChechForProperty(fragent.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
								{
									result = f;
									break;
								}
								else
								{
									b=false;
									foreach(Property p in consbj.FullProperties)
									{
										if(p.IsModified)
											if(ChechForProperty(f.Predicate.FullProperties[p.Name],p))
											{
												b=true;
												break;
											}
									}
									if(b)
									{
										result = f;
										break;
									}
								}
							}
						
						}																																							  																																			   
						if(f.Predicate.FullProperties["THEME"] != null&&f.Predicate.FullProperties["THEME"].Fillers != null && f.Predicate.FullProperties["THEME"].Fillers.Count>0&&
							f.Predicate.FullProperties["THEME"].Fillers[0].Frames != null && f.Predicate.FullProperties["THEME"].Fillers[0].Frames.Count >0 && !b)
						{
							FR = (Frame)f.Predicate.FullProperties["THEME"].Fillers[0].Frames[0];
							frtheme = FR.Predicate;
							if(frtheme.Name==consbj.Name)
							{
								b=true;
								foreach(Property p in consbj.FullProperties)
								{
									if(p.IsModified)
										if(!ChechForProperty(frtheme.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
								{
									result = f;
									break;
								}
								else
								{
									b=false;
									foreach(Property p in consbj.FullProperties)
									{
										if(p.IsModified)
											if(ChechForProperty(f.Predicate.FullProperties[p.Name],p))
											{
												b=true;
												break;
											}
									}
									if(b)
									{
										result = f;
										break;
									}
								}
							}
						}						
					}
					if(result != null)
						form.Begin(null,true,AnswerType.Boolean);
					else
						form.Begin(null,false,AnswerType.Boolean);
				}

			}
			else if(Qnode.Children.Count==5)
			{
				ParseNode n0=(ParseNode)Qnode.Children[0];
				ParseNode n1=(ParseNode)Qnode.Children[1];
				ParseNode n2=(ParseNode)Qnode.Children[2];
				ParseNode n3=(ParseNode)Qnode.Children[3];
				if(n0.Goal == "QSW2" && (n1.Goal=="BE2" || n1.Goal=="HAV1" || n1.Goal=="WIL1" || n1.Goal=="XV" || n1.Goal=="DO2") && n2.Goal=="SBJ" && (n3.Goal == "GPRDS"||n3.Goal == "PPRDS"||n3.Goal == "SPRDS"||n3.Goal == "IPRDS"))
				{
					ArrayList arr = null;
					switch(n3.Goal)
					{
						case"GPRDS":
							arr = tmr.GetGPRDS(Question,n3);
							break;
						case"PPRDS":
							arr = tmr.GetPPRDS(Question,n3);
							break;
						case"SPRDS":
							arr = tmr.GetSPRDS(Question,n3);
							break;
						case"IPRDS":
							arr = tmr.GetIPRDS(Question,n3);
							break;
					}
					tmr=new TMR();
					tmr.Onto=Onto;
					tmr.Init(new ArrayList(),null,null,false);
					ArrayList Sbjs=tmr.GetSubjects(Question,n2);
					ArrayList Framesd=new ArrayList();
					foreach(Frame f in Frames)
						foreach(Frame fa in arr)
							if(f.Predicate.Name==fa.Predicate.Name)
							{
								fa.Predicate.FullProperties["AGENT"].Fillers[0].Frames = Sbjs;
								fa.Predicate.FullProperties["AGENT"].IsModified = true;
								if(CompareTwoFrames(f,fa))
								{
									Framesd.Add(f);
								}
							}
					string word = WSD.GetWordString(Question,n0);
					ArrayList result = new ArrayList(); 
					if(word == "WHAT" ||word == "WHICH" ||word == "WHO")
					{
						foreach(Frame f in  Framesd)
						{
							if(f.Predicate.FullProperties["THEME"].Fillers[0].Frames != null)
								result.AddRange(f.Predicate.FullProperties["THEME"].Fillers[0].Frames);
						}
					}
					else if(word == "WHEN" ||word == "WHY" )
					{
						foreach(Frame f in  Framesd)
						{
							if(f.Reason.Count!=0)
								result.AddRange(f.Reason);
							else if(f.Predicate.FullProperties["PURPOSE"]!=null&&f.Predicate.FullProperties["PURPOSE"].IsModified)
								result.AddRange(f.Predicate.FullProperties["PURPOSE"].Fillers[0].Frames);
						}
					}
					else
					{
						/**/result.Clear();

					}
					if(result.Count!=0)
						form.Begin(null,result,AnswerType.List);
					else
						form.Begin(null,null,AnswerType.NoAnswer);
				}
				else if(n0.Goal=="QSW2"&&n1.Goal=="BE2"&&n2.Goal=="SBJ"&&n3.Goal=="CMPAJ")
				{
					ArrayList nouns=tmr.GetSubjects(Question,n2);
					tmr.CurrNoun.Clear();
					tmr.CurrNoun.Add(((Frame)nouns[0]).Predicate);
					tmr.DisabiguteTree(Question,n3);
					Concept sbj=(Concept)tmr.CurrNoun[0];
					ArrayList reson = null;
					foreach(Frame c in Frames)
					{
						bool b=true;
						if(((Frame)c.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate.Name==sbj.Name)
						{
							foreach(Property p in sbj.FullProperties)
							{
								if(p.IsModified)
									if(!ChechForProperty(((Frame)c.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate.FullProperties[p.Name],p))
									{
										b=false;
										break;
									}
							}
							if(b)
							{
								if(c.Reason != null && c.Reason.Count != 0)
								{
									reson = c.Reason;
									break;
								}
								else if(c.Predicate.FullProperties["PURPOSE"]!=null&&c.Predicate.FullProperties["PURPOSE"].IsModified)
									reson=c.Predicate.FullProperties["PURPOSE"].Fillers[0].Frames;
							}
						}
					}
					if(reson!=null)
						form.Begin(null,reson,AnswerType.List);
					else
						form.Begin(null,null,AnswerType.NoAnswer);
				}
				else if((n0.Goal=="QSW2"&&n1.Goal=="BE2"&&n2.Goal=="SBJ"&&n3.Goal=="LPRPS"))
				{
					ArrayList arrSbj = tmr.GetSubjects(Question,n2);
					Concept consbj = ((Frame)arrSbj[0]).Predicate;
					ArrayList PrpFrames=new ArrayList();
					ParseNode nprp=(ParseNode)n3.Children[0];
					SetPRSSpecial(Question,nprp,consbj);
					Frame FR = null;
					Concept fragent = null;
					Concept frtheme = null;
					Frame result = null;
					bool b=false;
					Concept rescon = null;
					foreach(Frame f in Frames)
					{
						if(f.Predicate.FullProperties["AGENT"] != null&&f.Predicate.FullProperties["AGENT"].Fillers != null && f.Predicate.FullProperties["AGENT"].Fillers.Count>0&&
							f.Predicate.FullProperties["AGENT"].Fillers[0].Frames != null && f.Predicate.FullProperties["AGENT"].Fillers[0].Frames.Count >0 && !b)
						{
							FR = (Frame)f.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0];
							fragent = FR.Predicate;
							if(fragent.Name==consbj.Name)
							{
								b=true;
								foreach(Property p in consbj.FullProperties)
								{
									if(p.IsModified)
										if(!ChechForProperty(fragent.FullProperties[p.Name],p))
										{
											rescon = fragent;
											b=false;
											break;
										}
								}
								if(b)
								{
									rescon = fragent;
									result = f;
									break;
								}
								else
								{
									b=false;
									foreach(Property p in consbj.FullProperties)
									{
										if(p.IsModified)
											if(ChechForProperty(f.Predicate.FullProperties[p.Name],p))
											{
												b=true;
												break;
											}
									}
									if(b)
									{
										rescon = fragent;
										result = f;
										break;
									}
								}
							}
						
						}																																							  																																			   
						if(f.Predicate.FullProperties["THEME"] != null&&f.Predicate.FullProperties["THEME"].Fillers != null && f.Predicate.FullProperties["THEME"].Fillers.Count>0&&
							f.Predicate.FullProperties["THEME"].Fillers[0].Frames != null && f.Predicate.FullProperties["THEME"].Fillers[0].Frames.Count >0 && !b)
						{
							FR = (Frame)f.Predicate.FullProperties["THEME"].Fillers[0].Frames[0];
							frtheme = FR.Predicate;
							if(frtheme.Name==consbj.Name)
							{
								b=true;
								foreach(Property p in consbj.FullProperties)
								{
									if(p.IsModified)
										if(!ChechForProperty(frtheme.FullProperties[p.Name],p))
										{
											b=false;
											break;
										}
								}
								if(b)
								{
									rescon = frtheme;
									result = f;
									break;
								}
								else
								{
									b=false;
									foreach(Property p in consbj.FullProperties)
									{
										if(p.IsModified)
											if(ChechForProperty(f.Predicate.FullProperties[p.Name],p))
											{
												b=true;
												break;
											}
									}
									if(b)
									{
										rescon = frtheme;
										result = f;
										break;
									}
								}
							}
						}						
					}
					if(result != null)
					{
						string word = WSD.GetWordString(Question,n0);
						if(word == "WHICH" || word == "WHAT" ||word == "WHO")
						{
							form.Begin(null,rescon,AnswerType.Concept);
						}
						else if(word == "WHY" || word == "WHEN")
						{
							if(!(result.Predicate.FullProperties["PURPOSE"]!=null&&result.Predicate.FullProperties["PURPOSE"].IsModified))
								form.Begin(null,result.Reason,AnswerType.List);
							else
								form.Begin(null,result.Predicate.FullProperties["PURPOSE"].Fillers[0].Frames,AnswerType.List);
						}
						else
						{//where then write the location
							if(result.Predicate.FullProperties["LOCATION"].IsModified)
								form.Begin(null,result.Predicate.FullProperties["LOCATION"],AnswerType.Property);
							else
								form.Begin(null,null,AnswerType.NoAnswer);
						}
					}
					else
						form.Begin(null,null,AnswerType.NoAnswer);
				}
			}
		}


		public Frame SetPRSSpecial(ParseTree tree,ParseNode node,Concept CurrNoun)
		{
			string word  = WSD.GetWordString(tree,(ParseNode)node.Children[0]);
			ArrayList Modification = tmr.GetPRSModification(word);
			ArrayList arr = tmr.DisabiguteTree(tree,(ParseNode)node.Children[1]);
			if(Modification==null)
				return null;
			if(word == "IN")
			{
				Property p=CurrNoun.FullProperties["SOURCE"];
				if(p!=null)
				{
					p.Fillers[0] = new Filler(CurrNoun.FullProperties["SOURCE"],Modifier.UNKNOWN,null,arr,false);	
					p.IsModified=true;
				}
				p=CurrNoun.FullProperties["DESTINATION"];
				if(p!=null)
				{
					p.Fillers[0] = new Filler(CurrNoun.FullProperties["DESTINATION"],Modifier.UNKNOWN,null,arr,false);	
					p.IsModified=true;
				}
			}
			for(int i = 0;i<Modification.Count ; i++)
			{
				
				bool b = (bool)Modification[i];
				i++;
				string prop = (string)Modification[i];
				Property p=CurrNoun.FullProperties[prop];
				if(p!=null)
				{
					p.Fillers[0] = new Filler(CurrNoun.FullProperties[prop],Modifier.UNKNOWN,null,arr,false);	
					p.IsModified=true;
				}
			}
			return null;
		}

		
		public Concept CpmareBatekh(Hashtable cons,ArrayList arr)
		{
			foreach(Concept c in cons.Values)
			{
				Property pa = c.FullProperties["HAS-OBJECT-AS-PART"];
				bool b = false;
				if(pa!=null)
				{
					if(pa.IsModified)
					{
						foreach(Frame f in arr )
						{
							b = false;
							foreach(Frame fa in pa.Fillers[0].Frames )
							{
								if(CompareTwoFrames(fa , f) )
								{
									b = true;
									break;
								}
							}
							if(!b)
								break;
						}
						if(!b)
							continue;
						else
							return c;
					}
				}
			}
			return null;
		}

		bool CompareFrames(ArrayList Frames,ArrayList arr)
		{
			foreach(Frame Q in arr)
			{
				bool b=false;
				foreach(Frame F in Frames)
				{
					b=CompareTwoFrames(F,Q);
					if(b)
						break;
				}
				if(!b)
					return false;
			}
			return true;
		}
		bool CompareTwoFrames(Frame F,Frame Q)
		{ 
			bool Return = false;
			if(F.Predicate.Name==Q.Predicate.Name)
			{
				Concept c=F.Predicate;
				if(Q.IsConcept&&F.Predicate.FullProperties["SUB"]!=null)
				{
					foreach(Filler fl in F.Predicate.FullProperties["SUB"].Fillers)
					{
						Concept sub=fl.ConceptFiller;
						bool b=true;
						foreach(Property p in Q.Predicate.FullProperties)
						{
							if(p.IsModified)
							{
								if(!ChechForProperty(c.FullProperties[p.Name],p))
								{
									if(!ChechForProperty(sub.FullProperties[p.Name],p))
									{
										b=false;
										break;
									}
								}
							}
						}
						if(b)
							return true;
					}
				}
				else
				{
					bool s=true;
					foreach(Property P in Q.Predicate.FullProperties)
					{
						if(P.IsModified)
						{
							if(ChechForProperty(F.Predicate.FullProperties[P.Name],Q.Predicate.FullProperties[P.Name]))
								continue;
							else
								Return=s=false;
						}
					}
					if(!CompareFrames(F.Reason,Q.Reason))
						Return=s=false;
					if(s)
						return true;
					
				}
				if(!Return && F.Predicate.FullProperties["CARDINALITY"] != null&&F.Predicate.FullProperties["CARDINALITY"].IsModified &&F.Predicate.FullProperties["CARDINALITY"].Fillers[0].Frames != null)
				{
					bool b=false;
					foreach(Concept r in F.Predicate.FullProperties["CARDINALITY"].Fillers[0].Frames)
					{
						b=true;
						foreach(Property p in Q.Predicate.FullProperties)
						{
							if(p.IsModified)
							{
								if(!ChechForProperty(r.FullProperties[p.Name],p))
								{
									b=false;
									break;
								}
							}
						}
						if(b)
							return true;
					}
					return false;
				}
				return false;
			}
			else
				return false;
		}
		public bool ChechForProperty(Property FP,Property QP)
		{
			if(!FP.IsModified)
				return false;
			if(QP.Fillers[0].IsScalar)
				return (QP.Fillers[0].ScalarFiller==FP.Fillers[0].ScalarFiller);
			else
			{
				if(FP.Name == "AGENT" || FP.Name == "THEME")
				{
					if(QP.Fillers[0].Frames!=null&&FP.Fillers[0].Frames!=null)
					{
						if(((Frame)FP.Fillers[0].Frames[0]).IsConcept && ((Frame)FP.Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"] !=null&&((Frame)FP.Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"].IsModified)
							return (CompareTwoFrames((Frame)FP.Fillers[0].Frames[0],(Frame)QP.Fillers[0].Frames[0]) || CompareTwoFrames((Frame)((Frame)FP.Fillers[0].Frames[0]).Predicate.FullProperties["PART-OF-OBJECT"].Fillers[0].Frames[0],(Frame)QP.Fillers[0].Frames[0]));
						else
							return CompareTwoFrames((Frame)FP.Fillers[0].Frames[0],(Frame)QP.Fillers[0].Frames[0]);
					}
					else
						return false;
				}
				else
				{
					if(QP.Fillers[0].Frames!=null&&FP.Fillers[0].Frames!=null)
						return CompareTwoFrames((Frame)FP.Fillers[0].Frames[0],(Frame)QP.Fillers[0].Frames[0]);
					else
						return false;
				}
			}
		}
		ArrayList GetAgentOf(ArrayList Frames,ArrayList arr)
		{
			ArrayList arr1=new ArrayList();
			Frame Q = (Frame)arr[0];
			foreach(Frame F in Frames)
			{
				bool b = CompareTwoFramesWOA(F,Q);
				if(b)
					arr1.Add(((Frame)F.Predicate.FullProperties["AGENT"].Fillers[0].Frames[0]).Predicate);
			}
			return arr1;
		}
		bool CompareTwoFramesWOA(Frame F,Frame Q)
		{	
			if(F.Predicate.Name==Q.Predicate.Name)
			{
				foreach(Property P in Q.Predicate.FullProperties)
				{
					if(P.IsModified)
					{
						if(P.Name=="AGENT")
							continue;
						if(ChechForProperty(F.Predicate.FullProperties[P.Name],Q.Predicate.FullProperties[P.Name]))
							continue;
						else
							return false;
					}
				}
			}
			else
				return false;
			return true;
		}
		ArrayList GetThemeOf(ArrayList Frames,ArrayList arr)
		{
			ArrayList arr1=new ArrayList();
			Frame Q = (Frame)arr[0];
			foreach(Frame F in Frames)
			{
				bool b = CompareTwoFramesWOT(F,Q);
				if(b)
					arr1.Add(((Frame)F.Predicate.FullProperties["THEME"].Fillers[0].Frames[0]).Predicate);
			}
			return arr1;
		}
		bool CompareTwoFramesWOT(Frame F,Frame Q)
		{	
			if(F.Predicate.Name==Q.Predicate.Name)
			{
				foreach(Property P in Q.Predicate.FullProperties)
				{
					if(P.IsModified)
					{
						if(P.Name=="THEME")
							continue;
						if(ChechForProperty(F.Predicate.FullProperties[P.Name],Q.Predicate.FullProperties[P.Name]))
							continue;
						else
							return false;
					}
				}
			}
			else
				return false;
			return true;
		}
	}
}