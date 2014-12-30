using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

using Ilan.Google.API.ImageSearch;
using System.Windows.Forms;

namespace Ilan.Test.Google.API
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : Form
	{
		private TextBox txtQuery;
		private Label label1;
		private Label label2;
		private NumericUpDown nudNumOfResults;
		private Label label3;
		private NumericUpDown nudStartPosition;
		private Button btnSearch;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private Size imageSize = new Size(150, 150);
		private ArrayList pics = new ArrayList();
		private CheckBox chkFilter;
		private Label label4;
		private ComboBox cmbSafeSearch;
		private readonly int startHeight = 70;
        private Button btnLoadRegex;
		private Thread picThread;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            // IMPORTANT - This is NOT how to deal with this issue.
            // Any call to a UI control should be done via the UI
            // thread (i.e. using Invoke). However, since this is
            // just a sample application to show how to work with
            // the Google Image Search API, we will just tell
            // the framework not to check for illegal cross thread 
            // calls.
            //Form.CheckForIllegalCrossThreadCalls = false;

			cmbSafeSearch.SelectedIndex = 1; // Moderate
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudNumOfResults = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudStartPosition = new System.Windows.Forms.NumericUpDown();
            this.chkFilter = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSafeSearch = new System.Windows.Forms.ComboBox();
            this.btnLoadRegex = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(88, 8);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(336, 20);
            this.txtQuery.TabIndex = 0;
            this.txtQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuery_KeyDown);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Query:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nr. of results:";
            // 
            // nudNumOfResults
            // 
            this.nudNumOfResults.Location = new System.Drawing.Point(88, 40);
            this.nudNumOfResults.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudNumOfResults.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumOfResults.Name = "nudNumOfResults";
            this.nudNumOfResults.Size = new System.Drawing.Size(56, 20);
            this.nudNumOfResults.TabIndex = 3;
            this.nudNumOfResults.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(152, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Start position:";
            // 
            // nudStartPosition
            // 
            this.nudStartPosition.Location = new System.Drawing.Point(232, 40);
            this.nudStartPosition.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudStartPosition.Name = "nudStartPosition";
            this.nudStartPosition.Size = new System.Drawing.Size(48, 20);
            this.nudStartPosition.TabIndex = 5;
            // 
            // chkFilter
            // 
            this.chkFilter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFilter.Location = new System.Drawing.Point(296, 40);
            this.chkFilter.Name = "chkFilter";
            this.chkFilter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkFilter.Size = new System.Drawing.Size(128, 16);
            this.chkFilter.TabIndex = 7;
            this.chkFilter.Text = "Filter similar results?";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(430, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 24);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search!";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(432, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Safe search:";
            // 
            // cmbSafeSearch
            // 
            this.cmbSafeSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSafeSearch.Items.AddRange(new object[] {
            "Active",
            "Moderate",
            "Off"});
            this.cmbSafeSearch.Location = new System.Drawing.Point(504, 40);
            this.cmbSafeSearch.Name = "cmbSafeSearch";
            this.cmbSafeSearch.Size = new System.Drawing.Size(88, 21);
            this.cmbSafeSearch.TabIndex = 10;
            // 
            // btnLoadRegex
            // 
            this.btnLoadRegex.Location = new System.Drawing.Point(512, 8);
            this.btnLoadRegex.Name = "btnLoadRegex";
            this.btnLoadRegex.Size = new System.Drawing.Size(80, 24);
            this.btnLoadRegex.TabIndex = 11;
            this.btnLoadRegex.Text = "Load Regex";
            this.btnLoadRegex.Click += new System.EventHandler(this.btnLoadRegex_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(624, 477);
            this.Controls.Add(this.btnLoadRegex);
            this.Controls.Add(this.cmbSafeSearch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.chkFilter);
            this.Controls.Add(this.nudStartPosition);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudNumOfResults);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtQuery);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartPosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			btnSearch.Enabled = false;
		    btnLoadRegex.Enabled = false;
			foreach (PictureBox pic in pics)
			{
				pic.DoubleClick -= new EventHandler(pic_DoubleClick);
				Controls.Remove(pic);
			}
			pics.Clear();
			picThread = new Thread(new ThreadStart(GetPictures));
			picThread.IsBackground = true;
			picThread.Start();
		}

		private void GetPictures()
		{
			try
			{
                string query = string.Empty;
			    int startPosition = 0;
			    int resultsRequested = 0;
                SafeSearchFiltering safeSearch = SafeSearchFiltering.Moderate;
                txtQuery.Invoke(
                    new MethodInvoker(
                        delegate
                            {
                                query = Regex.Replace(txtQuery.Text, @"\s{1,}", "+");
                                startPosition = Decimal.ToInt32(nudStartPosition.Value);
                                resultsRequested = Decimal.ToInt32(nudNumOfResults.Value);
                                safeSearch =
                                    (SafeSearchFiltering)
                                    Enum.Parse(typeof (SafeSearchFiltering), (string) cmbSafeSearch.SelectedItem);
                            }));
				SearchResponse response = SearchService.SearchImages(query, startPosition, resultsRequested, chkFilter.Checked, safeSearch);
				if (response.Results.Length == 0)
				{
					MessageBox.Show("No results available");
				}
			    
			    // Note: This is not a good implementation - it generates a lot of threads and with a large number
			    // of requested results will be a problem. Using a pool (s.a. the ThreadPool) would be easier,
			    // But then I need to add some waiting mechanism, and I'm too lazy.
			    // After all, it's just a sample application so show how to work with the API.
                Thread[] threads = new Thread[response.Results.Length];
				for (int i = 0; i < response.Results.Length; i++)
				{
				    int index = i;
                    threads[index] = new Thread(
                        new ThreadStart(
                            delegate
                                {
                                    Image img = getImage(response.Results[index].ThumbnailUrl);
                                    Invoke(
                                        new MethodInvoker(
                                            delegate
                                                {
                                                    PictureBox pic = new PictureBox();
                                                    pic.BorderStyle = BorderStyle.Fixed3D;
                                                    pic.Size = imageSize;
                                                    pic.Location =
                                                        new Point(imageSize.Width * (index % 4),
                                                                  (index / 4) * imageSize.Height + startHeight);
                                                    pic.SizeMode = PictureBoxSizeMode.CenterImage;
                                                    pic.Image = img;
                                                    pics.Add(pic);
                                                    pic.Tag = response.Results[index];
                                                    pic.DoubleClick += new EventHandler(pic_DoubleClick);
                                                    // Adjust for scrolled location
                                                    if (AutoScrollPosition.Y != 0)
                                                    {
                                                        pic.Location =
                                                            new Point(pic.Location.X,
                                                                      pic.Location.Y + AutoScrollPosition.Y);
                                                    }
                                                    Controls.Add(pic);
                                                }));
                                }));
				    threads[i].IsBackground = true;
				    threads[i].Start();
				}
			    
			    foreach (Thread thread in threads)
			    {
			        thread.Join();
			    }

				MessageBox.Show("Done!");
			}
			catch (Exception ex)
			{
                Invoke(
                    new MethodInvoker(
                        delegate
                            {
                                MessageBox.Show(
                                    string.Format("An exception occurred while running the query: {0}{1}{2}",
                                                  ex.Message, Environment.NewLine, ex.StackTrace), "Query Aborted!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
			}
			finally
			{
                Invoke(
                    new MethodInvoker(
                        delegate
                            {
                                btnSearch.Enabled = true;
                                btnLoadRegex.Enabled = true;
                            }));
			}
		}

		private Image getImage(string url)
		{
			Image im = null;
			try
			{				
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "GET";
				request.Timeout = 15000;
				request.ProtocolVersion = HttpVersion.Version11;

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{	
						im = Image.FromStream(responseStream);
					}
				}
			}
			catch (Exception ex) 
			{
				Debug.WriteLine("Exception in getThumbnail. Url: " + url + ". Info: " + ex.Message + Environment.NewLine + "Stack: " + ex.StackTrace);
			}
			return im;
		}

		private void txtQuery_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnSearch_Click(this, null);
			}
		}

		private void pic_DoubleClick(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			PictureBox pic = sender as PictureBox;
			SearchResult result = pic.Tag as SearchResult;
			Image image = getImage(result.ImageUrl);
			if (null == image)
			{
				MessageBox.Show(this, "Unable to retrieve image: " + result.ImageUrl);
				return;
			}
			Form form = new Form();
			PictureBox fullImage = new PictureBox();
			fullImage.Image = image;
			fullImage.Height = image.Height;
			fullImage.Width = image.Width;
			fullImage.Dock = DockStyle.Fill;
			fullImage.SizeMode = PictureBoxSizeMode.StretchImage;
			Size sizeDiff = form.Size - form.ClientSize;
			form.Height = image.Height + sizeDiff.Height;
			form.Width = image.Width + sizeDiff.Width;
			form.Text = string.Format("{0} x {1} - {2} kb",
				result.ImageWidth.ToString(), result.ImageHeight.ToString(), ((int)(result.ImageSize/1000.0)).ToString());
			form.Controls.Add(fullImage);
			form.KeyDown += new KeyEventHandler(form_KeyDown);
			form.ShowDialog(this);

			Cursor = Cursors.Default;
		}

		private void form_KeyDown(object sender, KeyEventArgs e)
		{
			Form form = sender as Form;
			if ((form != null) && (e.KeyCode == Keys.Escape))
			{
				form.Hide();
				form.Close();
			}
		}

        private void btnLoadRegex_Click(object sender, EventArgs e)
        {
            SearchService.LoadRegexStrings();
        }
	}
}
