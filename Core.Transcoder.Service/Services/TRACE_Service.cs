using Core.Transcoder.Repository;
using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class TRACE_Service
    {
        public UnitOfWork UoW = new UnitOfWork();

        public bool AddTrace(TRACE trace)
        {
            if(UoW.TraceRepository.Insert(trace))
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
            return UoW.TraceRepository.GetAll();
        }


    }
}
