using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceDataModel.Models
{
    public class CustmerViewModel
    {
        public CustmerViewModel()
        {
            Detail = new List<CustomerCustomData>();
        }
        public CustomerModel Customer { get; set; }
        public List<CustomerCustomData> Detail { get; set; }
    }

    public class CustomerModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string City { get; set; }
    }

    public class CustomerCustomData
    {
        public string TableName { get; set; }
        public string FieldName { get;set;}
        public string FieldValue { get; set; }
        public int FieldID { get; set; }
        public int TableID { get; set; }
    }

    
}