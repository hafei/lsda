// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionConstantReplacer.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Class used to replace constants in a expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Linq
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Class used to replace constants in a expression.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Replacer", Justification = "No spelling error here.")]
    public class ExpressionConstantReplacer : ExpressionVisitor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionConstantReplacer"/> class for replacement of single constant. 
        /// </summary>
        /// <param name="constantToSearch">
        /// Constant to be replaced.
        /// </param>
        /// <param name="expressionToReplace">
        /// Expression that replaces.
        /// </param>
        public ExpressionConstantReplacer(object constantToSearch, Expression expressionToReplace)
        {
            this.ReplacementDictionary = new Dictionary<object, Expression> { { constantToSearch, expressionToReplace } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionConstantReplacer"/> class for replacement of single constant. 
        /// </summary>
        /// <param name="constantToSearch">
        /// Constant to be replaced.
        /// </param>
        /// <param name="constantToReplace">
        /// Constant that replaces.
        /// </param>
        public ExpressionConstantReplacer(object constantToSearch, object constantToReplace)
        {
            this.ReplacementDictionary = new Dictionary<object, Expression> { { constantToSearch, Expression.Constant(constantToReplace) } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionConstantReplacer"/> class for replacement of multiple constants. 
        /// </summary>
        /// <param name="replacementDictionary">
        /// Dictionary of replacement pairs (key is constant to be replaced, value is expression that replaces).
        /// </param>
        public ExpressionConstantReplacer(Dictionary<object, Expression> replacementDictionary)
        {
            this.ReplacementDictionary = replacementDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets replacement pairs list.
        /// </summary>
        private Dictionary<object, Expression> ReplacementDictionary { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the constant expression provided as parameter and
        /// returns an appropiated constant expression.
        /// </summary>
        /// <param name="constant">
        /// The constant expression to analyze.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            Expression replacement;
            return this.ReplacementDictionary.TryGetValue(constant.Value, out replacement)
                       ? replacement
                       : base.VisitConstant(constant);
        }

        #endregion
    }
}