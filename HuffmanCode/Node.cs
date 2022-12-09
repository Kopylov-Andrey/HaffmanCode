using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCode
{
    internal class Node                                 // класс для хранения данных о каждом элементе
    {
        public readonly byte symbol;                    // симвод
        public readonly int freq;                       // частота
        public readonly Node bit0;                      // ссылка на узел с битом 0
        public readonly Node bit1;                      // ссылка на узел с битом 1

        public Node(byte symbol, int freq)              // конструктор для создания списка
        {
            this.symbol = symbol;
            this.freq = freq;
        }

        public Node(int freq, Node bit0, Node bit1)     // конструктор для создания ссылки на узел
        {
            this.freq = freq;
            this.bit0 = bit0;
            this.bit1 = bit1;
        }
    }
}
