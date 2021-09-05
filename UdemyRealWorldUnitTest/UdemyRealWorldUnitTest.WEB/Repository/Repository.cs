using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.WEB.Models;

namespace UdemyRealWorldUnitTest.WEB.Repository
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		public Repository(UnitTestUdemyContext unitTestUdemyContext)
		{
			_unitTestUdemyContext = unitTestUdemyContext;
			_dbSet = unitTestUdemyContext.Set<TEntity>();
		}

		private readonly UnitTestUdemyContext _unitTestUdemyContext;

		private readonly DbSet<TEntity> _dbSet;

		public async Task Create(TEntity entity)
		{
			await _dbSet.AddAsync(entity);
			await _unitTestUdemyContext.SaveChangesAsync();
		}

		public void Delete(TEntity entity)
		{
			_dbSet.Remove(entity);
			_unitTestUdemyContext.SaveChanges();
		}

		public async Task<IEnumerable<TEntity>> GetAll()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<TEntity> GetById(int? id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task Update(TEntity entity)
		{
			_unitTestUdemyContext.Entry(entity).State = EntityState.Modified;
			await _unitTestUdemyContext.SaveChangesAsync();
		}
	}
}