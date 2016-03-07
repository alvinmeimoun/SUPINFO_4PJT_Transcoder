using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.DataAccess.ViewModels;

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
                //TaskCount = statsService.GetTasksCount(),
                CombinationCount = statsService.GetFormatCount()
            };

            return vm;
        }

    }
}
