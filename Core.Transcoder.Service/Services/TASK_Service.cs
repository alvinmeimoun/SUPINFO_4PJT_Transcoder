using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
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

        public bool AddTask(TASK task)
        {
            if(UoW.TaskRepository.Insert(task) == true)
            {
                UoW.Save();
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
