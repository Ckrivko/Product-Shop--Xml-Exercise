using ProductShop.Dtos.Export;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.ExportUsersAndProducts
{
    [XmlType("Users")]
    public class ExportUsersAndProductsMainDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public ExportUsersAndProductsDto[] Users { get; set; }

    }
}
