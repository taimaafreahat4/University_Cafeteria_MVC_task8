
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace University_Cafeteria_Task8.Models
{
    public class SupplyRequest
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public string UserId { get; set; } = "";

        public IdentityUser User { get; set; } = null!;

        public ICollection<RequestItem> RequestItems { get; set; }
            = new List<RequestItem>();
    }
}

