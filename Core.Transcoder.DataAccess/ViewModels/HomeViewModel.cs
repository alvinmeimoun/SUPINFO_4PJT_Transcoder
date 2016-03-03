using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class HomeViewModel
    {
        public bool IsLogged { get; set; } 

        public int UsersCount { get; set; }
        public int TaskCount { get; set; }
        public int CombinationCount { get; set; }
    }
}
