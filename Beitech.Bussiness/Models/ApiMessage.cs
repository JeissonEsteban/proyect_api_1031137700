using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Models
{
    public enum ResponseCode { ERROR=0, OK, NOT_RESULT}
    public class ApiMessageResponse<T>
    {
        public ResponseCode StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }


        public ApiMessageResponse(Dictionary<string, string> dictionary)
        {
            this.Message = "";
            this.StatusCode = ResponseCode.OK;
        }

        public ApiMessageResponse(T data)
        {
            this.Data=data;
            this.StatusCode = ResponseCode.OK;
        }

        public ApiMessageResponse(string messageError)
        {
            this.Message = messageError;
            this.StatusCode = ResponseCode.ERROR;
        }
    }
}
