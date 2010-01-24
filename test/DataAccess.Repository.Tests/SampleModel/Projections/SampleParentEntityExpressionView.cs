// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleParentEntityExpressionView.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample parent entity view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Extended.Interceptors.Common.Attributes;

    /// <summary>
    /// The sample parent entity view.
    /// </summary>
    [Projection(typeof(SampleParentEntity))]
    public class SampleParentEntityExpressionView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        [Expression("ChildrenExpression")]
        public List<SampleChildEntity> Children { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the entity.</value>
        [Property("Name")]
        public string Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the Children property expression.
        /// </summary>
        /// <param name="testScope">
        /// The test scope.
        /// </param>
        /// <returns>
        /// The Children property expression
        /// </returns>
        public static Expression<Func<SampleParentEntity, List<SampleChildEntity>>> ChildrenExpression(ITestScope testScope)
        {
            return p => p.Children.Where(c => c.Name.EndsWith("1")).ToList();
        }

        #endregion
    }
}