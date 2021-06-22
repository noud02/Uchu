using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates that a string uses wide
    /// characters (2 bytes per character).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Wide : Attribute
    {
        
    }
}