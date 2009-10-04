// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionParameterReplacer.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Class used to replace parameters in a expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Linq
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Class used to replace parameters in a expression.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Replacer", Justification = "Don't see any misspelling there. Do you?")]
    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParameterReplacer"/> class for replacement of single parameter.
        /// </summary>
        /// <param name="parameterToSearch">
        /// Parameter to be replaced.
        /// </param>
        /// <param name="expressionToReplace">
        /// Expression that replaces.
        /// </param>
        public ExpressionParameterReplacer(ParameterExpression parameterToSearch, Expression expressionToReplace)
        {
            this.ReplacementDictionary = new Dictionary<ParameterExpression, Expression> { { parameterToSearch, expressionToReplace } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParameterReplacer"/> class for replacement of multiple parameters.
        /// </summary>
        /// <param name="replacementDictionary">
        /// Dictionary of replacement pairs (key is parameter to be replaced, value is expression that replaces).
        /// </param>
        public ExpressionParameterReplacer(Dictionary<ParameterExpression, Expression> replacementDictionary)
        {
            this.ReplacementDictionary = replacementDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets replacement pairs list.
        /// </summary>
        private Dictionary<ParameterExpression, Expression> ReplacementDictionary { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the parameter expression provided as parameter and
        /// returns an appropiated parameter expression.
        /// </summary>
        /// <param name="parameter">
        /// The parameter expression to analyze.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            Expression replacement;
            return this.ReplacementDictionary.TryGetValue(parameter, out replacement)
                       ? replacement
                       : base.VisitParameter(parameter);
        }

        #endregion
    }
}