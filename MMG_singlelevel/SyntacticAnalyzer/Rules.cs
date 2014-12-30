using System;
using System.IO;
using System.Collections;


namespace SyntacticAnalyzer
{
	public class RulesReader
	{
		public Hashtable Keywords;
		public ArrayList Rules;

		private RulesReader(){}

		public RulesReader(string file)
		{
			Keywords = new Hashtable();
			Rules=new ArrayList();
			StreamReader sr = new StreamReader(file);
			while(true)
			{
				string line=sr.ReadLine();
				if(line==null)
					break;
				line=line.Trim().ToUpper();
				if(line=="" || line.StartsWith("//"))
					continue;
				line=line.Split(' ')[0];
				string[] parts=line.Split('=','+');
				string[] rhss=new string[parts.Length-1];
				Array.Copy(parts,1,rhss,0,rhss.Length);
				Rule rl = new Rule(parts.Length-1);
				rl.LHS = parts[0];
				rl.RHS.AddRange(rhss);
				int Index=Rules.Count;
				Rules.Add(rl);
				if(Keywords.Contains(rl.LHS))
					((ArrayList)Keywords[rl.LHS]).Add(Index);
				else
					Keywords.Add(rl.LHS,new ArrayList(new int[] {Index}));
			}
			sr.Close();
		}

		public ArrayList GetRules(string LHS)
		{
			return (ArrayList)Keywords[LHS];
		}

		public RulesReader Minimize(Hashtable MissedCATs,int MaxIterations)
		{
			Hashtable KeyCount = new Hashtable();
			foreach(string key in Keywords.Keys)
				KeyCount.Add(key,((ArrayList)Keywords[key]).Count);
			ArrayList CurrentRules=Rules;
			Hashtable CurrentMissed=MissedCATs;
			for(int i=0;i<MaxIterations&&CurrentMissed.Count>0;i++)
			{
				ArrayList NewRules = new ArrayList();
				Hashtable AddedMissed=new Hashtable();
				foreach(Rule r in CurrentRules)
				{
					bool ismissed=false;
					foreach(string missed in CurrentMissed.Keys)
					{
						if(r.RHS.Contains(missed))
						{
							int nc=((int)KeyCount[r.LHS])-1;
							KeyCount[r.LHS]=nc;
							ismissed=true;
							if(nc==0)
								AddedMissed.Add(r.LHS,null);
							break;
						}
					}
					if(ismissed)
						continue;
					NewRules.Add(r);
				}
				CurrentRules=NewRules;
				CurrentMissed=AddedMissed;
			}
			RulesReader NewReader = new RulesReader();
			NewReader.Rules=CurrentRules;
			NewReader.Keywords=new Hashtable();
			for(int i=0;i<CurrentRules.Count;i++)
			{
				Rule rl=(Rule)CurrentRules[i];
				if(NewReader.Keywords.Contains(rl.LHS))
					((ArrayList)NewReader.Keywords[rl.LHS]).Add(i);
				else
					NewReader.Keywords.Add(rl.LHS,new ArrayList(new int[] {i}));
			}
			return NewReader;
		}

	}

	public class Rule
	{
		public string LHS;
		public ArrayList RHS;
		public Rule(int Rcount)
		{
			RHS=new ArrayList(Rcount);
		}
	}
}
