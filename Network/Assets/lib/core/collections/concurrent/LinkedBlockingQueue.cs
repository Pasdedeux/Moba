using System;
using System.Threading;
namespace common.lib.core.collections.concurrent
{
    public class LinkedBlockingQueue<E> where E:class
    {
        class Node<T>
        {
            internal T item;
            internal Node<T> next;
            internal Node(T x) { item = x; }
        }
        private int capacity;
        private Node<E> head;
        private Node<E> last;
        private byte[] takeLock = new byte[0];
        private byte[] putLock = new byte[0];
        private byte[] countLock = new byte[0];
        private int count = 0;
        public LinkedBlockingQueue(int capacity)
        {
            if (capacity <= 0)
                throw new Exception("capacity is Illegal,capacity:"+ capacity);
            this.capacity = capacity;
            last = head = new Node<E>(null);
        }
        public LinkedBlockingQueue() : this(int.MaxValue){}
        private void Enqueue(Node<E> node)
        {
            last = last.next = node;
        }
        private E Dequeue()
        {
            Node<E> h = head;
            Node<E> first = h.next;
            h.next = h;
            head = first;
            E x = first.item;
            first.item= null;
            return x;
        }
        public void Put(E e)
        {
            if (e == null)
                throw new Exception("put fail,element is null");
            int c = -1;
            Node<E> node = new Node<E>(e);
            lock(putLock)
            {
                while (GetCount() == capacity)
                    NotFullWait();
                Enqueue(node);
                c = GetAndIncrementCount();
                if (c + 1 < capacity)
                    NotFullSignal();
            }
            if (c == 0)
                NotEmptySignal();
        }
        public E take()
        {
            E x;
            int c = -1;
            lock(takeLock)
            {
                while (GetCount() == 0)
                    NotEmptyWait();
                x = Dequeue();
                c = GetAndDecrementCount();
                if (c > 1)
                    NotEmptySignal();
            }
            
            if (c == capacity)
                NotFullSignal();
            return x;
        }
        public E Poll()
        {
            if (GetCount() == 0)
                return null;
            E x = null;
            int c = -1;
            lock(takeLock)
            {
                if(GetCount() > 0)
                {
                    x = Dequeue();
                    c = GetAndDecrementCount();
                    if (c > 1)
                        NotEmptySignal();
                }
            }
            if(c == capacity)
                NotFullSignal();
            return x;
        }
        public int GetAndIncrementCount()
        {
            lock (countLock)
            {
                int rs = count;
                count++;
                return rs;
            } 
        }
        public int GetAndDecrementCount()
        {
            lock (countLock)
            {
                int rs = count;
                count--;
                return rs;
            }
        }
        public int GetCount()
        {
            lock (countLock)
                return count;
        }
        public void NotFullWait()
        {
            lock (putLock)
                Monitor.Wait(putLock);
        }
        public void NotFullSignal()
        {
            lock (putLock)
                Monitor.PulseAll(putLock);
        }
        private void NotEmptyWait()
        {
            lock (takeLock)
                Monitor.Wait(takeLock);
        }
        private void NotEmptySignal()
        {
            lock (takeLock)
                Monitor.PulseAll(takeLock);
        }
    }
}
