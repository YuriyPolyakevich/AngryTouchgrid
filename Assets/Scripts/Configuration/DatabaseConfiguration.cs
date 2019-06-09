using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Configuration
{
    public class DatabaseConfiguration
    {
        private const string FileName = "GameDatabase.bytes";
        private static string _databasePath;
        private static SqliteConnection connection;
        private static SqliteCommand command;

        public DatabaseConfiguration()
        {
        }

//TODO: КАКИМ-ТО ХУЕМ ПРИЛЕПИТЬ СЮДА ORM!!!!!!!

        private static string GetDatabasePath()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, FileName);
#endif
#if UNITY_STANDALONE
            string filePath = Path.Combine(Application.dataPath, fileName);
            if(!File.Exists(filePath)) UnpackDatabase(filePath);
            return filePath;
#elif UNITY_ANDROID
            var filePath = Path.Combine(Application.persistentDataPath, FileName);
            if (!File.Exists(filePath)) UnpackDatabase(filePath);
            return filePath;
#endif
        }
        
        private static void UnpackDatabase(string toPath)
        {
            var fromPath = Path.Combine(Application.streamingAssetsPath, FileName);

            var reader = new WWW(fromPath);
            while (!reader.isDone) { }

            File.WriteAllBytes(toPath, reader.bytes);
        }
        
//        private static void OpenConnection()
//        {
//            _databasePath = GetDatabasePath();
//            connection = new SqliteConnection("Data Source=" + _databasePath);
//            command = new SqliteCommand(connection);
//            connection.Open();
//        }
//
//        public static void CloseConnection()
//        {
//            connection.Close();
//            command.Dispose();
//        }
//
//        public static void ExecuteQueryWithoutAnswer(string query)
//        {
//            OpenConnection();
//            command.CommandText = query;
//            command.ExecuteNonQuery();
//            CloseConnection();
//        }
//        
//        public static string ExecuteQueryWithAnswer(string query)
//        {
//            OpenConnection();
//            command.CommandText = query;
//            var answer = command.ExecuteScalar();
//            CloseConnection();
//
//            if (answer != null) return answer.ToString();
//            else return null;
//        }
//
//        public static DataTable GetTable(string query)
//        {
//            OpenConnection();
//
//            SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);
//
//            DataSet DS = new DataSet();
//            adapter.Fill(DS);
//            adapter.Dispose();
//
//            CloseConnection();
//
//            return DS.Tables[0];
//        }
    }
}