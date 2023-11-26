using System.Net;
using Microsoft.Extensions.Caching.Memory;

namespace kontaktica;

public static class Auth
{
    public static bool DynDnsCheck(this HttpContext ctx)
    {
        if (C.IsDebug)
            return true;

        var key = ctx.Connection.RemoteIpAddress?.ToString();
        ctx.Response.Headers.Add("Your-IP", key);
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var cache = ctx.RequestServices.GetRequiredService<IMemoryCache>();
        var allowed = cache.GetOrCreate(key, cacheEntry =>
        {
            cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(3);
            foreach (var dnsName in C.Settings.DynamicDnsNames)
                try
                {
                    var ip = Dns.GetHostEntry(dnsName);
                    var listed = ip.AddressList.Any(i => i.ToString() == key);
                    if (listed)
                        return true;
                }
                catch (Exception) { }

            return false;
        });

        return allowed;
    }
}