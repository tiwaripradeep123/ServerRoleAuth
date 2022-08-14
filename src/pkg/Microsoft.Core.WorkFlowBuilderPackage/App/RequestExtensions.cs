using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreFeed.Core.Com.Entity.App
{
    public static class RequestExtensions
    {
        public static IDictionary<string, string> GetProperties(this Request request)
        {
            return request?.GetType()
                             .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             .ToDictionary(
                    prop => prop.Name,
                    prop => Newtonsoft.Json.JsonConvert.SerializeObject(prop.GetValue(request, null) ?? string.Empty)
                );
        }
    }
}