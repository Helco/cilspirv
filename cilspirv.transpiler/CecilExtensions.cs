using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using BindingFlags = System.Reflection.BindingFlags;
using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;
using ReflType = System.Type;
using ReflMethodBase = System.Reflection.MethodBase;
using ReflMethodInfo = System.Reflection.MethodInfo;
using CecilMethodReference = Mono.Cecil.MethodReference;
using CecilType = Mono.Cecil.TypeReference;

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

        public static T Instantiate<T>(this ICustomAttribute attribute) where T : Attribute
        {
            var localType = typeof(T).Assembly.GetType(attribute.AttributeType.FullName);
            if (localType == null)
                throw new InvalidOperationException("Did not find specific attribute type");
            var ctors = localType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == attribute.ConstructorArguments.Count)
                .ToArray();
            if (ctors.Length != 1)
                throw new InvalidOperationException("Could not find unambiguous attribute constructor");

            return (T)ctors[0].Invoke(attribute.ConstructorArguments.Select(arg => arg.Value).ToArray());
        }

        /// <summary>Returns the full name of the method in our unified style</summary>param>
        public static string FullCilspirvName(this ReflMethodBase methodInfo)
        {
            var name = new StringBuilder();
            var returnType = (methodInfo as ReflMethodInfo)?.ReturnType ?? typeof(void);
            name.Append(FullName(returnType));
            name.Append(' ');
            name.Append(FullName(methodInfo.DeclaringType!));
            name.Append("::");
            name.Append(methodInfo.Name);
            name.Append('(');
            foreach (var parameter in methodInfo.GetParameters())
                name.Append(FullName(parameter.ParameterType));
            name.Append(')');
            return name.ToString();
        }

        public static string FullCilspirvName(this CecilMethodReference methodInfo)
        {
            var name = new StringBuilder();
            // the declaring type of the method is the generic instance
            // the declaring type of the return type is the generic type
            var potOwner = methodInfo.DeclaringType;
            if (methodInfo.ReturnType == null)
                name.Append(FullName(typeof(void)));
            else
                name.Append(FullName(methodInfo.ReturnType, potOwner));
            name.Append(' ');
            name.Append(FullName(methodInfo.DeclaringType!, potOwner));
            name.Append("::");
            name.Append(methodInfo.Name);
            name.Append('(');
            foreach (var parameter in methodInfo.Parameters)
                name.Append(FullName(parameter.ParameterType, potOwner));
            name.Append(')');
            return name.ToString();
        }

        private static string FullName(Type type)
        {
            if (type.FullName == null)
            {
                if (!type.IsGenericParameter)
                    throw new ArgumentException("Unknown type type, it has no full name but is not a generic parameter");
                return "!" + type.GenericParameterPosition;
            }

            var fullName = type.FullName;
            if (type.IsConstructedGenericType)
            {
                fullName = type.Namespace == null
                    ? type.Name
                    : $"{type.Namespace}.{type.Name}";
                return fullName + "<" + string.Join(',', type.GetGenericArguments().Select(FullName)) + ">";
            }

            if (type.ContainsGenericParameters)
                return fullName + "<" + string.Join(',', type.GetGenericArguments().Select(t => t.Name)) + ">";

            return fullName;
        }

        private static string FullName(CecilType type, CecilType potentialOwner)
        {
            return Ungeneric(type, potentialOwner).FullName;
        }

        public static CecilType Ungeneric(this CecilType type, CecilType? potentialOwner)
        {
            if (type is not GenericParameter genParam)
                return type;

            if (potentialOwner is not IGenericInstance genericOwner)
                throw new ArgumentException("Cannot ungeneric a generic parameter without a potential owner");

            var declaredGenParam = potentialOwner.GetElementType().GenericParameters.ElementAtOrDefault(genParam.Position);
            if (declaredGenParam != genParam)
                throw new ArgumentException("Potential owner is not the owner of the given generic parameter");

            return genericOwner.GenericArguments[genParam.Position];
        }

        public static TypeReference? GetRelatedType(this ICustomAttributeProvider provider) => (provider switch
        {
            FieldDefinition field => field.FieldType,
            ParameterDefinition param => param.ParameterType,
            MethodReturnType returnType => returnType.ReturnType,
            _ => null
        });
    }
}
