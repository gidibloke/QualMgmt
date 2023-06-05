using Application.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRepository<T>
    {
        Task<Result<T>> GetByIdAsync(object id);
        Task<Result<List<T>>> GetAllAsync();
        Task<Result<object>> AddAsync(T entity);
        Task<Result<object>> UpdateAsync(T entity);
        Task<Result<object>> DeleteAsync(T entity);
        Task<Result<List<T>>> GetByConditionAsync(Expression<Func<T, bool>> condition);

    }
}
