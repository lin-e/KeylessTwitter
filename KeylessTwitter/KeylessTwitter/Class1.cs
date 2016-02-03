using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
namespace KeylessTwitter
{
    public class Twitter
    {
        public string authUsername;
        public string authPassword;
        public TwitterAuthStatus twitterStatus = TwitterAuthStatus.Unknown;
        string authenticityToken;
        CookieContainer mainContainer = new CookieContainer();
        public Twitter(string loginUser, string loginPass)
        {
            authUsername = loginUser;
            authPassword = loginPass;
        }
        public bool tryLogin()
        {
            if (new TwitterAuthStatus[] { TwitterAuthStatus.SigningIn, TwitterAuthStatus.Success }.Contains(twitterStatus))
            {
                throw new InvalidTwitterActionException("You are already sign(ing) in");
            }
            twitterStatus = TwitterAuthStatus.SigningIn;
            try
            {
                if (true)
                {
                    HttpWebRequest webRequest = createWebRequest_GET("https://twitter.com/", new string[][] { }, "");
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        authenticityToken = new Regex("<input type=\"hidden\" value=\"(.*?)\" name=\"authenticity_token\">").Match(new StreamReader(webResponse.GetResponseStream()).ReadToEnd()).Groups[1].ToString();
                    }
                }
                if (true)
                {
                    string baseSubmit = "session%5Busername_or_email%5D={0}&session%5Bpassword%5D={1}&return_to_ssl=true&scribe_log=&redirect_after_login=%2F&authenticity_token={2}&js_inst=1";
                    HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/sessions", new string[][] { }, Encoding.ASCII.GetBytes(string.Format(baseSubmit, authUsername, authPassword, authenticityToken)), "application/x-www-form-urlencoded");
                    bool containsLangCookie = false;
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        foreach (Cookie singleResponseCookie in webResponse.Cookies)
                        {
                            if (singleResponseCookie.Name == "lang")
                            {
                                containsLangCookie = true;
                                break;
                            }
                        }
                    }
                    if (!(containsLangCookie))
                    {
                        throw new InvalidTwitterActionException("Failed to sign in");
                    }
                }
                twitterStatus = TwitterAuthStatus.Success;
                return true;
            }
            catch
            {
                twitterStatus = TwitterAuthStatus.Failed;
                return false;
            }
        }
        public void Tweet(string toTweet)
        {
            if (twitterStatus != TwitterAuthStatus.Success)
            {
                throw new InvalidTwitterActionException("You are not signed in");
            }
            HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/tweet/create", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&is_permalink_page=false&page_context=profile&place_id=&status={1}&tagged_users=", authenticityToken, HttpUtility.UrlEncode(toTweet))), "application/x-www-form-urlencoded", true);
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
        }
        public void TweetImage(byte[] imageData, string toTweet = "")
        {
            if (twitterStatus != TwitterAuthStatus.Success)
            {
                throw new InvalidTwitterActionException("You are not signed in");
            }
            string mediaID = "";
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_POST("https://upload.twitter.com/i/media/upload.json?origin=https%3A%2F%2Ftwitter.com", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&media={1}&origin=https%3A%2F%2Ftwitter.com", authenticityToken, HttpUtility.UrlEncode(Convert.ToBase64String(imageData)))), "application/x-www-form-urlencoded", true);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    mediaID = new Regex("{\"media_id\":(.*?),\"media_id_string\"").Match(new StreamReader(webResponse.GetResponseStream()).ReadToEnd()).Groups[1].ToString();
                }
            }
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/tweet/create", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&is_permalink_page=false&page_context=profile&place_id=&status={1}&tagged_users=&media_ids={2}&tweetboxId=swift_tweetbox_1454534494328", authenticityToken, HttpUtility.UrlEncode(toTweet), mediaID)), "application/x-www-form-urlencoded", true);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
            }
        }
        public void TweetImages(byte[][] imageData, string toTweet = "")
        {
            if (twitterStatus != TwitterAuthStatus.Success)
            {
                throw new InvalidTwitterActionException("You are not signed in");
            }
            List<string> mediaIDs = new List<string>();
            if (true)
            {
                foreach (byte[] singleImage in imageData)
                {
                    HttpWebRequest webRequest = createWebRequest_POST("https://upload.twitter.com/i/media/upload.json?origin=https%3A%2F%2Ftwitter.com", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&media={1}&origin=https%3A%2F%2Ftwitter.com", authenticityToken, HttpUtility.UrlEncode(Convert.ToBase64String(singleImage)))), "application/x-www-form-urlencoded", true);
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        mediaIDs.Add(new Regex("{\"media_id\":(.*?),\"media_id_string\"").Match(new StreamReader(webResponse.GetResponseStream()).ReadToEnd()).Groups[1].ToString());
                    }
                }
            }
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/tweet/create", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&is_permalink_page=false&page_context=profile&place_id=&status={1}&tagged_users=&media_ids={2}", authenticityToken, HttpUtility.UrlEncode(toTweet), string.Join(HttpUtility.HtmlEncode(","), mediaIDs))), "application/x-www-form-urlencoded", true);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
            }
        }
        public enum TwitterAuthStatus
        {
            Unknown,
            Failed,
            Success,
            SigningIn
        }
        #region webRequestFunctions
        public HttpWebRequest createWebRequest_POST(string finalURL, string[][] requestHeaders, byte[] postData, string contentType, bool isTweet = false)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(finalURL);
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "POST";
            if (isTweet)
            {
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                webRequest.Referer = "https://twitter.com/";
                webRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
            }
            webRequest.ContentType = contentType;
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            webRequest.ContentLength = postData.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
            }
            return webRequest;
        }
        public HttpWebRequest createWebRequest_GET(string finalURL, string[][] requestHeaders, string contentType)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(finalURL);
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "GET";
            if (contentType != "")
            {
                webRequest.ContentType = contentType;
            }
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            return webRequest;
        }
        public HttpWebRequest createWebRequest_PUT(string finalURL, string[][] requestHeaders, byte[] postData, string contentType)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(finalURL);
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "PUT";
            webRequest.ContentType = contentType;
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            webRequest.ContentLength = postData.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
            }
            return webRequest;
        }
        #endregion
        #region customException
        public class InvalidTwitterActionException : Exception
        {
            public InvalidTwitterActionException() { }
            public InvalidTwitterActionException(string message) : base(message) { }
            public InvalidTwitterActionException(string message, Exception inner) : base(message, inner) { }
        }
        #endregion
    }
}
