using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Service.Services;

namespace Core.Transcoder.Service
{
    public class PARAM_LENGTH_Service : BaseService
    {
        public List<PARAM_LENGTH> GetAll()
        {
            return UoW.PARAM_LENGTH_Repository.Get(null, x => x.OrderByDescending(q => q.PK_ID_PARAM_LENGTH), "").ToList();
        }
        
    }
}
