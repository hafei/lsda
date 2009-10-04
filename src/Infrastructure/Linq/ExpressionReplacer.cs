// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionReplacer.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Class used to replace expressions in a expression tree.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Linq
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Class used to replace expressions in a expression tree.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Replacer", Justification = "No spelling error here.")]
    public class ExpressionReplacer : ExpressionVisitor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionReplacer"/> class for replacement of single expression. 
        /// </summary>
        /// <param name="expressionToSearch">
        /// Expression to be replaced.
        /// </param>
        /// <param name="expressionToReplace">
        /// Expression that replaces.
        /// </param>
        public ExpressionReplacer(Expression expressionToSearch, Expression expressionToReplace)
        {
            this.ReplacementDictionary = new Dictionary<Expression, Expression> { { expressionToSearch, expressionToReplace } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionReplacer"/> class for replacement of multiple constants. 
        /// </summary>
        /// <param name="replacementDictionary">
        /// Dictionary of replacement pairs (key is expression to be replaced, value is expression that replaces).
        /// </param>
        public ExpressionReplacer(Dictionary<Expression, Expression> replacementDictionary)
        {
            this.ReplacementDictionary = replacementDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets replacement pairs list.
        /// </summary>
        private Dictionary<Expression, Expression> ReplacementDictionary { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes the expression and returns it converted to the
        /// appropiated type.
        /// </summary>
        /// <param name="exp">
        /// The expression to be analyzed.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression of the real expression type.
        /// </returns>
        public override Expression Visit(Expression exp)
        {
            Expression replacement;
            return (exp != null) && this.ReplacementDictionary.TryGetValue(exp, out replacement)
                       ? replacement
                       : base.Visit(exp);
        }

        #endregion
    }
}