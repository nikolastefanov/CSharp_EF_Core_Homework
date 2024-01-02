using Microsoft.Data.SqlClient;
using System;

namespace _02_VillainNames
{
    public class StartUp
    {
        private const string ConnectionString =
         @"Server=.;Database=MinionDB; Integrated Security=true";
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();
            using (sqlConnection)
            {
                string getCreateDatabaseText =
                       @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount
                                 FROM Villains AS v
                                 JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                 GROUP BY v.Id, v.Name
                                 HAVING COUNT(mv.VillainId) > 3
                                  ORDER BY COUNT(mv.VillainId)";

                SqlCommand sqlCommand = new SqlCommand(getCreateDatabaseText, sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    string name=reader["Name"]?.ToString();
                    string minionsCount = reader["MinionsCount"]?.ToString();

                    Console.WriteLine($"{name} - {minionsCount}");
                }
            }
        }
    }
}
