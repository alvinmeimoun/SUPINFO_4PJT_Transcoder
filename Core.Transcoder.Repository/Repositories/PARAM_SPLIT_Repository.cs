using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class PARAM_SPLIT_Repository : GenericRepository<PARAM_SPLIT>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public PARAM_SPLIT_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
