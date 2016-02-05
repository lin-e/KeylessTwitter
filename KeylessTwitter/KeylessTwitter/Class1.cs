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
                HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/tweet/create", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&is_permalink_page=false&page_context=profile&place_id=&status={1}&tagged_users=&media_ids={2}", authenticityToken, HttpUtility.UrlEncode(toTweet), mediaID)), "application/x-www-form-urlencoded", true);
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
        public void SendMessage(string targetUser, string messageContents)
        {
            if (twitterStatus != TwitterAuthStatus.Success)
            {
                throw new InvalidTwitterActionException("You are not signed in");
            }
            string conversationID = "";
            Regex getUserID = new Regex("data-screen-name=\"(.*?)\" data-name=\"(.*?)\" data-user-id=\"(.*?)\"");
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_GET("https://twitter.com/" + targetUser, new string[][] { }, "");
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    string responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    foreach (Match singleMatch in getUserID.Matches(responseText))
                    {
                        if (singleMatch.Groups[1].ToString().ToLower() == targetUser.ToLower())
                        {
                            conversationID += singleMatch.Groups[3];
                            break;
                        }
                    }
                    conversationID += "-";
                }
            }
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_GET("https://twitter.com/", new string[][] { }, "");
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    string responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    foreach (Match singleMatch in getUserID.Matches(responseText))
                    {
                        if (singleMatch.Groups[1].ToString().ToLower() == authUsername.ToLower())
                        {
                            conversationID += singleMatch.Groups[3];
                            break;
                        }
                    }
                }
            }
            if (true)
            {
                try
                {
                    HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/direct_messages/new", new string[][] { }, Encoding.ASCII.GetBytes(
                    string.Format("authenticity_token={0}&conversation_id={2}&scribeContext%5Bcomponent%5D=tweet_box_dm&tagged_users=&text={1}", authenticityToken, HttpUtility.UrlEncode(messageContents), conversationID)), "application/x-www-form-urlencoded", true);
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
                }
                catch
                {
                    throw new InvalidTwitterActionException("You may not be able to DM this user");
                }
            }
        }
        public void Follow(string toFollow)
        {
            Regex getUserID = new Regex("data-screen-name=\"(.*?)\" data-name=\"(.*?)\" data-user-id=\"(.*?)\"");
            string userID = "";
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_GET("https://twitter.com/" + toFollow, new string[][] { }, "");
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    string responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    foreach (Match singleMatch in getUserID.Matches(responseText))
                    {
                        if (singleMatch.Groups[1].ToString().ToLower() == toFollow.ToLower())
                        {
                            userID = singleMatch.Groups[3].ToString();
                            break;
                        }
                    }
                }
            }
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/user/follow", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&challenges_passed=false&handles_challenges=1&user_id={1}", authenticityToken, userID)), "application/x-www-form-urlencoded; charset=UTF-8", true, "https://twitter.com/" + toFollow);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }

            }
            Console.WriteLine(userID);
        }
        public void Unfollow(string toFollow)
        {
            Regex getUserID = new Regex("data-screen-name=\"(.*?)\" data-name=\"(.*?)\" data-user-id=\"(.*?)\"");
            string userID = "";
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_GET("https://twitter.com/" + toFollow, new string[][] { }, "");
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    string responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    foreach (Match singleMatch in getUserID.Matches(responseText))
                    {
                        if (singleMatch.Groups[1].ToString().ToLower() == toFollow.ToLower())
                        {
                            userID = singleMatch.Groups[3].ToString();
                            break;
                        }
                    }
                }
            }
            if (true)
            {
                HttpWebRequest webRequest = createWebRequest_POST("https://twitter.com/i/user/unfollow", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("authenticity_token={0}&challenges_passed=false&handles_challenges=1&user_id={1}", authenticityToken, userID)), "application/x-www-form-urlencoded; charset=UTF-8", true, "https://twitter.com/" + toFollow);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }

            }
            Console.WriteLine(userID);
        }
        public enum TwitterAuthStatus
        {
            Unknown,
            Failed,
            Success,
            SigningIn
        }
        #region webRequestFunctions
        public HttpWebRequest createWebRequest_POST(string finalURL, string[][] requestHeaders, byte[] postData, string contentType, bool isTweet = false, string refererURL = "")
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
            if (refererURL != "")
            {
                webRequest.Referer = refererURL;
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
