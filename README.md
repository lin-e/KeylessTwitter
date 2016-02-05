# KeylessTwitter
A small module I was working on for a friend's project that allows (very) basic interaction with Twitter without using an API key
# Dependencies
- [Json.NET] {http://www.newtonsoft.com/json}
- 
# Features
- [x] Signing in
- [x] Sending simple tweets
- [x] Sending tweets with media
- [x] Sending tweets with more than one image (media item)
- [x] Direct Messages
- [x] DM Received Event
- [ ] New Tweet Event
- [ ] Notifications
- [x] Following / Unfollowing
- [ ] A lot more...

# Usage
Starting usage
```
Twitter mainTwitter = new Twitter("USERNAME", "PASSWORD"); // Creates a new Twitter with the specified credentials
mainTwitter.tryLogin(); // Logs in to the Twitter account (returns true or false depending on login status)
mainTwitter.directMessage += MainTwitter_directMessage; // Optional event handler, which fires when the user receives a DM
```
Sending a tweet
```
mainTwitter.Tweet("Hello world! #KeylessTwitter"); // Tweets the specified tweet. Remember, there is a 140 character limit, which will cause a 403 if exceeded
```
Tweeting an image (by URL)
```
// The general idea is that they use byte[], which allows for you to load from a file if needed
mainTwitter.TweetImage(new WebClient().DownloadData("http://example.com/my_twitter_picture.jpg"), "Check out this image!"); // Tweets from said URL
```
Tweeting images (by URLs)
```
mainTwitter.TweetImages(new byte[][] { new WebClient().DownloadData("http://example.com/my_twitter_picture.jpg"), new WebClient().DownloadData("http://example.com/my_other_picture.jpg") }, "Check out these images!"); // Tweets from the specified URLs
```
Follow / Unfollow a user
```
mainTwitter.Follow("c0mmodity"); // Follows me (shameless plug)
mainTwitter.Unfollow("c0mmodity"); // Unfollows specified user
```
DM Event Example
```
private static void MainTwitter_directMessage(string conversationID, string messageContents) // Auto generated
{
    Console.WriteLine("{0}: {1}", conversationID, messageContents); // Outputs the message and its conversation ID
    mainTwitter.SendMessage_ID(conversationID, messageContents); // Replies with the same message (using Twitter.SendMessage_ID)
}
```
