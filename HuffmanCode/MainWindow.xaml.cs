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

        public MainWindow()
        {
            InitializeComponent();
            Programm();
        }

        public void  Programm()
        {
            Huffman huffman = new Huffman();

            huffman.CompressFile("C:/Users/HP/Desktop/a.txt", "a.txt.huf"); // открываем файл и сжимаем его
            huffman.DeCompressFile("a.txt.huf", "a.txt.huf.txt");           // открываем архив и разархивируем его

        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)       // элеметы интерфейса (в них можно не разбираться)
        {
            
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
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
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
