// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadOptions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The load options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Basic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// The load options.
    /// </summary>
    public class LoadOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadOptions"/> class.
        /// </summary>
        public LoadOptions()
        {
            this.LoadWithOptions = new Collection<LoadWithOption>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return this.LoadWithOptions.Count == 0;
            }
        }

        /// <summary>
        /// Gets the load with options.
        /// </summary>
        /// <value>The load with options.</value>
        public Collection<LoadWithOption> LoadWithOptions { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the with.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="member">
        /// The member.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "To allow cool syntax construct like LoadWith<Task>(t => t.Project), but keep type safety")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "To allow cool syntax construct like LoadWith<Task>(t => t.Project), but keep type safety")]
        public void LoadWith<T>(Expression<Func<T, object>> member)
        {
            this.LoadWithOptions.Add(new LoadWithOption(member));
        }

        /// <summary>
        /// Loads the with.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        public void LoadWith(LambdaExpression member)
        {
            this.LoadWithOptions.Add(new LoadWithOption(member));
        }

        #endregion
    }
}