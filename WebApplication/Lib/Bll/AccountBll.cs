using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using log4net;
using Microsoft.Ajax.Utilities;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.User;


namespace WebApplication.Lib.Bll
{
    public static class AccountBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Account> FindAllAccount()
        {
            List<Account> list = new List<Account>();
            try
            {
                using (var dbContext = new Entities())
                {
                    list = dbContext.Accounts.Where(m=>m.Status == StatusEnum.Active).ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetAllAccount", exception);
            }
            return list;
        }

        public static List<long> FindAllAccountBySupplier(long supplier = 0)
        {
            List<long> list = new List<long>();
            if (supplier != 0)
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        var listTemp = dbContext.SupplierAccounts.Where(m => m.SupplierId == supplier && m.Status == StatusEnum.Active).ToList();
                        foreach (var item in listTemp)
                        {
                            list.Add(item.AccountId);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("FindAllAccountBySupplier", exception);
                }
            }
            return list;
        }
    }
}