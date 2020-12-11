using System.Collections.Generic;
using Microsoft.Data.Sqlite;
namespace emote_gui_dotnet_win.DB
{
    public class EmoteQueryClient
    {
        private SqliteConnection _conx = new SqliteConnection("Data Source=images.db");
        private SqliteCommand _cmmd;
        private SqliteParameter _param;

        public EmoteQueryClient()
        {
            _conx.Open();
            _cmmd = _conx.CreateCommand();
            _cmmd.CommandText = "SELECT * FROM images WHERE name LIKE $name";
            _param = _cmmd.CreateParameter();
            _param.ParameterName = "$name";
            _cmmd.Parameters.Add(_param);
        }

        public SqliteDataReader GetEmotesReader(string query)
        {
            _param.Value = $"%{query}%";
            return _cmmd.ExecuteReader();
        }

        ~EmoteQueryClient()
        {
            _conx.Close();
            _conx.Dispose();
        }

    }
}