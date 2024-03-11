namespace autox_data.Models
{
    public class Competitor
    {
        public string Name { get; set; }

        public string Year { get; set; }

        public int EventsCompleted {  get; set; }

        public string TotalPoints { get; set; }

        public string AvgPoints {  get; set; }

        public List<decimal> EventPoints { get; set; }

        public List<string> EventNames { get; set; }
    }
}
