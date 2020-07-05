using System;
using System.Collections.Generic;
using System.Text;

namespace AuthDtos
{
    public class BaseResponse
    {
        public BaseResponse(object data)
        {
            Data = data;
        }

        public int Version { get; set; }
        public int Revision { get; set; }

        public object Data { get; set; }

        public IList<string> Error { get; set; }
    }
}
