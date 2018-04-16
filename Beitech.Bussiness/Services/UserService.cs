using BeitechBussiness.Extensions;
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
    public class UserService
    {
        #region Public Properties
        #endregion
        #region Private Properties
        #endregion
        #region Constructors
        public UserService()
        {

        }
        #endregion
        #region Public Methods
        public async Task<UserLoginModelOut> GetUserAsync(UserLoginModelIn modelIn)
        {

            GeneralDbContext db = new GeneralDbContext();

            var ret = db.Database.SqlQuery<UserLoginModelOut>("SELECT Id,AccessCode,Name FROM v_user WHERE AccessCode=@AccessCode AND Pass=@Password", 
                new SqlParameter("AccessCode", modelIn.AccessCode),
                new SqlParameter("Password", modelIn.Password.ToBase64()));

            if(ret==null)
            {
                return null;
            }


            var _list = await ret.ToListAsync();

            if(_list.Count==0)
            {
                return null;
            }
            else
            {
                return _list[0];
            }

        }
        #endregion
    }
}
