using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpbooking.Models
{
    public class FileUpload
    {
        [NotMapped]
        public IFormFile image { get; set; }
    }
}
