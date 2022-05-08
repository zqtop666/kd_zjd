using Microsoft.VisualStudio.TestTools.UnitTesting;
using CQCore.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQCore.Code.Tests
{
    [TestClass()]
    public class AddUpTests
    {
        [TestMethod()]
        public void djqTest()
        {
            CQCore.Code.AddUp addUp = new AddUp();
            addUp.djq("");
            Assert.IsTrue(true);
        }
    }
}