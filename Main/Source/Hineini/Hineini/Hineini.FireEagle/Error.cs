using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hineini.FireEagle {
    [XmlRoot("err"), Serializable]
    
    public class Error {
        [XmlAttribute("code", Form = XmlSchemaForm.Unqualified)]
        public int Code;

        [XmlAttribute("msg", Form = XmlSchemaForm.Unqualified)]
        public string Message;
    }
}