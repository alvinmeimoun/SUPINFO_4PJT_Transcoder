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
using Core.Transcoder.Service.Utils;

namespace Core.Transcoder.WindowsService
{
    public static class TranscoderService
    {
        private static string rootFolder = new CONFIG_Service().GetConfigValueByName("TranscoderTempRoot");
        //ConfigurationManager.AppSettings["TranscoderTempRoot"];
        private static string sourceFolder = new CONFIG_Service().GetConfigValueByName("TranscoderTempRootSource");
        //ConfigurationManager.AppSettings["TranscoderTempRootSource"];
        private static string destinationFolder = new CONFIG_Service().GetConfigValueByName("TranscoderTempRootDestination");
        //ConfigurationManager.AppSettings["TranscoderTempRootDestination"];

        public static bool DoFFmpegConversion(TASK Task)
        {
            InitWorkspace();
            // on récupère le format de conversion de la tache
            FORMAT formatToConvert = new FORMAT_Service().GetFormatById((int)Task.FK_ID_FORMAT_TO_CONVERT);
            FORMAT formatBase = new FORMAT_Service().GetFormatById((int)Task.FK_ID_FORMAT_BASE);
            var formatTypeBase = new FORMAT_TYPE_Service().findById((int)formatBase.FK_ID_FORMAT_TYPE);

            FORMAT_TYPE formatTypeDestination = new FORMAT_TYPE_Service().findById((int)Task.FORMAT.FK_ID_FORMAT_TYPE);
            // S'il s'agit d'une extraction audio
            if (formatTypeDestination != null && formatTypeBase.PK_ID_FORMAT_TYPE == (int)EnumManager.FORMAT_TYPE.VIDEO && formatTypeDestination.PK_ID_FORMAT_TYPE == (int)EnumManager.FORMAT_TYPE.AUDIO)
            {
                return ExtractAudioSegment(Task);
            }
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
                        // On envoie la notification par mail
                        MailUtil.SendMail(StringManager.CONVERSION_TERMINEE, Task);
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
                    if(Task.STATUS == (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE && Task.FK_ID_PARENT_TASK == null)
                    {
                        // On envoie la notification par mail
                        MailUtil.SendMail(StringManager.CONVERSION_TERMINEE, Task);
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

                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, DATE_TRACE=DateTime.Now, NOM_SERVER= System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = e.Message +  " " + e.InnerException, METHOD = "CONVERSION FFMPEG", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
                return false;
            }
        }

        public static bool ExtractAudioSegment(TASK Task)
        {
            try
            {
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EN_COURS;
                Task.DATE_BEGIN_CONVERSION = DateTime.Now;
                string fileName = GetFileName(Task);

                CopyFileInTempFolder(fileName, Task);
                new TASK_Service().AddOrUpdateTask(Task);

                VideoFile VideoFile = new VideoFile(Task.FILE_URL_TEMP);
                VideoFile.GetVideoInfo();
                // On set le debut du premier split
                TimeSpan begin = new TimeSpan(0);
                // on récupère la durée totale de la video
                long durationTotal = VideoFile.Duration.Ticks;
                int count = (fileName.LastIndexOf('.') + 1);
                fileName = fileName.Substring(0, count);
                Task.FILE_URL_DESTINATION = destinationFolder + @"\" + fileName + Task.FORMAT.FORMAT_NAME;
                // A voir pour l'extraction d'un morceau de son particulier
                VideoFile.ExtractAudioSegment(begin.Ticks, durationTotal, Task.FORMAT.FORMAT_NAME, Task.FILE_URL_DESTINATION);
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE;
                Task.DATE_END_CONVERSION = DateTime.Now;
                new TASK_Service().AddOrUpdateTask(Task);
                return true;
            }
            catch(Exception e)
            {
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                Task.DATE_END_CONVERSION = DateTime.Now;
                new TASK_Service().AddOrUpdateTask(Task);
                var trace = new TRACE() { FK_ID_TASK = Task.PK_ID_TASK, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, DESCRIPTION = e.Message +  " " + e.InnerException, METHOD = "Erreur lors de l'extraction audio", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(trace);
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
                bool fileIsAvailable = CheckFileIsAvailable(Task.FILE_URL_DESTINATION);

                if (!fileIsAvailable)
                    throw new Exception();

                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.EFFECTUE;
            }
            catch(Exception e)
            {
                Task.DATE_END_CONVERSION = DateTime.Now;
                Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(Task);

                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, DESCRIPTION = e.Message +  " " + e.InnerException + " " + e.InnerException, METHOD = "Conversion FFMPEG Convert Task", TYPE = "ERROR" };
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
                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, NOM_SERVER = System.Environment.MachineName, DATE_TRACE = DateTime.Now, METHOD = "GetTaskAndSetIfTaskIsSplitted", TYPE = "INFO" };
                new TRACE_Service().AddTrace(Trace);

                string fileName = GetFileName(Task);

                // si le fichier n'a pas été encore copié, on le copie dans notre repertoire temporaire.
                CopyFileInTempFolder(fileName, Task);

                // On verifie si la tache doit être splittée ou non, si c'est le cas nous la splittons
                bool isSplitted = VerifyTaskLengthAndSplitTask(Task);
                if (isSplitted)
                {
                    Task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.DECOUPE;
                    result = true;
                }
                else
                {
                    int count = (fileName.LastIndexOf('.') + 1);
                    fileName = fileName.Substring(0, count);
                    Task.FILE_URL_DESTINATION = destinationFolder + @"\" + fileName + Format.FORMAT_NAME;
                    result = false;
                }
                new TASK_Service().AddOrUpdateTask(Task);

                return result;
            }
            catch (Exception e)
            {
                TRACE Trace = new TRACE { FK_ID_TASK = Task.PK_ID_TASK, FK_ID_SERVER = 1, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, DESCRIPTION = e.Message +  " " + e.InnerException +  " " + e.InnerException, METHOD = "CREATION DES FICHIERS TEMPORAIRES", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
                return false;
            }
        }

        public static string GetFileName(TASK Task)
        {
            int count = (Task.FILE_URL.LastIndexOf(@"\") + 1);
            string fileName = Task.FILE_URL.Substring(count);

            return fileName;
        }

        public static void CopyFileInTempFolder(string fileName, TASK Task)
        {
            if (Task.FILE_URL_TEMP == null)
            {
                Task.FILE_URL_TEMP = sourceFolder + @"\" + fileName;
                if (File.Exists(Task.FILE_URL))
                {
                    if (File.Exists(Task.FILE_URL_TEMP))
                    {
                        File.Delete(Task.FILE_URL_TEMP);
                    }
                    File.Copy(Task.FILE_URL, Task.FILE_URL_TEMP);
                }
            }
        }

        public static bool CheckFileIsAvailable(string fileUrl)
        {
            if (File.Exists(fileUrl))
            {
                return true;
            }
            return false;
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
                TRACE Trace = new TRACE { FK_ID_SERVER = 1, DESCRIPTION = e.Message +  " " + e.InnerException, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, METHOD = "CREATION DES REPERTOIRES " + folder, TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
            }
        }

        public static bool VerifyTaskLengthAndSplitTask(TASK Task)
        {
            // Si c'est une sous tache on ne split pas 
            if (Task.FK_ID_PARENT_TASK != null)
                return false;
            //On récupère la taille maximum sans split
            var listParamLength = new PARAM_LENGTH_Service().GetAll().ToList();
            //int.TryParse(MaxLengthString, out MaxLength);
            // Si la tache est trop lourde on la split
            int megabytes = (int)ConverterUtil.ConvertBytesToMegabytes((double)Task.LENGTH);
            if (listParamLength.Where( x => x.LENGTH <= megabytes).FirstOrDefault() != null)
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
                    long beginSpan = timeSpan.Ticks * (SplitsTotal - Splits);
                    long stopSpan = (durationTotal / SplitsTotal) + beginSpan;
                    
                    paramSplit = new PARAM_SPLIT() { BEGIN_PARAM_SPLIT = beginSpan.ToString(), END_PARAM_SPLIT = stopSpan.ToString() };
                }

                new PARAM_SPLIT_Service().AddOrUpdateParamSplit(paramSplit);
                // On crée une sous tache associé au paramSplit et la tache mere
                CreateSubTask(MotherTask, paramSplit, SplitsTotal);

                Splits--;
            }
        }

        public static void CreateSubTask(TASK MotherTask, PARAM_SPLIT paramSplit, int nbSplitsTotal)
        {
            TASK subTask = new TASK();
            subTask.FK_ID_PARENT_TASK = MotherTask.PK_ID_TASK;
            subTask.FK_ID_USER = MotherTask.FK_ID_USER;
            subTask.IS_PAID = MotherTask.IS_PAID;
            subTask.FK_ID_TRANSACTION = MotherTask.FK_ID_TRANSACTION;
            subTask.FK_ID_FORMAT_BASE = MotherTask.FK_ID_FORMAT_BASE;
            subTask.FK_ID_FORMAT_TO_CONVERT = MotherTask.FK_ID_FORMAT_TO_CONVERT;
            subTask.FK_ID_PARAM_SPLIT = paramSplit.PK_ID_PARAM_SPLIT;
            subTask.LENGTH = (int)MotherTask.LENGTH/nbSplitsTotal;

            // On set l'url de la tache mere pour que le service puisse aller créer le split a partir du fichier de base
            subTask.FILE_URL = MotherTask.FILE_URL_TEMP;
            new TASK_Service().AddOrUpdateTask(subTask);
            CreateSplit(subTask, paramSplit);

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
                    DESCRIPTION = e.Message +  " " + e.InnerException ,
                    DATE_TRACE = DateTime.Now,
                    NOM_SERVER = System.Environment.MachineName

                };
                new TRACE_Service().AddTrace(trace);
            }

        }

        public static int GetNumberOfSplits(TASK task)
        {
            var listParam = new PARAM_LENGTH_Service().GetAll().OrderByDescending(q => q.PK_ID_PARAM_LENGTH);
            try
            {
                double megabytes = ConverterUtil.ConvertBytesToMegabytes((double)task.LENGTH);  
                foreach(var param in listParam)
                {
                    if(megabytes >= param.LENGTH)
                    {
                        return (int)param.NB_OF_SPLITS;
                    }
                }
                return 1;
            }
            catch (Exception e)
            {
                task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.ERREUR;
                new TASK_Service().AddOrUpdateTask(task);

                TRACE trace = new TRACE()
                {
                    FK_ID_TASK = task.PK_ID_TASK,
                    FK_ID_SERVER = 1,
                    METHOD = "GetNumberOfSplits problème lors de la recupération de la length",
                    TYPE = "ERROR",
                    DESCRIPTION = e.Message +  " " + e.InnerException,
                    DATE_TRACE = DateTime.Now,
                    NOM_SERVER = System.Environment.MachineName

                };
                new TRACE_Service().AddTrace(trace);
                return 0;
            }
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
                VideoFile.MergeVideoWithSplits(listOfUrls, Task.FILE_URL_DESTINATION);

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
                    DESCRIPTION = e.Message +  " " + e.InnerException 

                };
                new TRACE_Service().AddTrace(trace);
                return false;
            }
        }


    }
}
