// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlProvider.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sql provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Extensions
{
    using System;
    using System.Data.Linq;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The sql provider.
    /// </summary>
    public class SqlProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The SqlProvider type.
        /// </summary>
        public static readonly Type SqlProviderType;

        /// <summary>
        /// The Compile method.
        /// </summary>
        private static readonly MethodInfo CompileMethod;

        /// <summary>
        /// The IProvider interface.
        /// </summary>
        private static readonly Type IProviderInterface;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="SqlProvider"/> class.
        /// </summary>
        static SqlProvider()
        {
            SqlProviderType = typeof(DataContext).Assembly.GetTypes().Where(t => t.Name == "SqlProvider" && t.IsClass).Single();
            IProviderInterface = typeof(DataContext).Assembly.GetTypes().Where(t => t.Name == "IProvider" && t.IsInterface).Single();

            CompileMethod = IProviderInterface.GetMethod("Compile");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProvider"/> class.
        /// </summary>
        /// <param name="internalValue">
        /// The internal value.
        /// </param>
        public SqlProvider(object internalValue)
        {
            if (!SqlProviderType.IsAssignableFrom(internalValue.GetType()))
            {
                throw new ArgumentException("Wrong object provided.");
            }

            this.InternalValue = internalValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets InternalValue.
        /// </summary>
        private object InternalValue { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// Compiled query.
        /// </returns>
        public CompiledQuery Compile(Expression expression)
        {
            var compiledQuery = CompileMethod.Invoke(this.InternalValue, new[] { expression });

            return new CompiledQuery(compiledQuery);
        }

        #endregion
    }
}