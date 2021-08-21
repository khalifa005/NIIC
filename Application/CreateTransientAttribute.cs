using System;


namespace Application
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateTransientAttribute : Attribute
    {
        public bool IsImplementingInterface { get; set; } = false;
    }
}
