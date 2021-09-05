using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyUnitTest.APP
{
	public class Calculator
	{
		private ICalculatorService _calculatorService;

		public Calculator(ICalculatorService calculatorService)
		{
			_calculatorService = calculatorService;
		}

		public int add(int a, int b)
		{
			var result = _calculatorService.add(a, b);

			return result;
		}

		public int multiple(int a, int b)
		{
			return _calculatorService.multiple(a, b);
		}
	}
}