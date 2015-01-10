using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Internet_Banking.Models
{
    public class TransferOperationModel
    {
        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }
        public string UserIdFrom { get; set; }
        public string UserIdTo { get; set; }
        public double TransferValue { get; set; }
        public string Currency { get; set; }
    }
}