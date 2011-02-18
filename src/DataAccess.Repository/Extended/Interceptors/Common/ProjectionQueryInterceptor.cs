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
    using System.Diagnostics.CodeAnalysis;
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
        ///   The method instance binding flags.
        /// </summary>
        private const BindingFlags MethodInstanceBindingFlags = MethodStaticBindingFlags | BindingFlags.Instance;

        /// <summary>
        ///   The method static binding flags.
        /// </summary>
        private const BindingFlags MethodStaticBindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        /// <summary>
        ///   The property binding flags.
        /// </summary>
        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        ///   The member types.
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
        ///   The member type handlers.
        /// </summary>
        private readonly Dictionary<ProjectionMemberType, Func<Expression, Type, object, IEnumerable<ProjectionMemberMetadata>, Expression>> MemberTypeHandlers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ProjectionQueryInterceptor" /> class.
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
            ///   The where member.
            /// </summary>
            Where, 

            /// <summary>
            ///   The order member.
            /// </summary>
            Order, 

            /// <summary>
            ///   The skip member.
            /// </summary>
            Skip, 

            /// <summary>
            ///   The take member.
            /// </summary>
            Take, 

            /// <summary>
            ///   The select member.
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
                                           ? ((ConstantExpression)e.MethodCall.Arguments.Last()).Value
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
        /// Gets the projection members.
        /// </summary>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="projectionConfig">
        /// The projection config.
        /// </param>
        /// <returns>
        /// The projection members lookup.
        /// </returns>
        private static ILookup<ProjectionMemberType, ProjectionMemberMetadata> GetProjectionMembers(Type projectionType, object projectionConfig)
        {
            return Enumerable.Empty<MemberInfo>()
                .Concat(projectionType.GetProperties(PropertyBindingFlags)) // public instance properties
                .Concat(projectionType.GetMethods(projectionConfig == null ? MethodStaticBindingFlags : MethodInstanceBindingFlags)) // public static or all methods
                .Where(member => member.IsDefined(typeof(ProjectionMemberAttribute), true)) // note: skipping members without attributes, can be replaced with null check later
                .Select(member => new ProjectionMemberMetadata(member, (ProjectionMemberAttribute)member.GetCustomAttributes(typeof(ProjectionMemberAttribute), true).SingleOrDefault()))
                .ToLookup(memberMetadata => MemberTypes[memberMetadata.MemberAttribute.GetType()]);
        }

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
            // note: hack that allows no parameters in method
            if (method.GetParameters().Length == 0)
            {
                args = new object[0];
            }

            // todo: performance issues (replace with compiled expressions and cache)
            // todo: rewrite to allow overriding (eg. Scope support in inheritor) 
            return method.IsStatic ? method.Invoke(null, args) : method.Invoke(obj, args);
        }

        /// <summary>
        /// Validates the type of the projection.
        /// </summary>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        /// <param name="sourceType">
        /// Type of the source.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified projection type is projection for specified source type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsProjectionFor(Type projectionType, Type sourceType)
        {
            var projectionAttribute = (ProjectionAttribute)projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true).SingleOrDefault();

            return projectionAttribute != null && projectionAttribute.RootType == sourceType;
        }

        /// <summary>
        /// Validates the type of the projection.
        /// </summary>
        /// <param name="sourceType">
        /// Type of the source.
        /// </param>
        /// <param name="projectionType">
        /// Type of the projection.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RootType", Justification = "Spelling is ok here.")]
        private static void ValidateProjectionType(Type sourceType, Type projectionType)
        {
            var projectionAttribute = (ProjectionAttribute)projectionType.GetCustomAttributes(typeof(ProjectionAttribute), true).SingleOrDefault();

            if (projectionAttribute == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has no projection attribute.", projectionType));
            }

            if (projectionAttribute.RootType != sourceType)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} has wrong RootType in projection attribute. Expected: {1}, actual: {2}.", projectionType, sourceType, projectionAttribute.RootType));
            }
        }

        /// <summary>
        /// Applies the object projection.
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
        /// <returns>
        /// New expression with applied projection.
        /// </returns>
        private Expression ApplyObjectProjection(Expression sourceExpression, Type projectionType, object projectionConfig)
        {
            ValidateProjectionType(sourceExpression.Type, projectionType);

            // todo: add cache
            var projectionMembers = GetProjectionMembers(projectionType, projectionConfig);

            // validating sub-projection
            if (projectionMembers.Any(memberGroup => memberGroup.Key != ProjectionMemberType.Select))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} with members other than select cannot be used as sub-object-projection.", projectionType));
            }

            var selectResultExpression = this.GetSelectExpression(sourceExpression, projectionType, projectionConfig, projectionMembers[ProjectionMemberType.Select]);

            // note: generating conditional expression to have nulls in result where original relationship has null too
            var conditionalSelectResultExpression = Expression.Condition(
                sourceExpression.Equal(Expression.Constant(null, sourceExpression.Type)), 
                Expression.Constant(null, projectionType), 
                selectResultExpression);

            return conditionalSelectResultExpression;
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
            // todo: make static
            var firstOrderingMethod = new Dictionary<Type, Func<Expression, LambdaExpression, Expression>>
                {
                    { typeof(OrderByAttribute), ExpressionExtensions.OrderBy }, 
                    { typeof(OrderByDescendingAttribute), ExpressionExtensions.OrderByDescending }, 
                };
            var secondOrderingMethod = new Dictionary<Type, Func<Expression, LambdaExpression, Expression>>
                {
                    { typeof(OrderByAttribute), ExpressionExtensions.ThenBy }, 
                    { typeof(OrderByDescendingAttribute), ExpressionExtensions.ThenByDescending }, 
                };

            return projectionMemberMetadatas
                .OrderBy(memberInfo => memberInfo.MemberAttribute is OrderByAttribute
                                           ? ((OrderByAttribute)memberInfo.MemberAttribute).Order
                                           : ((OrderByDescendingAttribute)memberInfo.MemberAttribute).Order)
                .Select(orderMemberMetadata => new
                    {
                        OrderMemberAttributeType = orderMemberMetadata.MemberAttribute.GetType(), 
                        OrderExpression = (LambdaExpression)InvokeMethod(projectionConfig, (MethodInfo)orderMemberMetadata.Member, this.Scope)
                    })
                .Aggregate(
                    orderMember => firstOrderingMethod[orderMember.OrderMemberAttributeType](sourceSequenceExpression /* note: closure here */, orderMember.OrderExpression), 
                    (expr, orderMember) => secondOrderingMethod[orderMember.OrderMemberAttributeType](expr, orderMember.OrderExpression));
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

            ValidateProjectionType(sourceElementType, projectionType);

            // todo: add cache
            // todo: encapsulate this logic somewhere outside
            var resultSequenceExpression = GetProjectionMembers(projectionType, projectionConfig)
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
                var skipMethod = (MethodInfo)skipMemberMetadata.Member;
                var skipCount = (int)InvokeMethod(projectionConfig, skipMethod, this.Scope);

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
                var takeMethod = (MethodInfo)takeMemberMetadata.Member;
                var takeCount = (int)InvokeMethod(projectionConfig, takeMethod, this.Scope);

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
                var whereMethod = (MethodInfo)whereMemberMetadata.Member;
                var whereExpression = (LambdaExpression)InvokeMethod(projectionConfig, whereMethod, this.Scope);

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
            // todo: performance issues
            var resultMemberBindings = new List<MemberBinding>();

            foreach (var selectMemberMetadata in projectionMemberMetadatas)
            {
                var resultProperty = (PropertyInfo)selectMemberMetadata.Member;

                // for SelectExpression simply bind property to expression
                var expressionAttribute = selectMemberMetadata.MemberAttribute as SelectExpressionAttribute;
                if (expressionAttribute != null)
                {
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
                    var customBindingExpression = (LambdaExpression)InvokeMethod(projectionConfig, expressionMethod, this.Scope);

                    // localize expression (replace its parameter with local sourceExpression)
                    var localizedCustomBindingExpression = new ExpressionParameterReplacer(
                        customBindingExpression.Parameters.Single(), 
                        sourceExpression)
                        .Visit(customBindingExpression.Body);

                    // fixing up collection type if needed
                    localizedCustomBindingExpression = localizedCustomBindingExpression.FixupCollectionType(resultProperty.PropertyType);

                    resultMemberBindings.Add(Expression.Bind(resultProperty, localizedCustomBindingExpression));
                }

                // for SelectProperty there may be variants
                var propertyAttribute = selectMemberMetadata.MemberAttribute as SelectPropertyAttribute;
                if (propertyAttribute != null)
                {
                    var sourcePropertyPath = propertyAttribute.Path ?? resultProperty.Name;
                    var sourcePropertyExpression = (Expression)sourceExpression.PropertyPath(sourcePropertyPath);

                    var sourcePropertyElementType = TypeSystem.GetElementType(sourcePropertyExpression.Type);
                    var resultPropertyElementType = TypeSystem.GetElementType(resultProperty.PropertyType);

                    // note: trying to do as much as it is possible
                    // todo: maybe do less? eg. no blind converts
                    if (sourcePropertyExpression.Type == resultProperty.PropertyType)
                    {
                        // 1. simple bind if types are equal
                        resultMemberBindings.Add(Expression.Bind(resultProperty, sourcePropertyExpression));
                    }
                    else if (resultProperty.PropertyType.IsAssignableFrom(sourcePropertyExpression.Type))
                    {
                        // 2. if types are assignable then assign with cast (which is crucial for eg. lift to null)
                        // this will cover lift to nulls (int -> int?), List<int> -> IEnumerable<int>, and some other situations
                        // but will not cover eg. some element types conversions (eg. int -> decimal)
                        var resultPropertyBindingExpression = Expression.Convert(sourcePropertyExpression, resultProperty.PropertyType);

                        resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                    }
                    else if (((sourcePropertyElementType == null) && (resultPropertyElementType == null))
                             && (sourcePropertyExpression.Type != resultProperty.PropertyType))
                    {
                        // 4-5. if both properties are not sequence types and property types are differ then it is probably an object projection or simple convert
                        if (IsProjectionFor(resultProperty.PropertyType, sourcePropertyExpression.Type))
                        {
                            // 4. object projection    
                            var resultPropertyBindingExpression = this.ApplyObjectProjection(sourcePropertyExpression, resultProperty.PropertyType, null /* note: no projectionConfig here */);

                            resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                        }
                        else
                        {
                            // 5. simple convert
                            var resultPropertyBindingExpression = Expression.Convert(sourcePropertyExpression, resultProperty.PropertyType);

                            resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                        }
                    }
                    else if (((sourcePropertyElementType != null) && (resultPropertyElementType != null))
                             && (sourcePropertyElementType != resultPropertyElementType))
                    {
                        // 6-7. if both properties are sequence types and element types are differ then it is probably a sequence projection or sequence convert
                        if (IsProjectionFor(resultPropertyElementType, sourcePropertyElementType))
                        {
                            // 6. sequence projection
                            var resultPropertyBindingExpression = this.ApplySequenceProjection(sourcePropertyExpression, resultPropertyElementType, null /* note: no projectionConfig here */)
                                .FixupCollectionType(resultProperty.PropertyType);

                            resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                        }
                        else
                        {
                            // 7. sequence convert: sequence.Select(element => (type) element)
                            var sourceElementParameter = Expression.Parameter(sourcePropertyElementType, sourcePropertyElementType.Name);
                            var sourceElementConvertedExpression = Expression.Convert(sourceElementParameter, resultPropertyElementType);

                            var resultPropertyBindingExpression = sourcePropertyExpression.Select(sourceElementParameter.ToLambda(sourceElementConvertedExpression))
                                .FixupCollectionType(resultProperty.PropertyType);

                            resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Unknown binding of projection property. Projection {0}, property {1}.", projectionType, selectMemberMetadata.Member));
                    }
                }
            }

            var resultInitExpression = Expression.MemberInit(Expression.New(projectionType), resultMemberBindings);

            return resultInitExpression;
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
            ///   Gets the member.
            /// </summary>
            /// <value>The member.</value>
            public MemberInfo Member { get; private set; }

            /// <summary>
            ///   Gets the member attribute.
            /// </summary>
            /// <value>The member attribute.</value>
            public ProjectionMemberAttribute MemberAttribute { get; private set; }

            #endregion
        }

        #endregion
    }
}