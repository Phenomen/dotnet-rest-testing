using NUnit.Framework;
using RestSharp;
using Rokolabs.AutomationTestingTask.Entities;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Rokolabs.AutomationTestingTask.Entities.Enums;


//Bugs found:
//1. GrouppedView with GroupBy for Title, Company and MeetingTypes return Internal Server Error


namespace Accounts
{
    [TestFixture]
    public class GrouppedViewTest
    {
        private RestClient _grouppedClient;
        private ServiceReference1.AccountsClient _accountsClient;

        [OneTimeSetUp]
        public void Initialize()
        {
            _grouppedClient = new RestClient("http://rest-att.dio.red/v3/GrouppedView");
        }


        public string NewSession()
        {
            _accountsClient = new ServiceReference1.AccountsClient();
            string randomLogin = "NewUser." + RandomGenerator.GetRandomString();
            _accountsClient.Registrate(randomLogin, "RealPassword.©");
            string sessionId = _accountsClient.Login(randomLogin, "RealPassword.©");
            //string sessionId = "23d81af9-350f-406c-a185-5ccca470bf84";
            return sessionId;
        }

        [OneTimeTearDown]
        public void CloseConnection()
        {
            _accountsClient.Close();
        }

        [Test]
        public void GetGrouppedViewWithIncorrectId()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", "0");

            var response = _grouppedClient.Get<Summary>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response: " + response.Content);
            StringAssert.Contains("Access is denied", response.Content);
        }


        [Test]
        public void GrouppedViewWithoutGroupByReturnError()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());

            var response = _grouppedClient.Get<Summary>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response: " + response.Content);
        }

        [TestCase("Title")]
        [TestCase("Broker")]
        [TestCase("InteractionType")]
        [TestCase("MeetingTypes")]
        [TestCase("Location")]
        [TestCase("Company")]
        [TestCase("AddressType")]
        public void GrouppedViewGroupByFilter(string filter)
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddQueryParameter("GroupBy", filter);

            var response = _grouppedClient.Get<Summary>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);
        }
    }
}