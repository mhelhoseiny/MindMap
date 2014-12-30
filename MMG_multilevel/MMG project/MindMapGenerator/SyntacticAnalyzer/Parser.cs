using System;
using WnLexicon;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using SyntacticAnalyzer;

namespace SyntacticAnalyzer
{
    [Serializable]
	public class SentenceParser
	{
		RulesReader RR;
		BitArray[] RuleStarts;
		ArrayList[] EdgesByStart;
		ArrayList[] EdgesByEnd;
		ArrayList Words;
		bool tagged_only;
		int threshold;
		int percent;
        public SentenceParser()
        { }
		public SentenceParser(RulesReader rr,ArrayList words)
		{
			// Initialize Lexicon
            //if(w!=null)
            //{
            //    Lexicon.tagged_only=w.tagged_only;
            //    tagged_only=w.tagged_only;
            //    threshold=w.threshold;
            //    percent=w.percent;
            //}
            //else
            //{
            Lexicon.tagged_only = false;
				tagged_only=false;
				threshold=3;
				percent=30;
            //}
			Words=words;
			for(int i=0;i<words.Count;i++)
				words[i]=((string)words[i]).ToUpper();
			EdgesByStart=new ArrayList[words.Count];
			for(int i=0;i<EdgesByStart.Length;i++)
				EdgesByStart[i]=new ArrayList();
			EdgesByEnd=new ArrayList[words.Count+1];
			for(int i=0;i<EdgesByEnd.Length;i++)
				EdgesByEnd[i]=new ArrayList();
			RR=rr;
		}




		public ArrayList Parse()
		{
			Initialize();
			PredEdges=new Stack();
			PredEdges.Push(EdgesByStart[0][0]);
			Predict();
			while(PredEdges.Count>0)
			{
				Complete();
				Predict();
			}
			FinishParsing();
			ArrayList parseTrees = GetParseTrees();
			if(parseTrees!=null)
			{
                for (int i = parseTrees.Count - 1; i >= 0; i--)
                    if (!IsCorrectTree((ParseTree)parseTrees[i]))
                        parseTrees.RemoveAt(i);
                for (int index = 0; index < parseTrees.Count; index++)
                    if (!AdjustForCMP(parseTrees, (ParseTree)parseTrees[index], index))
                        parseTrees.RemoveAt(index--);
                //-------------------------------------
                //for (int i = 0; i < parseTrees.Count; i++)
                //    if (!FillVerbSenses((ParseTree)parseTrees[i], percent, threshold))
                //        parseTrees.RemoveAt(i--);
                //------------------------------------------
				double maxScore=-1000;
                for (int i = 0; i < parseTrees.Count; i++)
                {
                    LinkParents(((ParseTree)parseTrees[i]).Root);
                    double score = PutSyntaxScore((ParseTree)parseTrees[i]);
                    if (score > maxScore)
                        maxScore = score;
                }
                //Mohamed Elhoseiny comemnted at 03/16/2011 as it cause problems in ignorning the correct parse trees
                //for(int i=parseTrees.Count-1;i>=0;i--)
                //    if(((ParseTree)parseTrees[i]).Score<maxScore)
                //        parseTrees.RemoveAt(i);
                //    else
                //        ((ParseTree)parseTrees[i]).Score=0;

			}
			return parseTrees;
		}
        public static List<string> GetPOSString(ParseTree ptree)
        { 
            List<String> strArr = new List<string>();

            GetFilledPOsString(ptree.Root, strArr);
            return strArr;
        }

        private static void GetFilledPOsString(ParseNode parseNode, List<string> strArr)
        {
            if (parseNode.Children == null)
                strArr.Add(parseNode.Goal);
            else
            {
                for (int i = 0; i < parseNode.Children.Count; i++)
                {
                    GetFilledPOsString((ParseNode)parseNode.Children[i], strArr);
                }
            }
        }



		private double PutSyntaxScore(ParseTree ps)
		{
			ps.Score=50;
			ArrayList path=new ArrayList();
			TraverseTree(ps,path,ps.Root);
			return ps.Score;
		}

		public static string GetWordString(ParseTree tree,ParseNode node)
		{
			string str=(string)tree.Words[node.Start];
			for(int i=node.Start+1;i<node.End;i++)
				str+="_"+(string)tree.Words[i];
			return str;
		}

		private void TraverseTree(ParseTree ps,ArrayList path,ParseNode node)
		{
			//code here
			
			switch(node.Goal)
			{
				case"NC":
					if(IsGoalInPath("SBJ",null,path)&&
						IsGoalInPath("DS",null,path)&&
						IsGoalInPath("INS",null,path))
						ps.Score-=100;
					break;
					
				case "PRS":
					if(IsBetterForNP(GetWordString(ps,node)))
					{
						if(IsGoalInPath("FADJ","CMP",path))
							ps.Score+=10;
					}
					else
					{
						if(!IsGoalInPath("FADJ","CMP",path))
							ps.Score+=10;
					}
					break;
				case "VINF":
				case "VPSP":
				case "VING":
					if(node.Parent.Goal=="VP"||node.Parent.Goal=="IVP"||
						node.Parent.Goal=="SVP"||node.Parent.Goal=="PVP"||
						node.Parent.Goal=="GVP")
						ps.Score+=10;
					break;
				case "NP":
				case "NPC":
					ps.Score-=10;
					break;
			}
			//end
            if (node.Children != null)
            {
                path.Add(node);
                foreach (ParseNode child in node.Children)
                    TraverseTree(ps, path, child);
                path.RemoveAt(path.Count - 1);
            }
            else
            {
                ps.Score -= 10;
            }
			
		}

		private bool IsGoalInPath(string goal,string stop,ArrayList path)
		{
			for(int i=path.Count-1;i>=0;i--)
			{
				string CurrG=((ParseNode)path[i]).Goal;
				if(CurrG==goal)
					return true;
				else if(CurrG==stop)
					return false;
			}
			return false;
		}

		private bool IsBetterForNP(string prs)
		{
			return prs=="OF";
		}

		private bool ContainVerbs(ParseNode node)
		{
			if(node.Goal=="VING"||node.Goal=="VPSP"||node.Goal=="VINF"||
				node.Goal=="ADVC"||node.Goal=="ADJC"||node.Goal=="NC")
				return true;
			if(node.Children!=null)
				foreach(ParseNode ch in node.Children)
					if(ContainVerbs(ch))
						return true;
			return false;
		}

		private bool IsCorrectTree(ParseTree ps)
		{
			ParseNode first=(ParseNode)ps.Root.Children[0];
			if(first.Goal=="ADVS")
			{
				ParseNode advc=(ParseNode)((ParseNode)first.Children[0]).Children[0];
				if(advc.Goal=="ADVC")
					return false;
			}
			ArrayList CMPnodes=GetCMPNodes(ps.Root);
			foreach(ParseNode pn in CMPnodes)
			{
				ArrayList cmps=VerbSense.GetCMPList(pn);
				int no=0;
				bool verb=false;
				foreach(ParseNode cmp in cmps)
				{
					ParseNode fch=(ParseNode)cmp.Children[0];
					if(fch.Goal=="OBJ"||fch.Goal=="ADJ")
						no++;
					if(no>2||verb)
						return false;
					if(ContainVerbs(fch))
						verb=true;
				}
			}
			return true;
		}

		private bool FillVerbSenses(ParseTree ps,int percent,int threshold)
		{
			ArrayList CMPnodes=GetCMPNodes(ps.Root);
			CMPnodes.AddRange(GetSingleNodes(ps.Root));
			foreach(ParseNode pn in CMPnodes)
			{
				ParseNode vnode=GetVerbNode(pn);
				if(vnode==null)
					continue;
				vnode.Senses=new ArrayList();
				string v=(string)ps.Words[vnode.Start];
				for(int i=vnode.Start+1;i<vnode.End;i++)
					v+="_"+(string)ps.Words[i];
				ArrayList verbs=GetINFOfVerb(v);
				ParseNode cmpnode=(ParseNode)pn.Children[pn.Children.Count-1];
				foreach(string vb in verbs)
				{
					ArrayList senses=VerbSense.GetVerbSenses(vb,percent,threshold);
					for(int i=0;i<senses.Count;i++)
					{
						if(cmpnode.Goal=="CMPS")
						{
							if(((VerbSense)senses[i]).CheckValidity(ps,vnode,cmpnode))
								vnode.Senses.Add(senses[i]);
						}
						else
						{
							if(((VerbSense)senses[i]).CheckValidity(ps,vnode,null))
								vnode.Senses.Add(senses[i]);
						}
					}
				}
				if(vnode.Senses.Count==0)
					return false;
			}
			return true;
		}

		private bool AdjustForCMP(ArrayList trees,ParseTree ps,int index)
		{
			ArrayList CMPnodes=GetCMPNodes(ps.Root);
			foreach(ParseNode pn in CMPnodes)
			{
				ArrayList pt=ModifyTreeForCMPs(ref ps,pn);
				trees[index]=ps;
				if(pt==null)
					return false;
				trees.AddRange(pt);
			}
			return true;
		}

		private ArrayList ModifyTreeForCMPs(ref ParseTree ps,ParseNode pn)
		{
			ArrayList parsetrees=new ArrayList();
			ParseNode vnode=GetVerbNode(pn);
			if(vnode==null)
				return parsetrees;
			string v=(string)ps.Words[vnode.Start];
			ArrayList verbs=GetINFOfVerb(v);
			ArrayList starts=new ArrayList();
			bool prep=false;
			foreach(string vb in verbs)
			{
				int prevchilds=pn.Children.Count;
				ParseTree temp=(ParseTree)ps.Clone();
				ParseNode advnode=FindPVinCMPS(vb,ps.Words,
					pn,(ParseNode)pn.Children[pn.Children.Count-1],out prep);
				if(advnode!=null)
				{
					if(!starts.Contains(advnode.Start))
					{
						starts.Add(advnode.Start);
						ModifyVerb(ps,vnode,advnode);
						if(prevchilds==pn.Children.Count)
						{
							ShiftNodes((ParseNode)pn.Children[pn.Children.Count-1],
								advnode.Start,advnode.End);
						}
						parsetrees.Add(ps);
					}
					ps=temp;
				}
				else if(prep)
					return null;
			}
			if(prep)
			{
				ps=(ParseTree)parsetrees[0];
				parsetrees.RemoveAt(0);
			}
			return parsetrees;
		}

		private void ModifyVerb(ParseTree ps,ParseNode vnode,ParseNode advnode)
		{
			ArrayList advwords=new ArrayList();
			for(int i=advnode.Start;i<advnode.End;i++)
			{
				advwords.Add(ps.Words[i]);
				ps.Words.RemoveAt(i);
			}
			if(vnode.End<ps.Words.Count)
				ps.Words.InsertRange(vnode.End,advwords);
			else
				ps.Words.AddRange(advwords);
			vnode.End+=advwords.Count;
			if(vnode.Children!=null)
				((ParseNode)vnode.Children[0]).End+=advwords.Count;
		}

		public ArrayList GetINFOfVerb(string verb)
		{
			WordInfo vinfo=Lexicon.FindWordInfo(verb,tagged_only,true,true);
			ArrayList texts=vinfo.texts;
			for(int i=0;i<texts.Count;i++)
			{
				WordInfo wi = Lexicon.FindWordInfo((string)texts[i],tagged_only,false);
				if(wi.senseCounts[2]==0)
					texts.RemoveAt(i);
			}
			return texts;
		}

		private ParseNode GetVerbNode(ParseNode pn)
		{
			ParseNode v=(ParseNode)pn.Children[pn.Children.Count-1];
			if(v.Goal=="VP"||v.Goal=="IVP"||v.Goal=="SVP"||v.Goal=="PVP"||v.Goal=="GVP")
				return (ParseNode)v.Children[v.Children.Count-1];
			else if(v.Goal=="V"||v.Goal=="VING"||v.Goal=="VPSP"||v.Goal=="VINF")
				return v;
			else if(pn.Children.Count==4||pn.Children.Count==3)
				return (ParseNode)pn.Children[pn.Children.Count-2];
			else
			{
				ParseNode vchild=(ParseNode)pn.Children[pn.Children.Count-2];
				if(vchild.Goal=="VING"||vchild.Goal=="VINF"||vchild.Goal=="VPSP")
					return vchild;
				else if(pn.Goal=="PRD" && vchild.Goal=="V")
					return vchild;
				else
				{
					ParseNode tv=(ParseNode)vchild.Children[vchild.Children.Count-1];
					if(tv.Goal=="VINF"||tv.Goal=="VING"||tv.Goal=="VPSP")
						return tv;
					else
						return null;
				}
			}

		}

		private ParseNode FindPVinCMPS(string verb,
			ArrayList words,ParseNode parent,ParseNode pn,out bool prep)
		{
			ParseNode fchild=(ParseNode)pn.Children[0];
			prep=((ParseNode)fchild.Children[0]).Goal=="PRS";
			ParseNode retchild=null;
			if(IsADV(fchild)||prep)
			{
				string adv=(string)words[fchild.Start];
				for(int i=fchild.Start+1;i<fchild.End;i++)
					adv+="_"+(string)words[i];
				string phv=verb+"_"+adv;
				ArrayList cat1=Lexicon.GetCATs(adv);
				if(!prep || cat1==null || !cat1.Contains("PADV"))
				{
					ArrayList cats=Lexicon.GetCATs(phv);
					if(cats!=null&&cats.Contains("VINF"))
					{
						if(pn.Children.Count==2)
						{
							parent.Children[parent.Children.Count-1]=pn.Children[1];
							retchild=fchild;
						}
						else
						{
							parent.Children.RemoveAt(parent.Children.Count-1);
							retchild=fchild;
						}
					}
				}
			}
			if(prep && retchild==null)
				return null;
			if(pn.Children.Count==2)
			{
				bool temp=prep;
				ParseNode ret2=
					FindPVinCMPS(verb,words,pn,(ParseNode)pn.Children[1],out prep);
				if(prep)
					return null;
				prep=temp;
				if(retchild!=null)
					return retchild;
				else
					return ret2;
			}
			return retchild;
		}

		public static bool IsADV(ParseNode p)
		{
			
			if(p.Goal=="CMPAVS")
				return true;
			else if(p.Children==null||p.Children.Count>1)
				return false;
			else
				return IsADV((ParseNode)p.Children[0]);
			/*
			ParseNode fch=(ParseNode)p.Children[0];
			if(fch.Children==null)
				return false;
			return (((ParseNode)fch.Children[0]).Goal=="CMPAVS");
			*/
		}

		public static bool IsADJ(ParseNode p)
		{
			ParseNode fch=(ParseNode)p.Children[0];
			return (((ParseNode)fch.Children[0]).Goal=="CMPAJS");
		}

		private void ShiftNodes(ParseNode pn,int start,int end)
		{
			if(pn.Start<=start && pn.End>=end)
			{
				pn.Start+=end-start;
				if(pn.Children!=null)
					for(int i=0;i<pn.Children.Count;i++)
						ShiftNodes((ParseNode)pn.Children[i],start,end);
			}
			else if(pn.End<=start)
			{
				pn.Start+=end-start;
				pn.End+=end-start;
				if(pn.Children!=null)
					for(int i=0;i<pn.Children.Count;i++)
						ShiftNodes((ParseNode)pn.Children[i],start,end);
			}

		}

		private ArrayList GetCMPNodes(ParseNode pn)
		{
			ArrayList nodes=new ArrayList();
			if(pn.Children!=null && pn.Goal!="CMPS" && pn.Goal!="CMADJ" && pn.Goal!="CMADV" &&
				((ParseNode)pn.Children[pn.Children.Count-1]).Goal=="CMPS")
				nodes.Add(pn);
			if(pn.Children!=null)
				foreach(ParseNode cpn in pn.Children)
					nodes.AddRange(GetCMPNodes(cpn));
			return nodes;
		}

		private ArrayList GetSingleNodes(ParseNode pn)
		{
			ArrayList nodes=new ArrayList();
			if(pn.Children!=null &&
				pn.Goal!="VP"&&pn.Goal!="IVP"&&pn.Goal!="SVP"&&
				pn.Goal!="PVP"&&pn.Goal!="GVP"&&
				IsVerb((ParseNode)pn.Children[pn.Children.Count-1]))
				nodes.Add(pn);
			if(pn.Children!=null)
				foreach(ParseNode cpn in pn.Children)
					nodes.AddRange(GetSingleNodes(cpn));
			return nodes;
		}

		private bool IsVerb(ParseNode v)
		{
			return (v.Goal=="V"||v.Goal=="VING"||v.Goal=="VPSP"||v.Goal=="VINF"||
				v.Goal=="VP"||v.Goal=="IVP"||v.Goal=="SVP"||v.Goal=="PVP"||v.Goal=="GVP");
		}

		private ArrayList GetParseTrees()
		{
			ArrayList rootEdges=null;
			ArrayList parseRoots=null;
			ArrayList parseTrees=null;
			if(SearchEdge(0,Words.Count,Words.Count,"S'",out rootEdges))
				parseRoots=ParseTreesOf((Edge)rootEdges[0]);
			if(parseRoots==null)
				return null;
			parseTrees=new ArrayList();
			for(int i=0;i<parseRoots.Count;i++)
			{
				ParseNode root=(ParseNode)((ParseNode)parseRoots[i]).Children[0];
				LinkParents(root);
				parseTrees.Add(new ParseTree(root,(ArrayList)Words.Clone()));
			}
			return parseTrees;
		}

		public static void LinkParents(ParseNode root)
		{
			if(root.Children!=null)
			{
				foreach(ParseNode child in root.Children)
				{
					child.Parent=root;
					LinkParents(child);
				}
			}
		}

		private void FinishParsing()
		{
			EdgesByStart=new ArrayList[Words.Count];
			for(int i=0;i<EdgesByStart.Length;i++)
				EdgesByStart[i]=new ArrayList();
			for(int i=0;i<EdgesByEnd.Length;i++)
				foreach(Edge e in EdgesByEnd[i])
					if(e.Needed.Count==0 && IsNewEdge(e))
						EdgesByStart[e.Start].Add(e);
			EdgesByEnd=null;
		}

		private bool SearchEdge(int Start,int End,int Max,string Goal,out ArrayList Edges)
		{
			Edges=new ArrayList();
			foreach(Edge e in EdgesByStart[Start])
				if(e.Goal==Goal && ( e.End==End || End==-1 && e.End<=Max ) )
					Edges.Add(e);
			return Edges.Count>0;
		}

		private ArrayList ParseTreesOf(Edge root)
		{
			if(root==null)
				return null;
			ParseNode ps=new ParseNode();
			ps.Start=root.Start;
			ps.End=root.End;
			ps.Goal=root.Goal;
			if(root.Finished==null)
				return new ArrayList(new ParseNode[]{ps});
			ps.Children=new ArrayList();
			ArrayList trees=new ArrayList();
			trees.Add(ps);

			ArrayList childtrees=null;
			ArrayList edges=null;
			int chEnd=-1;
			int Max=0;
			if(root.Finished.Count==1)
				chEnd=root.End;
			else
				Max=root.End-1;
			SearchEdge(root.Start,chEnd,Max,(string)root.Finished[0],out edges);
			childtrees=new ArrayList();
			for(int i=0;i<edges.Count;i++)
				childtrees.AddRange(ParseTreesOf((Edge)edges[i]));
			ArrayList temptrees=new ArrayList();
			for(int i=0;i<childtrees.Count;i++)
			{
				for(int j=0;j<trees.Count;j++)
				{
					ParseNode pt=(ParseNode)((ParseNode)trees[j]).Clone();
					pt.Children.Add(childtrees[i]);
					temptrees.Add(pt);
				}
			}
			trees=temptrees;

			//-----------------------

			for(int ch=1;ch<root.Finished.Count;ch++)
			{
				temptrees=new ArrayList();
				ArrayList trcopy=(ArrayList)trees.Clone();
				for(int tr=0;tr<trees.Count;tr++)
				{
					childtrees=null;
					edges=null;
					int chStart=((ParseNode)((ParseNode)trees[tr]).Children[ch-1]).End;
					chEnd=-1;
					Max=0;
					if(ch==root.Finished.Count-1)
						chEnd=root.End;
					else
						Max=root.End-1;
					SearchEdge(chStart,chEnd,Max,(string)root.Finished[ch],out edges);

					childtrees=new ArrayList();
					for(int i=0;i<edges.Count;i++)
						childtrees.AddRange(ParseTreesOf((Edge)edges[i]));
					for(int i=0;i<childtrees.Count;i++)
					{
						ParseNode pt=(ParseNode)((ParseNode)trees[tr]).Clone();
						pt.Children.Add(childtrees[i]);
						temptrees.Add(pt);
					}
				}
				trees=temptrees;
			}

			return trees;
		}

		private void Initialize()
		{
            int limit = 4;
			Edge edge = new Edge();
			edge.Start=edge.End=0;
			edge.Goal="S'";
			edge.Needed=new ArrayList(new string[] {"S"});
			edge.Finished=new ArrayList(0);
			AddEdge(edge);
			Hashtable FoundCATs = new Hashtable();
			ArrayList CurrEdges=null;
			for(int i=0;i<Words.Count;i++)
			{
				CurrEdges=new ArrayList();
				for(int j=1;j+i<=Words.Count;j++)
				{
					string word=(string)Words[i];
					for(int k=1;k<j;k++)
						word+="_"+Words[i+k];
					ArrayList CATs=Lexicon.GetCATs(word);
					if(j>1 && CATs.Contains("VINF"))
						CATs.Remove("VINF");
                    if (j > limit)//(CATs==null || CATs.Count==0) )//&&  ) 
						break;
                    /*
                    else if (CATs != null && CATs.Count != 0)
						CurrEdges.Clear();
                    */
					foreach(string cat in CATs)
					{
						edge=new Edge();
						edge.Start=i;
						edge.End=i+j;
						edge.Goal=cat;
						edge.Needed=new ArrayList(0);
						CurrEdges.Add(edge);
					}
				}
				foreach(Edge e in CurrEdges)
				{
					AddEdge(e);
					if(!FoundCATs.Contains(e.Goal))
						FoundCATs.Add(e.Goal,null);
				}
			}
			RR=RR.Minimize(Lexicon.GetDiff(FoundCATs),2);
			RuleStarts=new BitArray[RR.Rules.Count];
			for(int i=0;i<RuleStarts.Length;i++)
				RuleStarts[i]=new BitArray(Words.Count,false);
		}

		private void Predict()
		{
			NoEdges=0;
			ArrayList Indices=null;
			Edge edge=null;
			while(PredEdges.Count>0)
			{
				while(PredEdges.Count>0)
				{
					edge=(Edge)PredEdges.Pop();
					Indices=RR.GetRules((string)edge.Needed[0]);
					if(Indices!=null)
						break;
				}
				if(Indices==null)
					return;
				if(RuleStarts[(int)Indices[0]][edge.End])
					continue;
				RuleStarts[(int)Indices[0]][edge.End]=true;
				foreach(int I in Indices)
				{
					Rule R=(Rule)RR.Rules[I];
					Edge e = new Edge();
					e.Start=edge.End;
					e.End=e.Start;
                    e.Goal = R.LHS;
					e.Finished=new ArrayList(0);
					e.Needed=R.RHS;
					AddEdge(e);
					PredEdges.Push(e);
				}
				NoEdges=Indices.Count;
				return;
			}
		}

        Stack PredEdges;
        int NoEdges = 0;

		private void Complete()
		{
            ArrayList CompEdges = new ArrayList(PredEdges);
            CompEdges = CompEdges.GetRange(0, NoEdges);
			while(CompEdges.Count>0)
			{
				ArrayList NewEdges = new ArrayList();
				foreach(Edge src in CompEdges)
				{
                    if (src.Needed.Count > 0 && src.End < Words.Count)
                    {
                        foreach (Edge dst in EdgesByStart[src.End])
                        {
                            if ((string)src.Needed[0] == dst.Goal && dst.Needed.Count == 0)
                            {
                                Edge e = MakeEdge(src, dst);
                                NewEdges.Add(e);
                                if (e.Needed.Count > 0 && e.End < Words.Count)
                                {
                                    PredEdges.Push(e);
                                }
                            }
                        }
                    }
                    else if (src.Needed.Count == 0)
                    {
                        foreach (Edge dst in EdgesByEnd[src.Start])
                        {
                            if (dst.Needed.Count > 0 && (string)dst.Needed[0] == src.Goal)
                            {
                                Edge e = MakeEdge(dst, src);
                                NewEdges.Add(e);
                                if (e.Needed.Count > 0 && e.End < Words.Count)
                                {
                                    PredEdges.Push(e);
                                }
                            }
                        }
                    }
				}
				foreach(Edge e in NewEdges)
					AddEdge(e);
                CompEdges = NewEdges;
			}
		}

		private bool IsNewEdge(Edge e)
		{
            bool added = false;
			foreach(Edge pedge in EdgesByStart[e.Start])
			{
                if (pedge.End == e.End &&
                    pedge.Goal == e.Goal &&
                    pedge.Finished == null && e.Finished == null)
                {
                    added = true;
                }
                else if (pedge.End == e.End &&
                    pedge.Goal == e.Goal &&
                    pedge.Finished == null || e.Finished == null)
                {
                    added = false;
                }
                else if (pedge.End == e.End &&
                    pedge.Goal == e.Goal &&
                    pedge.Needed.Count == e.Needed.Count &&
                    pedge.Finished.Count == e.Finished.Count)
                {
                    added = true;
                    for (int i = 0; i < e.Needed.Count; i++)
                        if ((string)pedge.Needed[i] != (string)e.Needed[i])
                            added = false;
                    for (int i = 0; i < e.Finished.Count; i++)
                        if ((string)pedge.Finished[i] != (string)e.Finished[i])
                            added = false;
                }
				if(added)
					break;
			}
			return !added;
		}
		
		private Edge MakeEdge(Edge src,Edge dst)
		{
			Edge e = new Edge();
            e.Start = src.Start;
            e.End = dst.End;
            e.Goal = src.Goal;
            e.Finished = (ArrayList)src.Finished.Clone();
			e.Finished.Add(dst.Goal);
            e.Needed = (ArrayList)src.Needed.Clone();
			e.Needed.RemoveAt(0);
			return e;
		}

		private void AddEdge(Edge e)
		{
			EdgesByStart[e.Start].Add(e);
			EdgesByEnd[e.End].Add(e);
		}

        //public ArrayList GetINFOfVerb(string p)
        //{
        //    WordInfo vinfo = Lexicon.FindWordInfo(verb, tagged_only, true, true);
        //    ArrayList texts = vinfo.texts;
        //    for (int i = 0; i < texts.Count; i++)
        //    {
        //        WordInfo wi = Lexicon.FindWordInfo((string)texts[i], tagged_only, false);
        //        if (wi.senseCounts[2] == 0)
        //            texts.RemoveAt(i);
        //    }
        //    return texts;
        //}
    }
    [Serializable]
	public class Edge
	{
		public int Start;
		public int End;
		public string Goal;
		public ArrayList Finished;
		public ArrayList Needed;
	}

    [Serializable]
	public class ParseNode : ICloneable
	{
		public int Start;
		public int End;
		public string Goal;
		public ArrayList Children;
		public ArrayList Senses;
		public ParseNode Parent;
		public bool visited=false;
		public bool counted=false;
		//internal OntoSem.Concept concept;
        public ParseNode ReferedAnaphoraNode;
        public string Text;
        public String Sense;
        public int SenseNo;
      
		public object Clone()
		{
			ParseNode pt = new ParseNode();
            pt.Start = Start;
            pt.End = End;
            pt.Goal = Goal;
            pt.Sense = Sense;
            pt.Senses = Senses;
            pt.SenseNo = SenseNo;
            pt.ReferedAnaphoraNode = ReferedAnaphoraNode;
            if (Children != null)
            {
                pt.Children = new ArrayList();
                foreach (ParseNode node in Children)
                    pt.Children.Add(node.Clone());
            }
			return pt;
		}
	}

    [Serializable]
	public class ParseTree : ICloneable
	{
		public ParseNode Root;
		public ArrayList Words;
        public ArrayList Senses;
        public List<string> ResolvedAnaphora= new List<string>();
        public double Score;
		public int Count;
		
		public ParseTree(ParseNode root,ArrayList words)
		{
			Root=root;
			Words=words;
		}

		public object Clone()
		{
            ParseNode roots = (ParseNode)Root.Clone();
            ArrayList words = new ArrayList();
			foreach(string w in Words)
				words.Add(w);
            return new ParseTree(roots, words);
		}

	}

}
