using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Models
{
    public class UserLoginModelIn
    {
        [Required]
        public string AccessCode { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserLoginModelOut
    {
        public int Id { get; set; }
        public string AccessCode { get; set; }
        public string Name { get; set; }
    }


}
