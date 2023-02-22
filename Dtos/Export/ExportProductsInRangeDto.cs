﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Product")]
    
   public  class ExportProductsInRangeDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }



        [XmlElement("buyer")]
        
        public string BuyerFullName { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool aSpecified { get { return this.BuyerFullName != null; } }
    }
}
