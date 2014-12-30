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

namespace MindMapViewingManagement
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

                Google.API.Search.GimageSearchClient cl = new Google.API.Search.GimageSearchClient("http://www.rutgers.edu/");
                IList<Google.API.Search.IImageResult> imres = cl.Search(text, 1);
                Image im = DownloadImage(imres[0].TbImage.Url);
                return new Bitmap(im);
            }

            /// <summary>
            /// Function to download Image from website
            /// </summary>
            /// <param name="_URL">URL address to download image</param>
            /// <returns>Image</returns>
            public static Image DownloadImage(string _URL)
            {
                Image _tmpImage = null;

                try
                {
                    // Open a connection
                    System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                    _HttpWebRequest.AllowWriteStreamBuffering = true;

                    // You can also specify additional header values like the user agent or the referer: (Optional)
                    _HttpWebRequest.UserAgent = "Chrome/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                    _HttpWebRequest.Referer = "http://www.rutgers.edu/";

                    // set timeout for 20 seconds (Optional)
                    _HttpWebRequest.Timeout = 20000;

                    // Request response:
                    System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                    // Open data stream:
                    System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                    // convert webstream to image
                    _tmpImage = Image.FromStream(_WebStream);

                    // Cleanup
                    _WebResponse.Close();
                    _WebResponse.Close();
                }
                catch (Exception _Exception)
                {
                    // Error
                    Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                    return null;
                }

                return _tmpImage;
            }
            //public static Bitmap GetImage(string text)
            //{
            //    GoogleSearch gSearch = new GoogleSearch(text);
            //    gSearch.GetPictures();
            //    return gSearch.bitmap;

            //}

            
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
                    //MessageBox.Show("Done!");
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
