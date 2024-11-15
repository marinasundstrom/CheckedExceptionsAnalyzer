# Checked exceptions for C#/.NET

Adds checked exceptions to C#/.NET via the ``ThrowsAttribute`` and analyzers and code fixes. 

Generated with help from ChatGPT.

Similar to Checked Exceptions in Java. But as Warnings by default. And it's opt-in.

Works for: Methods, properties (accessors), constructors, lambda expressions, and local functions.

Even works on members that have been annotated with XML doc, having ``<exception>`` element, such as ``int.Parse``.

Supports propagation of the warnings. Also deals with inheritance hierarchies for exceptions.

Examples below, and in the "Test" project.

## Purpose

Being explicit about the exceptions being thrown, and letting the compiler guide you in catching them.

Since .NET already uses exceptions.

## Contents

Analyzers:
* Unhandled exception (THROW001)
* Unhandled exception thrown (THROW002)
* Avoid declaring Throws(typeof(Exception)) (THROW003)
* Avoid throwing general Exception (THROW004)

Code fixes:
* Add ThrowsAttribute (for propagated exceptions)
* Add try-catch block (to handle exceptions)

## ``ThrowsAttribute``

This can be (re-)defined and used:

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Delegate, AllowMultiple = true)]
public class ThrowsAttribute : Attribute
{
    public Type ExceptionType { get; }

    public ThrowsAttribute(Type exceptionType)
    {
        if (!typeof(Exception).IsAssignableFrom(exceptionType))
            throw new ArgumentException("ExceptionType must be an Exception type.");

        ExceptionType = exceptionType;
    }
}
```

## Annotating methods

Annotate a method with one or more ``ThrowsAttribute`` to indicate what exceptions it might throw:

```csharp
public class DataFetcher
{
    [Throws(typeof(NullReferenceException))]
    public void FetchData()
    {
        throw new NullReferenceException("Data source is null.");
    }
}
```

Utilizing the class:

```csharp
var fetcher = new DataFetcher();
```

Test error:

```csharp
// Method 'FetchData' throws exception 'NullReferenceException' which is not handled(THROW001)
fetcher.FetchData();
```

Fix this warning like so by catching the exceptions:

```csharp
try
{
    fetcher.FetchData();
}
catch (NullReferenceException ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
```

Now the warnings won't propagate.

Also handles inheritance hierarchies, such as base class ``Exception``:

```csharp
try
{
    fetcher.FetchData();
}
catch (Exception ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
```

### Multiple attributes

A method throwing two exceptions:

```csharp
public class DataFetcher2
{
    [Throws(typeof(NullReferenceException))]
    [Throws(typeof(ArgumentException))]
    public void FetchData()
    {
        throw new NullReferenceException("Data source is null.");
    }
}
```

When you don't handle all exceptions:

```csharp
var fetcher = new DataFetcher2();

try
{
    // Method 'FetchData' throws exception 'ArgumentException' which is not handled(THROW001)
    fetcher.FetchData();
}
catch (NullReferenceException ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
```

Handling all exceptions:

```csharp
var fetcher = new DataFetcher2();

try
{
    fetcher.FetchData();
}
catch (NullReferenceException ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
catch (ArgumentException ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
```

Or catch a base class covering all exceptions, such as ``Exception``.

```csharp
var fetcher = new DataFetcher2();

try
{
    fetcher.FetchData();
}
catch (Exception ex)
{
    Console.WriteLine("Handled exception: " + ex.Message);
}
```

### Opt-in and Disable warning

If methods are not annotated with ``ThrowsAttribute``, or don't they declare exceptions via XML doc, they will not cause any errors for callers. But the analyzer will warn that you have not applied matching attributes for any exception thrown in that method.

But you can disable warnings like so:

```csharp
#pragma warning disable THROW001 // Checked exception
            TestFoo();
#pragma warning restore THROW001 // Checked exception
```

## To do

Handle rethrow.

## Proposed syntax

The analyzer could be added to C# and there could be special syntax:

```csharp
public class DataFetcher
{
    public void FetchData()
        throws NullReferenceException
        throws ArgumentException
    {
        throw new NullReferenceException("Data source is null.");
    }
}
```

## Framework support

The framework library should be annotated for this to work seamlessly.
