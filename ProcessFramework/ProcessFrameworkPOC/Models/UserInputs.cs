namespace ProcessFrameworkPOC.Models
{
    public enum Tone
    {
        Casual,
        Professional,
        Informative,
        Friendly
    }

    public enum DesiredLength
    {
        Big,
        Short,
        Medium
    }

    public class UserInputs
    {
        // Title property
        public string Title { get; set; } = string.Empty;

        // Summary/Description property
        public string Description { get; set; } = String.Empty;

        // Tone property using the Tone enum
        public Tone Tone { get; set; }

        // Keywords property as an array of strings
        public string[] Keywords { get; set; } = [];

        // Backing field for Creativity
        private float creativity;

        // Creativity property: ensures the value is within the range 0 to 2.
        public float Creativity
        {
            get => creativity;
            set
            {
                if (value < 0 || value > 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(Creativity), "Creativity must be between 0 and 2.");
                }
                creativity = value;
            }
        }

        // Desired Length property using the DesiredLength enum
        public DesiredLength DesiredLength { get; set; }

        // Articles property as a dictionary of strings
        public List<Articles> Articles { get; set; } = [];

        // The Output from the Previous Process output
        public string PreviousProcessOutput { get; set; } = string.Empty;

        // Query used to gather the Information
        public string Query { get; set; } = string.Empty;

        public string References { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string OutlineAndSections { get; set; } = string.Empty;

        public string ModificationsNeeded { get; set; } = string.Empty;
    }
}


