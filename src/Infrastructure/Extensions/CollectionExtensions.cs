using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LogicSoftware.Infrastructure.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return (collection == null) || (collection.Count == 0);
        }
    }
}
