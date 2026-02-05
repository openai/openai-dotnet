using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenAI.Tests;

/// <summary>
/// Provides utilities for testing Clone() methods on options types using reflection.
/// Automatically populates all properties with test values and validates that cloning
/// works correctly, including collection independence.
/// </summary>
internal static class CloneTestHelper
{
    [ThreadStatic]
    private static HashSet<Type> t_visitedTypes;

    [ThreadStatic]
    private static int t_instanceCounter;

    /// <summary>
    /// Validates that a Clone() method correctly clones all properties of an options object.
    /// </summary>
    /// <typeparam name="T">The type of the options object.</typeparam>
    /// <param name="original">The original instance to populate and clone.</param>
    /// <param name="cloneMethodName">The name of the clone method (default: "Clone").</param>
    public static void ValidateCloneMethod<T>(T original, string cloneMethodName = "Clone") where T : class
    {
        ResetState();

        // Use reflection to populate all settable properties with test values
        PopulateObject(original);

        // Call the Clone method via reflection
        var cloneMethod = typeof(T).GetMethod(cloneMethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.That(cloneMethod, Is.Not.Null, $"{cloneMethodName} method should exist on {typeof(T).Name}");

        var clone = (T)cloneMethod.Invoke(original, null);
        Assert.That(clone, Is.Not.Null, $"{cloneMethodName} should not return null");

        // Use reflection to validate all properties were cloned correctly
        ValidateClone(original, clone, typeof(T).Name);
    }

    /// <summary>
    /// Resets the internal state used for recursion tracking and value generation.
    /// </summary>
    public static void ResetState()
    {
        t_visitedTypes = [];
        t_instanceCounter = 0;
    }

    #region Population Logic

    /// <summary>
    /// Recursively populates all properties of an object with test values.
    /// </summary>
    public static void PopulateObject(object obj)
    {
        if (obj == null) return;

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!property.CanRead || ShouldSkipProperty(property))
            {
                continue;
            }

            try
            {
                if (IsListType(property.PropertyType))
                {
                    PopulateList(obj, property);
                }
                else if (IsDictionaryType(property.PropertyType))
                {
                    PopulateDictionary(obj, property);
                }
                else if (property.CanWrite && property.GetSetMethod(true) != null)
                {
                    var value = CreateValue(property.PropertyType);
                    if (value != null)
                    {
                        property.SetValue(obj, value);
                    }
                }
            }
            catch
            {
                // Skip properties that can't be set
            }
        }
    }

    /// <summary>
    /// Populates a list property by adding a test item.
    /// </summary>
    private static void PopulateList(object obj, PropertyInfo property)
    {
        var listValue = property.GetValue(obj);
        if (listValue == null) return;

        var elementType = property.PropertyType.GetGenericArguments().FirstOrDefault();
        if (elementType == null) return;

        var addMethod = listValue.GetType().GetMethod("Add", [elementType]);
        if (addMethod == null) return;

        // Check if read-only
        var isReadOnly = listValue.GetType().GetProperty("IsReadOnly")?.GetValue(listValue) as bool?;
        if (isReadOnly == true) return;

        var item = CreateValue(elementType);
        if (item != null)
        {
            addMethod.Invoke(listValue, [item]);
        }
    }

    /// <summary>
    /// Populates a dictionary property by adding a test key-value pair.
    /// </summary>
    private static void PopulateDictionary(object obj, PropertyInfo property)
    {
        var dictValue = property.GetValue(obj);
        if (dictValue == null) return;

        var genericArgs = property.PropertyType.GetGenericArguments();
        if (genericArgs.Length != 2) return;

        // Check if read-only
        var isReadOnly = dictValue.GetType().GetProperty("IsReadOnly")?.GetValue(dictValue) as bool?;
        if (isReadOnly == true) return;

        var addMethod = dictValue.GetType().GetMethod("Add", genericArgs);
        if (addMethod == null) return;

        var key = CreateValue(genericArgs[0]);
        var value = CreateValue(genericArgs[1]);

        if (key != null && value != null)
        {
            addMethod.Invoke(dictValue, [key, value]);
        }
    }

    /// <summary>
    /// Creates a test value for a given type, recursively populating complex types.
    /// </summary>
    public static object CreateValue(Type type)
    {
        // Handle nullable types
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            return CreateValue(underlying);
        }

        // Primitives and common types
        if (type == typeof(string)) return $"test_{t_instanceCounter++}";
        if (type == typeof(int)) return 42 + t_instanceCounter++;
        if (type == typeof(long)) return 42L + t_instanceCounter++;
        if (type == typeof(float)) return 0.5f + t_instanceCounter++;
        if (type == typeof(double)) return 0.5 + t_instanceCounter++;
        if (type == typeof(bool)) return true;
        if (type == typeof(byte)) return (byte)(t_instanceCounter++ % 256);
        if (type == typeof(char)) return 'A';
        if (type == typeof(decimal)) return 0.5m + t_instanceCounter++;
        if (type == typeof(DateTime)) return DateTime.UtcNow;
        if (type == typeof(DateTimeOffset)) return DateTimeOffset.UtcNow;
        if (type == typeof(TimeSpan)) return TimeSpan.FromMinutes(5);
        if (type == typeof(Guid)) return Guid.NewGuid();
        if (type == typeof(Uri)) return new Uri("https://example.com");

        // Enums - pick first non-zero value if available
        if (type.IsEnum)
        {
            var values = Enum.GetValues(type);
            return values.Length > 1 ? values.GetValue(1) : (values.Length > 0 ? values.GetValue(0) : null);
        }

        // Handle BinaryData
        if (type == typeof(BinaryData))
        {
            return BinaryData.FromString("test data");
        }

        // Skip known obsolete types
        if (IsObsoleteType(type))
        {
            return null;
        }

        // Try factory methods for known patterns
        var factoryValue = TryCreateViaFactory(type);
        if (factoryValue != null) return factoryValue;

        // Try parameterless constructor
        var ctorValue = TryCreateViaConstructor(type);
        if (ctorValue != null) return ctorValue;

        return null;
    }

    /// <summary>
    /// Determines if a type is obsolete and should be skipped.
    /// </summary>
    private static bool IsObsoleteType(Type type)
    {
        return false;
    }

    /// <summary>
    /// Attempts to create an instance using common factory method patterns.
    /// </summary>
    private static object TryCreateViaFactory(Type type)
    {
        // Known factory methods for specific types that require special construction
        var knownFactory = GetKnownFactoryValue(type);
        if (knownFactory != null) return knownFactory;

        // Try common factory method patterns: Create, CreateDefault, Default
        var factoryMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.ReturnType == type && m.GetParameters().Length == 0)
            .Where(m => m.Name.StartsWith("Create") || m.Name == "Default")
            .ToList();

        foreach (var method in factoryMethods)
        {
            try
            {
                return method.Invoke(null, null);
            }
            catch
            {
                // Try next method
            }
        }

        // Try static properties that return the type (e.g., SomeType.Default, SomeType.Auto)
        var staticProps = type.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == type && p.CanRead);

        foreach (var prop in staticProps)
        {
            try
            {
                return prop.GetValue(null);
            }
            catch
            {
                // Try next property
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a known factory value for types that require special construction.
    /// Override this in derived helpers if needed for domain-specific types.
    /// </summary>
    private static object GetKnownFactoryValue(Type type)
    {
        return null;
    }

    /// <summary>
    /// Attempts to create an instance via constructor and recursively populate it.
    /// </summary>
    private static object TryCreateViaConstructor(Type type)
    {
        if (!type.IsClass || type.IsAbstract) return null;

        // Ensure thread-static field is initialized
        t_visitedTypes ??= [];

        try
        {
            // Prevent infinite recursion
            if (!t_visitedTypes.Add(type)) return null;

            // Try parameterless constructor first
            var parameterlessCtor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, Type.EmptyTypes, null);

            if (parameterlessCtor != null)
            {
                var instance = parameterlessCtor.Invoke(null);
                PopulateObject(instance);
                return instance;
            }

            // Try constructor with simplest parameters
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Length)
                .ToList();

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                var args = new object[parameters.Length];
                var canCreate = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var arg = CreateValue(parameters[i].ParameterType);
                    if (arg == null && !parameters[i].HasDefaultValue && !IsNullableOrReference(parameters[i].ParameterType))
                    {
                        canCreate = false;
                        break;
                    }
                    args[i] = arg ?? parameters[i].DefaultValue;
                }

                if (canCreate)
                {
                    try
                    {
                        var instance = ctor.Invoke(args);
                        PopulateObject(instance);
                        return instance;
                    }
                    catch
                    {
                        // Try next constructor
                    }
                }
            }
        }
        finally
        {
            t_visitedTypes.Remove(type);
        }

        return null;
    }

    #endregion

    #region Validation Logic

    /// <summary>
    /// Recursively validates that all properties were cloned correctly.
    /// </summary>
    public static void ValidateClone(object original, object clone, string path)
    {
        if (original == null && clone == null) return;

        Assert.That(clone, Is.Not.Null, $"{path}: Clone should not be null when original is not null");

        var type = original.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!property.CanRead || ShouldSkipProperty(property))
            {
                continue;
            }

            var propertyPath = $"{path}.{property.Name}";

            try
            {
                var originalValue = property.GetValue(original);
                var cloneValue = property.GetValue(clone);

                if (IsListType(property.PropertyType))
                {
                    ValidateList(originalValue, cloneValue, propertyPath, property.PropertyType);
                }
                else if (IsDictionaryType(property.PropertyType))
                {
                    ValidateDictionary(originalValue, cloneValue, propertyPath);
                }
                else if (IsPrimitiveOrSimple(property.PropertyType))
                {
                    Assert.That(cloneValue, Is.EqualTo(originalValue), $"{propertyPath}: Value mismatch");
                }
                else
                {
                    // For complex reference types, just verify both are null or both are non-null
                    // Deep comparison would require recursive validation which may cause issues
                    if (originalValue == null)
                    {
                        Assert.That(cloneValue, Is.Null, $"{propertyPath}: Should be null");
                    }
                    else
                    {
                        Assert.That(cloneValue, Is.Not.Null, $"{propertyPath}: Should not be null");
                    }
                }
            }
            catch (TargetInvocationException)
            {
                // Skip properties that throw on access
            }
        }
    }

    /// <summary>
    /// Validates that a list was cloned correctly.
    /// </summary>
    private static void ValidateList(object originalObj, object cloneObj, string path, Type propertyType)
    {
        if (originalObj == null && cloneObj == null) return;

        // Get count via reflection since IList<T> doesn't implement IList
        var originalCount = GetCollectionCount(originalObj);
        var cloneCount = GetCollectionCount(cloneObj);

        if (originalCount == null && cloneCount == null) return;

        Assert.That(cloneCount, Is.Not.Null, $"{path}: Clone list should not be null when original is not null");
        Assert.That(cloneCount, Is.EqualTo(originalCount), $"{path}: List count mismatch");

        // Verify independence - modifying clone should not affect original
        if (cloneObj != null && cloneCount > 0)
        {
            var elementType = propertyType.GetGenericArguments().FirstOrDefault();
            if (elementType != null)
            {
                var addMethod = cloneObj.GetType().GetMethod("Add", [elementType]);
                var removeMethod = cloneObj.GetType().GetMethod("RemoveAt", [typeof(int)]);
                var isReadOnly = cloneObj.GetType().GetProperty("IsReadOnly")?.GetValue(cloneObj) as bool?;

                if (addMethod != null && removeMethod != null && isReadOnly != true)
                {
                    var newItem = CreateValue(elementType);
                    if (newItem != null)
                    {
                        addMethod.Invoke(cloneObj, [newItem]);
                        var newCloneCount = GetCollectionCount(cloneObj);
                        var unchangedOriginalCount = GetCollectionCount(originalObj);

                        Assert.That(unchangedOriginalCount, Is.EqualTo(originalCount),
                            $"{path}: Modifying clone should not affect original");

                        removeMethod.Invoke(cloneObj, [newCloneCount - 1]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Validates that a dictionary was cloned correctly.
    /// </summary>
    private static void ValidateDictionary(object originalObj, object cloneObj, string path)
    {
        var original = originalObj as IDictionary;
        var clone = cloneObj as IDictionary;

        if (original == null && clone == null) return;

        Assert.That(clone, Is.Not.Null, $"{path}: Clone dictionary should not be null when original is not null");
        Assert.That(clone?.Count, Is.EqualTo(original?.Count), $"{path}: Dictionary count mismatch");

        if (original != null && clone != null)
        {
            foreach (var key in original.Keys)
            {
                Assert.That(clone.Contains(key), Is.True, $"{path}: Should contain key '{key}'");
                Assert.That(clone[key], Is.EqualTo(original[key]), $"{path}: Value mismatch for key '{key}'");
            }
        }
    }

    #endregion

    #region Helper Methods

    private static bool ShouldSkipProperty(PropertyInfo property)
    {
        // Skip Patch property (ref return type)
        if (property.Name == "Patch") return true;

        // Skip indexers
        if (property.GetIndexParameters().Length > 0) return true;

        return false;
    }

    private static bool IsListType(Type type)
    {
        if (!type.IsGenericType) return false;

        var genericDef = type.GetGenericTypeDefinition();
        if (genericDef == typeof(IList<>)) return true;

        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
    }

    private static bool IsDictionaryType(Type type)
    {
        if (!type.IsGenericType) return false;

        var genericDef = type.GetGenericTypeDefinition();
        if (genericDef == typeof(IDictionary<,>)) return true;

        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }

    private static bool IsPrimitiveOrSimple(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(DateTime)
            || underlying == typeof(DateTimeOffset)
            || underlying == typeof(TimeSpan)
            || underlying == typeof(Guid);
    }

    private static bool IsNullableOrReference(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    private static int? GetCollectionCount(object collection)
    {
        if (collection == null) return null;

        var countProp = collection.GetType().GetProperty("Count");
        return countProp?.GetValue(collection) as int?;
    }

    #endregion
}
