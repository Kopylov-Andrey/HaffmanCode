using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HuffmanCode
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentfilePath;
        Huffman huffman;
        Label filePath;

        public MainWindow()
        {
            InitializeComponent();
            Programm();
        }

        public void  Programm()
        {
           huffman = new Huffman();
            filePath = FilePath;

           // huffman.CompressFile("C:/Users/HP/Desktop/a.txt", "a.txt.huf"); // открываем файл и сжимаем его
           // huffman.DeCompressFile("a.txt.huf", "a.txt.huf.txt");           // открываем архив и разархивируем его

        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)       // элеметы интерфейса (в них можно не разбираться)
        {
            richTextBox.Document.Blocks.Clear();
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Text files (*.txt)|*.txt| " +
                        "Huf files (*.huf)|*.huf| " +
                        "All files(*.*)|*.*";
            op.ShowDialog();
            if (op.FileName == null) return;

            else
            {
                if (File.Exists(op.FileName))
                    currentfilePath = op.FileName;
                else return;
            }
            string[] file = File.ReadAllLines(currentfilePath);
            for (int i = 0; i < file.Length; i++)
                richTextBox.AppendText(file[i] + "\n");
            filePath.Content = currentfilePath;

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            huffman.CompressFile(currentfilePath, "a.txt.huf");
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_Click_Encode(object sender, RoutedEventArgs e)
        {

            string defaultFileName = "Default Arh Name";
            string txtFile = "";

            if (currentfilePath != null)
                txtFile = currentfilePath.Substring(currentfilePath.Length - 4);
            

            if (txtFile != ".txt")
            {
                MessageBox.Show("Для архивации откройте файл формата '.txt'", "Предупреждение");
                
            }
            else
            {
                if (arhFileName.Text == "")
                    huffman.CompressFile(currentfilePath, defaultFileName + ".txt.huf");

                else
                    huffman.CompressFile(currentfilePath, arhFileName.Text + ".txt.huf");
            }    

            
           
           
        }

        private void Btn_Click_Decode(object sender, RoutedEventArgs e)
        {
            string defaultDearhName = "Default Dearh Name";
            string hufFile = "";

            if (currentfilePath != null)
                hufFile = currentfilePath.Substring(currentfilePath.Length - 4);

            if (hufFile != ".huf")
            {
                MessageBox.Show("Для разархивации откройте файл формата'.huf'", "Предупреждение");

            }
            else
            {

                if (deArhFileName.Text == "")
                    huffman.DeCompressFile(currentfilePath, defaultDearhName + ".txt.huf.txt");
                else
                    huffman.DeCompressFile(currentfilePath, deArhFileName.Text + ".txt.huf.txt");
            }
        }

      
    }
}
