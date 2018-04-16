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
    public class ProductController : ApiControllerSimple
    {
        [HttpGet]
        [Route("api/product/list")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<IEnumerable<GetProductModelOut>>))]
        public async Task<IHttpActionResult> GetProducts()
        {
            try
            {
             
                ProductService _productService = new ProductService();


                return Ok(new ApiMessageResponse<IEnumerable<GetProductModelOut>>(await _productService.GetProductsAsync()));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<IEnumerable<GetProductModelOut>>(ex.GetFirstException().Message));
            }
        }


        [HttpGet]
        [Route("api/product/sales")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(ApiMessageResponse<IEnumerable<GetProductSaleModelOut>>))]
        public async Task<IHttpActionResult> GetProductsSales()
        {
            try
            {
             

                ProductService _productService = new ProductService();


                return Ok(new ApiMessageResponse<IEnumerable<GetProductSaleModelOut>>(await _productService.GetProductSalesAsync()));

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiMessageResponse<IEnumerable<GetProductSaleModelOut>>(ex.GetFirstException().Message));
            }
        }
    }
}