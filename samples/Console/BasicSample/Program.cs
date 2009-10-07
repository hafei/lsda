// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicSample
{
    using System;
    using System.Linq;

    using Entities;

    using LogicSoftware.DataAccess.Repository.Basic;
    using LogicSoftware.DataAccess.Repository.LinqToSql;
    using LogicSoftware.DataAccess.Repository.Memory;

    using Mapping;

    /// <summary>
    /// Program entry point class
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// Creates the LINQ to SQL repository.
        /// </summary>
        /// <returns>
        /// The memory repository.
        /// </returns>
        private static IRepository CreateLinqToSqlRepository()
        {
            var connectionString = new ConfigurationConnectionString("BasicSampleDatabase");
            var connectionMananger = new SqlConnectionManager(connectionString);
            var mappingSourceManager = new SampleMappingSourceManager();
            return new LinqToSqlRepository(connectionMananger, mappingSourceManager);
        }

        /// <summary>
        /// Creates the memory repository.
        /// </summary>
        /// <returns>
        /// The memory repository.
        /// </returns>
        private static IRepository CreateMemoryRepository()
        {
            return new MemoryRepository(new SampleMappingSourceManager());
        }

        /// <summary>
        /// Fills the sample data.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        private static void FillSampleData(IRepository repository)
        {
            if (repository.All<Customer>().Count() < 3)
            {
                Console.WriteLine("Inserting some data...");

                for (int i = 0; i < 3; i++)
                {
                    var customer = new Customer { Name = "Customer " + i, Email = i + "@domain.com" };

                    repository.Insert(customer);

                    for (int j = 0; j < i + 2; j++)
                    {
                        var project = new Project
                            {
                                Title = String.Format("Project {0}-{1}", i, j), 
                                Description = "some test for proj #" + (i + j), 
                                CustomerId = customer.CustomerId
                            };

                        repository.Insert(project);

                        for (int k = 0; k < j + 2; k++)
                        {
                            var task = new Task
                                {
                                    Title = String.Format("Task {0}-{1}-{2}", i, j, k), 
                                    Description = "some test for proj #" + (i + j + k), 
                                    ProjectId = project.ProjectId
                                };

                            repository.Insert(task);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mains the specified args ;)
        /// </summary>
        /// <param name="args">
        /// The program args.
        /// </param>
        private static void Main(string[] args)
        {
            Console.WriteLine("Running samples on MemoryRepository.");
            IRepository memoryRepository = CreateMemoryRepository();
            RunSamples(memoryRepository);

            Console.WriteLine("Running samples on LinqToSqlRepository.");
            IRepository sqlRepository = CreateLinqToSqlRepository();
            RunSamples(sqlRepository);
        }

        /// <summary>
        /// Runs the samples.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        private static void RunSamples(IRepository repository)
        {
            // insert some data
            FillSampleData(repository);

            // count
            Console.WriteLine("Count: ");
            Console.WriteLine("Customers: " + repository.All<Customer>().Count());
            Console.WriteLine("Projects: " + repository.All<Project>().Count());
            Console.WriteLine("Tasks: " + repository.All<Task>().Count());

            // simple select
            Console.WriteLine("3-rd Customer: " + repository.All<Customer>().OrderBy(c => c.Name).Skip(2).First());

            // implicit joins
            // note that c.Projects and t.Project.Customer are not actually loaded
            Console.WriteLine(
                "Number of customers with more than 2 projects: " +
                repository.All<Customer>().Where(c => c.Projects.Count > 2).Count());

            Console.WriteLine(
                "Some Customer with 4 Projects: " + repository.All<Customer>().Where(c => c.Projects.Count == 4).First());

            Console.WriteLine(
                "Tasks for Customer #2 count: " +
                repository.All<Task>().Where(t => t.Project.Customer.CustomerId == 2).Count());

            Console.WriteLine(
                "Some Task for Customer #3 count: " +
                repository.All<Task>().Where(t => t.Project.Customer.CustomerId == 3).First());
        }

        #endregion
    }
}