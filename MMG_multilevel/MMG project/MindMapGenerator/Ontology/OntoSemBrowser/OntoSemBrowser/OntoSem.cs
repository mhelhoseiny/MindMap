using System;
using System.Collections;
using System.Runtime.Serialization;

namespace OntoSem
{
	public enum Modifier
	{
		VALUE,DEFAULT,SEM,RELAXABLE_TO,NOT,MAP_LEX,INV
	}

	[Serializable]
	public class Filler
	{
		private Property property;
		private Modifier modifier;
		private bool isScalar;
		private Concept conceptFiller;
		private string scalarFiller;

		public Filler(Property property,Modifier modifier,object filler,bool isScalar)
		{
			this.property=property;
			this.modifier=modifier;
			this.isScalar=isScalar;
			if(isScalar)
				scalarFiller=(string)filler;
			else
				conceptFiller=(Concept)filler;
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
	}

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
		private Concept concept;
		private string name;
		private Concept inheritedFrom;
		public FillerList Fillers;

		public Property(Concept concept,string name,Concept inheritedFrom)
		{
			this.concept=concept;
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
				return (Property)properties[name];
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
			return properties.GetEnumerator();
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
		private Ontology ontology;
		private string name;
		public PropertiesDictionary Properties;

		public Concept(Ontology ontology,string name)
		{
			this.ontology=ontology;
			this.name=name;
			Properties=new PropertiesDictionary();
		}
		
		public string Name
		{
			get
			{
				return name;
			}
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
			return concepts.GetEnumerator();
		}

		#endregion

		private Hashtable concepts;

		public Ontology()
		{
			concepts=new Hashtable();
		}
	}
}
