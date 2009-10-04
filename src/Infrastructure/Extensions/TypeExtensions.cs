// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The type extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        #region Public Methods

        /// <summary>
        /// Searches and returns attributes. The inheritance chain is not used to find the attributes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of attribute to search for.
        /// </typeparam>
        /// <param name="type">
        /// The type which is searched for the attributes.
        /// </param>
        /// <returns>
        /// Returns all attributes.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static T[] GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            return GetCustomAttributes(type, typeof(T), false).Cast<T>().ToArray();
        }

        /// <summary>
        /// Searches and returns attributes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of attribute to search for.
        /// </typeparam>
        /// <param name="type">
        /// The type which is searched for the attributes.
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes. Interfaces will be searched, too.
        /// </param>
        /// <returns>
        /// Returns all attributes.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static T[] GetCustomAttributes<T>(this Type type, bool inherit) where T : Attribute
        {
            return GetCustomAttributes(type, typeof(T), inherit).Cast<T>().ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Private helper for searching attributes.
        /// </summary>
        /// <param name="type">
        /// The type which is searched for the attribute.
        /// </param>
        /// <param name="attributeType">
        /// The type of attribute to search for.
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attribute. Interfaces will be searched, too.
        /// </param>
        /// <returns>
        /// An array that contains all the custom attributes, or an array with zero elements if no attributes are defined.
        /// </returns>
        private static object[] GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            if (!inherit)
            {
                return type.GetCustomAttributes(attributeType, false);
            }

            var attributeCollection = new List<object>();

            type.GetCustomAttributes(attributeType, true).ForEach(attributeCollection.Add);

            // comment previous line and uncomment this block to make this method iterate through all base types manually
            // difference will be in attributes with Inherit = false (now they are not returned)
            // var baseType = type;
            // do
            // {
            // baseType.GetCustomAttributes(attributeType, false).ForEach(attributeCollection.Add);
            // baseType = baseType.BaseType;
            // }
            // while (baseType != null && baseType != typeof(object));
            foreach (var interfaceType in type.GetInterfaces())
            {
                GetCustomAttributes(interfaceType, attributeType, true).ForEach(attributeCollection.Add);
            }

            return attributeCollection.ToArray();
        }

        #endregion
    }
}