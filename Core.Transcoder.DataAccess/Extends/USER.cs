using Core.Transcoder.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.DataAccess.ViewModels.User;

namespace Core.Transcoder.DataAccess
{
    public partial class USER
    {

        //public USER CreateFromModel()

        public USER CreateFromModel(RegisterViewModel model)
        {
            USERNAME = model.Username;
            EMAIL = model.Email;
            PASSWORD = model.Password;

            return this;
        }

        public USER EditFromModel(EditUserViewModel model)
        {
       
            PK_ID_USER = model.Pk_id_user;
            USERNAME = model.Username;
            EMAIL = model.Email;
            PASSWORD = model.Password;
            FIRSTNAME = model.Firstname;
            LASTNAME = model.Lastname;

            return this;
        }

        public USER CreateFromExternalLoginModel(LoginExternalViewModel model)
        {
            switch (model.ProviderType)
            {
                    case LoginExternalViewModel.Provider.Facebook:
                        USERNAME = "fb-" + model.ProviderUserId;
                        break;
                    case LoginExternalViewModel.Provider.Google:
                        USERNAME = "gl-" + model.ProviderUserId;
                        break;
            }

            EMAIL = model.Email;
            LASTNAME = model.LastName;
            FIRSTNAME = model.FirstName;

            return this;
        }
    }
}
