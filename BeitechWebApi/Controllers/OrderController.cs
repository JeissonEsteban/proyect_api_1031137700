using BeitechBussiness.Models;
using BeitechBussiness.Services;
using BeitechWebApi.Extensions;
using BeitechWebApi.Jwt;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BeitechWebApi.Controllers
{
    [JwtAuthorize]
    public class OrderController : ApiControllerSimple
    {
        [HttpPost]
        [Route("api/order/add")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<NewOrderModelOut>))]
        public async Task<IHttpActionResult> SetOrder(NewOrderModelIn modelIn)
        {
            try
            {

                if (modelIn.IsNull())
                {
                    return BadRequest(new ApiMessageResponse<NewOrderModelOut>("Modelo Inválido"));
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.GetModelErrors();
                    
                    return BadRequest(new ApiMessageResponse<NewOrderModelOut>(ModelState.StringifyModelErrors()));
                }


                OrderService _orderService = new OrderService();


                var ret = await _orderService.SetNewOrderAsync(modelIn);


                return Ok(new ApiMessageResponse<NewOrderModelOut>(ret));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<TokenModelOut>(ex.GetFirstException().Message));
            }
        }


        [HttpPost]
        [Route("api/order/getbycustomer")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<IEnumerable<GetOrderModelOut>>))]
        public async Task<IHttpActionResult> GetByCustomer(GetOrderByCustomerModelIn modelIn)
        {
            try
            {
                if (modelIn.IsNull())
                {
                    return BadRequest(new ApiMessageResponse<IEnumerable<GetOrderModelOut>>("Modelo Inválido"));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.GetModelErrors();
                    return BadRequest(new ApiMessageResponse<IEnumerable<GetOrderModelOut>>(ModelState.StringifyModelErrors()));
                }

                OrderService _orderService = new OrderService();


                return Ok(new ApiMessageResponse<IEnumerable<GetOrderModelOut>>(await _orderService.GetByCustomerAsync(modelIn)));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<IEnumerable<GetOrderModelOut>>(ex.GetFirstException().Message));
            }
        }
    }
}