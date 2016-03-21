using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class ListCommandesViewModel
    {
        public int UserId { get; set; }
        public List<ListTaskViewModel> ListOfConversions { get; set; }

        public ListCommandesViewModel()
        {

        }
    }
}
