using Application.Core;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<object>> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return Result<object>.Success(result);
        }

        public async Task<Result<object>> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return Result<object>.Success(result);
        }

        public async Task<Result<List<T>>> GetAllAsync()
        {
            List<T> entities = await _context.Set<T>().ToListAsync();
            if(entities != null)
                return Result<List<T>>.Success(entities);
            return Result<List<T>>.Failure(errorMessage: "Sequence does not contain element");
        }

        public async Task<Result<List<T>>> GetByConditionAndIncludeAsync(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (condition != null)
            {
                query = query.Where(condition);
            }
            if(includes.Length != 0)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            List<T> entities = await query.ToListAsync();
            if( entities != null)
                return Result<List<T>>.Success(entities);
            return Result<List<T>>.Failure(errorMessage: "Sequence does not contain element");
        }

        public async Task<Result<T>> GetByIdAsync(object id)
        {
            if(id.GetType() == typeof(string))
            {
                id = Guid.Parse(id.ToString());
            }
            T entity = await _context.Set<T>().FindAsync(id);
            if(entity != null)
                return Result<T>.Success(entity);
            return Result<T>.Failure("Not found");
        }

        public async Task<Result<object>> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return Result<object>.Success(result);
        }
    }
}
