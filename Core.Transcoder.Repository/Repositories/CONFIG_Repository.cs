using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class CONFIG_Repository : GenericRepository<CONFIG>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public CONFIG_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
