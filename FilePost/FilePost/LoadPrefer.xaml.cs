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
using FilePost.Util;

namespace FilePost
{
    /// <summary>
    /// LoadPrefer.xaml 的交互逻辑
    /// </summary>
    public partial class LoadPrefer : Window
    {
        private IList<PreferData> mPreferListData;

        public LoadPrefer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App app = Application.Current as App;
            mPreferListData = app.FPM.FolderPrefer;

            if (mPreferListData != null && mPreferListData.Count != 0)
            {
                mPreferList.ItemsSource = mPreferListData;
            }
        }

        private void OnSelectPrefer(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList list = e.AddedItems;
            PreferData data = (PreferData)list[0];
            mFolderList.ItemsSource = data.mFolderList;
            
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (mPreferList.SelectedIndex == -1)
                return;
            this.DialogResult = true;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
