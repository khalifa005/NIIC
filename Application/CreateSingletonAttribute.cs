using System;

namespace Application
{
    /// <summary>
    /// Make a class to be automatically registered as singleton by DI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateSingletonAttribute : Attribute
    {
        public bool IsImplementingInterface { get; set; } = false;
    }
}
