using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    using Extended.Attributes.Views;

    [View(typeof(SampleChildEntity))]
    public class SampleChildEntityView
    {
        [Property("Name")]
        public string Name { get; set; }

        [Property("Name")]
        public string EntityName { get; set; }

        [Property("Parent.Name")]
        public string ParentName { get; set; }

        [Property("Parent.SuperParent.Name")]
        public string SuperParentName { get; set; }
    }
}
