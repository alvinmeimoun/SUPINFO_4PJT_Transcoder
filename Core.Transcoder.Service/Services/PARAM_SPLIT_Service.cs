using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class PARAM_SPLIT_Service
    {
        public UnitOfWork UoW = new UnitOfWork();

        public bool AddOrUpdateParamSplit(PARAM_SPLIT paramSplit)
        {
            if (paramSplit.PK_ID_PARAM_SPLIT != 0)
            {
                UoW.PARAM_SPLIT_Repository.Update(paramSplit);
                UoW.Save();
                return true;
            }
            else
            {
                UoW.PARAM_SPLIT_Repository.Insert(paramSplit);
                UoW.Save();
                return true;
            }
        }
    }
}
