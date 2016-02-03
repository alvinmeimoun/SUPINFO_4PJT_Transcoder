using Core.Transcoder.DataAccess;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Core.Transcoder.Service.Enums;

namespace Core.Transcoder.WindowsService
{
    public static class TranscoderService
    {
        private static string rootFolder = ConfigurationManager.AppSettings["TranscoderTempRoot"];
        private static string sourceFolder = ConfigurationManager.AppSettings["TranscoderTempRootSource"];
        private static string destinationFolder = ConfigurationManager.AppSettings["TranscoderTempRootDestination"];

        public static bool DoFFmpegConversion(TASK Task)
        {
            InitWorkspace();
            // on récupère le format de conversion de la tache
            FORMAT formatToConvert = new FORMAT_Service().GetFormatById((int)Task.FK_ID_FORMAT_TO_CONVERT);

            try
            {  // On vérifie si la tache est à reassembler ou pas
                if (Task.STATUS == (int)EnumManager.PARAM_TASK_STATUS.A_REASSEMBLER)
                {
                    // On met le statut en cours
                    Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EN_COURS;
                    new TASK_Service().AddOrUpdateTask(Task);

                    // On liste les sous taches
                    List<TASK> listOfSubTaskByParent = new TASK_Service().GetSubTaskByMotherTask(Task.PK_ID_TASK);
                    // On va merge les sous taches pour renvoyer la conversion complete
                    bool isMerged = FFmpegMergeSplits(Task, listOfSubTaskByParent);
                    // si le merge s'est bien passé on change le statut de notre Tache.
                    if (isMerged)
                    {
                        Task.DATE_END_CONVERSION = DateTime.Now;
                        Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE;
                    }
                }
                else
                // Dans le cas ou la tache n'a pas besoin d'etre splitté, on fait le traitement habituel.
                if (!GetTaskAndSetIfTaskIsSplitted(Task, formatToConvert))
                {
                    // On lance la conversion
                    ConvertTask(Task, formatToConvert);

                    // Verification si c'est une sous tache
                    // On vérifie si les sous taches ont été effectuées ou pas.
                    if (Task.FK_ID_PARENT_TASK != null)
                    {
                        List<TASK> listOfSubTaskByParent = new TASK_Service().GetSubTaskByMotherTask((int)Task.FK_ID_PARENT_TASK);
                        int totalEffectue = listOfSubTaskByParent.Count(x => x.STATUS == (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE);
                        if (totalEffectue.Equals(listOfSubTaskByParent.Count))
                        {
                            TASK MotherTask = new TASK_Service().GetTaskById((int)Task.FK_ID_PARENT_TASK);
                            MotherTask.STATUS = (int)EnumManager.PARAM_TASK_STATUS.A_REASSEMBLER;
                            new TASK_Service().AddOrUpdateTask(MotherTask);
                        }
                    }
                }
                new TASK_Service().AddOrUpdateTask(Task);
                return true;
            }
            catch (Exception e)
            {

                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(Task);

                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, DATE_TRACE=DateTime.Now, NOM_SERVER= System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = e.Message, METHOD = "CONVERSION FFMPEG", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
                return false;
            }
        }

        public static void ConvertTask(TASK Task, FORMAT formatToConvert)
        {
            try
            {
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EN_COURS;
                new TASK_Service().AddOrUpdateTask(Task);
                FFMpegService.Execute(Task.FILE_URL_TEMP, formatToConvert.FORMAT_FFMPEG_VALUE, Task.FILE_URL_DESTINATION);
                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE;
            }
            catch(Exception e)
            {
                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(Task);

                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, DESCRIPTION = e.Message, METHOD = "Conversion FFMPEG Convert Task", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
            }

        }

        public static void InitWorkspace()
        {
            CreateWorkDirectories(rootFolder);
            CreateWorkDirectories(sourceFolder);
            CreateWorkDirectories(destinationFolder);
        }

        public static bool GetTaskAndSetIfTaskIsSplitted(TASK Task, FORMAT Format)
        {
            bool result = false;
            try
            {
                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, METHOD = "CREATION DES SPLIT TEMPORAIRES", TYPE = "INFO" };
                new TRACE_Service().AddTrace(Trace);

                string fileUrl = Task.FILE_URL;
                int count = (fileUrl.LastIndexOf(@"\") + 1);
                string fileName = fileName = fileUrl.Substring(count);

                // si le fichier n'a pas été encore copié, on le copie dans notre repertoire temporaire.
                if (Task.FILE_URL_TEMP == null)
                {
                    Task.FILE_URL_TEMP = sourceFolder + @"\" + fileName;
                    if (File.Exists(fileUrl))
                    {
                        if (File.Exists(Task.FILE_URL_TEMP))
                        {
                            File.Delete(Task.FILE_URL_TEMP);
                        }
                        File.Copy(fileUrl, Task.FILE_URL_TEMP);
                    }
                }
                // On verifie si la tache doit être splittée ou non, si c'est le cas nous la splittons
                bool isSplitted = VerifyTaskLengthAndSplitTask(Task);
                if (isSplitted)
                {
                    Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.DECOUPE;
                    result = true;
                }
                else
                {
                    count = (fileName.LastIndexOf('.') + 1);
                    fileName = fileName.Substring(0, count);
                    Task.FILE_URL_DESTINATION = destinationFolder + @"\" + fileName + Format.FORMAT_NAME;
                    result = false;
                }
                new TASK_Service().AddOrUpdateTask(Task);

                return result;
            }
            catch (Exception e)
            {
                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, DESCRIPTION = e.Message, METHOD = "CREATION DES FICHIERS TEMPORAIRES", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
                return false;
            }
        }

        public static void CreateWorkDirectories(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception e)
            {
                TRACE Trace = new TRACE { FK_ID_SERVER = 1, DESCRIPTION = e.Message, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, METHOD = "CREATION DES REPERTOIRES " + folder, TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
            }
        }

        public static bool VerifyTaskLengthAndSplitTask(TASK Task)
        {
            int MaxLength;
            //On récupère la taille maximum sans split
            string MaxLengthString = new CONFIG_Service().GetConfigValueById((int)EnumManager.CONFIG.MAXLENGTHWITHOUTSPLIT);
            int.TryParse(MaxLengthString, out MaxLength);
            // Si la tache est trop lourde on la split
            if (Task.LENGTH >= MaxLength)
            {
                VideoFile VideoFile = new VideoFile(Task.FILE_URL);
                VideoFile.GetVideoInfo();
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.SPLIT_EN_COURS;
                new TASK_Service().AddOrUpdateTask(Task);
                CreateSplits(Task, VideoFile);
                return true;
            }
            return false;
        }

        public static void CreateSplits(TASK MotherTask, VideoFile VideoFile)
        {
            // On récupère le nombre de split a effectuer
            int Splits = GetNumberOfSplits(MotherTask);
            int SplitsTotal = Splits;
            // On récupère la longueur de chaque split
            TimeSpan timeSpan = new TimeSpan(VideoFile.Duration.Ticks / Splits);
            // On set le debut du premier split
            TimeSpan begin = new TimeSpan(0);
            // on récupère la durée totale de la video
            long durationTotal = VideoFile.Duration.Ticks;
            // On crée notre param split
            PARAM_SPLIT paramSplit = new PARAM_SPLIT();

            while (Splits != 0)
            {
                if (Splits == SplitsTotal)
                {   
                    long stopSpan = new TimeSpan(begin.Ticks + timeSpan.Ticks).Ticks;
                    paramSplit = new PARAM_SPLIT() { BEGIN_PARAM_SPLIT = begin.Ticks.ToString(), END_PARAM_SPLIT = stopSpan.ToString() };
                }
                else
                {
                    long stopSpan = new TimeSpan(timeSpan.Ticks + (durationTotal - timeSpan.Ticks)).Ticks;
                    paramSplit = new PARAM_SPLIT() { BEGIN_PARAM_SPLIT = timeSpan.Ticks.ToString(), END_PARAM_SPLIT = stopSpan.ToString() };
                }

                new PARAM_SPLIT_Service().AddOrUpdateParamSplit(paramSplit);
                // On crée une sous tache associé au paramSplit et la tache mere
                CreateSubTask(MotherTask, paramSplit);

                Splits--;
            }
        }

        public static void CreateSubTask(TASK MotherTask, PARAM_SPLIT paramSplit)
        {
            TASK subTask_1 = new TASK();
            subTask_1.FK_ID_PARENT_TASK = MotherTask.PK_ID_TASK;
            subTask_1.FK_ID_USER = MotherTask.FK_ID_USER;
            subTask_1.FK_ID_FORMAT_TO_CONVERT = MotherTask.FK_ID_FORMAT_TO_CONVERT;
            subTask_1.FK_ID_PARAM_SPLIT = paramSplit.PK_ID_PARAM_SPLIT;
            // On set l'url de la tache mere pour que le service puisse aller créer le split a partir du fichier de base
            subTask_1.FILE_URL = MotherTask.FILE_URL_TEMP;
            new TASK_Service().AddOrUpdateTask(subTask_1);
            CreateSplit(subTask_1, paramSplit);

        }

        public static void CreateSplit(TASK SubTask, PARAM_SPLIT ParamSplit)
        {
            try
            {
                VideoFile split = new VideoFile(SubTask.FILE_URL);
                int count = SubTask.FILE_URL.LastIndexOf('.');
                string leftPart = SubTask.FILE_URL.Substring(0, count);
                string format = SubTask.FILE_URL.Substring(count);
                string fullName = leftPart + "_" + SubTask.PK_ID_TASK.ToString() + format;

                SubTask.FILE_URL = fullName;
                SubTask.FILE_URL_TEMP = fullName;
                // On met un statut temporaire a 0 pour initialiser la tache
                SubTask.STATUS = (int)EnumManager.PARAM_TASK_STATUS.SPLIT_EN_COURS;
                new TASK_Service().AddOrUpdateTask(SubTask);
                // on extrait notre split
                split.ExtractVideoSegment(fullName, Convert.ToInt64(ParamSplit.BEGIN_PARAM_SPLIT), Convert.ToInt64(ParamSplit.END_PARAM_SPLIT), Core.Transcoder.FFmpegWrapper.Videos.VideoFormat.avi);
                SubTask.STATUS = (int)EnumManager.PARAM_TASK_STATUS.A_FAIRE;
                new TASK_Service().AddOrUpdateTask(SubTask);
            }
            catch (Exception e)
            {
                SubTask.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(SubTask);

                TRACE trace = new TRACE()
                {
                    FK_ID_TASK = SubTask.PK_ID_TASK,
                    FK_ID_SERVER = 1,
                    METHOD = "FFMPEG Split",
                    TYPE = "ERROR",
                    DESCRIPTION = e.Message ,
                    DATE_TRACE = DateTime.Now,
                    NOM_SERVER = System.Environment.MachineName

                };
                new TRACE_Service().AddTrace(trace);
            }

        }

        public static int GetNumberOfSplits(TASK task)
        {
            if (task.LENGTH >= 1000)
            {
                return 5;
            }
            if (task.LENGTH >= 600)
            {
                return 4;
            }
            if (task.LENGTH >= 300)
            {
                return 2;
            }
            return 1;
        }

        public static bool FFmpegMergeSplits(TASK Task, List<TASK> ListSubTasks)
        {
            try
            {
                // On récupère la liste des urls d'acces des sous taches pour reassemblage
                List<string> listOfUrls = new List<string>();
                foreach (var item in ListSubTasks)
                {
                    listOfUrls.Add(item.FILE_URL_DESTINATION);
                }
                // On va former l'url de destination du fichier de base 
                string fileUrl = Task.FILE_URL_TEMP;
                int count = (fileUrl.LastIndexOf(@"\") + 1);
                string fileName = fileName = fileUrl.Substring(count);
                count = (fileName.LastIndexOf('.') + 1);
                fileName = fileName.Substring(0, count);
                Task.FILE_URL_DESTINATION = destinationFolder + @"\" + fileName + new FORMAT_Service().GetFormatById((int)Task.FK_ID_FORMAT_TO_CONVERT).FORMAT_NAME;
                
                // On merge les splits
                VideoFile.MergeVideoSegment(listOfUrls, Task.FILE_URL_DESTINATION);
                return true;
            }
            catch (Exception e)
            {
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(Task);

                TRACE trace = new TRACE()
                {
                    FK_ID_TASK = Task.PK_ID_TASK,
                    FK_ID_SERVER = 1,
                    DATE_TRACE = DateTime.Now,
                    NOM_SERVER = System.Environment.MachineName,
                    METHOD = "FFMPEG Merge Split",
                    TYPE = "ERROR",
                    DESCRIPTION = e.Message 

                };
                new TRACE_Service().AddTrace(trace);
                return false;
            }
        }

    }
}
