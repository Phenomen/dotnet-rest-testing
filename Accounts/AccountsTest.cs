using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Accounts
{
    [TestFixture]
    public class AccountsTest
    {
        private ServiceReference1.AccountsClient _client;


        [OneTimeSetUp]
        public void Initialize()
        {
            _client = new ServiceReference1.AccountsClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Close();
        }

        //256 symbols string
        private const string lipsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, "+ 
            "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
            "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in";

        //////////////////////////////REGISTRATION

        //Register with empty username should throw exception
        [TestCase("", "fakepassword")]
        public void RegisterWithEmptyUsername(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Value cannot be null.")
                            .And.Message.Contains("Parameter name: login"),
            () => _client.Registrate(login, password));
        }

        //Register with empty password should throw exception
        [TestCase("chupacabra", "")]
        public void RegisterWithEmptyPassword(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Value cannot be null.")
                            .And.Message.Contains("Parameter name: password"),
            () => _client.Registrate(login + RandomGenerator.GetRandomString(), password));
        }

        //Register with password < 6 symbols should throw exception
        [TestCase("chupacabra", "12345")]
        public void RegisterWithShortPassword(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Password should have not have less than 6 symbols"),
            () => _client.Registrate(login + RandomGenerator.GetRandomString(), password));
        }

        //Register with password > 255 symbols should throw exception
        [TestCase("chupacabra")]
        public void RegisterWithLongPassword(string login)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Password should have not have more than 255 symbols"),
            () => _client.Registrate(login + RandomGenerator.GetRandomString(), lipsum));
        }

        //Register with username < 6 symbols should throw exception
        [TestCase("chu", "LegitPassword.123")]
        public void RegisterWithShortUsername(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Login should have not have less than 6 symbols"),
            () => _client.Registrate(login, password));
        }

        //Register with username > 255 symbols should throw exception
        [TestCase(null, "LegitPassword.123")]
        public void RegisterWithLongUsername(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Login should have not have more than 255 symbols"),
            () => _client.Registrate(lipsum, password));
        }

        //Register with username that does not satisfy conditions should throw exception
        [Test]
        public void RegisterWithIncorrectUsername()
        {
            List<string> list = new List<string> { "!", "?", "#", "$", "%", "№", "©", "^", "&", "*", "'", "`", "~", "(", ")", "|", "/", "\\", ";", ":", "<", ">", "+", "=", "а", "Ё", "й" };
            foreach (string symbol in list)
            {
                Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Login should have only letters, digits and one of the following symbols: . , @ - _"),
                () => _client.Registrate("chupacabra_" + RandomGenerator.GetRandomString() + symbol, "LegitPassword.123"));

                TestContext.Out.WriteLine(symbol, "Check \"Output\" in test results to see symbols that failed assertion");
            }
        }

        //Register with password that does not satisfy at least 3 conditions should throw exception
        [TestCase("LegitUser_", "Password")] //2 conditions
        [TestCase("LegitUser_", "password1")] //2 conditions
        [TestCase("LegitUser_", "password©")] //2 conditions
        [TestCase("LegitUser_", "p@ssword")] //2 conditions
        [TestCase("LegitUser_", "p.ssword")] //2 conditions
        [TestCase("LegitUser_", "1.2.3.4.5")] //2 conditions
        [TestCase("LegitUser_", "Пароль123")] //1 condition. Failed: Cyrillic is accepted as valid condition.
        [TestCase("LegitUser_", "@@@@@@")] //1 condition
        [TestCase("LegitUser_", "p......!")] //1 condition
        public void RegisterWithIncorrectPassword(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Password should satisfy at least 3 conditions"),
            () => _client.Registrate(login + RandomGenerator.GetRandomString(), password));
        }

        //Register with existing credentials should throw exception
        [TestCase("RealUser.123", "RealPassword.123")] //Failed because scenario is not implemented
        public void RegisterWithExistingCredentials(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                           .And.Message.Contains("Username is already used"),
           () => _client.Registrate(login, password));
        }

        //Register with correct credentials should return user
        [TestCase("RealUser.", "RealPassword.©")] 
        public void RegisterWithCorrectCredentials(string login, string password)
        {
            string randomLogin = login + RandomGenerator.GetRandomString();
            ServiceReference1.AccountModel result = _client.Registrate(randomLogin, password);
            Assert.AreEqual(randomLogin, result.Login);         
        }

        //Register with password that does satisfy ALL conditions should return user
        [TestCase("LegitUser_", "P.©swW076")] 
        public void RegisterWithPasswordThatSatisfyAllConditions(string login, string password)
        {
            string randomLogin = login + RandomGenerator.GetRandomString();
            ServiceReference1.AccountModel result = _client.Registrate(randomLogin, password);
            Assert.AreEqual(randomLogin, result.Login);
        }

        //////////////////////////////LOGIN

        //Login with empty username should throw exception
        [TestCase("", "fakepassword")]
        [TestCase("", "")]
        public void LoginWithEmptyUsername(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Value cannot be null.")
                            .And.Message.Contains("Parameter name: login"),
            () => _client.Login(login, password));
        }

        //Login with incorrect username should throw exception
        [TestCase("fakelogin", "fakepassword")]
        public void LoginWithIncorrectUsername(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Login not found"),
            () => _client.Login(login, password));
        }

        //Login with incorrect password should throw exception
        [TestCase("RealUser.123", "fakepassword")] 
        public void LoginWithIncorrectPassword(string login, string password)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.Security.SecurityAccessDeniedException>()
                            .And.Message.Contains("Incorrect password"),
            () => _client.Login(login, password));
        }


        //Login with >50 username
        [TestCase("RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.RealUser.", "RealPassword.©")]
        public void LoginWithLongUsername(string login, string password)
        {
            string randomLogin = login + RandomGenerator.GetRandomString();
            _client.Registrate(randomLogin, password);
            string sessionId = _client.Login(randomLogin, password);
            bool result = _client.Logout(sessionId);
            Assert.IsTrue(result);
        }

        //Login with correct credentials return correct session ID
        [TestCase("RealUser.123", "RealPassword.123")]
        public void LoginWithCorrectPassword(string login, string password)
        {
            string sessionId = _client.Login(login, password);
            Assert.IsNotNull(sessionId);
        }

        //////////////////////////////LOGOUT

        //Logout existing session should return True
        [TestCase("RealUser.123", "RealPassword.123")]
        public void LogoutCorrectSession(string login, string password)
        {
            string sessionId = _client.Login(login, password);
            bool result = _client.Logout(sessionId);
            Assert.IsTrue(result);
        }

        //Logout non-existing session should return False
        [Test]
        public void LogoutIncorrectSession()
        {
            bool result = _client.Logout("11111111-1111-1111-1111-111111111111");
            Assert.IsFalse(result);
        }

        //Logout with empty session should throw exception
        [Test]
        public void LogoutWithEmptySession()
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Value cannot be null.")
                            .And.Message.Contains("Parameter name: sessionId"),
            () => _client.Logout(""));
        }

        //Logout with incorrect GUID format should throw exception
        [Test]
        public void LogoutWithIncorrectGUID()
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)."),
            () => _client.Logout("1111"));
        }

        //////////////////////////////GET USER BY SESSION ID

        //Get user with existing session should return user login
        [TestCase("RealUser.123", "RealPassword.123")]
        public void GetUserWithExistingSessionId(string login, string password)
        {
            string sessionId = _client.Login(login, password);
            ServiceReference1.AccountModel result = _client.GetUserBySessionId(sessionId);
            string userLogin = result.Login;
            Assert.AreEqual(login, userLogin);
        }

        //Get user with non-existing session should throw exception
        [TestCase("11111111-1111-1111-1111-111111111111")]
        public void GetUserWithNonExistingSessionId(string sessionId)
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                           .And.Message.Contains($"User with session ID {sessionId} is not found."),
            () => _client.GetUserBySessionId(sessionId));
        }

        //Get user with empty session should throw exception
        [Test]
        public void GetUserWithEmptySession()
        {
            Assert.Throws(Is.InstanceOf<System.ServiceModel.FaultException>()
                            .And.Message.Contains("Value cannot be null.")
                            .And.Message.Contains("Parameter name: sessionId"),
            () => _client.GetUserBySessionId(""));
        }
    }
}