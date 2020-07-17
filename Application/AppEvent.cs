using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public enum AppEventType
    {
        Created,
        Updated,
        Deleted,
        Restored,
        HardDeleted
    }

    public abstract class AppEvent
    {
        public AppEventType Type { get; set; }

        protected AppEvent()
        {
        }

        protected AppEvent(AppEventType type)
        {
            Type = type;
        }
    }
}
