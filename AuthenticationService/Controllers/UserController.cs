using AuthenticationService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly Domain.AuthService _authService;

    public UserController(Domain.AuthService authService)
    {
        this._authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Post([FromBody] AuthenticationRequest user)
    {
        var token = _authService.Authenticate(user.Login, user.Password);

        if (token is null)
            return BadRequest(new {message = "Username of password incorrect"});

        return Ok(new {Token = token});
    }

    [HttpGet]
    public IActionResult Get() => Ok(_authService.AgentFromLogin(HttpContext.User.Identity?.Name));
}