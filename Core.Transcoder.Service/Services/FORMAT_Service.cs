using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class FORMAT_Service
    {
        public UnitOfWork UoW = new UnitOfWork();

        public FORMAT GetFormatById(int Id)
        {
           return UoW.FORMAT_Repository.GetByID(Id);
            
        }
    }
}
