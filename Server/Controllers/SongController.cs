using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public SongsController(DataDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            if (song == null)
            {
                return BadRequest("Song cannot be null.");
            }

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSongById(Guid id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            return song;
        }

        // Search endpoint
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> SearchSongs([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            query = query.ToLower();

            // Querying results to search title, artist, or album
            var results = await _context.Songs
                .Where(song => 
                    song.Title.ToLower().Contains(query) || 
                    song.Artist.ToLower().Contains(query) || 
                    song.Album.ToLower().Contains(query))
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound("No songs match the search criteria.");
            }

            return Ok(results);
        }
    }
}
