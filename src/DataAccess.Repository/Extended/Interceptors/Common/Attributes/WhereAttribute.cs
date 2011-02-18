// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The where attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The where attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class WhereAttribute : ProjectionMemberAttribute
    {
    }
}
