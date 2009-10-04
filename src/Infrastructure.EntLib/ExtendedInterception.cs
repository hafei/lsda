// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedInterception.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib
{
    using Microsoft.Practices.Unity.InterceptionExtension;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The extended interception.
    /// </summary>
    public class ExtendedInterception : Interception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedInterception"/> class.
        /// </summary>
        public ExtendedInterception()
        {
            // this.Interceptor = new InterfaceInterceptor();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Interceptor.
        /// </summary>
        /// <value>The interceptor.</value>
        public IInterceptor Interceptor { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            this.Context.Strategies.Add(new ExtendedInstanceInterceptionStrategy(this), UnityBuildStage.Setup);
            this.Context.Strategies.Add(new ExtendedTypeInterceptionStrategy(this), UnityBuildStage.PreCreation);
        }

        #endregion
    }
}