using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateCase.Models
{
    public class ResponseMinMaxAvg
    {
        public ResponseMinMaxAvg(string min, string max, string avg)
        {
            this.min = min;
            this.max = max;
            this.avg = avg;
        }
        public string avg { get; set; }

        public string max { get; set; }

        public string min { get; set; }
    }
}
