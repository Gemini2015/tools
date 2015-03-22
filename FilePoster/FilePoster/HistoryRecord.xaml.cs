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

namespace FilePoster
{
    /// <summary>
    /// HistoryRecord.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryRecord : Window
    {
        public HistoryRecord()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<FPRecord> recordList = DBUtil.GetAllRecord();
            mHistoryRecordTable.ItemsSource = recordList;
        }


    }
}
