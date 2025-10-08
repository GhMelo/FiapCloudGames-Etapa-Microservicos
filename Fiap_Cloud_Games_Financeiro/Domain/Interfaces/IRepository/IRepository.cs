using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces.IRepository
{
    public interface IRepository<T> where T : EntityBase
    {
        IList<T> ObterTodos();
        T ObterPorId(int id);
        void Cadastrar(T entidade);
        void Alterar(T entidade);
        void Deletar(int id);
        IEnumerable<T> Buscar(Expression<Func<T, bool>> predicate);
        T? BuscarUm(Expression<Func<T, bool>> predicate);
    }
}
