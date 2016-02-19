using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class PARAM_LENGTH_Service
    {
        public UnitOfWork UoW = new UnitOfWork();

        public List<PARAM_LENGTH> GetAll()
        {
            return UoW.PARAM_LENGTH_Repository.Get(null, x => x.OrderByDescending(q => q.PK_ID_PARAM_LENGTH), "").ToList();
        }
        
    }
}
