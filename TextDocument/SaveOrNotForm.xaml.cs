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
using System.Windows.Shapes;

namespace TextDocument
{
    /// <summary>
    /// Логика взаимодействия для SaveOrNotForm.xaml
    /// </summary>
    public partial class SaveOrNotForm : Window
    {
        public SaveOrNotForm(string text)
        {
            InitializeComponent();
            Text = text;
        }

        public string Text { get; set; }

        private void ClickSave(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Текст (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile(), System.Text.Encoding.Default))
                {
                    sw.Write(Text);
                    sw.Close();
                }
            }
            Environment.Exit(0);
        }

        private void ClickNotSave(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ClickBack(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
