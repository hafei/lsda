// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExpressionVisitor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Base for the expression tree visitor pattern implementation.
//   ExpressionVisitor is part of System.Query namespace, but is marked as internal,
//   so this implementation was extracted using Reflector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq.Expressions;

    /// <summary>
    /// Base for the expression tree visitor pattern implementation.
    /// ExpressionVisitor is part of System.Query namespace, but is marked as internal,
    /// so this implementation was extracted using Reflector.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of the context.
    /// </typeparam>
    /// <remarks>
    /// Context-aware modification.
    /// </remarks>
    public abstract class ContextExpressionVisitor<TContext>
    {
        #region Public Methods

        /// <summary>
        /// Analyzes the expression and returns it converted to the
        /// appropiated type.
        /// </summary>
        /// <param name="exp">
        /// The expression to be analyzed.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression of the real expression type.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
        public virtual Expression Visit(Expression exp, TContext context)
        {
            if (exp == null)
            {
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.VisitBinary((BinaryExpression) exp, context);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression) exp, context);

                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression) exp, context);

                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression) exp, context);

                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression) exp, context);

                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression) exp, context);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression) exp, context);

                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression) exp, context);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression) exp, context);

                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression) exp, context);

                case ExpressionType.New:
                    return this.VisitNew((NewExpression) exp, context);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression) exp, context);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression) exp, context);

                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression) exp, context);

                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the binary expression provided as parameter and
        /// returns an appropiated binary expression.
        /// </summary>
        /// <param name="binary">
        /// The binary expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitBinary(BinaryExpression binary, TContext context)
        {
            if (binary == null)
            {
                throw new ArgumentNullException("binary");
            }

            Expression left = this.Visit(binary.Left, context);
            Expression right = this.Visit(binary.Right, context);
            Expression conversion = this.Visit(binary.Conversion, context);

            if (left == binary.Left && right == binary.Right && conversion == binary.Conversion)
            {
                return binary;
            }

            if (binary.NodeType == ExpressionType.Coalesce && binary.Conversion != null)
            {
                return Expression.Coalesce(left, right, conversion as LambdaExpression);
            }

            return Expression.MakeBinary(binary.NodeType, left, right, binary.IsLiftedToNull, binary.Method);
        }

        /// <summary>
        /// Analyzes the member binding provided as parameter and calls
        /// the appropiated visitor according to the definition type.
        /// </summary>
        /// <param name="binding">
        /// The binding to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.MemberBinding.
        /// </returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding, TContext context)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment) binding, context);

                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding) binding, context);

                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding) binding, context);

                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// Analyzes the binding expression list provided as parameter and returns
        /// the analyzed expressions.
        /// </summary>
        /// <param name="original">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original, TContext context)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            List<MemberBinding> list = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                MemberBinding item = this.VisitBinding(original[num], context);

                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<MemberBinding>(capacity);

                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }

                    list.Add(item);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        /// <summary>
        /// Analyzes the conditional expression provided as parameter and
        /// returns an appropiated binary expression.
        /// </summary>
        /// <param name="conditional">
        /// The conditional expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitConditional(ConditionalExpression conditional, TContext context)
        {
            if (conditional == null)
            {
                throw new ArgumentNullException("conditional");
            }

            Expression test = this.Visit(conditional.Test, context);
            Expression ifTrue = this.Visit(conditional.IfTrue, context);
            Expression ifFalse = this.Visit(conditional.IfFalse, context);

            if (test == conditional.Test && ifTrue == conditional.IfTrue && ifFalse == conditional.IfFalse)
            {
                return conditional;
            }

            return Expression.Condition(test, ifTrue, ifFalse);
        }

        /// <summary>
        /// Analyzes the constant expression provided as parameter and
        /// returns an appropiated constant expression.
        /// </summary>
        /// <param name="constant">
        /// The constant expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitConstant(ConstantExpression constant, TContext context)
        {
            return constant;
        }

        /// <summary>
        /// Analyzes the element initializer provided as parameter and calls
        /// the appropiated visitor according to the definition type.
        /// </summary>
        /// <param name="initializer">
        /// The initializer to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.ElementInit.
        /// </returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer, TContext context)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments, context);

            if (arguments == initializer.Arguments)
            {
                return initializer;
            }

            return Expression.ElementInit(initializer.AddMethod, arguments);
        }

        /// <summary>
        /// Analyzes element initializer list expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="original">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original, TContext context)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            List<ElementInit> list = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                ElementInit item = this.VisitElementInitializer(original[num], context);

                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<ElementInit>(capacity);

                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }

                    list.Add(item);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        /// <summary>
        /// Analyzes many expressions provided as parameter and returns
        /// the collection of the analyzed expressions.
        /// </summary>
        /// <param name="original">
        /// The expressions to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original, TContext context)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            List<Expression> sequence = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                Expression item = this.Visit(original[num], context);

                if (sequence != null)
                {
                    sequence.Add(item);
                }
                else if (item != original[num])
                {
                    sequence = new List<Expression>(capacity);

                    for (int i = 0; i < num; i++)
                    {
                        sequence.Add(original[i]);
                    }

                    sequence.Add(item);
                }
            }

            if (sequence != null)
            {
                return sequence.AsReadOnly();
            }

            return original;
        }

        /// <summary>
        /// Analyzes invocation expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="iv">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv, TContext context)
        {
            if (iv == null)
            {
                throw new ArgumentNullException("iv");
            }

            IEnumerable<Expression> arguments = this.VisitExpressionList(iv.Arguments, context);
            Expression expression = this.Visit(iv.Expression, context);

            if (arguments == iv.Arguments && expression == iv.Expression)
            {
                return iv;
            }

            return Expression.Invoke(expression, arguments);
        }

        /// <summary>
        /// Analyzes labda expression expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="lambda">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda, TContext context)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException("lambda");
            }

            Expression body = this.Visit(lambda.Body, context);

            if (body == lambda.Body)
            {
                return lambda;
            }

            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
        }

        /// <summary>
        /// Analyzes list initialization expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="init">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitListInit(ListInitExpression init, TContext context)
        {
            if (init == null)
            {
                throw new ArgumentNullException("init");
            }

            NewExpression newExpression = this.VisitNew(init.NewExpression, context);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers, context);

            if (newExpression == init.NewExpression && initializers == init.Initializers)
            {
                return init;
            }

            return Expression.ListInit(newExpression, initializers);
        }

        /// <summary>
        /// Analyzes the member access expression provided as parameter and
        /// returns an appropiated member access.
        /// </summary>
        /// <param name="member">
        /// The member access to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMemberAccess(MemberExpression member, TContext context)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            Expression expression = this.Visit(member.Expression, context);

            if (expression == member.Expression)
            {
                return member;
            }

            return Expression.MakeMemberAccess(expression, member.Member);
        }

        /// <summary>
        /// Analyzes member assignment expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="assignment">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment, TContext context)
        {
            if (assignment == null)
            {
                throw new ArgumentNullException("assignment");
            }

            Expression expression = this.Visit(assignment.Expression, context);

            if (expression == assignment.Expression)
            {
                return assignment;
            }

            return Expression.Bind(assignment.Member, expression);
        }

        /// <summary>
        /// Analyzes member initialization expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="init">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init, TContext context)
        {
            if (init == null)
            {
                throw new ArgumentNullException("init");
            }

            NewExpression newExpression = this.VisitNew(init.NewExpression, context);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings, context);

            if (newExpression == init.NewExpression && bindings == init.Bindings)
            {
                return init;
            }

            return Expression.MemberInit(newExpression, bindings);
        }

        /// <summary>
        /// Analyzes member binding expressions provided as parameter and returns
        /// the analyzed expressions.
        /// </summary>
        /// <param name="binding">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding, TContext context)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers, context);

            if (initializers == binding.Initializers)
            {
                return binding;
            }

            return Expression.ListBind(binding.Member, initializers);
        }

        /// <summary>
        /// Analyzes member member binding expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="binding">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding, TContext context)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings, context);

            if (bindings == binding.Bindings)
            {
                return binding;
            }

            return Expression.MemberBind(binding.Member, bindings);
        }

        /// <summary>
        /// Analyzes the method call expression provided as parameter and
        /// returns an appropiated member access.
        /// </summary>
        /// <param name="methodCall">
        /// The method call to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression methodCall, TContext context)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException("methodCall");
            }

            Expression instance = this.Visit(methodCall.Object, context);
            IEnumerable<Expression> arguments = this.VisitExpressionList(methodCall.Arguments, context);

            if (instance == methodCall.Object && arguments == methodCall.Arguments)
            {
                return methodCall;
            }

            return Expression.Call(instance, methodCall.Method, arguments);
        }

        /// <summary>
        /// Analyzes new expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="newExpression">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "By design.")]
        protected virtual NewExpression VisitNew(NewExpression newExpression, TContext context)
        {
            if (newExpression == null)
            {
                throw new ArgumentNullException("newExpression");
            }

            IEnumerable<Expression> arguments = this.VisitExpressionList(newExpression.Arguments, context);

            if (arguments == newExpression.Arguments)
            {
                return newExpression;
            }

            if (newExpression.Members != null)
            {
                return Expression.New(newExpression.Constructor, arguments, newExpression.Members);
            }

            return Expression.New(newExpression.Constructor, arguments);
        }

        /// <summary>
        /// Analyzes new array expression provided as parameter and returns
        /// the analyzed expression.
        /// </summary>
        /// <param name="na">
        /// The expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na, TContext context)
        {
            if (na == null)
            {
                throw new ArgumentNullException("na");
            }

            IEnumerable<Expression> initializers = this.VisitExpressionList(na.Expressions, context);

            if (initializers == na.Expressions)
            {
                return na;
            }

            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(na.Type.GetElementType(), initializers);
            }

            return Expression.NewArrayBounds(na.Type.GetElementType(), initializers);
        }

        /// <summary>
        /// Analyzes the parameter expression provided as parameter and
        /// returns an appropiated parameter expression.
        /// </summary>
        /// <param name="parameter">
        /// The parameter expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitParameter(ParameterExpression parameter, TContext context)
        {
            return parameter;
        }

        /// <summary>
        /// Analyzes the type binary expression provided as parameter and
        /// returns an appropiated type binary expression.
        /// </summary>
        /// <param name="typeBinary">
        /// The type binary expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression typeBinary, TContext context)
        {
            if (typeBinary == null)
            {
                throw new ArgumentNullException("typeBinary");
            }

            Expression expression = this.Visit(typeBinary.Expression, context);

            if (expression == typeBinary.Expression)
            {
                return typeBinary;
            }

            return Expression.TypeIs(expression, typeBinary.TypeOperand);
        }

        /// <summary>
        /// Analyzes the unary expression provided as parameter and
        /// returns an appropiated unariy expression.
        /// </summary>
        /// <param name="unary">
        /// The unary expression to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitUnary(UnaryExpression unary, TContext context)
        {
            if (unary == null)
            {
                throw new ArgumentNullException("unary");
            }

            Expression operand = this.Visit(unary.Operand, context);

            if (operand == unary.Operand)
            {
                return unary;
            }

            return Expression.MakeUnary(unary.NodeType, operand, unary.Type, unary.Method);
        }

        #endregion
    }
}