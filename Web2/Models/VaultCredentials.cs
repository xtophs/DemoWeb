using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Web2.Models
{
    public class VaultCredentials : TableEntity
    {
        public VaultCredentials() { }

        public VaultCredentials(
            string url, string clientID, string clientKeySecret )
        {
            SecretUri = url;
            KVClientID = clientID;
            KVClientKeySecret = clientKeySecret;

        }

        public VaultCredentials(string pk, string rk) : base(pk, rk) { PartitionKey = pk; RowKey = rk; }

        public string SecretUri { get; set; }

        public string KVClientID { get; set; }

        public string KVClientKeySecret { get; set; }

    }
}
