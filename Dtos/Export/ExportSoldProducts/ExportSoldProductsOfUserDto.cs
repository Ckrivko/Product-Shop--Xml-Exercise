using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.ExportUsersAndProducts.ExportSoldProducts
{
    [XmlType("User")]
    public class ExportSoldProductsOfUserDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }


        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public ExportProductDto[] Products { get; set; }


    }
}
