// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensionsProvider.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The repository extensions provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using LogicSoftware.DataAccess.Repository.Extended.Attributes;
    using LogicSoftware.DataAccess.Repository.Extended.Events;
    using LogicSoftware.DataAccess.Repository.Extended.Interceptors;
    using LogicSoftware.Infrastructure.Extensions;

    /// <summary>
    /// The repository extensions provider.
    /// </summary>
    public class RepositoryExtensionsProvider : IRepositoryExtensionsProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryExtensionsProvider"/> class.
        /// </summary>
        /// <param name="interceptorFactory">
        /// The interceptor factory.
        /// </param>
        /// <param name="scope">
        /// The object scope.
        /// </param>
        public RepositoryExtensionsProvider(IInterceptorFactory interceptorFactory, IScope scope)
        {
            this.InterceptorFactory = interceptorFactory;
            this.Scope = scope;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interceptor factory.
        /// </summary>
        /// <value>The interceptor factory.</value>
        public IInterceptorFactory InterceptorFactory { get; set; }

        /// <summary>
        /// Gets or sets the object scope.
        /// </summary>
        /// <value>The object scope.</value>
        public IScope Scope { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IRepositoryExtensionsProvider methods

        /// <summary>
        /// Initializes the operation interceptor.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type.
        /// </typeparam>
        /// <returns>
        /// The operation interceptor.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "by design, to avoid typeof(T) at caller")]
        public IEnumerable<IOperationInterceptor> InitializeOperationInterceptors<T>()
        {
            var interceptQueryAttributes = typeof(T).GetCustomAttributes<InterceptAttribute>(true)
                .Where(attribute => attribute.InterceptorType.GetInterfaces()
                                        .Where(t => t == typeof(IOperationInterceptor))
                                        .Count() > 0);

            var interceptors = interceptQueryAttributes
                .Select(attr => this.InterceptorFactory.CreateOperationInterceptor(attr.InterceptorType))
                .ToList();

            interceptors.ForEach(x => x.Initialize(this.Scope));

            return interceptors;
        }

        /// <summary>
        /// Initializes the query context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void InitializeQueryContext(QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // fill type interceptors according to T
            // todo: add caching?
            IEnumerable<InterceptAttribute> interceptQueryAttributes = context.ElementType.GetCustomAttributes<InterceptAttribute>(true)
                .Where(attribute => attribute.InterceptorType.GetInterfaces()
                                        .Where(t => t == typeof(IQueryInterceptor))
                                        .Count() > 0);

            foreach (InterceptAttribute attribute in interceptQueryAttributes)
            {
                if (attribute.InterceptorType.GetInterfaces().Where(t => t == typeof(IQueryInterceptor)).Count() > 0)
                {
                    this.AddInterceptorToContext(attribute.InterceptorType, context);
                }
            }
        }

        /// <summary>
        /// Notifies interceptor about LoadOptionsCreating stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.LoadOptionsCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        public bool OnLoadOptionsCreating(LoadOptionsCreatingEventArgs e, QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // no suitable interceptors found
            if (context.Interceptors.Count == 0)
            {
                return false;
            }

            // intercept
            context.Interceptors.ForEach(pair => pair.Value.OnLoadOptionsCreating(e));

            return true;
        }

        /// <summary>
        /// Notifies interceptors about MethodCallVisit stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.MethodCallVisitEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if method call was interepted, <c>false</c> otherwise.
        /// </returns>
        public bool OnMethodCallVisit(MethodCallVisitEventArgs e, QueryContext context)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // checking if this method call visit must be intercepted

            // getting interceptor that subscibed to this method call visit
            // allowing to specify InterceptVisitAttribute for all methods in class
            // todo: add caching?
            InterceptVisitAttribute attribute = e.MethodCall.Method.GetCustomAttributes<InterceptVisitAttribute>().SingleOrDefault()
                ?? e.MethodCall.Method.DeclaringType.GetCustomAttributes<InterceptVisitAttribute>().SingleOrDefault();

            // no suitable interceptors found
            if (attribute == null)
            {
                return false;
            }

            // get from dictionary or create new
            IQueryInterceptor interceptor;
            if (!context.Interceptors.TryGetValue(attribute.InterceptorType, out interceptor))
            {
                interceptor = this.AddInterceptorToContext(attribute.InterceptorType, context);
            }

            // intercept
            interceptor.OnMethodCallVisit(e);

            return true;
        }

        /// <summary>
        /// Notifies interceptors about PreExecute stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.PreExecuteEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        public bool OnPreExecute(PreExecuteEventArgs e, QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // no suitable interceptors found
            if (context.Interceptors.Count == 0)
            {
                return false;
            }

            // intercept
            context.Interceptors.ForEach(pair => pair.Value.OnPreExecute(e));

            return true;
        }

        /// <summary>
        /// Notifies interceptors about QueryCreated stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatedEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        public bool OnQueryCreated(QueryCreatedEventArgs e, QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // no suitable interceptors found
            if (context.Interceptors.Count == 0)
            {
                return false;
            }

            // intercept
            context.Interceptors.ForEach(pair => pair.Value.OnQueryCreated(e));

            return true;
        }

        /// <summary>
        /// Notifies interceptor about QueryCreating stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        public bool OnQueryCreating(QueryCreatingEventArgs e, QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // no suitable interceptors found
            if (context.Interceptors.Count == 0)
            {
                return false;
            }

            // intercept
            context.Interceptors.ForEach(pair => pair.Value.OnQueryCreating(e));

            return true;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Adds the interceptor to query context.
        /// </summary>
        /// <param name="interceptorType">
        /// Type of the interceptor.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// Created query interceptor.
        /// </returns>
        private IQueryInterceptor AddInterceptorToContext(Type interceptorType, QueryContext context)
        {
            IQueryInterceptor interceptor = this.InterceptorFactory.CreateQueryInterceptor(interceptorType);

            // initializing interceptor with query context and object scope
            interceptor.Initialize(context, this.Scope);

            context.Interceptors.Add(interceptorType, interceptor);

            return interceptor;
        }

        #endregion
    }
}