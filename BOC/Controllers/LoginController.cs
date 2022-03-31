using BOC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BOC.Controllers
{
    public class LoginController : Controller
    {

        [HttpGet]
        public IActionResult Index(string msg)
        {
            var model = new LoginModel();
            model.Username = HttpContext.Request.Cookies["Username"];
            model.Password = HttpContext.Request.Cookies["Password"];

            if (!String.IsNullOrEmpty(msg))
            {
                model.Result = msg;
            }
            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(LoginModel model)
        {
            var x = model.TypeOfDevice;
         
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                model.ErrorMessage = "Data entered cannot be left blank.";
                return View(model);
            }
            else
            {
                //Get path url api
                Url login = new Url();
                string uri = login.Get("Login");
                HttpClient Client = new HttpClient();
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("UserCode", model.Username));
                nvc.Add(new KeyValuePair<string, string>("Password", model.Password));
                nvc.Add(new KeyValuePair<string, string>("DeviceID", "C11FCC37-16D6-11EB-BADE-000C29D93A49"));
                var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(nvc) };

                string Content;
                HttpResponseMessage res;
                res = Client.SendAsync(req).Result;
                Content = res.Content.ReadAsStringAsync().Result;
                var oData = JObject.Parse(Content);

                model.Result = oData["ResultCode"].ToString();
                if (model.Result == "0")
                {

                    // Save  Session TypeOfDevice
                    var typeofdevice = model.TypeOfDevice;
                    HttpContext.Session.SetString("TypeOfDevice", typeofdevice);
                    // Save  Session Token
                    var token = oData["Data"]["Token"].ToString();
                    HttpContext.Session.SetString("Token", token);
                    // Save  User Name to Session
                    var username = model.Username;
                    string[] collection = username.Split('@');
                    string name = collection[0].ToString();
                    HttpContext.Session.SetString("Username", name);
                    if (model.Remember == true)
                    {
                        // create claims
                        List<Claim> claims = new List<Claim>
                            {
                                //new Claim(ClaimTypes.Name, model.Username),
                                new Claim("Username", model.Username),
                                new Claim("Password", model.Password)

                            };

                        // create identity
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                        // create principal
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        // sign-in
                        await HttpContext.SignInAsync(
                                scheme: "CookieAuthentication",
                                principal: principal,
                                properties: new AuthenticationProperties
                                {
                                    IsPersistent = true, // for 'remember me' feature
                                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                                });

                        model.Remember = false;

                    }
                    if (model.TypeOfDevice == "DeskTop")
                    {
                        return RedirectToAction("Desktop", "Home");
                    }
                    if (model.TypeOfDevice == "Mobile")
                    {
                        return RedirectToAction("Mobile", "Home");
                    }

                }
                else
                {
                    model.ErrorMessage = "Login fail.Please check your data input!";
                }


            }


            return View(model);

        }


        public IActionResult Error()
        {

            return View();

        }


    }
}