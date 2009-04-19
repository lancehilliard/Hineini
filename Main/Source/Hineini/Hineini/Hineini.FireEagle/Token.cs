using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Hineini.FireEagle {
    /// <summary>
    /// Class for containing public/secret token pairs.
    /// </summary>
    [XmlRoot("token")]
    [Serializable]
    public class Token
    {
        [XmlAttribute("public", Form = XmlSchemaForm.Unqualified)]
        public string PublicToken;
        [XmlAttribute("secret", Form = XmlSchemaForm.Unqualified)]
        public string SecretToken;

        public Token() { }

        public Token(string pub, string sec)
        {
            PublicToken = pub;
            SecretToken = sec;
        }

        public Token(string resp)
        {
            PublicToken = resp.Split('&')[0].Split('=')[1];
            SecretToken = resp.Split('&')[1].Split('=')[1];
        }
    }
}