using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
		private readonly Mock<IRepository<Category>> _repositoryCatMock;
		private readonly ProductsController _productController;
		private List<Product> _products;

		public ProductControllerTest()
		{
			_repositoryMock = new Mock<IRepository<Product>>();
			_repositoryCatMock = new Mock<IRepository<Category>>();
			_productController = new ProductsController(_repositoryMock.Object, _repositoryCatMock.Object);
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

		[Fact]
		public async void Edit_IdIsNull_ReturnNotFound()
		{
			var result = await _productController.Edit(null);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async void Edit_InvalidId_ReturnNotFound()
		{
			Product product = null;
			_repositoryMock.Setup(x => x.GetById(4)).ReturnsAsync(product);
			var result = await _productController.Edit(4);

			var statusCode = Assert.IsType<NotFoundResult>(result);
			Assert.Equal(404, statusCode.StatusCode);
		}

		[Theory]
		[InlineData(2)]
		public async void Edit_ValidId_ReturnViewAndProduct(int id)
		{
			Product product = _products.FirstOrDefault(x => x.Id == id);
			_repositoryMock.Setup(x => x.GetById(id)).ReturnsAsync(product);
			var result = await _productController.Edit(id);

			var view = Assert.IsType<ViewResult>(result);
			var assign = Assert.IsAssignableFrom<Product>(view.Model);
			Assert.Equal(id, assign.Id);
		}

		[Theory]
		[InlineData(2)]
		public async void EditPOST_IdIsNotEqualProductId_ReturnRedirectToIndex(int id)
		{
			var result = await _productController.Edit(id, _products.First());

			var redirect = Assert.IsType<RedirectToActionResult>(result);

			Assert.Equal("Index", redirect.ActionName);
		}

		[Theory]
		[InlineData(1)]
		public async void EditPost_InvalidModel_ReturnRedirectToIndex(int id)
		{
			_productController.ModelState.AddModelError("Name", "Has Error");

			var result = await _productController.Edit(id, _products.FirstOrDefault(x => x.Id == id));

			var view = Assert.IsType<ViewResult>(result);

			Assert.Equal(_products.FirstOrDefault(x => x.Id == id), view.Model);
		}

		[Theory]
		[InlineData(1)]
		public async void EditPost_ValidModel_ReturnRedirectToIndex(int id)
		{
			var result = await _productController.Edit(id, _products.FirstOrDefault(x => x.Id == id));

			var view = Assert.IsType<RedirectToActionResult>(result);

			Assert.Equal("Index", view.ActionName);
		}

		[Theory]
		[InlineData(1)]
		public async void EditPost_ValidModel_ThrowDbCuncerrencyException(int id)
		{
			DbUpdateConcurrencyException ex = new DbUpdateConcurrencyException();
			_repositoryMock.Setup(x => x.Update(_products.FirstOrDefault(x => x.Id == id))).Throws(ex);

			var result = await _productController.Edit(id, _products.FirstOrDefault(x => x.Id == id));

			var view = Assert.IsType<RedirectToActionResult>(result);
		}

		[Theory]
		[InlineData(1)]
		public async void EditPost_ValidModel_UpdateMethodExecute(int id)
		{
			var product = _products.FirstOrDefault(x => x.Id == id);

			_repositoryMock.Setup(x => x.Update(product));

			await _productController.Edit(id, product);

			_repositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
		}
	}
}