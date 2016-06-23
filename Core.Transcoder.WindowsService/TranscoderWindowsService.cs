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
using System.Runtime.InteropServices;
using System.IO;
using System.Configuration;

namespace Core.Transcoder.WindowsService
{
    public partial class TranscoderWindowsService : ServiceBase
    {
        private object _lockObject = new object();

        // This is a flag to indicate the service status
        private static bool serviceStarted = true;

        // the thread that will do the work
        Thread workerThread;

        public TranscoderWindowsService()
        {
            InitializeComponent();
        }

        public void StartDebug()
        {
            while (true)
            {
                try
                {
                    CreateFileData("Events.txt", "avant requete : ok");
                    TASK_Service taskService = new TASK_Service();
                    TASK task = taskService.GetListOfTaskByStatusToDoOrToMerge().FirstOrDefault();
                    CreateFileData("Events.txt", "tache trouvée : " + task.PK_ID_TASK.ToString());

                    lock (_lockObject)
                    {
                        CreateFileData("Events.txt", "lock : ok");
                        if (task != null)
                        {
                            CreateFileData("Events.txt", "tache not null : ok");
                            task.DATE_BEGIN_CONVERSION = DateTime.Now;
                            taskService.UpdateTask(task);
                            new TRACE_Service().AddTrace(new TRACE() { FK_ID_TASK = task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, METHOD = "INITIALISATION TASK", DESCRIPTION = "Récupération de la tache à effectuer" });
                            TranscoderService.DoFFmpegConversion(task);
                        }
                    }

                }
                catch (Exception f)
                {
                    string path = ConfigurationManager.AppSettings["TranscoderRootServiceForLogs"] + "Exception.txt";
                    StreamWriter oStreamWriter = new StreamWriter(path, true);
                    oStreamWriter.WriteLine(f.Message);
                    oStreamWriter.WriteLine(f.InnerException);
                    oStreamWriter.WriteLine(f.StackTrace);
                    oStreamWriter.Close();
                    oStreamWriter = null;
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            Trace.TraceInformation("Working");
            // Create worker thread; this will invoke the WorkerFunction
            // when we start it.
            // Since we use a separate worker thread, the main service
            // thread will return quickly, telling Windows that service has started
            ThreadStart st = new ThreadStart(WorkerFunction);
            workerThread = new Thread(st);

            // set flag to indicate worker thread is active
            serviceStarted = true;

            // start the thread
            workerThread.Start();

        }
        public int tick = 0;
        private void WorkerFunction()
        {
            // start an endless loop; loop will abort only when "serviceStarted" flag = false
            while (true)
            {
                try
                {
                    tick++;
                    CreateFileData("Events.txt", "tick: " + tick.ToString() +  " Date : " + DateTime.Now.ToString());
                    CreateFileData("Events.txt", "avant requete : ok");
                    TASK_Service taskService = new TASK_Service();
                    TASK task = taskService.GetLastTaskToConvert();


                    if (task != null)
                    {

                        CreateFileData("Events.txt", "tache trouvée : " + task.PK_ID_TASK.ToString());
                        lock (_lockObject)
                        {
                            CreateFileData("Events.txt", "tache not null et lock : ok");
                            task.DATE_BEGIN_CONVERSION = DateTime.Now;
                            taskService.UpdateTask(task);
                            new TRACE_Service().AddTrace(new TRACE() { FK_ID_TASK = task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, METHOD = "INITIALISATION TASK", DESCRIPTION = "Récupération de la tache à effectuer" });
                            TranscoderService.DoFFmpegConversion(task);
                        }
                    }

                }
                catch (Exception f)
                {
                    string path = ConfigurationManager.AppSettings["TranscoderRootServiceForLogs"] + "Exception.txt";
                    StreamWriter oStreamWriter = new StreamWriter(path, true);
                    oStreamWriter.WriteLine("DATE : " + DateTime.Now.ToString());
                    oStreamWriter.WriteLine(f.Message);
                    oStreamWriter.WriteLine(f.InnerException);
                    oStreamWriter.WriteLine(f.StackTrace);
                    oStreamWriter.Close();
                    oStreamWriter = null;
                    serviceStarted = false;
                }
                Thread.Sleep(5000);
                serviceStarted = true;
            }
            // time to end the thread
            // Thread.CurrentThread.Abort();
        }





        void CreateFileData(string name, string text)
        {
            string path = ConfigurationManager.AppSettings["TranscoderRootServiceForLogs"] + name;
            StreamWriter oStreamWriter = new StreamWriter(path, true);
            oStreamWriter.WriteLine(text);
            oStreamWriter.Close();
            oStreamWriter = null;
        }

        protected override void OnStop()
        {
            // flag to tell the worker process to stop
            //serviceStarted = false;
            // give it a little time to finish any pending work
            workerThread.Join(new TimeSpan(0, 2, 0));
            this.Stop();
            Thread.CurrentThread.Abort();
        }


    }


}
