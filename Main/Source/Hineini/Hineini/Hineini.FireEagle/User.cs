using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hineini.FireEagle {
    [XmlRoot("user")]
    [Serializable]
    public class User
    {
        [XmlAttribute("token", Form = XmlSchemaForm.Unqualified)]
        public string Token;

        [XmlElement("location-hierarchy", Form = XmlSchemaForm.Unqualified)]
        public LocationHierarchy LocationHierarchy;

    }
}