using Playmove.DAO.Models;
using System.Linq.Expressions;

namespace Playmove.DAO.Service
{
    public class ServiceFornecedor : IServiceFornecedor
    {
        private readonly IRepositoryFornecedor _repositoryFornecedor;

        public ServiceFornecedor(IRepositoryFornecedor repositoryFornecedor)
        {
            _repositoryFornecedor = repositoryFornecedor;
        }

        public Fornecedor ObterPorId(int id, string includeProperties = "")
        {
            return _repositoryFornecedor.FirstOrDefault(x => x.Id == id, includeProperties);
        }

        public Fornecedor[] ObterTodos(string includeProperties = "", bool noTracking = true)
        {
            return _repositoryFornecedor.Get(includeProperties: includeProperties, noTracking: noTracking);
        }

        public bool Adicionar(Fornecedor entity)
        {
            return _repositoryFornecedor.Insert(entity);
        }

        public bool Editar(Fornecedor entity)
        {
            return _repositoryFornecedor.Update(entity);
        }

        public bool Deletar(int id)
        {
            return _repositoryFornecedor.Delete(id);
        }

        public bool Existe(int id)
        {
            return _repositoryFornecedor.Exists(x => x.Id == id);
        }
        public bool ExisteCNPJ(string cnpj)
        {
            return _repositoryFornecedor.Exists(x => x.Cnpj.Equals(cnpj));
        }

        public string BuscarCNPJ(int id)
        {
            return _repositoryFornecedor.FirstOrDefault(x => x.Id == id, noTracking: true)?.Cnpj ?? "";

        }

    }
}
