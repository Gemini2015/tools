using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FilePoster
{

    public class FPFolder
    {
        public string mName;
        public string mPath;
        public FPStatus mStatus;

        public IList<FPFile> mFileList;

        public FPFolder(string name, string path)
        {
            mName = name;
            mPath = path;
            mFileList = new List<FPFile>();
        }

        public FPFolder(string pathname)
        {
            if (pathname.Length == 0)
            {
                mStatus = FPStatus.None;
                return;
            }
            if(!Directory.Exists(pathname))
            {
                mStatus = FPStatus.Not_Exists;
                return;
            }
            DirectoryInfo info = new DirectoryInfo(pathname);
            mName = info.Name;
            mPath = pathname;
            mFileList = new List<FPFile>();
            mStatus = FPStatus.OK;
        }

        public bool AddFile(FPFile file)
        {
            file.SetDstPath(mPath);
            if(file.mStatus != FPStatus.OK)
            {
                return false;
            }
            bool bExists = false;
            foreach(FPFile fi in mFileList)
            {
                if(fi.mSrcName == file.mSrcName &&
                    fi.mSrcPath == file.mSrcPath)
                {
                    bExists = true;
                    break;
                }
            }
            if (bExists)
                return false;
            mFileList.Add(file);
            return true;
        }

        public bool RemoveFile(FPFile file)
        {
            if (file.mSrcName.Length == 0 || file.mSrcPath.Length == 0)
                return true;
            IEnumerator<FPFile> it = mFileList.GetEnumerator();
            while(it.MoveNext())
            {
                if(it.Current.mSrcName == file.mSrcName &&
                    it.Current.mSrcPath == file.mSrcPath)
                {
                    mFileList.Remove(it.Current);
                    break;
                }
            }
            return true;
        }

        public FPStatus Move()
        {
            FPStatus ret = FPStatus.OK;
            IEnumerator<FPFile> it = mFileList.GetEnumerator();
            while(it.MoveNext())
            {
                if(it.Current.Move() != FPStatus.OK)
                    ret = FPStatus.Error;
            }
            return ret;
        }

        public FPStatus Copy()
        {
            FPStatus ret = FPStatus.OK;
            IEnumerator<FPFile> it = mFileList.GetEnumerator();
            while(it.MoveNext())
            {
                if(it.Current != null && it.Current.Copy() != FPStatus.OK)
                    ret = FPStatus.Error;
            }
            return ret;
        }

        public FPStatus DrawBack()
        {
            mFileList.Clear();
            return FPStatus.OK;
        }

        public override string ToString()
        {
            string str = mName;
            if(mFileList != null)
            {
                str += Environment.NewLine + mFileList.Count;
            }
            return str;
        }

        public int GetCount()
        {
            if (mFileList != null)
                return mFileList.Count;
            else return 0;
        }
        

        //public override bool Equals(object obj)
        //{
        //    if (obj == null || !(obj is FPFile))
        //        return false;
        //    FPFolder folder = (FPFolder)obj;
        //    if (this.mName == folder.mName &&
        //        this.mPath == folder.mPath)
        //        return true;
        //    else return false;
        //}
    }
}
