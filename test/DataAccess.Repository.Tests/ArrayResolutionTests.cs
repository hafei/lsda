// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayResolutionTests.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The array resolution tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The array resolution tests.
    /// </summary>
    [TestClass]
    public class ArrayResolutionTests : UnitTestBase
    {
        #region Interfaces

        /// <summary>
        /// The dependency interface.
        /// </summary>
        public interface IDependency
        {
        }

        /// <summary>
        /// The service interface.
        /// </summary>
        public interface IService
        {
            IDependency Dependency { get; set; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The array_dependency_items_should_be_resolved_in_current_context.
        /// </summary>
        [TestMethod]
        public void Array_dependency_items_should_be_resolved_in_current_context()
        {
            // using default container
            var defaultContainer = new UnityContainer();

            defaultContainer.RegisterType<IDependency, Dependency>(new PerResolveLifetimeManager());
            defaultContainer.RegisterType<IService, Service1>();
            defaultContainer.RegisterType<IService, Service2>("service2");
            defaultContainer.RegisterType<IService, Service3>("service3");

            var parent = defaultContainer.Resolve<Parent>();

            Assert.AreEqual(2, parent.Services.Length);

            foreach (var service in parent.Services)
            {
                Assert.AreNotSame(parent.Dependency, service.Dependency);
            }

            // now using container with overriden array dependency resolution logic
            this.Container.RegisterType<IDependency, Dependency>(new PerResolveLifetimeManager());
            this.Container.RegisterType<IService, Service1>();
            this.Container.RegisterType<IService, Service2>("service2");
            this.Container.RegisterType<IService, Service3>("service3");

            parent = this.Container.Resolve<Parent>();

            Assert.AreEqual(3, parent.Services.Length);

            foreach (var service in parent.Services)
            {
                Assert.AreSame(parent.Dependency, service.Dependency);
            }
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// The common dependency.
        /// </summary>
        public class Dependency : IDependency
        {
        }

        /// <summary>
        /// The parent.
        /// </summary>
        public class Parent
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Parent"/> class.
            /// </summary>
            /// <param name="services">
            /// The services.
            /// </param>
            /// <param name="dependency">
            /// The dependency.
            /// </param>
            public Parent(IService[] services, IDependency dependency)
            {
                this.Services = services;
                this.Dependency = dependency;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the dependency.
            /// </summary>
            /// <value>The dependency.</value>
            public IDependency Dependency { get; set; }

            /// <summary>
            /// Gets or sets the services.
            /// </summary>
            /// <value>The services.</value>
            public IService[] Services { get; set; }

            #endregion
        }

        /// <summary>
        /// The service 1.
        /// </summary>
        public class Service1 : IService
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Service1"/> class.
            /// </summary>
            /// <param name="dependency">
            /// The dependency.
            /// </param>
            public Service1(IDependency dependency)
            {
                this.Dependency = dependency;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the dependency.
            /// </summary>
            /// <value>The dependency.</value>
            public IDependency Dependency { get; set; }

            #endregion
        }

        /// <summary>
        /// The service 2.
        /// </summary>
        public class Service2 : IService
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Service2"/> class.
            /// </summary>
            /// <param name="dependency">
            /// The dependency.
            /// </param>
            public Service2(IDependency dependency)
            {
                this.Dependency = dependency;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the dependency.
            /// </summary>
            /// <value>The dependency.</value>
            public IDependency Dependency { get; set; }

            #endregion
        }

        /// <summary>
        /// The service 3.
        /// </summary>
        public class Service3 : IService
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Service3"/> class.
            /// </summary>
            /// <param name="dependency">
            /// The dependency.
            /// </param>
            public Service3(IDependency dependency)
            {
                this.Dependency = dependency;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the dependency.
            /// </summary>
            /// <value>The dependency.</value>
            public IDependency Dependency { get; set; }

            #endregion
        }

        #endregion
    }
}