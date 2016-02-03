using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public class CONFIG_Service
    {
        public UnitOfWork UoW = new UnitOfWork();
        public string GetConfigValueById(int id)
        {
            return UoW.CONFIG_Repository.GetByID(id).CONFIG_VALUE;
        }

        public string GetConfigValueByName(string configName)
        {
            return UoW.CONFIG_Repository.Get(x => x.CONFIG_NAME == configName).FirstOrDefault().CONFIG_VALUE;
        }
    }
}
