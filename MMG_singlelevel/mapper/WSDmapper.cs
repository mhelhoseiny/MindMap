using System;
using Wnlib;
using System.Collections;
using System.Text;
//using WSD_TypeLib;
using WordNetClasses;
using SyntacticAnalyzer;

namespace OntologyLibrary
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

		public static ArrayList GetSenses(string pos,string noun)
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
		
		private int count=0;
		private double score=0;


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
