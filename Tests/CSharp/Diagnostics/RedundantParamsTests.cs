using System;
using NUnit.Framework;
using RefactoringEssentials.CSharp.Diagnostics;

namespace RefactoringEssentials.Tests.CSharp.Diagnostics
{
    [TestFixture]
    public class RedundantParamsTests : CSharpDiagnosticTestBase
    {
        [Test]
        public void TestBasicCase()
        {
            Analyze<RedundantParamsAnalyzer>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, $params$ object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}", @"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
        }

        [Test]
        public void TestValidCase()
        {
            Analyze<RedundantParamsAnalyzer>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
        }

        [Test]
        public void ValideParamsUsageTests()
        {
            Analyze<RedundantParamsAnalyzer>(@"class FooBar
{
	public virtual void Foo(string fmt, params object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, $params$ object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
        }

        [Test]
        public void TestDisable()
        {
            Analyze<RedundantParamsAnalyzer>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	// ReSharper disable once RedundantParams
#pragma warning disable " + CSharpDiagnosticIDs.RedundantParamsAnalyzerID + @"
	public override void Foo(string fmt, params object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
        }
    }
}

