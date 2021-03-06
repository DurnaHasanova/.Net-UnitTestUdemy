using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.WEB.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IRepository<Product> repository;
		private readonly IRepository<Category> categoryrepository;

		public ProductsController(IRepository<Product> repository, IRepository<Category> catrepository)
		{
			this.repository = repository;
			this.categoryrepository = catrepository;
		}

		// GET: Products
		public async Task<IActionResult> Index()
		{
			return View(await repository.GetAll());
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await repository.GetById(id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// GET: Products/Create
		public async Task<IActionResult> Create()
		{
			var cat = await categoryrepository.GetAll();
			ViewData["CategoryId"] = new SelectList(cat, "Id", "Name");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color, CategoryId")] Product product)
		{
			if (ModelState.IsValid)
			{
				await repository.Create(product);
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await repository.GetById(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(await categoryrepository.GetAll(), "Id", "Name", product.CategoryId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock,Color, CategoryId")] Product product)
		{
			if (id != product.Id)
			{
				return RedirectToAction(nameof(Index));
			}

			if (ModelState.IsValid)
			{
				try
				{
					await repository.Update(product);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!await ProductExists(product.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await repository.GetById(id);

			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await repository.GetById(id);
			repository.Delete(product);
			return RedirectToAction(nameof(Index));
		}

		private async Task<bool> ProductExists(int id)
		{
			var product = await repository.GetById(id);
			if (product == null) return false;
			else return true;
		}
	}
}