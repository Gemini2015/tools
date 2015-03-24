using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FilePost.Util;

namespace FilePost
{
    using IFolderMap = IDictionary<string, FPFolder>;
    public class FPManager
    {
        private IFolderMap mFolderMap;
        private IList<PreferData> mFolderPrefer;

        public IList<PreferData> FolderPrefer
        {
            get
            {
                return mFolderPrefer;
            }
        }
   
        public FPManager()
        {
            mFolderMap = new Dictionary<string, FPFolder>();
            mFolderPrefer = new List<PreferData>();

            XMLUtil.LoadConfig(mFolderPrefer);
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

        public FPStatus SavePreferToXML(string preferName)
        {
            if (preferName == "")
                return FPStatus.Error;

            PreferData preferData = new PreferData("");
            preferData.Name = preferName;
            foreach (KeyValuePair<string, FPFolder> kv in mFolderMap)
            {
                PreferFolderData folderData = new PreferFolderData();
                folderData.Name = kv.Key;
                folderData.Path = kv.Value.mPath;

                preferData.mFolderList.Add(folderData);
            }

            mFolderPrefer.Add(preferData);

            XMLUtil.SaveConfig(mFolderPrefer);

            return FPStatus.OK;
        }

        public FPStatus ApplyPrefer(int index)
        {
            if (index < 0 || index >= mFolderPrefer.Count)
                return FPStatus.Error;
            FPStatus ret = FPStatus.OK;
            PreferData list = mFolderPrefer[index];
            foreach(PreferFolderData data in list.mFolderList)
            {
                if (AddFolder(data.Path) != FPStatus.OK)
                    ret = FPStatus.Error;
            }
            return ret;
        }

        public FPStatus DrawBackFile(string folderName, string fileName)
        {
            if (folderName == "" || fileName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.DrawBackFile(fileName);
            return ret;
        }

        public FPStatus MoveFile(string folderName, string fileName)
        {
            if (folderName == "" || fileName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.MoveFile(fileName);
            return ret;
        }

        public FPStatus CopyFile(string folderName, string fileName)
        {
            if (folderName == "" || fileName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.CopyFile(fileName);
            return ret;
        }

        public FPStatus DrawBackFolder(string folderName)
        {
            if (folderName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.DrawBack();
            return ret;
        }

        public FPStatus MoveFolder(string folderName)
        {
            if (folderName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.Move();
            return ret;
        }

        public FPStatus CopyFolder(string folderName)
        {
            if (folderName == "")
                return FPStatus.Error;

            if (!mFolderMap.ContainsKey(folderName))
                return FPStatus.Error;
            FPFolder folder = mFolderMap[folderName];
            FPStatus ret = folder.Copy();
            return ret;
        }
    }
}
