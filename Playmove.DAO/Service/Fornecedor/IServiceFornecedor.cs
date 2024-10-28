using Playmove.DAO.Generic.Interface;
using Playmove.DAO.Models;

namespace Playmove.DAO.Service
{
    public interface IServiceFornecedor : IService<Fornecedor>
    {
        public bool ExisteCNPJ(string cnjp);
        public string BuscarCNPJ(int id);
    }
}
