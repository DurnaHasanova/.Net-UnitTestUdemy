using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.WEB.Helpers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.WEB.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsAPIController : ControllerBase
	{
		private readonly IRepository<Product> repository;

		public ProductsAPIController(IRepository<Product> repository)
		{
			this.repository = repository;
		}

		[HttpGet("{a}/{b}")]
		public IActionResult Add(int a, int b)
		{
			var helper = new Helper();

			return Ok(helper.add(a, b));
		}

		// GET: api/ProductsAPI
		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			var data = await repository.GetAll();
			return Ok(data);
		}

		// GET: api/ProductsAPI/5
		[HttpGet("{id}")]
		public async Task<ActionResult> GetProduct(int id)
		{
			var product = await repository.GetById(id);

			if (product == null)
			{
				return NotFound();
			}

			return Ok(product);
		}

		// PUT: api/ProductsAPI/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProduct(int id, Product product)
		{
			if (id != product.Id)
			{
				return BadRequest();
			}

			await repository.Update(product);

			return NoContent();
		}

		// POST: api/ProductsAPI
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Product>> PostProduct(Product product)
		{
			await repository.Create(product);

			return CreatedAtAction("GetProduct", new { id = product.Id }, product);
		}

		// DELETE: api/ProductsAPI/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var product = await repository.GetById(id);
			if (product == null)
			{
				return NotFound();
			}

			repository.Delete(product);

			return NoContent();
		}

		private async Task<bool> ProductExists(int id)
		{
			var product = await repository.GetById(id);
			return product == null ? false : true;
		}
	}
}