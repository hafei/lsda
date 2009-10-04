using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using LogicSoftware.DataAccess.Repository.Extended.Attributes;

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    public static class QueryableExtensions
    {
        [InterceptVisit(typeof(TestInterceptor))]
        public static IQueryable<T> TestMethod<T>(this IQueryable<T> source)
        {
            return source.Provider.CreateQuery<T>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)), source.Expression));
        }
    }
}
