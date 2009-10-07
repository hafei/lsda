// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Task.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicSample.Entities
{
    using System;

    /// <summary>
    /// The task class.
    /// </summary>
    internal class Task
    {
        #region Constants and Fields

        /// <summary>
        /// Project property backing field
        /// </summary>
        private Project project;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public Project Project
        {
            get
            {
                return this.project;
            }

            set
            {
                this.project = value;
                if (this.project != null)
                {
                    this.ProjectId = this.project.ProjectId;
                }
            }
        }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the task id.
        /// </summary>
        /// <value>The task id.</value>
        public int TaskId { get; set; }

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
                "Id = {0}, Name = {1}, Project = #{2}, ({3})", 
                this.TaskId, 
                this.Title, 
                this.ProjectId, 
                this.Project == null ? "not loaded" : "loaded");
        }

        #endregion
    }
}