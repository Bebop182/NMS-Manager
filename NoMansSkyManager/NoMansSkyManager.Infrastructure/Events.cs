using System;
using Prism.Events;

namespace NoMansSkyManager.Infrastructure
{
    public class ModuleLoadedEvent : PubSubEvent<ModuleLoadedEvent> {
        public Type ModuleType { get; set; }
        public Type EntryPointView { get; set; }
    }

    public class RequestedLaunchEvent : PubSubEvent
    {
    }
}
