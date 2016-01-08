using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service
{
    public static class FileHelper
    {

        public static string ConvertToEscapeString(string FileLink)
        {
            FileLink.Replace(@"\",@"\\");

            return FileLink;
        }
    }
}
