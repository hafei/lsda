// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The project class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicSample.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The project class.
    /// </summary>
    internal class Project
    {
        #region Constants and Fields

        /// <summary>
        /// Customer property backing field
        /// </summary>
        private Customer customer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>The customer.</value>
        public Customer Customer
        {
            get
            {
                return this.customer;
            }

            set
            {
                this.customer = value;
                if (this.customer != null)
                {
                    this.CustomerId = this.customer.CustomerId;
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        /// <value>The customer id.</value>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        public List<Task> Tasks { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(
                "Id = {0}, Title = {1}, Customer = #{2}, ({3}), Tasks = {4}", 
                this.ProjectId, 
                this.Title, 
                this.ProjectId, 
                this.Customer == null ? "not loaded" : "loaded", 
                this.Tasks == null ? "not loaded" : this.Tasks.Count.ToString());
        }

        #endregion
    }
}