namespace Weather.Models
{

    public class ResultViewModel
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public HashSet<String> Date { get; set; }
        public List<double> DailyAverage { get; set; }

        public ResultViewModel(double lat, double lon)
        {
            this.Lat = lat;
            this.Lon = lon;
            this.Date = new HashSet<String>();
            this.DailyAverage = new List<double>();
        }

        public void CalculateAveragesForGivenDays(Root root)
        {
            DateTime calcDay = DateTime.Parse(root.hourly.time[0]).Date;
            double sum = 0;
            for (int i = 0; i < root.hourly.time.Count(); i++)
            {
                this.Date.Add(DateTime.Parse(root.hourly.time[i]).Date.ToString("d"));
                if (calcDay == DateTime.Parse(root.hourly.time[i]).Date && i != root.hourly.time.Count() - 1)
                {
                    sum += root.hourly.temperature_2m[i];
                }
                else
                {
                    this.DailyAverage.Add(Math.Round(sum / 24, 1));
                    sum = 0;
                    calcDay = DateTime.Parse(root.hourly.time[i]).Date;
                }
            }
        }
    }
    public class Hourly
    {
        public List<double> temperature_2m { get; set; }
        public List<string> time { get; set; }
    }

    public class HourlyUnits
    {
        public string temperature_2m { get; set; }
        public string time { get; set; }
    }

    public class Root
    {
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public HourlyUnits hourly_units { get; set; }
        public double elevation { get; set; }
        public double latitude { get; set; }
        public Hourly hourly { get; set; }
        public double longitude { get; set; }
    }

}
