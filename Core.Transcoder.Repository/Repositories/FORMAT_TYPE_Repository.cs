using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class FORMAT_TYPE_Repository : GenericRepository<FORMAT_TYPE>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public FORMAT_TYPE_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
