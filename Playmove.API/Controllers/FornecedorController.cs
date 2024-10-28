using Playmove.DAO.Models;
using Playmove.DAO.Service;
using Playmove.DAO.Generic.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;
using System.Text;
using Playmove.Util;
using Playmove.API.ViewModel;
using BaseProject.API.Util;
using Playmove.API.Util;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net;

namespace Playmove.API.Controllers
{
    [Authorize]
    [Route("api/fornecedores")]
    public class FornecedorController : CrudController<Fornecedor, IServiceFornecedor, FornecedorVM>
    {

        private readonly IServiceFornecedor _serviceFornecedor;

        public FornecedorController(IServiceFornecedor serviceFornecedor)
                : base(serviceFornecedor)
        {
            this.DefaultNavigationMapping = "";
            _serviceFornecedor = serviceFornecedor;

        }

        /// <summary>
        /// Obtém todos os fornecedores cadastrados no sistema.
        /// </summary>
        /// <remarks>
        /// O metodo retorna um objeto JSON com duas proprieades: uma contagem total de fornecedores e a lista com todos os fornecedores
        /// </remarks>
        /// <returns>Objeto JSON com duas propriedades: total de fornecedores (int) e Lista de fornecedores (Fornecedor).
        /// </returns>
        /// <response code="200">Lista de fornecedores encontrada com sucesso.
        /// </response>
        /// <response code="500">Erro interno no servidor.
        /// </response>


        [HttpGet(Order = -1)]
        public IActionResult Fornecedores()

        {
            return base.Listar(base.Service);

        }


        /// <summary>
        /// Faz a busca de um fornecedor no sistema.
        /// </summary>
        /// <remarks>
        /// O metodo realiza a busca de um novo fornecedor no sistema usando seu ID como referência. Alguns campos retornados não possuem formatação ou pontuação.
        /// </remarks>
        /// <returns>Objeto do tipo "Fornecedor".
        /// </returns>
        /// <response code="200">Fornecedor encontrado com sucesso.
        /// </response>       
        /// <response code="404">Fornecedor não encontrado no sistema.
        /// </response>
        /// <response code="401">Requisição não autorizada pelo servidor.
        /// </response>
        /// <response code="500">Erro interno no servidor.
        /// </response>

        [HttpGet("{id:int}")]
        public IActionResult Fornecedores([FromRoute] int id)
        {
            return base.Visualizar(id, base.Service);

        }

        /// <summary>
        /// Realiza o cadastro de um novo fornecedor no sistema.
        /// </summary>
        /// <remarks>
        /// O metodo realiza o cadastro de um novo fornecedor no sistema. Os campos são validados automaticamente seguindo padrões de formatação e, caso estejam formatados incorretamente, o sistema não executará a inclusão e notificara os campos a serem corrigidos; 
        /// o sistema também não permite a inclusão de um CNPJ já cadastrado anteriormente
        /// </remarks>
        /// <response code="200">Fornecedor cadastrado com sucesso.
        /// </response>       
        /// <response code="400">Erro em um ou mais campos de cadastro.
        /// </response>
        ///     /// <response code="401">Requisição não autorizada pelo servidor.
        /// </response>
        /// <response code="500">Erro interno no servidor.
        /// </response>

        [HttpPost]
        public IActionResult Fornecedores([FromBody] FornecedorVM model)
        {
            if (!FieldValidation.IsCnpjValid(model.Cnpj))
                return this.CreateResponse(
                    false,
                    errorMessage: "O CNPJ inserido não é válido!"

                    );

            if (base.Service.ExisteCNPJ(model.NormalizeCNJP()))
                return this.CreateResponse(
                    false,
                    errorMessage: "O CNPJ inserido ja foi cadastrado no sistema anteriormente!"

                    );


            bool sucesso = base.Service.Adicionar(model.Cast(model));
            return this.CreateResponse(
                sucesso,
                successMessage: sucesso ? "Sucesso ao adicionar as informações!" : null,
                errorMessage: !sucesso ? "Erro ao adicionar as informações, verifique as informações inseridas e tente novamente mais tarde!" : null
                );
        }


        /// <summary>
        /// Realiza a atualização das informações de um fornecedor no sistema
        /// </summary>
        /// <remarks>
        /// O metodo realiza a atualização de informações de um novo fornecedor no sistema. Os campos são validados automaticamente seguindo padrões de formatação e, caso estejam formatados incorretamente, o sistema não executará a alteração e notificara os campos a serem corrigidos
        /// o sistema também não permite a inclusão de um CNPJ já cadastrado anteriormente, mas permite que o CNPJ de um fornecedor seja alterado desde que mantenha a integridade do sistema.
        /// </remarks>
        /// <response code="200">Fornecedor atualizado com sucesso.
        /// </response>       
        /// <response code="400">Erro em um ou mais campos de cadastro.
        /// </response>
       /// <response code="401">Requisição não autorizada pelo servidor.
        /// </response>
        /// <response code="500">Erro interno no servidor.
        /// </response>

        [HttpPut("{id:int}")]
        public IActionResult Fornecedores([FromRoute] int id, [FromBody] FornecedorVM model)
        {
            if (!Service.Existe(id))
                return this.CreateResponse(
                    false,
                    errorMessage: "O Id enviado não foi encontrado no sistema, verifique a informação e tente novamente!",
                    errorStatus: HttpStatusCode.NotFound);

            if (!FieldValidation.IsCnpjValid(model.Cnpj))
                return this.CreateResponse(
                    false,
                    errorMessage: "O CNPJ inserido não é válido!"

                    );
            string cnpjNormalizado = model.NormalizeCNJP();
            string cnpjCadastrado = base.Service.BuscarCNPJ(model.Id);

            if (base.Service.ExisteCNPJ(model.NormalizeCNJP()) && !cnpjCadastrado.Equals(cnpjNormalizado))
                return this.CreateResponse(
                    false,
                    errorMessage: "O CNPJ inserido ja foi cadastrado no sistema anteriormente!"

                    );

            model.Id = id;

            return base.Editar(id, model, base.Service);


        }

        /// <summary>
        /// Faz a exclusão de um fornecedor no sistema.
        /// </summary>
        /// <remarks>
        /// O metodo realiza a exclusão de um fornecedor no sistema usando seu ID como referência
        /// </remarks>
        /// <response code="200">Fornecedor excluido com sucesso!.
        /// </response>       
        /// <response code="404">Fornecedor não encontrado no sistema.
        /// </response>
        /// <response code="401">Requisição não autorizada pelo servidor.
        /// </response>
        /// <response code="500">Erro interno no servidor.
        /// </response>

        [HttpDelete("{id:int}")]

        public IActionResult _Fornecedores([FromRoute] int id)
        {
            return base.Excluir(id);


        }
    }



}