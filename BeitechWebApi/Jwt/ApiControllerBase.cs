using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using BeitechWebApi.Extensions;

namespace BeitechWebApi.Jwt
{
	public class ApiControllerBase : ApiController
	{
		public HttpContext CurrentContext;
		public HttpRequest CurrentRequest;
		public HttpResponse CurrentResponse;
		private ClaimsPrincipal _currentUser;
		private IDictionary<string, string> _userProfile;
		static string argumentValidation = "ModelValidations";
		public static string modelErrorTitle = "Validación modelo de datos";
		static string reasonPhrase = "Error en modelo de datos";
		//public static string modelMensajeExitosoTitle = "Operación exitosa";
		//public static string modelMensajeNOExitosoTitle = "Operación no exitosa";

		public ApiControllerBase()
		{
			CurrentContext = HttpContext.Current;
			CurrentRequest = CurrentContext.Request;
			CurrentResponse = CurrentContext.Response;
			_currentUser = CurrentContext.User as ClaimsPrincipal;

			FillUserProfile();

		}

		private void FillUserProfile()
		{
			_userProfile = new Dictionary<string, string>();
			var userProfileCaim = CurrentUserPrincipal.FindFirst(delegate (Claim it) { return it.Value == "userprofile"; });
			if (userProfileCaim.IsNotNull())
			{
				_userProfile = userProfileCaim.Properties;
			}
		}

		public string ModelStateErrorMessage
		{
			get
			{
				object objReturn = string.Empty;
				var actArgs = ActionContext.ActionArguments;
				if (actArgs.ContainsKey(argumentValidation))
				{
					actArgs.TryGetValue(argumentValidation, out objReturn);
				}
				return objReturn.ToString();
			}
		}

		public ClaimsPrincipal CurrentUserPrincipal
		{
			get
			{
                return _currentUser;
                //return _currentUser ?? Request.GetOwinContext().Authentication.User;
			}
			private set
			{
				_currentUser = value;
			}
		}

		public string CurrentUserId
		{
			get
			{
                return "";
				//return CurrentUserPrincipal.Identity.GetUserId();
			}
		}

		public string CurrentUserName
		{
			get
			{
                return "";
               // return CurrentUserPrincipal.Identity.GetUserName();
			}
		}

		public string CurrentUserFullName
		{
			get
			{
				return CurrentUserPrincipal.FindFirst(ClaimTypes.Surname).Value;
			}
		}

		public IDictionary<string, string> CurrentUserProfile
		{
			get
			{
				return _userProfile;
			}
		}

		public short CurrentAgencyId
		{
			get
			{
				//return CurrentUserPrincipal.FindFirst(delegate (Claim it) { return it.Properties.Count > 0 && it.Properties.ContainsKey("agencyid"); }).Value;
				return (_userProfile.ContainsKey("agencyid")) ? short.Parse(_userProfile["agencyid"]) : short.MinValue;
			}
		}

		public string GePlainTextClaims()
		{
			var serializer = new JavaScriptSerializer();
			StringBuilder serializedResult = new StringBuilder();
			//serializedResult.Append("{claims:[");
			//var serializedResult = serializer.Serialize(CurrentUserPrincipal.Claims);
			foreach (var item in CurrentUserPrincipal.Claims)
			{
				//serializedResult.Append(serializer.Serialize(item));
				//var itemSerial = string.Format("");
				//serializedResult.AppendFormat("type:{0},value:{1},valuetype:{2},properties:{3}", item.Type, item.Value, item.ValueType, serializer.Serialize(item.Properties));
				serializedResult.AppendFormat("type={1},value={2},properties={3}{0}", System.Environment.NewLine, item.Type, item.Value, serializer.Serialize(item.Properties));
			}
			//serializedResult.Append("]}");
			return serializedResult.ToString();
		}
		protected internal virtual IHttpActionResult Unathorized<T>(string reason, T data)
		{
			var responseMessage = Request.CreateResponse(HttpStatusCode.Unauthorized, data);
			responseMessage.ReasonPhrase = reason;
			return ResponseMessage(responseMessage);

		}

		//public IHttpActionResult Unathorized(string reason)
		//{
		//	var responseMessage = Request.CreateResponse(HttpStatusCode.Unauthorized);
		//	responseMessage.ReasonPhrase = reason;
		//	return ResponseMessage(responseMessage);

		//}

		protected internal virtual IHttpActionResult BadRequestModelErrorOut(string data, short statusCode = default(short))
		{
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,new { }));
		}

		protected internal virtual IHttpActionResult BadRequest<T>(T data)
		{
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, data));
		}

		protected internal virtual IHttpActionResult BadRequest<T>(string reason, T data)
		{
			var responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest, data);
			responseMessage.ReasonPhrase = reason;
			return ResponseMessage(responseMessage);
		}

		protected internal virtual IHttpActionResult BadRequestModelState()
		{
			var responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest, new { });
			responseMessage.ReasonPhrase = reasonPhrase;
			return ResponseMessage(responseMessage);
		}

	}
}
