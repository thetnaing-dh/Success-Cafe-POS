using System.Data.SqlClient;
using System.Data.SQLite;
using System.Configuration;

namespace SuccessCafePOS
{
    class DBConnection
    {
        public SQLiteConnection cn = new SQLiteConnection("Data Source=successcafepos.db");
    }
}
