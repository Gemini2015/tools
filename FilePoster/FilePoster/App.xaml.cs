using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FilePoster
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private FPManager mFPManager;

        public FPManager FPM
        {
            get
            {
                if(mFPManager == null)
                {
                    mFPManager = new FPManager();
                }
                return mFPManager;
            }
        }

    }
}
