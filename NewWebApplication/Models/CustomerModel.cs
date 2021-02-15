using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewWebApplication.Models
{
    public class CustomerModel
    {
        [Key]
        public string customer_id { get; set; }
        public string Contact_Name { get; set; }
        public string Customer_Type { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public string Registration_No { get; set; }
        public DateTime? Date_Incorp { get; set; }
        public string Address_Line1 { get; set; }
        public string Address_Line2 { get; set; }
        public string Town_City { get; set; }
        public string Country { get; set; }
        public Int64 Contact_Number { get; set; }
        public Int64 Num_Shares { get; set; }
        public decimal Share_Price { get; set; }

        [NotMapped]
        public decimal balance { get; set; }
    }
}
