// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkipAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The skip attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The skip attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SkipAttribute : ProjectionMemberAttribute
    {
    }
}
