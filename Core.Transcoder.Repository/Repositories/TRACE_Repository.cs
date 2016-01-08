using Core.Transcoder.Repository;
using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class TRACE_Repository : GenericRepository<TRACE>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public TRACE_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TRACE> GetAll()
        {
            return Get(null, null, "").OrderBy(x => x.PK_ID_TRACE);
        }

       

    }
}
