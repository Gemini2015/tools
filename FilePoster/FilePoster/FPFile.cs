using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FilePoster
{
    public enum FPStatus
    {
        None,
        OK,
        No_Dst,
        Not_Exists,
        Already_Exists,
        Error,
    }

    public class FPFile
    {
        
        public string mSrcName;
        public string mDstName;
        public string mSrcPath;
        public string mDstPath;
 
        public FPStatus mStatus;
        
        public FPFile(string srcName, string srcPath, string dstPath)
        {
            mSrcName = srcName;
            mDstName = srcName;
            mSrcPath = srcPath;
            mDstPath = dstPath;
        }

        public FPFile(string srcName, string srcPath)
        {
            mSrcName = srcName;
            mDstName = mSrcName;
            mSrcPath = srcPath;
            mDstPath = "";
        }

        public FPStatus UpdateStatus()
        {
            if (mSrcPath.Length == 0 || mSrcName.Length == 0)
            {
                mStatus = FPStatus.None;
                return mStatus;
            }
            string srcPathName = Path.Combine(mSrcPath, mSrcName);
            if (!File.Exists(srcPathName))
            {
                mStatus = FPStatus.Not_Exists;
                return mStatus;
            }

            if (mDstPath.Length == 0 || mDstName.Length == 0)
            {
                mStatus = FPStatus.No_Dst;
                return mStatus;
            }

            string dstPathName = Path.Combine(mDstPath, mDstName);
            if(File.Exists(dstPathName))
            {
                mStatus = FPStatus.Already_Exists;
                return mStatus;
            }
            mStatus = FPStatus.OK;
            return mStatus;
        }

        public void SetDstPath(string path)
        {
            if (path.Length == 0)
                return;
            if (!Directory.Exists(path))
                return;
            mDstPath = path;

            UpdateStatus();
        }

        public FPStatus Move()
        {
            UpdateStatus();
            if (mStatus != FPStatus.OK)
                return mStatus;
            string srcPathName = Path.Combine(mSrcPath, mSrcName);
            string destPathName = Path.Combine(mDstPath, mDstName);
            FPStatus ret = FPStatus.OK;
            try
            {
                File.Move(srcPathName, destPathName);
            }
            catch (System.IO.IOException ex)
            {
                ret = FPStatus.Error;
            }
            return ret;
        }

        public FPStatus Copy()
        {
            UpdateStatus();
            if (mStatus != FPStatus.OK)
                return mStatus;
            string srcPathName = Path.Combine(mSrcPath, mSrcName);
            string destPathName = Path.Combine(mDstPath, mDstName);
            FPStatus ret = FPStatus.OK;
            try
            {
                File.Copy(srcPathName, destPathName);
            }
            catch (System.IO.IOException ex)
            {
                ret = FPStatus.Error;
            }
            return ret;
        }

        //public override bool Equals(object obj)
        //{
        //    FPFile file = (FPFile)obj;
        //    if (this.mSrcName == file.mSrcName
        //        && this.mSrcPath == file.mSrcPath
        //        && this.mDstName == file.mDstName
        //        && this.mDstPath == file.mDstPath
        //        && this.mStatus == file.mStatus)
        //        return true;
        //    else return false;
        //}
        
    }
}
