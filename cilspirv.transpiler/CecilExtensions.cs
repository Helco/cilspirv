﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;

namespace cilspirv
{
    internal static class CecilExtensions
    {
        public static bool IsExact<T>(this TypeReference typeRef) =>
            typeRef.IsExact(typeof(T));

        public static bool IsExact(this TypeReference typeRef, Type type) =>
            typeRef.FullName == type.FullName;

        public static bool IsChildOf<T>(this TypeReference typeRef) =>
            typeRef.IsChildOf(typeof(T));

        public static bool IsChildOf(this TypeReference typeRef, Type type)
        {
            var currentType = typeRef;
            while (currentType != null)
            {
                if (currentType.FullName == type.FullName)
                    return true;
                currentType = currentType.Resolve().BaseType;
            }
            return false;
        }

        public static IEnumerable<CustomAttribute> GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool exactType = true) =>
            GetCustomAttributes(provider, typeof(T), exactType);

        public static IEnumerable<CustomAttribute> GetCustomAttributes(this ICustomAttributeProvider provider, Type type, bool exactType = true) => exactType
            ? provider.CustomAttributes.Where(attr => attr.AttributeType.IsExact(type))
            : provider.CustomAttributes.Where(attr => attr.AttributeType.IsChildOf(type));

        /// <summary>Returns the full name of the method in Cecil style</summary>param>
        public static string FullName(this MethodBase methodInfo) =>
            $"{FullNameOfReturnType(methodInfo)} {methodInfo.DeclaringType?.FullName}::{methodInfo.Name}" +
            $"({string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName))})";

        private static string FullNameOfReturnType(MethodBase methodBase) =>
            (methodBase as MethodInfo)?.ReturnType?.FullName ?? typeof(void).FullName!;
    }
}
