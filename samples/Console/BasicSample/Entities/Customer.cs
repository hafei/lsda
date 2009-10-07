// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Customer.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicSample.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The customer class.
    /// </summary>
    internal class Customer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        /// <value>The customer id.</value>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The customer name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        /// <value>The projects.</value>
        public List<Project> Projects { get; set; }

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
                "Id = {0}, Name = {1}, Projects = {2}", 
                this.CustomerId, 
                this.Name, 
                this.Projects == null ? "not loaded" : this.Projects.Count.ToString());
        }

        #endregion
    }
}