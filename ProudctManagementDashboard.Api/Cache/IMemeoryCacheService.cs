namespace ProudctManagementDashboard.Api.Cache
{
    public interface IMemeoryCacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
        void Remove(string key);
    }
}
