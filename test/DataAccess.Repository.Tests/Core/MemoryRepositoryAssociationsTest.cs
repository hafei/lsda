using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using Memory;

    using Basic;

    [TestClass]
    public class MemoryRepositoryAssociationsTest
    {
        [TestMethod]
        public void Load_With_No_Associations()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassA entityA = new ClassA() { };

            repository.Insert(entityA);

            ClassB entityB = new ClassB() 
            {
                ClassA = entityA
            };

            repository.Insert(entityB);

            var selected = repository.All<ClassB>().Where(b => b.Id == entityB.Id).SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.IsNull(selected.ClassA);
        }

        [TestMethod]
        public void Load_With_Many_To_One()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassA entityA = new ClassA() { };

            repository.Insert(entityA);

            ClassB entityB = new ClassB()
            {
                ClassAId = entityA.Id
            };

            repository.Insert(entityB);

            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassB>(b => b.ClassA);

            var selected = repository.All<ClassB>(options)
                .Where(b => b.Id == entityB.Id)
                .SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.IsNotNull(selected.ClassA);
            Assert.AreEqual(entityA.Id, selected.ClassA.Id);
        }

        [TestMethod]
        public void Load_With_Many_To_One_Chain()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassA entityA = new ClassA() { };
            repository.Insert(entityA);

            ClassB entityB = new ClassB() { ClassAId = entityA.Id };
            repository.Insert(entityB);

            ClassC entityC = new ClassC() { ClassBId = entityB.Id };
            repository.Insert(entityC);

            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassB>(b => b.ClassA);
            options.LoadWith<ClassC>(c => c.ClassB);

            var selected = repository.All<ClassC>(options)
                .Where(c => c.Id == entityC.Id)
                .SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.IsNotNull(selected.ClassB);
            Assert.IsNotNull(selected.ClassB.ClassA);

            Assert.AreEqual(entityB.Id, selected.ClassB.Id);
            Assert.AreEqual(entityA.Id, selected.ClassB.ClassA.Id);
        }

        [TestMethod]
        public void Load_With_One_To_Many()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassA entityA = new ClassA() { };
            repository.Insert(entityA);

            ClassB entityB1 = new ClassB() { ClassAId = entityA.Id };
            repository.Insert(entityB1);

            ClassB entityB2 = new ClassB() { ClassAId = entityA.Id };
            repository.Insert(entityB2);

            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassA>(a => a.ClassBs);

            var selected = repository.All<ClassA>(options).Where(a => a.Id == entityA.Id).SingleOrDefault();

            Assert.IsNotNull(selected);
            /*Assert.IsNotNull(selected);
            Assert.IsNotNull(selected.ClassA);
            Assert.AreEqual(entityA.Id, selected.ClassA.Id);
             * */
        }

        [TestMethod]
        public void MemoryRepository_should_support_filters_in_LoadWith_expressions()
        {
            IRepository repository = new MemoryRepository(new CoreTestsMappingSourceManager());

            ClassA entityA = new ClassA();
            repository.Insert(entityA);

            repository.Insert(new ClassB() { ClassAId = entityA.Id, Name = "B1" });
            repository.Insert(new ClassB() { ClassAId = entityA.Id, Name = "B2" });
            repository.Insert(new ClassB() { ClassAId = entityA.Id, Name = "B3" });

            LoadOptions options = new LoadOptions();
            options.LoadWith<ClassA>(a => a.ClassBs.Where(b => b.Name == "B1" || b.Name == "B2"));

            var selected = repository.All<ClassA>(options).SingleOrDefault();

            Assert.IsNotNull(selected);
            Assert.IsNotNull(selected.ClassBs);
            Assert.AreEqual(2, selected.ClassBs.Count);
        }
    }
}
