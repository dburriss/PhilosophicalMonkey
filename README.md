# Philosophical Monkey
I am a reflection helper that helps you reflect on the code around you.

> An important feature of this library is it abstracts the differences between the full .NET Framework and the new .NET Core reflection API.

## Install from nuget
> `Install-Package PhilosophicalMonkey -Pre`

# Basic Usage

The Philosophical Monkey has dozens of nugets of wisdom but here are a few examples of how he can help.
### Getting all types from a namespace

```csharp
var assembly = typeof(TestModel).GetTypeInfo().Assembly;
var types = Reflect.OnTypes.GetTypesFromNamespace(assembly, "TestModels");

Assert.Contains(types, t => t == typeof(TestModel));
```

### Get a null safe property value

```csharp
var obj = new TestModel
{
    Id = 1,
    MyString = null
};
var result = Reflect.OnProperties.NullSafeGetValue<TestModel, object>(obj, x => x.MyString, "Default");

Assert.Equal("Default", result);
```

### Mapping a dynamic to a dictionary

```csharp
dynamic d = new { Nr = 1, Name = "Devon" };
var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(d);

Assert.Equal(2, dictionary.Keys.Count);
```

Or fill object properties:

```csharp
dynamic d = new { StreetNr = 1, Street = "Main Rd" };
var dictionary = new Dictionary<string, object>() { { "StreetNr", 1 }, { "Street", "Main Rd" } };
var instance = new Address();
Reflect.OnMappings.Map(dictionary, instance);
Assert.Equal(instance.StreetNr, 1);
Assert.Equal(instance.Street, "Main Rd");
```

# Overview
The Philosphical Monkey has a couple topics he likes to reflect on. These include:
* Types
* Properties
* Attributes
* Mappings

## OnTypes
### Available Methods
* `IEnumerable<Type> GetTypesFromNamespace(Assembly assembly, params string[] @namespaces)`
* `Type[] GetAllExportedTypes(IEnumerable<Assembly> assemblies)`
* `Type[] GetAllTypes(IEnumerable<Assembly> assemblies)`
* `IEnumerable<Type> GetInterfaces(Type type)`
* `bool IsAbstract(Type type)`
* `bool ISClass(Type type)`
* `bool IsInterface(Type type)`
* `bool IsGenericType(Type type)`
* `bool IsPrimitive(Type type)`
* `bool IsSimple(Type type)`
* `IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types)`
* `Assembly GetAssembly(Type type)`

## OnProperties
### Available Methods
* `string GetPropertyName<T>(Expression<Func<T, object>> expression)`
* `string TypeSafeGetPropertyName<TInput, TResult>(Expression<Func<TInput, TResult>> expression)`
* `GetPropertyType<T>(Expression<Func<T, object>> expression)`
* `MemberInfo GetMemberInformation<T>(Expression<Func<T, object>> propertyExpression)`
* `MemberInfo GetMemberInformation(Expression propertyExpression)`
* `MemberInfo GetMemberInformation(Type type, string propertyName)`
* `PropertyInfo GetPropertyInformation(Type type, string propertyName)`
* `PropertyInfo GetPropertyInfoFromPath<T>(string path)`
* `PropertyInfo GetPropertyInfoFromPath(Type type, string path)`
* `TResult NullSafeGetValue<TSource, TResult>(TSource source, Expression<Func<TSource, TResult>> expression, TResult defaultValue)`
* `TCastResultType NullSafeGetValue<TSource, TResult, TCastResultType>(TSource source, Expression<Func<TSource, TResult>> expression, TCastResultType defaultValue, Func<object, TCastResultType> convertToResultToAction)`
* `string GetFullPropertyPathName<TSource, TResult>(Expression<Func<TSource, TResult>> expression)`
* `object GetValue<TSource, TResult>(TSource source, Expression<Func<TSource, TResult>> expression)`
* `object GetNestedPropertyValue(string name, object obj)`
* `void SetProperty<TModel>(TModel instance, Expression<Func<TModel, object>> exp, object value)`
* `void TypeSafeSetProperty<TModel, TPropertyType>(TModel instance, Expression<Func<TModel, TPropertyType>> exp, TPropertyType value)`

## OnAttributes
### Available Methods
* `T GetAttribute<T>(MemberInfo member, bool isRequired = false) where T : Attribute`
* `IEnumerable<CustomAttributeData> GetCustomAttributesData(MemberInfo memberInfo)`
* `ConstructorInfo ConstructorInfo(CustomAttributeData attributeData)`

## OnMappings
### Available Methods
* `void Map<T>(IDictionary<string, object> dictionary, T instance)`
* `IDictionary<string, object> TurnObjectIntoDictionary(object data)`