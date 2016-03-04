using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Services
{
    public class StatsService : BaseService
    {
        public int GetUsersCount()
        {
           return UoW.USER_Repository.Get().Count();
        }

        public int GetTasksCount()
        {
            return UoW.TASK_Repository.Get().Count();
        }

        public int GetFormatCount()
        {
            return UoW.FORMAT_Repository.Get().Count();
        }

    }
}
