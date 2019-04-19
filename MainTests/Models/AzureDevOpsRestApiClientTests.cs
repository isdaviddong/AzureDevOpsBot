using Microsoft.VisualStudio.TestTools.UnitTesting;
using Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Tests
{
    [TestClass()]
    public class AzureDevOpsRestApiClientTests
    {
        [TestMethod()]
        public void QueueNewBuildTest()
        {
            var client = new AzureDevOpsRestApiClient(
                " ", " ", 
                " ", " ");

            client.QueueNewBuild(19);

            Assert.IsTrue(true);
        }
    }
}