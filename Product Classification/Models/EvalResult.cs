namespace Classification.Models
{
    public class EvalResult
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public string Expected { get; set; }
        public string Result { get; set; }
        public int Correct { get; set; }
        public int Total { get; set; }
        public double Accuracy { get; set; }

    }
}
