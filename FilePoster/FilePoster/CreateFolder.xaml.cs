using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;

namespace FilePoster
{
    /// <summary>
    /// CreateFolder.xaml 的交互逻辑
    /// </summary>
    public partial class CreateFolder : Window
    {
        public CreateFolder()
        {
            InitializeComponent();
        }

        private void OnBtnBrowse(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mFilePath.Text = dlg.SelectedPath;
                DirectoryInfo info = new DirectoryInfo(dlg.SelectedPath);
                mFolderName.Content = info.Name;
            }
        }

        private void OnBtnOK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnBtnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void mFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)sender;
            string filepath = textbox.Text;
            if(Directory.Exists(filepath))
            {
                DirectoryInfo info = new DirectoryInfo(filepath);
                mFolderName.Content = info.Name;
            }
        }
    }
}
