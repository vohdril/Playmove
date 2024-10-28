using Playmove.DAO.Data;
using Playmove.DAO.Generic.Class;
using Playmove.DAO.Models;

namespace Playmove.DAO.Service
{
    public class RepositoryFornecedor : Repository<Fornecedor, FornecedoresDbContext>, IRepositoryFornecedor
    {
        public RepositoryFornecedor() : base() { }

        public RepositoryFornecedor(FornecedoresDbContext context) : base(context) { }
        
    }
}
