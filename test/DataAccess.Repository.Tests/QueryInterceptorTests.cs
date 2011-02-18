// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryInterceptorTests.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Summary description for InterceptorTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Basic;

    using Extended;
    using Extended.Events;
    using Extended.Interceptors;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SampleModel;
    using SampleModel.Interceptors;

    /// <summary>
    /// Summary description for InterceptorTest
    /// </summary>
    [TestClass]
    public class QueryInterceptorTests : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The extended repository_ returns_ all_ from_ inner_ repository.
        /// </summary>
        [TestMethod]
        public void ExtendedRepository_Returns_All_From_Inner_Repository()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ load options creating_if_subscribed.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_LoadOptionsCreating_if_subscribed()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            bool loadOptionsCreatingExecuted = false;

            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) => { methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First(); });
            testInterceptor
                .Setup(i => i.OnLoadOptionsCreating(It.IsAny<LoadOptionsCreatingEventArgs>()))
                .Callback(() => loadOptionsCreatingExecuted = true);

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(It.Is<Type>(t => t == typeof(TestInterceptor))))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.IsTrue(loadOptionsCreatingExecuted);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ method call visit_if_subscribed.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_MethodCallVisit_if_subscribed()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            bool methodCallExecuted = false;

            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallExecuted = true;

                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(typeof(TestInterceptor)))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.IsTrue(methodCallExecuted);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ pre execute_if_subscribed.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_PreExecute_if_subscribed()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            bool preExecuteExecuted = false;

            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) => { methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First(); });
            testInterceptor
                .Setup(i => i.OnPreExecute(It.IsAny<PreExecuteEventArgs>()))
                .Callback(() => preExecuteExecuted = true);

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(typeof(TestInterceptor)))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.IsTrue(preExecuteExecuted);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ query created_if_subscribed.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_QueryCreated_if_subscribed()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            bool queryCreatedExecuted = false;

            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) => { methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First(); });
            testInterceptor
                .Setup(i => i.OnQueryCreated(It.IsAny<QueryCreatedEventArgs>()))
                .Callback(() => queryCreatedExecuted = true);

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(It.Is<Type>(t => t == typeof(TestInterceptor))))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.IsTrue(queryCreatedExecuted);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ query creating_if_subscribed.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_QueryCreating_if_subscribed()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            bool queryCreatingExecuted = false;

            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) => { methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First(); });
            testInterceptor
                .Setup(i => i.OnQueryCreating(It.IsAny<QueryCreatingEventArgs>()))
                .Callback(() => queryCreatingExecuted = true);

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(It.Is<Type>(t => t == typeof(TestInterceptor))))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.IsTrue(queryCreatingExecuted);
        }

        /// <summary>
        /// The interceptor_should_be_initialized_with_current_ query context_and_ scope_objects.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_initialized_with_current_QueryContext_and_Scope_objects()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance(mockRepository.Object);

            QueryContext providedQueryContext = null;
            IScope providedScope = null;

            // creating Interceptor mock
            var testInterceptor = new Mock<IQueryInterceptor>();
            testInterceptor
                .Setup(i => i.OnMethodCallVisit(It.IsAny<MethodCallVisitEventArgs>()))
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) => { methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First(); });
            testInterceptor
                .Setup(i => i.Initialize(It.IsAny<QueryContext>(), It.IsAny<IScope>()))
                .Callback((QueryContext queryContext, IScope scope) =>
                    {
                        providedQueryContext = queryContext;
                        providedScope = scope;
                    });

            // registering InterceptorFactory
            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateQueryInterceptor(typeof(TestInterceptor)))
                .Returns(testInterceptor.Object);

            this.Container.RegisterInstance(mockInterceptorFactory.Object);

            // registering IScope (twice!)
            var mockScope = new Mock<ITestScope>();
            this.Container.RegisterInstance<IScope>(mockScope.Object);
            this.Container.RegisterInstance<ITestScope>(mockScope.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>().TestMethod().ToList();

            // Assert
            Assert.AreSame(typeof(SampleEntity), providedQueryContext.ElementType);
            Assert.AreSame(mockScope.Object, providedScope);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create sample entity repository mock.
        /// </summary>
        /// <returns>
        /// </returns>
        private static Mock<IRepository> CreateSampleEntityRepositoryMock()
        {
            List<SampleEntity> mockResult = new List<SampleEntity>()
                {
                    new SampleEntity() { Id = 1 }, 
                    new SampleEntity() { Id = 2 }, 
                    new SampleEntity() { Id = 3 }
                };

            var mockRepository = new Mock<IRepository>();

            mockRepository
                .Setup(r => r.All<SampleEntity>(It.IsAny<LoadOptions>()))
                .Returns(mockResult.AsQueryable());
            mockRepository
                .Setup(r => r.All(typeof(SampleEntity), It.IsAny<LoadOptions>()))
                .Returns(mockResult.AsQueryable());

            return mockRepository;
        }

        #endregion
    }
}