using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    PostType = new PostType
                    {
                        Id = "EUB",
                        IsRemoteArea = true,
                        Name = "易邮宝"
                    },
                    Country = "Canada",
                    Length = 3.5M,
                    Width = 2.1M,
                    Height = 1.2M,
                    Weight = 1000,
                    Details = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            Quantity = 10,
                            Sku = "apple",
                            UnitPrice = 1.1M
                        },
                        new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            Quantity = 7,
                            Sku = "banana",
                            UnitPrice = 0.8M
                        }
                    }
                },
                ["random"] = new Random()
            };
            var expr =
                 "order.PostType.IsRemoteArea ? order.Details.Sum(d=>d.UnitPrice * d.Quantity) : order.Details.Sum(d=>d.UnitPrice * d.Quantity) * 0.8M";
            var result = (decimal) lambdaParser.Eval(expr, context);
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
                PostType = new PostType
                {
                    Id = "EUB",
                    IsRemoteArea = false,
                    Name = "易邮宝"
                },
                Country = "Canada",
                Length = 3.5M,
                Width = 2.1M,
                Height = 1.2M,
                Weight = 1000,
                Details = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        Quantity = 10,
                        Sku = "apple",
                        UnitPrice = 1.1M
                    },
                    new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        Quantity = 7,
                        Sku = "banana",
                        UnitPrice = 0.8M
                    }
                }
            });
            interpreter.SetVariable("random", new Random());

            //不支持lambda表达式，如以下代码会报“Unknown identifier 'd'”
            var expr0 =
                "order.PostType.IsRemoteArea ? order.Details.Sum(d=>d.UnitPrice * d.Quantity) : order.Details.Sum(d=>d.UnitPrice * d.Quantity) * 0.8M";
            var lambda0 = interpreter.Parse(expr0);
            var result = lambda0.Invoke();

//            var expr0 =
//                "order.PostType.IsRemoteArea ? order.TotalPrice : order.TotalPrice * 0.8M";
//            var lambda0 = interpreter.Parse(expr0);
//            var result = lambda0.Invoke();

            var expr =
                "order.Quantity * order.UnitPrice >10 ? (random.Next(1,1000) * order.Quantity * order.UnitPrice) / 1000 : order.Quantity * order.UnitPrice";
            var lambda = interpreter.Parse(expr);
            var results = new List<decimal>();
            var iterations = 1000000;

            var sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
                results.Add((decimal) lambda.Invoke());

            sw.Stop();
            var str = $"{iterations} iterations: {sw.Elapsed}";
        }
    }

    public class Order
    {
        public string Id { get; set; }
        public PostType PostType { get; set; }
        public string Country { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public decimal TotalPrice => Details.Sum(detail => detail.UnitPrice * detail.Quantity);
    }

    public class PostType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsRemoteArea { get; set; }
    }

    public class OrderDetail
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}