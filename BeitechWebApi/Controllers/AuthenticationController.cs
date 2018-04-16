using BeitechBussiness.Models;
using BeitechBussiness.Services;
using BeitechWebApi.App_Start;
using BeitechWebApi.Extensions;
using BeitechWebApi.Jwt;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BeitechWebApi.Controllers
{

    [ApiModelStateValidation]
    public class AuthenticationController : ApiControllerSimple
    {

        [HttpPost]
        [Route("api/authentication/login")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<TokenModelOut>))]
        public async Task<IHttpActionResult> Login(UserLoginModelIn modelIn)
        {
            try
            {
                if (modelIn.IsNull())
                {
                    return BadRequest(new ApiMessageResponse<TokenModelOut>("Modelo Inválido"));
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiMessageResponse<TokenModelOut>(ModelState.StringifyModelErrors()));
                }

                UserService userService = new UserService();

                UserLoginModelOut _user = await userService.GetUserAsync(modelIn);

                if(_user.IsNull())
                {
                    return BadRequest(new ApiMessageResponse<TokenModelOut>("Usuario no válido"));
                }

                var secretKey = ApplicationJjwtContext.SecretKey;
                var secretKeyMessage = ApplicationJjwtContext.SecretKeyMessage;
                var sessionMinutes = ApplicationJjwtContext.SessionMinutes;

                var TokenModelIn = new TokenModelIn()
                {
                    UserId = _user.Id.ToString(),

                    UserFullName = _user.Name,

                    SessionMinutes = sessionMinutes
                };

                if (!string.IsNullOrEmpty(secretKeyMessage))
                {
                    return BadRequest(new ApiMessageResponse<TokenModelOut>("No autenticado"));
                }

                var ret = JjwtTokens.CreateToken(secretKey, TokenModelIn);

                ret.UserId = _user.Id;
                ret.UserName = _user.Name;
              
                return Ok(new ApiMessageResponse<TokenModelOut>(ret));
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiMessageResponse<TokenModelOut>(ex.GetFirstException().Message));
            }
          


       

        }

        [HttpPost]
        [Route("api/authentication/renew")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<TokenModelOut>))]

        public IHttpActionResult RenewToken()
        {
            try
            {
                IEnumerable<string> authHeaderValues;
                Request.Headers.TryGetValues("UserToken", out authHeaderValues);

                var userToken = authHeaderValues.ElementAt(0);

                TokenModelIn TokenModelIn = JjwtTokens.GetTokenModel(userToken, ApplicationJjwtContext.SecretKey, ApplicationJjwtContext.SessionMinutes);

                var secretKeyMessage = ApplicationJjwtContext.SecretKeyMessage;

                if (!string.IsNullOrEmpty(secretKeyMessage))
                {
                    return BadRequestModelErrorOut(secretKeyMessage);
                }

                var ret = JjwtTokens.CreateToken(ApplicationJjwtContext.SecretKey, TokenModelIn);
                return Ok(new ApiMessageResponse<TokenModelOut>(ret));
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiMessageResponse<TokenModelOut>(ex.GetFirstException().Message));
            }
          
        }



    }
}
