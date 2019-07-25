using Xunit;

namespace Zaabee.ExpressionEngine.TestProject
{
    public class UnitTest1
    {
        public delegate decimal Calculate10Arguments(string countrycode, string zipcode,
            decimal weight, decimal chargedWeight, decimal bulkFactor,
            decimal length, decimal width, decimal height,
            decimal bunkerSurcharge, decimal discount);

        public delegate decimal Calculate5Arguments(string zipcode,
            decimal length, decimal width, decimal height, decimal discount);

        public delegate bool Bool10Arguments(string countrycode, string zipcode,
            decimal weight, decimal chargedWeight, decimal bulkFactor,
            decimal length, decimal width, decimal height,
            decimal bunkerSurcharge, decimal discount);

        [Fact]
        public void TestBool10Arguments()
        {
            var exBuilder = ExpressionEngine.Create<Bool10Arguments>();

            var formula = @"if weight < 30 then weight < 30 else if weight < 50 then weight > 50 else weight > 50";
            var calculator = exBuilder.BuildComplied(formula);

            var result1 = calculator("dd", null, 20m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);
            var result2 = calculator("dd", null, 40m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);
            var result3 = calculator("dd", null, 60m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);

            Assert.True(result1);
            Assert.False(result2);
            Assert.True(result3);

            var formula2 =
                @"20 > 1 and if weight < 30 then weight < 30 else if weight < 50 then weight > 50 else weight > 50";
            var calculator2 = exBuilder.BuildComplied(formula2);

            result1 = calculator2("dd", null, 20m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);
            result2 = calculator2("dd", null, 40m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);
            result3 = calculator2("dd", null, 60m, 0m, 0m, 0m, 0m, 0m, 100m, 0.9m);

            Assert.True(result1);
            Assert.False(result2);
            Assert.True(result3);
        }

        [Fact]
        public void TestCalculate5Arguments()
        {
            var exBuilder = ExpressionEngine.Create<Calculate5Arguments>();

            var formula =
                "((if zipcode in [aa, dd, bb] then 98 else 2) + max((max(1,2) + 3 + 1),(1+2))*3 + length * 1 + width * 3 + height * 2) * discount";
            var calculator = exBuilder.BuildComplied(formula);
            var result1 = calculator("aa", 3, 4, 5, 0.8M);
            var result2 = calculator("cc", 3, 4, 5, 0.8M);
        }
    }
}