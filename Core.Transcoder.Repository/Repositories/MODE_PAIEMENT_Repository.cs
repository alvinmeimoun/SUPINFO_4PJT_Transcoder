using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class MODE_PAIEMENT_Repository : GenericRepository<MODE_PAIEMENT>
    {
        public TRANSCODEREntities _dbContext = new TRANSCODEREntities();

        public MODE_PAIEMENT_Repository(TRANSCODEREntities dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
