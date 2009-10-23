using LogicSoftware.DataAccess.Repository.Mapping;
using LogicSoftware.DataAccess.Repository.Tests.SampleModel.Mapping;
using LogicSoftware.Infrastructure.EntLib.Unity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicSoftware.DataAccess.Repository.Tests
{
    [TestClass]
    public class UnitTestBase
    {
        protected IUnityContainer Container { get; set; }

        [TestInitialize]
        public void InitContainer()
        {
            this.Container = UnityBootstrapper.Bootstrap();
            this.Container.RegisterType<IMappingSourceManager, MappingSourceManager>();
        }
    }
}
