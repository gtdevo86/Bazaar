using My.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bazaar.Models
{
    public class ZipCodeManager
    {

        [Key]
        [StringLength(5)]
        public string Zipcode { get; set; }

        [Required]
        [Precision(9, 6)]
        public Nullable<decimal> Latitude { get; set; }

        [Required]
        [Precision(9,6)]
        public Nullable<decimal> Longitude { get; set; }
   



    }
}