using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OntoSem;

namespace QAS
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	 public enum AnswerType
	{
		Boolean,
		Concept,
		Property,
		List,
		NoAnswer,
		Frame,
		ConceptList,
		MixedList
	}
	public class frmSyntaxGenerator : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		
		private System.ComponentModel.Container components = null;
		public frmSyntaxGenerator()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.White;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(178)));
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(456, 272);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// Form2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 272);
			this.Controls.Add(this.textBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Form2";
			this.Text = "Answer Viewer";
			this.ResumeLayout(false);

		}
		#endregion

		string DisplayConcept(Concept c,bool reference,bool unique)
		{
			string output="";
			if(reference)
				output+="It";
			else
			{
				if(unique)
					output+="The "+c.Id;
				else
				{
					if(c.Id[0]=='A'||c.Id[0]=='E'||c.Id[0]=='I'
						||c.Id[0]=='U'||c.Id[0]=='O')
						output+="an "+c.Id;
					else
						output+="a "+c.Id;
				}
			}
			bool b=false;
			foreach(Property p in c.FullProperties )
			{
				if(p.IsModified)
				{
					b=true;
					output+=PropToFormat(p);
					output+=".\r\nIt";
				}
			}
			if(!b)
				return "";
			output = output.Remove(output.Length-2,2);
			output = output.Replace("_"," ");
			return output.ToLower();
		}

		string PropToFormat(Property Prop)
		{
			string format = "";
			switch(Prop.Name)
			{
				case"VOLUME":
				{
					if(Prop.Fillers[0].ScalarFiller=="<0.3")
						format=" is of small size";

				}break;
				case"STRENGTH":
					if(Prop.Fillers[0].ScalarFiller==">0.7")
						format=" is powerful";
					else if(Prop.Fillers[0].ScalarFiller==">0.5")
						format=" is strong";
					else if(Prop.Fillers[0].ScalarFiller=="<0.5")
						format=" is weak";
					break;
				case"AGE":
					break;
				case"CARDINALITY":
					if(Prop.Fillers[0].ScalarFiller!="1")
					{
						format=" contains ";
						format+=Prop.Fillers[0].ScalarFiller;
						format+=" "+Prop.Concept.Id;
					}
					break;
				case"LOCATION":
					format=" is located in ";
					format+=Prop.Fillers[0].ScalarFiller;
					if(Prop.Concept.FullProperties["PART-OF-OBJECT"]!=null&&Prop.Concept.FullProperties["PART-OF-OBJECT"].IsModified)
						format+=" of the "+((Frame)Prop.Concept.FullProperties["PART-OF-OBJECT"].Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"STATE-OF-REPAIR":
					if(Prop.Fillers[0].ScalarFiller=="<0.2")
						format=" which isn't loaded with oxygen";
					else if(Prop.Fillers[0].ScalarFiller==">0.8")
						format=" which is loaded with oxygen";
					break;
				case"PART-OF-OBJECT":
					format=" is contained in the "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"DURATION":
					format=" for 130 year";
					break;
				case"PATH":
					format=" through the "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"DESTINATION":
					format=" to the "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"SOURCE":
					format=" from the "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"INSTRUMENT":
					format=" through the "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"PURPOSE":
					format=" to "+DisplayFrame1((Frame)Prop.Fillers[0].Frames[0],true);
					break;
				case"HAS-OBJECT-AS-PART":
					format=" contains "+((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"AGENT":
					format=((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"THEME":
					format=((Frame)Prop.Fillers[0].Frames[0]).Predicate.Id;
					break;
				case"SUB":
					break;
				case"SUPER":
					break;
				default:
					throw new Exception("INVALID PROPERTY");
			}
			return format;
		}

		string DisplayFrame1(Frame frm,bool unique)
		{
			string Output = "";
			ArrayList Agent = frm.Predicate.FullProperties["AGENT"].Fillers[0].Frames;
			ArrayList Theme = frm.Predicate.FullProperties["THEME"].Fillers[0].Frames;
			if(frm.Predicate.FullProperties["AGENT"]!=null&&frm.Predicate.FullProperties["AGENT"].IsModified)
			{
				string x=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
				if(unique)
					Output+="The ";
				else
				{
					if(x[0]=='A'||x[0]=='E'||x[0]=='I'
						||x[0]=='U'||x[0]=='O')
						Output+="an ";
					else
						Output+="a ";
				}
				Output+=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
			}
			if(Output!="")
				Output+=" ";
			Output+=frm.Predicate.Id;
			if(frm.Predicate.FullProperties["THEME"]!=null&&frm.Predicate.FullProperties["THEME"].IsModified)
			Output+=" "+PropToFormat(frm.Predicate.FullProperties["THEME"]);
			foreach(Property p in frm.Predicate.FullProperties )
			{
				if(p.Name == "AGENT" ||p.Name == "THEME" )
					continue;
				if(p.IsModified)
					Output+=PropToFormat(p);
			}
			Output+=".\r\n";
			return Output;
		}

		string DisplayFrame2(Frame frm,bool unique)
		{
			string Output = "";
			ArrayList Agent = frm.Predicate.FullProperties["AGENT"].Fillers[0].Frames;
			ArrayList Theme = frm.Predicate.FullProperties["THEME"].Fillers[0].Frames;
			if(frm.Predicate.FullProperties["AGENT"]!=null&&frm.Predicate.FullProperties["AGENT"].IsModified)
			{
				string x=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
				if(unique)
					Output+="The ";
				else
				{
					if(x[0]=='A'||x[0]=='E'||x[0]=='I'
						||x[0]=='U'||x[0]=='O')
						Output+="an ";
					else
						Output+="a ";
				}
				Output+=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
			}
			if(Output!="")
				Output+=" ";
			Output+=frm.Predicate.Id;
			if(frm.Predicate.FullProperties["THEME"]!=null&&frm.Predicate.FullProperties["THEME"].IsModified)
			Output+=" "+PropToFormat(frm.Predicate.FullProperties["THEME"]);
			foreach(Property p in frm.Predicate.FullProperties )
			{
				if(p.Name == "AGENT" ||p.Name == "THEME" )
					continue;
				if(p.IsModified)
					Output+=PropToFormat(p);
			}
			Output+=".\r\n";
			if(frm.Predicate.FullProperties["AGENT"]!=null&&frm.Predicate.FullProperties["AGENT"].IsModified)
			{
				Output += DisplayConcept(((Frame)Agent[0]).Predicate,true,true);
				Output=Output.Remove(Output.Length-2,2);
			}
			return Output;
		}

		string DisplayFrame3(Frame frm,bool unique)
		{
			string Output = "";
			ArrayList Agent = frm.Predicate.FullProperties["AGENT"].Fillers[0].Frames;
			ArrayList Theme = frm.Predicate.FullProperties["THEME"].Fillers[0].Frames;
			if(frm.Predicate.FullProperties["AGENT"]!=null&&frm.Predicate.FullProperties["AGENT"].IsModified)
			{
				string x=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
				if(unique)
					Output+="The ";
				else
				{
					if(x[0]=='A'||x[0]=='E'||x[0]=='I'
						||x[0]=='U'||x[0]=='O')
						Output+="an ";
					else
						Output+="a ";
				}
				Output+=PropToFormat(frm.Predicate.FullProperties["AGENT"]);
			}
			if(Output!="")
				Output+=" ";
			Output+=frm.Predicate.Id;
			if(frm.Predicate.FullProperties["THEME"]!=null&&frm.Predicate.FullProperties["THEME"].IsModified)
				Output+=" "+PropToFormat(frm.Predicate.FullProperties["THEME"]);
			foreach(Property p in frm.Predicate.FullProperties )
			{
				if(p.Name == "AGENT" ||p.Name == "THEME" )
					continue;
				if(p.IsModified)
					Output+=PropToFormat(p);
			}
			Output+=".\r\n";
			if(frm.Predicate.FullProperties["THEME"]!=null&&frm.Predicate.FullProperties["THEME"].IsModified)
			{
				Output += DisplayConcept(((Frame)Theme[0]).Predicate,false,true);
				Output=Output.Remove(Output.Length-2,2);
			}
			return Output;
		}

		public void Begin(ParseNode Question,object answer,AnswerType at)
		{
			Begin(Question,answer,at,true,true);
		}
		public void Begin(ParseNode Question,object answer,AnswerType at,bool unique,bool agent)
		{
			this.Show();
			if(at==AnswerType.Boolean)
				this.textBox1.Text=(bool)answer?"Yes.":"No.";
			else if(at==AnswerType.Concept)
			{
				this.textBox1.Text= DisplayConcept((Concept)answer,false,true);
			}
			else if(at == AnswerType.NoAnswer)
				this.textBox1.Text= "No answer";
			else if(at == AnswerType.List)
			{
				foreach(Frame F in (ArrayList)answer)
					if(agent)
						this.textBox1.Text+= this.DisplayFrame2(F,unique)+"\r\n\r\n";
					else
						this.textBox1.Text+= this.DisplayFrame3(F,unique)+"\r\n\r\n";
			}
			else if(at == AnswerType.ConceptList)
			{
				foreach(Concept F in (ArrayList)answer)
					this.textBox1.Text+= this.DisplayConcept(F,false,false);
			}
			else if(at == AnswerType.MixedList)
			{
				foreach(object F in (ArrayList)answer)
				{
					Type t=F.GetType();
					if(t==typeof(Concept))
						this.textBox1.Text+= this.DisplayConcept((Concept)F,true,true);
					else
						this.textBox1.Text+= this.DisplayFrame1((Frame)F,unique);
				}
			}
			System.Text.StringBuilder text = new System.Text.StringBuilder(
				this.textBox1.Text.ToLower());
			text[0]=char.ToUpper(text[0]);
			bool nl=false;
			for(int i=1;i<text.Length;i++)
			{
				if(text[i]=='\n')
					nl=true;
				else
				{
					if(nl)
						text[i]=char.ToUpper(text[i]);
					nl=false;
				}
			}
			textBox1.Text=text.ToString().Trim();


			#region NOTNEEDED
			/*else if(at==AnswerType.List)
			{
				ArrayList Concepts=(ArrayList)answer;
				Concept mainc = (Concept)Concepts[0];
				Concepts.RemoveAt(0);
				string str = DisplayConcept(mainc).ToLower()+" which is the ";
				foreach(Concept c in Concepts)
					str+=DisplayConcept(c).ToLower()+" , ";
				str = str.Remove(str.Length-2,2);
				int x=str.LastIndexOf(",");
				string s1=str;
				if(x!=-1)
				{
					s1=str.Substring(0,x);
					s1+=" and ";
					s1+=str.Substring(x+1);
				}
				this.textBox1.Text=s1;
			}
			else if(at==AnswerType.NoAnswer)
				this.textBox1.Text="No Answer";
			else if(at==AnswerType.Concept)
			{
				Concept c = (Concept)answer;
				string str=DisplayConcept(c).ToLower()+" which ";
				foreach(Property p in c.FullProperties)
				{
					if(p.IsModified)
					{
						if(p.Fillers[0].IsScalar)
						{
							str+=p.Name.ToLower()+": ";
							str+=p.Fillers[0].ScalarFiller;
						}
					}
				}
				this.textBox1.Text=str;
			}
			else if(at==AnswerType.Property)
			{
				Property p=(Property)answer;
				string str=p.Concept.Name+"'s";
				str+=p.Name+": ";
				if(p.Fillers[0].IsScalar)
					str+=p.Fillers[0].ScalarFiller;
				else
					str+=DisplayConcept(((Frame)p.Fillers[0].Frames[0]).Predicate);
				this.textBox1.Text=str;
			}*/
			#endregion
		}


	}
}
