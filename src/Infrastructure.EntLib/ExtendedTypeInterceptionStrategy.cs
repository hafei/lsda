// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedTypeInterceptionStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended type interception strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib
{
    using System;

    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// The extended type interception strategy.
    /// </summary>
    public class ExtendedTypeInterceptionStrategy : TypeInterceptionStrategy
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTypeInterceptionStrategy"/> class.
        /// </summary>
        /// <param name="interception">
        /// The interception.
        /// </param>
        public ExtendedTypeInterceptionStrategy(ExtendedInterception interception)
        {
            if (null == interception)
            {
                throw new ArgumentNullException("interception");
            }

            this.Interception = interception;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Interception.
        /// </summary>
        /// <value>The interception.</value>
        public ExtendedInterception Interception { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">
        /// Context of the build operation.
        /// </param>
        /// <remarks>
        /// In this class, PreBuildUp is responsible for figuring out if the
        /// class is proxiable, and if so, replacing it with a proxy class.
        /// </remarks>
        public override void PreBuildUp(IBuilderContext context)
        {
            // intercepting only our types. todo: consider rewrite (maybe filter out only Unity types)
            if (!BuildKey.GetType(context.BuildKey).FullName.StartsWith("LogicSoftware", StringComparison.Ordinal))
            {
                return;
            }

            var policy = GetInterceptionPolicy(context);
            if (policy != null)
            {
                if (policy.Interceptor.CanIntercept(BuildKey.GetType(context.BuildKey)))
                {
                    this.Interception.SetInterceptorFor(BuildKey.GetType(context.BuildKey), policy.Interceptor);
                }
            }
            else
            {
                if (this.Interception.Interceptor.CanIntercept(BuildKey.GetType(context.BuildKey)) && this.Interception.Interceptor is ITypeInterceptor)
                {
                    this.Interception.SetDefaultInterceptorFor(BuildKey.GetType(context.BuildKey), (ITypeInterceptor) this.Interception.Interceptor);
                }
            }

            base.PreBuildUp(context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Just a copy of TypeInterceptionStrategy.GetInterceptorPolicy() which is private.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Existing ITypeInterceptionPolicy.</returns>
        private static ITypeInterceptionPolicy GetInterceptionPolicy(IBuilderContext context)
        {
            ITypeInterceptionPolicy policy = context.Policies.Get<ITypeInterceptionPolicy>(context.BuildKey, false);
            if (policy == null)
            {
                policy = context.Policies.Get<ITypeInterceptionPolicy>(BuildKey.GetType(context.BuildKey), false);
            }

            return policy;
        }

        #endregion
    }
}