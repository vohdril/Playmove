
using Playmove.DAO.Generic.Interface;
using Playmove.DAO.Models;
using Playmove.DAO.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BaseProject.DAO.Models.Partial;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BaseProject.API.Util;
namespace Playmove.Util
{
    [ModelStateValidationActionFilter]
    [Route("[controller]/[action]")]
    public class CrudController<TEntity, TService, TViewModel> : Controller
        where TEntity : class
        where TViewModel : GenericViewModel<TEntity, TViewModel>, new()
        where TService : IService<TEntity>
    {

        public string DefaultNavigationMapping { get; set; } = "";

        public TViewModel ViewModel { get; set; } = new TViewModel();
        public TService Service { get; set; }
        public CrudController(TService _service)
        {
            Service = _service;

        }


        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpPost(Order = 1)]
        public virtual IActionResult Adicionar([FromBody] TEntity model)
        {

            bool sucesso = Service.Adicionar(model);
            return this.CreateResponse(
                sucesso,
                payload: model,
                successMessage: sucesso ? "Sucesso ao adicionar as informações!" : "",
                errorMessage: !sucesso ? "Erro ao adicionar!" : ""
                );
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpPut(Order = 1)]
        public virtual IActionResult Editar([FromQuery] int id, [FromBody] TViewModel model, [FromServices] TService service)
        {

            if (!Service.Existe(id))
                return this.CreateResponse(
                    false,
                    errorMessage: "O Id enviado não foi encontrado no sistema, verifique a informação e tente novamente!",
                    errorStatus: HttpStatusCode.NotFound);

            bool sucesso = Service.Editar(ViewModel.Cast(model));
            return this.CreateResponse(
                sucesso,
                successMessage: sucesso ? "Sucesso ao editar as informações!" : null,
                errorMessage: !sucesso ? "Erro ao editar, verifique as informações e tente novamente! Se persitir o erro, contate a administração do sistema!" : null);
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpGet(Order = 1)]
        public virtual IActionResult Visualizar([FromRoute] int id, [FromServices] TService service)
        {

            TEntity model = Service.ObterPorId(id, includeProperties: DefaultNavigationMapping);

            if (model == null)
                return this.CreateResponse(
                    false,
                    errorMessage: "O Id enviado não foi encontrado no sistema, verifique a informação e tente novamente!",
                    errorStatus: HttpStatusCode.NotFound);


            return this.CreateResponse(true, payload: ViewModel.Cast(model));
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpPut(Order = 1)]
        public virtual IActionResult Ativar([FromQuery] int id, [FromServices] TService service)
        {
            dynamic model = Service.ObterPorId(id);

            if (model == null)
                return this.CreateResponse(
                    false,
                    errorMessage: "O Id enviado não foi encontrado no sistema, verifique a informação e tente novamente!",
                    errorStatus: HttpStatusCode.NotFound);

            model.Ativo = !model.Ativo;

            string mensagemSucesso = model.Ativo ? "Sucesso ao ativar!" : "Sucesso ao desativar!";
            string mensagemErro = model.Ativo ? "Erro ao ativar!" : "Erro ao desativar!";

            bool sucesso = Service.Editar(model);
            return this.CreateResponse(
                   sucesso,
                   successMessage: mensagemSucesso,
                   errorMessage: mensagemErro);
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpDelete(Order = 1)]

        public virtual IActionResult Excluir([FromRoute] int id)
        {
            if (!Service.Existe(id))
                return this.CreateResponse(
                    false,
                    errorMessage: "O Id enviado não foi encontrado no sistema, verifique a informação e tente novamente!",
                    errorStatus: HttpStatusCode.NotFound);

            bool sucesso = Service.Deletar(id);
            return this.CreateResponse(
                sucesso,
                successMessage: sucesso ? "Sucesso ao excluir as informações" : null,
                errorMessage: !sucesso ? "Erro ao excluir, verifique as informações e tente novamente! Se persitir o erro, contate a administração do sistema!" : null);

        }
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpGet(Order = 1)]
        public virtual IActionResult Listar([FromServices] TService service)
        {
            List<TEntity> model = service.ObterTodos(DefaultNavigationMapping).ToList();

            return this.CreateResponse(true, payload: new
            {
                total_encontrado = model.Count,
                resultados = model.Select(x => ViewModel.Cast(x))
            });
        }
    }



}

