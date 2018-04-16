using BeitechWebApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BeitechWebApi.Jwt
{
	public class ApiModelStateValidationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var ctxArgs = actionContext.ActionArguments;
			var ctxMods = actionContext.ModelState;
			var ctxArgsCount = ctxArgs.Count;
			var ctxArgs_Model = ctxArgs.FirstOrDefault();

			//IEnumerable<string> argsIn = ctxArgs.ToList().Select(x => x.Key);
			//if (ctxArgs.Count > 0 && ctxArgs_Model.IsNotNull())
			//{
			//	string asd = argsIn.FirstOrDefault();
			//	ctxArgs_Model = ctxArgs.FirstOrDefault(x => x.Key == asd);
			//}

			ctxArgs["ModelValidations"] = string.Empty;
			List<string> mensajes = new List<string>();

			if (ctxArgsCount > 0 && ctxArgs_Model.Value.IsNull())
			{
				//actionContext.ActionArguments["ModelValidations"] = "No se ha enviado el módelo de datos";
				actionContext.ModelState.AddModelError("ModelValidations", "No se ha enviado el módelo de datos");
			}

			if (!ctxMods.IsValid)
			{
				foreach (var item in ctxMods)
				{
					var mensajeValidacion = string.Empty;

					var errores = item.Value.Errors;
					if (errores.Count > 0)
					{
						mensajeValidacion = errores.LastOrDefault().ErrorMessage.Trim();

						if (string.IsNullOrEmpty(mensajeValidacion))
						{
							if (errores.LastOrDefault().Exception.IsNotNull())
							{
								mensajeValidacion = errores.LastOrDefault().Exception.Message;
							}
						}

						mensajes.Add(mensajeValidacion);
					}
					//message = !string.IsNullOrEmpty(message) ? message : errores.LastOrDefault().Exception.Message;
				}
				actionContext.ActionArguments["ModelValidations"] = string.Join(", ", mensajes.ToArray());
			}

			base.OnActionExecuting(actionContext);
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			base.OnActionExecuted(actionExecutedContext);
		}
	}
}
