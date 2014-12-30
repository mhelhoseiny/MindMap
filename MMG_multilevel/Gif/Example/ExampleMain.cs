using System;
using System.Drawing;
using System.Drawing.Imaging;
using Gif.Components;

namespace Example
{
	class ExampleMain
	{
		[STAThread]
		static void Main(string[] args)
		{
			/* create Gif */
			//you should replace filepath
			String [] imageFilePaths = new String[]{@"F:\Mohamed ELhoseiny\Dropbox\Public\MMExperiment\08. Joh Adams\JohAdams2AllAll.jpg",
                @"F:\Mohamed ELhoseiny\Dropbox\Public\MMExperiment\08. Joh Adams\JohAdams2AllClipArt.jpg",
                @"F:\Mohamed ELhoseiny\Dropbox\Public\MMExperiment\08. Joh Adams\JohAdams2AllLineArt.jpg"}; 
			String outputFilePath = "c:\\test.gif";
			AnimatedGifEncoder e = new AnimatedGifEncoder();
			e.Start( outputFilePath );
			e.SetDelay(4000);
			//-1:no repeat,0:always repeat
			e.SetRepeat(0);
			for (int i = 0, count = imageFilePaths.Length; i < count; i++ ) 
			{
				e.AddFrame( Image.FromFile( imageFilePaths[i] ) );
			}
			e.Finish();
			/* extract Gif */
			string outputPath = "c:\\";
            //GifDecoder gifDecoder = new GifDecoder();
            //gifDecoder.Read( "c:\\test.gif" );
            //for ( int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++ ) 
            //{
            //    Image frame = gifDecoder.GetFrame( i );  // frame i
            //    frame.Save( outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png );
            //}
		}
	}
}
