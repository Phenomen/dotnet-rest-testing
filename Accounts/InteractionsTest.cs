using NUnit.Framework;
using RestSharp;
using Rokolabs.AutomationTestingTask.Entities;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Rokolabs.AutomationTestingTask.Entities.Enums;


namespace Accounts
{
    [TestFixture]
    public class InteractionsTest
    {
        private RestClient _interactionsClient;
        private ServiceReference1.AccountsClient _accountsClient;

        [OneTimeSetUp]
        public void Initialize()
        {
            _interactionsClient = new RestClient("http://rest-att.dio.red/v3/Interaction");
        }


        public string NewSession()
        {
            _accountsClient = new ServiceReference1.AccountsClient();
            string randomLogin = "NewUser." + RandomGenerator.GetRandomString();
            _accountsClient.Registrate(randomLogin, "RealPassword.©");
            string sessionId = _accountsClient.Login(randomLogin, "RealPassword.©");
            //string sessionId = "00bad0c8-ab85-470c-a17a-c63a9ebed6ec";
            return sessionId;
        }

        [OneTimeTearDown]
        public void CloseConnection()
        {
            _accountsClient.Close();
        }

        //Incorrect session should return Access is denied
        [TestCase("")]
        [TestCase("1")]
        public void IncorrectSessionInInteractions(string incorrectSessionId)
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", incorrectSessionId);
            var response = _interactionsClient.Get<EventGroup>(request);
            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response: " + response.Content);
            StringAssert.Contains("Access is denied", response.Content);
        }

        //Non existing event should return Not Found
        [TestCase("0")]
        public void GetNonExistingEventInInteractions(string eventId)
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddQueryParameter("id", eventId);
            var response = _interactionsClient.Get<Event>(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response: " + response.Content);
        }

        //Get without id should return event list
        [Test]
        public void GetWithoutIdReturnEventListInInteractions()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            var response = _interactionsClient.Get<EventList>(request);

            Assert.Greater(response.Data.Items.Count, 0, "Event list should contain some items");
        }

        //Valid post request create new event
        [Test]
        public void CreateValidEventInInteractions()
        {
            var newEvent = new Event
            {
                Title = "titletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitle",
                Duration = 0,
                Broker = "TestBroker",
                Location = new Location() { City = "Earth", Country = "Solar City" },
                Date = new DateTime(2010, 1, 1),
                InteractionType = InteractionTypes.Sales
            };

            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddJsonBody(newEvent);

            var response = _interactionsClient.Post<Event>(request);
            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);

            //Make sure event is created correctly
            //response.Content returns id of event that was just created. 
            string newId = response.Content;
            request.AddQueryParameter("id", newId);
            response = _interactionsClient.Get<Event>(request);
            Assert.AreEqual("titletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitletitle", response.Data.Title);
            Assert.AreEqual(0, response.Data.Duration);
            Assert.AreEqual("TestBroker", response.Data.Broker);
            Assert.AreEqual(InteractionTypes.Sales, response.Data.InteractionType);
        }

        //Create new event (invalid data)
        //Failure: Title is requiring 24 symbols instead of 12.
        [TestCase("titletitletitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title cannot have less than 12 symbols")]
        [TestCase("TitleTitleTitleTitleTitleTitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title can contain only lower letters")]
        //Failure: Incorrect error message
        [TestCase("titletitletitle titletitletitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title can contain only lower letters")]
        [TestCase("", 1000, "TestBroker", "Somewhere", "Nowhere", "Title cannot be empty")]
        [TestCase("titletitletitletitletitletitle", -1, "TestBroker", "Somewhere", "Nowhere",
            "Duration cannot be negative")]
        //Failure: Broker accept string with spaces
        [TestCase("titletitletitletitletitletitle", 1000, "Test Broker", "Somewhere", "Nowhere",
            "Broker cannot contain spaces")]
        [TestCase("titletitletitletitletitletitle", 1000, "", "Somewhere", "Nowhere", "Broker cannot be empty")]
        [TestCase("titletitletitletitletitletitle", 1000,
            "TestBrokerTestBrokerTestBrokerTestBrokerTestBrokerTestBroker", "Somewhere", "Nowhere",
            "Broker cannot have more than 50 symbols")]
        [TestCase("titletitletitletitletitletitle", 1000, "TestBroker", "", "", "Location cannot be empty")]
        public void CreateInvalidEventInInteractions(string eventTitle, int eventDuration, string eventBroker,
            string eventCity, string eventCountry, string responseError)
        {
            var updatedEvent = new Event
            {
                Title = eventTitle,
                Duration = eventDuration,
                Broker = eventBroker,
                Location = new Location() { City = eventCity, Country = eventCountry },
                Date = new DateTime(2010, 1, 1)
            };
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddJsonBody(updatedEvent);
            var response = _interactionsClient.Post<Event>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, responseError);
            StringAssert.Contains(responseError, response.Content);
        }

        //INTERACTIONS SPECIFIC: Date should be in the past when Creating event
        [TestCase("titletitletitletitletitletitle", 1000, "TestBroker", "Somewhere", "Nowhere")]
        public void CreateEventDateToPastInInteractions(string eventTitle, int eventDuration, string eventBroker,
            string eventCity, string eventCountry)
        {
            var updatedEvent = new Event
            {
                Title = eventTitle,
                Duration = eventDuration,
                Broker = eventBroker,
                Location = new Location() { City = eventCity, Country = eventCountry },
                Date = new DateTime(2500, 1, 1)
            };
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddJsonBody(updatedEvent);
            var response = _interactionsClient.Post<Event>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response: " + response.Content);
            StringAssert.Contains("Interaction cannot have date in the future. Please, create an Event", response.Content);
        }

        //Update existing events (invalid data)
        //Failure: Title is requiring 24 symbols instead of 12.
        [TestCase("titletitletitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title cannot have less than 12 symbols")]
        [TestCase("TitleTitleTitleTitleTitleTitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title can contain only lower letters")]
        //Failure: Incorrect error message
        [TestCase("titletitletitle titletitletitle", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title cannot contain spaces")]
        [TestCase("", 1000, "TestBroker", "Somewhere", "Nowhere",
            "Title cannot be empty")]
        [TestCase("titletitletitletitletitletitle", -1, "TestBroker", "Somewhere", "Nowhere",
            "Duration cannot be negative")]
        //Failure: Broker accept string with spaces
        [TestCase("titletitletitletitletitletitle", 1000, "Test Broker", "Somewhere", "Nowhere",
            "Broker cannot contain spaces")]
        [TestCase("titletitletitletitletitletitle", 1000, "", "Somewhere", "Nowhere",
            "Broker cannot be empty")]
        [TestCase("titletitletitletitletitletitle", 1000,
            "TestBrokerTestBrokerTestBrokerTestBrokerTestBrokerTestBroker", "Somewhere", "Nowhere",
            "Broker cannot have more than 50 symbols")]
        [TestCase("titletitletitletitletitletitle", 1000, "TestBroker", "", "",
            "Location cannot be empty")]
        public void UpdateInvalidEventInInteractions(string eventTitle, int eventDuration, string eventBroker,
            string eventCity, string eventCountry, string responseError)
        {
            var updatedEvent = new Event
            {
                Title = eventTitle,
                Duration = eventDuration,
                Broker = eventBroker,
                Location = new Location() { City = eventCity, Country = eventCountry },
                Date = new DateTime(2010, 1, 1)
            };
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            var responseList = _interactionsClient.Get<EventList>(request);
            Event lastEvent = responseList.Data.Items[responseList.Data.Items.Count - 1];
            string lastId = lastEvent.EventId.ToString();
            request.AddQueryParameter("id", lastId);
            request.AddJsonBody(updatedEvent);
            var response = _interactionsClient.Put<Event>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response: " + response.Content);
            StringAssert.Contains(responseError, response.Content);
        }

        //Those tests should pass since location contain at least one field
        [TestCase("titletitletitletitletitletitle", 1000, "TestBroker", "", "Nowhere")]
        //Failure: InternalServerError for empty country
        [TestCase("titletitletitletitletitletitle", 1000, "TestBroker", "Somewhere", "")]
        public void UpdateEventLocationInInteractions(string eventTitle, int eventDuration,
            string eventBroker, string eventCity, string eventCountry)
        {
            var updatedEvent = new Event
            {
                Title = eventTitle,
                Duration = eventDuration,
                Broker = eventBroker,
                Location = new Location() { City = eventCity, Country = eventCountry },
                Date = new DateTime(2010, 1, 1)
            };
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            //Get last id in the event list
            var responseList = _interactionsClient.Get<EventList>(request);
            Event lastEvent = responseList.Data.Items[responseList.Data.Items.Count - 1];
            string lastId = lastEvent.EventId.ToString();
            request.AddQueryParameter("id", lastId);
            request.AddJsonBody(updatedEvent);
            var response = _interactionsClient.Put<Event>(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);
        }

        //Update existing event and validate new data
        [Test]
        public void UpdateEventWithValidDataInInteractions()
        {
            //Create new event 
            var newEvent = new Event
            {
                Title = "alphabetagammadeltaepsilonzeta",
                Duration = 1111,
                Broker = "Rincewind",
                Location = new Location() { City = "Ankh-Morpork", Country = "Discworld" },
                Date = new DateTime(2010, 1, 1)
            };
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddJsonBody(newEvent);
            _interactionsClient.Post<Event>(request);

            //Update it
            var updatedEvent = new Event
            {
                Title = "loremipsumdolorsitametconsectetur",
                Duration = 2222,
                Broker = "NewBroker",
                Location = new Location() { City = "Earth", Country = "Solar System" },
                Date = new DateTime(2010, 1, 1)
            };
            RestRequest request2 = new RestRequest();
            request2.SetJsonContentType();
            request2.AddQueryParameter("sessionId", NewSession());
            //Get last id in the event list
            var responseList = _interactionsClient.Get<EventList>(request2);
            Event lastEvent = responseList.Data.Items[responseList.Data.Items.Count - 1];
            string lastId = lastEvent.EventId.ToString();
            request2.AddQueryParameter("id", lastId);
            request2.AddJsonBody(updatedEvent);
            var response = _interactionsClient.Put<Event>(request2);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);

            response = _interactionsClient.Get<Event>(request2);
            Assert.AreEqual("loremipsumdolorsitametconsectetur", response.Data.Title);
            Assert.AreEqual(2222, response.Data.Duration);
            Assert.AreEqual("NewBroker", response.Data.Broker);
        }

        //Get and verify last event in the list
        [Test]
        public void GetExistingEventInInteractions()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            //Get last id in the event list
            var responseList = _interactionsClient.Get<EventList>(request);
            Event lastEvent = responseList.Data.Items[responseList.Data.Items.Count - 1];
            string lastId = lastEvent.EventId.ToString();
            request.AddQueryParameter("id", lastId);

            var response = _interactionsClient.Get<Event>(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);
            Assert.Greater(response.Data.Date, DateTime.Now, "Event should have date in the future");
            Assert.GreaterOrEqual(response.Data.Title.Length, 12, "Title cannot have less than 12 symbols");
            Assert.IsFalse(response.Data.Title.Any(char.IsUpper), "Title can contain only lower letters");
            Assert.IsFalse(response.Data.Title.Any(char.IsSeparator), "Title can contain only lower letters");
            Assert.IsNotEmpty(response.Data.Title, "Title cannot be empty");
            Assert.GreaterOrEqual(response.Data.Duration, 0, "Duration cannot be negative");
            Assert.IsFalse(response.Data.Broker.Any(char.IsSeparator), "Broker cannot contain spaces");
            Assert.IsNotEmpty(response.Data.Broker, "Broker cannot be empty");
            Assert.LessOrEqual(response.Data.Broker.Length, 50, "Broker cannot have more than 50 symbols");
            Assert.IsNotEmpty(response.Data.Location.ToString(), "Location cannot be empty");
        }

        //Delete last event in the list
        [Test]
        public void DeteleValidEventInInteractions()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());

            //Get last id in the event list
            var responseList = _interactionsClient.Get<EventList>(request);
            Event lastEvent = responseList.Data.Items[responseList.Data.Items.Count - 1];
            string lastId = lastEvent.EventId.ToString();
            request.AddQueryParameter("id", lastId);

            var response = _interactionsClient.Delete(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response: " + response.Content);

            //Make sure it's deleted
            response = _interactionsClient.Get<Event>(request);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response: " + response.Content);
        }

        [Test]
        public void DeteleInvalidEventInInteractions()
        {
            RestRequest request = new RestRequest();
            request.SetJsonContentType();
            request.AddQueryParameter("sessionId", NewSession());
            request.AddQueryParameter("id", "0");

            var response = _interactionsClient.Delete(request);

            TestContext.Out.WriteLine(response.Content);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response: " + response.Content);
        }
    }
}