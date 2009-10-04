using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    public class SampleParentEntity
    {
        public int Id { get; set; }

        public int SuperParentId { get; set; }

        public SampleSuperParentEntity SuperParent { get; set; }

        public List<SampleChildEntity> Children { get; set; }

        public string Name { get; set; }
    }
}
