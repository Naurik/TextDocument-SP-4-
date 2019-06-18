using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TextDocument
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            comboBoxFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            comboBoxFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            Timer time = new Timer(SaveText, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        }
       
        string copyOrCut = "";
        string textBefore = "";
        bool saveChanged = false;
        string path="";

        //--------------------------
        #region File
        #region Save&SaveAs
        private void ClickToSave(object sender, RoutedEventArgs e)
        {
            textBefore = new TextRange(richInput.Document.ContentStart, richInput.Document.ContentEnd).Text;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.Filter = "(*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.OpenFile(), System.Text.Encoding.Default))
                {
                    sw.Write(textBefore);
                    sw.Close();
                }
            }
            path = saveFileDialog.FileName;
            saveChanged = true;
        }
        #endregion

        #region OpenFile
        private void ClickToOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Текстик (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open);
                TextRange range = new TextRange(richInput.Document.ContentStart, richInput.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }
        #endregion

        #region Print
        private void ClickToPrint(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();
            if (print.ShowDialog() == true)
            {
                print.PrintVisual(richInput, "");
            }
        }
        #endregion

        #region Exit
        private void ClickToExit(object sender, RoutedEventArgs e)
        {
            
            if (saveChanged == true && textBefore==richInput.ToString())
            {
                Close();
            }
            else
            {
                SaveOrNotForm save = new SaveOrNotForm(richInput.ToString());
                save.Show();
            }
        }
        #endregion
        #endregion

        //--------------------------
        #region Correction
        #region Back
        private void ClickBack(object sender, RoutedEventArgs e)
        {
            return ;
        }
        #endregion

        #region CopyCutDeleteInsert
        private void ClickToCopy(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(richInput.ToString()) && richInput.Selection != null)
            {
                copyOrCut = richInput.Selection.Text;
            }
            
        }

        private void ClickToInsert(object sender, RoutedEventArgs e)
        {
            richInput.AppendText(copyOrCut);
        }

        private void ClickToCut(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(richInput.ToString()) && richInput.Selection != null)
            {
                copyOrCut = richInput.Selection.Text;
                richInput.Document.Blocks.Clear();
            }
        }

        private void ClickToDelete(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(richInput.ToString()) && richInput.Selection != null)
            {
                richInput.Cut();
            }
        }
        #endregion

        #endregion
        //--------------------------
        #region Format
        private void ComboFontFamilySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxFontFamily.SelectedItem != null)
                richInput.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, comboBoxFontFamily.SelectedItem);
        }

        private void ComboBoxFontSizeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            richInput.Selection.ApplyPropertyValue(Inline.FontSizeProperty, comboBoxFontSize.Text);
        }
        #endregion

        //--------------------------
        #region View
        //Строка состояния почти ничего не делает, поэтому его функционал не делал
        #endregion

        //--------------------------
        #region Information
        private void ClickToInformation(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.bing.com/search?q=%D1%81%D0%BF%D1%80%D0%B0%D0%B2%D0%BA%D0%B0+%D0%BF%D0%BE+%D0%B8%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D1%8E+%D0%B1%D0%BB%D0%BE%D0%BA%D0%BD%D0%BE%D1%82%D0%B0+%D0%B2+windows%c2%a010&filters=guid:%224466414-ru-dia%22%20lang:%22ru%22&form=T00032&ocid=HelpPane-BingIA");
        }
        #endregion
        
        //Шрифт
        private void RichBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = richInput.Selection.GetPropertyValue(Inline.FontWeightProperty);
            battonBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = richInput.Selection.GetPropertyValue(Inline.FontStyleProperty);
            battonItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = richInput.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            battonUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = richInput.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            comboBoxFontFamily.SelectedItem = temp;
            temp = richInput.Selection.GetPropertyValue(Inline.FontSizeProperty);
            comboBoxFontSize.Text = temp.ToString();
        }


        //Сохранение
        public async void SaveText(object state)
        {
            textBefore= new TextRange(richInput.Document.ContentStart, richInput.Document.ContentEnd).Text;
            MessageBox.Show("Сохраняем");
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                await sw.WriteAsync(textBefore);
                sw.Close();
            }

        }
    }
}
