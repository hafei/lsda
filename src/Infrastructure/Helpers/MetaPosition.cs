// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaPosition.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The meta position.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// The meta position.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MetaPosition : IEqualityComparer<MetaPosition>, IEqualityComparer
    {
        /// <summary>
        /// The metadata token.
        /// </summary>
        private readonly int metadataToken;

        /// <summary>
        /// The assembly.
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaPosition"/> struct.
        /// </summary>
        /// <param name="memberInfo">
        /// The mnember info.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "passed to another constructor, no way to validate")]
        public MetaPosition(MemberInfo memberInfo)
            : this(memberInfo.DeclaringType.Assembly, memberInfo.MetadataToken)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaPosition"/> struct.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <param name="metadataToken">
        /// The metadata token.
        /// </param>
        private MetaPosition(Assembly assembly, int metadataToken)
        {
            this.assembly = assembly;
            this.metadataToken = metadataToken;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">
        /// The first object.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "By design.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "By design.")]
        public static bool operator ==(MetaPosition x, MetaPosition y)
        {
            return AreEqual(x, y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">
        /// The first object.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "By design.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "By design.")]
        public static bool operator !=(MetaPosition x, MetaPosition y)
        {
            return !AreEqual(x, y);
        }

        /// <summary>
        /// Ares the same member.
        /// </summary>
        /// <param name="x">
        /// The first object.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// The are same member.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "By design.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "By design.")]
        public static bool AreSameMember(MemberInfo x, MemberInfo y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            return (x.MetadataToken == y.MetadataToken) && (x.DeclaringType.Assembly == y.DeclaringType.Assembly);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return AreEqual(this, (MetaPosition) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.metadataToken;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">
        /// The first object of type <see cref="MetaPosition"/> to compare.
        /// </param>
        /// <param name="y">
        /// The second object of type <see cref="MetaPosition"/> to compare.
        /// </param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(MetaPosition x, MetaPosition y)
        {
            return AreEqual(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(MetaPosition obj)
        {
            return obj.metadataToken;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="x">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        /// </exception>
        bool IEqualityComparer.Equals(object x, object y)
        {
            return this.Equals((MetaPosition) x, (MetaPosition) y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        int IEqualityComparer.GetHashCode(object obj)
        {
            return this.GetHashCode((MetaPosition) obj);
        }

        /// <summary>
        /// Checks whether objects are equal.
        /// </summary>
        /// <param name="x">
        /// The first object.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// True if equal.
        /// </returns>
        private static bool AreEqual(MetaPosition x, MetaPosition y)
        {
            return (x.metadataToken == y.metadataToken) && (x.assembly == y.assembly);
        }
    }
}