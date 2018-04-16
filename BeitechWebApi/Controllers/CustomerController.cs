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
    public class CustomerController : ApiControllerSimple
    {
        [HttpGet]
        [Route("api/customer/list")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<IEnumerable<GetCustomerModelOut>>))]
        public async Task<IHttpActionResult> GetCustomers()
        {
            try
            {
               
                CustomerService _customerService = new CustomerService();


                return Ok(new ApiMessageResponse<IEnumerable<GetCustomerModelOut>>(await _customerService.GetCustomersAsync()));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<IEnumerable<GetCustomerModelOut>>(ex.GetFirstException().Message));
            }
        }

        [HttpGet]
        [Route("api/customer/available_products")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<IEnumerable<GetProductsAvailableModelOut>>))]
        public async Task<IHttpActionResult> GetProductsAvailable()
        {
            try
            {

                CustomerService _customerService = new CustomerService();


                return Ok(new ApiMessageResponse<IEnumerable<GetProductsAvailableModelOut>>(await _customerService.GetProductsAvailableAsync()));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<IEnumerable<GetProductsAvailableModelOut>>(ex.GetFirstException().Message));
            }
        }
    }
}