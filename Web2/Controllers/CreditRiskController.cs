using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
        public JsonResult Index(CreditRiskApplication application)
        {
            //ScoreApplication(application);
            //return View();

            return Json(new 
            {
                Success = true,
                Object = new { message = "Hi, Json"  }
            });
        }

        private void ScoreApplication(CreditRiskApplication application)
        {
            //var data = new CreditRiskApplication(
   // "A11", "12", "A32", "A43", "701", "A61", "A73", "4", "A94", "A101", "2", "A121", "40", "A143", "A152", "1", "172", "1", "A191", "A201", "1"
   // );
            
            InvokeRequestResponseService(application).Wait();
        }

        static async Task InvokeRequestResponseService(CreditRiskApplication application)
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
                const string apiKey = "l8tzeyjJoGJab1Gpl1msh8dZfn7kmmYsEEaJhSi6/tS8wvSe6sMSmNj4RCcFmOD6cGY6BE0ZMhESolGWrOPlXw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/c47311ad3b65403c9b588f84582d47d3/services/598f417b641d426cb2f84a2773017d39/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine("Result: {0}", result);
                }
                else
                {
                    Trace.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Trace.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine(responseContent);
                }
            }
        }
    }
}