using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Models.Location;

namespace WebApplication.Controllers
{
    public class LocationController : Controller
    {
        //
        // GET: /Location/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetAllProvinces()
        {

            var models = new List<ProvinceModel>();
            List<Province> provinces = AddressBll.GetAllProvinces();
            if (provinces.Count > 0)
            {
                foreach (var item in provinces)
                {
                    models.Add(new ProvinceModel
                    {
                        ProvinceId = item.ProvinceId.ToString(),
                        Name = item.Name.ToString(),
                        Type = item.Type.ToString()
                    });
                }
            }


            return Json(new { provinces = models }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistrictsByProvince(string provinceId)
        {
            var models = new List<DistrictModel>();
            List<District> districts = AddressBll.GetDistrictsByProvince(provinceId);
            if (districts.Count > 0)
            {
                foreach (var item in districts)
                {
                    models.Add(new DistrictModel
                    {
                        ProvinceId = item.ProvinceId.ToString(),
                        Name = item.Name.ToString(),
                        Type = item.Type.ToString(),
                        Location = item.Location.ToString(),
                        DistrictId = item.DistrictId.ToString()
                    });
                }
            }

            return Json(new { districts = models }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWardsByDistrict(string districtId)
        {
            var models = new List<WardModel>();
            List<Ward> wards = AddressBll.GetWardsByDistrict(districtId);
            if (wards.Count > 0)
            {
                foreach (var item in wards)
                {
                    models.Add(new WardModel
                    {
                        WardId = item.WardId.ToString(),
                        Name = item.Name.ToString(),
                        Type = item.Type.ToString(),
                        Location = item.Location.ToString(),
                        DistrictId = item.DistrictId.ToString()
                    });
                }
            }
            return Json(new { wards = models }, JsonRequestBehavior.AllowGet);
        }
    }
}
