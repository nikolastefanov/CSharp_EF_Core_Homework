using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace _07_PrintAllMinionNames
{
    public class StartUp
    {
        private const string ConnectionString =
         @"Server=.;Database=MinionDB; Integrated Security=true";
        static void Main(string[] args)
        {
            List <string> list= new List<string>();

            using SqlConnection sqlConnection =
                 new SqlConnection(ConnectionString);

            sqlConnection.Open();


            string getMinionsText =
                @"SELECT [Name] FROM Minions";
            using SqlCommand getMinionsCmd =
                new SqlCommand(getMinionsText, sqlConnection);

           
           using SqlDataReader reader = getMinionsCmd.ExecuteReader();

            while (reader.Read())
            {
                string minionName = reader["Name"]?.ToString();

                list.Add(minionName);

            }
           
            for(int i = 0; i < list.Count/2; i++)
            {
                Console.WriteLine(list[i]);
                Console.WriteLine(list[list.Count-i-1]);
            }

            if (list.Count%2==0)
            {
                Console.WriteLine(list[list.Count/2+1]);
            }
        }  
    }
}
