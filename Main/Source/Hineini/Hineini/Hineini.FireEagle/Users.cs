using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hineini.FireEagle {
    [XmlRoot("users")]
    [Serializable]
    public class Users
    {
        private User[] m_users = new User[0];

        [XmlElement("user", Form = XmlSchemaForm.Unqualified)]
        public User[] UserCollection
        {
            get { return m_users; }
            set
            {
                m_users = value == null ? new User[0] : value;
            }
        }

        [XmlAttribute("token", Form = XmlSchemaForm.Unqualified)]
        public string Token;

        [XmlElement("location-hierarchy", Form = XmlSchemaForm.Unqualified)]
        public LocationHierarchy LocationHierarchy;

    }
}