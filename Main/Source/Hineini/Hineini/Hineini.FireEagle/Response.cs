using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Hineini.FireEagle {
    [XmlRoot("rsp", Namespace="", IsNullable=false)]
    [Serializable]
    public class Response
    {
        [XmlAttribute("stat", Form = XmlSchemaForm.Unqualified)]
        public ResponseStatus Status;

        [XmlElement("querystring", Form = XmlSchemaForm.Unqualified)]
        public string QueryString;

        [XmlElement("locations", Form = XmlSchemaForm.Unqualified)]
        public Locations Locations;

        [XmlElement("user", Form = XmlSchemaForm.Unqualified)]
        public User User;

        [XmlElement("users", Form = XmlSchemaForm.Unqualified)]
        public Users Users;

        [XmlElement("err", Form = XmlSchemaForm.Unqualified)]
        public Error Error;


    }

    /// <summary>
    /// The status of the response, either ok or fail.
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// An unknown status, and the default value if not set.
        /// </summary>
        [XmlEnum("unknown")]
        Unknown,

        /// <summary>
        /// The response returns "ok" on a successful execution of the method.
        /// </summary>
        [XmlEnum("ok")]
        OK,
        /// <summary>
        /// The response returns "fail" if there is an error, such as invalid API key or login failure.
        /// </summary>
        [XmlEnum("fail")]
        Failed
    }
}