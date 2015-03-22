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
using System.Collections.ObjectModel;

namespace FilePoster
{
    /// <summary>
    /// FolderList.xaml 的交互逻辑
    /// </summary>
    public partial class FolderList : Window
    {
        private App mApp;
        private ObservableCollection<FolderListData> mFolderListCollection;
        public FolderList()
        {
            InitializeComponent();
            mFolderListCollection = new ObservableCollection<FolderListData>();
            mFolderListTable.ItemsSource = mFolderListCollection;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mApp = Application.Current as App;
            mFolderListCollection.Clear();
            IDictionary<string, FPFolder> mFolderDic = mApp.FPM.GetAllFolder();
            foreach (KeyValuePair<string, FPFolder> kv in mFolderDic)
            {
                FolderListData data = new FolderListData();
                data.Name = kv.Key;
                data.Path = kv.Value.mPath;
                data.Quantity = kv.Value.mFileList.Count.ToString();

                mFolderListCollection.Add(data);
            }
        }

        public struct FolderListData
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Quantity { get; set; }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
