using System;
using System.Threading;

namespace UdemyUnitTest.APP
{
	public class CalculatorService : ICalculatorService
	{
		public int add(int a, int b)

		{
			if (a == 0 || b == 0) return 0;
			return a + b;
		}

		public int multiple(int a, int b)

		{
			if (a == 0) throw new Exception("a=0 olamaz");
			//Thread.Sleep(1000);
			return a * b;
		}
	}
}