using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.WEB.Controllers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.TEST
{
	public class ProductControllerTest
	{
		private readonly Mock<IRepository<Product>> _repositoryMock;
		private readonly ProductsController _productController;

		public ProductControllerTest()
		{
			_repositoryMock = new Mock<IRepository<Product>>();
			_productController = new ProductsController(_repositoryMock.Object);
		}
	}
}