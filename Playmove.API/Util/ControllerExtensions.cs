using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Playmove.DAO.Generic.Interface;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Intrinsics.X86;
using System.Xml.Serialization;
using System.Xml;
using System.Net;
using System.ComponentModel;

namespace Playmove.Util
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ResponseObject<T>
    {
        public bool? IsRequestSuccessful { get; set; }
        public int StatusCode { get; set; }

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        [JsonIgnore]
        public bool? ShowSuccessMessage { get; set; }

        public T Payload { get; set; }

    }


    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        public static T GetSessionObject<T>(this Controller controller, string objectName)
        {

            var requestObjectSession = controller.HttpContext.Session.GetString(objectName);
            return JsonConvert.DeserializeObject<T>(requestObjectSession);
        }
        public static string GetSessionObject(this Controller controller, string objectName)
        {

            return controller.HttpContext.Session.GetString(objectName);

        }

        public static void SetSessionObject<T>(this Controller controller, object sessionData, string sessionKey)
        {
            controller.HttpContext.Session.SetString(sessionKey, JSONExtensions.StringfyFromObject(sessionData));

        }

        public static T CreateVariableFromSession<T>(this Controller controller, string name)
        {
            JObject requestObjectSession;
            try
            {
                requestObjectSession = JObject.Parse(controller.HttpContext.Session.GetString("requestObject"));
            }
            catch (Exception e)
            {
                throw new AggregateException(e);

            }
            return requestObjectSession.GetValue(name).ToObject<T>();
        }
        public static ResponseObject<string> CreateResponseObject(bool responseSuccessfulValue, string? successMessage = null, string? errorMessage = null, string? redirectTo = null, HttpStatusCode successStatus = HttpStatusCode.OK, HttpStatusCode errorStatus = HttpStatusCode.BadRequest)
        {
            return new ResponseObject<string>
            {
                Message = successMessage,
                ErrorMessage = errorMessage,
                StatusCode = responseSuccessfulValue ? (int)successStatus : (int)errorStatus

            };
        }
        public static ResponseObject<T> CreateResponseObject<T>(this Controller controller, bool responseSuccessfulValue, T payload, string successMessage = "", string errorMessage = "", string redirectTo = "", HttpStatusCode successStatus = HttpStatusCode.OK, HttpStatusCode errorStatus = HttpStatusCode.BadRequest)
        {
            return new ResponseObject<T>
            {
                Message = successMessage,
                ErrorMessage = errorMessage,
                StatusCode = responseSuccessfulValue ? (int)successStatus : (int)errorStatus,

                Payload = payload
            };
        }

        public static JsonResult CreateResponse(this Controller controller, bool responseSuccessfulValue, string? successMessage = null, string? errorMessage = null, string? redirectTo = null, HttpStatusCode successStatus = HttpStatusCode.OK, HttpStatusCode errorStatus = HttpStatusCode.BadRequest)
        {
            return new JsonResult(new ResponseObject<string>
            {
                Message = successMessage,
                ErrorMessage = errorMessage,
                StatusCode = responseSuccessfulValue ? (int)successStatus : (int)errorStatus

            });
        }
        public static JsonResult CreateResponse<T>(this Controller controller, bool responseSuccessfulValue, T payload, string? successMessage = null, string? errorMessage = null, string? redirectTo = null, HttpStatusCode successStatus = HttpStatusCode.OK, HttpStatusCode errorStatus = HttpStatusCode.BadRequest)
        {
            return new JsonResult(new ResponseObject<T>
            {
                Message = successMessage,
                ErrorMessage = errorMessage,
                Payload = payload,
                StatusCode = responseSuccessfulValue ? (int)successStatus : (int)errorStatus


            });
        }

    }
}
