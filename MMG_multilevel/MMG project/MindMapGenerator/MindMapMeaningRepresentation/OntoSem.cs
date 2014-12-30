using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace mmTMR
{
	public enum Modifier
	{
		UNKNOWN,VALUE,DEFAULT,SEM,RELAXABLE_TO,NOT,MAP_LEX,DEFAULT_MEASURE,
		MEASURING_UNIT,INV
	}

	[Serializable]
	public class Filler
	{
		public Property Property;
		private Modifier modifier;
		private bool isScalar;
		private bool isInverse;
		private Concept conceptFiller;
		private string scalarFiller;
		public ArrayList Frames;

		public Filler()
		{}

		public Filler(Property property,Modifier modifier,object filler,ArrayList f,bool isScalar)
		{
			this.Property=property;
			this.modifier=modifier;
			isInverse=(modifier==Modifier.INV);
			this.isScalar=isScalar;
			if(isScalar)
				scalarFiller=(string)filler;
			else
			{
				conceptFiller=(Concept)filler;
				this.Frames = f;
			}
		}
	
		public Filler Copy(Property pro,Concept con)
		{
			Filler f = new Filler();
			f.Frames = this.Frames;
			f.conceptFiller =con;
			f.Property = pro;
			f.scalarFiller = this.scalarFiller;
			f.isInverse = this.isInverse;
			f.isScalar = this.isScalar;
			f.modifier = this.modifier;
			return f;
		}

		public Modifier Modifier
		{
			get
			{
				return modifier;
			}
		}
		public bool IsScalar
		{
			get
			{
				return isScalar;
			}
		}
		public bool IsInverse
		{
			get
			{
				return isInverse;
			}
		}
		public string ScalarFiller
		{
			get
			{
				return scalarFiller;
			}
		}
		public Concept ConceptFiller
		{
			get
			{
				return conceptFiller;
			}
		}

		public void RemoveInverse()
		{
			if(modifier!=Modifier.INV)
				return;
			Ontology onto = Property.Concept.Ontology;
			Concept PropCon = onto[Property.Name];
			Property p = PropCon.Properties[":INVERSE"];
			if(p==null)
			{
				modifier=Modifier.SEM;
				return;
			}
			string InvProp = p.Fillers[0].ConceptFiller.Name;
			Property FinProp = ConceptFiller.Properties[":"+InvProp];
			if(FinProp==null)
			{
				modifier=Modifier.SEM;
				return;
			}
			foreach(Filler fil in FinProp.Fillers)
				if(fil.ConceptFiller==Property.Concept)
				{
					modifier=fil.modifier;
					return;
				}
			modifier=Modifier.SEM;
		}
	}

	[Serializable]
	public class FillerList :ICollection
	{
		public Filler this[int index]
		{
			get
			{
				return (Filler)fillers[index];
			}
			set
			{
				fillers[index]=value;
			}
		}

		public void RemoveAt(int index)
		{
			fillers.RemoveAt(index);
		}

		public void Insert(int index, Filler value)
		{
			fillers.Insert(index,value);
		}

		public void Remove(Filler value)
		{
			fillers.Remove(value);
		}

		public FillerList Copy(Property Pro,Concept con)
		{
			FillerList newlist = new FillerList();
			ArrayList arr = new ArrayList();
			foreach(Filler f in this.fillers )
				arr.Add(f.Copy(Pro,con));
			newlist.fillers = arr;
			return newlist;
		}

		public bool Contains(Filler value)
		{
			return fillers.Contains(value);
		}

		public void Clear()
		{
			fillers.Clear();
		}

		public int IndexOf(Filler value)
		{
			return IndexOf(value);
		}

		public int Add(Filler value)
		{
			return fillers.Add(value);
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return fillers.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return fillers.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			fillers.CopyTo(array,index);
		}

		public object SyncRoot
		{
			get
			{
				return fillers.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return fillers.GetEnumerator();
		}

		#endregion

		private ArrayList fillers;

		public FillerList()
		{
			fillers=new ArrayList();
		}
	}

	[Serializable]
	public class Property
	{
		public bool IsModified  = false;
		public Concept Concept;
		private string name;
		private Concept inheritedFrom;
		public FillerList Fillers;

		public Property(Concept concept,string name,Concept inheritedFrom)
		{
			this.Concept=concept;
			this.name=name;
			this.inheritedFrom=inheritedFrom;
			Fillers=new FillerList();
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public Property()
		{}

		public Property Copy(Concept Con)
		{
			Property p = new Property();
			p.Concept =Con;
			p.Fillers = (FillerList)this.Fillers.Copy(p,Con);
			p.inheritedFrom = this.inheritedFrom;
			p.name =this.name;
			p.IsModified=this.IsModified;
			return p;
		}

		public string Key
		{
			get
			{
				if(inheritedFrom==null)
					return ':'+name;
				else
					return inheritedFrom.Name+':'+name;
			}
		}

		public Concept InheritedFrom
		{
			get
			{
				return inheritedFrom;
			}
		}
	}

	[Serializable]
	public class PropertiesDictionary : ICollection
	{
		public Property this[string name]
		{
			get
			{
				string p = "";
				foreach(string key in properties.Keys)
				{
					string x=key.Remove(0,key.IndexOf(':')+1);
					if(x == name)
						p = key;
				}
				return (Property)properties[p];
			}
			set
			{
				properties[name]=value;
			}
		}

		public void Remove(string name)
		{
			properties.Remove(name);
		}

		public bool Contains(string name)
		{
			return properties.Contains(name);
		}

		public void Clear()
		{
			properties.Clear();
		}

		public ICollection Properties
		{
			get
			{
				return properties.Values;
			}
		}

		public PropertiesDictionary Copy(Concept Con)
		{
			PropertiesDictionary p = new PropertiesDictionary();
			Hashtable arr = new Hashtable();
			foreach(string f in this.properties.Keys )
				arr.Add(f,((Property)this.properties[f]).Copy(Con));
			p.properties = arr;
			return p;
		}

		public void Add(string name, Property value)
		{
			properties.Add(name,value);
		}

		public ICollection Names
		{
			get
			{
				return properties.Keys;
			}
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return properties.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return properties.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			properties.CopyTo(array,index);
		}

		public object SyncRoot
		{
			get
			{
				return properties.SyncRoot;
			}
		}

		#endregion
		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return properties.Values.GetEnumerator();
		}

		#endregion

		private Hashtable properties;

		public PropertiesDictionary()
		{
			properties=new Hashtable();
		}
	}


	[Serializable]
	public class Concept
	{
		public Ontology Ontology;
		private string name;
		private bool isFull;
		private PropertiesDictionary properties;
		public string Id;

		public Concept(Ontology ontology,string name)
		{
			this.Ontology=ontology;
			this.name=name;
			properties=new PropertiesDictionary();
			isFull=false;
		}		
		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool IsFull
		{
			get
			{
				return isFull;
			}
		}

		public Concept()
		{}

		public Concept Copy()
		{
			Concept Co = new Concept();
			Co.isFull = this.isFull;
			Co.name = this.name;
			Co.Ontology = this.Ontology;
			Co.Id=this.Id;
			Co.properties =this.properties.Copy(Co);
			return Co;
		}

		public PropertiesDictionary Properties
		{
			get
			{
				return properties;
			}
		}

		public PropertiesDictionary FullProperties
		{
			get
			{
				if(!isFull)
					LoadInherited();
				return properties;
			}
		}

		public bool IsLoaded
		{
			get
			{
				return properties.Count!=0;
			}
		}

		private Modifier GetModifier(string mod)
		{
			switch(mod)
			{
				case"VALUE":
					return Modifier.VALUE;
				case"DEFAULT":
					return Modifier.DEFAULT;
				case"SEM":
					return Modifier.SEM;
				case"RELAXABALE-TO":
				case"RELAXABLE-TO":
					return Modifier.RELAXABLE_TO;
				case"NOT":
					return Modifier.NOT;
				case"MAP-LEX":
					return Modifier.MAP_LEX;
				case"DEFAULT-MEASURE":
					return Modifier.DEFAULT_MEASURE;
				case"INV":
					return Modifier.INV;
				case"MEASURING-UNIT":
					return Modifier.MEASURING_UNIT;
			}
			return Modifier.UNKNOWN;
		}
		
		public void LoadLocal()
		{
			if(properties.Count!=0)
				return;
			string file=Ontology.PathOf(name);
			StreamReader sr=new StreamReader(file);
			string line=null;
			Concept inherited=null;
			Property currProperty=null;
			Modifier currMod=Modifier.UNKNOWN;
			while((line=sr.ReadLine())!=null)
			{
				line=line.Trim();
				if(line=="")
					continue;
				if(line.StartsWith("Concept:"))
				{
					continue;
				}
				else if(line.StartsWith("Inherited from:"))
				{
					return;
				}
				else
				{
					string[] tokens=line.Split('\t');
					ArrayList parts=new ArrayList();
					foreach(string token in tokens)
					{
						if(token.Trim()!="")
							parts.Add(token.Trim());
					}
					if(parts.Count==3)
					{
						currProperty=new Property(this,(string)parts[0],inherited);
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						properties.Add(currProperty.Key,currProperty);
						currMod=GetModifier((string)parts[1]);
						string strfiller=(string)parts[2];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
					else if(parts.Count==2)
					{
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						currMod=GetModifier((string)parts[0]);
						string strfiller=(string)parts[1];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
					else
					{
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						string strfiller=(string)parts[0];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
				}
			}
			sr.Close();
		}

		public void LoadInherited()
		{
			if(isFull)
				return;
			if(properties.Count==0)
				LoadLocal();
			string file=Ontology.PathOf(name);
			StreamReader sr=new StreamReader(file);
			string line=null;
			Concept inherited=null;
			Property currProperty=null;
			Modifier currMod=Modifier.UNKNOWN;
			while((line=sr.ReadLine())!=null)
			{
				line=line.Trim();
				if(line=="")
					continue;
				if(line.StartsWith("Concept:"))
				{
					continue;
				}
				else if(line.StartsWith("Inherited from:"))
				{
					string inhname=line.Substring(15).Trim();
					inherited=Ontology[inhname];
				}
				else if(inherited!=null)
				{
					string[] tokens=line.Split('\t');
					ArrayList parts=new ArrayList();
					foreach(string token in tokens)
					{
						if(token.Trim()!="")
							parts.Add(token.Trim());
					}
					if(parts.Count==3)
					{
						currProperty=new Property(this,(string)parts[0],inherited);
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						properties.Add(currProperty.Key,currProperty);
						currMod=GetModifier((string)parts[1]);
						string strfiller=(string)parts[2];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
					else if(parts.Count==2)
					{
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						currMod=GetModifier((string)parts[0]);
						string strfiller=(string)parts[1];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
					else
					{
						if(currProperty.Name=="DEFINITION" || 
							currProperty.Name=="ENGLISH1" ||
							currProperty.Name=="NOTES" || 
							currProperty.Name=="TIMESTAMP")
							continue;
						string strfiller=(string)parts[0];
						Concept confiller=Ontology[strfiller];
						if(confiller==null)
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,strfiller,null,true));
						else
							currProperty.Fillers.Add(
								new Filler(currProperty,currMod,confiller,null,false));
					}
				}
			}
			isFull=true;
			sr.Close();
		}

		public void ClearInherited()
		{
			if(!isFull)
				return;
			ArrayList removal=new ArrayList();
			foreach(Property p in properties.Properties)
				if(p.InheritedFrom!=null)
					removal.Add(p.Name);
			foreach(string name in removal)
				properties.Remove(name);
			isFull=false;
		}
	}

	[Serializable]
	public class Ontology : ICollection
	{
		public Concept this[string name]
		{
			get
			{
				return (Concept)concepts[name];
			}
			set
			{
				concepts[name]=value;
			}
		}

		public void Remove(string name)
		{
			concepts.Remove(name);
		}

		public bool Contains(string name)
		{
			return concepts.Contains(name);
		}

		public void Clear()
		{
			concepts.Clear();
		}

		public ICollection Concepts
		{
			get
			{
				return concepts.Values;
			}
		}

		public void Add(string name, Concept value)
		{
			concepts.Add(name,value);
		}

		public ICollection Names
		{
			get
			{
				return concepts.Keys;
			}
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return concepts.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return concepts.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			concepts.CopyTo(array,index);
		}

		public object SyncRoot
		{
			get
			{
				return concepts.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return concepts.Values.GetEnumerator();
		}

		#endregion

		private Hashtable concepts;
		private string path;

		public Ontology(string path)
		{
			concepts=new Hashtable();
			this.path=path;
			InitOntology();
		}

		private void InitOntology()
		{
			StreamReader sr=new StreamReader(allConceptsPath);
			string str=null;
			while((str=sr.ReadLine())!=null)
			{
				if(str=="")
					continue;
				this.Add(str,new Concept(this,str));
			}
			sr.Close();
		}

		public void LoadConcept(string name)
		{
			Concept con=this[name];
			if(con==null)
				return;
			if(con.IsLoaded)
				return;
			con.LoadLocal();
		}

		public void LoadConceptFull(string name)
		{
			Concept con=this[name];
			if(con==null)
				return;
			if(!con.IsLoaded)
				con.LoadLocal();
			if(con.IsFull)
				return;
			con.LoadInherited();
		}

		public string OntologyPath
		{
			get
			{
				return path;
			}
		}

		private string allConceptsPath
		{
			get
			{
				return path+"\\AllConcepts.txt";
			}
		}

		public string PathOf(string concept)
		{
			return path+"\\"+concept[0]+"\\"+concept+".txt";
		}

		public void RemoveInverses()
		{
			int nc=0;
			foreach(Concept concept in concepts.Values)
			{
				nc++;
				foreach(Property prop in concept.Properties)
					foreach(Filler f in prop.Fillers)
						f.RemoveInverse();
			}
		}
	}
}
