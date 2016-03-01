using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class TRANSACTION_Repository : GenericRepository<TRANSACTION>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public TRANSACTION_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
