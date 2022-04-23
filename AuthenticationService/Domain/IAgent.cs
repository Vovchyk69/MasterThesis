namespace AuthenticationService.Domain;

public interface IAgent
{
    void Add(Agent agent);

    Agent FindByLogin(string login);
}