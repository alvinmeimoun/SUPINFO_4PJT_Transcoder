using Core.Transcoder.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess
{
    public partial class TASK
    {
        public TASK()
        {

        }

        public void CreateFromModel(CreateTaskViewModel model)
        {
            FILE_URL = model.FILE_URL;
            FK_ID_USER = model.FK_ID_USER;
            FK_ID_FORMAT_BASE = model.FK_ID_FORMAT_BASE;
            FK_ID_FORMAT_TO_CONVERT = model.FK_ID_FORMAT_TO_CONVERT;
            LENGTH = model.LENGTH;

        }
        public void UpdateFromModel(CreateTaskViewModel model)
        {
            
            FILE_URL = model.FILE_URL;
            FK_ID_USER = model.FK_ID_USER;
            FK_ID_FORMAT_BASE = model.FK_ID_FORMAT_BASE;
            FK_ID_FORMAT_TO_CONVERT = model.FK_ID_FORMAT_TO_CONVERT;
            LENGTH = model.LENGTH;
        }
    }
}
