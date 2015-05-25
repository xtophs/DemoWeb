// 
// Copyright © Microsoft Corporation, All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
// ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
// PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache License, Version 2.0 for the specific language
// governing permissions and limitations under the License.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Globalization;
using Microsoft.Azure.KeyVault.WebKey;
using Web2.Models;

namespace Web2
{
    /// <summary>    
    /// This class uses Microsoft.KeyVault.Client library to call into Key Vault and retrieve a secret.
    /// 
    /// Authentication when calling Key Vault is done through the configured X509 ceritifcate.
    /// </summary>
    public class KeyVaultAccessor
    {
        private static KeyVaultClient keyVaultClient;
        private static X509Certificate2 clientAssertionCertPfx;

        public static VaultCredentials creds = null;
        static KeyVaultAccessor()
        {
            creds = null;

            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            //clientAssertionCertPfx = CertificateHelper.FindCertificateByThumbprint(CloudConfigurationManager.GetSetting(Constants.KeyVaultAuthCertThumbprintSetting));
            clientAssertionCertPfx = CertificateHelper.FindCertificateByThumbprint(ConfigurationManager.AppSettings[Constants.KeyVaultAuthCertThumbprintSetting]);
        }

        public static void AddKey(X509Certificate2 cert, string keyName)
        {
            ImportKeyWithCertAsync(ConfigurationManager.AppSettings["KeyVaultAddress"], keyName, cert, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a secret from Key Vault
        /// </summary>
        /// <param name="secretId">ID of the secret</param>
        /// <returns>secret value</returns>
        public static string GetSecret(string secretId)
        {
            var secret = keyVaultClient.GetSecretAsync(secretId).Result;
            return secret.Value;
        }

        /// <summary>
        /// Authentication callback that gets a token using the X509 certificate
        /// </summary>
        /// <param name="authority">Address of the authority</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token</param>
        /// <param name="scope">Scope</param>
        /// <returns></returns>
        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var client_id = ConfigurationManager.AppSettings[Constants.KeyVaultAuthClientIdSetting];
            var context = new AuthenticationContext(authority, null);

            var assertionCert = new ClientAssertionCertificate(client_id, clientAssertionCertPfx);
            var result = await context.AcquireTokenAsync(resource, assertionCert);

            return result.AccessToken;
        }

        public static async Task<string> GetAccessTokenWithAuthority(string authority, string resource, string scope)
        {
            //ClientCredential credential = new ClientCredential(CloudConfigurationManager.GetSetting("KVClientId"),
            //    "+HEBib6PyYf+UHAje4tfVea0aJrVqBcZdTXuhdCbFfI=");
                //CloudConfigurationManager.GetSetting("KVClientKey"));

            ClientCredential credential = new ClientCredential(creds.KVClientID, creds.KVClientKeySecret);

            AuthenticationContext ctx = new AuthenticationContext(new Uri(authority).AbsoluteUri, false);
            AuthenticationResult result = await ctx.AcquireTokenAsync(resource, credential);

            return result.AccessToken;
        }



        public static async Task<KeyBundle> ImportKeyWithCertAsync(string vaultAddress, string keyName, X509Certificate2 certificate, bool? importToHardware = null)
        {
            if (string.IsNullOrEmpty(vaultAddress))
                throw new ArgumentNullException("vaultAddress");

            if (certificate == null)
                throw new ArgumentNullException("certificate");

            if (!certificate.HasPrivateKey)
                throw new ArgumentException("Certificate does not have a private key");

            var key = certificate.PrivateKey as RSA;

            if (key == null)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Certificate key uses unsupported algorithm {0}", certificate.GetKeyAlgorithm()));

            var rsaParameters = key.ExportParameters(false);
            var jsonKey = new JsonWebKey();

            jsonKey.Kty = JsonWebKeyType.Rsa;

            jsonKey.E = rsaParameters.Exponent;
            jsonKey.N = rsaParameters.Modulus;

            jsonKey.D = rsaParameters.D;
            jsonKey.DP = rsaParameters.DP;
            jsonKey.DQ = rsaParameters.DQ;
            jsonKey.QI = rsaParameters.InverseQ;
            jsonKey.P = rsaParameters.P;
            jsonKey.Q = rsaParameters.Q;



            var keyBundle = new KeyBundle()
            {
                Key = jsonKey,
                Attributes = new KeyAttributes
                {
                    Enabled = true,
                    Expires = certificate.NotAfter.ToUniversalTime(),
                    NotBefore = certificate.NotBefore.ToUniversalTime(),
                },
            };

            return await keyVaultClient.ImportKeyAsync(vaultAddress, keyName, keyBundle, importToHardware).ConfigureAwait(false);
        }
    }
}