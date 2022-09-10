using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Charts.ReadModels;

namespace Charts.Controllers
{
    public class WeatherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Weather()
        {
            var url = "https://meteostat.p.rapidapi.com/stations/monthly?station=37788&start=2021-01-01&end=2021-12-31";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.Headers["X-RapidAPI-Key"] = "1a37801c1cmshcc8fa95fb5b1728p1db6f1jsn0311afff1416";
            request.Headers["X-RapidAPI-Host"] = "meteostat.p.rapidapi.com";

            using var webResponse = request.GetResponse();

            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            JObject json = JObject.Parse(data);
            JToken dataToken = json["data"];

            IEnumerable<JToken> year = dataToken.Children();

            List<Data> dataForChart = new List<Data>();
            foreach (var month in year)
            {
                dataForChart.Add(new Data((double)month["tavg"]));
            }

            var rm = new WeatherDataRm
                (
                chart: new Chart
                    (
                        caption: "Weather in Yerevan",
                        subCaption: "Based on data collected last year",
                        numberPrefix: "",
                        theme: "fusion",
                        radarfillcolor: "#ffffff"
                    ),

                categories: new List<Category>
                {
                    new Category
                    (
                        new List<Label>
                        {
                            new Label("Jan"),
                            new Label("Feb"),
                            new Label("Mar"),
                            new Label("Apr"),
                            new Label("May"),
                            new Label("Jun"),
                            new Label("Jul"),
                            new Label("Aug"),
                            new Label("Sep"),
                            new Label("Oct"),
                            new Label("Nov"),
                            new Label("Dec")
                        }
                    ),
                },

                dataset: new List<DataSet>
                {
                    new DataSet
                    (
                        seriesname: "Temperature Average",
                        dataForChart
                    )
                }
            );

            return Ok(rm);
        }
    }
}
