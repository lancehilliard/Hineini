using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography;

using System.Xml.Serialization;
using System.Xml.Schema;
using Hineini.Utility;

namespace Hineini.FireEagle {
    /// <summary>
    /// The Fire Eagle Library
    /// </summary>
    /// <remarks>
    /// This is the only class you really need to care about creating. Everything else is created and returned by this object.
    /// 
    /// 
    /// </remarks>
    /// <example>
    /// <code>FireEagle fe = new FireEagle(CONSUMERKEY,SECRETKEY,);
    /// User user = flickr.PeopleFindByEmail("cal@iamcal.com");
    /// Console.WriteLine("User Id is " + u.UserId);</code>
    /// </example>
    public class FireEagle
    {
        #region [Private Variables]
        private string m_api;
        private string m_secret;
        private Token m_token;
        private Token m_general;

        private WebProxy m_proxy;

        private const string m_baseUrl = "https://fireeagle.yahooapis.com/";
        private const string m_siteUrl = "http://fireeagle.yahoo.net/";
        #endregion

        #region [Properties]
        /// <summary>
        /// User access token.
        /// </summary>
        public Token UserToken
        {
            get { return m_token; }
            set { m_token = value; }
        }

        /// <summary>
        /// General access token
        /// </summary>
        public Token GeneralToken
        {
            get { return m_general; }
            set { m_general = value; }
        }

        /// <summary>
        /// Web proxy used for connecting to Fire Eagle.
        /// </summary>
        public WebProxy Proxy
        {
            get { return m_proxy; }
            set { m_proxy = value; }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Create an instance of Fire Eagle without any access tokens.
        /// </summary>
        /// <param name="ConsumerKey">The consumer key. You can find this after registering your application at http://fireeagle.yahoo.net/developer/manage</param>
        /// <param name="ConsumerSecret">The consumer secret. You can find this after registering your application at http://fireeagle.yahoo.net/developer/manage</param>
        /// <remarks>
        /// Start here when you have a new user. Create an instance and then obtain an access token.
        /// </remarks>
        public FireEagle(string ConsumerKey, string ConsumerSecret)
            : this(ConsumerKey, ConsumerSecret, null)
        {
        }

        /// <summary>
        /// Create an instance of Fire Eagle with an access token.
        /// </summary>
        /// <param name="ConsumerKey"></param>
        /// <param name="ConsumerSecret"></param>
        /// <param name="AccessToken"></param>
        public FireEagle(string ConsumerKey, string ConsumerSecret, Token AccessToken)
            : this(ConsumerKey, ConsumerSecret, AccessToken, null)
        {
        }

        /// <summary>
        /// Create an instance of Fire Eagle with access and general tokens.
        /// </summary>
        /// <param name="ConsumerKey">The consumer key. You can find this after registering your application at http://fireeagle.yahoo.net/developer/manage</param>
        /// <param name="ConsumerSecret">The consumer secret. You can find this after registering your application at http://fireeagle.yahoo.net/developer/manage</param>
        /// <param name="AccessToken">User access token. Needed to perform Update or other user-specific queries.</param>
        /// <param name="GeneralToken">General access token. Needed for Within() and Recent().</param>
        public FireEagle(string ConsumerKey, string ConsumerSecret, Token AccessToken, Token GeneralToken) {
            m_api = ConsumerKey;
            m_secret = ConsumerSecret;
            m_general = GeneralToken;
            m_token = AccessToken;
        }
        #endregion

        #region [Private Methods]
        /// <summary>
        /// Get the raw response from Fire Eagle.
        /// </summary>
        /// <param name="httpmethod">Should be GET or POST. Not case-sensitive.</param>
        /// <param name="method">The Fire Eagle method to perform. (oauth/request_token, api/0.1/lookup, etc.)</param>
        /// <param name="token">The token used as part of the hash key (should be the secret). Pass an empty string for tokenless methods.</param>
        /// <param name="parameters">A list of all parameters to pass to the Fire Eagle method. Should include required OAuth parameters. (oauth_consumer, oauth_version, etc)</param>
        /// <returns>Raw response from Fire Eagle. May be XML.</returns>
        private string GetStringResponse(string httpmethod, string method, string token, List<string> parameters)
        {
            parameters.Sort();

            string url = m_baseUrl + method;
            string ParamString = string.Join("&", parameters.ToArray());
            string BaseString = httpmethod.ToUpper() + "&" + OAuth.UrlEncode(url) + "&" + OAuth.UrlEncode(ParamString);
            HmacSha1 sha1 = new HmacSha1();
            sha1.Key = Encoding.UTF8.GetBytes(m_secret + "&" + token);

            parameters.Add("oauth_signature=" + OAuth.UrlEncode(Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(BaseString)))));

            parameters.Sort();

            string resp = "";
            WebResponse objWebResponse = null;
            try
            {
                if (httpmethod.ToLower() == "post")
                {
                    // POST
                    Uri objURI = new Uri(url);
                    WebRequest objWebRequest = WebRequest.Create(objURI);
                    //objWebRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    objWebRequest.Method = httpmethod.ToUpper();
                    objWebRequest.Proxy = Proxy;
                    objWebRequest.ContentType = "application/x-www-form-urlencoded";
                    objWebRequest.ContentLength = string.Join("&", parameters.ToArray()).Length;

                    StreamWriter stOut = new StreamWriter(objWebRequest.GetRequestStream(), System.Text.Encoding.ASCII);
                    stOut.Write(string.Join("&", parameters.ToArray()));
                    stOut.Close();

                    // Do the request to get the response
                    StreamReader stIn = new StreamReader(objWebRequest.GetResponse().GetResponseStream());
                    resp = stIn.ReadToEnd();
                    stIn.Close();
                }
                else
                {
                    // GET
                    //throw new FireEagleException(url + "?" + string.Join("&", parameters.ToArray()));
                    Uri objURI = new Uri(url + "?" + string.Join("&", parameters.ToArray()));
                    WebRequest objWebRequest = WebRequest.Create(objURI);
                    objWebRequest.Method = httpmethod.ToUpper();
                    //objWebRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    objWebRequest.Proxy = Proxy;
                    objWebResponse = objWebRequest.GetResponse();
                    Stream objStream = objWebResponse.GetResponseStream();
                    StreamReader objStreamReader = new StreamReader(objStream);

                    resp = objStreamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                //Helpers.WriteToExtraLog("WebException in GetStringResponse...", ex);
                string errorMessage = ex.Message;
                if (ex.Response != null) {
                    Stream objStream = ex.Response.GetResponseStream();
                    StreamReader objStreamReader = new StreamReader(objStream);
                    resp = objStreamReader.ReadToEnd();
                    //Helpers.WriteToExtraLog("Response: " + resp, null);
                    Response err = (Response)Deserialize(resp, typeof(Response));
                    errorMessage = err.Error.Message;
                }
                throw new FireEagleException(errorMessage);
            }
            finally {
                if (objWebResponse != null) {
                    objWebResponse.Close();
                }
            }

            return resp;
        }

        /// <summary>
        /// Return a tokenized response from a fire eagle method.
        /// </summary>
        /// <param name="httpmethod">Should be GET or POST. Not case-sensitive.</param>
        /// <param name="method">The Fire Eagle method to perform. (oauth/request_token, api/0.1/lookup, etc.)</param>
        /// <param name="token">The token used as part of the hash key (should be the secret). Pass an empty string for tokenless methods.</param>
        /// <param name="parameters">A list of all parameters to pass to the Fire Eagle method. Should include required OAuth parameters. (oauth_consumer, oauth_version, etc)</param>
        /// <returns>A token representing the response from the method called.</returns>
        private Token GetTokenResponse(string httpmethod, string method, string token, List<string> parameters)
        {
            string resp = GetStringResponse(httpmethod, "oauth/" + method, token, parameters);
            return new Token(resp);
        }

        /// <summary>
        /// Return a tokenized response from a fire eagle method. For methods that do not require a token.
        /// </summary>
        /// <param name="httpmethod">Should be GET or POST. Not case-sensitive.</param>
        /// <param name="method">The Fire Eagle method to perform. (oauth/request_token, api/0.1/lookup, etc.)</param>
        /// <param name="parameters">A list of all parameters to pass to the Fire Eagle method. Should include required OAuth parameters. (oauth_consumer, oauth_version, etc)</param>
        /// <returns>A token representing the response from the method called.</returns>
        private Token GetTokenResponse(string httpmethod, string method, List<string> parameters)
        {
            return GetTokenResponse(httpmethod, method, "", parameters);
        }

        /// <summary>
        /// Return a Response object from a fire eagle method.
        /// </summary>
        /// <param name="httpmethod">Should be GET or POST. Not case-sensitive.</param>
        /// <param name="method">The Fire Eagle method to perform. (oauth/request_token, api/0.1/lookup, etc.)</param>
        /// <param name="token">The token used as part of the hash key (should be the secret). Pass an empty string for tokenless methods.</param>
        /// <param name="parameters">A list of all parameters to pass to the Fire Eagle method. Should include required OAuth parameters. (oauth_consumer, oauth_version, etc)</param>
        /// <returns>A deserialized response representing the xml from the method called.</returns>
        private Response GetResponse(string httpmethod, string method, string token, List<string> parameters)
        {
            string resp = GetStringResponse(httpmethod, "api/0.1/" + method, token, parameters);
            return (Response)Deserialize(resp,typeof(Response));
        }

        /// <summary>
        /// Return a Response object from a fire eagle method. For methods that do not require a token.
        /// </summary>
        /// <param name="httpmethod">Should be GET or POST. Not case-sensitive.</param>
        /// <param name="method">The Fire Eagle method to perform. (oauth/request_token, api/0.1/lookup, etc.)</param>
        /// <param name="token">The token used as part of the hash key (should be the secret). Pass an empty string for tokenless methods.</param>
        /// <param name="parameters">A list of all parameters to pass to the Fire Eagle method. Should include required OAuth parameters. (oauth_consumer, oauth_version, etc)</param>
        /// <returns>A deserialized response representing the xml from the method called.</returns>
        private Response GetResponse(string httpmethod, string method, List<string> parameters)
        {
            return GetResponse(httpmethod, method, "", parameters);
        }

        /// <summary>
        /// Deserialize XML sent from Fire Eagle. More than likely should be a Response object.
        /// </summary>
        /// <param name="resp">An xml string.</param>
        /// <param name="type">The type that resp with deserialize to.</param>
        /// <returns>An object of Type type that represents the data in resp.</returns>
        private object Deserialize(string resp, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            try
            {
                Response response;
                using (StringReader responseReader = new StringReader(resp))
                {
                    response = (Response)serializer.Deserialize(responseReader);
                    responseReader.Close();
                }
                return response;
            }
            catch (InvalidOperationException ex)
            {
                // Serialization error occurred!
                throw new FireEagleException("Invalid response received from FireEagle.", ex);
            }
        }

        private void AddStandardParams(List<string> parameters, Token token)
        {
            parameters.Add("oauth_token=" + OAuth.UrlEncode(token.PublicToken));
            AddStandardParams(parameters);
        }

        private void AddStandardParams(List<string> parameters)
        {
            parameters.Add("oauth_consumer_key=" + OAuth.UrlEncode(m_api));
            parameters.Add("oauth_signature_method=HMAC-SHA1");
            parameters.Add("oauth_version=1.0");
            parameters.Add("oauth_timestamp=" + OAuth.UrlEncode(OAuth.GetTimestamp()));
            parameters.Add("oauth_nonce=" + OAuth.UrlEncode(OAuth.GenerateNonce()));
		
        }

        /// <summary>
        /// Update a user's location.
        /// </summary>
        /// <param name="parameter">All required parameters to update a user location (including user token)</param>
        private void Update(string parameter)
        {
            List<string> parameters = new List<string>();
            parameters.Add(parameter);
            Update(parameters);
        }

        private void Update(List<string> parameters)
        {
            AddStandardParams(parameters, m_token);
            Response resp = GetResponse("post", "update", m_token.SecretToken, parameters);

        }

        #endregion

        #region [Public Methods]
        public Token OAuthGetRequestToken() {
            List<string> parameters = new List<string>();
            AddStandardParams(parameters);
            parameters.Add("oauth_callback=oob"); // Hineini has no "callback URL to which the Service Provider will redirect the User back when the Obtaining User Authorization (Obtaining User Authorization) step is completed" -- http://oauth.googlecode.com/svn/spec/core/1.0a/drafts/3/oauth-core-1_0a.html
            return GetTokenResponse("get", "request_token", parameters);
        }

        public string OAuthRequestUrl(Token token)
        {
            return m_siteUrl + "oauth/authorize?oauth_token=" + token.PublicToken;
        }

        public Token OAuthGetToken(Token token, string verifierToken)
        {
            List<string> parameters = new List<string>();
            AddStandardParams(parameters, token);
            parameters.Add("oauth_verifier=" + OAuth.UrlEncode(verifierToken));
            return GetTokenResponse("get", "access_token", token.SecretToken, parameters);
        }

        public User User()
        {
            List<string> parameters = new List<string>();
            AddStandardParams(parameters, m_token);
            Response resp = GetResponse("get", "user", m_token.SecretToken, parameters);
            return resp.User;

        }

        public User Recent()
        {
            return Recent(DateTime.Now, 0, 10);
        }

        public User Recent(DateTime time)
        {
            return Recent(time, 0, 10);
        }

        public User Recent(DateTime time, int page)
        {
            return Recent(time, page, 10);
        }

        public User Recent(DateTime time, int page, int PerPage)
        {
            List<string> parameters = new List<string>();

            parameters.Add("time=" + OAuth.UrlEncode(time.ToShortDateString() + " " + time.ToShortTimeString()));
            parameters.Add("page=" + OAuth.UrlEncode(page.ToString()));
            parameters.Add("per_page=" + OAuth.UrlEncode(PerPage.ToString()));

            AddStandardParams(parameters, m_general);

            Response resp = GetResponse("get", "recent", m_general.SecretToken, parameters);

            return resp.User;
        }

        public Users Within(string place_id, int woe_id)
        {
            List<string> parameters = new List<string>();

            parameters.Add("place_id=" + OAuth.UrlEncode(place_id.ToString()));
            parameters.Add("woe_id=" + OAuth.UrlEncode(woe_id.ToString()));
            AddStandardParams(parameters, m_general);
            Response resp = GetResponse("get", "within", m_general.SecretToken, parameters);

            return resp.Users;
        }

        public Locations Lookup(Address address) {
            List<string> parameters = new List<string>();

            if (!string.IsNullOrEmpty(address.StreetAddress))
                parameters.Add("address=" + OAuth.UrlEncode(address.StreetAddress.ToString()));
            if (!string.IsNullOrEmpty(address.Postal))
                parameters.Add("postal=" + OAuth.UrlEncode(address.Postal.ToString()));
            if (!string.IsNullOrEmpty(address.City))
                parameters.Add("city=" + OAuth.UrlEncode(address.City.ToString()));
            if (!string.IsNullOrEmpty(address.State))
                parameters.Add("state=" + OAuth.UrlEncode(address.State.ToString()));
            if (!string.IsNullOrEmpty(address.Country))
                parameters.Add("country=" + OAuth.UrlEncode(address.Country.ToString()));

            Response resp = Lookup(parameters);

            return resp.Locations;
        }

        public Locations Lookup(CellTower cellTower) {
            List<string> parameters = new List<string>();

            parameters.Add("cellid=" + OAuth.UrlEncode(cellTower.cellid.ToString()));
            parameters.Add("lac=" + OAuth.UrlEncode(cellTower.lac.ToString()));
            parameters.Add("mcc=" + OAuth.UrlEncode(cellTower.mcc.ToString()));
            parameters.Add("mnc=" + OAuth.UrlEncode(cellTower.mnc.ToString()));

            Response resp = Lookup(parameters);

            return resp.Locations;
        }

        public Locations Lookup(string address) {
            List<string> parameters = new List<string>();

            parameters.Add("address=" + OAuth.UrlEncode(address));

            Response resp = Lookup(parameters);

            return resp.Locations;
        }

        private Response Lookup(List<string> parameters) {
            AddStandardParams(parameters, m_token);
            Response resp = GetResponse("get", "lookup", m_token.SecretToken, parameters);

            resp.Locations.QueryString = resp.QueryString;
            return resp;
        }

        public void Update(LocationType locationType, string location)
        {
            Update(locationType.ToString() + "=" + OAuth.UrlEncode(location));
        }

        public void Update(LocationType locationType, int location)
        {
            Update(locationType.ToString() + "=" + OAuth.UrlEncode(location.ToString()));
        }

        public void Update(CellTower cell) {
            List<string> parameters = new List<string>();
            parameters.Add("mnc=" + OAuth.UrlEncode(cell.mnc.ToString()));
            parameters.Add("mcc=" + OAuth.UrlEncode(cell.mcc.ToString()));
            parameters.Add("lac=" + OAuth.UrlEncode(cell.lac.ToString()));
            parameters.Add("cellid=" + OAuth.UrlEncode(cell.cellid.ToString()));

            Update(parameters);
        }

        public void Update(Address address)
        {
            List<string> parameters = new List<string>();
            if (!string.IsNullOrEmpty(address.StreetAddress))
                parameters.Add("address=" + OAuth.UrlEncode(address.StreetAddress.ToString()));
            if (!string.IsNullOrEmpty(address.Postal))
                parameters.Add("postal=" + OAuth.UrlEncode(address.Postal.ToString()));
            if (!string.IsNullOrEmpty(address.City))
                parameters.Add("city=" + OAuth.UrlEncode(address.City.ToString()));
            if (!string.IsNullOrEmpty(address.State))
                parameters.Add("state=" + OAuth.UrlEncode(address.State.ToString()));
            if (!string.IsNullOrEmpty(address.Country))
                parameters.Add("country=" + OAuth.UrlEncode(address.Country.ToString()));

            Update(parameters);
        }

        public void Update(Position pos)
        {
            List<string> parameters = new List<string>();
            string latitude = pos.Latitude.ToString();
            string longitude = pos.Longitude.ToString();
            Helpers.WriteToExtraLog("Updating Fire Eagle position with lat/long: '" + latitude + "', '" + longitude + "'", null);
            parameters.Add("lat=" + OAuth.UrlEncode(latitude));
            parameters.Add("lon=" + OAuth.UrlEncode(longitude));

            Update(parameters);
        }
        #endregion

    }

    /// <summary>
    /// The types of location input/output supported by Fire Eagle.
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// Latitude and Longitude
        /// </summary>
        LatLon,

        /// <summary>
        /// As used on Flickr and Upcoming, valid values decrypts to an integer value. Recommend using woeid instead of place_id.
        /// </summary>
        [XmlEnum("place_id")]
        place_id,

        /// <summary>
        /// 32-bit identifier that uniquely represents spatial entities.
        /// </summary>
        [XmlEnum("woeid")]
        woe_id,
		
        /// <summary>
        /// A GeoJSON/GeoRSS element such as a bounding box, polygon, or point.
        /// </summary>
        [XmlEnum("georss")]
        geom,

        /// <summary>
        /// Street address (may contain a full address, but will be combined with postal, city, state, and country if those values are available).
        /// </summary>
        [XmlEnum("address")]
        address,

        /// <summary>
        /// Cell tower information, all values (mnc, mcc, lac, cellid) are in integers and required for a valid tower location.
        /// </summary>
        CellTower,
		    
        /// <summary>
        /// Exact address
        /// </summary>
        [XmlEnum("exact")]
        exact,
        
        /// <summary>
        /// A ZIP or postal code (combined with address, city, state, and country if those values are available).
        /// </summary>
        [XmlEnum("postal")]
        postal,

        /// <summary>
        /// City (combined with address, postal, state, and country if those values are available).
        /// </summary>
        [XmlEnum("city")]
        city,

        /// <summary>
        /// Neighborhood
        /// </summary>
        [XmlEnum("neighborhood")]
        neighborhood,

        /// <summary>
        /// State (combined with address, postal, city, and country if those values are available).
        /// </summary>
        [XmlEnum("state")]
        state,

        /// <summary>
        /// Region
        /// </summary>
        [XmlEnum("region")]
        region,

        /// <summary>
        /// Country (combined with address, postal, city, and state if those values are available).
        /// </summary>
        [XmlEnum("country")]
        country,
    }

    /// <summary>
    /// Struct representing multiple properties pertaining to the same location.
    /// </summary>
    /// <remarks>
    /// Will eventually deprecate and be moved into <see cref="Location"/>.
    /// </remarks>
    public struct Address
    {
        public string StreetAddress;
        public string Postal;
        public string City;
        public string State;
        public string Country;
    }

    /// <summary>
    /// Struct representing a Cell Tower location.
    /// </summary>
    /// <remarks>
    /// Will eventually deprecate and be moved into <see cref="Location"/>.
    /// </remarks>
    public struct CellTower
    {
        public int mnc, mcc, lac, cellid;

        public CellTower(int MNC, int MCC, int LAC, int CellID) {
            mnc = MNC;
            mcc = MCC;
            lac = LAC;
            cellid = CellID;
        }
    }

    /// <summary>
    /// Struct representing latitude and longitude.
    /// </summary>
    public struct Position
    {
        public double Latitude;
        public double Longitude;

        public Position(double Lat, double Lon)
        {
            Latitude = Lat;
            Longitude = Lon;
        }
    }
}