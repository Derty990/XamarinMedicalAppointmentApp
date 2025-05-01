// W Web API - Controllers/PingController.cs
using Microsoft.AspNetCore.Mvc;
using System; // Potrzebne dla Console.WriteLine

[Route("api/[controller]")] // Trasa: /api/ping
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet] // Odpowiada na metodę GET
    public ActionResult<string> Get()
    {
        // Zaloguj informację w konsoli API, że endpoint został trafiony
        Console.WriteLine($"---> API: Endpoint /api/ping został trafiony o {DateTime.Now} <---");
        // Zwróć prostą odpowiedź tekstową
        return Ok("Pong from API!");
    }
}