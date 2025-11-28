using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace PokeMetrix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MarketController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("top-movers")]
        public async Task<IActionResult> GetTopMovers()
        {
            string connectionString = _config.GetConnectionString("DatabricksSQL");
            
            // Mocking the connection logic for the portfolio code
            // using IDbConnection db = new OdbcConnection(connectionString);
            
            string sql = @"
                SELECT card_name, set_name, current_price, percent_change 
                FROM gold_daily_movers 
                WHERE ingestion_date = current_date() 
                ORDER BY percent_change DESC 
                LIMIT 10";

            // var result = await db.QueryAsync(sql);
            
            // returning dummy data so the code will compile and runs for the demo
            var dummyData = new[] {
                new { card_name = "Greninja EX 214", set_name = "Twilight Masquerade", percent_change = 2.5 },
                new { card_name = "Mega Absol EX 180", set_name = "Mega Evolution", percent_change = 5.2 }
            };

            return Ok(dummyData);
        }
    }
}