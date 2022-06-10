using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;



namespace GMS_LotteryTracker.db_stick
{
    public class db_stick
    {
        string connectionString;


        public db_stick(string connectionString) {
            this.connectionString = connectionString;
        }




        public DataTable? executeQuerry(string queryString, ref string errorMessage) {
            if (connectionString == null)
            {
                errorMessage = "Invalid QueryString";
                return null; //check and return if connection string missing
            }
            DataTable output = new DataTable(); //create the output table

            try
            {
                //open connection and execute the query
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection))
                {
                    //execute the query and store into the data table                
                    adapter.Fill(output); //execute the querry and fill the output table with the results
                }
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                return null;
            }

            return output;
        }


        //api to get all the games in a list
        public DataTable? get_All_Games(ref string retMsg) {
            //create and execute the querry
            return executeQuerry("SELECT * FROM games", ref retMsg);
        }

    }
}
