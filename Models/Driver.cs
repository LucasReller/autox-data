namespace autox_data.Models
{
    public class Driver
    {
        public string Name { get; set; }

        public List<string> ActiveYears { get; set; }

        public List<decimal> DOTYAvgList { get; set; }

        public int TotalEventsCompleted {  get; set; }

        public int BestDOTYFinish { get; set; }

        public decimal AvgDOTYFinish { get; set; }

        public decimal AvgDOTYPoints { get; set; }

        public decimal BestSeasonAvgPoints { get; set; }

        public decimal BestEventPoints { get; set; }

        public decimal AvgEventPoints {  get; set; }
    }
}
