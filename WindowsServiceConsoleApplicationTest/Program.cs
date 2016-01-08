using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.DataAccess;
using Core.Transcoder.Service;

namespace WindowsServiceConsoleApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // string fileBase = "D:\\Projets\TestsAssetsTranscoder\Music\HIN.wav";
            //FFMpegService.Execute("D:\\Projets\\TestsAssetsTranscoder\\Music\\HIN.wav", "-acodec libmp3lame", "D:\\Projets\\TestsAssetsTranscoder\\ConvertedMusic\\d.mp3");
            //FFMpegService.Execute("D:\\Projets\\TestsAssetsTranscoder\\Music\\Datsik.mp3", "-acodec pcm_u8", "D:\\Projets\\TestsAssetsTranscoder\\ConvertedMusic\\datsik.wav");

            //FFMpegService.Execute("D:\\Projets\\TestsAssetsTranscoder\\Video\\big_buck_bunny.avi", "-c:v libx264 -crf 19 -preset slow -c:a aac -strict experimental -b:a 192k -ac 2", "D:\\Projets\\TestsAssetsTranscoder\\ConvertedVideo\\big_buck_bunny.mp4");
            //
            // FFMpegService.Execute("D:\\Projets\\TestsAssetsTranscoder\\ConvertedVideo\\bbb.mp4", "-vcodec mpeg4 -acodec ac3 -ar 48000 -ab 192k", "D:\\Projets\\TestsAssetsTranscoder\\Video\\bbb.avi");

            TRACE trace = new TRACE();
            trace.FK_ID_SERVER = 2;
            trace.FK_ID_TASK = 1;
            trace.DESCRIPTION = "titi";
            trace.METHOD = "tito";
            trace.TYPE = "toto";

            new TRACE_Service().AddTrace(trace);

            List<TRACE> list = new List<TRACE>();
              list =  new TRACE_Service().GetAllTraces().ToList();

            


        }
    }
}
