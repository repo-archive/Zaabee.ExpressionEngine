using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NReco.Linq;
using Xunit;

namespace Zaabee.ExpressionEngine.TestProject
{

	public class LambdaParserTests
	{
		private static Dictionary<string, object> GetContext()
		{
			var varContext = new Dictionary<string, object>
			{
				["pi"] = 3.14M,
				["one"] = 1M,
				["two"] = 2M,
				["test"] = "test",
				["now"] = DateTime.Now,
				["testObj"] = new TestClass(),
				["getTestObj"] = (Func<TestClass>) (() => new TestClass()),
				["arr1"] = new[] {1.5, 2.5},
				["NOT"] = (Func<bool, bool>) (t => !t),
				["Yes"] = true,
				["nullVar"] = null,
				["name_with_underscore"] = "a_b",
				["_name_with_underscore"] = "_a_b",
				["day1"] = new DateTime().AddDays(1),
				["day2"] = new DateTime().AddDays(2),
				["oneDay"] = new TimeSpan(1, 0, 0, 0),
				["twoDays"] = new TimeSpan(2, 0, 0, 0)
			};
			return varContext;
		}

		[Fact]
		public void Eval()
		{
			var lambdaParser = new LambdaParser();

			var varContext = GetContext();

			Assert.Equal("st", lambdaParser.Eval("test.Substring(2)", varContext));

			Assert.Equal(3M, lambdaParser.Eval("1+2", varContext));
			Assert.Equal(6M, lambdaParser.Eval("1+2+3", varContext));
			Assert.Equal("b{0}_", lambdaParser.Eval("\"b{0}_\"", varContext));

			Assert.Equal(3M, lambdaParser.Eval("(1+(3-1)*4)/3", varContext));

			Assert.Equal(1M, lambdaParser.Eval("one*5*one-(-1+5*5%10)", varContext));

			Assert.Equal("ab", lambdaParser.Eval("\"a\"+\"b\"", varContext));

			Assert.Equal(4.14M, lambdaParser.Eval("pi + 1", varContext));

			Assert.Equal(5.14M, lambdaParser.Eval("2 +pi", varContext));

			Assert.Equal(2.14M, lambdaParser.Eval("pi + -one", varContext));

			Assert.Equal("test1", lambdaParser.Eval("test + \"1\"", varContext));

			Assert.Equal("a_b_a_b", lambdaParser.Eval(" name_with_underscore + _name_with_underscore ", varContext));

			Assert.Equal(1M, lambdaParser.Eval("true or false ? 1 : 0", varContext));

			Assert.Equal(true, lambdaParser.Eval("5<=3 ? false : true", varContext));

			Assert.Equal(5M, lambdaParser.Eval("pi>one && 0<one ? (1+8)/3+1*two : 0", varContext));

			Assert.Equal(4M, lambdaParser.Eval("pi>0 ? one+two+one : 0", varContext));

			Assert.Equal(DateTime.Now.Year, lambdaParser.Eval("now.Year", varContext));

			Assert.Equal(true, lambdaParser.Eval(" (1+testObj.IntProp)==2 ? testObj.FldTrue : false ", varContext));

			Assert.Equal("ab2_3",
				lambdaParser.Eval(" \"a\"+testObj.Format(\"b{0}_{1}\", 2, \"3\".ToString() ).ToString() ", varContext));

			Assert.Equal(true, lambdaParser.Eval(" testObj.Hash[\"a\"] == \"1\"", varContext));

			Assert.Equal(true, lambdaParser.Eval(" (testObj.Hash[\"a\"]-1)==testObj.Hash[\"b\"].Length ", varContext));

			Assert.Equal(4.0M, lambdaParser.Eval(" arr1[0]+arr1[1] ", varContext));

			Assert.Equal(2M, lambdaParser.Eval(" (new[]{1,2})[1] ", varContext));

			Assert.Equal(true, lambdaParser.Eval(" new[]{ one } == new[] { 1 } ", varContext));

			Assert.Equal(3,
				lambdaParser.Eval(" new dictionary{ {\"a\", 1}, {\"b\", 2}, {\"c\", 3} }.Count ", varContext));

			Assert.Equal(2M,
				lambdaParser.Eval(" new dictionary{ {\"a\", 1}, {\"b\", 2}, {\"c\", 3} }[\"b\"] ", varContext));

			var arr = ((Array) lambdaParser.Eval(" new []{ new dictionary{{\"test\",2}}, new[] { one } }", varContext));
			Assert.Equal(2M, ((IDictionary) arr.GetValue(0))["test"]);
			Assert.Equal(1M, ((Array) arr.GetValue(1)).GetValue(0));

			Assert.Equal("str", lambdaParser.Eval(" testObj.GetDelegNoParam()() ", varContext));
			Assert.Equal("zzz", lambdaParser.Eval(" testObj.GetDelegOneParam()(\"zzz\") ", varContext));

			Assert.Equal(false,
				lambdaParser.Eval("(testObj.FldTrue and false) || (testObj.FldTrue && false)", varContext));
			Assert.Equal(true, lambdaParser.Eval("false or testObj.FldTrue", varContext));
			Assert.Equal("True", lambdaParser.Eval("testObj.BoolParam(true)", varContext));
			Assert.Equal("True", lambdaParser.Eval("getTestObj().BoolParam(true)", varContext));

			Assert.True((bool) lambdaParser.Eval("true && NOT( false )", varContext));
			Assert.True((bool) lambdaParser.Eval("true && !( false )", varContext));
			Assert.False((bool) lambdaParser.Eval("!Yes", varContext));

			Assert.True((bool) lambdaParser.Eval("5>two && (5>7 || test.Contains(\"t\") )", varContext));
			Assert.True((bool) lambdaParser.Eval(
				"null!=test && test!=null && test.Contains(\"t\") && true == Yes && false==!Yes && false!=Yes",
				varContext));

			Assert.Equal(new DateTime().AddDays(2), lambdaParser.Eval("day1 + oneDay", varContext));
			Assert.Equal(new DateTime().AddDays(2), lambdaParser.Eval("oneDay + day1", varContext));
			Assert.Equal(new DateTime().AddDays(1), lambdaParser.Eval("day2 - oneDay", varContext));
			Assert.Equal(new DateTime().AddDays(1), lambdaParser.Eval("day2 + -oneDay", varContext));
			Assert.Equal(new DateTime().AddDays(1), lambdaParser.Eval("-oneDay + day2", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0), lambdaParser.Eval("day2 - day1", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0).Negate(), lambdaParser.Eval("day1 - day2", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0), lambdaParser.Eval("day2 - day1", varContext));
			Assert.Equal(new TimeSpan(2, 0, 0, 0), lambdaParser.Eval("oneDay + oneDay", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0), lambdaParser.Eval("twoDays - oneDay", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0), lambdaParser.Eval("twoDays + -oneDay", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0).Negate(), lambdaParser.Eval("oneDay - twoDays", varContext));
			Assert.Equal(new TimeSpan(1, 0, 0, 0).Negate(), lambdaParser.Eval("-twoDays + oneDay", varContext));
		}

		[Fact]
		public void SingleEqualSign()
		{
			var varContext = GetContext();
			var lambdaParser = new LambdaParser {AllowSingleEqualSign = true};
			Assert.True((bool) lambdaParser.Eval("null = nullVar", varContext));
			Assert.True((bool) lambdaParser.Eval("5 = (5+1-1)", varContext));
		}

		[Fact]
		public void NullComparison()
		{
			var varContext = GetContext();
			var lambdaParser = new LambdaParser();

			Assert.True((bool) lambdaParser.Eval("null == nullVar", varContext));
			Assert.True((bool) lambdaParser.Eval("5>nullVar", varContext));
			Assert.True((bool) lambdaParser.Eval("testObj!=null", varContext));
			Assert.Empty(LambdaParser.GetExpressionParameters(lambdaParser.Parse("20 == null")));

			lambdaParser = new LambdaParser(new ValueComparer()
				{NullComparison = ValueComparer.NullComparisonMode.Sql});
			Assert.False((bool) lambdaParser.Eval("null == nullVar", varContext));
			Assert.False((bool) lambdaParser.Eval("nullVar<5", varContext));
			Assert.False((bool) lambdaParser.Eval("nullVar>5", varContext));
		}

		[Fact]
		public void EvalCachePerf()
		{
			var lambdaParser = new LambdaParser();

			var varContext = new Dictionary<string, object> {["a"] = 55, ["b"] = 2};

			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < 10000; i++)
			{
				Assert.Equal(105M, lambdaParser.Eval("(a*2 + 100)/b", varContext));
			}

			sw.Stop();
			Console.WriteLine("10000 iterations: {0}", sw.Elapsed);
		}

		public class TestClass
		{

			public int IntProp => 1;

			public string StrProp => "str";

			public bool FldTrue => true;

			public IDictionary Hash =>
				new Hashtable
				{
					{"a", 1},
					{"b", ""}
				};

			public string Format(string s, object arg1, int arg2)
			{
				return string.Format(s, arg1, arg2);
			}

			public string BoolParam(bool flag)
			{
				return flag.ToString();
			}

			public Func<string, string> GetDelegateOneParam()
			{
				return (s) => s;
			}

			public Func<string> GetDelegateNoParam()
			{
				return () => StrProp;
			}
		}
	}
}