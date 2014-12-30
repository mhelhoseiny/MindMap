using System;
using Wnlib;
using System.Collections;

namespace SyntacticAnalyzer
{
	public class SyntaxStruct
	{
		public VerbSense.SyntaxChecker Check;
		public string Input = null;
	}

	public class NodeSense
	{
		public NodeSense(){}
		public NodeSense(string s)
		{
			Sense=s;
		}
		public string Sense;
	}

	public class VerbSense : NodeSense
	{
		public ArrayList Structs;

		public bool CheckValidity(ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			foreach(SyntaxStruct ss in Structs)
				if(ss.Check(ss.Input,ps,vnode,pn))
					return true;					
			return false;
		}

		public static ArrayList GetCMPList(ParseNode pn)
		{
			ArrayList cmps=new ArrayList();
			ParseNode fchild=(ParseNode)pn.Children[0];
			if(fchild.Goal=="CMP")
				cmps.Add(fchild);

            // we add here Fatima,May,Marwa
            if (fchild.Goal == "PRDS")
                cmps.Add(fchild);

			if(pn.Children.Count==2)
				cmps.AddRange(GetCMPList((ParseNode)pn.Children[1]));
			return cmps;
		}

		public delegate bool SyntaxChecker(string Input,ParseTree ps,ParseNode vnode,ParseNode pn);
		static bool CheckForm1(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			// No OBJ
			if(pn==null)
				return true;
			ArrayList cmps=GetCMPList(pn);
			foreach(ParseNode p in cmps)
			{
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		static bool CheckForm4(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ is ----ing PP
			//We don't know if adverbs or prep. phrases can come after
			ArrayList cmps=GetCMPList(pn);
			if(vnode.Goal=="VING")
			{
				if(!IsVPSP(ps,(ParseNode)cmps[0]))
					return false;
			}
			else
				return false;
			for(int i=1;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		
		static bool CheckForm22(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ---s PP
			//We don't know if adverbs or prep. phrases can come after
			ArrayList cmps=GetCMPList(pn);
			if(!IsVPSP(ps,(ParseNode)cmps[0]))
				return false;
			for(int i=1;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}

		static bool IsVPSP(ParseTree ps,ParseNode node)
		{
			bool nc=true;
			bool Chek = false;
			foreach(ParseNode Child in node.Children)
			{
				if(Child.Children == null)
				{
					if(Child.Goal == "VPSP")
						return true;
					Chek = (Child.Goal == "ETH")||
						(Child.Goal == "OR")||
						(Child.Goal == "NTH")||
						(Child.Goal == "NOR")||
						(Child.Goal == "NOTN")||
						(Child.Goal == "BUTN")||
						(Child.Goal == "BTH")||
						(Child.Goal == "AND")||
						(Child.Goal == "WTH")||
						(Child.Goal == "CMA")||
						(Child.Goal == "COR");
					return Chek;
				}
				else
					nc&=IsVPSP(ps,Child);
			}
			return nc;
		}

		static bool IsVING(ParseTree ps,ParseNode node)
		{
			bool nc=true;
			bool Chek = false;
			foreach(ParseNode Child in node.Children)
			{
				if(Child.Children == null)
				{
					if(Child.Goal == "VING")
						return true;
					Chek = (Child.Goal == "ETH")||
						(Child.Goal == "OR")||
						(Child.Goal == "NTH")||
						(Child.Goal == "NOR")||
						(Child.Goal == "NOTN")||
						(Child.Goal == "BUTN")||
						(Child.Goal == "BTH")||
						(Child.Goal == "AND")||
						(Child.Goal == "WTH")||
						(Child.Goal == "CMA")||
						(Child.Goal == "COR");
					return Chek;
				}
				else
					nc&=IsVING(ps,Child);
			}
			return nc;
		}

		/*static bool IsVPSP(ParseNode cmp)
		{
			// we don't know if two vpsp can come
			ParseNode avjch=(ParseNode)cmp.Children[0];
			ParseNode ajsch=(ParseNode)avjch.Children[0];
			if(ajsch.Goal=="CMPAJS")
			{
				ParseNode ajch=(ParseNode)ajsch.Children[0];
				if(ajch.Goal=="CMPAJ")
					return ((ParseNode)ajch.Children[0]).Goal=="VPSP";
			}
			return false;
		}*/
		

		// Syntax structure follows the verb immediately
		// Any ADJ can be preceeded by some adverbs
		// Prepositional phrases can't come before or within
		// syntax structure.

		static bool CheckForm5(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+ADJ
			//SBJ ----s OBJ+OBJ
			ArrayList cmps=GetCMPList(pn);
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ" || fcmp.Children.Count>1)
				return false;
			int i=1;
			for(;i<cmps.Count;i++)
				if(!SentenceParser.IsADV((ParseNode)cmps[i]))
					break;
			if(i>=cmps.Count)
				return false;
			ParseNode ch2=(ParseNode)cmps[i];
			if(i>1 && ((ParseNode)ch2.Children[0]).Goal=="OBJ")
				return false;
			if(!SentenceParser.IsADJ(ch2) && ((ParseNode)ch2.Children[0]).Goal!="OBJ")
				return false;
			i++;
			for(;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}

		static bool CheckForm6(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s ADJ
			//SBJ ----s OBJ
			ArrayList cmps=GetCMPList(pn);
			return CheckForm7(Input,ps,vnode,pn)||CheckForm8(Input,ps,vnode,pn);
		}
		static bool CheckForm7(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s ADJ
			ArrayList cmps=GetCMPList(pn);
			int i=0;
			for(;i<cmps.Count;i++)
				if(!SentenceParser.IsADV((ParseNode)cmps[i]))
					break;
			if(i>=cmps.Count)
				return false;
			ParseNode ch2=(ParseNode)cmps[i];
			if(!SentenceParser.IsADJ(ch2))
				return false;
			i++;
			for(;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		static bool CheckForm8(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ
			ArrayList cmps=GetCMPList(pn);
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			for(int i=1;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		static bool CheckForm12(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s Preposition+OBJ
			ArrayList cmps=GetCMPList(pn);
			int i=0;
			for(;i<cmps.Count;i++)
				if(!SentenceParser.IsADV((ParseNode)cmps[i]))
					break;
			if(i>=cmps.Count)
				return false;
			ParseNode prplnode=(ParseNode)((ParseNode)cmps[i]).Children[0];
			if(prplnode.Goal!="PRPL")
				return false;
			ArrayList preps=GetPrepositions(ps,prplnode);
			foreach(string prs in preps)
				if(prs!=Input)
					return false;
			i++;
			for(;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		
		static bool CheckForm14(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+OBJ
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count<2)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			ParseNode scmp=(ParseNode)cmps[1];
			if(scmp.Children==null)
				return false;
			ParseNode sch=(ParseNode)scmp.Children[0];
			if(sch.Goal!="OBJ")
				return false;
			for(int i=2;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		static bool CheckForm15(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+Preposition+OBJ
			ArrayList cmps=GetCMPList(pn);
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			int i=1;
			for(;i<cmps.Count;i++)
				if(!SentenceParser.IsADV((ParseNode)cmps[i]))
					break;
			if(i>=cmps.Count)
				return false;
			ParseNode prplnode=(ParseNode)((ParseNode)cmps[i]).Children[0];
			if(prplnode.Goal!="PRPL")
				return false;
			ArrayList preps=GetPrepositions(ps,prplnode);
			foreach(string prs in preps)
				if(prs!=Input)
					return false;
			i++;
			for(;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}

		static ArrayList GetPrepositions(ParseTree ps,ParseNode PRPLNode)
		{
			ArrayList Prepositions = new ArrayList();
			if(PRPLNode.Children != null)
			{
				foreach(ParseNode Child in PRPLNode.Children)
				{
					if(Child.Goal == "PRP")
					{
						ParseNode prs=(ParseNode)Child.Children[0];
						string str=(string)ps.Words[prs.Start];
						for(int i=prs.Start+1;i<prs.End;i++)
							str+="_"+ps.Words[i];
						Prepositions.Add(str);
					}
					else
						Prepositions.AddRange(VerbSense.GetPrepositions(ps,Child));
				}
			}
			return Prepositions;
		}

		static bool IsValidNP(ParseNode np)
		{
			ParseNode avj=null;
			int i=0;
			for(;i<np.Children.Count;i++)
			{
				ParseNode p=(ParseNode)np.Children[i];
				if(p.Goal=="AVJ")
				{
					avj=p;
					break;
				}
			}
			if(avj==null)
				return false;
			if(((ParseNode)np.Children[i+1]).Goal!="N")
				return false;
			if(i!=0 && ((ParseNode)np.Children[i-1]).Goal!="ARC")
				return false;
			if(avj.Children.Count>1)
				return false;
			return IsVING(null,(ParseNode)avj.Children[0]);
		}

		static bool GetNPs(ParseTree ps,ParseNode objNode)
		{
			bool np=true;
			if(objNode.Children != null)
			{
				foreach(ParseNode Child in objNode.Children)
				{
					if(Child.Goal == "SOBJ")
					{
						ParseNode temp=(ParseNode)Child.Children[0];
						if(temp.Goal!="NP" || !IsValidNP(temp))
							return false;
					}
					else
						np&=VerbSense.GetNPs(ps,Child);
				}
			}
			return np;
		}

		static bool IsIntoPhrase(ParseTree ps,ParseNode node)
		{
			bool nc=true;
			if(node.Goal == "PRP")
			{
				ParseNode prs=(ParseNode)node.Children[0];
				string str=(string)ps.Words[prs.Start];
				for(int i=prs.Start+1;i<prs.End;i++)
					str+="_"+ps.Words[i];
				if(str!="INTO")
					return false;
				ParseNode obj=(ParseNode)node.Children[1];
				if(!GetNPs(ps,obj))
					return false;
				return true;
			}
			bool Chek = false;
			foreach(ParseNode Child in node.Children)
			{
				if(Child.Children == null)
				{
					Chek = (Child.Goal == "ETH")||
						(Child.Goal == "OR")||
						(Child.Goal == "NTH")||
						(Child.Goal == "NOR")||
						(Child.Goal == "NOTN")||
						(Child.Goal == "BUTN")||
						(Child.Goal == "BTH")||
						(Child.Goal == "AND")||
						(Child.Goal == "WTH")||
						(Child.Goal == "CMA")||
						(Child.Goal == "COR");
					return Chek;
				}
				else
					nc&=IsIntoPhrase(ps,Child);
			}
			return nc;
		}

		static bool IsNC(ParseTree ps,ParseNode node)
		{
			bool nc=true;
			bool Chek = false;
			foreach(ParseNode Child in node.Children)
			{
				if(Child.Goal == "NC")
					return true;
				if(Child.Children == null)
				{
					Chek = (Child.Goal == "ETH")||
						(Child.Goal == "OR")||
						(Child.Goal == "NTH")||
						(Child.Goal == "NOR")||
						(Child.Goal == "NOTN")||
						(Child.Goal == "BUTN")||
						(Child.Goal == "BTH")||
						(Child.Goal == "AND")||
						(Child.Goal == "WTH")||
						(Child.Goal == "CMA")||
						(Child.Goal == "COR");
					return Chek;
				}
				else
					nc&=IsNC(ps,Child);
			}
			return nc;
		}
		
		static bool CheckForm20(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+VPSP
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count<2)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			if(!IsVPSP(ps,(ParseNode)cmps[1]))
				return false;
			for(int i=2;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}
		static bool CheckForm24(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+TO+VINF
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count!=2)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			ParseNode scmp=(ParseNode)cmps[1];
			if(((ParseNode)scmp.Children[0]).Goal!="INFPH")
				return false;
			return true;
		}
		static bool CheckForm25(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+VINF
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count!=2)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			ParseNode scmp=(ParseNode)cmps[1];
			if(((ParseNode)scmp.Children[0]).Goal!="INFPO")
				return false;
			return true;
		}
		static bool CheckForm26(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s that SS
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count>1)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			if(!IsNC(ps,fcmp))
				return false;
			return true;
		}
		static bool CheckForm32(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s VINF
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count>1)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="INFPO")
				return false;
			return true;
		}
		static bool CheckForm28(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s TO+VINF
			ArrayList cmps=GetCMPList(pn);
			if(cmps.Count>1)
				return false;
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="INFPH")
				return false;
			return true;
		}
		static bool CheckForm29(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s WETHER+VINF
			return VerbSense.CheckForm26(Input,ps,vnode,pn);
		}
		static bool CheckForm30(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s OBJ+INTO+VING+OBJ
			ArrayList cmps=GetCMPList(pn);
			ParseNode fcmp=(ParseNode)cmps[0];
			ParseNode fch=(ParseNode)fcmp.Children[0];
			if(fch.Goal!="OBJ")
				return false;
			int i=1;
			for(;i<cmps.Count;i++)
				if(!SentenceParser.IsADV((ParseNode)cmps[i]))
					break;
			if(i>=cmps.Count)
				return false;
			ParseNode prplnode=(ParseNode)((ParseNode)cmps[i]).Children[0];
			if(prplnode.Goal!="PRPL")
				return false;
			if(!IsIntoPhrase(ps,prplnode))
				return false;
			return true;
		}
		static bool CheckForm33(string Input,ParseTree ps,ParseNode vnode,ParseNode pn)
		{
			if(pn==null)
				return false;
			//SBJ ----s VING
			ArrayList cmps=GetCMPList(pn);
			ParseNode fcmp=(ParseNode)cmps[0];
			if(!IsVING(ps,fcmp))
				return false;
			for(int i=1;i<cmps.Count;i++)
			{
				ParseNode p=(ParseNode)cmps[i];
				string goal=((ParseNode)p.Children[0]).Goal;
				if(goal=="OBJ"||goal=="INFPH"||goal=="INFPO"||SentenceParser.IsADJ(p))
					return false;
			}
			return true;
		}

		public static ArrayList GetVerbSenses(string verb,int percent,int threshold)
		{
			WordNetClasses.WN wnc = new WordNetClasses.WN(Wnlib.WNCommon.path);
			bool b=false;
			SearchSet boj=null;
			ArrayList list=new ArrayList();
			wnc.OverviewFor(verb,"verb",ref b,ref boj,list);
			Search vs=(Search)list[0];
			ArrayList senses=new ArrayList();
			int cut=(int)Math.Ceiling(vs.senses.Count*percent/100.0);
			if(cut<threshold)
				cut=threshold;
			foreach(SynSet ss in vs.senses)
			{
				/*
				if(--cut<0)
					break;
					*/
				VerbSense vsense=new VerbSense();
				vsense.Sense=verb+"#v#"+(ss.sense+1).ToString();
				vsense.Structs=new ArrayList();
				foreach(SynSetFrame frame in ss.frames)
				{
					SyntaxStruct st = new SyntaxStruct();
					switch(frame.fr.str)
					{
						case "1":
							st.Check = new SyntaxChecker(VerbSense.CheckForm1);
							break;
						case "2":
							st.Check = new SyntaxChecker(VerbSense.CheckForm1);
							break;
						case "3":
							st.Check = new SyntaxChecker(VerbSense.CheckForm1);
							break;
						case "4":
							st.Check = new SyntaxChecker(VerbSense.CheckForm4);
							break;
						case "5":
							st.Check = new SyntaxChecker(VerbSense.CheckForm5);
							break;
						case "6":
							st.Check = new SyntaxChecker(VerbSense.CheckForm6);
							break;
						case "7":
							st.Check = new SyntaxChecker(VerbSense.CheckForm7);
							break;
						case "8":
							st.Check = new SyntaxChecker(VerbSense.CheckForm8);
							break;
						case "9":
							st.Check = new SyntaxChecker(VerbSense.CheckForm8);
							break;
						case "10":
							st.Check = new SyntaxChecker(VerbSense.CheckForm8);
							break;
						case "11":
							st.Check = new SyntaxChecker(VerbSense.CheckForm8);
							break;
						case "12":
							st.Check = new SyntaxChecker (VerbSense.CheckForm12);
							st.Input = "TO";
							break;
						case "13":
							st.Check = new SyntaxChecker (VerbSense.CheckForm12);
							st.Input = "ON";
							break;
						case "14":
							st.Check = new SyntaxChecker(CheckForm14);
							break;
						case "15":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "TO";
							break;
						case "16":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "FROM";
							break;
						case "17":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "WITH";
							break;
						case "18":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "OF";
							break;
						case "19":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "ON";
							break;
						case "20":
							st.Check = new SyntaxChecker(VerbSense.CheckForm20);
							break;
						case "21":
							st.Check = new SyntaxChecker(VerbSense.CheckForm20);
							break;
						case "22":
							st.Check = new SyntaxChecker(VerbSense.CheckForm22);
							break;
						case "23":
							st.Check = new SyntaxChecker(VerbSense.CheckForm1);
							break;
						case "24":
							st.Check = new SyntaxChecker(VerbSense.CheckForm24);
							break;
						case "25":
							st.Check = new SyntaxChecker(VerbSense.CheckForm25);
							break;
						case "26":
							st.Check = new SyntaxChecker(VerbSense.CheckForm26);
							break;
						case "27":
							st.Check = new SyntaxChecker(VerbSense.CheckForm12);
							break;
						case "28":
							st.Check = new SyntaxChecker(VerbSense.CheckForm28);
							break;
						case "29":
							st.Check = new SyntaxChecker(VerbSense.CheckForm29);
							break;
						case "30":
							st.Check = new SyntaxChecker(VerbSense.CheckForm30);
							break;
						case "31":
							st.Check = new SyntaxChecker(VerbSense.CheckForm15);
							st.Input = "WITH";
							break;
						case "32":
							st.Check = new SyntaxChecker(VerbSense.CheckForm32);
							break;
						case "33":
							st.Check = new SyntaxChecker(VerbSense.CheckForm33);
							break;
						case "34":
							st.Check = new SyntaxChecker(VerbSense.CheckForm26);
							break;
						case "35":
							st.Check = new SyntaxChecker(VerbSense.CheckForm32);
							break;
					}
					vsense.Structs.Add(st);
				}
				senses.Add(vsense);
			}
			return senses;
		}

	}

}
