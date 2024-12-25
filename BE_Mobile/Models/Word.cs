using BE_Mobile.Models.VocabularyApi.Models;

namespace BE_Mobile.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string? Term { get; set; }
        public string? Type { get; set; } // Ví dụ: noun, verb, adjective...
        public List<string>? Definitions { get; set; }
        public List<Example>? Examples { get; set; }
    }
}
