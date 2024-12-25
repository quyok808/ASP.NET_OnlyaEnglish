using BE_Mobile.Models.VocabularyApi.Models;
using BE_Mobile.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BE_Mobile.Context;
using Microsoft.EntityFrameworkCore;

namespace BE_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VocabularyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VocabularyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{term}")]
        public async Task<ActionResult<Word>> GetWord(string term)
        {
            var word = await _context.Words
                .Include(w => w.Examples)
                .FirstOrDefaultAsync(w => w.Term.Equals(term));

            if (word == null)
            {
                return NotFound();
            }

            return Ok(word);
        }

        [HttpPost]
        public async Task<ActionResult<Word>> CreateWordAsync([FromBody] Word newWord)
        {
            if (newWord == null || string.IsNullOrEmpty(newWord.Term) || newWord.Definitions == null || newWord.Definitions.Count == 0)
            {
                return BadRequest("Invalid word data. Term, Definitions are required!");
            }

            var existingWord = _context.Words.FirstOrDefault(w => w.Term == newWord.Term);
            if (existingWord != null)
            {
                return Conflict("This word already existed!");
            }

            // Đảm bảo examples không null (dù nó có thể rỗng)
            if (newWord.Examples == null)
            {
                newWord.Examples = new List<Example>();
            }

            _context.Words.Add(newWord);
            // Sau khi lưu từ vựng, lưu cả các examples
            if (newWord.Examples != null)
            {
                foreach (var example in newWord.Examples)
                {
                    _context.Examples.Add(example);
                }
            }

            await _context.SaveChangesAsync(); // Lưu các example
            return CreatedAtAction(nameof(GetWord), new { term = newWord.Term }, newWord);
        }

        [HttpPut("{term}")]
        public async Task<ActionResult<Word>> UpdateWord(string term, [FromBody] Word updatedWord)
        {
            if (updatedWord == null || term != updatedWord.Term)
            {
                return BadRequest();
            }

            var existingWord = await _context.Words.FindAsync(term);

            if (existingWord == null)
            {
                return NotFound();
            }

            // Update các thuộc tính của từ
            existingWord.Type = updatedWord.Type;
            existingWord.Definitions = updatedWord.Definitions;
            existingWord.Examples = updatedWord.Examples;

            await _context.SaveChangesAsync();
            return Ok(existingWord);
        }


        [HttpDelete("{term}")]
        public async Task<IActionResult> DeleteWord(string term)
        {
            var wordToRemove = await _context.Words.FirstOrDefaultAsync(w => w.Term == term);
            if (wordToRemove == null)
            {
                return NotFound();
            }

            _context.Words.Remove(wordToRemove);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
