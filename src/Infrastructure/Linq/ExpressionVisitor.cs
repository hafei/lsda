// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionVisitor.cs" company="Logic Software">
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
    public abstract class ExpressionVisitor
    {
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
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
        public virtual Expression Visit(Expression exp)
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
                    return this.VisitBinary((BinaryExpression) exp);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression) exp);

                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression) exp);

                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression) exp);

                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression) exp);

                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression) exp);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression) exp);

                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression) exp);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression) exp);

                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression) exp);

                case ExpressionType.New:
                    return this.VisitNew((NewExpression) exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression) exp);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression) exp);

                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression) exp);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitBinary(BinaryExpression binary)
        {
            Expression left = this.Visit(binary.Left);
            Expression right = this.Visit(binary.Right);
            Expression conversion = this.Visit(binary.Conversion);

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
        /// <returns>
        /// A System.Linq.Expressions.MemberBinding.
        /// </returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment) binding);

                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding) binding);

                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding) binding);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                MemberBinding item = this.VisitBinding(original[num]);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitConditional(ConditionalExpression conditional)
        {
            Expression test = this.Visit(conditional.Test);
            Expression ifTrue = this.Visit(conditional.IfTrue);
            Expression ifFalse = this.Visit(conditional.IfFalse);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitConstant(ConstantExpression constant)
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
        /// <returns>
        /// A System.Linq.Expressions.ElementInit.
        /// </returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                ElementInit item = this.VisitElementInitializer(original[num]);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> sequence = null;

            for (int num = 0, capacity = original.Count; num < capacity; num++)
            {
                Expression item = this.Visit(original[num]);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> arguments = this.VisitExpressionList(iv.Arguments);
            Expression expression = this.Visit(iv.Expression);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression newExpression = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMemberAccess(MemberExpression member)
        {
            Expression expression = this.Visit(member.Expression);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression expression = this.Visit(assignment.Expression);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression newExpression = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            Expression instance = this.Visit(methodCall.Object);
            IEnumerable<Expression> arguments = this.VisitExpressionList(methodCall.Arguments);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "By design.")]
        protected virtual NewExpression VisitNew(NewExpression newExpression)
        {
            IEnumerable<Expression> arguments = this.VisitExpressionList(newExpression.Arguments);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> initializers = this.VisitExpressionList(na.Expressions);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitParameter(ParameterExpression parameter)
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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression typeBinary)
        {
            Expression expression = this.Visit(typeBinary.Expression);

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
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected virtual Expression VisitUnary(UnaryExpression unary)
        {
            Expression operand = this.Visit(unary.Operand);

            if (operand == unary.Operand)
            {
                return unary;
            }

            return Expression.MakeUnary(unary.NodeType, operand, unary.Type, unary.Method);
        }

        #endregion
    }
}