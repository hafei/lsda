namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    internal class ClassB
    {
        public int Id { get; set; }

        public int ClassAId { get; set; }

        public ClassA ClassA { get; set; }

        public string Name { get; set; }
    }
}