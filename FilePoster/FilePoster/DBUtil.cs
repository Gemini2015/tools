﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.IO;

namespace FilePoster
{
    class DBUtil
    {
        public static string DBName = "FilePoster.sqlite";
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
                + "Status text,"
                + "recordtime datetime)";

            DBUtil.ExecuteNonQuery(sql, null);
        }

        public static void AddRecord(FPFile file)
        {
            string sql = "insert into record(name, srcpath, destpath, status, recordtime) values"
                + "(@name, @srcpath, @destpath, @status, @recordtime)";

            SQLiteParameter[] ps = new SQLiteParameter[]{
                new SQLiteParameter("@name", file.mSrcName),
                new SQLiteParameter("@srcpath", file.mSrcPath),
                new SQLiteParameter("@destpath", file.mDstPath),
                new SQLiteParameter("@status", FPFile.GetStatusString(file.mStatus)),
                new SQLiteParameter("@recordtime", DateTime.Now)
            };

            DBUtil.ExecuteNonQuery(sql, ps);
        }
    
    }
}