using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Services
{
    public class PARAM_TASK_STATUS_Service : BaseService
    {
        public List<PARAM_TASK_STATUS> GetAll()
        {
            return UoW.PARAM_TASK_STATUS_Repository.Get(null, q => q.OrderBy(s => s.PK_ID_STATUS), "").ToList();
        }

    }
}
