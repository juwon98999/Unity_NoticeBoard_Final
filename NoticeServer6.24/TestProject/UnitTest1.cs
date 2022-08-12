using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinalTestSample;

using System;
using System.IO;
using System.Text.Json;

namespace FinalTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            if (FinalTestSample.Program.Main() == 0)
            {

                Assert.IsTrue(false);

            }
        }
        [TestMethod]
        public void TestMethod2()
        {
            FileLogger f = new FileLogger("Log");

            if(f._fileName != "Log220616.log")
            {
                Assert.IsTrue(false);
            }

            
        }
    }
}