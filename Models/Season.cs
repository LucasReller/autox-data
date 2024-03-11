namespace autox_data.Models
{
    public class Season
    {
        public string Year { get; set; }

        public List<Competitor> Competitors { get; set;}

        public int ScoredEvents { get; set; }

        public int TotalEvents { get; set; }

        public int TotalCompetitors { get; set; }

        public int TotalDOTY { get; set; }

        public decimal TopTenAvg { get; set; }

        public decimal TopFiveAvg { get; set; }
    }
}
