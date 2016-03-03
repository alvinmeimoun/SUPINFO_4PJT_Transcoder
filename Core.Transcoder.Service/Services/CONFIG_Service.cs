using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Service.Services;

namespace Core.Transcoder.Service
{
    public class CONFIG_Service : BaseService
    {
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
