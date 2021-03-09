using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpbooking.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Floor { get; set; }
        public int RoomNumber { get; set; }
        public CategoryModel Category { get; set; }

        public List<CategoryModel> CategoryList { get; set; }

    }
}
