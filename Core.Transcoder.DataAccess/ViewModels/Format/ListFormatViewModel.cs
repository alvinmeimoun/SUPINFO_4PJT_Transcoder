using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class ListFormatViewModel
    {
        public int PK_ID_FORMAT { get; set; }
        public string FORMAT_NAME { get; set; }

        public int FK_ID_FORMAT_TYPE { get; set; }


        public ListFormatViewModel()
        {

        }
    }
}