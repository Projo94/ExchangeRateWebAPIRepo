using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateCase.Models
{
    public class ExchangeRate
    {
        public Dictionary<string, Dictionary<string, double>> rates { get; set; }
    }
}
