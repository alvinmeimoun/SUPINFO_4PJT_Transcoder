using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Utils
{
    public static class PriceGeneratorUtil
    {
        public static double GetPriceByDuration(double duration)
        {
            double price = duration / 60;
            price =  Math.Round(price,2);
            return price;
        }

    }
}
