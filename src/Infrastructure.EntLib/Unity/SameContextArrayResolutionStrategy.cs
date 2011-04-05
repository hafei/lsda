// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SameContextArrayResolutionStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The same context array resolution strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// The same context array resolution strategy.
    /// </summary>
    public class SameContextArrayResolutionStrategy : BuilderStrategy
    {
        #region Constants and Fields

        /// <summary>
        /// The generic resolve array method.
        /// </summary>
        private static readonly MethodInfo GenericResolveArrayMethod = typeof(SameContextArrayResolutionStrategy)
            .GetMethod("ResolveArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        #endregion

        #region Delegates

        /// <summary>
        /// The ArrayResolver delegate type.
        /// </summary>
        /// <param name="context">
        /// The builder context.
        /// </param>
        /// <returns>
        /// The resolved array.
        /// </returns>
        private delegate object ArrayResolver(IBuilderContext context);

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
            Type typeToBuild = context.BuildKey.Type;
            if (typeToBuild.IsArray && typeToBuild.GetArrayRank() == 1)
            {
                Type elementType = typeToBuild.GetElementType();

                MethodInfo resolverMethod = GenericResolveArrayMethod.MakeGenericMethod(elementType);

                ArrayResolver resolver = (ArrayResolver)Delegate.CreateDelegate(typeof(ArrayResolver), resolverMethod);

                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The resolve array generic method.
        /// </summary>
        /// <param name="context">
        /// The builder context.
        /// </param>
        /// <typeparam name="T">
        /// Type of the element.
        /// </typeparam>
        /// <returns>
        /// The resolved array.
        /// </returns>
        private static object ResolveArray<T>(IBuilderContext context)
        {
            IUnityContainer container = context.NewBuildUp<IUnityContainer>();

            var registrations = container.Registrations;

            if (typeof(T).IsGenericType)
            {
                registrations = registrations
                    .Where(registration => registration.RegisteredType == typeof(T)
                                           || registration.RegisteredType == typeof(T).GetGenericTypeDefinition());
            }
            else
            {
                registrations = registrations
                    .Where(registration => registration.RegisteredType == typeof(T));
            }

            var registeredNames = registrations
                .Select(registration => registration.Name) // note: including empty ones
                .Distinct()
                .ToList();

            var results = registeredNames
                .Select(name => context.NewBuildUp<T>(name))
                .ToArray();

            return results;
        }

        #endregion
    }
}