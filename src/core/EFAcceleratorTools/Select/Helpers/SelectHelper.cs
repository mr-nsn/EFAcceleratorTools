using System.Linq.Expressions;
using System.Reflection;

namespace EFAcceleratorTools.Select.Helpers;

/// <summary>
/// Provides utilities for dynamically generating LINQ select expressions based on a set of property names.
/// Useful for scenarios where only specific fields of an object need to be projected at runtime.
/// </summary>
public static class SelectHelper
{
    private static List<string> _avaibleParameters = Enumerable.Empty<string>().ToList();

    /// <summary>
    /// Generates a dynamic select expression for the specified type, projecting only the given fields.
    /// </summary>
    /// <typeparam name="T">The type of the entity to project.</typeparam>
    /// <param name="fields">The names of the fields to include in the projection. Supports nested properties using dot notation.</param>
    /// <returns>
    /// An expression that can be used to select only the specified fields from an object of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fields"/> is null or empty.</exception>
    public static Expression<Func<T, T>> DynamicSelectGenerator<T>(string[] fields)
    {
        if (fields is null || !fields.Any()) throw new ArgumentNullException(nameof(fields), "fields cannot be empty.");

        InitializeAvaiableParameters();

        // Group the fields by their first part (before any punctuation) to handle nested properties
        var groupedEntityFields = fields
            .GroupBy(g => string.Join("", g.TakeWhile(s => !char.IsPunctuation(s))))
            .ToDictionary(g => g.Key, g => g.Select(s => s.Contains(".") ? s.Substring(g.Key.Length + 1) : s).ToList());

        // Input parameter "o"
        var xParameter = Expression.Parameter(typeof(T), "o");

        // New statement "new T()"
        var xNew = Expression.New(typeof(T));

        // Create initializers
        var bindings = groupedEntityFields
            .Select(d => CreateBinding(typeof(T), d, xParameter))
            .ToList();

        // Initialization "new T { Field1 = o.Field1, Field2 = o.Field2... }"
        var xInit = Expression.MemberInit(xNew, bindings);

        // Expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

        // Compile to Func<Data, Data>
        return lambda;
    }

    // Prepare bindings to recusive bindings
    private static MemberAssignment CreateBinding(Type type, KeyValuePair<string, List<string>> propertyAndPaths, Expression xParameter)
    {
        var keyProperty = type.GetProperty(propertyAndPaths.Key);
        if (keyProperty == null) throw new ArgumentException($"Property '{propertyAndPaths.Key}' not found on type '{type.Name}'.");
        return CreateBinding(keyProperty, propertyAndPaths.Value, xParameter);
    }

    // Create binding for a property with nested properties recursivelly
    private static MemberAssignment CreateBinding(PropertyInfo keyProperty, List<string> nestedProperties, Expression xParameter)
    {
        Type nestedType = keyProperty.PropertyType;
        Expression nestedParam = Expression.Property(xParameter, keyProperty);

        if (nestedProperties.Any(p => p.Contains("."))) // Recursive case (When a nested property have a punctuation)
        {
            // Create a new grouped dictionary for nested properties 
            var newNestedProperties = nestedProperties
                .GroupBy(g => g.Contains(".") ? string.Join("", g.TakeWhile(s => !char.IsPunctuation(s))) : keyProperty.Name)
                .ToDictionary(g => g.Key, g => g.Select(s => s.Contains(".") ? s.Substring(g.Key.Length + 1) : s).ToList());

            // Threat when the nested property is a list or array
            if (IsCollection(nestedType))
            {
                var itemType = nestedType.IsGenericType
                    ? nestedType.GetGenericArguments()[0]
                    : nestedType.GetElementType();

                var newNestedParam = Expression.Parameter(itemType!, GetNextParameter());

                var nestedBindings = newNestedProperties.SelectMany(p =>
                {
                    if (p.Key == keyProperty.Name)
                    {
                        return CreateMemberBindingHelper(itemType!, p.Value, newNestedParam);
                    }
                    else
                    {
                        var newKeyProperty = itemType!.GetProperty(p.Key);

                        return new MemberAssignment[] { CreateBinding(newKeyProperty!, p.Value, newNestedParam) };
                    }
                }).ToArray();

                var newItem = Expression.MemberInit(Expression.New(itemType!), nestedBindings);
                var selectLambda = Expression.Lambda(newItem, newNestedParam);

                var newNested = CreateSelectCall(itemType!, selectLambda, nestedParam);
                return Expression.Bind(keyProperty, newNested);
            }
            // Threat when the nested property is an simple object
            else
            {
                var nestedBindings = newNestedProperties.SelectMany(p =>
                {
                    if (p.Key == keyProperty.Name)
                    {
                        return CreateMemberBindingHelper(nestedType, p.Value, nestedParam);
                    }
                    else
                    {
                        var newKeyProperty = nestedType.GetProperty(p.Key);

                        return new MemberAssignment[] { CreateBinding(newKeyProperty!, p.Value, nestedParam) };
                    }
                }).ToArray();

                var newNested = Expression.MemberInit(Expression.New(nestedType), nestedBindings);
                return Expression.Bind(keyProperty, newNested);
            }
        }
        else // Base case (When a nested property does not have a punctuation)
        {
            if (IsSimple(nestedType)) // Simple property (only create the binding)
            {
                var propertyExpr = Expression.Property(xParameter, keyProperty);
                return Expression.Bind(keyProperty, propertyExpr);
            }
            else if (IsComplex(nestedType)) // Complex property (create binding for all the properties selected)
            {
                var nestedBindings = CreateMemberBindingHelper(nestedType, nestedProperties, nestedParam);

                var newNested = Expression.MemberInit(Expression.New(nestedType), nestedBindings);
                return Expression.Bind(keyProperty, newNested);
            }
            else if (IsCollection(nestedType)) // Lists or arrays (create a binding for the list)
            {
                var nestedBindings = CreateListBindingHelper(nestedType, nestedProperties, nestedParam);
                return Expression.Bind(keyProperty, nestedBindings);
            }
            else
            {
                throw new Exception($"Unsupported type '{nestedType.Name}' for property '{keyProperty.Name}'.");
            }
        }
    }

    // Helper to create binding for complex object properties
    private static MemberAssignment[] CreateMemberBindingHelper(Type type, List<string> properties, Expression paramExpr)
    {
        return properties
            .Select(p =>
            {
                var propertyInfo = type.GetProperty(p);
                if (propertyInfo == null)
                    throw new ArgumentException($"Property '{p}' not found on type '{type.Name}'.");

                var propertyExpr = Expression.Property(paramExpr, propertyInfo);
                return Expression.Bind(propertyInfo, propertyExpr);
            }).ToArray();
    }

    // Helper to create binding for list properties
    private static MethodCallExpression CreateListBindingHelper(Type type, List<string> properties, Expression paramExpr)
    {
        var itemType = type.IsGenericType
                ? type.GetGenericArguments()[0]
                : type.GetElementType();

        // Create the parameter for the item
        var itemParam = Expression.Parameter(itemType!, GetNextParameter());

        var itemBindings = CreateMemberBindingHelper(itemType!, properties, itemParam);

        var newItem = Expression.MemberInit(Expression.New(itemType!), itemBindings);
        var selectLambda = Expression.Lambda(newItem, itemParam);

        return CreateSelectCall(itemType!, selectLambda, paramExpr);
    }

    // Helper to create the Select call expression
    private static MethodCallExpression CreateSelectCall(Type type, LambdaExpression lambda, Expression paramExpr)
    {
        var selectMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(type!, type!);

        var selectCall = Expression.Call(
            selectMethod,
            paramExpr,
            lambda
        );

        var toListMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
            .MakeGenericMethod(type!);

        return Expression.Call(toListMethod, selectCall);
    }

    // Verify if the type is simple
    private static bool IsSimple(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Nullable type, check if the nested type is simple.
            return IsSimple(type.GetGenericArguments()[0]);
        }

        return type.IsPrimitive
          || type.IsEnum
          || type.Equals(typeof(string))
          || type.Equals(typeof(decimal))
          || type.Equals(typeof(DateTime));
    }

    // Verify if the type is complex
    private static bool IsComplex(Type type)
    {
        return !IsSimple(type) && !IsCollection(type);
    }

    // Verify if the type is enumerable
    private static bool IsCollection(Type type)
    {
        var typeFullName = type.FullName ?? string.Empty;
        var collectionsNameSpace = string.Format("{0}.{1}.{2}", nameof(System), nameof(System.Collections), nameof(System.Collections.Generic));
        return typeFullName.StartsWith(collectionsNameSpace) && type != typeof(string);
    }

    // Initialize the available parameters
    private static void InitializeAvaiableParameters()
    {
        _avaibleParameters = Enumerable.Range('a', 26).Select(c => ((char)c).ToString()).ToList();
    }

    // Get the next available parameter
    private static string GetNextParameter()
    {
        var proximoParametro = _avaibleParameters.FirstOrDefault();
        _avaibleParameters = _avaibleParameters.Skip(1).ToList();
        return proximoParametro ?? throw new InvalidOperationException("No more parameters available.");
    }
}
