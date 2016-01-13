using Core.Transcoder.DataAccess;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.Service;
using Core.Transcoder.Service.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.WindowsService
{
   public static class TranscoderService
    {
        private static string rootFolder = ConfigurationManager.AppSettings["TranscoderTempRoot"];
        private static string sourceFolder = ConfigurationManager.AppSettings["TranscoderTempRootSource"];
        private static string destinationFolder = ConfigurationManager.AppSettings["TranscoderTempRootDestination"];

        public static bool DoFFmpegConvertion(TASK Task)
        {
            InitWorkspace();

            FORMAT formatToConvert = new FORMAT_Service().GetFormatById((int)Task.FK_ID_FORMAT_TO_CONVERT);
            CopyFilesInFolder(Task, formatToConvert);
            try
            {

                FFMpegService.Execute(Task.FILE_URL_TEMP, formatToConvert.FORMAT_FFMPEG_VALUE, Task.FILE_URL_DESTINATION);
                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE;
                new TASK_Service().AddOrUpdateTask(Task);

                return true;
            }
            catch (Exception e)
            {

                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(Task);

                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, DESCRIPTION = e.Message, METHOD = "CONVERSION FFMPEG", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
                throw e;
                return false;
            }
        }

        public static void InitWorkspace()
        {
            CreateWorkDirectories(rootFolder);
            CreateWorkDirectories(sourceFolder);
            CreateWorkDirectories(destinationFolder);
        }

        public static bool CopyFilesInFolder(TASK Task, FORMAT Format)
        {
            string fileUrl = Task.FILE_URL;
            // voir traitement
            int count = (fileUrl.LastIndexOf(@"\") + 1);
            string fileName = fileUrl.Substring(count);

            Task.FILE_URL_TEMP = sourceFolder + @"\" + fileName;
            count = (fileName.LastIndexOf('.') + 1);
            fileName = fileName.Substring(0, count);
            Task.FILE_URL_DESTINATION = destinationFolder + @"\" + fileName + Format.FORMAT_NAME;
            new TASK_Service().AddOrUpdateTask(Task);

            if (File.Exists(fileUrl))
            {
                if (File.Exists(Task.FILE_URL_TEMP))
                {
                    File.Delete(Task.FILE_URL_TEMP);
                }

                File.Copy(fileUrl, Task.FILE_URL_TEMP);

                return true;
            }
            return false;
        }

        public static void CreateWorkDirectories(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
