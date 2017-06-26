using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;

namespace WebApplication.Lib.Bll
{
    public class AddressBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Province> GetAllProvinces()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var provinces = dbContext.Provinces.OrderBy(m => m.Name).ToList();
                    return provinces;
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetAllProvinces", exception);
                return null;
            }
        }

        public static List<District> GetAllDistricts()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var districts = dbContext.Districts.OrderBy(m => m.Name).ToList();
                    return districts;
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetAllDistricts", exception);
                return null;
            }
        }

        public static List<District> GetDistrictsByProvince(string provinceId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var districts = dbContext.Districts.Where(m => m.Province.ProvinceId.Equals(provinceId)).Include(m => m.Province).Include(m => m.Wards).OrderBy(m => m.Name).ToList();
                    return districts;
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetDistrictsByProvince", exception);
                return null;
            }
        }

        public static List<Ward> GetWardsByDistrict(string districtId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var districts = dbContext.Wards.Where(m => m.District.DistrictId.Equals(districtId)).OrderBy(m => m.Name).ToList();
                    return districts;
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetDistricByProvince", exception);
                return null;
            }
        }

    }
}