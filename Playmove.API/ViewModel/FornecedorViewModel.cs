using BaseProject.DAO.Models.Partial;
using Playmove.DAO.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Playmove.API.ViewModel
{
    public class FornecedorVM : GenericViewModel<Playmove.DAO.Models.Fornecedor, FornecedorVM>
    {


        public int Id { get; set; }
        [Required(ErrorMessage = "É necessário incluir um nome!")]
        public string Nome { get; set; }


        [Required(ErrorMessage = "É necessário incluir um email válido!")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",
            ErrorMessage = "Insira um email válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É necessário incluir um CNPJ!")]
        [RegularExpression("[0-9]{2}\\.?[0-9]{3}\\.?[0-9]{3}\\/?[0-9]{4}\\-?[0-9]{2}",
            ErrorMessage = "Insira um CNPJ válido!")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "É necessário incluir um telefone!")]
        [RegularExpression("^1\\d\\d(\\d\\d)?$|^0800 ?\\d{3} ?\\d{4}$|^(\\(0?([1-9a-zA-Z][0-9a-zA-Z])?[1-9]\\d\\) ?|0?([1-9a-zA-Z][0-9a-zA-Z])?[1-9]\\d[ .-]?)?(9|9[ .-])?[2-9]\\d{3}[ .-]?\\d{4}$", ErrorMessage = "Insira um telefone válido!")]
        public string Telefone { get; set; }
        [Required(ErrorMessage = "É necessário incluir um endereço!")]

        public string Endereco { get; set; }
        [Required(ErrorMessage = "É necessário incluir a cidade!")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "É necessário incluir o estado (UF)!")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "É necessário incluir um CEP!")]
        [RegularExpression("(^[0-9]{5})-?([0-9]{3}$)",
            ErrorMessage = "Insira um CEP válido!")]
        public string Cep { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Status { get; set; }


        public FornecedorVM()
        : base()
        {

            DataCadastro = DateTime.Now;
            Status = false;
        }

        public FornecedorVM(Fornecedor model)
            : base()
        {
            Id = model.Id;
            Nome = model.Nome;
            Email = model.Email;
            Cnpj = model.Cnpj;
            Telefone = model.Telefone;
            Endereco = model.Endereco;
            Cep = model.Cep;
            Telefone = model.Telefone;
            DataCadastro = model.DataCadastro;
            Status = model.Status;

        }


        public override FornecedorVM Cast(Fornecedor model)
        {
            return new FornecedorVM()
            {
                Id = model.Id,
                Nome = model.Nome,
                Email = model.Email,
                Cnpj = model.Cnpj,
                Endereco = model.Endereco,
                Cep = model.Cep,
                Telefone = model.Telefone,
                DataCadastro = model.DataCadastro,
                Status = model.Status,
            };

        }


        public override Fornecedor Cast(FornecedorVM model)
        {
            return new Fornecedor()
            {
                Id = model.Id,
                Nome = model.Nome,
                Email = model.Email,
                Cnpj = model.Cnpj
                .Replace(".", "")
                .Replace("/", "")
                .Replace("-", ""),
                Telefone = model.Telefone
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "")
                    .Replace(" ", ""),

                Endereco = model.Endereco,
                Cidade = model.Cidade,
                Estado = model.Estado,
                Cep = model.Cep.Replace("-", ""),
                DataCadastro = model.DataCadastro,
                Status = model.Status,
            };

        }

        public string NormalizeCNJP(){
            return Cnpj
                    .Replace(".", "")
                    .Replace("/", "")
                    .Replace("-", "");


        }



    }
}
