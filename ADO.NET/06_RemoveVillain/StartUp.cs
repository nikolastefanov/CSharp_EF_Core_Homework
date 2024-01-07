using System;
using System.Data.SqlClient;
using System.Text;

namespace _06_RemoveVillain
{
    public class StartUp
    {

        private const string ConnectionString =
             @"Server=.;Database=MinionDB; Integrated Security=true";

        static void Main(string[] args)
        {
            
           SqlConnection sqlConnection = new SqlConnection(ConnectionString) ;
            using(sqlConnection)
            {
                sqlConnection.Open();

                int villainId = int.Parse(Console.ReadLine());

                string result = RemoveVillainById(sqlConnection, villainId);
            }
        }

        private static string RemoveVillainById(
                        SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();
            using(SqlTransaction sqlTransaction=
                            sqlConnection.BeginTransaction())
            {
                string getVillainNameQueryText =
                    @"SELECT [Name] FROM Villains
                        WHERE Id=@villainId";

                using (SqlCommand getVillainNameCmd = new SqlCommand(
                      getVillainNameQueryText, sqlConnection))
                {
                    getVillainNameCmd.Transaction = sqlTransaction;

                    string villainName = getVillainNameCmd.ExecuteScalar()?.ToString();

                    if (villainName==null)
                    {
                        sb.AppendLine($"No such villain was found");
                    }
                    else
                    {
                        try
                        {
                            string releaseMinionsQueryText =
                                @"DELETE FROM MinionsVillains
                                  WHERE VilainId=@vilainId";

                            using SqlCommand releaseMinionsCmd =
                                    new SqlCommand(
                                        releaseMinionsQueryText, sqlConnection);

                            releaseMinionsCmd.Parameters
                                    .AddWithValue("@villainId", villainId);

                            releaseMinionsCmd.Transaction = sqlTransaction;

                            int releaseMinionsCount =
                                    releaseMinionsCmd.ExecuteNonQuery();
                            string deleteVillainQueryText =
                                @"DELETE FROM Villains
                                WHERE Id=@VillainId";

                            using SqlCommand deleteVillainCmd =
                                new SqlCommand(deleteVillainQueryText, sqlConnection);

                            deleteVillainCmd.Parameters
                                .AddWithValue("@villainId", villainId);

                            deleteVillainCmd.Transaction = sqlTransaction;

                            deleteVillainCmd.ExecuteNonQuery();

                            sqlTransaction.Commit();

                            sb.AppendLine($"{villainName} was deleted.");
                            sb.AppendLine($"{releaseMinionsCount} minions were released.");

                        }
                        catch (Exception ex)
                        {

                            sb.AppendLine(ex.Message);
                            try
                            {
                                sqlTransaction.Rollback();
                            }
                            catch (Exception rollbackEx)
                            {

                                sb.AppendLine(rollbackEx.Message);
                            }
                        }
                    }

                    return sb.ToString().TrimEnd();
                }
            }

          
        }
    }
}
