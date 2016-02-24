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
            if (string.IsNullOrWhiteSpace(password)) return null; //Case append generally when trying to log with an account without password (created with external provider and standard password not defined)

            return UoW.USER_Repository.Get(x => x.USERNAME == Username && x.PASSWORD == password).FirstOrDefault();
        }
        public USER FindUserByUserName(string Username)
        {
            return UoW.USER_Repository.Get(x => x.USERNAME == Username).FirstOrDefault();
        }

        public USER FindByEmail(string email)
        {
            return UoW.USER_Repository.Get(x => x.EMAIL == email).FirstOrDefault();
        }

        public USER FindUserByID(int UserID)
        {
            return UoW.USER_Repository.GetByID(UserID);
        }

        public IEnumerable<USER> FindAllByUsernameStartWith(string startWithValue)
        {
            return UoW.USER_Repository.Get(x => x.USERNAME.StartsWith(startWithValue));
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
