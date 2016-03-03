using Core.Transcoder.Repository;
using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Service.Services;

namespace Core.Transcoder.Service
{
    public class TRACE_Service : BaseService
    {

        public bool AddTrace(TRACE trace)
        {
            if(UoW.TRACE_Repository.Insert(trace))
            {
                UoW.Save();
                return true;
            }
            else
            {
                return false;
            }
             
        }
        public IEnumerable<TRACE> GetAllTraces()
        {
            return UoW.TRACE_Repository.GetAll();
        }


    }
}
