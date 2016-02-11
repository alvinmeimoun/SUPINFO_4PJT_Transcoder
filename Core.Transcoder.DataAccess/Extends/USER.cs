using Core.Transcoder.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess
{
    public partial class USER
    {

        //public USER CreateFromModel()

        public void CreateFromModel(RegisterViewModel model)
        {
            USERNAME = model.Username;
            EMAIL = model.Email;
            PASSWORD = model.Password;
        }
    }
}
