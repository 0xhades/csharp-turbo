using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using SocketProxy.Proxy;

namespace hydra
{
    class Program
    {
        static public APIs API = null;
        static public EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        static public EventWaitHandle EndWait = new EventWaitHandle(false, EventResetMode.ManualReset);
        static public EventWaitHandle waitBlocked = new EventWaitHandle(false, EventResetMode.ManualReset);
        static public EventWaitHandle waitClaimed = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static string SessionID;
        static public string Username = null;
        static public Profile profile = null;
        static public Dictionary<string, string> data = new Dictionary<string, string>();
        static public Dictionary<string, string> Set_data = new Dictionary<string, string>();
        public static volatile int SetBlockingCounter;
        public static volatile int BlockingCounter;
        public static volatile int EditBlockingCounter;
        public static volatile int Success;
        public static volatile int connectedClients;
        public static volatile int RequestHandled;
        static public string Password = null;
        static public string Target = null;
        static public string last = null;
        static public int Workers = 5;
        static public int sleep = 5;
        static public int Clients = 0;
        static public bool check = false;
        static public int Loops = 5;
        static public int Speed = 0;
        static public bool claim;
        public static volatile int success;
        public static volatile int sent;
        static public bool stopS = false;
        static public bool stopC = false;
        static public bool stopB = false;
        static public bool stop = false;
        static public Object thisLock = new Object(); 
        static public Random random = new Random();
        static public List<HttpClient> httpClients = new List<HttpClient>();
        static public string banner = @"

 ██▀███   ██▓ ██▓███   ██▓███  ▓█████  ██▀███  
▓██ ▒ ██▒▓██▒▓██░  ██▒▓██░  ██▒▓█   ▀ ▓██ ▒ ██▒
▓██ ░▄█ ▒▒██▒▓██░ ██▓▒▓██░ ██▓▒▒███   ▓██ ░▄█ ▒
▒██▀▀█▄  ░██░▒██▄█▓▒ ▒▒██▄█▓▒ ▒▒▓█  ▄ ▒██▀▀█▄  
░██▓ ▒██▒░██░▒██▒ ░  ░▒██▒ ░  ░░▒████▒░██▓ ▒██▒
░ ▒▓ ░▒▓░░▓  ▒▓▒░ ░  ░▒▓▒░ ░  ░░░ ▒░ ░░ ▒▓ ░▒▓░
  ░▒ ░ ▒░ ▒ ░░▒ ░     ░▒ ░      ░ ░  ░  ░▒ ░ ▒░
  ░░   ░  ▒ ░░░       ░░          ░     ░░   ░ 
   ░      ░                       ░  ░   ░     

        ";
        static void Main(string[] args) {

            API = new APIs();
	    
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(banner);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("By Hades, @0xhades");
            Console.WriteLine();

            while (true) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Session ID [S] / Login [L]: ");
                Console.ForegroundColor = ConsoleColor.White;
                
                if (Console.ReadLine().Equals("l", StringComparison.CurrentCultureIgnoreCase)) {
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Username: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Username = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Password: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Password = Console.ReadLine();

                    SessionID = API.Login(Username, Password);
                    if (SessionID == null) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"@{Username} Didn't logged in for some reason");
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("There's something wrong!, try again? [Y/N]: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (Console.ReadLine().Equals("n", StringComparison.CurrentCultureIgnoreCase)) {
                            return;
                        } else {
                            continue;
                        }

                    }

                    profile = API.GetProfile(SessionID);
                    if (profile == null) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"@{Username} Didn't get profile for some reason");
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("There's something wrong!, try again? [Y/N]: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (Console.ReadLine().Equals("n", StringComparison.CurrentCultureIgnoreCase)) {
                            return;
                        } else {
                            continue;
                        }
                        
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"@{Username} Logged in Successfully");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Session ID: {SessionID}");
                    break;

                } else {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Session ID: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    SessionID = Console.ReadLine();
                    
                    profile = API.GetProfile(SessionID);
                    if (profile == null) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"this ID isn't a valid session ID");
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("There's something wrong!, try again? [Y/N]: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (Console.ReadLine().Equals("n", StringComparison.CurrentCultureIgnoreCase)) {
                            return;
                        } else {
                            continue;
                        }
                    }

                    Username = profile.username;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"@{Username} Logged in Successfully");
                    break;
                    
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Target: ");
            Console.ForegroundColor = ConsoleColor.White;
		    Target = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Check if you have been spamming? Be aware that this will");
	        Console.Write("Change the current username to @" + Username + ".titan (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
		    string choic = Console.ReadLine();

            if (choic.Equals("y", StringComparison.CurrentCultureIgnoreCase)) {
                check = true;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Prepare the payloads...");
    
            data.Add("username", Target);               
            if (profile.phone_number != null) data.Add("phone_number", profile.phone_number);
            if (profile.email != null) data.Add("email", profile.email);

            Set_data.Add("username", Target); 
           
            if (check) {
                if (!API.checkBlock(SessionID, profile, data)) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Your account is blocked, or your IP is blocked");
                    Environment.Exit(1);
                }
            }

            Workers = 1;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter Threads (OverPowered=5, Skip=1): ");
            Console.ForegroundColor = ConsoleColor.White;
		    var outin1 = Console.ReadLine();
            if (outin1 != "") {
                Workers = int.Parse(outin1);
            }

            Loops = 60;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter Loops (No=0, Ultimate=100, Skip=60): ");
            Console.ForegroundColor = ConsoleColor.White;
            var outin2 = Console.ReadLine();
            if (outin2 != "") {
                Loops = int.Parse(outin2);
            }

            Clients = 150;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter Clients Pool (Ultimate=500, Skip=150): ");
            Console.ForegroundColor = ConsoleColor.White;
            var outin3 = Console.ReadLine();
            if (outin3 != "") {
                Clients = int.Parse(outin3);
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(banner);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("By Hades, @0xhades");
            Console.WriteLine();
            
            for (int i = 0; i <= Workers; i++) {

                Thread t = new Thread(() => { RequestSender(); });
                t.Priority = ThreadPriority.Highest;
                t.IsBackground = true;
                t.Start();

            }

            Task.Run(() => {
                connect();
            });

            while (connectedClients != Clients) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Connecting all clients...  {connectedClients}/{Clients} \r");
                Thread.Sleep(10);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"All {connectedClients} Clients Connected Successfully");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Click any key to start...");
            Console.ReadLine();

            //WindowManager.Interaction.MsgBox("Hydra @0xhades", "Ready?", WindowManager.MsgBoxStyle.OkOnly);
            
            waitHandle.Set();

            Task.Run(() => {
                SuperVisior();
            });
            
            EndWait.WaitOne();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Click any key to exit..");
            Console.ReadLine();

        }
        public static void RequestSender() {

            void opreation(int i) {

                waitHandle.WaitOne();

                while (true) {

                    if (stop || (SetBlockingCounter >= 6 && EditBlockingCounter >= 6) || (sent >= 130 && BlockingCounter >= 130) || (sent >= 230 && BlockingCounter >= 50) || BlockingCounter >= 130) {
					    return;
				    }

                    int innerLoops = 0;

                    if (Loops != 0) {
                        innerLoops = Loops;
                    } else {
                        innerLoops = random.Next(5);
                    }

                    if (innerLoops == 0) {
                        innerLoops = 1;
                    }

                    for (int j = 0; j < innerLoops; j++) {

                        if (stop || (SetBlockingCounter >= 6 && EditBlockingCounter >= 6) || (sent >= 130 && BlockingCounter >= 130) || (sent >= 230 && BlockingCounter >= 50) || BlockingCounter >= 130) {
                            return;
				        }

                        void Send() {
                            
                            HttpRequestMessage requestMessage = new HttpRequestMessage();
                            requestMessage.Method = HttpMethod.Post;
                            requestMessage.Headers.Add("User-Agent", "Instagram 28.6.5 Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)");
                            requestMessage.Headers.Add("Accept", "*/*");
                            requestMessage.Headers.Add("Accept-Language", "en-US");
                            requestMessage.Headers.Add("Connection", "Keep-Alive");
                            if (i == 0) {
                                requestMessage.RequestUri = new Uri($"https://i.instagram.com/api/v1/accounts/set_username/");
                                requestMessage.Content = new FormUrlEncodedContent(Set_data);
                            } else {
                                requestMessage.RequestUri = new Uri($"https://i.instagram.com/api/v1/accounts/edit_profile/");
                                requestMessage.Content = new FormUrlEncodedContent(data);
                            }

                            httpClients[random.Next(httpClients.Count)].SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead).ContinueWith(responseTask => {
                                HttpContent content = responseTask.Result.Content;
                                content.ReadAsStringAsync().ContinueWith(responseBody => {
                                    string ResponseBody = responseBody.Result;
                                    HttpStatusCode statusCode = responseTask.Result.StatusCode;
                                    Uri RequestUri = responseTask.Result.RequestMessage.RequestUri;

                                    if (statusCode == HttpStatusCode.BadRequest) {
                                        Interlocked.Increment(ref BlockingCounter);
                                    }

                                    if (ResponseBody.Contains("user", StringComparison.CurrentCultureIgnoreCase) && statusCode == HttpStatusCode.OK) {

                                        lock (thisLock) {
                                            if (!stopS) {
                                                claim = true;
                                                waitClaimed.Set();
                                            }
                                        }

                                        Interlocked.Increment(ref Success);
                                        Interlocked.Increment(ref BlockingCounter);

                                        stop = true;
                                        stopS = true;
                                        return;
                                    }

                                    if (ResponseBody.Contains("wait", StringComparison.CurrentCultureIgnoreCase) || ResponseBody.Contains("please", StringComparison.CurrentCultureIgnoreCase) || statusCode == HttpStatusCode.TooManyRequests) {

                                        if (BlockingCounter >= 120 || (SetBlockingCounter >= 6 && EditBlockingCounter >= 6)) {

                                            lock (thisLock) {
                                                if (!stopB) {
                                                    waitBlocked.Set();
                                                }
                                            }

                                            stop = true;
                                            stopB = true;
                                            return;
                                        }

                                        if (RequestUri.OriginalString.Contains("set_username"))
                                            Interlocked.Increment(ref SetBlockingCounter);

                                        if (RequestUri.OriginalString.Contains("edit_profile"))
                                            Interlocked.Increment(ref EditBlockingCounter);
                                        
                                    }  

                                });  //.Wait();
                            });
 
                        }

                        Send();
                        Interlocked.Increment(ref sent);

                    }
                    
                    int Sleep = random.Next(150);
                    if (Sleep == 0) {
                        Sleep = 1;
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(Sleep));

                }

            }

            for (int i = 0; i < 2; i++) {

                void runner() {
                    Task.Run(() => {
                        opreation(i);                    
                    });
                }

                runner();

            }
            
        }   
        public static void connect() {

            Uri uri = new Uri("https://i.instagram.com/");

            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(uri, new Cookie("sessionid", SessionID));

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePoint servicePoint = ServicePointManager.FindServicePoint(uri);
            servicePoint.Expect100Continue = false;
            servicePoint.UseNagleAlgorithm = false;

            SocketsHttpHandler socketsHandler = new SocketsHttpHandler();   
            socketsHandler.PooledConnectionLifetime = TimeSpan.FromMinutes(30);
            socketsHandler.PooledConnectionIdleTimeout = TimeSpan.FromMinutes(30);
            socketsHandler.MaxConnectionsPerServer = 4096;
            socketsHandler.AllowAutoRedirect = false;
            socketsHandler.UseProxy = false;
            socketsHandler.Proxy = null;
            socketsHandler.UseCookies = true;
            socketsHandler.MaxConnectionsPerServer = 4096;
            socketsHandler.CookieContainer = cookieContainer;

            for (int i = 0; i < Clients; i++) {
                HttpClient httpClient = new HttpClient(socketsHandler);
                httpClient.DefaultRequestVersion = HttpVersion.Version11;
                httpClient.DefaultRequestHeaders.Host = "i.instagram.com";
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

                httpClient.GetAsync(uri).ContinueWith(responseTask => {
                    if (responseTask.Exception == null) {
                        httpClients.Add(httpClient);
                        Interlocked.Increment(ref connectedClients);
                    } else {
                        while (true) {
                            var respTask = httpClient.GetAsync(uri);
                            if (respTask.Exception == null) {
                                httpClients.Add(httpClient);
                                Interlocked.Increment(ref connectedClients);
                                break;
                            }
                            Thread.Sleep(TimeSpan.FromMilliseconds(10));
                        }
                    }
			    });  
            }
        }
        public static void Claimed() {
            waitClaimed.WaitOne();
            API.webHook(Target, BlockingCounter);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSuccessfully Claimed @{Target}\nAttempts: {BlockingCounter}");
            stopC = true;
        }
        public static void Blocked() {
            waitBlocked.WaitOne();
            Thread.Sleep(1500);
            Console.ForegroundColor = ConsoleColor.Red;
            if (Success == 0 && !claim)
                Console.WriteLine($"\nYou got blocked for spamming too many requestsR\nReached: {BlockingCounter}");
            
            stopC = true;
        }
        public static void SuperVisior() {
            while (true) {
                if (stopC) {
                    break;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Claiming [" + Target + $"] - Counter: {BlockingCounter}\r");
                Thread.Sleep(10);
            }
            EndWait.Set();
        }
    }
}
