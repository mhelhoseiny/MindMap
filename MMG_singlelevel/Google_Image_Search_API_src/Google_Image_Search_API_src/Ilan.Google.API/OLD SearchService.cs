using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Ilan.Google.API.ImageSearch
{
	/// <summary>
	/// A search utility used to retrieve images using Google Image Search.
	/// Since Google does not provide a proper API for their image search, this utility parses the html
	/// returned from running an image search query. Therefore, if Google will change the format of the
	/// returned html file, the parsing may fail and need to be adapted to succeed.
	/// This utility conforms to the html format used on October 5th 2005.
	/// </summary>
	public class SearchService
	{
		/// <summary>Google limits the results it returns to the first 1000 results for each query</summary>
		public const int MAX_RESULTS = 1000;
		/// <summary>Google returns up to 20 images each time a search is performed</summary>
		private const int RESULTS_PER_QUERY = 20;

        private static string imagesRegexStr;
        private static string dataRegexStr;
        private static string totalResultsRegexStr;	    
	    
		// Avoid initialization
		private SearchService() {}

	    static SearchService()
	    {
	        LoadRegexStrings();
	    }
	    
	    public static void LoadRegexStrings()
	    {
	        try
	        {
                ConfigurationManager.RefreshSection("appSettings");
                string fileName = ConfigurationManager.AppSettings["imagesSearchFile"];
	            using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.StartsWith("imagesRegex"))
                        {
                            imagesRegexStr = line.Substring(line.IndexOf("("));
                        }
                        else if (line.StartsWith("dataRegex"))
                        {
                            dataRegexStr = line.Substring(line.IndexOf("("));
                        }
                        else if (line.StartsWith("totalResultsRegex"))
                        {
                            totalResultsRegexStr = line.Substring(line.IndexOf("("));
                        }
                    }
                }	                
            }
	        catch(Exception ex)
	        {
	            Exception x = new Exception("Image Search API Could not load Regex file.", ex);
	            throw x;
	        }
	    }
	    
		/// <summary>
		/// Runs the given query against Google Image Search and returns a SearchResponse object with details
		/// for each returned image. The search is performed using Moderate SafeSearch setting.
		/// </summary>
		public static SearchResponse SearchImages(string query, int startPosition, int resultsRequested, bool filterSimilarResults)
		{
			return SearchImages(query, startPosition, resultsRequested, filterSimilarResults, SafeSearchFiltering.Moderate);
		}

		/// <summary>
		/// Runs the given query against Google Image Search and returns a SearchResponse object with details
		/// for each returned image.
		/// </summary>
		/// <param name="query">The query to be sent.</param>
		/// <param name="startPosition">The index of the first item to be retrieved (must be positive).</param>
		/// <param name="resultsRequested">The number of results to be retrieved (must be between 1 and (MAX_RESULTS - startPosition)</param>
		/// <param name="filterSimilarResults">Set to 'true' if you want Google to automatically omit similar entries. Set to 'false' if you want to retrieve every matching image.</param>
		/// <param name="safeSearch">Indicates what level of SafeSearch to use.</param>
		/// <returns>A SearchResponse object with details for each returned image.</returns>
		public static SearchResponse SearchImages(string query, int startPosition, int resultsRequested, bool filterSimilarResults, SafeSearchFiltering safeSearch)
		{
			// Check preconditions
			if (resultsRequested < 1)
			{
				throw new ArgumentOutOfRangeException("resultsRequested", "Value must be positive");
			}
			else if (startPosition < 0)
			{
				throw new ArgumentOutOfRangeException("startPosition", "Value must be positive");
			}
			else if (resultsRequested + startPosition > MAX_RESULTS)
			{
				throw new ArgumentOutOfRangeException("resultsRequested", "Sorry, Google does not serve more than 1000 results for any query");
			}
			
			string safeSearchStr = safeSearch.ToString().ToLower();
			SearchResponse response = new SearchResponse();
			ArrayList results = new ArrayList();
			
			// Since Google returns 20 results at a time, we have to run the query over and over again (each
			// time with a different starting position) until we get the requested number of results.
			for (int i = 0; i < resultsRequested; i+=RESULTS_PER_QUERY)
			{
				string requestUri = string.Format("http://images.google.com/images?q={0}&start={1}&filter={2}&safe={3}",
					query, (startPosition+i).ToString(), (filterSimilarResults)?1.ToString():0.ToString(), safeSearchStr );

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
				string resultPage = string.Empty;
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(responseStream))
						{
							resultPage = reader.ReadToEnd();
						}
					}
				}

				// Here's the parsing of the images' details. If the html format changes, that's most probably where we will have to update the code
				// There are two types of regions in the HTML we have to parse in order to gather all the information
				// about the images:
				// 1. The image's url, thumbnail url and thumbnail width&height will be extracted
				// by the imagesRegex and imagesMatches objects.
				// 2. The image's width&height&size will be extracted by the dataRegex and dataMatches objects.
				//Regex imagesRegex = new Regex(@"(\x3Ca\s+href=/imgres\x3Fimgurl=)(?<imgurl>http[^&>]*)([>&]{1})([^>]*)(>{1})(<img\ssrc\x3D)(""{0,1})(?<images>/images[^""\s>]*)([\s])+(width=)(?<width>[0-9,]*)\s+(height=)(?<height>[0-9,]*)");
                //Regex dataRegex = new Regex(@"([^>]*)(>)\s{0,1}(<br>){0,1}\s{0,1}(?<width>[0-9,]*)\s+x\s+(?<height>[0-9,]*)\s+pixels\s+-\s+(?<size>[0-9,]*)(k)");
                Regex imagesRegex = new Regex(imagesRegexStr);
                Regex dataRegex = new Regex(dataRegexStr);
				MatchCollection imagesMatches = imagesRegex.Matches(resultPage);
				MatchCollection dataMatches = dataRegex.Matches(resultPage);

				if ((imagesMatches == null) || 
					(imagesMatches.Count == 0) ||
					(dataMatches == null) ||
					(dataMatches.Count == 0))
				{
					Trace.WriteLine("Parsing of query " + query + " failed - collections count mismatch");
					break;
				}

				// The two MatchCollections should include an entry for each returned image. Therefore,
				// if they don't have the same number of items, then the parsing has failed and we
				// stop the query (this is just a provision, in reality using these expressions
				// for many thousands of queries it never broke here :-)
				if (imagesMatches.Count != dataMatches.Count)
				{
					throw new Exception("Parsing of the response failed for url: " + requestUri);
				}

				// Build a SearchResult object for each image
				for (int j = 0; j < imagesMatches.Count && (i+j) < resultsRequested ; j++)
				{
					Match imageMatch = imagesMatches[j];
					Match dataMatch = dataMatches[j];
					SearchResult result = new SearchResult();
					result.ImageUrl = imageMatch.Groups["imgurl"].Value;
					result.ThumbnailUrl = imageMatch.Groups["images"].Value;
					result.ThumbnailWidth = int.Parse(imageMatch.Groups["width"].Value);
					result.ThumbnailHeight = int.Parse(imageMatch.Groups["height"].Value);
					result.ImageWidth = int.Parse(dataMatch.Groups["width"].Value);
					result.ImageHeight = int.Parse(dataMatch.Groups["height"].Value);
					// Since the value in the HTML is in kb, this is only an approximation to the number of bytes
					result.ImageSize = int.Parse(dataMatch.Groups["size"].Value) * 1000;
					results.Add(result);
				}

				// Extract the total number of results available and make sure we didn't reach the end of the results
				//Regex totalResultsRegex = new Regex(@"(?<lastResult>[0-9,]*)(\s*</b>\s*)(of)(\s)+(about){0,1}(\s*<b>\s*)(?<totalResultsAvailable>[0-9,]*)");
                Regex totalResultsRegex = new Regex(totalResultsRegexStr);
				Match totalResultsMatch = totalResultsRegex.Match(resultPage);
				string totalResultsRaw = totalResultsMatch.Groups["totalResultsAvailable"].Value;
				response.TotalResultsAvailable = int.Parse(totalResultsRaw.Replace("\"", "").Replace(",", ""));
				int lastResult = int.Parse(totalResultsMatch.Groups["lastResult"].Value.Replace("\"", "").Replace(",", ""));
				if (lastResult >= response.TotalResultsAvailable)
				{
					break;
				}
			}

			response.Results = (SearchResult[]) results.ToArray(typeof(SearchResult));

			return response;
		}
	}

	/// <summary>
	/// Used to specify Google's SafeSearch setting. Google's SafeSearch blocks web pages containing 
	/// explicit sexual content from appearing in search results.
	/// </summary>
	public enum SafeSearchFiltering
	{
		/// <summary>
		/// Filter both explicit text and explicit images (a.k.a. Strict Filtering).
		/// </summary>
		Active,	
		/// <summary>
		/// Filter explicit images only - default behavior.
		/// </summary>
		Moderate,
		/// <summary>
		/// Do not filter the search results.
		/// </summary>
		Off
	}
}
