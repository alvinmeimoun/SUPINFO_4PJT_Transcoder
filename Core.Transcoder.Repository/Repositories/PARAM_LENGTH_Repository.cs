using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository.Repositories
{
    public class PARAM_LENGTH_Repository : GenericRepository<PARAM_LENGTH>
    {

        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public PARAM_LENGTH_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
