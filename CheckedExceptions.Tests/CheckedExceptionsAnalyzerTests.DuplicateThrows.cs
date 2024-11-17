using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace CheckedExceptions.Tests;

using Verifier = CSharpAnalyzerVerifier<CheckedExceptionsAnalyzer, DefaultVerifier>;

partial class CheckedExceptionsAnalyzerTests
{
    [Fact]
    public async Task DuplicateThrowsInPropertyGetterSetter_ShouldReportDiagnostic()
    {
        var test = @"
using System;

public class TestClass
{
    public int Property
    {
        [Throws(typeof(Exception))]
        [Throws(typeof(Exception))]
        get
        {
            throw new Exception();
        }

        [Throws(typeof(Exception))]
        [Throws(typeof(Exception))]
        set
        {
            throw new Exception();
        }
    }
}";

        var expectedGetter = Verifier.Diagnostic("THROW005")
            .WithSpan(9, 10, 9, 35)
            .WithArguments("Exception");

        var expectedSetter = Verifier.Diagnostic("THROW005")
            .WithSpan(16, 10, 16, 35)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(test, expectedGetter, expectedSetter);
    }

    [Fact]
    public async Task DuplicateThrowsInEventAddRemove_ShouldReportDiagnostic()
    {
        var test = @"
using System;

public class TestClass
{
    public event EventHandler MyEvent
    {
        [Throws(typeof(Exception))]
        [Throws(typeof(Exception))]
        add
        {
            throw new Exception();
        }

        [Throws(typeof(Exception))]
        [Throws(typeof(Exception))]
        remove
        {
            throw new Exception();
        }
    }
}";

        var expectedAdd = Verifier.Diagnostic("THROW005")
            .WithSpan(9, 10, 9, 35)
            .WithArguments("Exception");

        var expectedRemove = Verifier.Diagnostic("THROW005")
            .WithSpan(16, 10, 16, 35)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(test, expectedAdd, expectedRemove);
    }

    [Fact]
    public async Task DuplicateThrowsInLambda_ShouldReportDiagnostic()
    {
        var test = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        Action action = [Throws(typeof(Exception))][Throws(typeof(Exception))] () => throw new Exception();
        action();
    }
}";

        var expected = Verifier.Diagnostic("THROW005")
            .WithSpan(8, 53, 8, 78)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DuplicateThrowsInLocalFunction_ShouldReportDiagnostic()
    {
        var test = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        [Throws(typeof(Exception))]
        [Throws(typeof(Exception))]
        void LocalFunction()
        {
            throw new Exception();
        }

        LocalFunction();
    }
}";

        var expected = Verifier.Diagnostic("THROW005")
            .WithSpan(9, 10, 9, 35)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }
}
