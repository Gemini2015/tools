using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;

namespace FilePoster
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private App mApp;
        private string mCurrentFocuseTile;
        private ObservableCollection<FileData> mFileDataCollection;

        public MainWindow()
        {
            InitializeComponent();
            mApp = (App)Application.Current;
            mCurrentFocuseTile = "";
            mFileDataCollection = new ObservableCollection<FileData>();
            mTable.ItemsSource = mFileDataCollection;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnCreateFolder(object sender, RoutedEventArgs e)
        {
            CreateFolder createFolder = new CreateFolder();
            createFolder.Owner = this;
            createFolder.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ret = createFolder.ShowDialog();
            if(ret.HasValue && ret.Value == true)
            {
                string filepath = createFolder.mFilePath.Text;
                if (mApp.FPM.AddFolder(filepath) == FPStatus.OK)
                {
                    AddFolderTile(filepath);
                }
            }
        }

        void LabelTile_Drop(object sender, DragEventArgs e)
        {
            Label label = sender as Label;
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                FileAttributes fi = File.GetAttributes(fileName);
                if((fi & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string[] fileList = Directory.GetFiles(fileName);
                    if(fileList.Length > 0)
                        mApp.FPM.AddFileList(label.Tag as string, fileList);
                    UpdateTileData(label);
                    
                }
                else if ((fi & FileAttributes.Archive) == FileAttributes.Archive)
                {
                    mApp.FPM.AddFile(label.Tag as string, fileName);
                    UpdateTileData(label);
                }
                else
                {
                    MessageBox.Show("什么鬼！");
                }
            }
            else
            {
                MessageBox.Show("不支持的文件格式。");
            }
            e.Handled = true;
        }

        void LabelTile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Label thislabel = sender as Label;
            UpdateTileFocus(thislabel.Tag as string);
            e.Handled = true;
        }

        private void UpdateTileFocus(string current)
        {
            if (mCurrentFocuseTile == current)
                return;

            mCurrentFocuseTile = current;
            UIElementCollection items = mWrapPanel.Children;
            System.Collections.IEnumerator it = items.GetEnumerator();
            while (it.MoveNext())
            {
                Label label = (Label)it.Current;
                if (label != null)
                {
                    if (string.Compare(current, (string)label.Tag) == 0)
                    {
                        label.Style = (Style)this.Resources["LabelTileFocus"];
                    }
                    else label.Style = (Style)this.Resources["LabelTileNormal"];
                }
            }

            if(current == "")
            {
                mCurrentFolderName.Content = "全局";
            }
            else
            {
                mCurrentFolderName.Content = current;
                FPFolder folder = mApp.FPM.GetFolder(current);
                if(folder != null && folder.mStatus == FPStatus.OK)
                    mStatusBarFolderPath.Content = folder.mPath;
            }

            UpdateDataGrid(mCurrentFocuseTile);
        }

        private void UpdateTileData(Label label)
        {
            if (label != null)
            {
                label.Content = label.Tag + "\n" + "Files: " + mApp.FPM.GetFolderFilesCount(label.Tag as string);
            }
            else
            {
                System.Collections.IEnumerator it = mWrapPanel.Children.GetEnumerator();
                while(it.MoveNext())
                {
                    Label lb = it.Current as Label;
                    lb.Content = lb.Tag + "\n" + "Files: " + mApp.FPM.GetFolderFilesCount(lb.Tag as string);
                }
            }

            if(label == null)
            {
                UpdateDataGrid("");
            }
            else
            {
                UpdateDataGrid(label.Tag as string);
            }
        }

        private void UpdateDataGrid(string current)
        {
            if (mCurrentFocuseTile == "")
            {
                mFileDataCollection.Clear();

                IDictionary<string, FPFolder> mFolderMap = mApp.FPM.GetAllFolder();
                foreach (KeyValuePair<string, FPFolder> kv in mFolderMap)
                {
                    IEnumerator<FPFile> it = kv.Value.mFileList.GetEnumerator();
                    while(it.MoveNext())
                    {
                        if(it.Current != null)
                        {
                            FileData filedata = new FileData();
                            filedata.Name = it.Current.mSrcName;
                            filedata.Path = it.Current.mSrcPath;
                            filedata.Folder = kv.Key;
                            filedata.Status = it.Current.mStatus;

                            mFileDataCollection.Add(filedata);
                        }
                    }
                }
            }
            else if(mCurrentFocuseTile == current)
            {
                mFileDataCollection.Clear();
                FPFolder folder = mApp.FPM.GetFolder(current);
                if(folder != null)
                {
                    IEnumerator<FPFile> it = folder.mFileList.GetEnumerator();
                    while (it.MoveNext())
                    {
                        if (it.Current != null)
                        {
                            FileData filedata = new FileData();
                            filedata.Name = it.Current.mSrcName;
                            filedata.Path = it.Current.mSrcPath;
                            filedata.Folder = folder.mName;
                            filedata.Status = it.Current.mStatus;

                            mFileDataCollection.Add(filedata);
                        }
                    }
                }
            }
        }

        private void OnWrapPanelClick(object sender, MouseButtonEventArgs e)
        {
            UpdateTileFocus("");
            e.Handled = true;
        }

        private void OnWrapPanelDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                FileAttributes fi = File.GetAttributes(fileName);
                if ((fi & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    if(mApp.FPM.AddFolder(fileName) == FPStatus.OK)
                    {
                        AddFolderTile(fileName);
                    }
                }
                else
                {
                    MessageBox.Show("什么鬼！");
                }
            }
            else
            {
                MessageBox.Show("不支持的文件格式。");
            }
            e.Handled = true;
        }

        private void AddFolderTile(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                Label label = new Label();
                label.Content = info.Name + "\n" + "Files: " + mApp.FPM.GetFolderFilesCount(info.Name);
                label.Tag = info.Name;
                //label.Content = info.Name;
                label.Style = (Style)this.Resources["LabelTileNormal"];
                label.MouseLeftButtonUp += LabelTile_MouseLeftButtonUp;
                label.Drop += LabelTile_Drop;
                mWrapPanel.Children.Add(label);

            }
        }

        private void OnCopyAll(object sender, RoutedEventArgs e)
        {
            if (mApp.FPM.Copy() == FPStatus.OK)
            {
                MessageBox.Show("Copy completed!");
            }
            else MessageBox.Show("Something wrong with some files!");
        }

        private void OnMoveAll(object sender, RoutedEventArgs e)
        {
            if(mApp.FPM.Move() == FPStatus.OK)
            {
                MessageBox.Show("Move completed!");
            }
            else MessageBox.Show("Something wrong with some files!");
            UpdateTileData(null);
        }

        private void OnFolderList(object sender, RoutedEventArgs e)
        {
            FolderList folderListDlg = new FolderList();
            folderListDlg.Owner = this;
            folderListDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            folderListDlg.ShowDialog();
        }

        private void OnShowHistory(object sender, RoutedEventArgs e)
        {
            HistoryRecord dlg = new HistoryRecord();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowDialog();
        }

    }

    public class FileData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Folder { get; set; }
        public FPStatus Status { get; set; }
    }
}
