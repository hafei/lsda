using System;
using System.Collections.Generic;
using System.Linq;
using LogicSoftware.DataAccess.Repository.Extended;
using LogicSoftware.DataAccess.Repository.Extended.Events;
using LogicSoftware.DataAccess.Repository.Extended.Interceptors;
using LogicSoftware.DataAccess.Repository.Tests.SampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Basic;

    /// <summary>
    /// Summary description for InterceptorTest
    /// </summary>
    [TestClass]
    public class QueryInterceptorTests : UnitTestBase
    {
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
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });
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
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });
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
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });
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
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });
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
                .Callback((MethodCallVisitEventArgs methodCallVisitEventArgs) =>
                    {
                        methodCallVisitEventArgs.SubstituteExpression = methodCallVisitEventArgs.MethodCall.Arguments.First();
                    });
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
    }
}
