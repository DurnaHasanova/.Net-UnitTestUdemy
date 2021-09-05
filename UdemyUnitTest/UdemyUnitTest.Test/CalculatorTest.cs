using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using UdemyUnitTest.APP;
using Xunit;

namespace UdemyUnitTest.Test
{
	public class CalculatorTest
	{
		public Calculator calculator { get; set; }
		public Mock<ICalculatorService> myMock { get; set; }

		public CalculatorTest()
		{
			myMock = new Mock<ICalculatorService>();
			calculator = new Calculator(myMock.Object);

			//calculator = new Calculator(new CalculatorService());
		}

		[Fact]
		public void AddTest2()

		{
			var names = new List<string>() { "Durna", "Hasan", "Kenan" };

			Assert.Contains(names, x => x == "Durna");
		}

		[Theory]
		[InlineData(5, 10, 15)]
		[InlineData(5, 12, 17)]
		public void Add_SimpleValue_ReturnTotalValue(int a, int b, int expectedTotal)
		{
			myMock.Setup(x => x.add(a, b)).Returns(expectedTotal);

			var totalActual = calculator.add(a, b);

			// bir metod icerisinde bir nece defe assert metodu kullana biliriz.
			Assert.Equal(expectedTotal, totalActual);
			Assert.InRange<int>(expectedTotal, 10, 20);
			// bu metod bir kere calissin  ?????//
			myMock.Verify(x => x.add(a, b), Times.Once);
		}

		[Theory]
		[InlineData(5, 0, 0)]
		public void Add_ZeroValue_ReturnZeroValue(int a, int b, int expectedTotal)
		{
			// add metodunu taklidini olusturuyoryz, esas metod calismir esas kod calismir sahte calisir
			myMock.Setup(x => x.add(a, b)).Returns(expectedTotal);

			var totalActual = calculator.add(a, b);

			Assert.Equal(expectedTotal, totalActual);
		}

		[Theory]
		[InlineData(3, 5, 15)]
		public void Multip_SimpleValue_ReturnTotalValue(int a, int b, int expectedTotal)
		{
			int actualMultip = 0;
			// add metodunu taklidini olusturuyoryz, esas metod calismir esas kod calismir sahte calisir
			myMock.Setup(x => x.multiple(It.IsAny<int>(), It.IsAny<int>())).Callback<int, int>((x, y) => actualMultip = x * y);

			calculator.multiple(a, b);
			Assert.Equal(expectedTotal, actualMultip);

			calculator.multiple(5, 20);
			Assert.Equal(100, actualMultip);
		}

		[Theory]
		[InlineData(0, 3)]
		public void Multip_ZeroValue_ReturnException(int a, int b)
		{
			myMock.Setup(x => x.multiple(a, b)).Throws(new Exception("a=0 olamaz"));

			Exception ex = Assert.Throws<Exception>(() => calculator.multiple(a, b));

			Assert.Equal("a=0 olamaz", ex.Message);
		}
	}
}