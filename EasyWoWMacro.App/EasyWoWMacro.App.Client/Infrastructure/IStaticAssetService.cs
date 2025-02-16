namespace EasyWoWMacro.App.Client.Infrastructure;

public interface IStaticAssetService
{
    public Task<string?> GetAsync(string assetUrl, bool useCache = true);
}
