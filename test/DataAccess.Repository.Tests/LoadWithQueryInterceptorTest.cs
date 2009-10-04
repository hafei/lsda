using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicSoftware.DataAccess.Repository.Tests.SampleModel;
using LogicSoftware.DataAccess.Repository.Extended;
using Moq;
using System.Linq.Expressions;

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Extended.Interceptors.Common;

    using Basic;

    /// <summary>
    /// Summary description for LoadWithQueryInterceptorTest
    /// </summary>
    [TestClass]
    public class LoadWithQueryInterceptorTest : UnitTestBase
    {
        [TestMethod]
        public void Load_Children_Without_Parent()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleChildEntity>().ToList();

            // Assert
            int notNullParentsCount = result.Where(e => e.Parent != null).Count();
            Assert.AreEqual(0, notNullParentsCount);
        }

        [TestMethod]
        public void Load_Children_With_Parent()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleChildEntity>()
                .LoadWith(c => c.Parent)
                .ToList();

            // Assert
            int notNullParentsCount = result.Where(e => e.Parent != null).Count();
            Assert.AreEqual(3, notNullParentsCount);
        }

        [TestMethod]
        public void Load_Children_With_Parent_And_SuperParent()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act

            var result = extendedRepository.All<SampleChildEntity>()
                .LoadWith(c => c.Parent)
                .LoadWith<SampleChildEntity, SampleParentEntity>(p => p.SuperParent)
                .ToList();

            // Assert
            int notNullParentAndSuperParentCount = result.Where(e => e.Parent != null && e.Parent.SuperParent != null).Count();
            Assert.AreEqual(3, notNullParentAndSuperParentCount);
        }

        [TestMethod]
        public void Load_Children_With_Parent_Then_Without()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var resultWith = extendedRepository.All<SampleChildEntity>()
                .LoadWith(c => c.Parent)
                .ToList();

            var resultWithout = extendedRepository.All<SampleChildEntity>()
                .ToList();

            // Assert
            int notNullParentsCountWith = resultWith.Where(e => e.Parent != null).Count();
            Assert.AreEqual(3, notNullParentsCountWith);

            int notNullParentsCountWithout = resultWithout.Where(e => e.Parent != null).Count();
            Assert.AreEqual(0, notNullParentsCountWithout);
        }

        [TestMethod]
        public void Load_Children_Without_Parent_Then_With()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();


            var resultWithout = extendedRepository.All<SampleChildEntity>()
                .ToList();

            // Act
            var resultWith = extendedRepository.All<SampleChildEntity>()
                .LoadWith(c => c.Parent)
                .ToList();

            // Assert
            int notNullParentsCountWithout = resultWithout.Where(e => e.Parent != null).Count();
            Assert.AreEqual(0, notNullParentsCountWithout);

            int notNullParentsCountWith = resultWith.Where(e => e.Parent != null).Count();
            Assert.AreEqual(3, notNullParentsCountWith);
        }

        private static Mock<IRepository> CreateSampleEntityRepositoryMock()
        {
            var mockRepository = new Mock<IRepository>();

            var childrenNoParentsResult = new List<SampleChildEntity>()
            {
                new SampleChildEntity() { Id = 1 },
                new SampleChildEntity() { Id = 2 },
                new SampleChildEntity() { Id = 3 }
            };

            mockRepository
                .Setup(r => r.All<SampleChildEntity>(
                    It.Is<LoadOptions>(lo => lo.LoadWithOptions.Where(IsLoadParentOption).Count() == 0)))
                .Returns(childrenNoParentsResult.AsQueryable());
            mockRepository
                .Setup(r => r.All(
                    typeof(SampleChildEntity),
                    It.Is<LoadOptions>(lo => lo.LoadWithOptions.Where(IsLoadParentOption).Count() == 0)))
                .Returns(childrenNoParentsResult.AsQueryable());

            var childrenWithParentsResult = new List<SampleChildEntity>()
            {
                new SampleChildEntity() { Id = 1, Parent = new SampleParentEntity() },
                new SampleChildEntity() { Id = 2, Parent = new SampleParentEntity() },
                new SampleChildEntity() { Id = 3, Parent = new SampleParentEntity() }
            };

            mockRepository
                .Setup(r => r.All<SampleChildEntity>(
                    It.Is<LoadOptions>(lo =>
                        lo.LoadWithOptions.Where(IsLoadParentOption).Count() > 0 &&
                        lo.LoadWithOptions.Where(IsLoadSuperParentOption).Count() == 0)))
                .Returns(childrenWithParentsResult.AsQueryable());
            mockRepository
                .Setup(r => r.All(
                    typeof(SampleChildEntity),
                    It.Is<LoadOptions>(lo =>
                        lo.LoadWithOptions.Where(IsLoadParentOption).Count() > 0 &&
                        lo.LoadWithOptions.Where(IsLoadSuperParentOption).Count() == 0)))
                .Returns(childrenWithParentsResult.AsQueryable());

            var childrenWithParentsAndSuperResult = new List<SampleChildEntity>()
            {
                new SampleChildEntity() { Id = 1, Parent = new SampleParentEntity() { SuperParent = new SampleSuperParentEntity() }},
                new SampleChildEntity() { Id = 2, Parent = new SampleParentEntity() { SuperParent = new SampleSuperParentEntity() }},
                new SampleChildEntity() { Id = 3, Parent = new SampleParentEntity() { SuperParent = new SampleSuperParentEntity() }}
            };

            mockRepository
                .Setup(r => r.All<SampleChildEntity>(
                    It.Is<LoadOptions>(lo =>
                        lo.LoadWithOptions.Where(IsLoadParentOption).Count() > 0 &&
                        lo.LoadWithOptions.Where(IsLoadSuperParentOption).Count() > 0)))
                .Returns(childrenWithParentsAndSuperResult.AsQueryable());
            mockRepository
                .Setup(r => r.All(
                    typeof(SampleChildEntity),
                    It.Is<LoadOptions>(lo =>
                        lo.LoadWithOptions.Where(IsLoadParentOption).Count() > 0 &&
                        lo.LoadWithOptions.Where(IsLoadSuperParentOption).Count() > 0)))
                .Returns(childrenWithParentsAndSuperResult.AsQueryable());
            
            return mockRepository;
        }

        private static bool IsLoadParentOption(LoadWithOption item)
        {
            var firstParameter = item.Member.Parameters.SingleOrDefault();
            if (firstParameter == null || firstParameter.Type != typeof(SampleChildEntity))
            {
                return false;
            }

            var body = item.Member.Body as MemberExpression;
            if (body == null || body.Member != typeof(SampleChildEntity).GetMember("Parent").Single())
            {
                return false;
            }

            return true;
        }

        private static bool IsLoadSuperParentOption(LoadWithOption item)
        {
            var firstParameter = item.Member.Parameters.SingleOrDefault();
            if (firstParameter == null || firstParameter.Type != typeof(SampleParentEntity))
            {
                return false;
            }

            var body = item.Member.Body as MemberExpression;
            if (body == null || body.Member != typeof(SampleParentEntity).GetMember("SuperParent").Single())
            {
                return false;
            }

            return true;
        }
    }
}
