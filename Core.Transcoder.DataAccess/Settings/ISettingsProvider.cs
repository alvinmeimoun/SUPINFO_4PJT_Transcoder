using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.Settings
{
    public interface ISettingsProvider
    {
        T Setting<T>(string name);
    }
}
