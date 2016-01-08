using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class TASK_Repository : GenericRepository<TASK>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public TASK_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



    }
}
