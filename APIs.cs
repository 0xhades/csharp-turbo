using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace hydra {
    public class Profile {
        public string username;
        public string biography;
        public string full_name;
        public string phone_number;
        public string email;
        public string gender;
        public string external_url;
    }
   class APIs {
       public CookieContainer cookies = new CookieContainer();
       private Random random = new Random();
       public Profile GetProfile(string SessionID = null) {
            Profile Profile = new Profile();
            HttpWebRequest req = WebRequest.CreateHttp("https://i.instagram.com/api/v1/accounts/current_user/?edit=true");
            req.UserAgent = $"Instagram {random.Next(5, 50)}.{random.Next(6, 10)}.{random.Next(0, 10)} Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)";
            if (SessionID != null)
                req.Headers.Add("Cookie", $"sessionid={SessionID}");
            else
                req.CookieContainer = cookies;
            req.KeepAlive = false;
            req.ProtocolVersion = HttpVersion.Version10;
            req.Proxy = null;
            req.ServicePoint.UseNagleAlgorithm = false;
            req.ServicePoint.Expect100Continue = false;

            try {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                string Content = new StreamReader(res.GetResponseStream()).ReadToEnd();
                
                if (Content.ToLower().Contains("ok")) {
                    Profile.username = Regex.Match(Content, "\"username\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    Profile.email = Regex.Match(Content, "\"email\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    Profile.biography = Regex.Match(Content, "\"biography\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    Profile.full_name = Regex.Match(Content, "\"full_name\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    Profile.phone_number = Regex.Match(Content, "\"phone_number\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    Profile.gender = Regex.Match(Content, "\"gender\": (.*?),").Groups[1].Value.Replace(" ", "");
                    Profile.external_url = Regex.Match(Content, "\"external_url\": \"(.*?)\"").Groups[1].Value.Replace(" ", "");
                    return Profile;
                }
            } catch (WebException) {
                // HttpWebResponse HttpResponse = (HttpWebResponse)ex.Response;
                // StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
            }

            return null; 
        }
        public void webHook(string username, int attempts) {

            if (username.Length > 4) {
		        return;
	        }

            string data = "{\"embeds\": [{\"description\": \"Swapped Successfully!\\nAttempts: " + attempts + "\\nBy Fxler @zq\", \"title\": \"@" + username + "\", \"color\": 977678, \"image\": {\"url\": \"https://i.imgur.com/QyoizqY.jpg\"}}], \"username\": \"Titan\"}";
            string url = "https://discord.com/api/webhooks/776547226990149694/FHq3sf39N4vIqayR5yUBsglSgsNbNBt72g13FicopW8KywfcdL6yoFIV0lWBw0k3_lcF";
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = (long)bytes.Length;
            Stream StreamWriter = request.GetRequestStream();

            StreamWriter.Write(bytes, 0, bytes.Length);
            StreamWriter.Flush();
            StreamWriter.Close();
            StreamWriter.Dispose();

            try {
                HttpWebResponse HttpResponse = (HttpWebResponse)request.GetResponse();
            } catch {}
    
        }
        public string Request(string url, ref HttpStatusCode statuscode, string data = null) {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("User-Agent", $"Instagram 155.0.0. Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)");
            headers.Add("Cookie2", "$Version=1");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("X-IG-Connection-Type", "WIFI");
            headers.Add("Accept-Language", "en-US");
            headers.Add("X-FB-HTTP-Engine", "Liger");
            headers.Add("X-IG-Capabilities", "3brTBw==");
            headers.Add("Connection", "Close");

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers = headers;
            if (data != null) {
                request.Method = "POST";
            } else {
                request.Method = "GET";
            }
            request.Accept = "*/*";
            request.KeepAlive = false;
            request.CookieContainer = cookies;
            request.ProtocolVersion = HttpVersion.Version11;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;

            if (data != null) {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                request.ContentLength = (long)bytes.Length;
                Stream StreamWriter = request.GetRequestStream();

                StreamWriter.Write(bytes, 0, bytes.Length);
                StreamWriter.Flush();
                StreamWriter.Close();
                StreamWriter.Dispose();
            }

            string Content = null;

            try {
                HttpWebResponse HttpResponse = (HttpWebResponse)request.GetResponse();
                StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
                Content = Reader.ReadToEnd();
                statuscode = HttpResponse.StatusCode;
            }

            //secure login
            catch (WebException ex) {
                HttpWebResponse HttpResponse = (HttpWebResponse)ex.Response;
                StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
                Content = Reader.ReadToEnd();
                statuscode = HttpResponse.StatusCode;
            }

            return Content;
        }

        public bool checkBlock(string sessionid, Profile profile, Dictionary<string, string> body) {

            Dictionary<string, string> _body_ = body; 
            _body_.Add("username", profile.username + ".tita");
            string JSON = DictToJSON(_body_);
            string _body = $"signed_body=SIGNATURE.{JSON}";

            string url = "https://i.instagram.com/api/v1/accounts/edit_profile/";
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            string Result = Request(url, ref statusCode, _body);

            string _url = "https://i.instagram.com/api/v1/accounts/set_username/";
            HttpStatusCode _statusCode = HttpStatusCode.BadRequest;
            string _Result = Request(_url, ref _statusCode, "username=" + profile.username + ".titan");

            if (Result.Contains("user", StringComparison.CurrentCultureIgnoreCase) && statusCode == HttpStatusCode.OK && _Result.Contains("user", StringComparison.CurrentCultureIgnoreCase) && _statusCode == HttpStatusCode.OK) {
                return true;
            }
            return false;

        }

        public bool updateBTH(string sessionid) {

            string url = "https://i.instagram.com/api/v1/consent/update_dob/";
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            string Result = Request(url, ref statusCode, "SIGNATURE.{\"current_screen_key\":\"dob\",\"day\":\"1\",\"year\":\"1998\",\"month\":\"1\"}");

            if (statusCode == HttpStatusCode.OK) {
                return true;
            }
            return false;

        }

        public string Login(string username, string password) {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("User-Agent", $"Instagram {random.Next(5, 50)}.{random.Next(6, 10)}.{random.Next(0, 10)} Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)");
            headers.Add("Cookie2", "$Version=1");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("X-IG-Connection-Type", "WIFI");
            headers.Add("Accept-Language", "en-US");
            headers.Add("X-FB-HTTP-Engine", "Liger");
            headers.Add("X-IG-Capabilities", "3brTBw==");
            headers.Add("Connection", "Close");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("jazoest", "22713");
            data.Add("phone_id", Guid.NewGuid().ToString());
            data.Add("enc_password", $"#PWD_INSTAGRAM_BROWSER:0:{(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds}:{password}");
            data.Add("_csrftoken", "missing");
            data.Add("username", username);
            data.Add("adid", Guid.NewGuid().ToString());
            data.Add("device_id", $"android-{RandomString(16)}");
            data.Add("guid", Guid.NewGuid().ToString());
            data.Add("google_tokens", "[]");
            data.Add("login_attempt_count", "0");

            string JSON = DictToJSON(data);
		    string body = $"signed_body=SIGNATURE.{JSON}";

            HttpWebRequest request = WebRequest.CreateHttp("https://i.instagram.com/api/v1/accounts/login/");
            request.Headers = headers;
            request.Method = "POST";
            request.Accept = "*/*";
            request.KeepAlive = false;
            request.CookieContainer = cookies;
            request.ProtocolVersion = HttpVersion.Version11;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            request.ContentLength = (long)bytes.Length;
            Stream StreamWriter = request.GetRequestStream();
            StreamWriter.Write(bytes, 0, bytes.Length);
            StreamWriter.Flush();
            StreamWriter.Close();
            StreamWriter.Dispose();
            string Content = null;

            try {
                HttpWebResponse HttpResponse = (HttpWebResponse)request.GetResponse();
                StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
                Content = Reader.ReadToEnd();
                foreach (Cookie Cookie in cookies.GetCookies(new Uri("https://i.instagram.com/api/v1/accounts/login/"))) {
                    if (Cookie.Name == "sessionid")
                        return Cookie.Value;
                }
            }

            //secure login
            catch (WebException ex) {
                HttpWebResponse HttpResponse = (HttpWebResponse)ex.Response;
                StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
                Content = Reader.ReadToEnd();
                if (Content.Contains("challenge_required", StringComparison.CurrentCultureIgnoreCase)) {

                    Console.WriteLine($"challenge required!");

                }
            }

            return null;
        }
        public string DictToJSON(Dictionary<string, string> dict) {
            int i = 0;
            string json = "{";
            foreach (KeyValuePair<string, string> entry in dict) {
                json = ((i != dict.Count - 1) ? (json + "\"" + entry.Key + "\":\"" + entry.Value + "\",") : (json + "\"" + entry.Key + "\":\"" + entry.Value + "\""));
                i++;
            }
            return json + "}";
        }
        public string RandomString(int length) {
		    return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower(), length).Select((Func<string, char>)((string s) => s[random.Next(s.Length)])).ToArray());
	    }
    }
}
