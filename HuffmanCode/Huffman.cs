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
        public void CompressFile (string dataFilename, string archFilename)
        {
            byte[] data = File.ReadAllBytes(dataFilename);// Считываем все данные
            byte[] arch = CompressByte(data);
            File.WriteAllBytes(archFilename, arch);// записываем в архив все данные 

        }

        public void DeCompressFile(string archFileName, string dataFilename)
        {
            byte[] arch = File.ReadAllBytes(archFileName);// Считываем все данные
            byte[] data = DecompressByte(arch);
            File.WriteAllBytes(dataFilename, data);// записываем разархивированные данные данные 
        }

        private byte[] DecompressByte(byte[] arch)
        {
            ParseHeader(arch, out int dataLenght, out int startIndex, out int[] freqs);
            Node root = CreateHuffmanTree(freqs);// строим дерево для разархивации
            byte[] data = Decompress(arch, startIndex, dataLenght, root);
            return data;
        }

        private byte[] Decompress(byte[] arch, int startIndex, int dataLenght, Node root)
        {
            int size = 0;
            Node curr = root;
            List<byte> data = new List<byte>();
            for(int i = startIndex; i < arch.Length; i++)
                for(int bit = 1; bit <= 128; bit <<= 1)
                {
                    bool zero = (arch[i] & bit) == 0;
                    if (zero)
                        curr = curr.bit0;
                    else
                        curr = curr.bit1;
                    if (curr.bit0 != null)
                        continue;
                    if(size++ < dataLenght)
                        data.Add(curr.symbol);
                    curr = root;
                }
            return data.ToArray();

        }

        private void ParseHeader(byte[] arch, out int dataLenght, out int startIndex, out int[] freqs)
        {
            dataLenght =    arch[0] | // вычисляем длину
                    (arch[1] <<  8) | // данных в 4 байтах
                    (arch[2] << 16) | // (каждый по 8 бит)
                    (arch[3] << 24);

            freqs = new int[256];  // массив частотного словоря
            for (int i = 0; i < 256; i++)
            {
                freqs[i] = arch[4 + i];
            }
            startIndex = 4 + 256;
        }

        private byte[] CompressByte(byte[] data)
        {
            int[] freqs = CalculateFreq(data);
            byte[] head = CreateHeader(data.Length, freqs);
            Node root = CreateHuffmanTree(freqs);
            string[] codes = CreateHuffmanCode(root);            
            byte[] bits = Compress(data, codes);
           
            return head.Concat(bits).ToArray();

        }

       

        private byte[] CreateHeader(int datalenght, int[] freqs)
        {
           List<byte> head = new List<byte>();
            head.Add((byte)(datalenght         & 255));
            head.Add((byte)((datalenght >>  8) & 255));
            head.Add((byte)((datalenght >> 16) & 255));
            head.Add((byte)((datalenght >> 24) & 255));
            for(int j = 0; j < 256; j++)
            {
                head.Add((byte)(freqs[j]));
            }
            return head.ToArray();
        }

        private byte[] Compress(byte[] data, string[] codes)
        {
            List<byte> bits = new List<byte>();

            byte sum = 0;
            byte bit = 1;
            foreach (byte symbol in data)
                foreach (char c in codes[symbol])
                {
                    if (c == '1')
                        sum |= bit;
                    if (bit < 128)
                        bit <<= 1;
                    else
                    {
                        bits.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            if (bit > 1)
                bits.Add(sum);

            return bits.ToArray();
        }

        private string[] CreateHuffmanCode(Node root)
        {
            string[] codes = new string[256];
            Next(root, "");
            return codes;

            void Next(Node node, string code)
            {
                if (node.bit0 == null)
                    codes[node.symbol] = code;
                else
                {
                    Next(node.bit0, code + "0");
                    Next(node.bit1, code + "1");
                }
                   
            }
        }

        private int[] CalculateFreq(byte[] data)// составение таблицы символов
        {
            int[] freqs = new int[256];
            foreach (byte d in data)
                freqs[d]++;
            NormalizeFreqs();
            return freqs;


            void NormalizeFreqs()
            {
                int max = freqs.Max();
                if (max <= 255) return;
                for (int i = 0; i < 256; i++)
                    if (freqs[i] > 0)
                        freqs[i] = 1 + freqs[i] * 255 / (max + 1);
            }
        }

        private Node CreateHuffmanTree(int[] freqs)
        {
            PriorityQueue<Node> pq = new PriorityQueue<Node>();
            for (int i = 0; i < 256; i++)
            {
                if(freqs[i] > 0)
                    pq.Enqueue(freqs[i], new Node((byte)i, freqs[i]));
            }

            while (pq.Size() > 1)
            {
                Node bit0 = pq.Dequeue();
                Node bit1 = pq.Dequeue();
                int freq = bit0.freq + bit1.freq;
                Node next = new Node(freq, bit0, bit1);
                pq.Enqueue(freq, next);

            }

            return pq.Dequeue();
        }

    }
}
