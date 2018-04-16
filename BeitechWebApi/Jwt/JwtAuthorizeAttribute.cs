using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BeitechWebApi.Jwt
{
    public class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;

            if (headers.Authorization != null && headers.Authorization.Scheme == "Bearer")
            {
                var errAuth = GetAuthorizationError(headers);
                if (!string.IsNullOrWhiteSpace(errAuth))
                {
                    PutUnauthorizedResult(actionContext, errAuth);
                }
            }
            else
            {
                // No hay el header Authorization
                PutUnauthorizedResult(actionContext, "Solicitud rechazada, Token no enviado");
            }
        }

        private void PutUnauthorizedResult(HttpActionContext actionContext, string msg)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, msg);
        }

        private string GetAuthorizationError(System.Net.Http.Headers.HttpRequestHeaders headers)
        {
            try
            {
                headers.TryGetValues("UnauthorizedMessage", out IEnumerable<string> unauthorizedMessages);

                if (unauthorizedMessages != null && unauthorizedMessages.Count() > 0)
                {
                    return unauthorizedMessages.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

    }

    public class BasicAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            if (headers.Authorization != null && headers.Authorization.Scheme == "Bearer")
            {
                try
                {
                    var userPwd = Encoding.UTF8.GetString(Convert.FromBase64String(headers.Authorization.Parameter));
                    var user = userPwd.Substring(0, userPwd.IndexOf(':'));
                    var password = userPwd.Substring(userPwd.IndexOf(':') + 1);
                    // Validamos user y password (aquí asumimos que siempre son ok)
                }
                catch (Exception)
                {
                    PutUnauthorizedResult(actionContext, "Invalid Authorization header");
                }
            }
            else
            {
                // No hay el header Authorization
                PutUnauthorizedResult(actionContext, "Auhtorization needed");
            }
        }

        private void PutUnauthorizedResult(HttpActionContext actionContext, string msg)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(msg)
            };
        }
    }
}