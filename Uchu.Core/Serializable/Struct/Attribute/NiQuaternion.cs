using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates a Quaternion is a NiQuaternion (W,X,Y,Z) instead
    /// of a Quaternion (X,Y,Z,W).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NiQuaternion : Attribute
    {
        
    }
}