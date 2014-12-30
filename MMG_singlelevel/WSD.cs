using System;
using Wnlib;
using System.Collections;
using System.Text;
using WSD_TypeLib;

namespace QAS
{
	public class WSD
	{
		public int threshold;
		public int percent;
		public bool tagged_only;
		private WordNetClasses.WN wnc;
		private WSD_ControlClass control;
		public WSD(bool tagged,int Threshold,int Percent)
		{
			threshold=Threshold;
			percent=Percent;
			tagged_only=tagged;
			wnc = new WordNetClasses.WN(Wnlib.WNCommon.path);
			control = new WSD_ControlClass();
			control.Algorithm="WordNet::SenseRelate::Algorithm::Global";
			control.WNpath="../../../dict";
			control.Measure="WordNet::Similarity::lesk";
			control.Config="../../../config/config-lesk.conf";
		}

		public void Init()
		{
			control.Init();
		}

		private ArrayList GetSenses(string pos,string noun)
		{
			string pos2=null;
			switch(pos)
			{
				case "N":
					pos2="noun";break;
				case "V":
					pos2="verb";break;
				case "A":
					pos2="adj";break;
				case "R":
					pos2="adv";break;
			}
			bool b=false;
			SearchSet boj=null;
			ArrayList list=new ArrayList();
			wnc.OverviewFor(noun,pos2,ref b,ref boj,list);
			ArrayList senses=new ArrayList();
			for(int i=0;i<list.Count;i++)
			{
				Search vs=(Search)list[i];
				int cut=(int)Math.Ceiling(vs.senses.Count*percent/100.0);
				if(cut<threshold)
					cut=threshold;
				if(vs.taggedSenses>0 && tagged_only)
					cut=vs.taggedSenses;
				foreach(SynSet ss in vs.senses)
				{
					if(--cut<0)
						break;
					senses.Add((vs.word+'#'+pos+'#'+(ss.sense+1).ToString()).ToLower());
				}
			}
			return senses;
		}
		
		public static ArrayList Get_IDFP(ParseTree tree,ParseNode node)
		{
			node.visited=true;
			switch(GetWordString(tree,node))
			{
				case "ANYBODY":
				case "EVERYBODY":
				case "NOBODY":
					return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,node)});
				case "ANYONE":
				case "EVERYONE":
				case "NO_ONE":
				case "NONE":
					return new ArrayList( new WordSense [] {new WordSense("ONE","N",2,node)});
				case "EVERYTHING":
					return new ArrayList( new WordSense [] {new WordSense("THING","N",3,node)});
				default:
					throw new Exception("Invalid IDFP");
			}
		}

		public static ArrayList Get_DEMP(ParseTree tree,ParseNode node)
		{
			//Discourse
			node.visited=true;
			return new ArrayList( new WordSense [] {new WordSense("ONE","N",2,node)});
		}
		
		private ArrayList Get_main_OBJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
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
/*
		private ArrayList Get_main_IPRD(ParseTree tree, ParseNode node)
		{
			ArrayList V=new ArrayList();
			if(((ParseNode)node.Children[0]).Goal=="VINF")
			{
				string s=GetWordString(tree,(ParseNode)node.Children[0]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)node.Children[0]));
			}
			else if(((ParseNode)node.Children[0]).Goal=="IVP")
			{
				ParseNode n=(ParseNode)node.Children[0];
				string s=GetWordString(tree,(ParseNode)n.Children[n.Children.Count-1]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)n.Children[n.Children.Count-1]));
			}
			if(node.Children.Count==2)
				V.AddRange(Get_main_CMPS(tree,(ParseNode)node.Children[1]));
			return V;
		}

		private ArrayList Get_main_IPRDS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="IPRD")
			{
				MainOfObjs.AddRange(Get_main_IPRD(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_IPRDS(tree,n));
			return MainOfObjs;
		}
*/
		private ArrayList Get_main_PRPS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRP")
			{
				MainOfObjs.AddRange(Get_main_OBJ(tree,(ParseNode)node.Children[1]));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_PRPS(tree,n));
			return MainOfObjs;
		}

		/*private ArrayList Get_main_SPRD(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList V=new ArrayList();
			if(((ParseNode)node.Children[0]).Goal=="VINF")
			{
				string s=GetWordString(tree,(ParseNode)node.Children[0]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)node.Children[0]));
			}
			else if(((ParseNode)node.Children[0]).Goal=="SVP")
			{
				ParseNode n=(ParseNode)node.Children[0];
				string s=GetWordString(tree,(ParseNode)n.Children[n.Children.Count-1]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)n.Children[n.Children.Count-1]));
			}
			if(node.Children.Count==2)
				V.AddRange(Get_main_CMPS(tree,(ParseNode)node.Children[1]));
			return V;
		}

		private ArrayList Get_main_SPRDS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="SPRD")
			{
				node.visited=true;
				MainOfObjs.AddRange(Get_main_SPRD(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_SPRDS(tree,n));
			return MainOfObjs;
		}

		private ArrayList Get_main_PPRD(ParseTree tree, ParseNode node)
		{
			ArrayList V=new ArrayList();
			if(((ParseNode)node.Children[0]).Goal=="VPSP")
			{
				string s=GetWordString(tree,(ParseNode)node.Children[0]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)node.Children[0]));
			}
			else if(((ParseNode)node.Children[0]).Goal=="PVP")
			{
				ParseNode n=(ParseNode)node.Children[0];
				string s=GetWordString(tree,(ParseNode)n.Children[n.Children.Count-1]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)n.Children[n.Children.Count-1]));
			}
			if(node.Children.Count==2)
				V.AddRange(Get_main_CMPS(tree,(ParseNode)node.Children[1]));
			return V;
		}

		public static ArrayList GetPPRDS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PPRD")
			{
				MainOfObjs.Add(node);
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(GetPPRDS(tree,n));
			return MainOfObjs;
		}
*/
		private ArrayList Get_main_SOBJ(ParseTree tree,ParseNode node)
		{
			node.visited=true;
			switch (((ParseNode)node.Children[0]).Goal)
			{
				case "NPC":
					return Get_main_NP(tree,(ParseNode)node.Children[0]);
				case "NC":
					return Get_main_NC(tree,(ParseNode)node.Children[0]);
				default:
					throw new Exception("Invalid SOBJ");
			}
		}

		private ArrayList Get_main_ABPH(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mains = new ArrayList();
			if(((ParseNode)node.Children[0]).Goal == "NP")
				Mains.AddRange(Get_main_NP(tree,(ParseNode)node.Children[0]));
			else
			{
				foreach(ParseNode n in node.Children)
				{
					if(n.Goal=="NN")
					{
						n.visited=true;
						ParseNode term=(ParseNode)n.Children[0];
						if(term.Goal=="ADVS")
							term=(ParseNode)n.Children[1];
						string s=GetWordString(tree,term);
						switch(term.Goal)
						{
							case "PPN" :
								Mains.Add(new WordSense("ONE","N",2,term));
								break;
							case "VING" :
								Mains.Add(new WordSense(s,"V",-1,term));
								break;
							case "N" :
								Mains.Add(new WordSense(s,"N",-1,term));
								break;
						}
						break;
					}
				}
			}
			Mains.AddRange(Get_main_PRPH(tree,
				(ParseNode)node.Children[node.Children.Count-1]));
			return Mains;
		}

		private ArrayList Get_main_PRPH(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList V=new ArrayList();
			ParseNode vnode=(ParseNode)node.Children[node.Children.Count-1];
			if(vnode.Goal=="VING"||vnode.Goal=="VPSP")
			{
				string str=GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			else
			{
				vnode=(ParseNode)node.Children[node.Children.Count-2];
				if(!vnode.visited && vnode.Senses.Count>1)
				{
					vnode.visited=true;
					DisabiguteTree(tree,node);
				}
				string str=GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			return V;
		}

		private ArrayList Get_main_ADVC(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			return (Get_main_DS(tree,((ParseNode)node.Children[1])));
		}

		private ArrayList Get_main_ADJC(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mains = new ArrayList();
			Mains.Add( new WordSense("ONE","N",2,(ParseNode)node.Children[0]));
			if(((ParseNode)node.Children[1]).Goal == "PRD")
				Mains.AddRange(Get_main_PRD(tree,(ParseNode)node.Children[1]));
			else if(((ParseNode)node.Children[1]).Goal == "DS")
				Mains.AddRange(Get_main_DS(tree,(ParseNode)node.Children[1]));
			else
				throw new Exception("Invalid ADJC");
			return Mains;
		}

		private ArrayList Get_main_OPP(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			string word=GetWordString(tree,node);
			switch(word)
			{
				case "ONESELF":
				case "YOU":
				case "YOURSELF":
				case "YOURSELVES":
				case "ME":
				case "MYSELF":
				case "US":
				case "OURSELVES":
				case "OURSELF":
				case "THEM":
				case "THEMSELVES":
					return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,node)});

				case "IT":
				case "ITSELF":
				case"MINE":
				case"OURS":
				case"YOURS":
				case"HERS":
				case"THEIRS":
					return new ArrayList( new WordSense [] {new WordSense("THING","N",3,node)});

				case "HER":
				case "HERSELF":
					return new ArrayList( new WordSense [] {new WordSense("WOMAN","N",1,node)});

				case "HIM":
				case "HIMSELF":
					return new ArrayList( new WordSense [] {new WordSense("MAN","N",1,node)});

				default:
					throw new Exception("Invalid OPP");
			}
		}
		
		private ArrayList Get_main_NP(ParseTree tree, ParseNode node)
		{
			switch (((ParseNode)node.Children[0]).Goal)
			{
				case "IDFP":
					return Get_IDFP(tree,(ParseNode)node.Children[0]);
				case "DEMP":
					return Get_DEMP(tree,(ParseNode)node.Children[0]);
				case "NPP":
					return Get_main_NPP(tree,(ParseNode)node.Children[0]);
				case "OPP":
					return Get_main_OPP(tree,(ParseNode)node.Children[0]);
				case "RXPN":
					return Get_main_OPP(tree,(ParseNode)node.Children[0]);
				case "EACHO":
				case"ONEAN":
					return new ArrayList( new WordSense [] {new WordSense("ONE","N",2,(ParseNode)node.Children[0])});
			}
			node.visited=true;
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
			string s=GetWordString(tree,term);
			switch(term.Goal)
			{
				case "PPN" :
					return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,term)});
				case "VING" :
				{
					bool iscmp=((ParseNode)n.Children[n.Children.Count-1]).Goal=="CMPS";
					if(iscmp && !term.visited && term.Senses.Count>1)
					{
						term.visited=true;
						DisabiguteTree(tree,n);
					}
					return new ArrayList( new WordSense [] {new WordSense(s,"V",-1,term)});
				}
				default :
					return new ArrayList( new WordSense [] {new WordSense(s,"N",-1,term)});
			}
		}

		private ArrayList Get_main_NC(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			if(((ParseNode)node.Children[0]).Goal=="SOR")
				return Get_main_DS(tree,(ParseNode)node.Children[1]);
			else
			{
				ArrayList mains=new ArrayList();
				mains.AddRange(Get_main_RPN(tree,(ParseNode)node.Children[0]));
				mains.AddRange(Get_main_PRD(tree,(ParseNode)node.Children[1]));
				return mains;
			}
			
		}
		
		private ArrayList Get_main_RPN(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			string s=GetWordString(tree,node);
			ArrayList V=new ArrayList();
			ParseNode n=node;
			if(node.Children!=null)
			{
				n=(ParseNode)node.Children[node.Children.Count-1];
				s=GetWordString(tree,(ParseNode)n.Children[0]);
			}
			switch(s)
			{
				case"WHICHEVER":
				case"WHICH":
				case"THAT":
					if(n.Children==null)
						V.Add(new WordSense("ONE","N",2,n));
					else
						V.Add(new WordSense("ONE","N",2,(ParseNode)n.Children[0]));
					break;
				case"WHO":
				case"WHOES":
				case"WHOEVER":
				case"WHOMEVER":
				case"WHOM":
					if(n.Children==null)
						V.Add(new WordSense("PERSON","N",1,n));
					else
						V.Add(new WordSense("PERSON","N",1,(ParseNode)n.Children[0]));
					break;
				case"WHAT":
				case"WHATEVER":
					if(n.Children==null)
						V.Add(new WordSense("THING","N",3,n));
					else
						V.Add(new WordSense("THING","N",3,(ParseNode)n.Children[0]));
					break;
			}
			if(n!=null && n.Children!=null && n.Children.Count>1)
				V.AddRange(Get_main_NP(tree,(ParseNode)n.Children[1]));
			return V;
		}

		private ArrayList Get_main_CMADJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs=new ArrayList();
			foreach(ParseNode n in node.Children)
				if(n.Goal=="PADJ" || n.Goal=="CADJ" || n.Goal=="SADJ")
					MainOfObjs.Add( new WordSense(GetWordString(tree,n),"A",-1,n));
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

		private ArrayList Get_main_CMPAJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mians = new ArrayList();
			((ParseNode)node.Children[0]).visited=true;
			switch(((ParseNode)node.Children[0]).Goal)
			{
				case"CADJ":
				case"CPADJ":
					string s=GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"A",-1,(ParseNode)node.Children[0]));
					return Mians;
				case"CMADJ":
					return Get_main_CMADJ(tree,((ParseNode)node.Children[0]));
				case"VPSP":
					s=GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"V",-1,(ParseNode)node.Children[0]));
					return Mians;
				default:
					throw new Exception("Invalid CMPAJ");
			}
		}

		private ArrayList Get_main_CMPAV(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mians = new ArrayList();
			((ParseNode)node.Children[0]).visited=true;
			switch(((ParseNode)node.Children[0]).Goal)
			{
				case"PADV":
					string s=GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"R",-1,(ParseNode)node.Children[0]));
					return Mians;
				case"CMADV":
					return Get_main_CMADV(tree,((ParseNode)node.Children[0]));
				case"ADVC":
					return Get_main_ADVC(tree,((ParseNode)node.Children[0]));
				default:
					throw new Exception("Invalid CMPAV");
			}
		}

		private ArrayList Get_main_FAVJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			return Get_main_FADJS(tree,((ParseNode)node.Children[node.Children.Count-1]));
		}

		private ArrayList Get_main_ADV(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mians= new ArrayList();
			((ParseNode)node.Children[0]).visited=true;
			switch(((ParseNode)node.Children[0]).Goal)
			{
				case"ADVC":
					return Get_main_ADVC(tree,((ParseNode)node.Children[0]));
				case"INFPH":
					return Get_main_INFPH(tree,((ParseNode)node.Children[0]));
				case"PRP":
					return Get_main_PRPS(tree,((ParseNode)node.Children[0]));
				case"PADV":
					string s=GetWordString(tree,(ParseNode)node.Children[0]);
					Mians.Add(new WordSense (s,"R",-1,(ParseNode)node.Children[0]));
					return Mians;
				case"CMADV":
					return Get_main_CMADV(tree,((ParseNode)node.Children[0]));
				default:
					throw new Exception("Invalid ADV");
			}
		}

		private ArrayList Get_main_IMS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList Mains = new ArrayList();
			ParseNode Chi = (ParseNode)node.Children[0];
			if(Chi.Goal!="VINF")
				Chi = (ParseNode)node.Children[1];
			string s=GetWordString(tree,Chi);
			Mains.Add(new WordSense (s,"V",-1,Chi));
			return Mains;
		}

		private ArrayList Get_main_FADJS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="FADJ")
			{
				MainOfObjs.AddRange(Get_main_FADJ(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_FADJS(tree,n));
			return MainOfObjs;
		}

		private ArrayList Get_main_CMADV(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs=new ArrayList();
			foreach(ParseNode n in node.Children)
				if(n.Goal=="PADV")
					MainOfObjs.Add( new WordSense(GetWordString(tree,n),"R",-1,node));
				else if(n.Goal=="CADJ" || n.Goal=="SADJ")
					MainOfObjs.Add( new WordSense(GetWordString(tree,n),"A",-1,node));
			return MainOfObjs;
		}

		private ArrayList Get_main_BADJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ParseNode Child = (ParseNode)node.Children[0];
			string s=GetWordString(tree,Child);
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

		private ArrayList Get_main_FADJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ParseNode Chi;
			if(node.Children.Count>1)
				Chi = (ParseNode)node.Children[1];
			else
				Chi = (ParseNode)node.Children[0];
			switch(Chi.Goal)
			{
				case "PRPH":
					return Get_main_PRPH(tree,Chi);
				case "PRPS":
					return Get_main_PRPS(tree,Chi);
				case "ADJC":
					return Get_main_ADJC(tree,Chi);
				case "ABPH":
					return Get_main_ABPH(tree,Chi);
				default:
					throw new Exception("Invalid FADJ");
			}
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

		private ArrayList Get_main_DS(ParseTree tree, ParseNode node)
		{
			if(!node.visited)
			{
				node.visited=true;
				DisabiguteTree(tree,node);
			}
			ArrayList DS =new ArrayList();
			ArrayList PRDs=GetPRDS(tree,(ParseNode)node.Children[1]);
			foreach(ParseNode n in PRDs)
			{
				ParseNode nn=(ParseNode)n.Children[0];
				if(nn.Goal=="PADV")
					nn=(ParseNode)n.Children[1];
				if(nn.Goal=="V")
				{
					string s=GetWordString(tree,nn);
					DS.Add(new WordSense(s,"V",-1,nn));
				}
				else
				{
					string s=GetWordString(tree,(ParseNode)nn.Children[nn.Children.Count-1]);
					DS.Add(new WordSense(s,"V",-1,(ParseNode)nn.Children[nn.Children.Count-1]));
				}
			}
			return DS;
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
/*
		private ArrayList Get_main_PRDS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRD")
			{
				MainOfObjs.AddRange(Get_main_PRD(tree,node).GetRange(0,1));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_PRDS(tree,n));
			return MainOfObjs;
		}
*/
		public static string GetWordString(ParseTree tree,ParseNode node)
		{
			string str=(string)tree.Words[node.Start];
			for(int i=node.Start+1;i<node.End;i++)
				str+="_"+(string)tree.Words[i];
			return str;
		}
		
		private ArrayList Get_main_INS(ParseTree tree, ParseNode node)
		{
			ParseNode n1=(ParseNode)node.Children[node.Children.Count-2];
			if(n1.Goal=="PRDS"||n1.Goal=="IPRDS"||n1.Goal=="PPRDS"||n1.Goal=="GPRDS"||
				n1.Goal=="SPRDS")
				return Get_main_PRD(tree,n1);
			else if(n1.Goal=="DS")
				return Get_main_DS(tree,n1);
			else if(n1.Goal=="SBJ")
				return Get_main_SBJ(tree,n1);
			else
				return Get_main_SBJ(tree,(ParseNode)node.Children[node.Children.Count-3]);
		}

		private ArrayList Get_main_SS(ParseTree tree, ParseNode node)
		{
			ParseNode n=(ParseNode)node.Children[0];
			if(n.Goal=="IMS")
				return Get_main_IMS(tree,n);
			else
				return Get_main_DS(tree,n);
		}

		private ArrayList Get_main_CS(ParseTree tree, ParseNode node)
		{
			ArrayList V=new ArrayList();
			foreach(ParseNode n in node.Children)
			{
				if(n.Children==null)
					continue;
				if(n.Goal=="IMS")
					V.AddRange(Get_main_IMS(tree,n));
				else if(n.Goal=="DS")
					V.AddRange(Get_main_DS(tree,n));
				else if(n.Goal=="INS")
					V.AddRange(Get_main_INS(tree,n));
				else
					V.AddRange(Get_main_SS(tree,n));
			}
			return V;
		}

		private ArrayList Get_main_CNP(ParseTree tree, ParseNode node)
		{
			string word=GetWordString(tree,node);
			if(word.EndsWith("'S"))
				word=word.Remove(word.Length-2,2);
			else
				word=word.Remove(word.Length-1,1);
			WnLexicon.WordInfo wordinfo = 
				WnLexicon.Lexicon.FindWordInfo(word,tagged_only,true,true);
			if(wordinfo.senseCounts!=null && wordinfo.senseCounts[1]>0)
				return new ArrayList( new WordSense [] {new WordSense(word,"N",-1,node)});
			else
				return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,node)});
		}

		private ArrayList Get_main_PRD(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList V=new ArrayList();
			ParseNode ch=(ParseNode)node.Children[0];
			if(ch.Goal=="PADV")
				ch=(ParseNode)node.Children[1];
			if(ch.Goal=="V"||ch.Goal=="VINF"||ch.Goal=="VPSP")
			{
				string s=GetWordString(tree,ch);
				V.Add(new WordSense(s,"V",-1,ch));
			}
			else if(ch.Goal=="VP"||ch.Goal=="PVP"||
				ch.Goal=="IVP"||ch.Goal=="GVP"||ch.Goal=="SVP")
			{
				string s=GetWordString(tree,(ParseNode)ch.Children[ch.Children.Count-1]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)ch.Children[ch.Children.Count-1]));
			}
			ParseNode last=(ParseNode)node.Children[node.Children.Count-1];
			if(last.Goal=="CMPS")
				V.AddRange(Get_main_CMPS(tree,last));
			return V;
		}

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
				case "PRPS":
					return Get_main_PRPS(tree,(ParseNode)node.Children[0]);
				case "NC":
					return Get_main_NC(tree,(ParseNode)node.Children[0]);
				case "NP":
					return Get_main_NP(tree,(ParseNode)node.Children[0]);
				case "INFPH":
					return Get_main_INFPH(tree,(ParseNode)node.Children[0]);
				default:
					throw new Exception("Invalid SSBJ");
			}
		}
		
		private ArrayList Get_main_NPP(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			switch(GetWordString(tree,node))
			{
				case"I":
				case"YOU":
				case"WE":
				case"THEY":
					return new ArrayList( new WordSense [] {new WordSense("PERSON","N",1,node)});
				case"HE":
					return new ArrayList( new WordSense [] {new WordSense("MAN","N",1,node)});
				case"SHE":
					return new ArrayList( new WordSense [] {new WordSense("WOMAN","N",1,node)});
				case"IT":
					return new ArrayList( new WordSense [] {new WordSense("THING","N",3,node)});
				default:
					throw new Exception("Invalid NPP");
			}
		}

		private ArrayList Get_main_INFPH(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList V=new ArrayList();
			ParseNode vnode=(ParseNode)node.Children[node.Children.Count-1];
			if(vnode.Goal=="VINF")
			{
				string str=GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			else
			{
				vnode=(ParseNode)node.Children[node.Children.Count-2];
				if(!vnode.visited && vnode.Senses.Count>1)
				{
					vnode.visited=true;
					DisabiguteTree(tree,node);
				}
				string str=GetWordString(tree,vnode);
				V.Add(new WordSense(str,"V",-1,vnode));
			}
			return V;
		}
		
		private ArrayList Get_main_CMPS(ParseTree tree, ParseNode node)
		{
			//eslam
			node.visited=true;
			ArrayList Cmps=VerbSense.GetCMPList(node);
			ArrayList Mains = new ArrayList();
			foreach(ParseNode n in Cmps)
			{
				n.visited=true;
				ParseNode ch=(ParseNode)n.Children[0];
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
					default:
						break;
				}
			}
			return Mains;
		}

		private ArrayList Get_main_CMPAVJ(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			if(((ParseNode)node.Children[0]).Goal=="CMPAJS")
				return Get_main_CMPAJS(tree,(ParseNode)node.Children[0]);
			else
				return new ArrayList();
		}
		
		private ArrayList Get_main_CMPAJS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
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
		
		private ArrayList Get_main_VP(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			string s=GetWordString(tree,(ParseNode)node.Children[node.Children.Count-1]);
			return new ArrayList( new WordSense [] {new WordSense(s,"V",-1,(ParseNode)node.Children[node.Children.Count-1])});
		}
/*
		private ArrayList Get_main_GPRDS(ParseTree tree, ParseNode node)
		{
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="GPRD")
			{
				MainOfObjs.AddRange(Get_main_GPRD(tree,node));
				return MainOfObjs;
			}
			if(node.Goal=="PPRD")
			{
				MainOfObjs.AddRange(Get_main_PPRD(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_GPRDS(tree,n));
			return MainOfObjs;
		}

		private ArrayList Get_main_GPRD(ParseTree tree, ParseNode node)
		{
			ArrayList V=new ArrayList();
			if(((ParseNode)node.Children[0]).Goal=="GVP")
			{
				ParseNode n=(ParseNode)node.Children[0];
				string s=GetWordString(tree,(ParseNode)n.Children[n.Children.Count-1]);
				V.Add(new WordSense(s,"V",-1,(ParseNode)n.Children[n.Children.Count-1]));
			}
			if(node.Children.Count==2)
				V.AddRange(Get_main_CMPS(tree,(ParseNode)node.Children[1]));
			return V;
		}
*/
		
		private ArrayList Get_main_PRPHS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRPH")
			{
				MainOfObjs.AddRange(Get_main_PRPH(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_PRPHS(tree,n));
			return MainOfObjs;
		}

		private ArrayList Get_main_CMPAVS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="CMPAV")
			{
				MainOfObjs.AddRange(Get_main_CMPAV(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_CMPAVS(tree,n));
			return MainOfObjs;
		}
		
		private ArrayList Get_main_ADVS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="ADV")
			{
				MainOfObjs.AddRange(Get_main_ADV(tree,node));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_ADVS(tree,n));
			return MainOfObjs;
		}
		
		private ArrayList Get_main_LPRPS(ParseTree tree, ParseNode node)
		{
			node.visited=true;
			ArrayList MainOfObjs = new ArrayList();
			if(node.Goal=="PRP")
			{
				MainOfObjs.AddRange(Get_main_OBJ(tree,(ParseNode)node.Children[1]));
				return MainOfObjs;
			}
			foreach(ParseNode n in node.Children)
				if(n.Children!=null)
					MainOfObjs.AddRange(Get_main_LPRPS(tree,n));
			return MainOfObjs;
		}
		
//		public ArrayList Get_main_CS(ParseTree tree,ParseNode node)
//		{
//
//		}
		
		private int count=0;
		private double score=0;

		public void Disambigute(ParseTree ps)
		{
			count=0;
			score=0;
			DisabiguteTree(ps,ps.Root);
			ps.Count=count;
			ps.Score=score;
		}

		public void DisabiguteTree(ParseTree tree,ParseNode node)
		{
			switch(node.Goal)
			{
				case"S":
				{
					ArrayList PRDW=null;
					foreach(ParseNode n in node.Children )
						if(n.Goal=="CS")
						{
							DisabiguteTree(tree,n);
							PRDW=Get_main_CS(tree,n);
						}
					foreach(ParseNode n in node.Children )
						if(n.Goal=="ABPH")
						{
							ArrayList SbjW=Get_main_ABPH(tree,n);
							DisabiguteWords(PRDW,SbjW);
							DisabiguteTree(tree,n);
						}
						else if(n.Goal=="PRPHS")
						{
							ArrayList SbjW=Get_main_PRPHS(tree,n);
							DisabiguteWords(PRDW,SbjW);
							DisabiguteTree(tree,n);
						}
						else if(n.Goal=="ADVS")
						{
							ArrayList SbjW=Get_main_ADVS(tree,n);
							DisabiguteWords(PRDW,SbjW);
							DisabiguteTree(tree,n);
						}
						else if(n.Goal=="PRPS")
						{
							ArrayList SbjW=Get_main_PRPS(tree,n);
							DisabiguteWords(PRDW,SbjW);
							DisabiguteTree(tree,n);
						}
						else if(n.Goal=="NPF")
						{
							ArrayList SbjW=Get_main_NP(tree,n);
							DisabiguteWords(PRDW,SbjW);
							DisabiguteTree(tree,n);
						}

				}break;
				case"CS":
				{
					foreach(ParseNode n in node.Children )
						if(n.Goal=="SS"||n.Goal=="DS"||n.Goal=="IMS"||n.Goal=="INS")
							DisabiguteTree(tree,n);
				}break;
				case"SS":
				{
					DisabiguteTree(tree,(ParseNode)node.Children[0]);
				}break;
				case"DS":
				{
					ArrayList PRDS=GetPRDS(tree,(ParseNode)node.Children[1]);
					ArrayList SbjW=Get_main_SBJ(tree,(ParseNode)node.Children[0]);
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
					foreach(ParseNode n in PRDS)
						DisabiguteTree(tree,n);
					DisabiguteTree(tree,(ParseNode)node.Children[0]);
				}break;
				case"ABPH":
				{
					ArrayList APHS=Get_main_ABPH(tree,node);
					if(!node.visited)
						DisabiguteWords(APHS);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="AVJ")
							DisabiguteWords(APHS.GetRange(0,1),Get_main_AVJ(tree,n));						
					if(((ParseNode)node.Children[0]).Goal=="NP")
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
					DisabiguteTree(tree,(ParseNode)node.Children[node.Children.Count-1]);
				}break;

				case"CMPS":
				case"CMP":
				case"CMPAVJ":
				case"SSBJ":
				case"SOBJ":
				case"FADJ":
				case"ADV":
				
				case"PRDS":
				case"IPRDS":
				case"GPRDS":
				case"SPRDS":
				case"PPRDS":
				case"PRPHS":
				case"BADJS":
				case"FADJS":
				case"CMPAVS":
				case"CMPAJS":
				case"LPRPS":
				case"ADVS":
				case"PRPS":
				case"OBJ":
				case"SBJ":
				
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
					DisabiguteTree(tree,(ParseNode)node.Children[1]);
				}break;
				case"NC":
				{
					ParseNode n=(ParseNode)node.Children[0];
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
					}
				}break;
				case"ADJC":
				{
					if(!node.visited)
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
				}break;
				case"RPN":
				{
					DisabiguteTree(tree,(ParseNode)node.Children[node.Children.Count-1]);
				}break;
				case"DRPN":
				{
					if(node.Children.Count==1)
						return;
					if(!node.visited)
					{
						ArrayList SbjW=Get_main_RPN(tree,(ParseNode)node.Children[0]);
						ParseNode n=(ParseNode)node.Children[1];
						ArrayList PRDW=Get_main_NP(tree,n);
						DisabiguteWords(PRDW,SbjW);
					}
					DisabiguteTree(tree,(ParseNode)node.Children[1]);
				}break;
				case"NNC":
				case"NN":
				{
					if(node.Children.Count==1)
						return;
					ArrayList r=Get_main_NN(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
				}break;
				case"PRPH":
				{
					if(node.Children.Count==1)
						return;
					ArrayList r=Get_main_PRPH(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
				}break;
				case"BADJ":
					return;
				case"INFPH":
				{
					if(node.Children.Count==2)
						return;
					ArrayList r=Get_main_INFPH(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
				}break;
				case"INFPO":
				{
					if(node.Children.Count==1)
						return;
					ArrayList r=Get_main_INFPH(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
				}break;
				case"NPADJ":
				case"NPF":
				case"NPC":
				case"NP":
				{
					ArrayList r=Get_main_NP(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="AVJ")
							s.AddRange(Get_main_AVJ(tree,n));
						else if(n.Goal=="FAVJ")
							s.AddRange(Get_main_FAVJ(tree,n));
						else if(n.Goal=="CNP")
							s.AddRange(Get_main_CNP(tree,n));
					DisabiguteWords(r,s);
					foreach(ParseNode n in node.Children)
						if(n.Children!=null)
							DisabiguteTree(tree,n);
				}break;
				case"AVJ":
				{
					if(node.Children.Count==1)
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
					else
					{
						ArrayList SbjW=Get_main_ADVS(tree,(ParseNode)node.Children[0]);
						ParseNode n=(ParseNode)node.Children[1];
						ArrayList PRDW=Get_main_BADJS(tree,n);
						DisabiguteWords(PRDW,SbjW);
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
						DisabiguteTree(tree,(ParseNode)node.Children[1]);
					}
				}break;
				case"CMPAJ":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal=="CMADJ")
						DisabiguteTree(tree,n);
				}break;
				case"CMPAV":
				{
					ParseNode n=(ParseNode)node.Children[0];
					if(n.Goal!="PADV")
						DisabiguteTree(tree,n);
				}break;
				case"FAVJ":
				{
					if(node.Children.Count==1)
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
					else
					{
						ArrayList SbjW=Get_main_ADVS(tree,(ParseNode)node.Children[0]);
						ParseNode n=(ParseNode)node.Children[1];
						ArrayList PRDW=Get_main_FADJS(tree,n);
						DisabiguteWords(PRDW,SbjW);
						DisabiguteTree(tree,(ParseNode)node.Children[0]);
						DisabiguteTree(tree,(ParseNode)node.Children[1]);
					}
				}break;
				case"IVP":
				case"GVP":
				case"PVP":
				case"SVP":
				case"VP":
				{
					ArrayList r=Get_main_VP(tree,node);
					ArrayList s=new ArrayList();
					foreach(ParseNode n in node.Children)
						if(n.Goal=="ADVS")
							s.AddRange(Get_main_ADVS(tree,n));
					DisabiguteWords(r,s);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="ADVS")
							DisabiguteTree(tree,n);
				}break;
				case"PRD":
				case"IPRD":
				case"PPRD":
				case"GPRD":
				case"SPRD":
				{
					ParseNode ch=(ParseNode)node.Children[0];
					if(ch.Goal=="PADV")
					{
						ch=(ParseNode)node.Children[1];
						if(ch.Goal=="VP"||ch.Goal=="IVP"||
							ch.Goal=="PVP"||ch.Goal=="GVP"||ch.Goal=="SVP")
							DisabiguteTree(tree,ch);
						ArrayList verb=Get_main_PRD(tree,node).GetRange(0,1);
						ArrayList adv=Get_main_ADV(tree,node);
						DisabiguteWords(verb,adv);
					}
					else if(ch.Goal=="VP"||ch.Goal=="IVP"||
						ch.Goal=="PVP"||ch.Goal=="GVP"||ch.Goal=="SVP")
						DisabiguteTree(tree,ch);
					ParseNode last=(ParseNode)node.Children[node.Children.Count-1];
					if(last.Goal=="CMPS")
					{
						ArrayList verb=Get_main_PRD(tree,node).GetRange(0,1);
						ArrayList Cmps=VerbSense.GetCMPList(last);
						foreach(ParseNode cmp in Cmps)
						{
							ParseNode n=(ParseNode)cmp.Children[0];
							if(n.Goal=="LPRPS")
							{
								ArrayList CmpW=Get_main_LPRPS(tree,n);
								DisabiguteWords(verb,CmpW);
							}
							else if(n.Goal=="CMPAVJ")
							{
								ParseNode ch2=(ParseNode)n.Children[0];
								if(ch2.Goal=="CMPAVS")
								{
									ArrayList CmpW=Get_main_CMPAVS(tree,ch2);
									DisabiguteWords(verb,CmpW);
								}
							}
						}
						DisabiguteTree(tree,last);
					}
				}break;
				case"IMS":
				{
					ArrayList r=Get_main_IMS(tree,node);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
							DisabiguteWords(r,Get_main_ADVS(tree,n));
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS"||n.Goal=="ADVS")
							DisabiguteTree(tree,n);
				}break;
				case"CMADJ":
				{
					ArrayList r=Get_main_CMADJ(tree,node);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
							DisabiguteTree(tree,n);
				}break;
				case"CMADV":
				{
					ArrayList r=Get_main_CMADV(tree,node);
					foreach(ParseNode n in node.Children)
						if(n.Goal=="CMPS")
						{
							ArrayList Cmps=VerbSense.GetCMPList(n);
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
				}break;
				case"INS":
				{
					int i=0;
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
							DisabiguteTree(tree,n);
				}break;
			}
		}
		/*
		public void ConvertMeasure()
		{
			control.Measure="WordNet::Similarity::wup";
			control.Config="../../../config/config-wup.conf";
			control.UpdateMeasure();
		}
		*/
		public bool IsHuman(string word)
		{
			return control.IsPerson(word);
		}

		private void DisabiguteWords(params ArrayList [] wordsenses)
		{
			ArrayList all_structs=new ArrayList();
			foreach(ArrayList list1 in wordsenses)
				all_structs.AddRange(list1);
			if(all_structs.Count<2)
				return;
			count+=all_structs.Count*(all_structs.Count-1)/2;
			ArrayList WordsInfo=new ArrayList();
			foreach(WordSense s in all_structs)
			{
				ArrayList wi=new ArrayList();
				if(s.SenNum==-1)
				{
					if(s.Node.Senses==null)
					{
						wi=GetSenses(s.POS,s.Word);
						
						if(wi.Count>1 &&
							control.Frequency((string)wi[0])>
							4*control.Frequency((string)wi[1]))
							wi=wi.GetRange(0,1);

					}
					else if(s.Node.Senses.Count==1)
						foreach(NodeSense ss in s.Node.Senses)
							wi.Add(ss.Sense);
					else
					{
						int x=GetSenses("V",s.Word).Count;
						int countx=0;
						foreach(NodeSense ss in s.Node.Senses)
							if(int.Parse(ss.Sense.Substring(
								ss.Sense.LastIndexOf('#')+1))<=x)
								countx++;
						if(countx>0)
							s.Node.Senses=s.Node.Senses.GetRange(0,countx);
						else
							s.Node.Senses=s.Node.Senses;
						foreach(NodeSense ss in s.Node.Senses)
							wi.Add(ss.Sense);
						
						if(wi.Count>1 &&
							control.Frequency((string)wi[0])>
							4*control.Frequency((string)wi[1]))
							wi=wi.GetRange(0,1);
							
					}
				}
				else
					wi.Add((s.Word+'#'+s.POS+'#'+s.SenNum.ToString()).ToLower());
				WordsInfo.Add(wi);
			}
			ArrayList results=new ArrayList();
			StringBuilder str=new StringBuilder("");
			// get result
			for(int i=0;i<WordsInfo.Count;i++)
			{
				ArrayList ws=(ArrayList)WordsInfo[i];
				str.Append("?-?-");
				foreach(string sen in ws)
					str.Append(sen+",");
				str[str.Length-1]='-';
				str.Append(((WordSense)all_structs[i]).POS.ToLower()+"|");
			}
			str.Remove(str.Length-1,1);
			string res=control.Disambigute(str.ToString());
			for(int i=0;i<WordsInfo.Count;i++)
			{
				int sep=res.IndexOf(':');
				results.Add(res.Substring(0,sep));
				res=res.Remove(0,sep+2);
			}
			score+=double.Parse(res);
			// end result;
			for(int i=0;i<results.Count;i++)
			{
				((WordSense)all_structs[i]).Node.Senses=new ArrayList();
				((WordSense)all_structs[i]).Node.Senses.Add(new NodeSense((string)results[i]));
			}
		}

	}
	struct WordSense
	{
		public string Word;
		public int SenNum;
		public ParseNode Node;
		public string POS;
		public WordSense(string word,string pos,int sn,ParseNode N)
		{
			Word=word;
			POS=pos;
			SenNum=sn;
			Node = N;
		}

	}
}
