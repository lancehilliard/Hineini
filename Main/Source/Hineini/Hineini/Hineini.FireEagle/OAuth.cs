using System;
using System.Collections.Generic;
using System.Text;

namespace Hineini.FireEagle {
    internal sealed class OAuthParameter: IComparable
    {
        public string Name;
        public string Value;

        public OAuthParameter(string ParameterName, string ParameterValue)
        {
            Name = ParameterName;
            Value = ParameterValue;
        }

        public override string ToString()
        {
            return OAuth.UrlEncode(Name) + "=" + OAuth.UrlEncode(Value);
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj is OAuthParameter)
                return this.Name.CompareTo(((OAuthParameter)obj).Name);
            else
                return 0;
        }

        #endregion
    }

    internal sealed class OAuth
    {
        private const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        public static string GenerateNonce()
        {
            Random rand = new Random();
            return rand.Next(999, 999999).ToString();

        }

        public static string GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds.ToString();
        }

        
    }
}