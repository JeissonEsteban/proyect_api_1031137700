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
    public class CustomerService
    {
        #region Public Properties
        #endregion
        #region Private Properties
        private GeneralDbContext db;
        #endregion
        #region Constructors
        public CustomerService()
        {
            this.db= new GeneralDbContext();
        }
        #endregion
        #region Public Methods
        public async Task<IEnumerable<GetCustomerModelOut>> GetCustomersAsync()
        {
            var ret = db.Database.SqlQuery<GetCustomerModelOut>("SELECT * FROM v_customer ORDER BY Id DESC");
            
            return await ret.ToListAsync();
        }
        public async Task<IEnumerable<GetProductsAvailableModelOut>> GetProductsAvailableAsync()
        {
            var ret = db.Database.SqlQuery<GetProductsAvailableModelOut>("SELECT * FROM v_customer_available_products");

            return await ret.ToListAsync();
        }
        #endregion
        #region Private Methods

        #endregion
    }
}
