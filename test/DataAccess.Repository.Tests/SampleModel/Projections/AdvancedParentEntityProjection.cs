// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedParentEntityProjection.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The advanced parent entity projection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Extended.Interceptors.Common.Attributes;

    /// <summary>
    /// The advanced parent entity projection.
    /// </summary>
    [Projection(typeof(SampleParentEntity))]
    public class AdvancedParentEntityProjection
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedParentEntityProjection"/> class.
        /// </summary>
        public AdvancedParentEntityProjection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedParentEntityProjection"/> class.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="take">
        /// The take arg.
        /// </param>
        private AdvancedParentEntityProjection(string pattern, int take)
        {
            this.Pattern = pattern;
            this.Take = take;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        [SelectProperty]
        public List<ChildProjection> Children { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id of the entity.</value>
        [SelectProperty]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the entity.</value>
        [SelectProperty]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the super parent.
        /// </summary>
        /// <value>The super parent.</value>
        [SelectProperty]
        public SuperParentProjection SuperParent { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        private string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the take.
        /// </summary>
        /// <value>The take arg.</value>
        private int Take { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the number of items to skip.
        /// </summary>
        /// <returns>
        /// The number of items to skip.
        /// </returns>
        [Skip]
        public static int ItemsToSkip()
        {
            return 1;
        }

        /// <summary>
        /// Returns the ordering by name.
        /// </summary>
        /// <returns>
        /// The ordering by name.
        /// </returns>
        [OrderByDescending(0)]
        public static Expression<Func<SampleParentEntity, string>> OrderByNameDescending()
        {
            return e => e.Name;
        }

        /// <summary>
        /// Returns the projection config with the pattern and take.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="take">
        /// The take arg.
        /// </param>
        /// <returns>
        /// The projection config with the pattern and take.
        /// </returns>
        public static AdvancedParentEntityProjection WithPatternAndTake(string pattern, int take)
        {
            return new AdvancedParentEntityProjection(pattern, take);
        }

        /// <summary>
        /// Returns the number of items to take.
        /// </summary>
        /// <returns>
        /// The number of items to take.
        /// </returns>
        [Take]
        public int ItemsToTake()
        {
            return this.Take;
        }

        /// <summary>
        /// Returns the name contains filter.
        /// </summary>
        /// <returns>
        /// The name contains filter.
        /// </returns>
        [Where]
        public Expression<Func<SampleParentEntity, bool>> NameContainsPattern()
        {
            return e => e.Name.Contains(this.Pattern);
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// The child projection.
        /// </summary>
        [Projection(typeof(SampleChildEntity))]
        public class ChildProjection
        {
            #region Properties

            /// <summary>
            /// Gets or sets the id of the entity.
            /// </summary>
            /// <value>The id of the entity.</value>
            [SelectProperty]
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the entity.
            /// </summary>
            /// <value>The name of the entity.</value>
            [SelectProperty]
            public string Name { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// Returns the ordering by name.
            /// </summary>
            /// <returns>
            /// The ordering by name.
            /// </returns>
            [OrderByDescending(0)]
            public static Expression<Func<SampleChildEntity, string>> OrderByNameDescending()
            {
                return e => e.Name;
            }

            #endregion
        }

        /// <summary>
        /// The super parent projection.
        /// </summary>
        [Projection(typeof(SampleSuperParentEntity))]
        public class SuperParentProjection
        {
            #region Properties

            /// <summary>
            /// Gets or sets the id of the entity.
            /// </summary>
            /// <value>The id of the entity.</value>
            [SelectProperty]
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the entity.
            /// </summary>
            /// <value>The name of the entity.</value>
            [SelectProperty]
            public string Name { get; set; }

            #endregion
        }

        #endregion
    }
}