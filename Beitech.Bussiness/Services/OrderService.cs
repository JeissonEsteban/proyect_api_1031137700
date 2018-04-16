using BeitechBussiness.Models;
using BeitechBussiness.Persistent;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Services
{
    public class OrderService
    {
        #region Public Properties
        #endregion
        #region Private Properties
        private GeneralDbContext db;
        #endregion
        #region Constructors
        public OrderService()
        {
            this.db= new GeneralDbContext();
        }
        #endregion
        #region Public Methods
        public async Task<NewOrderModelOut> SetNewOrderAsync(NewOrderModelIn modelIn)
        {
            //cantidad de productos

            if (modelIn.OrderDetails==null || modelIn.OrderDetails.Count() == 0)
            {
                throw new Exception("No existen productos asociados a esta orden.");
            }


            if ( modelIn.OrderDetails.Count() > 5)
            {
                throw new Exception("Solo se pueden registrar hasta 5 productos por orden.");
            }

            var retOrderInsert = await db.Database.SqlQuery<GetResponseOrderDb>("sp_new_order @customer_id,@delivery_address",
                new SqlParameter("customer_id", modelIn.CustomerId),
                new SqlParameter("delivery_address", modelIn.DeliveryAddress)
                ).FirstOrDefaultAsync();

          
            if(retOrderInsert == null)
            {
                throw new Exception("Error al ingresar la Orden. Error DataBase.");
            }

            if(retOrderInsert.state==0)
            {
                throw new Exception($"Error al ingresar la Orden. Error DataBase.{retOrderInsert.msg}");
            }

            foreach (NewOrderDetailModelIn orderDetail in modelIn.OrderDetails)
            {
                var retOrderDetailInsert = db.Database.SqlQuery<GetResponseOrderDb>("sp_new_order_detail @order_id,@product_id,@count",
                        new SqlParameter("order_id", retOrderInsert.id),
                        new SqlParameter("product_id", orderDetail.ProductId),
                        new SqlParameter("count", orderDetail.Count)
                        ).FirstOrDefault();

                if (retOrderDetailInsert == null)
                {
                    this.CallBackNewOrder(retOrderInsert.id);
                    throw new Exception("Error al ingresar el detalle de la Orden. Error DataBase.");
                }

                if (retOrderDetailInsert.state == 0)
                {
                    this.CallBackNewOrder(retOrderInsert.id);
                    throw new Exception($"Error al ingresar el detalle de la Orden. Error DataBase.{retOrderDetailInsert.msg}");
                }
                

            }



            return new NewOrderModelOut { Id=retOrderInsert.id};
        }

        /// <summary>
        /// Este metodo se reemplaza por el manejo de Transacciones que se defina
        /// </summary>
        /// <param name="orderId"></param>
        public void CallBackNewOrder(int orderId)
        {
            db.Database.SqlQuery<GetResponseOrderDb>("sp_callback_new_order @order_id",
                        new SqlParameter("order_id", orderId)
                        ).FirstOrDefault();
        }
        public async Task<IEnumerable<GetOrderModelOut>> GetByCustomerAsync(GetOrderByCustomerModelIn modelIn)
        {
            //set yy-mm-dd
            modelIn.StartDate = new DateTime(modelIn.StartDate.Year, modelIn.StartDate.Month, modelIn.StartDate.Day);
            modelIn.EndDate = new DateTime(modelIn.EndDate.Year, modelIn.EndDate.Month, modelIn.EndDate.Day);

            var ret = db.Database.SqlQuery<GetOrderModelOut>("SELECT * FROM v_order WHERE CustomerId=@CustomerId AND OrderDate BETWEEN @StartDate AND @EndDate", 
                new SqlParameter("CustomerId", modelIn.CustomerId),
                new SqlParameter("StartDate", modelIn.StartDate),
                new SqlParameter("EndDate", modelIn.EndDate)
                );
            var list = await ret.ToListAsync();

            if (list.Count > 0)
            {

                list.ForEach(o =>
                {
                  
                    var retDetail = db.Database.SqlQuery<GetOrderDetailModelOut>($"SELECT * FROM v_order_detail WHERE OrderId={o.OrderId}");

                    var _details = retDetail.ToList();

                    o.OrderDetails = _details;
                });


            }


            return list;
        }
        #endregion
        #region Private Methods
        private async Task<NewOrderModelOut> SetNewOrderDetailAsync(NewOrderDetailModelIn modelIn)
        {

            var ret = await db.Database.ExecuteSqlCommandAsync("SELECT Id,AccessCode,Name FROM v_user WHERE AccessCode=@AccessCode AND Pass=@Password",
                new SqlParameter("AccessCode", modelIn),
                new SqlParameter("Password", modelIn));


            return null;
        }

       
        #endregion
    }
}
