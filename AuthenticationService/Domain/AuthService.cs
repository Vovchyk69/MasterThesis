using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OcelotGateway;

namespace AuthenticationService.Domain;

public class AuthService
{
    private readonly IAgent _agents;
    private readonly IApplicationSettings _appSettings;
        
    public AuthService(IAgent agents, IApplicationSettings app)
    {
        _agents = agents;
        _appSettings = app;
    }

    public string Authenticate(string login, string pwd)
    {
        var agent = _agents.FindByLogin(login);

        if (agent == null)
            return null;

        if (!agent.PasswordMatches(pwd))
            return null;
            
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.AppSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim("sub", agent.Login), 
                new Claim(ClaimTypes.Name, agent.Login),
                new Claim(ClaimTypes.Role, "SALESMAN"),
                new Claim(ClaimTypes.Role, "USER"),
                new Claim("avatar", agent.Avatar),
                new Claim("userType", "SALESMAN"), 
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Agent AgentFromLogin(string login) => _agents.FindByLogin(login);
}