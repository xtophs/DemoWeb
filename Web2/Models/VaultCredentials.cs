using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Web2.Models
{
    public class VaultCredentials : TableEntity
    {
        public VaultCredentials() {
            SecretUri = "";
            KVClientID = "";
            KVClientKeySecret = "";
        }

        public VaultCredentials(
            string url, string clientID, string clientKeySecret)
        {
            SecretUri = url;
            KVClientID = clientID;
            KVClientKeySecret = clientKeySecret;

        }

        public VaultCredentials(string pk, string rk) : base(pk, rk) { PartitionKey = pk; RowKey = rk; }


        [DisplayName("URL to your Secret")]
        public string SecretUri { get; set; }

        [DisplayName("Key Vault ClientID")]
        public string KVClientID { get; set; }

        [DisplayName("Key Vault Access Secret")]
        public string KVClientKeySecret { get; set; }

    }
}
