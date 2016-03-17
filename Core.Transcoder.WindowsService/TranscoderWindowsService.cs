using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.FFmpegWrapper;
using System.Threading;
using Core.Transcoder.Service;
using Core.Transcoder.DataAccess;
using Core.Transcoder.Service.Utils;

namespace Core.Transcoder.WindowsService
{
    public partial class TranscoderWindowsService : ServiceBase
    {
        public TranscoderWindowsService()
        {
            InitializeComponent();
            // DEBUG
          
        }

        public void StartDebug()
        {
            RunTask runTask = new RunTask();
            runTask.Run();
        }

        protected override void OnStart(string[] args)
        {
            RunTask runTask = new RunTask();
            runTask.Run();
        }

        protected override void OnStop()
        {
        }
    }
    public class RunTask
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private object _lockObject = new object();

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
                TASK task = taskService.GetListOfTaskByStatusToDoOrToMerge().FirstOrDefault();

                lock(_lockObject)
                {
                    if (task != null)
                    {
                        task.DATE_BEGIN_CONVERSION = DateTime.Now;
                        taskService.AddOrUpdateTask(task);
                        new TRACE_Service().AddTrace(new TRACE() { FK_ID_TASK = task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, METHOD = "INITIALISATION TASK", DESCRIPTION = "Récupération de la tache à effectuer" });
                        TranscoderService.DoFFmpegConversion(task);
                    }
                }
                await Task.Delay(60000);
            }
        }



    }

}
