using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using BankAccount.SpecFlowNUnit.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace BankAccount.SpecFlowNUnit.StepDefinitions
{
    [Binding]
    public class SpecFlowFeatureSteps
    {
        private string _baseUrl;
        HttpClient client;
        HttpRequestMessage request;

        public static AventStack.ExtentReports.ExtentReports _extent;
        static ExtentTest _test;
        static string ReportTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        //static string ReportName = "Bank account Api test";

        [BeforeTestRun]
        protected static void OneTimeSetup()
        {
            string pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "reports/" + ReportTime + "/";
            string dir = Regex.Replace(finalpth, @"file:///", "");
            (new FileInfo(dir)).Directory.Create();
            var fileName = "bankAccountReports " + ReportTime + ".html";
            var htmlReporter = new ExtentHtmlReporter(dir + fileName);

            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AttachReporter(htmlReporter);
        }

        [AfterTestRun]
        protected static void OneTimeTearDown()
        {
            _extent.Flush();
            string pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "reports/" + "userManagement/";
            var fileName = "userManagementReports " + ReportTime + ".html";
        }

        [BeforeScenario]
        protected static void Setup()
        {
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        [AfterScenario]
        protected static void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                        ? ""
                        : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
                Status logstatus;

                switch (status)
                {
                    case TestStatus.Failed:
                        logstatus = Status.Fail;
                        _test.Fail(TestContext.CurrentContext.Result.Message);
                        break;
                    case TestStatus.Inconclusive:
                        logstatus = Status.Warning;
                        break;
                    case TestStatus.Skipped:
                        logstatus = Status.Skip;
                        break;
                    default:
                        logstatus = Status.Pass;
                        break;
                }
                _test.Log(logstatus, "Test ended with " + logstatus + stacktrace);
                _extent.Flush();


            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }


        [Given(@"a request with url of '(.*)'")]
        [Given(@"a request with an invalid url of '(.*)'")]
        public void GivenARequestWithUrlOf(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        


        [When(@"valid request is posted to api with token '(.*)' and bankAccount '(.*)' and content type of '(.*)' and url of '(.*)'")]
        [When(@"invalid request is posted to api with invalid token '(.*)' and bankAccount '(.*)' and content type of '(.*)' and url of '(.*)'")]
        [When(@"invalid request is posted to api with empty token '(.*)' bankAccount '(.*)' and content type of '(.*)' and url of '(.*)'")]
        [When(@"invalid request is posted to api with token '(.*)' and invalid bankAccount (.*) and content type of '(.*)' and url of '(.*)'")]
        [When(@"invalid request is posted to api with token '(.*)' and empty bankAccount '(.*)' content type of '(.*)' and url of '(.*)'")]
        [When(@"invalid request with invalid url is posted to api with token '(.*)' and '(.*)' and content type of '(.*)' and url of '(.*)'")]
        public void WhenValidRequestIsPostedToApiWithTokenAndAndContentTypeOfAndUrlOf(string token, string bankAccount, string contentType, string url)
        {
            _test.Log(Status.Info, "Token: " + token + "<br>" + "Bank account: " + bankAccount + "<br>" + "content type: " + contentType + "<br>" + "Url: " + _baseUrl + url);
            try
            {
                client = new HttpClient();
                request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + url);
                if(token != "empty")
                {
                    request.Headers.Add("X-Auth-Key", token);
                }

                if (bankAccount == "empty")
                {
                    bankAccount = null;
                }
                var content = new JObject
                {
                    { "bankAccount", bankAccount }
                };

                request.Content = new StringContent(
                    content.ToString(),
                    Encoding.UTF8,
                    contentType
                );


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        
        
        [Then(@"Api returns ok")]
        public async System.Threading.Tasks.Task ThenApiReturnsOkAsync()
        {
            BankApiTestSuccessResponse result = new BankApiTestSuccessResponse();
            var response = await client.SendAsync(request);
            string responseConvertToText = await response.Content.ReadAsStringAsync();
            _test.Log(Status.Info, "Json response: " + responseConvertToText);
            result = JsonConvert.DeserializeObject<BankApiTestSuccessResponse>(responseConvertToText);
            Assert.AreEqual(true, result.IsValid);
        }
        
        [Then(@"Api returns error '(.*)'")]
        public async System.Threading.Tasks.Task ThenApiReturnsAsync(string message)
        {
            _test.Log(Status.Info, "Corresponding error message: " + message);
            BankApiTestInvalidResponseMessage result = new BankApiTestInvalidResponseMessage();
            var response = await client.SendAsync(request);
            string responseConvertToText = await response.Content.ReadAsStringAsync();
            _test.Log(Status.Info, "Json response: " + responseConvertToText);
            result = JsonConvert.DeserializeObject<BankApiTestInvalidResponseMessage>(responseConvertToText);
            Assert.AreEqual(message, result.Message);
        }

        [Then(@"Api returns bank account validation errors withe corresponding bankAccount (.*) causes error message (.*)")]
        public async System.Threading.Tasks.Task ThenApiReturnsBankAccountValidationErrorsWitheCorrespondingBankAccountAsync(string bankAccount, string errorMessage)
        {
            _test.Log(Status.Info, "Corresponding error message: " + errorMessage);
            var response = await client.SendAsync(request);
            string responseConvertToText = await response.Content.ReadAsStringAsync();
            _test.Log(Status.Info, "Json response: " + responseConvertToText);
            if (bankAccount.Length < 7 || bankAccount.Length > 34)
            {
                List<BankApiTestInvalidBankAccountMessage> result = new List<BankApiTestInvalidBankAccountMessage>();
                result = JsonConvert.DeserializeObject<List<BankApiTestInvalidBankAccountMessage>>(responseConvertToText);
                foreach (var element in result)
                {
                    Assert.AreEqual(errorMessage, element.Message);
                }
            }
            else
            {
                BankApiTestInvalidResponseMessage result = new BankApiTestInvalidResponseMessage();
                result = JsonConvert.DeserializeObject<BankApiTestInvalidResponseMessage>(responseConvertToText);
                Assert.AreEqual(false, result.IsValid);
            }
        }

    }
}
