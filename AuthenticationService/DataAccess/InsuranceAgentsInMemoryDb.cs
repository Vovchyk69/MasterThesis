using System.Collections.Concurrent;
using AuthenticationService.Domain;

namespace AuthenticationService.DataAccess;

public class InsuranceAgentsInMemoryDb : IAgent
{
    private readonly IDictionary<string, Agent> db = new ConcurrentDictionary<string, Agent>();

    public InsuranceAgentsInMemoryDb()
    {
        Add(new Agent("jimmy.solid", "secret", "static/avatars/jimmy_solid.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
        Add(new Agent("danny.solid", "secret", "static/avatars/danny.solid.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
        Add(new Agent("admin", "admin", "static/avatars/admin.png", new List<string>() {"TRI", "HSI", "FAI", "CAR"}));
    }

    public void Add(Agent agent) => db[agent.Login] = agent;

    public Agent FindByLogin(string login) => db[login];
}