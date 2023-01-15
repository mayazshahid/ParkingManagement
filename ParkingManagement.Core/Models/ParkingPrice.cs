using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Core.Models
{
    public class ParkingPrice
    {
        public string SeasonName { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
