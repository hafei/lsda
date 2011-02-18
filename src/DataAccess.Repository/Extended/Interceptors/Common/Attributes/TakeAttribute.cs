// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TakeAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The take attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The take attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TakeAttribute : ProjectionMemberAttribute
    {
    }
}
