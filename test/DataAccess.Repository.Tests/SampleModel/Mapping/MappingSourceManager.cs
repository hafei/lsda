using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicSoftware.DataAccess.Repository.Mapping;
using System.Reflection;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Mapping
{
    public class MappingSourceManager : XmlMappingSourceManager
    {
        public MappingSourceManager()
            : base(Assembly.GetExecutingAssembly().GetManifestResourceStream("LogicSoftware.DataAccess.Repository.Tests.SampleModel.Mapping.LinqToSqlMapping.xml"))
        {

        }
    }
}
