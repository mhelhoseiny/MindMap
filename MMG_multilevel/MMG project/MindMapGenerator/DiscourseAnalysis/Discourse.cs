using System;
using System.Collections;
using WnLexicon;
using SyntacticAnalyzer;
using System.Collections;

namespace MMG
{
	/// <summary>
	/// Summary description for Discourse.
	/// </summary>
	public class Discourse
	{
		public Discourse(//NewWSD Wsd
            )
		{
			//New this.wsd=Wsd;
		//	wsd.ConvertMeasure();
		}

        public ArrayList NewSParseTrees;
        public ArrayList SParseTrees;

        
        public Discourse(ArrayList SPT)
        {
            SParseTrees = SPT;
        }

		static string[] cogverbs={"ANTICIPATED","APPEARS","ASSUMED","BELIEVED","EXPECTED",
							"FOLLOWS","KNOWN","MEANS","RECOMMENDED","SEEMS","THOUGHT"};

		static string[] modaladjs={"ADVISABLE","BEST","BETTER","CERTAIN","CONVENIENT",
			"DESIRABLE","DIFFICULT","EASY","EASIER","EASIEST","ECONOMICAL","GOOD",
			"IMPORTANT","LEGAL","LIKELY","NECESSARY","POSSIBLE","SUFFICIENT","USEFUL"};

        bool IsCognitive(string verb)
        {
            return Array.BinarySearch(cogverbs, verb) >= 0;
        }

        bool IsModal(string adj)
        {
            return Array.BinarySearch(modaladjs, adj) >= 0;
        }

        private bool IsPleonasticPronoun(ParseNode Itnode, ParseTree ps)
        {
            ParseNode ds = GetDSifSbj(Itnode);
            if (ds == null)
                return false;
            ParseNode prd = null;
            ParseNode verb = GetVerb(ds, out prd);
            if (!IsCognitive(GetWordString(ps, verb)))
                return false;
            ParseNode cmp = (ParseNode)prd.Children[prd.Children.Count - 1];
            if (cmp.Goal != "CMPS")
                return false;
            while (cmp.Goal != "NC")
                if (cmp.Children != null)
                    cmp = (ParseNode)cmp.Children[0];
                else
                    return false;
            cmp = (ParseNode)cmp.Children[0];
            return cmp.Goal == "SOR" && GetWordString(ps, cmp) == "THAT";
        }

        private ParseNode GetVerb(ParseNode node, out ParseNode predicate)
        {
            ParseNode child = (ParseNode)node.Children[1];
            while (child.Goal != "PRD")
                child = (ParseNode)child.Children[0];
            predicate = child;
            child = (ParseNode)child.Children[0];
            if (child.Goal == "PADV")
                child = (ParseNode)child.Parent.Children[1];
            if (child.Goal == "V")
                return child;
            if (child.Goal == "VING")
                return child;

            else
                return (ParseNode)child.Children[child.Children.Count - 1];
        }

        private ParseNode GetDSifSbj(ParseNode child)
        {
            bool sbj = false;
            ParseNode parent = child.Parent;
            while (parent != null)
            {
                sbj |= parent.Goal == "SBJ";
                if (parent.Goal == "DS")
                {
                    if (sbj)
                        return parent;
                    else
                        return null;
                }
                parent = parent.Parent;
            }
            return null;
        }

        private int GetScore(ParseNode node, ParseTree ps)
        {
            int score = 0;
            ParseNode obj = null;
            ParseNode cmps = null;
            bool bopj = false;
            if (IsHeadNoun(node.Parent))
                score += 80;
            if (IsSBJ(node))
                score += 80;
            if (IsOBJ(node, out obj, out cmps, out bopj))
            {
                if (IsDirectOBJ(obj, cmps))
                    score += 50;
                else
                    score += 40;
            }
            if (bopj)
                score += 40;
            if (!IsAdverbial(node))
                score += 50;
            if (IsExistential(node, ps))
                score += 70;
            return score;
        }

        private bool IsAdverbial(ParseNode child)
        {
            ParseNode parent = child;
            while (parent != null && parent.Goal != "ADV")
                parent = parent.Parent;
            return parent != null;
        }

        private static ArrayList GetCMPList(ParseNode pn)
        {
            ArrayList cmps = new ArrayList();
            ParseNode fchild = (ParseNode)pn.Children[0];
            if (fchild.Goal == "CMP")
                cmps.Add(fchild);
            if (pn.Children.Count == 2)
                cmps.AddRange(GetCMPList((ParseNode)pn.Children[1]));
            return cmps;
        }

        private bool IsDirectOBJ(ParseNode obj, ParseNode cmps)
        {
            bool isobj = false;
            ArrayList cmplist = GetCMPList(cmps);
            foreach (ParseNode cmp in cmplist)
            {
                ParseNode child = (ParseNode)cmp.Children[0];
                if (child == obj)
                    isobj = true;
                else if (child.Goal == "OBJ")
                    isobj = false;
            }
            return isobj;
        }

        private bool IsHeadNoun(ParseNode node)
        {
            ParseNode parent = node.Parent;
            while (parent != null)
            {
                if (parent.Goal == "NP" || parent.Goal == "NPC")
                    return false;
                parent = parent.Parent;
            }
            return true;
        }

        private bool IsOBJ(ParseNode child,
            out ParseNode obj, out ParseNode cmps, out bool bopj)
        {
            obj = null;
            cmps = null;
            bopj = false;
            if (!IsValidArgument(child))
                return false;
            ParseNode parent = child;
            while (parent != null && parent.Goal != "CMPS")
            {
                if (parent.Goal == "OBJ")
                    obj = parent;
                if (parent.Goal == "PRP")
                    bopj = true;
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return false;
            }
            if (parent == null || obj == null)
                return false;
            while (parent.Parent.Goal == "CMPS")
                parent = parent.Parent;
            cmps = parent;
            return true;
        }

        private bool IsExistential(ParseNode node, ParseTree ps)
        {
            ParseNode prd = null;
            ParseNode ds = GetDSifOBJ(node, out prd);
            if (ds == null)
                return false;
            ParseNode verb = GetVerb(ds, out prd);
            string v = GetWordString(ps, verb);
            if (v == "IS" || v == "ARE" || v == "WAS" || v == "WERE" || v == "EXIST" || v == "EXISTS")
                if (GetWordString(ps, (ParseNode)ds.Children[0]) == "THERE")
                    return true;
            return false;
        }

        private ParseNode GetDSifOBJ(ParseNode child, out ParseNode prd)
        {
            prd = null;
            bool obj = false;
            if (!IsValidArgument(child))
                return null;
            ParseNode parent = child;
            while (parent != null && parent.Goal != "DS")
            {
                if (parent.Goal == "PRD")
                    prd = parent;
                if (parent.Goal == "OBJ")
                    obj = true;
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return null;
            }
            if (obj)
                return parent;
            return null;
        }

        private bool IsSBJ(ParseNode child)
        {
            if (!IsValidArgument(child))
                return false;
            ParseNode parent = child;
            while (parent != null && parent.Goal != "SBJ")
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return false;
            return parent != null;
        }

        private bool IsWrongReferent(ParseNode ch, ParseTree ps)
        {
            string str = GetWordString(ps, ch);
            return str == "THERE";
        }

        public Hashtable EquivalenceClasses;
        public ArrayList DistinctClasses;

        public void Begin(ArrayList ParseTrees)
        {
            EquivalenceClasses = new Hashtable();
            DistinctClasses = new ArrayList();
            ArrayList PronounNPs = new ArrayList();
            ArrayList OtherNPs = new ArrayList();
            int tnum = 0;
            foreach (ParseTree ps in ParseTrees)
            {
                Stack nodes = new Stack();
                nodes.Push(ps.Root);
                while (nodes.Count > 0)
                {
                    ParseNode pn = (ParseNode)nodes.Pop();
                    if (pn.Goal == "CMADJ" || pn.Goal == "CMADV")
                        continue;
                    if (pn.Goal == "NP" || pn.Goal == "NPC" || pn.Goal == "NPF")
                    {
                        ParseNode fch = (ParseNode)pn.Children[0];
                        if (fch.Goal == "DEMP" || fch.Goal == "NPP" || fch.Goal == "OPP" ||
                            fch.Goal == "RXPN" || fch.Goal == "EACHO" || fch.Goal == "ONEAN")
                        {
                            string p = GetWordString(ps, fch);
                            if (fch.Goal == "NPP" && p == "IT" && IsPleonasticPronoun(fch, ps))
                                continue;
                            Agreement g = new Agreement(p, p, true//New ,wsd
                                );
                            DiscourseEntry de = new DiscourseEntry();
                            de.Ag = g;
                            de.Node = fch;
                            de.Score = 100 + GetScore(fch, ps);
                            de.TreeNum = tnum;
                            PronounNPs.Add(de);
                        }
                        else if (fch.Goal == "IDFP")
                        {
                            string p = GetWordString(ps, fch);
                            Agreement g = new Agreement(p, p, true//New ,wsd
                                );
                            DiscourseEntry de = new DiscourseEntry();
                            de.Ag = g;
                            de.Node = fch;
                            de.Score = 100 + GetScore(fch, ps);
                            de.TreeNum = tnum;
                            OtherNPs.Add(de);
                        }
                        else if (fch.Goal == "NP" || fch.Goal == "NPC")
                        {
                            continue;
                        }
                        else
                        {
                            foreach (ParseNode ch in pn.Children)
                                if (ch.Goal == "NN" || ch.Goal == "NNC")
                                {
                                    if (IsWrongReferent(ch, ps))
                                        continue;
                                    ParseNode term = (ParseNode)ch.Children[0];
                                    if (term.Goal == "ADVS")
                                        term = (ParseNode)ch.Children[1];
                                    string w = GetWordString(ps, term);
                                    //string p=((NodeSense)term.Senses[0]).Sense;
                                    string p = w + "#n#1";////////Sevre Bug

                                    Agreement g = new Agreement(p, w, false//New ,wsd
                                        );
                                    DiscourseEntry de = new DiscourseEntry();
                                    de.Ag = g;
                                    de.Node = ch;
                                    de.Score = 100 + GetScore(ch, ps);
                                    de.TreeNum = tnum;
                                    OtherNPs.Add(de);
                                }
                        }
                        if (fch.Goal == "PPJ" || fch.Goal == "REPC")
                        {
                            string p = GetWordString(ps, fch);
                            Agreement g = new Agreement(p, p, true//New ,wsd
                                );
                            DiscourseEntry de = new DiscourseEntry();
                            de.Ag = g;
                            de.Node = fch;
                            de.Score = 100 + GetScore(fch, ps);
                            de.TreeNum = tnum;
                            PronounNPs.Add(de);
                        }
                        if (fch.Goal == "ARC")
                            fch = (ParseNode)pn.Children[1];
                        if (fch.Goal == "CNP")
                        {

                            string p = "";
                            if (fch.Senses != null)
                                p = ((NodeSense)fch.Senses[0]).Sense;
                            string w = GetWordString(ps, fch);
                            if (w.EndsWith("'s"))
                                w = w.Remove(w.Length - 2, 2);
                            if (w.EndsWith("s'"))
                                w = w.Remove(w.Length - 1, 1);
                            Agreement g = new Agreement(p, w, false//New ,wsd
                                );
                            DiscourseEntry de = new DiscourseEntry();
                            de.Ag = g;
                            de.Node = fch;
                            de.Score = 100 + GetScore(fch, ps);
                            de.TreeNum = tnum;
                            OtherNPs.Add(de);
                        }
                    }
                    if (pn.Goal == "SBJ" || pn.Goal == "OBJ")
                    {
                        ParseNode fch = (ParseNode)pn.Children[0];
                        if (fch.Goal != "SSBJ" && fch.Goal != "SOBJ")
                        {
                            Agreement g = new Agreement(3, 0);
                            DiscourseEntry de = new DiscourseEntry();
                            de.Ag = g;
                            de.Node = pn;
                            de.Score = 100 + GetScore(pn, ps);
                            de.TreeNum = tnum;
                            OtherNPs.Add(de);
                        }
                    }
                    if (pn.Children == null)
                        continue;
                    foreach (ParseNode n in pn.Children)
                        nodes.Push(n);
                }
                tnum++;
                // choose and move
                for (int i = 0; i < PronounNPs.Count; i++)
                {
                    DiscourseEntry PrnEntry = (DiscourseEntry)PronounNPs[i];
                    ArrayList ResEntries = (ArrayList)OtherNPs.Clone();
                    if (i > 0)
                        ResEntries.AddRange(PronounNPs.GetRange(0, i));
                    if (PronounNPs.Count > i + 1)
                        ResEntries.AddRange(PronounNPs.GetRange(i + 1, PronounNPs.Count - i - 1));
                    int maxScore = -1000;
                    DiscourseEntry maxEntry = null;
                    foreach (DiscourseEntry de in ResEntries)
                    {
                        int currScore = de.Score;
                        if (IsCataphora(PrnEntry, de))
                            currScore -= 175;
                        if (IsRoleParallel(PrnEntry, de))
                            currScore += 35;
                        if (currScore >= maxScore && IsValidEntry(ps, PrnEntry, de) && 
                            (de.TreeNum < PrnEntry.TreeNum || ( de.TreeNum == PrnEntry.TreeNum && de.Node.Start < PrnEntry.Node.Start ) ) )
                        {
                            if (currScore > maxScore)
                            {
                                maxScore = currScore;
                                maxEntry = de;
                            }
                            else if(IsMoreNear(PrnEntry, de, maxEntry))
                            {
                                maxScore = currScore;
                                maxEntry = de;
                            }
                        }
                    }
                    if (maxEntry != null)
                    {
                        PrnEntry.Score += maxScore / 2;
                        // code to save results;
                        ArrayList items1 = (ArrayList)EquivalenceClasses[maxEntry];
                        ArrayList items2 = (ArrayList)EquivalenceClasses[PrnEntry];
                        if (items1 == null || items2 == null || items1 != items2)
                        {
                            ArrayList newitems = new ArrayList();
                            if (items1 != null)
                                DistinctClasses.Remove(items1);
                            else
                            {
                                items1 = new ArrayList();
                                items1.Add(maxEntry);
                            }
                            if (items2 != null)
                                DistinctClasses.Remove(items2);
                            else
                            {
                                items2 = new ArrayList();
                                items2.Add(PrnEntry);
                            }
                            foreach (DiscourseEntry di in items1)
                                items2.Remove(di);
                            newitems.AddRange(items1);
                            newitems.AddRange(items2);
                            DistinctClasses.Add(newitems);
                            foreach (DiscourseEntry di in newitems)
                                EquivalenceClasses[di] = newitems;
                        }
                        //
                        OtherNPs.Add(PrnEntry);
                        PronounNPs.RemoveAt(i);
                        i--;
                    }
                }
                //
                for (int i = OtherNPs.Count - 1; i >= 0; i--)
                {
                    DiscourseEntry de = (DiscourseEntry)OtherNPs[i];
                    de.Score /= 2;
                    if (de.Score <= 0 || tnum - de.TreeNum >= 4)
                        OtherNPs.RemoveAt(i);
                }
                foreach (DiscourseEntry de in PronounNPs)
                    de.Score /= 2;
            }

        }

        private bool IsValidEntry(ParseTree ps, DiscourseEntry prn, DiscourseEntry de)
        {
            if (prn.Ag.Person != 0 && de.Ag.Person != 0 && de.Ag.Person != prn.Ag.Person)
                return false;
            if (prn.Ag.Type != 0 && de.Ag.Type != 0 && de.Ag.Type != prn.Ag.Type)
                return false;
            bool reflexive = prn.Node.Goal == "RXPN" || prn.Node.Goal == "REPC" ||
                prn.Node.Goal == "EACHO" || prn.Node.Goal == "ONEAN";
            if (reflexive)
            {
                if (prn.TreeNum != de.TreeNum)
                    return false;
                if (prn.Node.Parent == de.Node.Parent)
                    return false;
                if (IsInNPDomain(prn, de))
                    return true;
                if (IsArgumentDomain(prn, de))
                    return true;
                if (IsAdjunctDomain(prn, de))
                    return true;
                if (IsCondition5(prn, de))
                    return true;
                if (IsCondition3(prn, de))
                    return true;
                return false;
            }
            else
            {
                if (prn.TreeNum != de.TreeNum)
                    return true;
                if (prn.Node.Parent == de.Node.Parent)
                    return false;
                if (IsInNPDomain(prn, de))
                    return false;
                if (IsInNPDomain2(prn, de))
                    return false;
                if (IsCondition4(prn, de))
                    return false;
                if (IsArgumentDomain(prn, de))
                    return false;
                if (IsAdjunctDomain(prn, de))
                    return false;
                if (IsWrongSBJ(prn, de))
                    return false;
                return true;
            }
        }

        private bool IsWrongSBJ(DiscourseEntry P, DiscourseEntry N)
        {
            if (N.Node.Parent.Goal == "NP" || N.Node.Parent.Goal == "NPC")
                if (FoundSBJ(P.Node, N.Node.Parent))
                    return true;
            if (P.Node.Parent.Goal == "NP" || P.Node.Parent.Goal == "NPC")
                if (FoundSBJ(N.Node, P.Node.Parent))
                    return true;
            return false;
        }

        private bool FoundSBJ(ParseNode child, ParseNode stop)
        {
            bool sbj = false;
            ParseNode parent = child.Parent;
            while (parent != null)
            {
                sbj |= parent.Goal == "SBJ";
                if (parent == stop)
                    return sbj;
                parent = parent.Parent;
            }
            return false;
        }

        private bool IsCondition3(DiscourseEntry P, DiscourseEntry N)
        {
            bool isprp;
            ParseNode CurrP = GetNPorPRP(P.Node.Parent, out isprp);
            if (isprp)
                CurrP = GetParentNP(CurrP);
            if (((ParseNode)CurrP.Children[0]).Goal == "CNP")
                return false;
            if (GetUpperHead(CurrP) == GetUpperHead(N.Node))
                return true;
            ParseNode prp = GetPrep(CurrP);
            if (prp == null)
                return false;
            ParseNode h1 = GetUpperHead(prp);
            return h1 != null && h1 == GetUpperHead(N.Node);
        }

        private ParseNode GetNPorPRP(ParseNode child, out bool prp)
        {
            prp = false;
            ParseNode parent = child.Parent;
            while (parent != null)
            {
                if (parent.Goal != "PRP")
                {
                    prp = true;
                    return parent;
                }
                else if (parent.Goal == "NP" || parent.Goal == "NPC")
                    return parent;
                else
                    parent = parent.Parent;
            }
            return parent;
        }

        private bool IsCondition5(DiscourseEntry P, DiscourseEntry N)
        {
            if (P.Node.Goal != "REPC" && P.Node.Goal != "PPJ")
                return false;
            if (GetUpperHead(P.Node.Parent) == GetUpperHead(N.Node))
                return true;
            ParseNode prp = GetPrep(P.Node);
            if (prp == null)
                return false;
            ParseNode h1 = GetUpperHead(prp);
            return h1 != null && h1 == GetUpperHead(N.Node);
        }

        private bool IsAdjunctDomain(DiscourseEntry P, DiscourseEntry N)
        {
            ParseNode prp = GetPrep(P.Node);
            if (prp == null)
                return false;
            ParseNode h1 = GetUpperHead(prp);
            return h1 != null && h1 == GetUpperHead(N.Node);
        }

        private ParseNode GetPrep(ParseNode child)
        {
            if (!IsValidArgument(child))
                return null;
            ParseNode parent = child.Parent;
            if (child.Goal == "OBJ")
            {
                parent = child;
                goto skip;
            }
            while (parent != null && parent.Goal != "OBJ")
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return null;
            if (parent == null)
                return null;
        skip:
            if (parent.Parent.Goal != "PRP")
                return null;
            else
                return parent.Parent;
        }

        private bool IsArgumentDomain(DiscourseEntry P, DiscourseEntry N)
        {
            ParseNode h1 = GetParentPRDorDS(P.Node);
            ParseNode h2 = GetParentPRDorDS(N.Node);
            if (h1 == h2)
                return true;
            else if (h1 == null || h2 == null)
                return false;
            else if (h1.Goal == "DS" && h1 == h2.Parent)
                return true;
            else if (h2.Goal == "DS" && h2 == h1.Parent)
                return true;
            return false;
        }

        private ParseNode GetParentPRDorDS(ParseNode child)
        {
            if (!IsValidArgument(child))
                return null;
            ParseNode parent = child.Parent;
            while (parent != null && parent.Goal != "DS" && parent.Goal != "PRD")
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return null;
            return parent;
        }

        private ParseNode GetUpperHead(ParseNode child)
        {
            ParseNode head = null;
            if (!IsValidArgument(child))
                return head;
            ParseNode parent = child.Parent;
            while (parent != null)
            {
                if (IsValidHead(parent))
                    head = parent;
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return head;
            }
            return head;
        }


        private bool IsCondition4(DiscourseEntry P, DiscourseEntry N)
        {
            if (N.Node.Goal == "PPJ" || N.Node.Goal == "REPC" || N.Node.Goal == "DEMP" ||
                N.Node.Goal == "NPP" || N.Node.Goal == "OPP" || N.Node.Goal == "RXPN" ||
                N.Node.Goal == "EACHO" || N.Node.Goal == "ONEAN")
                return false;
            ParseNode head = GetDirectHead(P.Node);
            if (head == null)
                return false;
            if (!IsContained(N.Node, head))
                return false;
            if (N.Node.Goal == "CNP" && N.Node.Start < P.Node.Start)
                return false;
            return true;
        }

        private ParseNode GetDirectHead(ParseNode child)
        {
            if (!IsValidArgument(child))
                return null;
            ParseNode parent = child.Parent;
            while (parent != null && !IsValidHead2(parent))
                if (IsValidArgPath(parent))
                    parent = parent.Parent;
                else
                    return null;
            return parent;
        }

        private bool IsValidArgPath(ParseNode node)
        {
            return !(node.Goal == "PRP" ||
                node.Goal == "CMPAVJ" || node.Goal == "AVJ" ||
                node.Goal == "FAVJ" || node.Goal == "NC");
        }

        private bool IsValidArgument(ParseNode node)
        {
            return node.Goal == "NN" || node.Goal == "NNC" || node.Goal == "DEMP" ||
                node.Goal == "NPP" || node.Goal == "OPP" || node.Goal == "RXPN" ||
                node.Goal == "EACHO" || node.Goal == "ONEAN" || node.Goal == "IDFP" ||
                node.Goal == "SBJ" || node.Goal == "OBJ" | node.Goal == "NP" ||
                node.Goal == "NPC" || node.Goal == "PRP";
        }

        private bool IsValidHead(ParseNode node)
        {
            return node.Goal == "NN" || node.Goal == "PRPH" || node.Goal == "INFPH" ||
                node.Goal == "INFPO" || node.Goal == "IMS" || node.Goal == "INS" ||
                node.Goal == "DS" || node.Goal == "NC" || node.Goal == "ADJC";
        }

        private bool IsValidHead2(ParseNode node)
        {
            return node.Goal == "NN" || node.Goal == "PRPH" || node.Goal == "INFPH" ||
                node.Goal == "INFPO" || node.Goal == "IMS" || node.Goal == "INS" ||
                node.Goal == "DS" || node.Goal == "NC" || node.Goal == "ADJC" ||
                node.Goal == "PRD" || node.Goal == "PPRD" || node.Goal == "SPRD" ||
                node.Goal == "IPRD" || node.Goal == "GPRD";
        }

        private bool IsInNPDomain2(DiscourseEntry P, DiscourseEntry N)
        {
            if (P.Node.Goal != "PPJ" && P.Node.Goal != "REPC")
                return false;
            return IsContained(N.Node, P.Node.Parent);
        }

        private bool IsContained(ParseNode child, ParseNode parent)
        {
            ParseNode currp = child.Parent;
            while (currp != null && currp != parent)
                currp = currp.Parent;
            return currp != null;
        }

        private bool IsInNPDomain(DiscourseEntry P, DiscourseEntry N)
        {
            if (N.Node.Goal != "CNP")
                return false;
            ParseNode Pnp = GetParentNP(P.Node);
            if (Pnp == null)
                return false;
            if (Pnp == N.Node.Parent)
                return true;
            ParseNode prp = GetParentPRP(Pnp, N.Node.Parent);
            if (prp == null)
                return false;
            Pnp = GetParentNP(prp);
            if (Pnp == null)
                return false;
            if (Pnp == N.Node.Parent)
                return true;
            return false;
        }

        private ParseNode GetParentPRP(ParseNode child, ParseNode stopNP)
        {
            ParseNode parent = child.Parent;
            while (parent != null && parent.Goal != "PRP")
                if (parent == stopNP)
                    return null;
                else
                    parent = parent.Parent;
            return parent;
        }

        private ParseNode GetParentNP(ParseNode child)
        {
            ParseNode parent = child.Parent;
            while (parent != null && parent.Goal != "NP" && parent.Goal != "NPC")
                parent = parent.Parent;
            return parent;
        }

        private bool IsCataphora(DiscourseEntry prn, DiscourseEntry de)
        {
            if (de.TreeNum > prn.TreeNum)
                return true;
            else if (de.TreeNum < prn.TreeNum)
                return false;
            else
                return de.Node.Start > prn.Node.Start;
        }

        private bool IsMoreNear(DiscourseEntry prn, DiscourseEntry de1, DiscourseEntry de2)
        {
            if (prn.TreeNum >= de1.TreeNum && prn.TreeNum >= de2.TreeNum)
            {
                if (prn.TreeNum - de1.TreeNum < prn.TreeNum - de2.TreeNum)
                    return true;
                else if (prn.TreeNum - de1.TreeNum > prn.TreeNum - de2.TreeNum)
                    return false;
                else
                    return de1.Node.Start > de2.Node.Start;
            }
            else if (prn.TreeNum >= de1.TreeNum && prn.TreeNum < de2.TreeNum)
                return true;
            else if (prn.TreeNum < de1.TreeNum && prn.TreeNum >= de2.TreeNum)
                return false;
            else
            {
                if (prn.TreeNum - de1.TreeNum < prn.TreeNum - de2.TreeNum)
                    return false;
                else if (prn.TreeNum - de1.TreeNum > prn.TreeNum - de2.TreeNum)
                    return true;
                else
                    return de1.Node.Start < de2.Node.Start;
            }
        }

        private bool IsRoleParallel(DiscourseEntry prn, DiscourseEntry de)
        {
            if (IsSBJ(prn.Node) && IsSBJ(de.Node))
                return true;
            ParseNode obj = null;
            ParseNode cmps = null;
            bool bopj1 = false;
            bool bopj2 = false;
            bool dobj1 = false;
            bool dobj2 = false;
            bool iobj1 = false;
            bool iobj2 = false;
            if (IsOBJ(prn.Node, out obj, out cmps, out bopj1))
            {
                if (IsDirectOBJ(obj, cmps))
                    dobj1 = true;
                else
                    iobj1 = true;
            }
            if (IsOBJ(de.Node, out obj, out cmps, out bopj2))
            {
                if (IsDirectOBJ(obj, cmps))
                    dobj2 = true;
                else
                    iobj2 = true;
            }
            if (dobj1 && dobj2)
                return true;
            if (iobj1 && iobj2)
                return true;
            if (bopj1 && bopj2)
                return true;
            return false;
        }
        //namespace;
        private static string GetWordString(ParseTree tree, ParseNode node)
        {
            string str = (string)tree.Words[node.Start];
            for (int i = node.Start + 1; i < node.End; i++)
                str += "_" + (string)tree.Words[i];
            return str;
        }
        public bool PrepareParseTrees()
        {
            bool modified = false;
            NewSParseTrees = new ArrayList();
            ParseTree pt = (ParseTree)SParseTrees[0];
            ParseTree Ppt;
            bool flag = false;
            NewSParseTrees.Add(pt);
            for (int i = 1; i < SParseTrees.Count; i++)
            {
                pt = (ParseTree)SParseTrees[i];
                Ppt = (ParseTree)SParseTrees[i - 1];
                if (pt.Words.Count == Ppt.Words.Count)
                {
                    flag = false;
                    for (int j = 0; j < pt.Words.Count; j++)
                    {
                        if (pt.Words[j] != Ppt.Words[j])
                        {
                            NewSParseTrees.Add(pt);
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        modified = true;
                    }
                }
                else
                {

                    NewSParseTrees.Add(pt);
                    //el gomla msh heya heya
                }
            }
            return modified;
        }
        public void ReturnSParseTrees()
        {
            bool similar = false;
            bool flag = false;

            ParseTree NSpt = (ParseTree)NewSParseTrees[0];
            ParseTree ppt = (ParseTree)SParseTrees[0];
            ParseTree Cpt = (ParseTree)SParseTrees[0];
            Cpt.ResolvedAnaphora = NSpt.ResolvedAnaphora;
            SParseTrees[0] = Cpt;
            int k = 0;
            for (int i = 1; i < SParseTrees.Count; i++)
            {
                NSpt = (ParseTree)NewSParseTrees[k];
                ppt = (ParseTree)SParseTrees[i - 1];
                Cpt = (ParseTree)SParseTrees[i];
                if (ppt.Words.Count == Cpt.Words.Count)
                {
                    for (int j = 0; j < ppt.Words.Count; j++)
                    {
                        if (ppt.Words[j] != Cpt.Words[j])
                        {
                            flag = false;
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        Cpt.ResolvedAnaphora = NSpt.ResolvedAnaphora;
                        SParseTrees[i] = Cpt;

                    }
                    else
                    {
                        k++;
                        NSpt = (ParseTree)NewSParseTrees[k];
                        Cpt.ResolvedAnaphora = NSpt.ResolvedAnaphora;
                        SParseTrees[i] = Cpt;

                    }

                }
                else
                {
                    k++;
                    NSpt = (ParseTree)NewSParseTrees[k];
                    Cpt.ResolvedAnaphora = NSpt.ResolvedAnaphora;
                    SParseTrees[i] = Cpt;
                    //el gomla msh heya heya
                }
            }

        }
        public void assigndiscoursedata(ArrayList DisClasses,ArrayList ParseTrees)
        {
            foreach (ArrayList arr in DisClasses)
            {
                DiscourseEntry referredNounDE = (DiscourseEntry)arr[0];
                ParseTree ReferredNounParseTree = (ParseTree)ParseTrees[referredNounDE.TreeNum];
                string word = GetWordString(ReferredNounParseTree, referredNounDE.Node);

                for (int i = 1; i < arr.Count; i++)
                {
                    DiscourseEntry de = (DiscourseEntry)arr[i];
                    ParseTree p = (ParseTree)ParseTrees[de.TreeNum];
                    if (p.ResolvedAnaphora.Count != p.Words.Count)
                    {
                        for (int j = 0; j < p.Words.Count; j++)
                            p.ResolvedAnaphora.Add(null);
                    }
                    p.ResolvedAnaphora[de.Node.Start] = word;
                    //ParseNode node= GetReferedNode(ReferredNounParseTree);
                    de.Node.ReferedAnaphoraNode = referredNounDE.Node;
                }
            }

        }
	}

	public class DiscourseEntry
	{
		public ParseNode Node;
		public int TreeNum;
		public Agreement Ag;
		public int Score;
	}

	public class Agreement
	{
		public Agreement(string sense,string word,bool b//New ,WSD wsd
            )
		{
			if(b)
			{
				switch(word)
				{
					case "ANYBODY":
					case "NOBODY":
					case "ANYONE":
					case "NO_ONE":
					case "NONE":
					case"SHE":
					case"HE":
					case "ONESELF":					
					case "HER":
					case "HERSELF":
					case "HIM":
					case "HIMSELF":
					case"HERS":
					case"HIS":
						this.Type=1;
						this.Person=3;
						break;
					case"THOSE":
					case"THESE":
					case"THEY":
					case "THEM":
					case "THEMSELVES":
					case "EVERYBODY":
					case "EVERYONE":
					case "EVERYTHING":
					case"THEIR":
					case"THEIRS":
						this.Type=3;
						this.Person=3;
						break;
					case"IT":
					case"ITSELF":
					case"ITS":
						this.Type=2;
						this.Person=3;
						break;
					case "I":
					case "WE":
					case "ME":
					case "MYSELF":
					case"MINE":
					case"MY":
						this.Type=1;
						this.Person=1;
						break;
					case "YOU":
					case "YOURSELF":
					case "YOURSELVES":
					case"YOUR":
					case"YOURS":
						this.Type=1;
						this.Person=2;
						break;
					case "EACHO":
					case"ONEAN":
						this.Type=3;
						this.Person=0;
						break;
					case"OUR":
					case"OURS":
					case "US":
					case "OURSELVES":
					case "OURSELF":
						this.Type=3;
						this.Person=1;
						break;
					case"THIS":
					case"THAT":
						this.Type=0;
						this.Person=3;
						break;
				}
			}
			else
			{
				this.Person=3;
				if(Lexicon.IsPlural(word))
					this.Type=3;
				else
					this.Type=WSDReplace.GetType(sense); //here we will send the sense //New

			}
		}

		public Agreement(int t,int p)
		{
			Type=t;
			Person=p;
		}

		public int Type; //0 is null 1 is human 2 is thing 3 is plural
		public int Person;//0 is null 1 2 3
	}
}
