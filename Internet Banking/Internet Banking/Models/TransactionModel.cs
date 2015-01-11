using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Internet_Banking.Models
{
    public class TransactionModel
    {
        public TransactionModel() { }

        public int Id { get; set; }

        public System.DateTime Time { get; set; }

        public Nullable<System.DateTime> ReverseTime { get; set; }

        public string Status { get; set; }

        public int PaymentsId { get; set; }

        public int ParentPaymentsId { get; set; }

        public int CurrencyId { get; set; }

        public int CommissionsId { get; set; }

        public int VendorsId { get; set; }

        public string Amount { get; set; }

        public long Card { get; set; }

        public string Fld001 { get; set; }

        public string Fld002 { get; set; }

        public string Fld003 { get; set; }

        public string Fld004 { get; set; }

        public string Fld005 { get; set; }

        public string Fld006 { get; set; }

        public string Fld007 { get; set; }

        public string Fld008 { get; set; }

        public string Fld009 { get; set; }

        public string Fld010 { get; set; }
    }
}