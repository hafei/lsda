using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicSoftware.DataAccess.Repository.Mapping;
using System.Reflection;

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    public class CoreTestsMappingSourceManager : XmlMappingSourceManager
    {
        public CoreTestsMappingSourceManager()
            : base(Assembly.GetExecutingAssembly().GetManifestResourceStream("LogicSoftware.DataAccess.Repository.Tests.Core.SimpleEntityMapping.xml"))
        {

        }
    }
}
