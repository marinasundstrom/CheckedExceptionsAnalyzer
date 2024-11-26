using Microsoft.CodeAnalysis.Testing;

namespace Sundstrom.CheckedExceptions.Tests;

using Verifier = CSharpAnalyzerVerifier<CheckedExceptionsAnalyzer, DefaultVerifier>;

public partial class RethtrowsTest
{
    [Fact]
    public async Task Should_ReportDiagnostic_WhenRethrownExceptionIsNotDeclared()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.IsThrown("InvalidOperationException")
            .WithSpan(19, 13, 19, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task Should_NotReportDiagnostic_WhenRethrownExceptionIsDeclared()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(InvalidOperationException))]
            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }
        }
        """;

        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task Should_ReportDiagnostic_ForDeclaredButNotThrownException()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(InvalidOperationException))]
            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
         .WithSpan(17, 13, 17, 31);

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task Should_ReportDiagnostics_ForAllRethrownExceptions_WhenCatchingWithoutSpecificException()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
         .WithSpan(20, 13, 20, 19);

        var expected2 = Verifier.MightBeThrown("InvalidOperationException")
            .WithSpan(20, 13, 20, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected, expected2);
    }


    [Fact]
    public async Task Should_ReportDiagnostics_ForAllRethrownExceptions_WhenCatchingWithoutSpecificException2()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            public void MethodThatThrows1()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows2()
            {
                throw new ArgumentException();
            }

            public void Foo1()
            {
                try
                {
                    MethodThatThrows1();
                    MethodThatThrows2();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
         .WithSpan(26, 13, 26, 19);

        var expected2 = Verifier.MightBeThrown("InvalidOperationException")
            .WithSpan(26, 13, 26, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected, expected2);
    }


    [Fact]
    public async Task Should_ReportDiagnostics_ForAllRethrownExceptions_WhenCatchingWithoutSpecificException3()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                    
                    throw new ArgumentException();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
         .WithSpan(21, 13, 21, 19);

        var expected2 = Verifier.MightBeThrown("InvalidOperationException")
            .WithSpan(21, 13, 21, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected, expected2);
    }

    [Fact]
    public async Task Should_ReportDiagnostic_ForUndeclaredExceptionRethrown_WhenSomeExceptionsAreDeclared()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(ArgumentException))]
            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("InvalidOperationException")
            .WithSpan(21, 13, 21, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task Should_ReportDiagnostic_ForUndeclaredExceptionRethrown_WhenOtherExceptionsAreDeclared()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(InvalidOperationException))]
            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
            .WithSpan(21, 13, 21, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task Should_NotReportDiagnostic_WhenAllRethrownExceptionsAreDeclared()
    {
        var test = /* lang=c#-test */ """
        using System;

        public class RethrowTest
        {
            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new InvalidOperationException();
            }

            [Throws(typeof(InvalidOperationException))]
            [Throws(typeof(ArgumentException))]
            public void Foo1()
            {
                try
                {
                    MethodThatThrows();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task Should_ReportDiagnostics_ForMultipleUndeclaredExceptions_WhenCatchingWithoutSpecificException()
    {
        var test = /* lang=c#-test */ """
        #nullable enable
        using System;
        using System.Text;

        public class RethrowTest
        {
            public void Test()
            {
                try
                {
                    int.Parse("Foo");

                    StringBuilder sb = new StringBuilder();

                    sb.Length = 2;
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("FormatException")
            .WithSpan(19, 13, 19, 19);

        var expected2 = Verifier.MightBeThrown("OverflowException")
            .WithSpan(19, 13, 19, 19);

        var expected3 = Verifier.MightBeThrown("ArgumentOutOfRangeException")
            .WithSpan(19, 13, 19, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected, expected2, expected3);
    }

    [Fact]
    public async Task Should_ReportDiagnostics_ForRethrownExceptions_FromPropertyAndMethod_WhenCatchingWithoutSpecificException()
    {
        var test = /* lang=c#-test */ """
        #nullable enable
        using System;
        using System.Collections.Generic;
        using System.Text;

        public class RethrowTest
        {
            public int Foo
            {
                [Throws(typeof(InvalidOperationException))]
                set
                {

                }
            }

            public void Foo5()
            {
                try
                {
                    List<int> list = new List<int>();
                    var x = list[-1];

                    Foo = 2;
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected1 = Verifier.MightBeThrown("ArgumentOutOfRangeException")
            .WithSpan(28, 13, 28, 19);

        var expected2 = Verifier.MightBeThrown("InvalidOperationException")
            .WithSpan(28, 13, 28, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task Should_ReportDiagnostics_ForRethrownExceptions_FromPropertyAndMethod_WhenCatchingWithoutSpecificException2()
    {
        var test = /* lang=c#-test */ """
        #nullable enable
        using System;

        public class RethrowTest
        {
            [Throws(typeof(ArgumentException))]
            public void MethodThatThrows()
            {
                throw new ArgumentException();
            }

            public void Foo5()
            {
                try
                {
                    MethodThatThrows();
                }
                catch
                {
                    throw;
                }
            }
        }
        """;

        var expected = Verifier.MightBeThrown("ArgumentException")
            .WithSpan(20, 13, 20, 19);

        await Verifier.VerifyAnalyzerAsync(test, expected);
    }
}