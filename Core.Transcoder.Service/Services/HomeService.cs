using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.Service.Enums;

namespace Core.Transcoder.Service.Services
{
    public class HomeService : BaseService
    {

        public HomeViewModel GenerateHomeViewModel(bool isLogged)
        {
            var statsService = new StatsService();

                var vm = new HomeViewModel
                {
                    IsLogged = isLogged,
                    UsersCount = statsService.GetUsersCount(),
                    TaskCount = statsService.GetTasksCount(),
                    CombinationCount = statsService.GetFormatCount()
                };
               return vm;
        }

        public HomeAuthViewModel GetDataFromUserId(int userId)
        {
            var taskService = new TASK_Service();
            var lastTask = taskService.GetLastTaskByUserId(userId);
         
            var vm = new HomeAuthViewModel();
            if (lastTask != null)
            {
                var status = new PARAM_TASK_STATUS_Service().GetAll().Where(x => x.PK_ID_STATUS == lastTask.STATUS).FirstOrDefault();
                string fileName = lastTask.FILE_URL.Substring(lastTask.FILE_URL.LastIndexOf(@"\") + 1);
                vm = new HomeAuthViewModel()
                {
                    DateDemande = lastTask.TRANSACTION.DATE_TRANSACTION,
                    LastTranscode = fileName,
                    Status = status.LIBELLE
                };
            }
            return vm;
        }

    }
}
