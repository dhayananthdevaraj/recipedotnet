namespace dotnetapp.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; } // Assuming you have a custom ObjectId type
        public int UserId { get; set; } // Assuming you have a custom ObjectId type
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Category { get; set; }
        public string Photo { get; set; } // Assuming this is a string for URL or file path
        public decimal Price { get; set; } // Price per day in USD
        public int ServingSize { get; set; }
    }
}