﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGDChatBot.Models
{
    [Serializable]
    public class UnauthenticatedCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}