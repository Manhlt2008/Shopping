using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.User
{

    [Serializable]
    public class ResultModel
    {
        public int Code = 0;
        public string Message = "Thành công";
        public object Data { get; set; }
        public ResultModel() { }

        public ResultModel(ResultModel Obj)
        {
            Code = Obj.Code;
            Message = Obj.Message;
        }

        public void setCode(ResultModel Obj)
        {
            Code = Obj.Code;
            Message = Obj.Message;
        }
    }
}