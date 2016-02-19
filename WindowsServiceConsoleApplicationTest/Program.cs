using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.DataAccess;
using Core.Transcoder.Service;
using Core.Transcoder.Service.Enums;
using System.Threading;
using System.Diagnostics;
using WindowsServiceConsoleApplicationTest;

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


            //RunTask runTask = new RunTask();
            //runTask.Run();
            //double test = ConvertBytesToMegabytes(220514438);
            //Console.WriteLine(test);

            //qConsole.Read();
        }


        public static double ConvertBytesToMegabytes(int bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}

    public class RunTask
    {
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);


    public void Run()
        {
            try
            {
                RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
            }

        }
        private async Task RunAsync(CancellationToken cancellationToken)
        {

            // TODO: Remplacez le texte suivant par votre propre logique.
            while (!cancellationToken.IsCancellationRequested)
            {

                Trace.TraceInformation("Working");
                TASK_Service taskService = new TASK_Service();

                List<TASK> listOfTasks = taskService.GetListOfTaskByStatusToDoOrToMerge();

                if (listOfTasks.Count() > 0)
                {
                    TASK task = listOfTasks.First();
                
                    task.DATE_BEGIN_CONVERSION = DateTime.Now;
                    taskService.AddOrUpdateTask(task);
                    new TRACE_Service().AddTrace(new TRACE() { FK_ID_TASK = task.PK_ID_TASK, FK_ID_SERVER = 1, METHOD = "INITIALISATION TASK", DESCRIPTION = "Récupération de la tache à effectuer" });
                    TranscoderService.DoFFmpegConversion(task);
                }

                await Task.Delay(60000);
            }
        }



}
