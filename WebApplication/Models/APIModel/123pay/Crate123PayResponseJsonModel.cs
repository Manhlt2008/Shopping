using System.Collections.Generic;
using Newtonsoft.Json;
using WebApplication.Lib.Bll.Payments;
using WebApplication.Lib.Dal.ConfigData;
using WebApplication.Lib.Util.Common;

namespace WebApplication.Models.APIModel._123pay
{
    public class Crate123PayResponseJsonModel
    {
        [JsonProperty("result")]
        public List<string> Result { get; set; }

        public Create123PayResponseModel GetResponseModel()
        {
            Create123PayResponseModel response;
            var payment123PayConfig = Payment123PayConfig.Instance;

            if (Result[0].Equals(A123PayBll.ErrorCodeEnum.NoError.GetHashCode().ToString("D")))
            {
                var correctChecksum = Utils.Sha1(string.Format("{0}{1}{2}{3}", Result[0], Result[1], Result[2], payment123PayConfig.SecretKey));

                if (!correctChecksum.Equals(Result[3]))
                {
                    response = new Create123PayResponseModel
                    {
                        ReturnCode = A123PayBll.ErrorCodeEnum.InvalidChecksum.GetHashCode().ToString("D"),
                        Description = A123PayBll.ErrorCodeDetail(A123PayBll.ErrorCodeEnum.InvalidChecksum),
                        Checksum = Result[3]
                    };
                }
                else
                {
                    response = new Create123PayResponseModel
                    {
                        ReturnCode = Result[0],
                        TransactionId = Result[1],
                        RedirectUrl = Result[2],
                        Checksum = Result[3]
                    };
                }
            }
            else
            {
                response = new Create123PayResponseModel
                {
                    ReturnCode = Result[0],
                    Description = Result[1]
                };
            }

            return response;
        }
    }
}