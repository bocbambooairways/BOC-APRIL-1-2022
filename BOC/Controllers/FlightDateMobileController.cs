using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOC.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BOC.Controllers
{
    public class FlightDateMobileController : Controller
    {
        
        [HttpGet]
        public IActionResult Index(string TimeZone, string Date, string Key, string SelectedRouting, string VType, string AutoHide)
        {
            if (TimeZone == null) { TimeZone = "HAN"; }
        
            var dd = Date.ToString();
            string[] collection = dd.Split('-');
            // to get the full month name
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Int16.Parse(collection[1]));
            string day = collection[2];
            TempData["Date"] = day + monthName;
            ////Get Session AirportChoose
            var AirportChoose = HttpContext.Session.GetString("AirportChoose");

            if (AirportChoose == null)
            {
                AirportChoose = "";
            }
            if (Key == null)
            {
                Key = "";
            }
            var KeySearch = Key.ToString();
            if (VType == null)
            {
                VType = "";
            }
            var ViewType = VType.ToString();


            //Get Session Token
            var token = HttpContext.Session.GetString("Token");
            //Access API with Header
            Url flightget = new Url();
            string uri = flightget.Get("FlightGet");

            HttpClient Client = new HttpClient();

            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("FlightID", "0"));
            nvc.Add(new KeyValuePair<string, string>("AreaCode", AirportChoose));
            nvc.Add(new KeyValuePair<string, string>("TimeZone", TimeZone.ToString()));
            nvc.Add(new KeyValuePair<string, string>("Date", Date.ToString()));
            nvc.Add(new KeyValuePair<string, string>("KeySearch", KeySearch));
            nvc.Add(new KeyValuePair<string, string>("Int_Dom", SelectedRouting.ToString()));
            nvc.Add(new KeyValuePair<string, string>("ViewType", ViewType));
            nvc.Add(new KeyValuePair<string, string>("AutoHide", AutoHide.ToString()));



            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(nvc) };

            string Content;
            HttpResponseMessage res;
            res = Client.SendAsync(req).Result;
            Content = res.Content.ReadAsStringAsync().Result;

            JToken _FltItem;
            JObject ser = JObject.Parse(Content);
            Int32 _Result = (int)ser.SelectToken("ResultCode");

            List<Flight> lst = new List<Flight>();
            var ser2 = ser.SelectToken("Data");
            List<JToken> data = new List<JToken>(ser2.Children());

            foreach (var item in data)
            {
                Flight Flt = new Flight();

                item.CreateReader();
                Int32 _FlightIDs = Int32.Parse(item.SelectToken("FlightID").ToString());
                Flt.FlightID = _FlightIDs;


                _FltItem = item.SelectToken("Date");
                Flt.Date = (string)_FltItem.SelectToken("Value");
                Flt.Date_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("FltNo");
                Flt.FltNo = (string)_FltItem.SelectToken("Value");
                Flt.FltNo_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("RegisterNo");
                Flt.RegisterNo = (string)_FltItem.SelectToken("Value");
                Flt.RegisterNo_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Aircraft");
                Flt.Aircraft = (string)_FltItem.SelectToken("Value");
                Flt.Aircraft_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Route");
                Flt.Route = (string)_FltItem.SelectToken("Value");
                Flt.Route_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("DateTime_ATA");
                Flt.DateTime_ATA = (string)_FltItem.SelectToken("Value");
                Flt.DateTime_ATA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("STD");
                Flt.STD = (string)_FltItem.SelectToken("Value");
                Flt.STD_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ETD");
                Flt.ETD = (string)_FltItem.SelectToken("Value");
                Flt.ETD_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("BDT");
                Flt.BDT = (string)_FltItem.SelectToken("Value");
                Flt.BDT_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("DoorClose");
                Flt.DoorClose = (string)_FltItem.SelectToken("Value");
                Flt.DoorClose_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("TOff");
                Flt.TOff = (string)_FltItem.SelectToken("Value");
                Flt.TOff_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("STA");
                Flt.STA = (string)_FltItem.SelectToken("Value");
                Flt.STA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ETA");
                Flt.ETA = (string)_FltItem.SelectToken("Value");
                Flt.ETA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("TDown");
                Flt.TDown = (string)_FltItem.SelectToken("Value");
                Flt.TDown_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ATD");
                Flt.ATD = (string)_FltItem.SelectToken("Value");
                Flt.ATD_Color = (string)_FltItem.SelectToken("BackColor");


                _FltItem = item.SelectToken("ATA");
                Flt.ATA = (string)_FltItem.SelectToken("Value");
                Flt.ATA_Color = (string)_FltItem.SelectToken("BackColor");


                _FltItem = item.SelectToken("Terminal");
                Flt.Terminal = (string)_FltItem.SelectToken("Value");
                Flt.Terminal_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Gate");
                Flt.Gate = (string)_FltItem.SelectToken("Value");
                Flt.Gate_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Belt_Dep");
                Flt.Belt_Dep = (string)_FltItem.SelectToken("Value");
                Flt.Belt_Dep_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Bay_Dep");
                Flt.Bay_Dep = (string)_FltItem.SelectToken("Value");
                Flt.Bay_Dep_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Bay_Arr");
                Flt.Bay_Arr = (string)_FltItem.SelectToken("Value");
                Flt.Bay_Arr_Color = (string)_FltItem.SelectToken("BackColor");

                lst.Add(Flt);
            }
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].ID = i + 1;
            }
            //var model = new FlightModel();
            //model.Ds = lst;
            return View(lst);
        }

      

    }
}
