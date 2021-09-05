using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.WEB.Controllers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.TEST
{
	public class ProductControllerTest
	{
		private readonly Mock<IRepository<Product>> _repositoryMock;
		private readonly ProductsController _productController;
		private List<Product> _products;

		public ProductControllerTest()
		{
			_repositoryMock = new Mock<IRepository<Product>>();
			_productController = new ProductsController(_repositoryMock.Object);
			_products = new List<Product>() {
				new Product { Id = 1, Name = null, Price = 10, Stock = 5, Color = "red" },
				new Product { Id = 2, Name = "Test2", Price = 9, Stock = 15, Color = "yellow" },
				new Product { Id = 3, Name = "Test3", Price = 5, Stock = 17, Color = "green" }};
		}

		[Fact]
		public async void Index_ActionExecute_ReturnView()
		{
			var result = await _productController.Index();

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async void Index_ActionExecute_ReturnProducts()
		{
			_repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

			var result = await _productController.Index();

			var viewResult = Assert.IsType<ViewResult>(result);
			var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
			Assert.Equal<int>(3, productList.Count());
			Assert.IsType<List<Product>>(productList);
		}

		[Fact]
		public async void Detailt_IdIsNull_ReturnNotFound()
		{
			var result = await _productController.Details(null);

			var redirect = Assert.IsType<NotFoundResult>(result);
			Assert.Equal(404, redirect.StatusCode);
		}

		[Theory]
		[InlineData(0)]
		public async void Details_IdIsInvalid_ReturnNotFound(int id)
		{
			Product product = null;
			_repositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

			var result = await _productController.Details(id);

			var redirect = Assert.IsType<NotFoundResult>(result);
			Assert.Equal(404, redirect.StatusCode);
		}

		[Theory]
		[InlineData(3)]
		public async void Details_IdIsValid_ReturnProduct(int id)
		{
			var product = _products.FirstOrDefault(x => x.Id == id);
			_repositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(product);

			var result = await _productController.Details(id);

			var viewResult = Assert.IsType<ViewResult>(result);
			var data = Assert.IsAssignableFrom<Product>(viewResult.Model);

			Assert.IsType<Product>(data);
			Assert.Equal(product.Id, data.Id);
			Assert.Equal(product, data);
		}

		[Fact]
		public void Create_ActionExecute_ReturnView()
		{
			var result = _productController.Create();

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async void CreatePOST_InvalidModel_ReturnView()
		{
			_productController.ModelState.AddModelError("Name", "Name alani zorunludur");
			var product = new Product() { Name = null, Price = 10, Stock = 1, Color = "green" };
			var result = await _productController.Create(product);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsType<Product>(viewResult.Model);
		}

		[Fact]
		public async void CreatePOST_ValidModel_ReturnRedirectToIndex()
		{
			var product = new Product() { Name = "NoteBook", Price = 10, Stock = 1, Color = "green" };
			var result = await _productController.Create(product);

			var viewResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", viewResult.ActionName);
		}

		[Fact]
		public async void CreatePOST_ValidModel_AddDataBase()
		{
			Product product = null;
			_repositoryMock.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);
			var result = await _productController.Create(_products.First());

			_repositoryMock.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
			Assert.Equal(_products.First().Id, product.Id);
		}

		[Fact]
		public async void CreatePOST_InValidModel_AddDataBase()
		{
			_productController.ModelState.AddModelError("Name", "Name alani zorunludur");
			Product product = null;
			_repositoryMock.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);
			var result = await _productController.Create(_products.First());

			_repositoryMock.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
		}
	}
}