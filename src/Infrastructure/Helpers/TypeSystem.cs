// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSystem.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Helper class decompiled using Reflector from System.Data.Linq assembly.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Helper class decompiled using Reflector from System.Data.Linq assembly.
    /// </summary>
    public static class TypeSystem
    {
        #region Constants and Fields

        /// <summary>
        /// The query methods.
        /// </summary>
        private static ILookup<string, MethodInfo> queryMethods;

        /// <summary>
        /// The sequence methods.
        /// </summary>
        private static ILookup<string, MethodInfo> sequenceMethods;

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the proper extension method.
        /// </summary>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="collectionType">
        /// The type of the collection.
        /// </param>
        /// <param name="otherArgs">
        /// The other args.
        /// </param>
        /// <param name="otherTypeArgs">
        /// The other type args.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindExtensionMethod(string name, Type collectionType, Type[] otherArgs, Type[] otherTypeArgs)
        {
            otherArgs = otherArgs ?? Type.EmptyTypes;
            otherTypeArgs = otherTypeArgs ?? Type.EmptyTypes;

            var queryType = FindIQueryable(collectionType);
            if (queryType != null)
            {
                for (int i = 0; i < otherArgs.Length; i++)
                {
                    // replacing all delegates with proper Expressions
                    if (typeof(Delegate).IsAssignableFrom(otherArgs[i]))
                    {
                        otherArgs[i] = typeof(Expression<>).MakeGenericType(otherArgs[i]);
                    }
                }

                var queryableMethod = FindQueryableMethod(
                    name, 
                    (new[] { collectionType }).Concat(otherArgs).ToArray(), 
                    (new[] { GetElementType(queryType) }).Concat(otherTypeArgs).ToArray());

                if (queryableMethod != null)
                {
                    return queryableMethod;
                }
            }

            var sequenceType = FindIEnumerable(collectionType);
            if (sequenceType != null)
            {
                return FindSequenceMethod(
                    name, 
                    (new[] { collectionType }).Concat(otherArgs).ToArray(), 
                    (new[] { GetElementType(sequenceType) }).Concat(otherTypeArgs).ToArray());
            }

            return null;
        }

        /// <summary>
        /// Finds the method in specified type with specified arguments.
        /// </summary>
        /// <param name="type">
        /// The type to find method in.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The arguments of the method.
        /// </param>
        /// <param name="typeArgs">
        /// The type argumets of the method.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindMethod(Type type, string name, Type[] args, params Type[] typeArgs)
        {
            // todo: maybe refactor FindSequenceMethod & FindQueryableMethod to this
            var availableMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToLookup<MethodInfo, string>(m => m.Name);

            MethodInfo info = availableMethods[name].FirstOrDefault<MethodInfo>(m => ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                return null;
            }

            if (typeArgs != null && typeArgs.Length > 0)
            {
                return info.MakeGenericMethod(typeArgs);
            }

            return info;
        }

        /// <summary>
        /// Finds the queryable method.
        /// </summary>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The arguments of the method.
        /// </param>
        /// <param name="typeArgs">
        /// The type arguments of generic method.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindQueryableMethod(string name, Type[] args, params Type[] typeArgs)
        {
            if (queryMethods == null)
            {
                queryMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).ToLookup<MethodInfo, string>(m => m.Name);
            }

            MethodInfo info = queryMethods[name].FirstOrDefault<MethodInfo>(m => ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                return null;
            }

            if (typeArgs != null && typeArgs.Length > 0)
            {
                return info.MakeGenericMethod(typeArgs);
            }

            return info;
        }

        /// <summary>
        /// Finds the sequence method.
        /// </summary>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="sequence">
        /// The sequence.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindSequenceMethod(string name, IEnumerable sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            return FindSequenceMethod(name, new[] { sequence.GetType() }, new[] { GetElementType(sequence.GetType()) });
        }

        /// <summary>
        /// Finds the sequence method.
        /// </summary>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The arguments of the method.
        /// </param>
        /// <param name="typeArgs">
        /// The type arguments of generic method.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindSequenceMethod(string name, Type[] args, params Type[] typeArgs)
        {
            if (sequenceMethods == null)
            {
                sequenceMethods = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).ToLookup<MethodInfo, string>(m => m.Name);
            }

            MethodInfo info = sequenceMethods[name].FirstOrDefault<MethodInfo>(m => ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                return null;
            }

            if (typeArgs != null && typeArgs.Length > 0)
            {
                return info.MakeGenericMethod(typeArgs);
            }

            return info;
        }

        /// <summary>
        /// Finds the static method.
        /// </summary>
        /// <param name="type">
        /// The declaring type.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="args">
        /// The arguments of the method.
        /// </param>
        /// <param name="typeArgs">
        /// The type arguments of generic method.
        /// </param>
        /// <returns>
        /// MethodInfo of found method.
        /// </returns>
        public static MethodInfo FindStaticMethod(Type type, string name, Type[] args, params Type[] typeArgs)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            MethodInfo info = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).FirstOrDefault<MethodInfo>(delegate(MethodInfo m) { return (m.Name == name) && ArgsMatchExact(m, args, typeArgs); });
            if (info == null)
            {
                return null;
            }

            if (typeArgs != null && typeArgs.Length > 0)
            {
                return info.MakeGenericMethod(typeArgs);
            }

            return info;
        }

        /// <summary>
        /// Gets all fields.
        /// </summary>
        /// <param name="type">
        /// The declaring type.
        /// </param>
        /// <param name="bindingAttributes">
        /// The binding attributes.
        /// </param>
        /// <returns>
        /// IEnumerable of fields.
        /// </returns>
        public static IEnumerable<FieldInfo> GetAllFields(Type type, BindingFlags bindingAttributes)
        {
            Dictionary<MetaPosition, FieldInfo> dictionary = new Dictionary<MetaPosition, FieldInfo>();
            Type baseType = type;
            do
            {
                foreach (FieldInfo info in baseType.GetFields(bindingAttributes))
                {
                    if (info.IsPrivate || (type == baseType))
                    {
                        MetaPosition position = new MetaPosition(info);
                        dictionary[position] = info;
                    }
                }

                baseType = baseType.BaseType;
            }
            while (baseType != null);
            return dictionary.Values;
        }

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <param name="type">
        /// The declaring type.
        /// </param>
        /// <param name="bindingAttributes">
        /// The binding attributes.
        /// </param>
        /// <returns>
        /// The IEnumerable of properties.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(Type type, BindingFlags bindingAttributes)
        {
            Dictionary<MetaPosition, PropertyInfo> dictionary = new Dictionary<MetaPosition, PropertyInfo>();
            Type baseType = type;
            do
            {
                foreach (PropertyInfo info in baseType.GetProperties(bindingAttributes))
                {
                    if ((type == baseType) || IsPrivate(info))
                    {
                        MetaPosition position = new MetaPosition(info);
                        dictionary[position] = info;
                    }
                }

                baseType = baseType.BaseType;
            }
            while (baseType != null);
            return dictionary.Values;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="seqType">
        /// Type of the sequence.
        /// </param>
        /// <returns>
        /// The type of element.
        /// </returns>
        public static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }

            return type.GetGenericArguments().Single();
        }

        /// <summary>
        /// Gets the type of the flat sequence.
        /// </summary>
        /// <param name="elementType">
        /// Type of the element.
        /// </param>
        /// <returns>
        /// Gets IEnumerable of element type.
        /// </returns>
        public static Type GetFlatSequenceType(Type elementType)
        {
            Type type = FindIEnumerable(elementType);
            if (type != null)
            {
                return type;
            }

            return typeof(IEnumerable<>).MakeGenericType(new[] { elementType });
        }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <param name="mi">
        /// The MemberInfo.
        /// </param>
        /// <returns>
        /// Type of the member.
        /// </returns>
        public static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fieldInfo = mi as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            PropertyInfo propertyInfo = mi as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            MethodInfo methodInfo = mi as MethodInfo;
            if (methodInfo != null)
            {
                return methodInfo.ReturnType;
            }

            EventInfo eventInfo = mi as EventInfo;
            if (eventInfo != null)
            {
                return eventInfo.EventHandlerType;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the non nullable.
        /// </summary>
        /// <param name="type">
        /// The input type.
        /// </param>
        /// <returns>
        /// The type of nullable. 
        /// </returns>
        public static Type GetNonNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (IsNullableType(type))
            {
                return type.GetGenericArguments().Single();
            }

            return type;
        }

        /// <summary>
        /// Gets the type of the sequence.
        /// </summary>
        /// <param name="elementType">
        /// Type of the element.
        /// </param>
        /// <returns>
        /// IEnumerable of element type.
        /// </returns>
        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(new[] { elementType });
        }

        /// <summary>
        /// Determines whether [has I enumerable] [the specified seq type].
        /// </summary>
        /// <param name="seqType">
        /// Type of the seq.
        /// </param>
        /// <returns>
        /// <c>true</c> if [has I enumerable] [the specified seq type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasIEnumerable(Type seqType)
        {
            return FindIEnumerable(seqType) != null;
        }

        /// <summary>
        /// Determines whether the specified sequence type is IQueryable.
        /// </summary>
        /// <param name="sequenceType">
        /// Type of the sequence.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified sequence type is IQueryable; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasIQueryable(Type sequenceType)
        {
            return FindIQueryable(sequenceType) != null;
        }

        /// <summary>
        /// Determines whether [is nullable type] [the specified type].
        /// </summary>
        /// <param name="type">
        /// The input type.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(Type type)
        {
            return ((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Determines whether [is null assignable] [the specified type].
        /// </summary>
        /// <param name="type">
        /// The input type.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is null assignable] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullAssignable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsValueType)
            {
                return IsNullableType(type);
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified method is IQueryable extension.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified method is IQueryable extension; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsQueryableExtension(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            // todo: for standard extensions simple 'method.DeclaringType == typeof(Queryable)' is enough
            if (method.IsStatic)
            {
                var firstParameter = method.GetParameters().First();

                if (firstParameter.ParameterType.IsGenericType &&
                    firstParameter.ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is sequence type] [the specified seq type].
        /// </summary>
        /// <param name="seqType">
        /// Type of the seq.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is sequence type] [the specified seq type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSequenceType(Type seqType)
        {
            return (((seqType != typeof(string)) && (seqType != typeof(byte[]))) && (seqType != typeof(char[]))) && (FindIEnumerable(seqType) != null);
        }

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">
        /// The input type.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments().Single();
            }

            if (type.IsEnum)
            {
                return true;
            }

            if (type == typeof(Guid))
            {
                return true;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    return (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type);

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to gets the type of the element.
        /// </summary>
        /// <param name="sequenceType">
        /// Type of the sequence.
        /// </param>
        /// <param name="elementType">
        /// Type of the element.
        /// </param>
        /// <returns>
        /// True if sequence is provided, otherwise false.
        /// </returns>
        public static bool TryGetElementType(Type sequenceType, out Type elementType)
        {
            elementType = null;

            Type type = FindIEnumerable(sequenceType);
            if (type != null)
            {
                elementType = type.GetGenericArguments().Single();

                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether arguments match exactly.
        /// </summary>
        /// <param name="m">
        /// The MethodInfo.
        /// </param>
        /// <param name="argTypes">
        /// The argument types.
        /// </param>
        /// <param name="typeArgs">
        /// The type arguments.
        /// </param>
        /// <returns>
        /// True if arguments match exactly.
        /// </returns>
        private static bool ArgsMatchExact(MethodInfo m, Type[] argTypes, Type[] typeArgs)
        {
            ParameterInfo[] parameters = m.GetParameters();
            if (parameters.Length != argTypes.Length)
            {
                return false;
            }

            if ((!m.IsGenericMethodDefinition && m.IsGenericMethod) && m.ContainsGenericParameters)
            {
                m = m.GetGenericMethodDefinition();
            }

            if (m.IsGenericMethodDefinition)
            {
                if ((typeArgs == null) || (typeArgs.Length == 0))
                {
                    return false;
                }

                if (m.GetGenericArguments().Length != typeArgs.Length)
                {
                    return false;
                }

                m = m.MakeGenericMethod(typeArgs);
                parameters = m.GetParameters();
            }
            else if ((typeArgs != null) && (typeArgs.Length > 0))
            {
                return false;
            }

            int index = 0;
            int length = argTypes.Length;
            while (index < length)
            {
                Type parameterType = parameters[index].ParameterType;
                if (parameterType == null)
                {
                    return false;
                }

                Type c = argTypes[index];
                if (!parameterType.IsAssignableFrom(c))
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        /// <summary>
        /// Finds IEnumerable.
        /// </summary>
        /// <param name="sequenceType">
        /// The sequence type.
        /// </param>
        /// <returns>
        /// Found IEnumerable.
        /// </returns>
        private static Type FindIEnumerable(Type sequenceType)
        {
            if ((sequenceType != null) && (sequenceType != typeof(string)))
            {
                if (sequenceType.IsArray)
                {
                    return typeof(IEnumerable<>).MakeGenericType(new[] { sequenceType.GetElementType() });
                }

                if (sequenceType.IsGenericType)
                {
                    foreach (Type genericArgument in sequenceType.GetGenericArguments())
                    {
                        Type enumerableFromGenericArgument = typeof(IEnumerable<>).MakeGenericType(new[] { genericArgument });
                        if (enumerableFromGenericArgument.IsAssignableFrom(sequenceType))
                        {
                            return enumerableFromGenericArgument;
                        }
                    }
                }

                Type[] interfaces = sequenceType.GetInterfaces();
                if ((interfaces != null) && (interfaces.Length > 0))
                {
                    foreach (Type interfaceType in interfaces)
                    {
                        Type enumerableFromInterface = FindIEnumerable(interfaceType);
                        if (enumerableFromInterface != null)
                        {
                            return enumerableFromInterface;
                        }
                    }
                }

                if ((sequenceType.BaseType != null) && (sequenceType.BaseType != typeof(object)))
                {
                    return FindIEnumerable(sequenceType.BaseType);
                }
            }

            return null;
        }

        /// <summary>
        /// Finds IQueryable.
        /// </summary>
        /// <param name="sequenceType">
        /// The sequence type.
        /// </param>
        /// <returns>
        /// Found IQueryable.
        /// </returns>
        private static Type FindIQueryable(Type sequenceType)
        {
            if (sequenceType != null)
            {
                if (sequenceType.IsGenericType)
                {
                    foreach (Type genericArgument in sequenceType.GetGenericArguments())
                    {
                        Type queryableFromGenericArgument = typeof(IQueryable<>).MakeGenericType(new[] { genericArgument });
                        if (queryableFromGenericArgument.IsAssignableFrom(sequenceType))
                        {
                            return queryableFromGenericArgument;
                        }
                    }
                }

                Type[] interfaces = sequenceType.GetInterfaces();
                if ((interfaces != null) && (interfaces.Length > 0))
                {
                    foreach (Type interfaceType in interfaces)
                    {
                        Type queryableFromInterface = FindIQueryable(interfaceType);
                        if (queryableFromInterface != null)
                        {
                            return queryableFromInterface;
                        }
                    }
                }

                if ((sequenceType.BaseType != null) && (sequenceType.BaseType != typeof(object)))
                {
                    return FindIQueryable(sequenceType.BaseType);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if property is private.
        /// </summary>
        /// <param name="pi">
        /// The PropertyInfo.
        /// </param>
        /// <returns>
        /// True if private.
        /// </returns>
        private static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo info = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (info != null)
            {
                return info.IsPrivate;
            }

            return true;
        }

        #endregion
    }
}