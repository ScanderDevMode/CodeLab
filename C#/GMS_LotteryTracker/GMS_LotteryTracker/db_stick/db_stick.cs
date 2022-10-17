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




        public DataTable? executeQuerry(string? queryString, ref string errorMessage) {
            if (connectionString == null)
            {
                errorMessage = "Database not initialized properly.";
                return null; //check and return if connection string missing
            }

            if (queryString == null || queryString == "") {
                errorMessage = "Invalid Query String";
                return null;
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


        //private function to do roll back of an entered game
        public void do_BulkDeleteGameRecords(bool rbr_games, bool rbr_tickets, bool rbr_gamesPrizePos, bool rbr_prizeList, string gameId)
        {
            string retMsg = "";

            //for games
            if (rbr_games)
            {
                //do the roll back here for game inserted
                executeQuerry(string.Format("DELETE FROM games WHERE ID = {0}", gameId), ref retMsg);
            }

            //for tickets
            if (rbr_tickets)
            {
                //do the roll backs for the tickets
                executeQuerry(string.Format("DELETE FROM tickets WHERE GAME_ID = {0}", gameId), ref retMsg);
            }

            if (rbr_gamesPrizePos) {
                //do the roll backs for the prize
                executeQuerry(string.Format("DELETE FROM gamePrizePos WHERE GAME_ID = {0}", gameId), ref retMsg);
            }

            if (rbr_prizeList) {
                //do the roll backs for the prize list
                executeQuerry(string.Format("DELETE FROM prizeList WHERE GAME_ID = {0}", gameId), ref retMsg);
            }
        }

    }
}
