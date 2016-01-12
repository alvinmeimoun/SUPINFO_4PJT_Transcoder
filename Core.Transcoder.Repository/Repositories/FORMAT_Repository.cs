using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class FORMAT_Repository : GenericRepository<FORMAT>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public FORMAT_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



    }
}
