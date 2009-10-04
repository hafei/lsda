using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    public class SampleChildEntity
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public SampleParentEntity Parent { get; set; }

        public string Name { get; set; }
    }
}
