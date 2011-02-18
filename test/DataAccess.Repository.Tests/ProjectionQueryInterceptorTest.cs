// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionQueryInterceptorTest.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The projection query interceptor test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using Basic;

    using Extended;
    using Extended.Interceptors.Common;
    using Extended.Interceptors.Common.Attributes;

    using Memory;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleModel;
    using SampleModel.Mapping;
    using SampleModel.Projections;

    /// <summary>
    /// The projection query interceptor test.
    /// </summary>
    [TestClass]
    public class ProjectionQueryInterceptorTest : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The all_advanced_projection_features_should_work.
        /// </summary>
        [TestMethod]
        public void All_advanced_projection_features_should_work()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var projectionResult = extendedRepository.All<SampleParentEntity>()
                .Select(AdvancedParentEntityProjection.WithPatternAndTake("arent", 3))
                .ToList();

            var queryResult = extendedRepository.All<SampleParentEntity>()
                .Where(u => u.Name.Contains("arent"))
                .OrderByDescending(u => u.Name)
                .Skip(1)
                .Take(3)
                .Select(e => new AdvancedParentEntityProjection
                    {
                        Id = e.Id, 
                        Name = e.Name, 
                        SuperParent = new AdvancedParentEntityProjection.SuperParentProjection
                            {
                                Id = e.SuperParent.Id, 
                                Name = e.SuperParent.Name
                            }, 
                        Children = e.Children
                            .OrderByDescending(c => c.Name)
                            .Select(c => new AdvancedParentEntityProjection.ChildProjection
                                {
                                    Id = c.Id, 
                                    Name = c.Name
                                })
                            .ToList()
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(projectionResult, queryResult), "Projection result differs from direct query.");
        }

        /// <summary>
        /// The instance_methods_should_be_ignored_if_calling_without_projection_instance.
        /// </summary>
        [TestMethod]
        public void Instance_methods_should_be_ignored_if_calling_without_projection_instance()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var instanceProjectionResult = extendedRepository.All<SampleParentEntity>()
                .Select(AdvancedParentEntityProjection.WithPatternAndTake("arent 1", 1))
                .ToList();

            var staticProjectionResult = extendedRepository.All<SampleParentEntity>()
                .Select<AdvancedParentEntityProjection>()
                .ToList();

            var instanceQueryResult = extendedRepository.All<SampleParentEntity>()
                .Where(u => u.Name.Contains("arent 1"))
                .OrderByDescending(u => u.Name)
                .Skip(1)
                .Take(1)
                .Select(e => new AdvancedParentEntityProjection
                    {
                        Id = e.Id, 
                        Name = e.Name, 
                        SuperParent = new AdvancedParentEntityProjection.SuperParentProjection
                            {
                                Id = e.SuperParent.Id, 
                                Name = e.SuperParent.Name
                            }, 
                        Children = e.Children
                            .OrderByDescending(c => c.Name)
                            .Select(c => new AdvancedParentEntityProjection.ChildProjection
                                {
                                    Id = c.Id, 
                                    Name = c.Name
                                })
                            .ToList()
                    })
                .ToList();

            var staticQueryResult = extendedRepository.All<SampleParentEntity>()
                .OrderByDescending(u => u.Name)
                .Skip(1)
                .Select(e => new AdvancedParentEntityProjection
                    {
                        Id = e.Id, 
                        Name = e.Name, 
                        SuperParent = new AdvancedParentEntityProjection.SuperParentProjection
                            {
                                Id = e.SuperParent.Id, 
                                Name = e.SuperParent.Name
                            }, 
                        Children = e.Children
                            .OrderByDescending(c => c.Name)
                            .Select(c => new AdvancedParentEntityProjection.ChildProjection
                                {
                                    Id = c.Id, 
                                    Name = c.Name
                                })
                            .ToList()
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(instanceProjectionResult, instanceQueryResult), "Projection result differs from direct query.");
            Assert.IsTrue(Compare(staticProjectionResult, staticQueryResult), "Projection result differs from direct query.");
        }

        /// <summary>
        /// The lift_to_null_should_be_supported_in_binding.
        /// </summary>
        [TestMethod]
        public void Lift_to_null_should_be_supported_in_binding()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var projectionResult = extendedRepository.All<SampleParentEntity>()
                .Select<ProjectionWithLifting>()
                .ToList();

            var queryResult = extendedRepository.All<SampleParentEntity>()
                .Select(e => new ProjectionWithLifting
                    {
                        Id = e.Id
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(projectionResult, queryResult), "Projection result differs from direct query.");
        }

        /// <summary>
        /// The projections_should_support_deep_level_properties.
        /// </summary>
        [TestMethod]
        public void Projections_should_support_deep_level_properties()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var projectionResult = extendedRepository.All<SampleChildEntity>()
                .Select<SampleChildEntityView>()
                .ToList();

            var queryResult = extendedRepository.All<SampleChildEntity>()
                .Select(e => new SampleChildEntityView
                    {
                        Name = e.Name, 
                        EntityName = e.Name, 
                        ParentName = e.Parent.Name, 
                        SuperParentName = e.Parent.SuperParent.Name
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(projectionResult, queryResult), "Projection result differs from direct query.");
        }

        /// <summary>
        /// The projections_should_support_expressions.
        /// </summary>
        [TestMethod]
        public void Projections_should_support_expressions()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var projectionResult = extendedRepository.All<SampleParentEntity>()
                .Select<SampleParentEntityExpressionView>()
                .ToList();

            var queryResult = extendedRepository.All<SampleParentEntity>()
                .Select(e => new SampleParentEntityExpressionView
                    {
                        Name = e.Name, 
                        Children = e.Children.Where(c => c.Name.EndsWith("1")).ToList()
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(projectionResult, queryResult), "Projection result differs from direct query.");
        }

        /// <summary>
        /// The projections_should_support_sub_projections.
        /// </summary>
        [TestMethod]
        public void Projections_should_support_sub_projections()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var projectionResult = extendedRepository.All<SampleParentEntity>()
                .Select<SampleParentEntitySubProjectionView>()
                .ToList();

            var queryResult = extendedRepository.All<SampleParentEntity>()
                .Select(e => new SampleParentEntitySubProjectionView
                    {
                        Name = e.Name, 
                        ChildrenViews = e.Children
                            .Select(c => new SampleChildEntityView
                                {
                                    Name = c.Name, 
                                    EntityName = c.Name, 
                                    ParentName = c.Parent.Name, 
                                    SuperParentName = c.Parent.SuperParent.Name
                                })
                            .ToList()
                    })
                .ToList();

            // Assert
            Assert.IsTrue(Compare(projectionResult, queryResult), "Projection result differs from direct query.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the specified obj1 with obj2 via xml serialization.
        /// </summary>
        /// <param name="obj1">
        /// The object 1.
        /// </param>
        /// <param name="obj2">
        /// The object 2.
        /// </param>
        /// <returns>
        /// True if objects are equal.
        /// </returns>
        private static bool Compare(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if ((obj1 == null) ^ (obj2 == null))
            {
                return false;
            }

            if (obj1.GetType() != obj2.GetType())
            {
                return false;
            }

            var serializer = new XmlSerializer(obj1.GetType());
            using (StringWriter sw1 = new StringWriter(), sw2 = new StringWriter())
            {
                serializer.Serialize(sw1, obj1);
                serializer.Serialize(sw2, obj2);

                return sw1.ToString() == sw2.ToString();
            }
        }

        /// <summary>
        /// Creates the sample entity repository mock.
        /// </summary>
        /// <returns>
        /// The sample entity repository mock.
        /// </returns>
        private static IRepository CreateSampleEntityRepository()
        {
            var memoryRepository = new MemoryRepository(new MappingSourceManager());

            memoryRepository.Insert(new SampleSuperParentEntity() { Id = 1, Name = "SuperParent 1" });
            memoryRepository.Insert(new SampleParentEntity() { Id = 1, Name = "Parent 1 1", SuperParentId = 1 });
            memoryRepository.Insert(new SampleParentEntity() { Id = 2, Name = "Parent 1 2", SuperParentId = 1 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 1, Name = "Child '1 2' 1", ParentId = 2 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 2, Name = "Child '1 2' 2", ParentId = 2 });

            memoryRepository.Insert(new SampleSuperParentEntity() { Id = 2, Name = "SuperParent 2" });
            memoryRepository.Insert(new SampleParentEntity() { Id = 3, Name = "Parent 2 1", SuperParentId = 2 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 3, Name = "Child '2 1' 1", ParentId = 3 });
            memoryRepository.Insert(new SampleParentEntity() { Id = 4, Name = "Parent 2 2", SuperParentId = 2 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 4, Name = "Child '2 2' 1", ParentId = 4 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 5, Name = "Child '2 2' 2", ParentId = 4 });
            memoryRepository.Insert(new SampleChildEntity() { Id = 6, Name = "Child '2 2' 3", ParentId = 4 });

            return memoryRepository;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// The projection with lifting.
        /// </summary>
        [Projection(typeof(SampleParentEntity))]
        public class ProjectionWithLifting
        {
            #region Properties

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id of the entity.</value>
            [SelectProperty]
            public int? Id { get; set; }

            #endregion
        }

        #endregion
    }
}