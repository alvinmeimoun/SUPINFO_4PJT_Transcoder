using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Repository;

namespace Core.Transcoder.Service.Services
{
    public abstract class BaseService
    {
        private UnitOfWork uow;
        public UnitOfWork UoW
        {
            get
            {
                if (uow == null)
                {
                    uow = new UnitOfWork();
                }
                return uow;
            }

        }
    }
}
