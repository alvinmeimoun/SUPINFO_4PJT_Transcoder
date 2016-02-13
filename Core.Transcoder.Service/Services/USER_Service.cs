using Core.Transcoder.DataAccess;
using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class USER_Service
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


        public USER LoginUser(string Username, string password)
        {
            return UoW.USER_Repository.Get(x => x.USERNAME == Username && x.PASSWORD == password).FirstOrDefault();

        }
        public bool FindUserByUserName(string Username)
        {
            return UoW.USER_Repository.Get(x => x.USERNAME == Username).FirstOrDefault() != null ? true : false;
        }
        public USER FindUserByID(int UserID)
        {
            return UoW.USER_Repository.GetByID(UserID);
        }
        public bool AddOrUpdateUser(USER user)
        {
            if (user.PK_ID_USER != 0)
            {

                UoW.USER_Repository.Update(user);
                UoW.Save();
                return true;
            }
            else
            {
                UoW.USER_Repository.Insert(user);
                UoW.Save();
                return true;
            }
        }

        public EditUserViewModel GetEditUserViewModelByID(int UserID)
        {
            var user = FindUserByID(UserID);
            EditUserViewModel model = new EditUserViewModel(user);

            return model;
        }

    }
}
