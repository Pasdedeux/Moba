using System.Collections.Generic;

namespace common.net.netEvent
{
    public class NetEventProcessor
    {
        protected Queue<NetEvent> events = new Queue<NetEvent>();
        protected Queue<NetEvent> execEvents = null;
        protected byte[] eventsLock = new byte[0];
        public void Trigger(NetEvent netEvent)
        {
            lock (eventsLock)
            {
                events.Enqueue(netEvent);
            }
        }
        protected void Fire()
        {
            lock (eventsLock)
            {
                if (events.Count == 0)
                    return;
                execEvents = events;
                events = new Queue<NetEvent>();
            }
            while (execEvents.Count != 0)
            {
                NetEvent netEvent = execEvents.Dequeue();
                if (netEvent != null)
                    netEvent.Fire();
            }
        }
    }
}
