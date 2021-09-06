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
	public class ProductAPIcontrollerTest
	{
		private readonly Mock<IRepository<Product>> mockRepo;
		private readonly ProductsAPIController aPIcontroller;
		private List<Product> _products;

		public ProductAPIcontrollerTest()
		{
			mockRepo = new Mock<IRepository<Product>>();
			aPIcontroller = new ProductsAPIController(mockRepo.Object);
			_products = new List<Product>() {
				new Product { Id = 1, Name = null, Price = 10, Stock = 5, Color = "red" },
				new Product { Id = 2, Name = "Test2", Price = 9, Stock = 15, Color = "yellow" },
				new Product { Id = 3, Name = "Test3", Price = 5, Stock = 17, Color = "green" }};
		}

		[Fact]
		public async void Get_ExecuteAction_ReturnOkStatusCodeAndProductList()
		{
			mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);

			var result = await aPIcontroller.GetProducts();

			var okResult = Assert.IsType<OkObjectResult>(result);

			var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
			Assert.Equal(_products.Count, returnProducts.Count());
		}

		[Theory]
		[InlineData(5)]
		public async void Get_WithNullId_ReturnNotFound(int id)
		{
			Product product = null;
			mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);

			var result = await aPIcontroller.GetProduct(id);

			var okResult = Assert.IsType<NotFoundResult>(result);
		}

		[Theory]
		[InlineData(2)]
		public async void Get_WithId_ReturnOkStatusCodeAndProduct(int id)
		{
			Product product = _products.FirstOrDefault(x => x.Id == id);
			mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);

			var result = await aPIcontroller.GetProduct(id);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnProducts = Assert.IsType<Product>(okResult.Value);
			Assert.Equal(product, returnProducts);
		}

		[Theory]
		[InlineData(2, 3)]
		public async void Put_IdDoesnotMatchWithProductId_ReturnBadRequest(int id, int invalidId)
		{
			Product product = _products.FirstOrDefault(x => x.Id == invalidId);
			var result = await aPIcontroller.PutProduct(id, product);

			var okResult = Assert.IsType<BadRequestResult>(result);
		}

		[Theory]
		[InlineData(2)]
		public async void Put_IdMatchesWithProductId_ReturnNoContent(int id)
		{
			Product product = _products.FirstOrDefault(x => x.Id == id);
			mockRepo.Setup(x => x.Update(product));

			var result = await aPIcontroller.PutProduct(id, product);

			var okResult = Assert.IsType<NoContentResult>(result);
			mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
		}

		[Fact]
		public async void Post_ExecuteAction_ReturnCreateAction()
		{
			Product product = new Product { Name = "test", Price = 1, Stock = 1, Color = "red" };
			mockRepo.Setup(x => x.Create(product));

			var result = await aPIcontroller.PostProduct(product);

			var okResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var size = okResult.RouteValues.Count;
			Assert.Equal(1, size);
			Assert.Equal("GetProduct", okResult.ActionName);
			Assert.True(okResult.RouteValues.ContainsKey("id"));

			mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
		}

		[Theory]
		[InlineData(5)]
		public async void Delete_WithNullId_ReturnNotFound(int id)
		{
			Product product = null;
			mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);

			var result = await aPIcontroller.DeleteProduct(id);

			var okResult = Assert.IsType<NotFoundResult>(result);
		}

		[Theory]
		[InlineData(2)]
		public async void Delete_WithId_ReturnNoContent(int id)
		{
			Product product = _products.FirstOrDefault(x => x.Id == id);
			mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
			mockRepo.Setup(x => x.Delete(product));
			var result = await aPIcontroller.DeleteProduct(id);

			mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
			var okResult = Assert.IsType<NoContentResult>(result);
		}
	}
}