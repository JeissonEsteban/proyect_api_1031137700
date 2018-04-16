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
    public class ProductService
    {
        #region Public Properties
        #endregion
        #region Private Properties
        private GeneralDbContext db;
        #endregion
        #region Constructors
        public ProductService()
        {
            this.db= new GeneralDbContext();
        }
        #endregion
        #region Public Methods
        public async Task<IEnumerable<GetProductModelOut>> GetProductsAsync()
        {
            var ret = db.Database.SqlQuery<GetProductModelOut>("SELECT * FROM v_product");
            
            return await ret.ToListAsync();
        }

        public async Task<IEnumerable<GetProductSaleModelOut>> GetProductSalesAsync()
        {
            var ret = db.Database.SqlQuery<GetProductSaleModelOut>("SELECT * FROM v_product_sales");
            var list = await ret.ToListAsync();

            if(list.Count>0)
            {

                decimal totalPrice = list.Sum(p => p.TotalPrice);
                if(totalPrice>0)
                {
                    list.ForEach(p =>
                    {
                        p.PercentPrice = Convert.ToInt32(Math.Round((p.TotalPrice / totalPrice) * 100, 0));
                    });
                }
                
             
            }
            return list;
        }
        #endregion
        #region Private Methods

        #endregion
    }
}
