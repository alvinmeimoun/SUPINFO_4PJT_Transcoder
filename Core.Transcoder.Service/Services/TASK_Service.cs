using Core.Transcoder.DataAccess;
using Core.Transcoder.DataAccess.ViewModels;
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
        private UnitOfWork uow;
        public UnitOfWork UoW
        {
            get
            {
                if (uow == null)
                {
                    uow = new UnitOfWork();
                }
                return uow;
            }
            
        }

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

        public List<TASK> GetListOfTaskByStatusToDoOrToMerge()
        {

            List<TASK> listTasks = UoW.TASK_Repository.Get(x => x.STATUS == (int)EnumManager.PARAM_TASK_STATUS.A_FAIRE 
            || x.STATUS == (int)EnumManager.PARAM_TASK_STATUS.A_REASSEMBLER, 
            q => q.OrderBy(s => s.PK_ID_TASK), "").ToList();
            return listTasks;
        }

        public List<TASK> GetSubTaskByMotherTask(int motherTaskId)
        {
            return UoW.TASK_Repository.Get(x => x.FK_ID_PARENT_TASK == motherTaskId, q => q.OrderBy(s => s.PK_ID_TASK), "").ToList();
        }

        public TASK GetTaskById(int Id)
        {
            return UoW.TASK_Repository.GetByID(Id);
        }



        public CreateTaskViewModel InitCreateTaskViewModel(int userId)
        {
            var listFormatTypes = UoW.FORMAT_TYPE_Repository.Get(null, q => q.OrderBy(s => s.PK_ID_FORMAT_TYPE), "").ToList();
            var listFormat = new FORMAT_Service().GetAll();
            return new CreateTaskViewModel(userId,listFormatTypes,listFormat);
        }

        public bool AddTaskByViewModel(CreateTaskViewModel model)
        {
            var task = new TASK();
            task.CreateFromModel(model);

            return AddOrUpdateTask(task);
        }

    }
}
