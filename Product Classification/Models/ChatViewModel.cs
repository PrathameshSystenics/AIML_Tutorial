using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductClassification.Models
{
    public class ChatViewModel
    {
        public List<SelectListItem> Models { get; set; }
        public List<string> SuggestedExamples
        {
            get
            {
                return new List<string>
                {
                    "give the similar products for 'kingston'",
                    "get the list of the categories you supported",
                    "what task you can perform",
                    "classify the category for the description 'Heavy, bright nickel-plated steel cooking grate fits 22-1/2-inch kettles.'"
                };
            }
        }

    }
}



