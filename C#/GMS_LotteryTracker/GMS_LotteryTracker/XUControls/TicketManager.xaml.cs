using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GMS_LotteryTracker.XUControls
{
    /// <summary>
    /// Interaction logic for TicketManager.xaml
    /// </summary>
    public partial class TicketManager : UserControl
    {

        private Func<int> completeFunction;
        private int totalTickets;
        private int ticketsSet;
        private string gameIDComp;

        private db_stick.db_stick db;

        public TicketManager(Func<int> completeFunction, string gameIDComp, int totalTickets, db_stick.db_stick db)
        {
            InitializeComponent();

            //init
            this.db = db;
            this.completeFunction = completeFunction;
            this.totalTickets = totalTickets;
            this.gameIDComp = gameIDComp;

            TicketCounterText.Content = String.Format("Tickets : %d/%d", ticketsSet, totalTickets);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            completeFunction();
        }




        private int getTotalTicketStreams() {
            string retMsg = "";

            DataTable? tbl = db.executeQuerry(
                string.Format("SELECT COUNT(*) AS TOTAL_STREAMS FROM ticketStream WHERE GAME_ID = '%s'", gameIDComp),
                ref retMsg);

            if (tbl == null)
                return 0;

            return (int)tbl.Rows[0][0];
        }


        private void ticketStreamDeleteCallback() { 
            
        }

        
        private void refreshStreamList() {

            TicketListSTack.Children.Clear();

            //get the total number of streams
            int totalStreams = getTotalTicketStreams();

            for (int i = 0; i < totalStreams; i++) {
                TicketListSTack.Children.Add(new TicketStreamBlock());
            }
        }


        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string retMsg = "";

            //add a new ticket stream
            //insert into database
            if (db.executeQuerry(
                    string.Format("INSERT INTO ticketStream (GAME_ID, STREAM_ID) values (%s, %d)", gameIDComp, getTotalTicketStreams() + 1),
                    ref retMsg
                ) == null) {
                Status.Content = "Status : Failed to insert ticket stream : E - " + retMsg;
                return;
            }
            
            refreshStreamList();
            TicketCounterText.Content = String.Format("Tickets : %d/%d", ticketsSet, totalTickets);
        }
    }
}
