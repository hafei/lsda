// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationConnectionString.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System.Configuration;

    /// <summary>
    /// Connection String in configuration file
    /// </summary>
    public class ConfigurationConnectionString : IConnectionString
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationConnectionString"/> class.
        /// </summary>
        /// <param name="name">
        /// The configuration element name.
        /// </param>
        public ConfigurationConnectionString(string name)
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        #endregion

        #region Implemented Interfaces (Properties)

        #region IConnectionString properties

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        #endregion

        #endregion
    }
}