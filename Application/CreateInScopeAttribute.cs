using System;


namespace Application
{
    /// <summary>
    /// Make a class to be automatically registered as in scope by DI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateInScopeAttribute : Attribute
    {
        public bool IsImplementingInterface { get; set; } = false;
    }
}
