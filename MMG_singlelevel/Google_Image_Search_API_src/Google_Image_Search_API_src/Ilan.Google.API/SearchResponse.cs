using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Ilan.Google.API.ImageSearch
{
	/// <summary>
	/// The response object that stores an image search response from Google Image Search.
	/// For each image returned we get the original image url and the thumbnail url.
	/// Also, for each search response we get the total number of available results for the query.
	/// </summary>
	[Serializable]
	public class SearchResponse
	{
		private SearchResult[] results;
		private int totalResultsAvailable;

		public SearchResponse()
		{
			this.Results = new SearchResult[0];
			this.TotalResultsAvailable = 0;
		}

		public SearchResult[] Results
		{
			get { return results; }
			set { results = value; }
		}

		public int TotalResultsAvailable
		{
			get { return totalResultsAvailable; }
			set { totalResultsAvailable = value; }
		}
	}

	/// <summary>
	/// Information returned from Google Image Search for each image.
	/// </summary>
	[Serializable]
	public class SearchResult
	{
		private string imageUrl;
		private string thumbnailUrl;
        private string thumbnailCode;
		private int thumbnailWidth;
		private int thumbnailHeight;
		private int imageWidth;
		private int imageHeight;
		private int imageSize;
		private string fileExtension;
		private string fileName;

		public SearchResult()
		{
			this.ImageUrl = string.Empty;
			this.ThumbnailUrl = string.Empty;
		}

		public string FullFileName
		{
			get { return FileName + "." + FileExtension; }
		}

		public string FileExtension
		{
			get { return fileExtension; }
			set { fileExtension = value; }
		}

		// Does not include the extension
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		public int ThumbnailWidth
		{
			get { return thumbnailWidth; }
			set { thumbnailWidth = value; }
		}

		public int ThumbnailHeight
		{
			get { return thumbnailHeight; }
			set { thumbnailHeight = value; }
		}

		public int ImageWidth
		{
			get { return imageWidth; }
			set { imageWidth = value; }
		}

		public int ImageHeight
		{
			get { return imageHeight; }
			set { imageHeight = value; }
		}

		public int ImageSize
		{
			get { return imageSize; }
			set { imageSize = value; }
		}

		public string ImageUrl
		{
			get { return imageUrl; }
			set
			{
				// Before keeping the image's url we try to extract the file name and the extension from the url
				int lastSlash = value.LastIndexOf("/");				
				if (lastSlash < 0)
				{
					// If there is no "/" in the url we cannot extract the file's name or extension
					FileName = string.Empty;					
					FileExtension = string.Empty;					
				}
				else
				{
					int lastDot = value.LastIndexOf(".");
					if (lastDot - lastSlash - 1 < 0)
					{
						// If there is no "." after the last "/" - then there is no extension
						FileName = value.Substring(lastSlash + 1);						
						FileExtension = string.Empty;
					}
					else
					{
						// Extract both file name and extension from the url
						FileName = value.Substring(lastSlash + 1, lastDot - lastSlash - 1);
						FileExtension = value.Substring(lastDot + 1);
					}					
				}
				imageUrl = value;
                if (null != thumbnailCode)
                {
                    thumbnailUrl = "http://images.google.com/images?q=tbn:" + thumbnailCode + imageUrl;
                }
			}
		}

		public string ThumbnailUrl
		{
			get { return thumbnailUrl; }
			set
			{
				if (value.StartsWith("/"))
				{
					thumbnailUrl = "http://images.google.com" + value;
				}
				else
				{
					thumbnailUrl = value;
				}
			}
		}

	    public string ThumbnailCode
	    {
	        get { return thumbnailCode; }
	        set
	        {
	            thumbnailCode = value;
	            if (null != imageUrl)
	            {
                    thumbnailUrl = "http://images.google.com/images?q=tbn:" + thumbnailCode + imageUrl;
	            }
	        }
	    }
	}
}
