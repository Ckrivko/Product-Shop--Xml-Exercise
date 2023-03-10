using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.ExportUsersAndProducts.ExportSoldProducts
{
    [XmlType("Product")]
   public class ExportProductDto
    {

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
