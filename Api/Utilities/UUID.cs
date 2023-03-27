using System;

namespace Api.Utilities
{
    public class UUID
    {
        public static string Generate() { return Guid.NewGuid().ToString("D"); }
    }
}
