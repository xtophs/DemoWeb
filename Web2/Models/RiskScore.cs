using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Web2.Models
{
    public class RiskScore
    {
        public RiskScore() { }

        public string Label { get; set; }
        public string Probability { get; set; }
    }

    public class RiskScoreEntity : TableEntity
    {
        public RiskScoreEntity() {

        }

        public RiskScoreEntity(string pk, string rk) : base (pk, rk){ PartitionKey = pk; RowKey = rk; }

        [EncryptProperty]
        public string FirstName { get; set; }
        [EncryptProperty]
        public string LastName { get; set; }

        public string Label { get; set; }
        public string Probability { get; set; }


    }
}
