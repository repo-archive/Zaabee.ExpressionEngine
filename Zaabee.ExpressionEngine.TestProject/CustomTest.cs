using System;
using System.Collections.Generic;
using System.Diagnostics;
using DynamicExpresso;
using NReco.Linq;
using Xunit;

namespace Zaabee.ExpressionEngine.TestProject
{
    public class CustomTest
    {
        [Fact]
        public void TestLambdaParser()
        {
            var lambdaParser = new LambdaParser();
            var context = new Dictionary<string, object>
            {
                ["order"] = new Order
                {
                    Id = "F100021909092009",
                    PostTypeId = "EUB",
                    Country = "Canada",
                    Length = 3.5M,
                    Width = 2.1M,
                    Height = 1.2M,
                    Weight = 1000,
                    UnitPrice = 1.0M,
                    Quantity = 11
                },
                ["random"] = new Random()
            };
            var expr =
                "order.Quantity * order.UnitPrice >10 ? (random.Next(1,1000) * order.Quantity * order.UnitPrice) / 1000 : order.Quantity * order.UnitPrice";
            var results = new List<decimal>();
            var iterations = 1000000;
            
            var sw = Stopwatch.StartNew();
            
            for (var i = 0; i < iterations; i++)
                results.Add((decimal) lambdaParser.Eval(expr, context));

            sw.Stop();
            var str = $"{iterations} iterations: {sw.Elapsed}";
        }

        [Fact]
        public void TestDynamicExpresso()
        {
            var interpreter = new Interpreter();
            interpreter.SetVariable("order", new Order
            {
                Id = "F100021909092009",
                PostTypeId = "EUB",
                Country = "Canada",
                Length = 3.5M,
                Width = 2.1M,
                Height = 1.2M,
                Weight = 1000,
                UnitPrice = 1.0M,
                Quantity = 11
            });
            interpreter.SetVariable("random", new Random());
            
            var expr =
                "order.Quantity * order.UnitPrice >10 ? (random.Next(1,1000) * order.Quantity * order.UnitPrice) / 1000 : order.Quantity * order.UnitPrice";
            var lambda = interpreter.Parse(expr);
            var results = new List<decimal>();
            var iterations = 1000000;
            
            var sw = Stopwatch.StartNew();
            
            for (var i = 0; i < iterations; i++)
                results.Add((decimal)lambda.Invoke());

            sw.Stop();
            var str = $"{iterations} iterations: {sw.Elapsed}";
        }
    }

    public class Order
    {
        public string Id { get; set; }
        public string PostTypeId { get; set; }
        public string Country { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class PostType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsRemoteArea { get; set; }
    }
}