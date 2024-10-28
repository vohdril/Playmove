using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Playmove.API.Models;
using Playmove.DAO.IService;
using Playmove.DAO.Models;
using Playmove.DAO.Service.Token;
using Playmove.Util;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Playmove.API.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<IdentityUser> _userManager;
        private readonly IServiceToken _serviceToken;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public AccountController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,

            IServiceToken serviceToken,
            SignInManager<IdentityUser> signInManager


            )
        {


            _logger = logger;
            _userManager = userManager;
            _serviceToken = serviceToken;
            _signInManager = signInManager;
            _roleManager = roleManager;



        }
        /// <summary>
        /// Faz a autenticação do usuário na API
        /// </summary>
        /// <remarks>
        /// Realiza uma autenticação baseada em JWT. As informações ficam armazenadas no servidor, então a parte do cliente já possui as configurações necessárias, 
        /// tendo como 5 minutos a duração do acesso. Os campos são validados automaticamente seguindo padrões de formatação e, caso estejam formatados incorretamente, o sistema não executará a alteração e notificara os campos a serem corrigidos
        /// </remarks>
        /// <response code="200">Usuário autenticado com sucesso!.
        /// </response>       
        /// <response code="400">Informações de usuário incorretas.
        /// </response>

        [HttpPost("AutenticarUsuario")]
        public async Task<IActionResult> Autenticar([FromBody] AuthenticationVM model)
        {
            if (!ModelState.IsValid) return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: "Dados inválidos!"));

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: "Email ou senha incorreto!"));

            if (await _userManager.IsLockedOutAsync(user))
            {
                var now = DateTimeOffset.Now;
                var end = user.LockoutEnd.Value;
                var minutes = Math.Ceiling((end - now).TotalMinutes);

                return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: $"Devido a várias tentativas de acesso, a sua conta foi bloqueada. Tente novamente em {minutes + (minutes == 1 ? " minuto" : " minutos")}!"));
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

            if (!result.Succeeded)
            {
                return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: "Email ou senha incorreto!"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _serviceToken.GenerateToken(user, roles.ToList());

            Response.Cookies.Delete("redirectUrl");
            Response.Cookies.Append("access_token", token);

            return Json(this.CreateResponseObject<string>(true, "bearer :" + token));
        }

        /// <summary>
        /// Limpa a autenticação da API
        /// </summary>
        /// <remarks>
        /// Remove as informações do token JWT do servidor, bloqueando o acesso não autorizado
        /// </remarks>
        ///    /// <response code="200">Autenticação de acesso limpada com sucesso.
        /// </response>       
        /// <response code="500">Erro inesperado no servidor
        /// </response>
        [HttpDelete]
        [Route("LimparCookies")]
        public async Task<JsonResult> LimparCookies()
        {

            Response.Cookies.Delete("access_token");
            return Json(this.CreateResponseObject<string>(true, "Cookie removido."));


        }

        /// <summary>
        /// Cadastra um novo usuario na API
        /// </summary>
        /// <remarks>
        /// Cadastra um novo usuário no sistema, autorizando acesso às demais áreas do sistema. Os campos são validados automaticamente seguindo padrões de formatação e, caso estejam formatados incorretamente, o sistema não executará a alteração e notificara os campos a serem corrigidos
        /// </remarks>
        /// <response code="200">Usuario cadastrado com sucesso.
        /// </response>       
        /// <response code="500">Erro inesperado no servidor
        /// </response>

        [HttpPost]
        [Route("CriarUsuario")]
        [ModelStateValidationActionFilter]

        public async Task<JsonResult> CriarUsuario([FromBody] UserRegisterVM model)
        {

            IdentityUser user = new IdentityUser();
            user.UserName = model.Nome;
            user.Email = model.Email;
            string userPWD = model.Password;

            IdentityResult result = await _userManager.CreateAsync(user, userPWD);

            return ResolveIdentityResult(result, "Usuário criado com sucesso!");

        }

        /// <summary>
        /// Cadastra uma nova "Role" na API
        /// </summary>
        /// <remarks>
        /// Cadastra uma nova "Role" (papel) no sistema. 
        /// </remarks>
        /// <response code="200">Role cadastrada com sucesso.
        /// </response>       
        /// <response code="500">Erro inesperado no servidor
        /// </response>
        [HttpPost]
        [Route("AdicionarRole")]
        [ModelStateValidationActionFilter]
        public async Task<JsonResult> AddRole([FromBody] RoleVM role)
        {

            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole() { Name = role.Nome });
            return ResolveIdentityResult(result, "Role criada com sucesso!");

        }

        /// <summary>
        /// Atribui uma "Role" para um usuário da API
        /// </summary>
        /// <remarks>
        /// Atribui uma "role" existente para um usuário. Os usuários podem apenas acessar as areas dos sistemas atribuidas às suas "roles". Por padrão, apenas é necessario um usuario autenticado para acessar, podendo ser configurado posteriormente nos arquivos e ações das controladoras
        /// </remarks>
        /// <response code="200">Role atribuída ao usuário cadastrado com sucesso.
        /// </response>       
        /// <response code="500">Erro inesperado no servidor
        /// </response>
        [HttpPut]
        [ModelStateValidationActionFilter]
        [Route("AdicionarRoleAoUsuario")]

        public async Task<JsonResult> AddRoleToUser([FromBody] UserRoleVM userRole)
        {

            return this.CreateResponse(true, (ResolveUserRole(userRole, (int)UserRoleAction.Adicionar)));

        }

        /// <summary>
        /// Remove uma "Role" atribuída de um usuário da API
        /// </summary>
        /// <remarks>
        /// remove uma "role" existente de um usuario.
        /// </remarks>
        /// <response code="200">Role removida do usuário cadastrado com sucesso.
        /// </response>       
        /// <response code="500">Erro inesperado no servidor
        /// </response>

        [HttpDelete]
        [ModelStateValidationActionFilter]
        [Route("RemoverRoleDoUsuario")]

        public async Task<JsonResult> RemoveRoleToUser([FromBody] UserRoleVM userRole)
        {

            return this.CreateResponse(true, (ResolveUserRole(userRole, (int)UserRoleAction.Remover)));



        }


        #region Metodos Privados
        [NonAction]
        private JsonResult ResolveIdentityResult(IdentityResult result, string successMessage)
        {
            if (!result.Succeeded)
            {
                string[] errorsArray = result
                    .Errors
                    .Select(x => x.Description)
                    .ToArray();

                return this.CreateResponse(false, errorMessage: "\"Houve erro na requisição, verifique: '\"" + string.Join(", ", errorsArray) + "\', se o erro peristir contate o suporte!\"");
            }
            return this.CreateResponse(true, successMessage: successMessage);


        }
        [NonAction]

        private JsonResult ResolveUserRole(UserRoleVM model, int actionType)
        {
            IdentityUser user = _userManager.FindByEmailAsync(model.UserEmail).Result;

            if (user == null)
                return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: "Email ou senha incorreto!"));

            IdentityRole role = _roleManager.FindByNameAsync(model.RoleName).Result;
            if (role == null)
                return Json(ControllerExtensions.CreateResponseObject(false, errorMessage: "Email ou senha incorreto!"));

            IdentityResult result = new IdentityResult();
            string message = "";

            switch (actionType) {

                case (int)UserRoleAction.Adicionar:
                    result = _userManager.AddToRoleAsync(user, model.RoleName).Result;
                    message = "Role atribuída ao usuário com sucesso!";
                   break;

                case (int)UserRoleAction.Remover:
                   result = _userManager.RemoveFromRoleAsync(user, model.RoleName).Result;
                    message = "Role removida do usuário com sucesso!";

                    break;

            }

            return ResolveIdentityResult(result, message);


        }


        private enum UserRoleAction
        {

            [Display(Name = "Criação")]
            Adicionar = 1,
            [Display(Name = "Remover")]
            Remover = 2



        }
        #endregion

    }
}
