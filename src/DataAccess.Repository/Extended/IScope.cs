// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScope.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The scope marker interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The scope marker interface.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Custom object scopes are derived from this interface.")]
    public interface IScope
    {
    }
}