using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Core;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Web2.Models;

namespace Web2.Controllers
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class CreditRiskController : Controller
    {
        // GET: CreditRisk
        public ActionResult Index()
        {
            return View("~/Views/CreditRisk/CreditApplicationView.cshtml");
        }

        [HttpPost]
        public ActionResult Index(CreditRiskApplication applicationData)
        {
            var Score = ScoreApplication(applicationData);

            Record(applicationData, Score);
            return View("~/Views/CreditRisk/CreditApplicationView.cshtml");
        }

        private RiskScore ScoreApplication(CreditRiskApplication application)
        {
            //var data = new CreditRiskApplication(
            // "A11", "12", "A32", "A43", "701", "A61", "A73", "4", "A94", "A101", "2", "A121", "40", "A143", "A152", "1", "172", "1", "A191", "A201", "1"
            // );

            return InvokeRequestResponseService(application);
        }

        static RiskScore InvokeRequestResponseService(CreditRiskApplication application)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Col1", "Col2", "Col3", "Col4", "Col5", "Col6", "Col7", "Col8", "Col9", "Col10", "Col11", "Col12", "Col13", "Col14", "Col15", "Col16", "Col17", "Col18", "Col19", "Col20", "Col21"},
                                Values = new string[,] {
                                    { application.CheckingAccountStatus, application.DurationInMonths, application.CreditHistory, application.Purpose, application.CreditAmount, application.SavingsAccount, application.PresentEmploymentSince, application.InstallmentPecentageOfDI, application.PersonalStatusAndSex, application.OtherDebtors, application.PresentResidenceSince, application.Property, application.AgeInYears, application.OtherInstallments, application.Housing, application.NumberOfExistingCredits, application.Job, application.NumberOfPeopleLiable, application.Telephone, application.ForeignWorker, application.CreditRisk }
                                    //{ application.Col1, "0", application.Col3, application.Col4, "0", application.Col6, application.Col7, "0", application.Col9, application.Col10, "0", application.Col12, "0", application.Col14, application.Col15, "0", application.Col17, "0", application.Col19, application.Col20, "0" },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "zetGy183tz8tazAW+RhsnTwL93qYDfEjOhZVwJHUcGV7mSOIW9ZpIN/4vcF+EumEZBekawFoOig781mPwtztKw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/c47311ad3b65403c9b588f84582d47d3/services/0a0ed061dba14caaac5e0d7982be2361/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = client.PostAsJsonAsync("", scoreRequest).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    //string result = await response.Content.ReadAsStringAsync();
                    string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var score = GetApplication(result);

                    Trace.WriteLine("Result: {0}", result);
                    var riskScore = new RiskScore();
                    riskScore.Probability = score.ScoredProbablities;
                    riskScore.Label = score.ScoredLabels;
                    return riskScore;
                }
                else
                {
                    Trace.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Trace.WriteLine(response.Headers.ToString());

                    //string responseContent = await response.Content.ReadAsStringAsync();
                    string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Trace.WriteLine(responseContent);
                    throw new Exception();
                }


            }
        }
        static CreditRiskApplication GetApplication(string result)
        {
            var score = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(result);

            var scoredApplication = new CreditRiskApplication();
            scoredApplication.CheckingAccountStatus = score.Results.output1.value.Values[0][0];
            scoredApplication.DurationInMonths = score.Results.output1.value.Values[0][1];
            scoredApplication.CreditHistory = score.Results.output1.value.Values[0][2];
            scoredApplication.Purpose = score.Results.output1.value.Values[0][3];
            scoredApplication.CreditAmount = score.Results.output1.value.Values[0][4];
            scoredApplication.SavingsAccount = score.Results.output1.value.Values[0][5];
            scoredApplication.PresentEmploymentSince = score.Results.output1.value.Values[0][6];
            scoredApplication.InstallmentPecentageOfDI = score.Results.output1.value.Values[0][7];
            scoredApplication.PersonalStatusAndSex = score.Results.output1.value.Values[0][8];
            scoredApplication.OtherDebtors = score.Results.output1.value.Values[0][9];
            scoredApplication.PresentResidenceSince = score.Results.output1.value.Values[0][10];
            scoredApplication.Property = score.Results.output1.value.Values[0][11];
            scoredApplication.AgeInYears = score.Results.output1.value.Values[0][12];
            scoredApplication.OtherInstallments = score.Results.output1.value.Values[0][13];
            scoredApplication.Housing = score.Results.output1.value.Values[0][14];
            scoredApplication.NumberOfExistingCredits = score.Results.output1.value.Values[0][15];
            scoredApplication.Job = score.Results.output1.value.Values[0][16];
            scoredApplication.NumberOfPeopleLiable = score.Results.output1.value.Values[0][17];
            scoredApplication.Telephone = score.Results.output1.value.Values[0][18];
            scoredApplication.ForeignWorker = score.Results.output1.value.Values[0][19];
            scoredApplication.CreditRisk = score.Results.output1.value.Values[0][20];
            scoredApplication.ScoredLabels = score.Results.output1.value.Values[0][21];
            scoredApplication.ScoredProbablities = score.Results.output1.value.Values[0][22];


            return scoredApplication;

        }

        static void Record(CreditRiskApplication data, RiskScore score)
        {
            var entity = new RiskScoreEntity()
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Probability = score.Probability,
                Label = score.Label,
                PartitionKey = "1",
                RowKey = DateTime.Now.Ticks.ToString()
            };

            if (KeyVaultAccessor.creds != null)
            {
                KeyVaultKeyResolver cloudResolver = new KeyVaultKeyResolver(KeyVaultAccessor.GetAccessTokenWithAuthority);

                // await causes the function to hang.
                // GetAwaiter().GetResult() works
                //Secret cloudSecret = cloudVault.SetSecretAsync(vaultUri, "secret", symmetricBytes, null, "application/octet-stream").GetAwaiter().GetResult();
                // cloudResolver: await hangs.
                // cloudResolver: GetAwaiter().GetResult() hangs.
                //var secretId = "https://ckeyvault.vault.azure.net:443/secrets/secret";
                cloudResolver.ResolveKeyAsync(KeyVaultAccessor.creds.SecretUri, CancellationToken.None).ContinueWith(
                    k =>
                    {
                        Trace.WriteLine("in lambda");
                        var waiter = k.GetAwaiter();
                        while (waiter.IsCompleted == false)
                        {
                            Thread.Sleep(1000);
                            Trace.WriteLine("one more");
                        }
                        IKey cloudKey = waiter.GetResult();
                        Trace.WriteLine("got key");

                        // Insert Entity
                        GetTable().Execute(
                            TableOperation.Insert(entity),
                            new TableRequestOptions()
                            {
                                EncryptionPolicy = new TableEncryptionPolicy(cloudKey, null)
                            },
                            null);

                    });
            }
            else
            {

                // Insert Entity
                GetTable().Execute(
                    TableOperation.Insert(entity),
                    null,
                    null);
            }
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Trace.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
            return storageAccount;
        }

        public  static CloudTable GetTable()
        {
            string demoTable = "ScoreResults";
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(demoTable);

            table.CreateIfNotExists();
            return table;
        }
    }

}