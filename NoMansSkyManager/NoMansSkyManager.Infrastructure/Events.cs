using System;
using Prism.Events;

namespace NMSM.Infrastructure
{
    public class ModuleLoadedEvent : PubSubEvent<ModuleLoadedEvent> {
        public Type ModuleType { get; set; }
        public Type EntryPointView { get; set; }
    }

    public class RequestedLaunchEvent : PubSubEvent
    {
    }
}
