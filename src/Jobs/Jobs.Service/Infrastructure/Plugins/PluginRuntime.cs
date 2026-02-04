namespace Jobs.Service.Infrastructure.Plugins;

/// <summary>
/// プラグインランタイム
/// プラグイン方式で後付け拡張可能
/// </summary>
public class PluginRuntime
{
    private readonly List<IPlugin> _plugins = new();

    public void RegisterPlugin(IPlugin plugin)
    {
        _plugins.Add(plugin);
    }

    public async Task ExecutePluginsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var plugin in _plugins)
        {
            await plugin.ExecuteAsync(cancellationToken);
        }
    }
}

/// <summary>
/// プラグインインターフェース
/// </summary>
public interface IPlugin
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
