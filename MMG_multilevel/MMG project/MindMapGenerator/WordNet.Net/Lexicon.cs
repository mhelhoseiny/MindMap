/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Author:	Jeff Martin
 * Date:	7/07/2005
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson, Jeff Martin
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace WnLexicon
{
	/// <summary>This class contains information about the word</summary>
	public class WordInfo
	{
		public ArrayList texts = null;
		public Wnlib.PartsOfSpeech partOfSpeech = Wnlib.PartsOfSpeech.Unknown;
		public int[] senseCounts = null;

		/// <summary>a sum of all the sense counts hints at the commonality of a word</summary>
		public int Strength
		{
			get
			{
				if( senseCounts == null ) return 0;
				int strength = 0;
				foreach( int i in senseCounts )
					strength += i;
				return strength;
			}
		}

		public static WordInfo Compine(ArrayList morphinfos)
		{
			WordInfo res=new WordInfo();
			res.senseCounts=new int[5];
			res.texts=new ArrayList();
			foreach(WordInfo morph in morphinfos)
			{
				for(int i=1;i<5;i++)
					res.senseCounts[i]+=morph.senseCounts[i];
				if(!res.texts.Contains(morph.texts[0]))
					res.texts.Add(morph.texts[0]);
			}
			return res;
		}
		public static WordInfo Compine(WordInfo word,WordInfo morph)
		{
			if( word.Strength == 0 )
				return morph;
			if( morph.Strength == 0 )
				return word;
			WordInfo result=new WordInfo();
			result.texts=new ArrayList();
			result.texts.AddRange(morph.texts);
			if(!result.texts.Contains(word.texts[0]))
				result.texts.Add(word.texts[0]);
			int MaxLen=Math.Max(word.senseCounts.Length,morph.senseCounts.Length);
			result.senseCounts=new int[MaxLen];
			for(int i=0;i<MaxLen;i++)
				result.senseCounts[i]=word.senseCounts[i]+morph.senseCounts[i];
			Wnlib.PartsOfSpeech[] enums = 
				(Wnlib.PartsOfSpeech[])Enum.GetValues(typeof(Wnlib.PartsOfSpeech));
			int MaxCount=0;
			result.partOfSpeech=Wnlib.PartsOfSpeech.Unknown;
			for( int i=0; i<enums.Length; i++ )
			{
				if(result.senseCounts[i]>MaxCount)
				{
					MaxCount=result.senseCounts[i];
					result.partOfSpeech=enums[i];
				}
			}
			return result;
		}

		public static bool operator == ( WordInfo a, WordInfo b )
		{
			if( (a == null) && (b == null) )
				return true;

			if( (a == null) != (b == null) )
				return false;

			if( a.partOfSpeech != b.partOfSpeech )
				return false;

			if( (a.senseCounts == null) != (b.senseCounts == null) )
				return false;

			if( a.senseCounts != null && b.senseCounts != null )
				return a.senseCounts.Equals( b.senseCounts );

			return true;
		}

		public static bool operator != ( WordInfo a, WordInfo b )
		{
			return !( a == b );
		}

		public override bool Equals( object obj )
		{
			if( obj is WordInfo )
				return this == (WordInfo)obj;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return
				partOfSpeech.GetHashCode() ^
				senseCounts.GetHashCode();
		}
	}

	/// <summary>An efficient feature-specific front-end for WordNet.NET</summary>
	/// <remarks>
	/// <p>
	/// Previously, the only readily-available interface to WordNet was the all-encompassing
	/// Search function which returns everything you could possibly ever want to know about
	/// the target word. Unfortunately, this approach is not very efficient for natural
	/// language processing applications that use WordNet as a database rather than a dictionary.
	/// </p>
	/// <p>
	/// Therefore, the Lexicon class is intended to provide a simple and straightforward interface
	/// to WordNet for very targeted functionality. (ie, what my application needs it to do). It also
	/// attempts to retrieve the desired information in the most efficient way possible.
	/// </p>
	/// </remarks>
	public class Lexicon
	{
        public static string EnglishParserCFG
        {
            get
            {
                return Application.StartupPath + "\\EngParserCFG";
            }
        }
		static Lexicon()
		{
			OtherWords=new Hashtable();
			OtherWords.Add("A",new ArrayList(new string[]{"ARC"}));
			OtherWords.Add("AN",new ArrayList(new string[]{"ARC"}));
			OtherWords.Add("THE",new ArrayList(new string[]{"ARC"}));

			StreamReader sr = new StreamReader(EnglishParserCFG+"\\other.txt");
			while(sr.Peek()!=-1)
			{
				string l=sr.ReadLine();
				if(l=="")
					continue;
				string []line=l.ToUpper().Split(',');
				ArrayList cats=new ArrayList();
				for(int i=1;i<line.Length;i++)
					cats.Add(line[i]);
				OtherWords.Add(line[0],cats);
			}
			sr.Close();

			InitRuleWords();
			
			InitAllCATs();
			
			//**************************************************
			//**************************************************
			Verbs=new Hashtable();
			sr = new StreamReader(EnglishParserCFG+"\\src_verb.exc");
			while(sr.Peek()!=-1)
			{
				string l=sr.ReadLine();
				if(l=="")
					continue;
				string []line=l.ToUpper().Split(' ');
				ArrayList ver=new ArrayList();
				for(int i=0;i<line.Length;i++)
					ver.Add(line[i]);
				Verbs.Add(line[0],ver);
			}

			Adj=new Hashtable();
			sr = new StreamReader(EnglishParserCFG+"\\src_adj.exc");
			while(sr.Peek()!=-1)
			{
				string l=sr.ReadLine();
				if(l=="")
					continue;
				string []line=l.ToUpper().Split(' ');
				ArrayList ver=new ArrayList();
				for(int i=0;i<line.Length;i++)
					ver.Add(line[i]);
				Adj.Add(line[0],ver);
			}

			Noun=new Hashtable();
			sr = new StreamReader(EnglishParserCFG+"\\noun.exc");
			while(sr.Peek()!=-1)
			{
				string l=sr.ReadLine();
				if(l=="")
					continue;
				string []line=l.ToUpper().Split(' ');
				ArrayList ver=new ArrayList();
				for(int i=0;i<line.Length;i++)
					ver.Add(line[i]);
				Noun.Add(line[0],ver);
			}

			NamesList=new ArrayList();
			sr = new StreamReader(EnglishParserCFG+"\\names.txt");
			while(sr.Peek()!=-1)
			{
				string l=sr.ReadLine();
				if(l=="")
					continue;
				NamesList.Add(l);
			}
		}

		public static bool IsPlural(string Word)
		{
			foreach(string s in Noun.Keys)
				if(s.ToUpper()==Word)
					return true;
			/*
			 "s", "ses", "xes", "zes", "ches", "shes", "men", "ies",*/
			/*"", "s",   "x",   "z",   "ch",   "sh",   "man",  "y",*/
			if(Word.EndsWith("CHES"))
			{
				Word=Word.Remove(Word.Length-4,4);
				Word+="CH";
			}
			else if(Word.EndsWith("SHES"))
			{
				Word=Word.Remove(Word.Length-4,4);
				Word+="SH";
			}
			else if(Word.EndsWith("SES"))
			{
				Word=Word.Remove(Word.Length-3,3);
				Word+="S";
			}
			else if(Word.EndsWith("XES"))
			{
				Word=Word.Remove(Word.Length-3,3);
				Word+="X";
			}
			else if(Word.EndsWith("ZES"))
			{
				Word=Word.Remove(Word.Length-3,3);
				Word+="Z";
			}
			else if(Word.EndsWith("MEN"))
			{
				Word=Word.Remove(Word.Length-3,3);
				Word+="MAN";
			}
			else if(Word.EndsWith("IES"))
			{
				Word=Word.Remove(Word.Length-3,3);
				Word+="Y";
			}
			else if(Word.EndsWith("S"))
				Word=Word.Remove(Word.Length-1,1);
			else
				return false;
			WordInfo wi = FindWordInfo(Word,tagged_only,false);
			if(wi.senseCounts[1]>0)
				return true;
			else
				return false;
		}

		private static void InitAllCATs()
		{
			AllCATs=new Hashtable();
			AllCATs.Add("CNP",null);
			AllCATs.Add("CPADJ",null);
			AllCATs.Add("BPADJ",null);
			AllCATs.Add("PADJ",null);
			AllCATs.Add("CADJ",null);
			AllCATs.Add("SADJ",null);
			AllCATs.Add("PADV",null);
			AllCATs.Add("PRS",null);
			AllCATs.Add("NPP",null);
			AllCATs.Add("PPN",null);
			AllCATs.Add("OPP",null);
			AllCATs.Add("PPJ",null);
			AllCATs.Add("SRPN",null);
			AllCATs.Add("IDFP",null);
			AllCATs.Add("DEMP",null);
			AllCATs.Add("ETH",null);
			AllCATs.Add("OR",null);
			AllCATs.Add("NTH",null);
			AllCATs.Add("NOR",null);
			AllCATs.Add("NOTN",null);
			AllCATs.Add("BUTL",null);
			AllCATs.Add("BTH",null);
			AllCATs.Add("WTH",null);
			AllCATs.Add("AND",null);
			AllCATs.Add("ONEAN",null);
			AllCATs.Add("EACHO",null);
			AllCATs.Add("QSM",null);
			AllCATs.Add("CMA",null);
			AllCATs.Add("SCLN",null);
			AllCATs.Add("BEN",null);
			AllCATs.Add("HAV",null);
			AllCATs.Add("BE",null);
			AllCATs.Add("BING",null);
			AllCATs.Add("TO",null);
			AllCATs.Add("XV",null);
			AllCATs.Add("HAV1",null);
			AllCATs.Add("HAV2",null);			
			AllCATs.Add("DO1",null);
			AllCATs.Add("DO2",null);
			AllCATs.Add("BE1",null);
			AllCATs.Add("BE2",null);
			AllCATs.Add("WCH",null);
			AllCATs.Add("QSW1",null);
			AllCATs.Add("QSW2",null);
			AllCATs.Add("WHO",null);
			AllCATs.Add("WIL1",null);
			AllCATs.Add("OUT",null);
			AllCATs.Add("GNTO",null);
			AllCATs.Add("SOR",null);
			AllCATs.Add("COR",null);
			AllCATs.Add("ARC",null);
			AllCATs.Add("N",null);
			AllCATs.Add("V",null);
			AllCATs.Add("VINF",null);
			AllCATs.Add("VING",null);
			AllCATs.Add("VPSP",null);
			
		}

		/*-------------
		 * Data Members
		 *-------------*/

		/// <summary>This gets used a lot, so I decided to cache it in static memory.</summary>
		private static Wnlib.PartsOfSpeech[] enums =
			(Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );

		static Hashtable RuleWords=null;
		static Hashtable OtherWords=null;
		static Hashtable Verbs=null;
		static Hashtable Adj=null;
		static Hashtable Noun=null;
		static Hashtable AllCATs=null;
		static ArrayList NamesList=null;
		public static bool tagged_only=false;

		/*--------
		 * Methods
		 *--------*/

		/// <summary>Finds the part of speech for a given single word</summary>
		/// <param name="word">the word</param>
		/// <param name="includeMorphs">include morphology? (fuzzy matching)</param>
		/// <returns>a structure containing information about the word</returns>
		/// <remarks>
		/// This function is designed to determine the part of speech of a word. Since all
		/// of the WordNet search functions require the part of speech, this function will be useful
		/// in cases when the part of speech of a word is not known. It is not 100% correct
		/// because WordNet was most likely not intended to be used this way. However, it is
		/// accurate enough for most applications.
		/// </remarks>
		public static WordInfo FindWordInfo( string word,bool tagged_only, bool includeMorphs)
		{
			return FindWordInfo(word,tagged_only,includeMorphs,false);
		}
		
		public static WordInfo FindWordInfo(
			string word, bool tagged_only, bool includeMorphs, bool forceMorhps)
		{
			word = word.ToLower();
			WordInfo wordinfo = lookupWord(word, tagged_only);

			if(forceMorhps)
			{
				// include morphology even if some parts of speech were found
				if( includeMorphs )
				{
					WordInfo morphinfo = lookupWordMorphs(word, tagged_only);
					wordinfo=WordInfo.Compine(wordinfo,morphinfo);
				}
			}
			else
			{
				// include morphology if nothing was found on the original word
				if( wordinfo.Strength == 0 && includeMorphs )
					wordinfo = lookupWordMorphs(word, tagged_only);
			}
			
			return wordinfo;
		}

		private static void InitRuleWords()
		{
			RuleWords=new Hashtable();
			RuleWords.Add("?",new ArrayList(new string[]{"QSM"}));
			RuleWords.Add(",",new ArrayList(new string[]{"CMA"}));
			RuleWords.Add(";",new ArrayList(new string[]{"SCLN"}));
			RuleWords.Add("BEEN",new ArrayList(new string[]{"BEN"}));
			RuleWords.Add("HAVE",new ArrayList(new string[]{"HAV","HAV1","HAV2"}));
			RuleWords.Add("BE",new ArrayList(new string[]{"BE"}));
			RuleWords.Add("BEING",new ArrayList(new string[]{"BING"}));
			RuleWords.Add("TO",new ArrayList(new string[]{"TO"}));
			RuleWords.Add("WILL",new ArrayList(new string[]{"WIL1"}));
			RuleWords.Add("WILL_NOT",new ArrayList(new string[]{"WIL1"}));
			RuleWords.Add("WON'T",new ArrayList(new string[]{"WIL1"}));
			RuleWords.Add("WOULD",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHALL",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHOULD",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("CAN",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("COULD",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MUST",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MAY",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MIGHT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("WOULD_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHALL_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHOULD_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("CANNOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("COULD_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MUST_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MAY_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MIGHT_NOT",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("WOULDN'T",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHAN'T",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("SHOULDN'T",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("CAN'T",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("COULDN'T",new ArrayList(new string[]{"XV"}));
			RuleWords.Add("MUSTN'T",new ArrayList(new string[]{"XV"}));			
			RuleWords.Add("HAD",new ArrayList(new string[]{"HAV1"}));
			RuleWords.Add("HAS",new ArrayList(new string[]{"HAV1","HAV2"}));
			RuleWords.Add("HASN'T",new ArrayList(new string[]{"HAV2"}));
			RuleWords.Add("HAVEN'T",new ArrayList(new string[]{"HAV2"}));
			RuleWords.Add("HAS_NOT",new ArrayList(new string[]{"HAV2"}));
			RuleWords.Add("HAVE_NOT",new ArrayList(new string[]{"HAV2"}));
			RuleWords.Add("DO",new ArrayList(new string[]{"DO1","DO2"}));
			RuleWords.Add("DOES",new ArrayList(new string[]{"DO1","DO2"}));
			RuleWords.Add("DID",new ArrayList(new string[]{"DO1","DO2"}));
			RuleWords.Add("DON'T",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("DOESN'T",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("DIDN'T",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("DO_NOT",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("DOES_NOT",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("DID_NOT",new ArrayList(new string[]{"DO2"}));
			RuleWords.Add("AM",new ArrayList(new string[]{"BE1"}));
			RuleWords.Add("IS",new ArrayList(new string[]{"BE1","BE2"}));
			RuleWords.Add("ARE",new ArrayList(new string[]{"BE1","BE2"}));
			RuleWords.Add("WAS",new ArrayList(new string[]{"BE1","BE2"}));
			RuleWords.Add("WERE",new ArrayList(new string[]{"BE1","BE2"}));
			RuleWords.Add("ISN'T",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("AREN'T",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("WASN'T",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("WEREN'T",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("IS_NOT",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("ARE_NOT",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("WAS_NOT",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("WERE_NOT",new ArrayList(new string[]{"BE2"}));
			RuleWords.Add("WHAT",new ArrayList(new string[]{"QSW1","QSW2"}));
			RuleWords.Add("WHICH",new ArrayList(new string[]{"WCH","QSW1","QSW2"}));
			RuleWords.Add("WHO",new ArrayList(new string[]{"QSW2","WHO"}));
			RuleWords.Add("WHEN",new ArrayList(new string[]{"QSW2"}));
			RuleWords.Add("WHERE",new ArrayList(new string[]{"QSW2"}));
			RuleWords.Add("WHY",new ArrayList(new string[]{"QSW2"}));
			RuleWords.Add("TO_WHOM",new ArrayList(new string[]{"QSW2"}));
			RuleWords.Add("OUGHT_TO",new ArrayList(new string[]{"OUT"}));
			RuleWords.Add("GOING_TO",new ArrayList(new string[]{"GNTO"}));
			RuleWords.Add("EACH_OTHER",new ArrayList(new string[]{"EACHO"}));
			RuleWords.Add("ONE_ANOTHER",new ArrayList(new string[]{"ONEAN"}));
			RuleWords.Add("EITHER",new ArrayList(new string[]{"ETH"}));
			RuleWords.Add("OR",new ArrayList(new string[]{"OR"}));
			RuleWords.Add("NEITHER",new ArrayList(new string[]{"NTH"}));
			RuleWords.Add("NOR",new ArrayList(new string[]{"NOR"}));
			RuleWords.Add("NOT_ONLY",new ArrayList(new string[]{"NOTN"}));
			RuleWords.Add("BUT_ALSO",new ArrayList(new string[]{"BUTL"}));
			RuleWords.Add("BOTH",new ArrayList(new string[]{"BTH"}));
			RuleWords.Add("AND",new ArrayList(new string[]{"AND"}));
			RuleWords.Add("WHETHER",new ArrayList(new string[]{"WTH"}));
			RuleWords.Add("AS",new ArrayList(new string[]{"AS"}));
			RuleWords.Add("MORE",new ArrayList(new string[]{"MORE"}));
			RuleWords.Add("MOST",new ArrayList(new string[]{"MOST"}));
			RuleWords.Add("LESS",new ArrayList(new string[]{"LESS"}));
			RuleWords.Add("LEAST",new ArrayList(new string[]{"LST"}));
			RuleWords.Add("THE",new ArrayList(new string[]{"THE"}));
			RuleWords.Add("THAN",new ArrayList(new string[]{"THAN"}));
		}
		
		private static ArrayList findverbinfo(string word,ArrayList texts)
		{
			ArrayList verbinfo=new ArrayList();
			foreach(string s in texts)
			{
				if(Verbs.Contains(s.ToUpper()))
				{
					ArrayList verbs=(ArrayList)Verbs[s.ToUpper()];
					for(int i=0;i<verbs.Count;i++)
					{
						foreach(string r in ((string)verbs[i]).Split(','))
						{
							if(word==r)
							{
								switch(i)
								{
									case 0:
										if(!verbinfo.Contains("VINF"))
											verbinfo.Add("VINF");
										break;
									case 1:
										if(!verbinfo.Contains("V"))
											verbinfo.Add("V");
										break;
									case 2:
										if(!verbinfo.Contains("VPSP"))
											verbinfo.Add("VPSP");
										break;
									case 3:
										if(!verbinfo.Contains("VING"))
											verbinfo.Add("VING");
										break;
									case 4:
										if(!verbinfo.Contains("V"))
											verbinfo.Add("V");
										break;
								}
							}
						}
					}
					if(word==s.ToUpper()+"ING")
					{
						if(!verbinfo.Contains("VING"))
							verbinfo.Add("VING");
					}
					if(s.ToUpper().EndsWith("E") &&(word==s.ToUpper().Substring(0,s.Length-1)+"ING"||word==s.ToUpper().Substring(0,s.Length-1)+"ES"))
					{
						if(word==s.ToUpper().Substring(0,s.Length-1)+"ING")
							if(!verbinfo.Contains("VING"))
								verbinfo.Add("VING");
						else if(word==s.ToUpper().Substring(0,s.Length-1)+"ES")
							if(!verbinfo.Contains("V"))
								verbinfo.Add("V");
					}
					if(word==s.ToUpper()+"S" || word==s.ToUpper()+"ES")
					{
						if(!verbinfo.Contains("V"))
							verbinfo.Add("V");
					}
					if(s.ToUpper().EndsWith("Y"))
					{
						if(word==s.ToUpper().Substring(0,s.Length-1)+"IES")
							if(!verbinfo.Contains("V"))
								verbinfo.Add("V");
					}
				}
				if(word==s.ToUpper()+"ED")
				{	
					if(!verbinfo.Contains("VPSP"))
						verbinfo.Add("VPSP");
				}
				else if(word==s.ToUpper()+"ING")
				{
					if(!verbinfo.Contains("VING"))
						verbinfo.Add("VING");
				}
				else if(word==s.ToUpper()+"S" || word==s.ToUpper()+"ES")
				{
					if(!verbinfo.Contains("V"))
						verbinfo.Add("V");
				}
				else if(s.ToUpper().EndsWith("E")&&(word==s.ToUpper().Substring(0,s.Length-1)+"ING" ||word==s.ToUpper().Substring(0,s.Length-1)+"ES" ||word==s.ToUpper().Substring(0,s.Length-1)+"ED"))
				{
					if(word==s.ToUpper().Substring(0,s.Length-1)+"ING")
					{
						if(!verbinfo.Contains("VING"))
							verbinfo.Add("VING");
					}
					else if (word==s.ToUpper().Substring(0,s.Length-1)+"ES")
					{
						if(!verbinfo.Contains("V"))
							verbinfo.Add("V");
					}
					else if(word==s.ToUpper().Substring(0,s.Length-1)+"ED")
					{
						if(!verbinfo.Contains("VPSP"))
							verbinfo.Add("VPSP");
					}
				}
				else if(s.ToUpper().EndsWith("Y") && word==s.ToUpper().Substring(0,s.Length-1)+"IES")
				{
					if(word==s.ToUpper().Substring(0,s.Length-1)+"IES")
						if(!verbinfo.Contains("V"))
							verbinfo.Add("V");
				}

				else
				{
					WordInfo wi = FindWordInfo(word,tagged_only,false);
					if(wi.senseCounts[2]>0 && !verbinfo.Contains("VINF"))
						verbinfo.Add("VINF");
				}
			}
			if(!verbinfo.Contains("V")&&
				(verbinfo.Contains("VINF")||verbinfo.Contains("VPSP")))
				verbinfo.Add("V");
				
			return verbinfo;
		}

		public static string findadjinfo(string word,ArrayList texts)
		{
			foreach(string s in texts)
			{
				if(Adj.Contains(s.ToUpper()))
				{
					ArrayList Adjects=(ArrayList)Adj[s.ToUpper()];
					if(Adjects.Count==1)
						return "PADJ";
					for(int i=0;i<Adjects.Count;i++)
					{
						foreach(string r in ((string)Adjects[i]).Split(','))
						{
							if(word==r)
							{
								switch(i)
								{
									case 0:
										return "PADJ";
									case 1:
										return "CADJ";
									case 2:
										return "SADJ";
								}
							}
						}
					}
				}
				else if(word==s.ToUpper()+"ER")
					return "CADJ";
				else if(word==s.ToUpper()+"EST")
					return "SADJ";
				else if(s.ToUpper().EndsWith("E") && (word==s.ToUpper().Substring(0,s.Length-1)+"ER")||word==s.ToUpper().Substring(0,s.Length-1)+"EST")
				{
					if(word==s.ToUpper().Substring(0,s.Length-1)+"ER")
						return "CADJ";
					if(word==s.ToUpper().Substring(0,s.Length-1)+"EST")
						return "SADJ";
				}
				else
				{
					WordInfo wi = FindWordInfo(word,tagged_only,false);
					if(wi.senseCounts[3]>0)
						return "PADJ";
				}
			}
			return null;
		}

		public static ArrayList GetCATs(string word)
		{
			ArrayList CATs=new ArrayList();
			WnLexicon.WordInfo wordinfo=null;
			bool wordnet=false;
			bool ppn=false;
			if(OtherWords.Contains(word))
				CATs.AddRange((ArrayList)OtherWords[word]);
			else
			{
				if(NamesList.BinarySearch(word)>=0)
				{
					ppn=true;
					CATs.Add("PPN");
				}
				wordnet=true;
				wordinfo = WnLexicon.Lexicon.FindWordInfo(word,tagged_only,true,true);
				if(wordinfo.senseCounts!=null)
				{
					for(int i=1;i<5;i++)
					{
						if(wordinfo.senseCounts[i]>0)
						{
							switch(i)
							{
								case 1:
								{
									if(!ppn)
										CATs.Add("N");
									break;
								}
								case 2:
								{
									ArrayList r=findverbinfo(word,wordinfo.texts);
									if(r==null)
										break;
									CATs.AddRange(r);
									break;
								}
								case 3:
								{
									string s=findadjinfo(word,wordinfo.texts);
									if(s==null)
										break;
									CATs.Add(s);
									break;
								}
								case 4:
									if(word=="BEST"||word=="BETTER"||word=="DEEPER"||
										word=="FURTHER"||word=="FARTHER"||
										word=="HARDER"||word=="HARDEST")
										break;
									CATs.Add("PADV");break;
							}
						}
					}
				}
			}
			if(CATs.Contains("PADJ"))
			{
				CATs.Add("BPADJ");
				CATs.Add("CPADJ");
			}
			if((CATs.Contains("VPSP")||CATs.Contains("VING"))&&CATs.Contains("BPADJ"))
				CATs.Remove("BPADJ");
			if(CATs.Contains("VPSP")&&CATs.Contains("CPADJ"))
				CATs.Remove("CPADJ");
			if(CATs.Contains("VING")&&CATs.Contains("N"))
				CATs.Remove("N");

			if(!wordnet || wordinfo.senseCounts==null || wordinfo.senseCounts[1]==0)
			{
				string org=null;
				if(word.EndsWith("'S"))
					org=word.Remove(word.Length-2,2);
				else if(word.EndsWith("S'"))
					org=word.Remove(word.Length-1,1);
				if(org!=null)
					CATs.Add("CNP");
			}
			if(RuleWords.Contains(word))
				CATs.AddRange((ArrayList)RuleWords[word]);
			if(CATs.Count==0)
			{
				bool n=true;
				foreach(char c in word)
					if(!char.IsLetterOrDigit(c))
					{
						n=false;
						break;
					}
				if(n)
					CATs.Add("PPN");
			}
			return CATs;
		}

		public static Hashtable GetDiff(Hashtable FoundCATs)
		{
			Hashtable Diff=(Hashtable)AllCATs.Clone();
			foreach(string cat in FoundCATs.Keys)
				try
				{Diff.Remove(cat);}
				catch{}
			return Diff;
		}

		/// <summary>Returns a list of Synonyms for a given word</summary>
		/// <param name="word">the word</param>
		/// <param name="pos">The Part of speech of a word</param>
		/// <param name="includeMorphs">include morphology? (fuzzy matching)</param>
		/// <returns>An array of strings containing the synonyms found</returns>
		/// <remarks>
		/// Note that my usage of 'Synonyms' here is not the same as hypernyms as defined by
		/// WordNet. Synonyms in this sense are merely words in the same SynSet as the given
		/// word. Hypernyms are found by tracing the pointers in a given synset.
		/// </remarks>
		public static string[] FindSynonyms( string word, Wnlib.PartsOfSpeech pos, bool includeMorphs )
		{
			// get an index to a synset collection
			word = word.ToLower();
			Wnlib.Index index = Wnlib.Index.lookup( word, Wnlib.PartOfSpeech.of( pos ) );

			// none found?
			if( index == null )
			{
				if( !includeMorphs )
					return null;

				// check morphs
				Wnlib.MorphStr morphs = new Wnlib.MorphStr( word, Wnlib.PartOfSpeech.of( pos ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					index = Wnlib.Index.lookup( morph, Wnlib.PartOfSpeech.of( pos ) );
					if( index != null )
						break;
				}
			}

			// still none found?
			if( index == null )
				return null;

			// at this point we will always have a valid index
			return lookupSynonyms( index );
		}

		private static string[] lookupSynonyms( Wnlib.Index index )
		{
			// OVERVIEW: For each sense, grab the synset associated with our index.
			//           Then, add the lexemes in the synset to a list.

			ArrayList synonyms = new ArrayList( 10 );

			// for each sense...
			for( int s=0; s<index.offs.Length; s++ )
			{
				// read in the word and its pointers
				Wnlib.SynSet synset = new Wnlib.SynSet( index.offs[s], index.pos, index.wd, null, s );

				// build a string out of the words
				for( int i=0; i<synset.words.Length; i++ )
				{
					string word = synset.words[i].word.Replace( "_", " " );

					// if the word is capitalized, that means it's a proper noun. We don't want those.
					if( word[0] <= 'Z' )
						continue;

					// add it to the list if it's a different word
					if( string.Compare( word, index.wd, true ) != 0 )
						synonyms.Add( word );
				}
			}

			return (string[])synonyms.ToArray( typeof( string ) );
		}

		private static WordInfo lookupWord( string word , bool tagged_only)
		{
			// OVERVIEW: For each part of speech, look for the word.
			//           Compare relative strengths of the synsets in each category
			//			 to determine the most probable part of speech.
			//
			// PROBLEM:  Word definitions are often context-based. It would be better
			//           to find a way to search in-context in stead of just singling
			//           out an individual word.
			//
			// SOLUTION: Modify FindPartOfSpeech to include a second argument, string
			//           context. The pass the entire sentence as the context for part
			//           of speech determination.
			//
			// PROBLEM:  That's difficult to do so I'm going to keep this simple for now.

			int maxCount = 0;
			WordInfo wordinfo = new WordInfo();
			wordinfo.partOfSpeech = Wnlib.PartsOfSpeech.Unknown;
			wordinfo.texts=new ArrayList();
			wordinfo.texts.Add(word);

			// for each part of speech...
			Wnlib.PartsOfSpeech[] enums = (Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );
			wordinfo.senseCounts = new int[enums.Length];
			for( int i=0; i<enums.Length; i++ )
			{
				// get a valid part of speech
				Wnlib.PartsOfSpeech pos = enums[i];
				if( pos == Wnlib.PartsOfSpeech.Unknown )
					continue;

				// get an index to a synset collection
				Wnlib.Index index = Wnlib.Index.lookup( word, Wnlib.PartOfSpeech.of( pos ) );

				// none found?
				if( index == null )
					continue;
				// none tagged
				if( tagged_only && index.tagsense_cnt==0 )
					continue;

				// does this part of speech have a higher sense count?
				if( tagged_only )
					wordinfo.senseCounts[i] = index.tagsense_cnt;
				else
					wordinfo.senseCounts[i] = index.sense_cnt;
				if( wordinfo.senseCounts[i] > maxCount )
				{
					maxCount = wordinfo.senseCounts[i];
					wordinfo.partOfSpeech = pos;
				}
			}
	
			return wordinfo;
		}

		private static WordInfo lookupWordMorphs(string word, bool tagged_only)
		{
			// OVERVIEW: This functions only gets called when the word was not found with
			//           an exact match. So, enumerate all the parts of speech, then enumerate
			//           all of the word's morphs in each category. Perform a lookup on each
			//           morph and save the morph/strength/part-of-speech data sets. Finally,
			//           loop over all the data sets and then pick the strongest one.

			ArrayList wordinfos = new ArrayList();

			// for each part of speech...
			for( int i=0; i<enums.Length; i++ )
			{
				// get a valid part of speech
				Wnlib.PartsOfSpeech pos = enums[i];
				if( pos == Wnlib.PartsOfSpeech.Unknown )
					continue;

				// generate morph list
				Wnlib.MorphStr morphs = new Wnlib.MorphStr( word, Wnlib.PartOfSpeech.of( pos ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					// get an index to a synset collection
					Wnlib.Index index = Wnlib.Index.lookup( morph, Wnlib.PartOfSpeech.of( pos ) );

					// none found?
					if( index == null )
						continue;
					// none tagged
					if( tagged_only && index.tagsense_cnt==0 )
						continue;

					// save the wordinfo
					WordInfo wordinfo = getMorphInfo( wordinfos, morph );
					if( tagged_only )
						wordinfo.senseCounts[i] = index.tagsense_cnt;
					else
						wordinfo.senseCounts[i] = index.sense_cnt;
				}
			}

			return WordInfo.Compine(wordinfos);
/*
			// search the wordinfo list for the best match
			WordInfo bestWordInfo = new WordInfo();
			int maxStrength = 0;
			foreach( WordInfo wordinfo in wordinfos )
			{
				// for each part of speech...
				int maxSenseCount = 0;
				int strength = 0;
				for( int i=0; i<enums.Length; i++ )
				{
					// get a valid part of speech
					Wnlib.PartsOfSpeech pos = enums[i];
					if( pos == Wnlib.PartsOfSpeech.Unknown )
						continue;

					// determine part of speech and strength
					strength += wordinfo.senseCounts[i];
					if( wordinfo.senseCounts[i] > maxSenseCount )
					{
						maxSenseCount = wordinfo.senseCounts[i];
						wordinfo.partOfSpeech = pos;
					}			
				}

				// best match?
				if( strength > maxStrength )
				{
					maxStrength = strength;
					bestWordInfo = wordinfo;
				}
			}

			return bestWordInfo;
*/
		}

		private static WordInfo getMorphInfo( ArrayList morphinfos, string morph )
		{
			// Attempt to find the morph string in the list.
			// NOTE: Since the list should never get very large, a selection search will work just fine
			foreach( WordInfo morphinfo in morphinfos )
				if( (string)morphinfo.texts[0] == morph )
					return morphinfo;

			// if not found, create a new one
			WordInfo wordinfo = new WordInfo();
			wordinfo.texts=new ArrayList();
			wordinfo.texts.Add(morph);
			wordinfo.senseCounts = new int[enums.Length];
			return (WordInfo)morphinfos[morphinfos.Add( wordinfo )];
		}
	}
}
