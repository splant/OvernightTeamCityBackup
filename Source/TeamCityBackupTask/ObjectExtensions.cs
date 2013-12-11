using System;

namespace TeamCityBackupTask
{
    public static class ObjectExtensions
    {
        public static Out IfNotNull<In, Out>(this In value, Func<In, Out> selector) where In : class where Out : class
        {
            return value == null ? null : selector(value);
        }
    }
}