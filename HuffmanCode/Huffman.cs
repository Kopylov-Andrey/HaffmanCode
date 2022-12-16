using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCode
{
    class Huffman
    {
        const int BITS_PER_BYTE = 256;
        const int BITS_PER_BYTE_WITH_ZERO = 255;
        

        public void CompressFile (string dataFilename, string archFilename)                 //Метод архивирования файла
        {       
            byte[] data = File.ReadAllBytes(dataFilename);                                  // Считываем все данные
            byte[] arch = CompressByte(data);                                               // вызываем метод сжатия данных
            File.WriteAllBytes(archFilename, arch);                                         // записываем в архив все данные 

        }

        public void DeCompressFile(string archFileName, string dataFilename)                //Разархивация
        {
            byte[] arch = File.ReadAllBytes(archFileName);                                  // Считываем все данные
            byte[] data = DecompressByte(arch);                                             // Разархивируем данные
            File.WriteAllBytes(dataFilename, data);                                         // записываем разархивированные данные данные 
        }

        private byte[] DecompressByte(byte[] arch)                                          // Разархивируем данные
        {
            ParseHeader(arch, out int dataLenght, out int startIndex, out int[] freqs);     // Читаем заголовок
            Node root = CreateHuffmanTree(freqs);                                           // строим дерево для разархивации
            byte[] data = Decompress(arch, startIndex, dataLenght, root);                   // разархивация
            return data;
        }

        private byte[] Decompress(byte[] arch, int startIndex, int dataLenght, Node root)   // разархивация
        {
            int size = 0;                                                                   // размер 
            Node curr = root;                                                               // текущее положение дерева
            List<byte> data = new List<byte>();                                             //массив для данных
            for (int i = startIndex; i < arch.Length; i++)                                  // перебераем со стартового индекса 
                for (int bit = 1; bit <= BITS_PER_BYTE/2; bit <<= 1) //почему 128(то же, что и < 256)   // перебираем биты (1, 2, 4, 8 и тд)
                {
                    bool zero = (arch[i] & bit) == 0;                                       // проверка на символ бита 
                    if (zero)
                        curr = curr.bit0;                                                   // если ноль, то идем по ветке бит 0
                    else
                        curr = curr.bit1;                                                   // иначе идем по ветке бит 1 
                    if (curr.bit0 != null)                                                  // если ссылка дальше есть то переходим к следующему биту 
                        continue;
                    if(size++ < dataLenght)                                                 // если дошли до листа то нужно записать результат
                        data.Add(curr.symbol);                                              // записываем символ вмассив
                    curr = root;                                                            // начинаем опять с корня
                }
            return data.ToArray();                                                          // возвращаем разархивированный массив

        }

        private void ParseHeader(byte[] arch, out int dataLenght, out int startIndex, out int[] freqs)
        {
            dataLenght =    arch[0] |                                                       // вычисляем длину
                    (arch[1] <<  8) |                                                       // данных в 4 байтах
                    (arch[2] << 16) |                                                       // (каждый по 8 бит)
                    (arch[3] << 24);

            freqs = new int[BITS_PER_BYTE];                                                           // массив частотного словоря
            for (int i = 0; i < BITS_PER_BYTE; i++)       
            {
                freqs[i] = arch[4 + i];                                                     // 4 байта было потрачено на заголовок
            }
            startIndex = 4 + BITS_PER_BYTE;                                                           // начальный индекс закодированного файла
        }

        private byte[] CompressByte(byte[] data)                                            // метод сжатия данных
        {
            int[] freqs = CalculateFreq(data);                                              // создаем частотный словарь
            byte[] head = CreateHeader(data.Length, freqs);                                 // создание заголовка(длина массива, частотный словари)
            Node root = CreateHuffmanTree(freqs);                                           // создание дерева 
            string[] codes = CreateHuffmanCode(root);                                       // создаем код для каждого символа        
            byte[] bits = Compress(data, codes);                                            // сжатие данных (данные, коды)

            return head.Concat(bits).ToArray();                                             // возвращаем заголовок и массив битов

        }

       

        private byte[] CreateHeader(int datalenght, int[] freqs)                            // Создание заголовка
        {
            List<byte> head = new List<byte>();                                             //список заголовка//длина(выделяется 4 байта)
            head.Add((byte)(datalenght         & BITS_PER_BYTE_WITH_ZERO));                                     //1 байт (младшие(последние) 8 бит)
            head.Add((byte)((datalenght >>  8) & BITS_PER_BYTE_WITH_ZERO));                                     //2 байт сдвигаем на 8 бит
            head.Add((byte)((datalenght >> 16) & BITS_PER_BYTE_WITH_ZERO));                                     //3 байт сдвигаем уже на 16 бит
            head.Add((byte)((datalenght >> 24) & BITS_PER_BYTE_WITH_ZERO));                                     //4 байт сдвигаем на 24 бит
            for (int j = 0; j < BITS_PER_BYTE; j++)                                                   //записываем таблицу 
            {                                                                               //записать 256 чисел каждое из которых означает очередной байт
                head.Add((byte)(freqs[j]));
            }
            return head.ToArray();
        }

        private byte[] Compress(byte[] data, string[] codes)                                // сжатие данных
        {
            List<byte> bits = new List<byte>();                                             //динамический список байтов

            byte sum = 0;                                                                   //накапливаем сумму очередного байта
            byte bit = 1;                                                                   //позиция бита 1, 2, 4, 8 и т.д. 
            foreach (byte symbol in data)                                                   // перебор всех символов в исходном файле
                foreach (char c in codes[symbol])                                           // достаем код символа
                {                                                                           // разбираем код на отдельные смволы
                    if (c == '1')
                        sum |= bit;                                                         // добавляем бит через дизъюнкцию 
                    if (bit < BITS_PER_BYTE/2)                                                          // если бит меньше 128
                        bit <<= 1;                                                          // сдвигаем бит
                    else
                    {
                        bits.Add(sum);                                                      // иначе,  байт готов и добавляем его в массив
                        sum = 0;                                                            // обнуляем сумму
                        bit = 1;                                                            // обнуляем позицию
                    }
                }
            if (bit > 1)                                                                    // проверка на оставшиеся неполные биты
                bits.Add(sum);                                                              // их тоже добавляем, чтоб не потерять данные

            return bits.ToArray();
        }

        private string[] CreateHuffmanCode(Node root)                                       // создание кода для каждого элемента 
        {                                                                                   //(реализация алгоритма поиска вглубину)
            string[] codes = new string[BITS_PER_BYTE]; 
            Next(root, "");                                                                 // рекурсивная функция (корневой элемент, начальный символ)
            return codes;

            void Next(Node node, string code)                                               // новый элемент и создание кода для него
            {
                if (node.bit0 == null)                                                      // если дошли до листа 
                    codes[node.symbol] = code;                                              // передаем первый смимвол кода 
                else
                {
                    Next(node.bit0, code + "0");                                            // идем по нулевому биту и дописываем ноль к коду
                    Next(node.bit1, code + "1");                                            // идем по первому биту и дописываем один к коду
                }
                   
            }
        }

        private int[] CalculateFreq(byte[] data)                                            //создание частотного словаря (составение таблицы символов)
        {
            int[] freqs = new int[BITS_PER_BYTE];                                                     // пустой словарь (256 - кол-во значений в 1 байте)
            foreach (byte d in data)                                                        // перебор всех байтов
                freqs[d]++;                                                                 // считаем кол-во
            NormalizeFreqs();                                                               //нормализация соворя для исключения ошибок при файле размером более 255
            return freqs;


            void NormalizeFreqs() //??                                                          // тут не важно
            {
                int max = freqs.Max();
                if (max <= BITS_PER_BYTE_WITH_ZERO) return;
                for (int i = 0; i < BITS_PER_BYTE; i++)
                    if (freqs[i] > 0)
                        freqs[i] = 1 + freqs[i] * BITS_PER_BYTE_WITH_ZERO / (max + 1);
            }
        }

        private Node CreateHuffmanTree(int[] freqs)                                         // создание дерева
        {
            PriorityQueue<Node> pq = new PriorityQueue<Node>();                             // приоритетная очередь
            for (int i = 0; i < BITS_PER_BYTE; i++)                                                  // перебор элементов 
            {
                if(freqs[i] > 0)                                                           // добавлям только те, которе больше 0
                    pq.Enqueue(freqs[i], new Node((byte)i, freqs[i]));                     // добавляем элемент и его частоту
            }

            while (pq.Size() > 1)                                                          // работа с очередью(пока размер больше 1)
            {
                Node bit0 = pq.Dequeue();                                                  // берем два 
                Node bit1 = pq.Dequeue();                                                  //  элемента
                int freq = bit0.freq + bit1.freq;                                          // суммируем частоту нулевого и первого бита 
                Node next = new Node(freq, bit0, bit1);                                    // создаем новый общий узел из двух предыдущих
                pq.Enqueue(freq, next);                                                    // добавляем его в очередь

            }

            return pq.Dequeue();                                                           // возврщаем последний элемент который остался в очереди
        }

    }
}
