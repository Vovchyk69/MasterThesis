using System.Dynamic;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService;

public interface IApplicationSettings
{
    dynamic AppSettings { get; } 
    
    dynamic ConnectionStrings { get; }
}

public class ApplicationSettings : IApplicationSettings
{
    private class Section : DynamicObject
    {
        private IConfigurationSection _section;
        public Section(IConfigurationSection section)
        {
            _section = section;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = NormalizeName(binder.Name);
            result = _section[binder.Name] ?? _section[name];

            if (result is not null)
            {
                TryConvert((string) result, out result);
                return true;
            }

            var section = GetSection(binder.Name) ?? GetSection(name);
            if (section is null) return true;

            result = new Section(section);
            return true;
        }

        private static void TryConvert(string value, out object result)
        {
            switch (value)
            {
                case null or "":
                    result = value;
                    return;
            }

            var parsed = bool.TryParse(value, out var boolValue);
            if (parsed)
                result = boolValue;
            else
            {
                parsed = int.TryParse(value, out var intValue);
                result = (!parsed) ? value : intValue;
            }
        }

        private static string NormalizeName(string name) =>
            name.Replace('_', '-');

        private IConfigurationSection? GetSection(string name) =>
            _section.GetChildren().FirstOrDefault(n =>
                string.Compare(n.Key, name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    private sealed class AppSettingsObject : Section
    {
        public AppSettingsObject(IConfiguration config): base(config.GetSection(AppSettingName))
        {
        }
    }
    
    private sealed class ConnectionStringsObject : Section
    {
        public ConnectionStringsObject(IConfiguration config): base(config.GetSection(ConnectionStringName))
        {
        }
    }

    private const string AppSettingName = "AppSettings";
    private const string ConnectionStringName = "ConnectionStrings";

    private readonly IConfiguration _config = GetConfig();

    public dynamic AppSettings => new AppSettingsObject(_config);
    public dynamic ConnectionStrings => new ConnectionStringsObject(_config);

    private static IConfiguration GetConfig()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();

        return builder.Build();
    }
}