using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2.Models
{
    public class CRAViewModel
    {
        public string Message { get; set; }
        public CreditRiskApplication applicationData { get; set; }
        public RiskScore Score { get; set; }
    }
}
