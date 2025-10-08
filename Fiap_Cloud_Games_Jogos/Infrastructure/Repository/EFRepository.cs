using Domain.Entities;
using Domain.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Infrastructure.Repository
{
    public class EFRepository<T> : IRepository<T> where T : EntityBase
    {
        protected ApplicationDbContext _context { get; set; }
        protected DbSet<T> _dbSet { get; set; }

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Alterar(T entidade)
        {
            try
            {
                _dbSet.Update(entidade);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void Cadastrar(T entidade)
        {
            try
            {
                _dbSet.Add(entidade);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void Deletar(int id)
        {
            try
            {
                var entity = ObterPorId(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public T ObterPorId(int id)
        {
            try
            {
                return _dbSet.FirstOrDefault(entity => entity.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IList<T> ObterTodos()
        {
            try
            {
                return _dbSet.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<T> Buscar(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.Where(predicate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public T? BuscarUm(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}