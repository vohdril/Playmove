namespace Playmove.DAO.Generic.Interface
{
    public interface IService<TEntity>
    {
        TEntity ObterPorId(int id, string includeProperties = "");

        TEntity[] ObterTodos(string includeProperties = "", bool noTracking = true);

        bool Adicionar(TEntity entity);

        bool Editar(TEntity entity);

        bool Deletar(int id);

        bool Existe(int id);


    }
}
