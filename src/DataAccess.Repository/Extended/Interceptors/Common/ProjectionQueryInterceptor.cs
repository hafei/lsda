// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Typed Projections interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Attributes;

    using Events;

    using Infrastructure.Extensions;
    using Infrastructure.Helpers;
    using Infrastructure.Linq;

    /// <summary>
    /// Typed Projections interceptor.
    /// </summary>
    public class ProjectionQueryInterceptor : QueryInterceptor<IScope>
    {
        #region Constants and Fields

        /// <summary>
        /// The member types.
        /// </summary>
        private static readonly Dictionary<Type, ProjectionMemberType> MemberTypes = new Dictionary<Type, ProjectionMemberType>
            {
                { typeof(WhereAttribute), ProjectionMemberType.Where }, 
                { typeof(OrderByAttribute), ProjectionMemberType.Order }, 
                { typeof(OrderByDescendingAttribute), ProjectionMemberType.Order }, 
                { typeof(SkipAttribute), ProjectionMemberType.Skip }, 
                { typeof(TakeAttribute), ProjectionMemberType.Take }, 
                { typeof(SelectPropertyAttribute), ProjectionMemberType.Select }, 
                { typeof(SelectExpressionAttribute), ProjectionMemberType.Select }
            };

        /// <summary>
        /// The member type handlers.
        /// </summary>
        private readonly Dictionary<ProjectionMemberType, Func<Expression, Type, object, IEnumerable<ProjectionMemberMetadata>, Expression>> MemberTypeHandlers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionQueryInterceptor"/> class.
        /// </summary>
        public ProjectionQueryInterceptor()
        {
            this.MemberTypeHandlers = new Dictionary<ProjectionMemberType, Func<Expression, Type, object, IEnumerable<ProjectionMemberMetadata>, Expression>>
                {
                    { ProjectionMemberType.Where, this.ApplyWhere }, 
                    { ProjectionMemberType.Order, this.ApplyOrder }, 
                    { ProjectionMemberType.Skip, this.ApplySkip }, 
                    { ProjectionMemberType.Take, this.ApplyTake }, 
                    { ProjectionMemberType.Select, this.ApplySelect }
                };
        }

        #endregion

        #region Enums

        /// <summary>
        /// The projection member type.
        /// </summary>
        private enum ProjectionMemberType
        {
            /// <summary>
            /// The where member.
            /// </summary>
            Where, 

            /// <summary>
            /// The order member.
            /// </summary>
            Order, 

            /// <summary>
            /// The skip member.
            /// </summary>
            Skip, 

            /// <summary>
            /// The take member.
            /// </summary>
            Take, 

            /// <summary>
            /// The select member.
            /// </summary>
            Select
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the MethodCallVisit event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.MethodCallVisitEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnMethodCallVisit(MethodCallVisitEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.MethodCall.Method.DeclaringType == typeof(ProjectionExtensions))
            {
                var sourceQueryExpression = e.MethodCall.Arguments.First();
                var projectionConfig = e.MethodCall.Arguments.Count == 2
                                           ? ((ConstantExpression) e.MethodCall.Arguments.Last()).Value
                                           : null;
                var projectionType = projectionConfig != null
                                         ? projectionConfig.GetType()
                                         : TypeSystem.GetElementType(e.MethodCall.Type);

                e.SubstituteExpression = this.ApplySequenceProjection(sourceQueryExpression, projectionType, projectionConfig);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="args">
        /// The argumentss.
        /// </param>
        /// <returns>
        /// Result of method invoking.
        /// </returns>
        private static object InvokeMethod(object obj, MethodInfo method, params object[] args)
        {
            // todo: remove hack that allows no parameters in method
            if (method.GetParameters().Length == 0)
            {
                args = new object[0];
            }

            // todo: performance issues (replace with compiled expressions and cache)
            // todo: rewrite to allow overriding (eg. Scope support in inheritor) 
            return method.IsStatic ? method.Invoke(null, args) : method.Invoke(obj, args);
        }

        /// <summary>
        /// Applies the order.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// New sequence expression with applied order.
        /// </returns>
        private Expression ApplyOrder(Expression sourceSequenceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            foreach (var orderMemberMetadata in projectionMemberMetadatas
                .OrderBy(memberInfo => memberInfo.MemberAttribute is OrderByAttribute
                                           ? ((OrderByAttribute) memberInfo.MemberAttribute).Order
                                           : ((OrderByDescendingAttribute) memberInfo.MemberAttribute).Order))
            {
                var orderMethod = (MethodInfo) orderMemberMetadata.Member;
                var orderExpression = (LambdaExpression) InvokeMethod(projectionConfig, orderMethod, this.Scope);

                if (orderMemberMetadata.MemberAttribute is OrderByAttribute)
                {
                    sourceSequenceExpression = sourceSequenceExpression.OrderBy(orderExpression);
                }
                else if (orderMemberMetadata.MemberAttribute is OrderByDescendingAttribute)
                {
                    sourceSequenceExpression = sourceSequenceExpression.OrderByDescending(orderExpression);
                }
            }

            return sourceSequenceExpression;
        }

        /// <summary>
        /// Applies the select.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// New sequence expression with applied select.
        /// </returns>
        private Expression ApplySelect(Expression sourceSequenceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            var sourceElementType = TypeSystem.GetElementType(sourceSequenceExpression.Type);
            var sourceElementParameter = Expression.Parameter(sourceElementType, "source");

            var selectResultExpression = this.GetSelectExpression(sourceElementParameter, projectionType, projectionConfig, projectionMemberMetadatas);

            var resultSelectorLambda = sourceElementParameter.ToLambda(selectResultExpression);

            return sourceSequenceExpression.Select(resultSelectorLambda);
        }

        /// <summary>
        /// Applies the sequence projection.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <returns>
        /// New sequence expression with applied projection.
        /// </returns>
        private Expression ApplySequenceProjection(Expression sourceSequenceExpression, Type projectionType, object projectionConfig)
        {
            var sourceElementType = TypeSystem.GetElementType(sourceSequenceExpression.Type);

            // checking projection attribute validity (integrity check, may be removed)
            var projectionAttribute = (ProjectionAttribute) projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true).SingleOrDefault();

            if (projectionAttribute == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has no projection attribute.", projectionType));
            }

            if (projectionAttribute.RootType != sourceElementType)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has wrong RootType in projection attribute. Expected: {1}, actual: {2}.", projectionType, sourceElementType, projectionAttribute.RootType));
            }

            // todo: add cache
            var resultSequenceExpression = projectionType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(member => member.IsDefined(typeof(ProjectionMemberAttribute), true)) // note: skipping members without attributes, can be replaced with null check later
                .Select(member => new ProjectionMemberMetadata(member, (ProjectionMemberAttribute) member.GetCustomAttributes(typeof(ProjectionMemberAttribute), true).SingleOrDefault()))
                .GroupBy(memberMetadata => MemberTypes[memberMetadata.MemberAttribute.GetType()])
                .OrderBy(memberGroup => memberGroup.Key) // note: ordering by enum underlying int values, order is significant
                .Aggregate(
                    sourceSequenceExpression, 
                    (previous, memberGroup) => this.MemberTypeHandlers[memberGroup.Key](previous, projectionType, projectionConfig, memberGroup));

            return resultSequenceExpression;
        }

        /// <summary>
        /// Applies the skip.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// New sequence expression with applied skip.
        /// </returns>
        private Expression ApplySkip(Expression sourceSequenceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            var skipMemberMetadata = projectionMemberMetadatas.SingleOrDefault();
            if (skipMemberMetadata != null)
            {
                var skipMethod = (MethodInfo) skipMemberMetadata.Member;
                var skipCount = (int) InvokeMethod(projectionConfig, skipMethod, this.Scope);

                return sourceSequenceExpression.Skip(skipCount);
            }

            return sourceSequenceExpression;
        }

        /// <summary>
        /// The apply take.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// New sequence expression with applied take.
        /// </returns>
        private Expression ApplyTake(Expression sourceSequenceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            var takeMemberMetadata = projectionMemberMetadatas.SingleOrDefault();
            if (takeMemberMetadata != null)
            {
                var takeMethod = (MethodInfo) takeMemberMetadata.Member;
                var takeCount = (int) InvokeMethod(projectionConfig, takeMethod, this.Scope);

                return sourceSequenceExpression.Take(takeCount);
            }

            return sourceSequenceExpression;
        }

        /// <summary>
        /// Applies the where.
        /// </summary>
        /// <param name="sourceSequenceExpression">
        /// The source sequence expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// New sequence expression with applied filter.
        /// </returns>
        private Expression ApplyWhere(Expression sourceSequenceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            foreach (var whereMemberMetadata in projectionMemberMetadatas)
            {
                var whereMethod = (MethodInfo) whereMemberMetadata.Member;
                var whereExpression = (LambdaExpression) InvokeMethod(projectionConfig, whereMethod, this.Scope);

                sourceSequenceExpression = sourceSequenceExpression.Where(whereExpression);
            }

            return sourceSequenceExpression;
        }

        /// <summary>
        /// Gets the select expression.
        /// </summary>
        /// <param name="sourceExpression">
        /// The source expression.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <param name="projectionMemberMetadatas">
        /// The projection member metadatas.
        /// </param>
        /// <returns>
        /// The select expression from source to result via projection.
        /// </returns>
        private Expression GetSelectExpression(Expression sourceExpression, Type projectionType, object projectionConfig, IEnumerable<ProjectionMemberMetadata> projectionMemberMetadatas)
        {
            var resultMemberBindings = new List<MemberBinding>();

            foreach (var selectMemberMetadata in projectionMemberMetadatas)
            {
                var resultProperty = (PropertyInfo) selectMemberMetadata.Member;

                // for SelectExpression simply bind property to expression
                var expressionAttribute = selectMemberMetadata.MemberAttribute as SelectExpressionAttribute;
                if (expressionAttribute != null)
                {
                    // todo: performance issues
                    var declaringType = expressionAttribute.DeclaringType ?? projectionType;
                    var methodName = expressionAttribute.MethodName ?? resultProperty.Name;

                    var expressionMethod = declaringType.GetMethod(
                        methodName, 
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    if (expressionMethod == null)
                    {
                        throw new ArgumentException(String.Format(
                            CultureInfo.InvariantCulture, 
                            "Method specified in SelectExpressionMethod attribute of '{0}' property in '{1}' class is not found.", 
                            resultProperty, 
                            projectionType));
                    }

                    // todo: rewrite (projectionConfig is passed inside even if DeclaringType is another, works because with static methods in another DeclaringTypes projectionConfig is not used)
                    var customBindingExpression = (LambdaExpression) InvokeMethod(projectionConfig, expressionMethod, this.Scope);

                    // localize expression (replace its parameter with local sourceExpression)
                    var localizedCustomBindingExpression = new ExpressionParameterReplacer(
                        customBindingExpression.Parameters.Single(),
                        sourceExpression)
                        .Visit(customBindingExpression.Body);

                    // fixing up collection type if needed
                    localizedCustomBindingExpression = localizedCustomBindingExpression.FixupCollectionType(resultProperty.PropertyType);

                    resultMemberBindings.Add(Expression.Bind(resultProperty, localizedCustomBindingExpression));
                }

                // for SelectProperty there may be variants: 1. simple bind, 2. bind with convert (eg. lift to null)?, 3. bind via sequence projection, 4. bind via object projection
                var propertyAttribute = selectMemberMetadata.MemberAttribute as SelectPropertyAttribute;
                if (propertyAttribute != null)
                {
                    var sourcePropertyPath = propertyAttribute.Path ?? resultProperty.Name;
                    var sourcePropertyExpression = (Expression) sourceExpression.PropertyPath(sourcePropertyPath);

                    var sourcePropertyElementType = TypeSystem.GetElementType(sourcePropertyExpression.Type);
                    var resultPropertyElementType = TypeSystem.GetElementType(resultProperty.PropertyType);

                    // 1. simple bind if types are equal
                    if (sourcePropertyExpression.Type == resultProperty.PropertyType)
                    {
                        resultMemberBindings.Add(Expression.Bind(resultProperty, sourcePropertyExpression));

                        continue;
                    }

                    // todo: 2. bind with convert (eg. lift to null) if needed

                    // 3. bind via sequence projection if both properties are sequence types and element types are differ
                    if (((sourcePropertyElementType != null) && (resultPropertyElementType != null))
                        && (sourcePropertyElementType != resultPropertyElementType))
                    {
                        var resultPropertyBindingExpression = this.ApplySequenceProjection(sourcePropertyExpression, resultPropertyElementType, null /* note: no projectionConfig here */)
                            .FixupCollectionType(resultProperty.PropertyType);

                        resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));

                        continue;
                    }

                    // 4. bind via object projection if both properties are not sequence types and element types are differ
                    if (((sourcePropertyElementType == null) && (resultPropertyElementType == null))
                        && (sourcePropertyExpression.Type != resultProperty.PropertyType))
                    {
                        var resultPropertyBindingExpression = this.ApplyObjectProjection(sourcePropertyExpression, resultProperty.PropertyType, null /* note: no projectionConfig here */);

                        resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                    }
                }
            }

            var resultInitExpression = Expression.MemberInit(Expression.New(projectionType), resultMemberBindings);

            return resultInitExpression;
        }

        /// <summary>
        /// Applies the object projection.
        /// </summary>
        /// <param name="sourceExpression">The source expression.</param>
        /// <param name="projectionType">Type of the projection.</param>
        /// <param name="projectionConfig">The projection config.</param>
        /// <returns>New expression with applied projection.</returns>
        private Expression ApplyObjectProjection(Expression sourceExpression, Type projectionType, object projectionConfig)
        {
            // checking projection attribute validity (integrity check, may be removed)
            var projectionAttribute = (ProjectionAttribute) projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true).SingleOrDefault();

            if (projectionAttribute == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has no projection attribute.", projectionType));
            }

            if (projectionAttribute.RootType != sourceExpression.Type)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has wrong RootType in projection attribute. Expected: {1}, actual: {2}.", projectionType, sourceExpression.Type, projectionAttribute.RootType));
            }

            // todo: add cache
            var projectionMembers = projectionType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(member => member.IsDefined(typeof(ProjectionMemberAttribute), true)) // note: skipping members without attributes, can be replaced with null check later
                .Select(member => new ProjectionMemberMetadata(member, (ProjectionMemberAttribute) member.GetCustomAttributes(typeof(ProjectionMemberAttribute), true).SingleOrDefault()))
                .ToLookup(memberMetadata => MemberTypes[memberMetadata.MemberAttribute.GetType()]);

            // validating sub-projection
            if (projectionMembers.Any(memberGroup => memberGroup.Key != ProjectionMemberType.Select))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} with members other than select cannot be used as sub-object-projection.", projectionType));
            }

            var selectResultExpression = this.GetSelectExpression(sourceExpression, projectionType, projectionConfig, projectionMembers[ProjectionMemberType.Select]);

            return selectResultExpression;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// The projection member metadata.
        /// </summary>
        private class ProjectionMemberMetadata
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ProjectionMemberMetadata"/> class.
            /// </summary>
            /// <param name="member">
            /// The member.
            /// </param>
            /// <param name="memberAttribute">
            /// The member attribute.
            /// </param>
            public ProjectionMemberMetadata(MemberInfo member, ProjectionMemberAttribute memberAttribute)
            {
                this.Member = member;
                this.MemberAttribute = memberAttribute;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the member.
            /// </summary>
            /// <value>The member.</value>
            public MemberInfo Member { get; private set; }

            /// <summary>
            /// Gets the member attribute.
            /// </summary>
            /// <value>The member attribute.</value>
            public ProjectionMemberAttribute MemberAttribute { get; private set; }

            #endregion
        }

        #endregion
    }
}