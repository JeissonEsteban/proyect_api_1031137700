using BeitechWebApi.Extensions;
using Jose;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace BeitechWebApi.Jwt
{
    public class ApplicationJjwtContext : IDisposable
    {
        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        static string JWTSecretKeyName = "JWTSecretKeyShared";
        //static string JWTSessionMinutesKey = "JWTSessionMinutes";
        public static string KeyName_App = string.Format("App_{0}", JWTSecretKeyName);
        public static string KeyName_AppMessage = string.Format("App_{0}_Exists", JWTSecretKeyName);
        public static string KeyName_AppSessionMinutes = string.Format("App_{0}_SessionMinutes", JWTSecretKeyName);

        public static string SecretKey
        {
            get
            {
                return "SYEDEV_ZlcxTWV2Z1puTlBsVHFJOE1ycnk=";
            }
            private set
            {
            }
        }

        public static string SecretKeyMessage
        {
            get
            {
                return string.Format("{0}", HttpContext.Current.Application.Get(KeyName_AppMessage));
            }
            private set
            {
            }
        }

        public static short SessionMinutes
        {
            get
            {
                return 30;
            }
            private set
            {
            }
        }


        public ApplicationJjwtContext(){ }



        public static ApplicationJjwtContext Create()
        {
            string applicationKeyString = string.Format("{0}", HttpContext.Current.Application.Get(JWTSecretKeyName));
            var settingKeyString = new ApplicationJjwtContext();
            return settingKeyString;
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ApplicationJjwtContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        #endregion
    }

    public class TokenModelIn
    {
        public virtual string UserId { get; set; }

        public string UserFullName { get; set; }

        public short SessionMinutes { get; set; }
    }

    public class TokenModelOut //: TokenModelIn
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public static class JjwtTokens
    {
        //static int versionToken = 1;
        const int constVersionToken = 1;
        public static TokenModelOut CreateToken(string clientApiKey, TokenModelIn ModelData)
        {
            var tokenReturn = new TokenModelOut()
            {
                //IdUsuario = ModelData.IdUsuario,
                //Usuario = ModelData.Usuario,
                //NombreUsuario = ModelData.NombreUsuario,
                //Token = clientToken,
            };

            CreateToken(clientApiKey, ModelData, ref tokenReturn);

            return tokenReturn;
        }

        internal static bool CreateToken(string apikey, TokenModelIn modelDataIn, ref TokenModelOut modelDataOut)
        {
            var unixEpoch = FromUnixTime(0);
            var expiry = Math.Round((DateTime.UtcNow.AddMinutes(modelDataIn.SessionMinutes) - unixEpoch).TotalSeconds);
            var issuedAt = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            var notBefore = Math.Round((DateTime.UtcNow.AddMonths(6) - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>
            {
                {"version", constVersionToken},
                {"userid", modelDataIn.UserId},
                {"surname", modelDataIn.UserFullName},
                {"role", "User"},
                {"nbf", notBefore},
                {"iat", issuedAt},
                {"exp", expiry},
				//{"sub:userid", ModelDataIn.UserId},
				//{"sub:username", ModelDataIn.UserName},
				//{"sub:fullname", ModelDataIn.UserFullName},
            };

            byte[] apikeyObj = GetBytesSecretKey(apikey);
            var token = JWT.Encode(payload, apikeyObj, JwsAlgorithm.HS256);
            modelDataOut.Token = token;
            return true;
        }

        public static Dictionary<string, object> GetTokenDictionary(string token, string clientApiKey)
        {
            Dictionary<string, object> payloadData;
            try
            {
                byte[] apikeyObj = GetBytesSecretKey(clientApiKey);
                var jsonSerializer = new JavaScriptSerializer();
                var payloadJson = JWT.Decode(token, apikeyObj);
                payloadData = jsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
            }
            catch (IntegrityException jiex)
            {
                //if (jiex.HResult == -2146233088)
                //{
                throw new ApplicationException(
                    string.Format(
                        "  Clave compartida JWT no válida.{0}  * La Clave JWT del servidor y de la aplicación son diferentes.",
                        Environment.NewLine
                    ),
                    jiex
                );
                //}
                //throw;
            }
            return payloadData;
        }

        public static TokenModelIn GetTokenModel(string token, string clientApiKey, short sessionMinutes)
        {
            var resultModel = new TokenModelIn()
            {
                SessionMinutes = sessionMinutes,
            };

            var payloadData = GetTokenDictionary(token, clientApiKey);

            if (payloadData != null)
            {
                foreach (var pair in payloadData)
                {
                    var claimType = pair.Key;

                    if (pair.Value is ArrayList source)
                    {
                        continue;
                    }

                    switch (pair.Key.ToLower())
                    {
                        case "userid":
                            resultModel.UserId = pair.Value.ToString();
                            break;
                        case "surname":
                            resultModel.UserFullName = pair.Value.ToString();
                            break;
                    }
                }
            }
            return resultModel;
        }

        public static ClaimsPrincipal ValidateToken(string token, byte[] secret, bool checkExpiration)
        {
            Dictionary<string, object> payloadData = new Dictionary<string, object>();
            /*
						Dictionary<string, object> payloadData;
						try
						{
							byte[] apikeyObj = GetBytesSecretKey(clientApiKey);
							var jsonSerializer = new JavaScriptSerializer();
							var payloadJson = JWT.Decode(token, apikeyObj);
							payloadData = jsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
						}
						catch (IntegrityException jiex)
						{
							//if (jiex.HResult == -2146233088)
							//{
							throw new ApplicationException(
								string.Format(
									"  Clave compartida JWT no válida.{0}  * La Clave JWT del servidor y de la aplicación son diferentes.",
									Environment.NewLine
								),
								jiex
							);
							//}
							//throw;
						}
			*/
            try
            {
                var jsonSerializer = new JavaScriptSerializer();
                var payloadJson = JWT.Decode(token, secret);
                payloadData = jsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
            }
            catch (IntegrityException jiex)
            {
                throw new JoseException(
                    string.Format(
                    "  Clave compartida JWT no válida.{0}  * La Clave JWT del servidor y de la aplicación son diferentes.",
                    Environment.NewLine
                    ),
                    jiex
                );
            }

            int version = 0;
            payloadData.TryGetValue("version", out object versionJbj);

            if (versionJbj != null)
            {
                int.TryParse(versionJbj.ToString(), out version);
            }

            if (payloadData != null && version != constVersionToken)
            {
                throw new JoseException(
                    string.Format("Versión de token obsoleto. Versión enviada: {0}; Actual: {1}", version, constVersionToken)
                );
            }

            if (payloadData != null && (checkExpiration && payloadData.TryGetValue("exp", out object exp)))
            {
                var validTo = FromUnixTime(long.Parse(exp.ToString()));
                if (DateTime.Compare(validTo, DateTime.UtcNow) <= 0)
                {
                    throw new JoseException(
                        //string.Format("Token is expired. Expiration: '{0}'. Current: '{1}'", validTo, DateTime.UtcNow)
                        string.Format("Token ya expiró. Valido hasta [{0}], Hora del servidor", validTo.ToLocalTime())
                    );
                    //return null;
                }
            }

            var subject = new ClaimsIdentity("Federation", ClaimTypes.Name, ClaimTypes.Role);

            var claims = new List<Claim>();
            var userProfileClaim = new Claim(ClaimTypes.UserData, "userprofile", ClaimValueTypes.KeyInfo);

            if (payloadData != null)
            {
                foreach (var pair in payloadData)
                {
                    var claimType = pair.Key;

                    if (pair.Value is ArrayList source)
                    {
                        claims.AddRange(from object item in source
                                        select new Claim(claimType, item.ToString(), ClaimValueTypes.String));

                        continue;
                    }

                    switch (pair.Key.ToLower())
                    {
                        case "userid":
                            //claims.Add(new Claim(ClaimTypes.UserData, pair.Value.ToString(), ClaimValueTypes.Integer));
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                        case "name":
                            claims.Add(new Claim(ClaimTypes.Name, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                        case "surname":
                            claims.Add(new Claim(ClaimTypes.Surname, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                        case "email":
                            claims.Add(new Claim(ClaimTypes.Email, pair.Value.ToString(), ClaimValueTypes.Email));
                            break;
                        case "role":
                            claims.Add(new Claim(ClaimTypes.Role, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                        case "exp":
                            claims.Add(new Claim(ClaimTypes.Expiration, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                        case "sub:agencyid":
                            userProfileClaim.Properties.Add(new KeyValuePair<string, string>("agencyid", pair.Value.ToString()));
                            break;
                        default:
                            //claims.Add(new Claim(claimType, pair.Value.ToString(), ClaimValueTypes.String));
                            claims.Add(new Claim(ClaimTypes.UserData, pair.Value.ToString(), ClaimValueTypes.String));
                            break;
                    }
                }
            }
            claims.Add(userProfileClaim);
            subject.AddClaims(claims);
            return new ClaimsPrincipal(subject);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static Byte[] GetBytesSecretKey(string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new JoseException("No se ha podido obtener un secretKey valido.");
            }
            return Encoding.UTF8.GetBytes(secretKey);
        }
    }
    public class JjwtAuthMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;

            try
            {
                headers.TryGetValues("Authorization", out IEnumerable<string> authHeaderValues);

                if (authHeaderValues == null)
                {
                    return base.SendAsync(request, cancellationToken);
                }

                if (headers.Authorization.Scheme != "Bearer")
                {
                    return base.SendAsync(request, cancellationToken);
                }

                var token = authHeaderValues.ElementAt(0).Replace("Bearer", string.Empty).Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return Task.FromResult(request.CreateErrorResponse(HttpStatusCode.BadRequest, "Token no agregado al request"));
                }

                /* Assign current token in the header to use it when token refresh */
                request.Headers.Add("UserToken", token);

                var secretKey = ApplicationJjwtContext.SecretKey;
                var secretKeyMessage = ApplicationJjwtContext.SecretKeyMessage;

                if (!string.IsNullOrEmpty(secretKeyMessage))
                {
                    return Task.FromResult(request.CreateErrorResponse(HttpStatusCode.BadRequest, secretKeyMessage));
                }

                byte[] secret = JjwtTokens.GetBytesSecretKey(secretKey);

                Thread.CurrentPrincipal = JjwtTokens.ValidateToken(token, secret, true);

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = Thread.CurrentPrincipal;
                }
            }
            catch (JoseException ex)
            {
                AddAuthorizationError(headers, ex.Message);
            }
            catch (Exception ex)
            {
                AddAuthorizationError(headers, ex.GetFirstException().Message);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private bool AddAuthorizationError(System.Net.Http.Headers.HttpRequestHeaders headers, string errorMessage)
        {
            try
            {
                headers.Add("UnauthorizedMessage", errorMessage);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}