using BookList.Helper;
using BookList.Models;
using BookList.Models.ViewModells;
using BookList.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace BookList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidateTokenService _validateTokenService;

        const string SessionKeyToken = "SessionKey";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IValidateTokenService validateTokenService) 
        {
            _logger = logger;
            _configuration = configuration;
            _validateTokenService = validateTokenService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Get<SessionVM>(SessionKeyToken) == default)
            {
                HttpContext.Session.Set<SessionVM>(SessionKeyToken, new SessionVM());
            }
            SessionVM sessionObject = HttpContext.Session.Get<SessionVM>(SessionKeyToken);
            
            if (sessionObject.loggedIn == false)
            {
                HttpContext.Session.SetString("logged", "nope");
            }
            else
            {
                HttpContext.Session.SetString("logged", "yep");
            }
            
            if (sessionObject.token == null)
            {
                return View();
            }
            else
            {
                ViewBag.test = sessionObject.token;
            }
            

            return View();
        }
        
        public IActionResult Citat()
        {
            if (HttpContext.Session.Get<SessionVM>(SessionKeyToken) == default)
            {
                HttpContext.Session.Set<SessionVM>(SessionKeyToken, new SessionVM());
            }
            SessionVM sessionObject = HttpContext.Session.Get<SessionVM>(SessionKeyToken);

            ViewBag.logged = _validateTokenService.ValidateToken(sessionObject);
           

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            var user = new UserDto
            {
                Username = username,
                Password = password
            };
            var client = new HttpClient();
            //client.BaseAddress = new Uri("https://localhost:7272/api/JWTAPI/"); //Fungerar lokalt
            client.BaseAddress = new Uri("https://booklistapplication.azurewebsites.net/api/JWTAPI/"); //Azure
            var json = System.Text.Json.JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("register", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var postResponse = System.Text.Json.JsonSerializer.Deserialize<PostResponse>(responseContent, options);
                Console.WriteLine("Post successful! UserName: " + postResponse.username);

            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = new UserDto
            {
                Username = username,
                Password = password
            };
            var client = new HttpClient();
            //client.BaseAddress = new Uri("https://localhost:7272/api/JWTAPI/");
            client.BaseAddress = new Uri("https://booklistapplication.azurewebsites.net/api/JWTAPI/"); //Azure
            var json = System.Text.Json.JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("login", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;

                if (HttpContext.Session.Get<SessionVM>(SessionKeyToken) == default)
                {
                    HttpContext.Session.Set<SessionVM>(SessionKeyToken, new SessionVM());
                }
                SessionVM sessionObject = HttpContext.Session.Get<SessionVM>(SessionKeyToken);
                sessionObject.token = responseContent;
                sessionObject.loggedIn = true;
                HttpContext.Session.Set<SessionVM>(SessionKeyToken, sessionObject);
                

                
                Console.WriteLine("Login succesfull!");

            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.Get<SessionVM>(SessionKeyToken) == default)
            {
                HttpContext.Session.Set<SessionVM>(SessionKeyToken, new SessionVM());
            }
            SessionVM sessionObject = HttpContext.Session.Get<SessionVM>(SessionKeyToken);
            sessionObject.token = "";
            sessionObject.loggedIn = false;
            HttpContext.Session.Set<SessionVM>(SessionKeyToken, sessionObject);

            return RedirectToAction("Index");
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AngularTest()
        {
            return View();
        }

    }
}
