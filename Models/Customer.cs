using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGDChatBot.Models
{
    [Serializable]
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetName { get; set; }
        public int HouseNumber { get; set; }
        public string Email { get; set; }
    }
}