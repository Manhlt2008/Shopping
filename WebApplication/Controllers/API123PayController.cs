using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Bll.Payments;

namespace WebApplication.Controllers
{
    public class API123PayController : Controller
    {
        //
        // GET: /API123Pay/
        public string MERCHANTCODE = ConfigurationManager.AppSettings["MERCHANTCODE"];
        public string PASSCODE = ConfigurationManager.AppSettings["PASSCODE"];
        public string SECRETKEY = ConfigurationManager.AppSettings["SECRETKEY"];
        public string CREATEORDER_URL = ConfigurationManager.AppSettings["CREATEORDER_URL"];
        public string QUERYORDER_URL = ConfigurationManager.AppSettings["QUERYORDER_URL"];//CALL_BACK_URL = ConfigurationManager.AppSettings["CALL_BACK_URL"];
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //[HttpPost]
        public JsonResult Notify(string mTransactionID,string bankCode,string transactionStatus,string description, string ts,string checksum)
        {
            var returnCode = string.Empty;
            try
            {
                string data = mTransactionID + bankCode + transactionStatus + ts + SECRETKEY;
                var keyPNM = CalculateSHA1(data);

                var currentTime = DateTime.Now.Millisecond;
                var totalSecond = currentTime - Convert.ToInt64(ts);
                var limitSecond = 300;//5 min = 5*60 = 300
                if (totalSecond < 0 || totalSecond > limitSecond)
                {
                    returnCode = "-2";//Expire Request
                }
                else if (keyPNM != checksum)
                {
                    returnCode = "-1";//checksum invalid
                }
                else
                {
                    //Update DB
                    A123PayBll.UpdateTransactionFromNotify(mTransactionID, transactionStatus, description, ts);
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetTransactionById ([Notify] error: " + mTransactionID.ToString() + " bankCode: " + bankCode + " transactionStatus: " + transactionStatus+ " description: " + description + " ts: " + ts + ex.ToString());
            }
            return Json(new { mTransactionID = mTransactionID, returnCode = returnCode, ts = ts, checksum = checksum }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReturnUrl(string mTransactionID)
        {
            var tran = A123PayBll.GetTransactionById(mTransactionID);
            if(tran!=null)
            {
                if (tran.ReturnCode != "1")
                {
                    string Url = QUERYORDER_URL;
                    //queryOrder , return message 
                    string JsonContent = string.Empty;
                    using (var webclient = new System.Net.WebClient())
                    {
                        webclient.Encoding = Encoding.UTF8;
                        JsonContent = webclient.DownloadString(Url);
                    }

                    dynamic objResult = JsonConvert.DeserializeObject(JsonContent);
                    ViewBag.Message = "Thanh toán không thành thành công";
                }
                else
                {
                    ViewBag.Message = "Thanh toán thành công";
                }
            }
            return View();
        }
        public static string CalculateSHA1(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 =
            new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash.ToLower();
        }
    }
}
