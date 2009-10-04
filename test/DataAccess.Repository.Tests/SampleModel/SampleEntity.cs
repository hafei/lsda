using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicSoftware.DataAccess.Repository.Extended.Attributes;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    [Intercept(typeof(TestOperationInterceptor))]
    public class SampleEntity
    {
        public int Id { get; set; }
    }
}
