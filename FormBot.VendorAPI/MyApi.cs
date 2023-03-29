using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FormBot.VendorAPI
{
    //public class User
    //{
    //    public string Username { get; set; }
    //    public string Password { get; set; }
    //}

    [Authorize]
    public class MyApiController : ApiController
    {
        //[HttpGet]
        //public IHttpActionResult Test()
        //{
        //    return this.Ok(new User() { Username = "Sandip" });
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public IHttpActionResult Login([FromBody]User request)
        //{
        //    User user = new FormBot.VendorAPI.User();
        //    //user = Request.Content.ReadAsAsync(request.GetType()).Result as User;
        //    //Token TokenResult = CreateToken(user.Username, user.Password);
        //    Token TokenResult = CreateToken(request.Username, request.Password);
        //    return this.Ok(new { TokenResult });
        //}


        //public Token CreateToken(string username, string password)
        //{
        //    string token;
        //    string siteUrl = String.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.ApplicationPath); ;

        //    using (WebClient client = new WebClient())
        //    {
        //        //client.Headers.Add("content-type", "application/x-www-form-urlencoded");
        //        token = client.UploadString(siteUrl + "/token", "post", "grant_type=password&username='" + username + "'");
        //    }

        //    dynamic tokendata = JObject.Parse(token);
        //    Token TokenResult = new Token();
        //    TokenResult.access_token = tokendata.access_token;
        //    TokenResult.token_type = tokendata.token_type;
        //    TokenResult.expires_in = tokendata.expires_in;

        //    return TokenResult;
        //}

    }

    //public class Token
    //{
    //    public string access_token { get; set; }
    //    public string token_type { get; set; }
    //    public int expires_in { get; set; }
    //    public string userName { get; set; }
    //    [JsonProperty(".issued")]
    //    public string issued { get; set; }
    //    [JsonProperty(".expires")]
    //    public string expires { get; set; }
    //}
}