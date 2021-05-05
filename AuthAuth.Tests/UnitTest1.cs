using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;
using AuthAuthEasyLib;
using AuthAuthEasyLib.Services;
using AuthAuthEasyLib.Interfaces;

namespace AuthAuth.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mongoConfig = new MongoCrudServiceConfig("", "", "");
            var authService = new AuthService<IAuthUser>(mongoConfig);
        }


    }
}
