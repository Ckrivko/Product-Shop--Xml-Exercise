using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("User")]
    public  class ImportUsersDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [Required]
        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

    }
}
