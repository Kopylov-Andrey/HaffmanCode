using System.Collections.Generic;

namespace HuffmanCode
{
    internal class PriorityQueue<T>                                 //приоритетная очередь
    {
        int size;                                                   // размер очереди
        SortedDictionary<int, Queue<T>> storage;                    // отсортированный словарь(элемент, словарь)

        public PriorityQueue()                                      // конструктор
        {
            storage = new SortedDictionary<int, Queue<T>>();
            size = 0;

        }

        public int Size() =>  size;                                 // гетер для размера

        public void Enqueue(int priority, T item)                   //добавление элемента в очередь
        {
            if (! storage.ContainsKey(priority))                    // выбираем очередь
                storage.Add(priority, new Queue<T>());              // если нет, то создаем 
            storage[priority].Enqueue(item);                        // добавляем элемент в очередь
            size++;                                                 // увеличиваем размер
        }

        public T Dequeue()                                          // достаем следующий в очереди элемент  
        {
            if (size == 0) 
                throw new System.Exception("Queue is empty");       // заглушка на пустую очередь
            size--;                                                 // уменьшаем размер
            foreach (Queue<T> q in storage.Values)                  //поиск самой приоритетной очереди
                if (q.Count > 0)                                    // если есть хотябы 1 элемент, то достаем первый 
                    return q.Dequeue();
            throw new System.Exception("Queue error");              // заглушка если что-то пойдет не так 
        }
    }
}