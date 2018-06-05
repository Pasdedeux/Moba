using System.Collections.Generic;
using System.Threading;

namespace common.net.netEvent
{
    public class BlockingNetEventProcessor:NetEventProcessor
    {
        public void SignalFire()
        {
            lock (eventsLock)
            {
                Monitor.PulseAll(eventsLock);
            }
        }
        public void FireWait()
        {
            lock (eventsLock)
            {
                if (events.Count == 0)
                {
                    lock (eventsLock)
                        Monitor.Wait(eventsLock);
                }
            }
        }
        public new void Trigger(NetEvent netEvent)
        {
            lock (eventsLock)
            {
                events.Enqueue(netEvent);
                Monitor.PulseAll(eventsLock);
            }
        }
        public new void Fire()
        {
            lock (eventsLock)
            {
                if (events.Count == 0)
                    Monitor.Wait(eventsLock);
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
