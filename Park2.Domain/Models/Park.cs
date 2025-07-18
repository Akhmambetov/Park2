using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Domain.Models
{
    public class Park
    {
        public List<Attraction> Attractions { get; set; } = new();
        public List<Visitor> ActiveVisitors { get; private set; } = new();
        public List<Visitor> DepartedVisitors { get; private set; } = new();
        public decimal TotalRevenue { get; set; } = 0; // Возможно здесь стоит внести изменения
    }
}
