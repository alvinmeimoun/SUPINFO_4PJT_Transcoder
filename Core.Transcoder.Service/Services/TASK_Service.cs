using Core.Transcoder.DataAccess;
using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.Repository;
using Core.Transcoder.Service.Enums;
using Core.Transcoder.Service.Services;
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

        public bool DeleteTaskById(int id)
        {
            try
            {
                UoW.TASK_Repository.Delete(id);
                return true;
            }
            catch (Exception e)
            {
                return false;
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
            var user = UoW.USER_Repository.GetByID(userId);
            var shortEditUserViewModel = new ShortEditUserViewModel(user);

            return new CreateTaskViewModel(userId,listFormatTypes,listFormat, shortEditUserViewModel);
        }

       
        public bool AddTaskByViewModel(CreateTaskViewModel model)
        {
            // On update les infos user
            var user = UoW.USER_Repository.GetByID(model.FK_ID_USER);
            user.LASTNAME = model.ShortEditUserViewModel.Lastname;
            user.FIRSTNAME = model.ShortEditUserViewModel.Firstname;
            user.EMAIL = model.ShortEditUserViewModel.Email;

            bool userEdited = new USER_Service().AddOrUpdateUser(user);
            // on créé la tache
            var task = new TASK();
            task.STATUS = (int)EnumManager.PARAM_TASK_STATUS.A_FAIRE;
            task.CreateFromModel(model);


            return AddOrUpdateTask(task);
        }

        public PanierViewModel GetPanierViewModel(int userId)
        {
            PanierViewModel panier = new PanierViewModel();
            panier.ListOfConversions = GetListTaskViewModelByUserId(userId).Where( x=> x.IS_PAID == false).ToList();
            panier.GlobalPrice = panier.ListOfConversions.Sum(x => x.PRICE);

            return panier;

        }

        public List<TASK> GetListOfTaskByUserId(int userId)
        {
            return UoW.TASK_Repository.Get(x => x.FK_ID_USER == userId, q => q.OrderBy(s => s.PK_ID_TASK), "").ToList();
        }

        public List<ListTaskViewModel> GetListTaskViewModelByUserId(int userId)
        {
            var listOfFormat = new FORMAT_Service().GetAll();

            var query = (from task in GetListOfTaskByUserId(userId)
                         join status in new PARAM_TASK_STATUS_Service().GetAll() on task.STATUS equals status.PK_ID_STATUS
                         select new ListTaskViewModel
                         {
                             PK_ID_TASK = task.PK_ID_TASK,
                             FILE_URL_ACCESS = task.FILE_URL.Substring(task.FILE_URL.LastIndexOf(@"\") + 1),
                             FK_ID_USER = (int)task.FK_ID_USER,
                             FORMAT_BASE = listOfFormat.Find(x => x.PK_ID_FORMAT == task.FK_ID_FORMAT_BASE).FORMAT_NAME,
                             FORMAT_CONVERT = listOfFormat.Find(x => x.PK_ID_FORMAT == task.FK_ID_FORMAT_TO_CONVERT).FORMAT_NAME,
                             LENGTH = (double)task.LENGTH,
                             STATUS = status.LIBELLE,
                             PRICE = (double)task.PRICE,
                             DURATION = (double)task.DURATION,
                             IS_PAID = (bool)task.IS_PAID
                         });

            var listOfTasks = query.ToList();

            return listOfTasks;

        }

    }
}
