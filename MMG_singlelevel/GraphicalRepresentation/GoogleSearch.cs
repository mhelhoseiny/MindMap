using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ilan;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using Ilan.Google.API.ImageSearch;
using System.Diagnostics;

namespace GoogleImageDrawingEntity
{
    public class GoogleSearch
    {
            private Size imageSize = new Size(150, 150);
            private ArrayList pics = new ArrayList();
            private readonly int startHeight = 70;
            public string Text;
            public Bitmap bitmap;
            public PictureBox picbox = new PictureBox();
            public GoogleSearch(string t)
            {
                Text = t;
            }
            public static Bitmap GetImage(string text)
            {
                GoogleSearch gSearch = new GoogleSearch(text);
                gSearch.GetPictures();
                return gSearch.bitmap;

            }
            public void GetPictures()
            {
                //try
                //{
                    string query = Text;
                    int startPosition = 0;
                    int resultsRequested = 0;
                    Ilan.Google.API.ImageSearch.SafeSearchFiltering safeSearch = Ilan.Google.API.ImageSearch.SafeSearchFiltering.Moderate;

                    query = Regex.Replace(Text, @"\s{1,}", "+");
                    //safeSearch =
                    //    (Ilan.Google.API.ImageSearch.SafeSearchFiltering)
                    //    Enum.Parse(typeof(Ilan.Google.API.ImageSearch.SafeSearchFiltering), "Moderate");

                    Ilan.Google.API.ImageSearch.SearchResponse response = Ilan.Google.API.ImageSearch.SearchService.SearchImages(query, 0, 3, false, safeSearch);
                    if (response.Results.Length == 0)
                    {
                        MessageBox.Show("No results available");
                    }

                    // Note: This is not a good implementation - it generates a lot of threads and with a large number
                    // of requested results will be a problem. Using a pool (s.a. the ThreadPool) would be easier,
                    // But then I need to add some waiting mechanism, and I'm too lazy.
                    // After all, it's just a sample application so show how to work with the API.

                    for (int i = 0; i < response.Results.Length; i++)
                    {
                        int index = i;
                        Image img = getImage(response.Results[index].ThumbnailUrl);
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
                        //pic.DoubleClick += new EventHandler(pic_DoubleClick);
                        // Adjust for scrolled location
                        /*
                        if (AutoScrollPosition.Y != 0)
                        {
                            pic.Location =
                                new Point(pic.Location.X,
                                          pic.Location.Y + AutoScrollPosition.Y);
                        }
                        Controls.Add(pic);
                         */
                        bitmap = new Bitmap(img);
                    }
                    MessageBox.Show("Done!");
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(
                //        string.Format("An exception occurred while running the query: {0}{1}{2}",
                //                      ex.Message, Environment.NewLine, ex.StackTrace), "Query Aborted!",
                //        MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}


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
                        using (System.IO.Stream responseStream = response.GetResponseStream())
                        {
                            im = Image.FromStream(responseStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in getThumbnail. Url: " + url + ". Info: " + ex.Message + Environment.NewLine + "Stack: " + ex.StackTrace);
                }
                return im;
            }        
    }
}
