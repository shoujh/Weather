using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using Weather.Models;

namespace Weather.Controllers
{
    public class WeatherController : Controller
    {
        private string jsonweatherdata;
        private Root weatherinfo;

        public IActionResult Index()
        {
            return View();
        }

        private string GetDoubleToStringComa(double tostring)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            return tostring.ToString("G", CultureInfo.InvariantCulture);
        }

        private void GetWeatherInfo(double lon, double lat)
        {
            string url = "https://api.open-meteo.com/v1/forecast?latitude="
            + GetDoubleToStringComa(lat) + "&longitude="
            + GetDoubleToStringComa(lon) + "&hourly=temperature_2m&timezone=Europe%2FMoscow";
            using (WebClient client = new())
            {
                string json = client.DownloadString(url);
                this.jsonweatherdata = json;
            }

        }
        private void ParseWeatherInfo()
        {
            this.weatherinfo = JsonConvert.DeserializeObject<Root>(this.jsonweatherdata);
        }

        private string GetResultViewModelToJson()
        {
            ResultViewModel rslt = new ResultViewModel(weatherinfo.latitude, weatherinfo.longitude);
            rslt.CalculateAveragesForGivenDays(this.weatherinfo);

            var jsonstring = JsonConvert.SerializeObject(rslt);
            return jsonstring;
        }

        private ResultViewModel GetResultViewModel()
        {
            ResultViewModel rslt = new ResultViewModel(weatherinfo.latitude, weatherinfo.longitude);
            rslt.CalculateAveragesForGivenDays(this.weatherinfo);
            return rslt;
        }


        [HttpPost]
        public string WeatherDetail(double lon, double lat)
        {

            this.GetWeatherInfo(lon, lat);
            string json = this.jsonweatherdata;
            this.ParseWeatherInfo();
            return this.GetResultViewModelToJson();

        }

        [HttpGet]
        public ActionResult DownloadFile(double lon, double lat)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("eu-ES");
            this.GetWeatherInfo(lon, lat);
            this.ParseWeatherInfo();
            ResultViewModel rslt = this.GetResultViewModel();

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine("Weather Forecast");
            tw.WriteLine("Latitude: " + GetDoubleToStringComa(rslt.Lat));
            tw.WriteLine("Longitutde: " + GetDoubleToStringComa(rslt.Lon));
            tw.WriteLine("Average temperatures for following days:");
            int i = 0;
            foreach (string day in rslt.Date)
            {
                tw.WriteLine(day + ": " + GetDoubleToStringComa(rslt.DailyAverage[i]));
                i++;
            }

            tw.Flush();
            var length = memoryStream.Length;
            tw.Close();
            var toWrite = new byte[length];
            Array.Copy(memoryStream.GetBuffer(), 0, toWrite, 0, length);

            return File(toWrite, "text/plain", "result.txt");
        }

    }

}
