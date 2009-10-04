// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedInstanceInterceptionStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended instance interception strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib
{
    using System;

    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// The extended instance interception strategy.
    /// </summary>
    public class ExtendedInstanceInterceptionStrategy : InstanceInterceptionStrategy
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedInstanceInterceptionStrategy"/> class.
        /// </summary>
        /// <param name="interception">
        /// The interception.
        /// </param>
        public ExtendedInstanceInterceptionStrategy(ExtendedInterception interception)
        {
            if (interception == null)
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
        public override void PreBuildUp(IBuilderContext context)
        {
            // if (BuildKey.GetType(context.BuildKey) == typeof(IUnityContainer))
            // {
            // return;
            // }

            // intercepting only our types. todo: consider rewrite (maybe filter out only Unity types)
            if (!BuildKey.GetType(context.BuildKey).FullName.StartsWith("LogicSoftware", StringComparison.Ordinal))
            {
                return;
            }

            IInstanceInterceptionPolicy policy = FindInterceptorPolicy(context);
            if (policy != null)
            {
                if (policy.Interceptor.CanIntercept(BuildKey.GetType(context.BuildKey)))
                {
                    this.Interception.SetDefaultInterceptorFor(BuildKey.GetType(context.BuildKey), policy.Interceptor);
                }
            }
            else
            {
                if (this.Interception.Interceptor.CanIntercept(BuildKey.GetType(context.BuildKey)) && this.Interception.Interceptor is IInstanceInterceptor)
                {
                    this.Interception.SetDefaultInterceptorFor(BuildKey.GetType(context.BuildKey), (IInstanceInterceptor) this.Interception.Interceptor);
                }
            }

            base.PreBuildUp(context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the interceptor policy.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// Existing IInstanceInterceptionPolicy.
        /// </returns>
        private static IInstanceInterceptionPolicy FindInterceptorPolicy(IBuilderContext context)
        {
            Type buildKey = BuildKey.GetType(context.BuildKey);
            Type type = BuildKey.GetType(context.OriginalBuildKey);
            IInstanceInterceptionPolicy policy = context.Policies.Get<IInstanceInterceptionPolicy>(context.BuildKey, false) ?? context.Policies.Get<IInstanceInterceptionPolicy>(buildKey, false);
            if (policy != null)
            {
                return policy;
            }

            policy = context.Policies.Get<IInstanceInterceptionPolicy>(context.OriginalBuildKey, false) ?? context.Policies.Get<IInstanceInterceptionPolicy>(type, false);
            return policy;
        }

        #endregion
    }
}