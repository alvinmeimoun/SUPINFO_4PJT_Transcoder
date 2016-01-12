using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using Core.Transcoder.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
   public class TASK_Service
    {

        public UnitOfWork UoW = new UnitOfWork();

        public bool AddOrUpdateTask(TASK task)
        {
            if(task.PK_ID_TASK != 0)
            {
                UoW.TASK_Repository.Update(task);
                UoW.Save();
                return true;
            }
            else
            {
                UoW.TASK_Repository.Insert(task);
                UoW.Save();
                return true;
            }
        }

        public List<TASK> GetListOfTaskByStatus(EnumManager.PARAM_TASK_STATUS Status)
        {
            int statut = ((int)Status);
            List<TASK> listTasks = UoW.TASK_Repository.Get(x => x.STATUS == statut, q => q.OrderBy(s => s.PK_ID_TASK), "").ToList();
            return listTasks;
        }

    }
}
