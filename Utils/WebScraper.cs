using HtmlAgilityPack;
using autox_data.Models;

namespace autox_data.Utils
{
    public class WebScraper
    {
        private readonly string baseUrl = "https://www.mnautox.com/";

        private HtmlWeb web;
        private HtmlDocument doc;
        public List<Season> seasons;

        public WebScraper() 
        {
            web = new HtmlWeb();
            doc = web.Load(baseUrl + "results-archive"); //load archive page initially so year doesn't have to be updated as time goes on
            List<string> years = LoadSideList();

            seasons = new List<Season>();

            foreach (string year in years)
            {
                if (year != "2017" && year != "2016") //no 2016 or 2017 support for now since HTML structure is different
                    seasons.Add(PopulateSeason(year));
            }
        }

        private List<string> LoadSideList()
        {
            //since we initially navigate to the archives page we can only select the unselected nodes to keep the archive info seperate
            var unSelectedYearNodes = doc.DocumentNode.SelectNodes("//li[@class='page-collection']");

            List<string> years = new List<string>(); 
            foreach (var item in unSelectedYearNodes) 
            { 
                years.Add(item.InnerText ); 
            }
            return years;
        }

        public Driver GetDriver(string name, bool onlyComplete)
        {
            Driver driver = new Driver();

            List<string> years = new List<string>();
            List<int> dotyFinish = new List<int>();
            List<decimal> dotyPoints = new List<decimal>();
            List<decimal> eventPoints = new List<decimal>();

            int dotyPlacement;

            driver.Name = name;
            foreach(Season season in seasons)
            {
                dotyPlacement = 0;
                foreach(Competitor competitor in season.Competitors) 
                {
                    dotyPlacement++;
                    if(competitor.Name == name)
                    {
                        if (onlyComplete)
                        {
                            if (competitor.EventsCompleted >= season.ScoredEvents)
                            {
                                years.Add(season.Year);
                                dotyFinish.Add(dotyPlacement);
                                dotyPoints.Add(decimal.Parse(competitor.AvgPoints));

                                foreach (decimal dEvent in competitor.EventPoints)
                                {
                                    eventPoints.Add(dEvent);

                                }
                            }
                        }
                        else
                        {
                            years.Add(season.Year);
                            dotyFinish.Add(dotyPlacement);
                            dotyPoints.Add(decimal.Parse(competitor.AvgPoints));

                            foreach (decimal dEvent in competitor.EventPoints)
                            {
                                eventPoints.Add(dEvent);

                            }
                        }
                    }
                }
            }

            driver.ActiveYears = years;
            driver.TotalEventsCompleted = eventPoints.Count;
            driver.DOTYAvgList = dotyPoints;

            driver.BestDOTYFinish = dotyFinish.Min();
            driver.AvgDOTYFinish = Math.Truncate((decimal)dotyFinish.Average()*1000)/1000;

            driver.BestSeasonAvgPoints = Math.Truncate(dotyPoints.Max() * 1000) / 1000;
            driver.AvgDOTYPoints = Math.Truncate(dotyPoints.Average() * 1000) / 1000;

            driver.BestEventPoints = Math.Truncate(eventPoints.Max() * 1000) / 1000;
            driver.AvgEventPoints = Math.Truncate(eventPoints.Average() * 1000) / 1000;
            return driver;
        }

        public Competitor GetCompetitor(string name, string year)
        {
            Season season = GetSeason(year);
            foreach(Competitor c in season.Competitors)
            {
                if(c.Name == name)
                    return c;
            }
            return null;
        }

        public Season GetSeason(string year)
        {
            Season season;
            foreach(Season s in seasons)
            {
                if(s.Year == year)
                {
                    season = CalculateDOTYCount(s);
                    season = CalculateAverages(s);
                    return season;
                }
            }
            return null;
        }

        private Season CalculateDOTYCount(Season s)
        {
            Season season = s;
            int DOTYcount = 0;

            foreach(Competitor c in season.Competitors)
            {
                if(c.EventsCompleted >= season.ScoredEvents)
                    DOTYcount++;
            }
            season.TotalDOTY = DOTYcount;
            return season;
        }

        private Season CalculateAverages(Season s)
        {
            Season season = s;
            List<decimal> topTen = new List<decimal>();
            List<decimal> topFive = new List<decimal>();
            for(int i = 0; i < 10; i++)
            {
                if(i<5)
                    topFive.Add(decimal.Parse(season.Competitors[i].AvgPoints));
                topTen.Add(decimal.Parse(season.Competitors[i].AvgPoints));
            }
            season.TopFiveAvg = Math.Truncate(topFive.Average()*1000)/1000;
            season.TopTenAvg = Math.Truncate(topTen.Average() * 1000) / 1000;
            return season;
        }

        private Season PopulateSeason(string year)
        {
            Season season = new Season();
            season.Competitors = new List<Competitor>();
            season.Year = year;
            doc = web.Load(GetDOTYDocUrl(year));

            var eventCountNodes = doc.DocumentNode.SelectNodes("//td[@class='num-events']");
            season.ScoredEvents = int.Parse(eventCountNodes[1].InnerText);
            int temp = 0;
            foreach( var e in eventCountNodes)
            {
                if(int.Parse(e.InnerText) > temp)
                    temp = int.Parse(e.InnerText);
            }
            int totalEvents = temp;
            season.TotalEvents = totalEvents;

            var totalDriverNodes = doc.DocumentNode.SelectNodes("//td[@class='driver']");
            var pointsNodes = doc.DocumentNode.SelectNodes("//td[contains(@class, 'score') or contains(@class, 'score best')]");

            int pointsNodeIndex = 0;
            int eventsNodeIndex = 0;

            for (int i = 0; i < totalDriverNodes.Count; i++)
            {
                Competitor c = new Competitor();
                c.Name = totalDriverNodes[i].InnerText;
                c.EventsCompleted = int.Parse(eventCountNodes[eventsNodeIndex].InnerText);
                c.TotalPoints = pointsNodes[pointsNodeIndex++].InnerText;
                c.AvgPoints = pointsNodes[pointsNodeIndex++].InnerText;
                eventsNodeIndex += 2;

                c.EventPoints = new List<decimal>();
                c.EventNames = new List<string>();
                int eventCount = 1;
                for (int j = pointsNodeIndex; j < totalEvents+pointsNodeIndex; j++)
                {
                    if (!pointsNodes[j].InnerText.Contains('-'))
                    {
                        c.EventPoints.Add(decimal.Parse(pointsNodes[j].InnerText));
                        c.EventNames.Add("M" + eventCount);
                    }
                    eventCount++;
                }
                pointsNodeIndex += totalEvents + 1;

                c.Year = season.Year;

                season.Competitors.Add(c);
            }
            season.TotalCompetitors = season.Competitors.Count();
            return season;
        }
       
        private string GetDOTYDocUrl(string year)
        {
            string url = baseUrl + year + "-results";
            doc = web.Load(url);

            var dotyNode = doc.DocumentNode.SelectSingleNode("//a[. ='MAC Driver of the Year']");
            HtmlAttributeCollection htmlAtr = dotyNode.Attributes;

            return htmlAtr[0].Value;
        }

        //TODO: eventually move competitior population code into here
        private Competitor CreateCompetitor(int driverCount)
        {
            Competitor c = new Competitor();
            return c;
        }
       
    }
}
