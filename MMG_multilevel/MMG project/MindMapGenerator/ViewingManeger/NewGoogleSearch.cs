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
using Google.API.Search;
using System.Diagnostics;
using MindMapViewingManagement;

namespace ViewingManeger
{
    public class NewGoogleSearch
    {
        GimageSearchClient gimC;
        public GoogleImageSearchSettings GImSearchSettings { get; set; }
        public NewGoogleSearch()
        {
            GImSearchSettings = new GoogleImageSearchSettings();
            gimC = new GimageSearchClient("www.rutgers.edu");
        }
        public NewGoogleSearch(GoogleImageSearchSettings gImSearchSettings)
        {
            GImSearchSettings = gImSearchSettings;
            gimC = new GimageSearchClient("www.rutgers.edu");

        }

        public IList<IImageResult> Search2(string query)
        {
            IList<IImageResult> Results = gimC.Search(query, GImSearchSettings.Count, GImSearchSettings.ImSize.ToString().ToLower(), GImSearchSettings.Coloriztion.ToString().ToLower(), GImSearchSettings.Imtype.ToString().ToLower(), GImSearchSettings.FileTypeStr);

            
            return Results;
        }

        public IList<IImageResult> Search2(string query, ImageSize imageSize)
        {
            IList<IImageResult> Results = gimC.Search(query, GImSearchSettings.Count, imageSize.ToString().ToLower(), GImSearchSettings.Coloriztion.ToString().ToLower(), GImSearchSettings.Imtype.ToString().ToLower(), GImSearchSettings.FileTypeStr);


            return Results;
        }

        public Image GetImage(string query)
        {
            IList<IImageResult> Results = Search2(query);
            if (Results.Count >= 1 )
            {
                return LoadImageFromUrl(Results[0].TbImage.Url);
              
            }
            else return null;
        }
        
        public Image LoadImageFromUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            Image img = Image.FromStream(response.GetResponseStream());

            response.Close();
            return img;
        }
        public void LoadImageFromUrl(string url, PictureBox pb)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            
            Image img = Image.FromStream(response.GetResponseStream());
            
            response.Close();
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Image = img;
        }
    }
}
