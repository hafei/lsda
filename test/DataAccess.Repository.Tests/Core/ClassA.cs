namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using System.Collections.Generic;

    internal class ClassA
    {
        #region Properties

        public List<ClassB> ClassBs { get; set; }

        public int Id { get; set; }

        #endregion
    }
}