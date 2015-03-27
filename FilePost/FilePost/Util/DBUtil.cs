using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Windows;

namespace FilePost
{
    class DBUtil
    {
        public static string DBName = "FilePost.sqlite";
        public static string DBPath = "data";


        private static SQLiteConnection _connection = null;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    string fullpath = Path.Combine(DBPath, DBName);
                    if (!File.Exists(fullpath))
                    {
                        Directory.CreateDirectory(DBPath);
                        SQLiteConnection.CreateFile(fullpath);
                    }
                    SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
                    builder.DataSource = fullpath;
                    _connection = new SQLiteConnection(builder.ConnectionString);
                    _connection.Open();
                }
                return _connection;
            }
        }


        public static int ExecuteNonQuery(string sql, SQLiteParameter[] parameters)
        {
            int affectedRows = 0;

            DbTransaction transaction = Connection.BeginTransaction();
            SQLiteCommand command = new SQLiteCommand(Connection);
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            affectedRows = command.ExecuteNonQuery();
            transaction.Commit();

            return affectedRows;
        }

        public static DataTable ExecuteQuery(string sql, SQLiteParameter[] parameters)
        {
            DataTable data = new DataTable();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            return data;
        }

        public static void CreateTable()
        {
            string sql = "create table if not exists record ("
                + "No integer primary key autoincrement,"
                + "Name text,"
                + "SrcPath text,"
                + "DestPath text,"
                + "Method text,"
                + "Status text,"
                + "recordtime datetime)";

            DBUtil.ExecuteNonQuery(sql, null);
        }

        public static void AddRecord(FPFile file, string method)
        {
            string sql = "insert into record(name, srcpath, destpath, method, status, recordtime) values"
                + "(@name, @srcpath, @destpath, @method, @status, @recordtime)";

            SQLiteParameter[] ps = new SQLiteParameter[]{
                new SQLiteParameter("@name", file.mSrcName),
                new SQLiteParameter("@srcpath", file.mSrcPath),
                new SQLiteParameter("@destpath", file.mDstPath),
                new SQLiteParameter("@method", method),
                new SQLiteParameter("@status", FPFile.GetStatusString(file.mStatus)),
                new SQLiteParameter("@recordtime", DateTime.Now)
            };

            DBUtil.ExecuteNonQuery(sql, ps);
        }

        public static IList<FPRecord> GetAllRecord()
        {
            string sql = "select * from record";
            DataTable table = DBUtil.ExecuteQuery(sql, null);
            IList<FPRecord> recordList = new List<FPRecord>();
            System.Collections.IEnumerator it = table.Rows.GetEnumerator();
            while(it.MoveNext())
            {
                DataRow row = it.Current as DataRow;
                FPRecord record = new FPRecord();
                record.Name = row["name"].ToString();
                record.SrcPath = row["srcpath"].ToString();
                record.DestPath = row["destpath"].ToString();
                record.Method = row["method"].ToString();
                record.Status = row["status"].ToString();
                record.Datetime = row["recordtime"].ToString();

                recordList.Add(record);
            }
            return recordList;
        }

        public static bool CheckDepedencies()
        {
            bool ret = true;
            if(!File.Exists("System.Data.SQLite.dll"))
            {
                MessageBox.Show("Missing System.Data.SQLite.dll!");
                ret = false;
            }

            if(!File.Exists("SQLite.Interop.dll"))
            {
                MessageBox.Show("Missing SQLite.Interop.dll");
                ret = false;
            }
            return ret;
        }
    
    }

    public struct FPRecord
    {
        public string Name { get; set; }
        public string SrcPath { get; set; }
        public string DestPath { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string Datetime { get; set; }
    }
}
