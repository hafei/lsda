using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using LinqToSql;

    using Memory;

    using Basic;

    /// <summary>
    /// MemoryRepositoryTest tests.
    /// </summary>
    [TestClass]
    public class MemoryRepositoryTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Select_SimpleEntities()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var allEntities = repository.All<SimpleEntity>();

            Assert.IsNotNull(allEntities);

            var list = allEntities.ToList();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void Select_SimpleEntities_Ordered()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var allEntities = repository.All<SimpleEntity>()
                .OrderBy(se => se.StringField);

            Assert.IsNotNull(allEntities);

            var list = allEntities.ToList();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void Insert_SimpleEntity_And_Check_ID_Generation()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entityToInsert = new SimpleEntity() { StringField = "test" };

            repository.Insert(entityToInsert);

            Assert.IsTrue(entityToInsert.Id > 0);
        }

        [TestMethod]
        public void Insert_SimpleEntity_And_Check_Fields()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entityToInsert = new SimpleEntity() { StringField = "test" };

            repository.Insert(entityToInsert);

            var selectedEntity = repository.All<SimpleEntity>().Where(e => e.Id == entityToInsert.Id).SingleOrDefault();

            Assert.IsNotNull(selectedEntity);

            Assert.AreEqual("test", selectedEntity.StringField);
        }

        [TestMethod]
        public void Insert_SimpleEntity_And_Update()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new SimpleEntity() { StringField = "test" };
            repository.Insert(entity);

            entity.StringField = "changed";

            repository.Update(entity);

            var updated = repository.All<SimpleEntity>()
                .Where(e => e.Id == entity.Id).Select(e => e.StringField).SingleOrDefault();

            Assert.IsNotNull(updated);
            Assert.AreEqual("changed", updated);
        }

        [TestMethod]
        public void Insert_And_Delete_SimpleEntity()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new SimpleEntity() { StringField = "test" };
            repository.Insert(entity);

            repository.Delete(entity);

            var count = repository.All<SimpleEntity>().Count();

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void Int_Autogeneration()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassWithGeneratedFields() { };
            repository.Insert(entity);

            Assert.AreNotEqual(0, entity.Id);

            var selected = repository.All<ClassWithGeneratedFields>().Where(e => e.Id == entity.Id).SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.AreNotEqual(0, selected.IntIdentity);
        }

        [TestMethod]
        public void Guid_Autogeneration()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassWithGeneratedFields() { };
            repository.Insert(entity);

            Assert.AreNotEqual(0, entity.Id);

            var selected = repository.All<ClassWithGeneratedFields>().Where(e => e.Id == entity.Id).SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.AreNotEqual(Guid.Empty, selected.GuidRowId);
        }

        [TestMethod]
        public void Guid_Autogeneration_Keeps_Existing_Value()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            Guid existingGuid = Guid.NewGuid();

            var entity = new ClassWithGeneratedFields() { };

            entity.GuidRowId = existingGuid;

            repository.Insert(entity);

            Assert.AreNotEqual(0, entity.Id);

            var selected = repository.All<ClassWithGeneratedFields>().Where(e => e.Id == entity.Id).SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.AreEqual(existingGuid, selected.GuidRowId);
        }

        [TestMethod]
        public void Selecting_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            bool raised = false;

            repository.Selecting += delegate { raised = true; };

            // Act
            repository.All<ClassA>().ToList();

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Selected_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            bool raised = false;

            repository.Selected += delegate { raised = true; };

            // Act
            repository.All<ClassA>().ToList();

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Inserting_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            bool raised = false;

            repository.Inserting += delegate { raised = true; };

            // Act
            repository.Insert(new ClassA());

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Inserted_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            bool raised = false;

            repository.Inserted += delegate { raised = true; };

            // Act
            repository.Insert(new ClassA());

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Updating_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassA();
            repository.Insert(entity);

            bool raised = false;

            repository.Updating += delegate { raised = true; };

            // Act
            repository.Update(entity);

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Updated_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassA();
            repository.Insert(entity);

            bool raised = false;

            repository.Updated += delegate { raised = true; };

            // Act
            repository.Update(entity);

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Deleting_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassA();
            repository.Insert(entity);

            bool raised = false;

            repository.Deleting += delegate { raised = true; };

            // Act
            repository.Delete(entity);

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Deleted_Event_Raised()
        {
            // Arrange
            MemoryRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            var entity = new ClassA();
            repository.Insert(entity);

            bool raised = false;

            repository.Deleted += delegate { raised = true; };

            // Act
            repository.Delete(entity);

            // Assert
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Explicit_Joins_Lists_On_Where()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            repository.Insert(new ClassA());
            repository.Insert(new ClassA());
            repository.Insert(new ClassA());

            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 2 });

            // Act
            var classAwithClassBs = repository.All<ClassA>().Where(a => a.ClassBs.Count > 0).ToList();

            // Assert
            Assert.AreEqual(2, classAwithClassBs.Count);
            
            // make sure that references was not loaded
            Assert.IsNull(classAwithClassBs[0].ClassBs);
            Assert.IsNull(classAwithClassBs[1].ClassBs);
        }

        [TestMethod]
        public void Implicit_Joins_Lists_On_Where()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            repository.Insert(new ClassA());
            repository.Insert(new ClassA());
            repository.Insert(new ClassA());

            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 2 });

            // Act
            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassB>(b => b.ClassA);

            var classBwithClassA1 = repository.All<ClassB>(options).Where(a => a.ClassA.ClassBs.Count > 0).ToList();

            // Assert
            Assert.AreEqual(3, classBwithClassA1.Count);

            Assert.IsTrue(classBwithClassA1.All(b => b.ClassA != null));

            // make sure that references was not loaded
            Assert.IsTrue(classBwithClassA1.All(b => b.ClassA.ClassBs == null));
        }

        [TestMethod]
        public void Explicit_Joins_Lists_On_Select()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            for (int i = 0; i < 100; i++)
            {
                repository.Insert(new ClassA());
                repository.Insert(new ClassA());
                repository.Insert(new ClassA());
            }

            for (int i = 0; i < 100; i++)
            {
                repository.Insert(new ClassB() { ClassAId = 1 });
                repository.Insert(new ClassB() { ClassAId = 1 });
                repository.Insert(new ClassB() { ClassAId = 2 });
            }
            // Act
            var classAwithClassBs = repository.All<ClassA>().Select(a => new { a, a.ClassBs.Count }).ToList();

            // Assert
            Assert.AreEqual(300, classAwithClassBs.Count);

            // make sure that references was not loaded
            Assert.IsTrue(classAwithClassBs.All(p => p.a.ClassBs == null));
            Assert.AreEqual(300, classAwithClassBs.Select(p => p.Count).Sum());
        }

        [TestMethod]
        public void Explicit_Joins_FK_On_Where()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            repository.Insert(new ClassA());
            repository.Insert(new ClassA());
            repository.Insert(new ClassA());

            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 2 });

            // Act
            var classBwithClassA1 = repository.All<ClassB>().Where(b => b.ClassA.Id == 1).ToList();

            // Assert
            Assert.AreEqual(2, classBwithClassA1.Count);

            // make sure that references was not loaded
            Assert.IsNull(classBwithClassA1[0].ClassA);
            Assert.IsNull(classBwithClassA1[1].ClassA);
        }

        [TestMethod]
        public void Explicit_Chain_Joins_FK_On_Where()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            repository.Insert(new ClassA());
            repository.Insert(new ClassA());
            repository.Insert(new ClassA());

            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 2 });

            repository.Insert(new ClassC() { ClassBId = 1 });
            repository.Insert(new ClassC() { ClassBId = 3 });

            // Act
            var classCWithBwithClassA1 = repository.All<ClassC>().Where(c => c.ClassB.ClassA.Id == 1).ToList();

            // Assert
            Assert.AreEqual(1, classCWithBwithClassA1.Count);

            // make sure that references was not loaded
            Assert.IsNull(classCWithBwithClassA1[0].ClassB);
        }

        [TestMethod]
        public void Explicit_Joins_FK_On_Select()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            repository.Insert(new ClassA());
            repository.Insert(new ClassA());
            repository.Insert(new ClassA());

            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 1 });
            repository.Insert(new ClassB() { ClassAId = 2 });

            // Act
            var classBwithClassAId = repository.All<ClassB>().Select(b => new { b, b.ClassA.Id }).ToList();

            // Assert
            Assert.AreEqual(3, classBwithClassAId.Count);

            // make sure that references was not loaded
            Assert.IsTrue(classBwithClassAId.All(p => p.b.ClassA == null));
            Assert.IsTrue(classBwithClassAId.All(p => p.Id > 0));
        }

        [TestMethod]
        public void Implicit_Join_On_Nullable_NotNullable()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassG1 g1 = new ClassG1()
                {
                    RowId = Guid.NewGuid()
                };

            repository.Insert(g1);

            ClassG1 g2 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g2);

            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g2.RowId.Value });

            // Act
            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassG1>(g => g.ClassG2s);

            var reloadedG1 = repository.All<ClassG1>(options).Where(g => g.RowId == g1.RowId).First();

            // Assert
            Assert.IsNotNull(reloadedG1.ClassG2s);
            Assert.AreEqual(2, reloadedG1.ClassG2s.Count);
        }

        [TestMethod]
        public void Implicit_Join_On_NotNullable_Nullable()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassG1 g1 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g1);

            ClassG1 g2 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g2);

            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g2.RowId.Value });

            // Act
            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassG2>(g => g.ClassG1);

            var reloadedG2 = repository.All<ClassG2>(options).Where(g => g.Id == 1).First();

            // Assert
            Assert.IsNotNull(reloadedG2.ClassG1);
        }

        [TestMethod]
        public void Explicit_Join_On_Nullable_NotNullable()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassG1 g1 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g1);

            ClassG1 g2 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g2);

            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g2.RowId.Value });

            // Act
            var reloadedG1 = repository.All<ClassG1>().Where(g => g.ClassG2s.Count == 2).First();

            // Assert
            Assert.AreEqual(g1.RowId, reloadedG1.RowId);
            Assert.IsNull(reloadedG1.ClassG2s);
        }

        [TestMethod]
        public void Explicit_Join_On_NotNullable_Nullable()
        {
            // Arrange
            var repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassG1 g1 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g1);

            ClassG1 g2 = new ClassG1()
            {
                RowId = Guid.NewGuid()
            };

            repository.Insert(g2);

            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g1.RowId.Value });
            repository.Insert(new ClassG2() { RowIdRef = g2.RowId.Value });

            // Act
            var reloadedG2s = repository.All<ClassG2>().Where(g => g.ClassG1.RowId == g1.RowId).ToList();

            // Assert
            Assert.AreEqual(2, reloadedG2s.Count);
            Assert.IsNull(reloadedG2s.First().ClassG1);
        }
    }
}
