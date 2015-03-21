using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FilePoster
{
    using IFolderMap = IDictionary<string, FPFolder>;
    using IFolderSetMap = IDictionary<string, IDictionary<string, FPFolder>>;
    public class FPManager
    {
        private IFolderMap mFolderMap;
        private IFolderSetMap mFolderSetMap;
   
        public FPManager()
        {
            mFolderMap = new Dictionary<string, FPFolder>();
            mFolderSetMap = new Dictionary<string, IFolderMap>();

            DBUtil.CreateTable();
        }

        public FPStatus Copy()
        {
            FPStatus ret = FPStatus.OK;
            foreach (KeyValuePair<string, FPFolder> item in mFolderMap)
            {
                if(item.Value != null && item.Value.Copy() != FPStatus.OK)
                {
                    ret = FPStatus.Error;
                }
            }
            return ret;
        }

        public FPStatus Move()
        {
            FPStatus ret = FPStatus.OK;
            foreach (KeyValuePair<string, FPFolder> item in mFolderMap)
            {
                if (item.Value != null && item.Value.Move() != FPStatus.OK)
                {
                    ret = FPStatus.Error;
                }
            }
            return ret;
        }


        public FPStatus DrawBack()
        {
            FPStatus ret = FPStatus.OK;
            foreach (KeyValuePair<string, FPFolder> item in mFolderMap)
            {
                if (item.Value != null && item.Value.DrawBack() != FPStatus.OK)
                {
                    ret = FPStatus.Error;
                }
            }
            return ret;
        }

        public FPStatus AddFile(string folderName, string fileName)
        {
            if(!File.Exists(fileName))
                return FPStatus.Not_Exists;
            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;

            FPFolder folder = mFolderMap[folderName];
            if(folder == null)
                return FPStatus.Error;

            FileInfo info = new FileInfo(fileName);
            FPFile file = new FPFile(info.Name, info.DirectoryName);

            if (folder.AddFile(file))
                return FPStatus.OK;
            else return FPStatus.Error;
        }

        public FPStatus AddFileList(string folderName, string[] fileList)
        {
            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            if (folder == null)
                return FPStatus.Error;

            FPStatus ret = FPStatus.OK;
            foreach (string fileName in fileList)
            {
                FileInfo info = new FileInfo(fileName);
                if (!info.Exists)
                {
                    ret = FPStatus.Not_Exists;
                    continue;
                }

                FPFile file = new FPFile(info.Name, info.DirectoryName);
                if(folder.AddFile(file))
                {
                    ret = FPStatus.Error;
                }
            }

            return ret;
        }

        public IDictionary<string, FPFolder> GetAllFolder()
        {
            return mFolderMap;
        }

        public FPFolder GetFolder(string folderName)
        {
            if (!mFolderMap.ContainsKey(folderName))
                return null;
            return mFolderMap[folderName];
        }

        public int GetFolderFilesCount(string folderName)
        {
            if (!mFolderMap.ContainsKey(folderName))
                return 0;
            FPFolder folder = mFolderMap[folderName];
            if (folder == null)
                return 0;

            return folder.GetCount();
        }

        public FPStatus AddFolder(string filename)
        {
            DirectoryInfo info = new DirectoryInfo(filename);
            if (!info.Exists)
                return FPStatus.Not_Exists;

            if (mFolderMap.ContainsKey(info.Name))
                return FPStatus.Already_Exists;

            FPFolder folder = new FPFolder(filename);
            mFolderMap[info.Name] = folder;
            return FPStatus.OK;
        }

    }
}
