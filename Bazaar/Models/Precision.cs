using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace My.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Precision : Attribute
    {
        public byte precision { get; set; }
        public byte scale { get; set; }
        public Precision(byte precision, byte scale)
        {
            this.precision = precision;
            this.scale = scale;
        }   
    }
}