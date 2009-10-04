using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using Basic;

    [TestClass]
    public class LoadOptionsTests
    {
        [TestMethod]
        public void All_LoadWithExpressions()
        {
            LoadOptions options = new LoadOptions();

            Expression<Func<ClassB, object>> loadClassAWithClassB = b => b.ClassA;

            options.LoadWith<ClassB>(b => b.ClassA);

            Assert.AreEqual(1, options.LoadWithOptions.Count);

            Assert.AreEqual(loadClassAWithClassB.ToString(), options.LoadWithOptions.First().Member.ToString());
        }
    }
}
