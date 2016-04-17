using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class HomeAuthViewModel
    {
        public string LastTranscode { get; set; }

        public DateTime DateDemande { get; set; }

        public string Status { get; set; }

        public HomeAuthViewModel()
        {

        }
    }
}
