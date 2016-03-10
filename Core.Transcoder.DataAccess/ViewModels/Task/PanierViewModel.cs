using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class PanierViewModel
    {
        public List<ListTaskViewModel> ListOfConversions { get; set; }

        public double GlobalPrice { get; set; }
        public string TransactionId { get; set; }

        public PanierViewModel()
        {

        }
    }
}
