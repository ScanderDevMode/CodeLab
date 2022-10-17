using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMS_LotteryTracker.Utilities.QuickUtil.DateTimeQuickUtils;
using GMS_LotteryTracker.XUControls;


namespace GMS_LotteryTracker
{
    /// <summary>
    /// Interaction logic for AddGamePopUp.xaml
    /// </summary>
    public partial class AddGamePopUp : UserControl
    {
        private enum StatusState {
            RED = 0,
            GREEN,
            YELLOW,
            WHITE
        }


        private theme_stick.Themes_Stick thm;
        private db_stick.db_stick db;
        private Popup thisPopup;

        private Func<bool, bool>? completeFunction;

        //for the game id to be created early
        private string gameIDComp;
        private DateTime gameCreateDate;

        private void setupThemeColor() {
            SolidColorBrush textBrush = new SolidColorBrush(thm.getSelectedTheme().Value.textColor);

            SolidColorBrush focusBrush = new SolidColorBrush(thm.getSelectedTheme().Value.FocusColor);

            //set the background
            MainGrid.Background = thm.getBackgroundGradientBrush();

            //set the other controls
            WindowTitleText.Foreground = textBrush;
            CloseBtn.Foreground = textBrush;
            CloseBtn.Background = focusBrush;

            GameTitleText.Foreground = textBrush;
            GameTitleIn.Foreground = textBrush;
            GameTitleIn.Background = focusBrush;

            ResultTimeText.Foreground = textBrush;
            PickDate.Foreground = textBrush;
            PickDate.Background = focusBrush;

            TimeHHIn.Foreground = textBrush;
            TimeHHIn.Background = focusBrush;
            TimeHHText.Foreground = textBrush;
            TimeMMIn.Foreground = textBrush;
            TimeMMIn.Background = focusBrush;
            TimeMMText.Foreground = textBrush;
            TimeSSIn.Foreground = textBrush;
            TimeSSIn.Background = focusBrush;
            TimeSSText.Foreground = textBrush;

            RadioAMBtn.Foreground = textBrush;
            RadioAMBtn.Background = focusBrush;
            RadioPMBtn.Foreground = textBrush;
            RadioPMBtn.Background = focusBrush;

            TicketText.Foreground = textBrush;
            TotalTicketIn.Foreground = textBrush;
            TotalTicketIn.Background = focusBrush;
            TicketSeriesText.Foreground = textBrush;

            SetTicketStreamBtn.Foreground = textBrush;
            SetTicketStreamBtn.Background = focusBrush;

            TicketBoughtAtIn.Foreground = textBrush;
            TicketBoughtAtIn.Background = focusBrush;
            TicketBoughtAtText.Foreground = textBrush;

            PrizeCountText.Foreground = textBrush;
            PrizeCountOut.Foreground = textBrush;
            PrizeSlider.Foreground = textBrush;

            StatusOut.Foreground = textBrush;
            StatusText.Foreground = textBrush;
            InsertBtn.Foreground = textBrush;
            InsertBtn.Background = focusBrush;

            for (int i = 1; i <= 15; i++) {
                TextBox prizeCountIn = (TextBox)PrizeCountStackPanel.FindName("PrizeCount" + i + "IN");
                Label prizeCountText = (Label)PrizeCountStackPanel.FindName("PrizeCount" + i + "Text");

                prizeCountIn.Background = focusBrush;
                prizeCountIn.Foreground = textBrush;
                prizeCountText.Foreground = textBrush;
            }

            GameDescIn.Background = focusBrush;
            GameDescIn.Foreground = textBrush;
            GameDescText.Foreground = textBrush;
        }


        private void disablePrizeInputs() {
            PrizeCount1Box.IsEnabled = false;
            PrizeCount2Box.IsEnabled = false;
            PrizeCount3Box.IsEnabled = false;
            PrizeCount4Box.IsEnabled = false;
            PrizeCount5Box.IsEnabled = false;
            PrizeCount6Box.IsEnabled = false;
            PrizeCount7Box.IsEnabled = false;
            PrizeCount8Box.IsEnabled = false;
            PrizeCount9Box.IsEnabled = false;
            PrizeCount10Box.IsEnabled = false;
            PrizeCount11Box.IsEnabled = false;
            PrizeCount12Box.IsEnabled = false;
            PrizeCount13Box.IsEnabled = false;
            PrizeCount14Box.IsEnabled = false;
            PrizeCount15Box.IsEnabled = false;
        }

        public void clearInputData() {
            //clear out all the fields
            GameTitleIn.Text = "";
            PickDate.SelectedDate = null;
            TimeHHIn.Text = "";
            TimeMMIn.Text = "";
            TimeSSIn.Text = "";
            RadioAMBtn.IsChecked = false;
            RadioPMBtn.IsChecked = false;
            TotalTicketIn.Text = "";
            SetTicketStreamBtn.Content = "0/0";
            TicketBoughtAtIn.Text = "";
            GameDescIn.Text = "";

            //clear out the status as well
            //setStatus("", StatusState.WHITE); //leave the status out, for user to view last action

            //for prize counters, this should trigger the slider action, which should take care of the disabling
            //we just clear the values later  on
            double prizeCount = PrizeSlider.Value;
            PrizeSlider.Value = 0;

            //clear the inputs in the prize count
            for (int i = 1; i <= prizeCount; i++) {
                ((TextBox)PrizeCountStackPanel.FindName("PrizeCount" + i + "IN")).Text = "";
            }
        }


        //Contructor
        public AddGamePopUp(Popup thisPopup, db_stick.db_stick db, Func<bool, bool>? completeCallBack)
        {
            InitializeComponent();

            //initialize sticks
            thm = new theme_stick.Themes_Stick();
            this.db = db;

            //refference to this popup, send by creator
            this.thisPopup = thisPopup;

            //set up the popup according to the theme
            setupThemeColor();

            //set the prize inputs to disabled
            disablePrizeInputs();

            //store the callback function
            this.completeFunction = completeCallBack;

            //setup the timmings for the game id to be generated
            //get the game creation date
            gameCreateDate = DateTime.Now;

            //create the unique game id, usign date :-)
             gameIDComp = String.Format("{0}{1}{2}{3}{4}{5}",
                gameCreateDate.Date.Day,
                gameCreateDate.Date.Month,
                gameCreateDate.Date.Year,
                gameCreateDate.TimeOfDay.Hours,
                gameCreateDate.TimeOfDay.Minutes,
                gameCreateDate.TimeOfDay.Seconds
                );
        }

        //Deprecated, for self closure, closing of the popups can be now done by the uirefs
        public void closePopup(Object sender, RoutedEventArgs args) {
            thisPopup.IsOpen = false;
            thisPopup.Child = null;
            //call complete call back
            if (completeFunction != null)
                completeFunction(true);
        }

        private void PrizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int currentPrizeCount = (int)PrizeSlider.Value; //get the value from the slider

            //according to that value set the prize pool boxes on or off
            for (int i = 1; i <= 15; i++) {

                string boxName = "PrizeCount" + i + "Box"; //box name
                object prizeBox = PrizeCountStackPanel.FindName(boxName);//below current prize count, switch box on

                //prizebox null, so move to the next one
                if (prizeBox == null)
                    continue;

                if (i <= currentPrizeCount)
                {
                    ((StackPanel)prizeBox).IsEnabled = true;
                }
                else {
                    ((StackPanel)prizeBox).IsEnabled = false;
                }
            }
        }

        private void setStatus(string message, StatusState ss) {
            switch (ss)
            {
                case StatusState.RED: {
                        StatusOut.Foreground = Brushes.Red;
                        StatusOut.Background = Brushes.DarkGray;
                    } break;

                case StatusState.GREEN: {
                        StatusOut.Foreground = Brushes.Green;
                        StatusOut.Background = Brushes.DarkGray;
                    } break;

                case StatusState.YELLOW: {
                        StatusOut.Foreground = Brushes.Yellow;
                        StatusOut.Background = Brushes.DarkGray;
                    }
                    break;

                default: {
                        StatusOut.Foreground = Brushes.White;
                        StatusOut.Background = Brushes.DarkGray;
                    }; break;
            }

            StatusOut.Content = message;
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private bool checkUserInputForInsert() {

            //check for the basic inputs
            if (GameTitleIn.Text == null || GameTitleIn.Text == "" || GameTitleIn.Text.Length == 0) {
                setStatus("Please enter Game Title", StatusState.YELLOW);
                return false;
            }

            if (!PickDate.SelectedDate.HasValue) {
                setStatus("Please enter Game Result Date.", StatusState.YELLOW);
                return false;
            }

            if (TimeHHIn.Text == null || TimeHHIn.Text.Length == 0 || TimeHHIn.Text == "" ||
                TimeMMIn.Text == null || TimeMMIn.Text.Length == 0 || TimeMMIn.Text == "" ||
                TimeSSIn.Text == null || TimeSSIn.Text.Length == 0 || TimeSSIn.Text == ""
                )
            {
                setStatus("Please enter Game Result Time.", StatusState.YELLOW);
                return false;
            }
            else {
                if (!IsDigitsOnly(TimeHHIn.Text) ||
                    !IsDigitsOnly(TimeMMIn.Text) ||
                    !IsDigitsOnly(TimeSSIn.Text)) {
                    setStatus("Please enter the time in digits only", StatusState.YELLOW);
                    return false;
                }

                if (int.Parse(TimeHHIn.Text) > 12 || int.Parse(TimeHHIn.Text) < 01 ||
                    int.Parse(TimeMMIn.Text) > 60 || int.Parse(TimeMMIn.Text) < 00 ||
                    int.Parse(TimeSSIn.Text) > 60 || int.Parse(TimeSSIn.Text) < 00) {
                    setStatus("Please enter the time in 12 Hour Format [HH = 01 - 12, MM/SS = 00 - 60].", StatusState.YELLOW);
                    return false;
                }
            }

            if (RadioAMBtn.IsChecked == false && RadioPMBtn.IsChecked == false) {
                setStatus("Please enter the time of the day [AM/PM].", StatusState.YELLOW);
                return false;
            }


            if (TotalTicketIn.Text == null || TotalTicketIn.Text.Length == 0 || TotalTicketIn.Text == "") {
                setStatus("Please enter the Total Ticket Count.", StatusState.YELLOW);
                return false;
            }
            else {
                if (!IsDigitsOnly(TotalTicketIn.Text)) {
                    setStatus("Total Ticket Count should be in digits only.", StatusState.YELLOW);
                    return false;
                }
            }

            //if (TicketSeriesIn.Text == null || TicketSeriesIn.Text.Length == 0 || TicketSeriesIn.Text == "") {
            //    setStatus("Please enter the Ticket Stream.", StatusState.YELLOW);
            //    return false;
            //}
            //else {
            //    if (!IsDigitsOnly(TicketSeriesIn.Text))
            //    {
            //        setStatus("Ticket Stream should be in digits only.", StatusState.YELLOW);
            //        return false;
            //    }
            //}

            if (TicketBoughtAtIn.Text == null || TicketBoughtAtIn.Text.Length == 0 || TicketBoughtAtIn.Text == "")
            {
                setStatus("Please enter the Ticket Bought At.", StatusState.YELLOW);
                return false;
            }
            else
            {
                if (!IsDigitsOnly(TicketBoughtAtIn.Text))
                {
                    setStatus("Ticket Bought At should be in digits only.", StatusState.YELLOW);
                    return false;
                }
            }


            if (PrizeSlider.Value == 0) {
                setStatus("Please enter the Prize Count.", StatusState.YELLOW);
                return false;
            }

            for (int i = 1; i <= PrizeSlider.Value; i++) {
                TextBox inBox = (TextBox)PrizeCountStackPanel.FindName("PrizeCount" + i + "IN");

                if (inBox == null || inBox.Text.Length == 0 || inBox.Text == "") {
                    setStatus("Please enter the " + i + " Prize Count.", StatusState.YELLOW);
                    return false;
                }
                else {
                    if (!IsDigitsOnly(inBox.Text)) {
                        setStatus("Please enter the " + i + " Prize Count in digits only.", StatusState.YELLOW);
                        return false;
                    }
                }
            }

            return true;
        }


        //private function to insert the tickets
        private List<string> makeTicketInsertQueries(string gameId, int totalTickets, int ticketStream, int pricePerTicket) {
            int breakPoint = 5;
            List<string> queries = new List<string>();

            //check
            if (gameId == "" || ticketStream == 0 || pricePerTicket == 00)
                return queries;

            //insert the values for every ticket
            for (int j = 1; j <= totalTickets;)
            {
                //prepare the query
                string querry = "insert into tickets (GAME_ID, TICKET_NUMBER, TICKET_PRICE) values ";
                int i = j;
                j = j + (((totalTickets - j) > breakPoint) ? breakPoint : (totalTickets - j) + 1);
                for (; i < j; i++)
                {
                    querry += String.Format("({0}, {1}, {2})" + ((i == j - 1) ? ";" : ", "),
                            gameId, ticketStream + (i - 1), pricePerTicket
                        );
                }
                //add to the list
                queries.Add(querry);
            }

            return queries;
        }


        private string? makePrizeCountInsertQuery(string gameId) {
            if (gameId == null || gameId == "") {
                return null;
            }

            string query = "INSERT INTO gamePrizePos (GAME_ID, PRIZE_POS, TOTAL_COUNT) VALUES ";
            if ((int)PrizeSlider.Value == 0)
            {
                return null;
            }


            for (int i = 0; i < PrizeSlider.Value; i++) {
                query += String.Format("({0}, {1}, {2})" + ((i == PrizeSlider.Value - 1) ? ";" : ", "),
                        gameId, i + 1, ((TextBox)PrizeCountStackPanel.FindName("PrizeCount" + (i + 1) + "IN")).Text
                    );
            }

            return query;
        }

        
        private bool insertPrizeDetails(string gameId, ref string retMsg) {
            //checks
            if (gameId != null || gameId == "")
            {
                retMsg = "Invalid Parameters";
                return false;
            }

            //make the query for prize positions
            string? query = makePrizeCountInsertQuery(gameId);
            if (query != null)
            {
                retMsg = "Invalid Parameters";
                return false;
            }

            //execute db query
            if (db.executeQuerry(query, ref retMsg) == null) {
                //error
                retMsg = "DB query failed : " + retMsg;
                return false;
            }
            
            return true;
        }


        


        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            //locals
            //for rollback required, if needed to backout
            bool rbr_games = false;
            bool rbr_tickets = false;

            
            //insert button clicked

            //check for all the inputs from the user
            if (!checkUserInputForInsert()) {
                //call complete call back
                if (completeFunction != null)
                    completeFunction(false);
                return;
            }

            

            //calculate the game principal
            int gamePrincipal = int.Parse(TotalTicketIn.Text) * int.Parse(TicketBoughtAtIn.Text);

            //fix hour of the day before making the querry [convert to 24 hrs for sql] //Deprecated
            int hr =  DateTimeQuickUtils.convertHourTo24(int.Parse(TimeHHIn.Text), (RadioPMBtn.IsChecked == true) ? true : false);
            

            //prepare the datetime object for use            
            DateTime gameResultDate = new DateTime(
                PickDate.SelectedDate.Value.Year,
                PickDate.SelectedDate.Value.Month,
                PickDate.SelectedDate.Value.Day,
                hr,
                int.Parse(TimeMMIn.Text),
                int.Parse(TimeSSIn.Text)
                );

            //Game Table
            //prepare the querry string
            string? querry = string.Format("insert into games (GAME_DATE, CREATE_DATE, GAME_PRINCIPAL, GAME_DESCRIPTION, GAME_NAME, ID, GAME_PRIZE_COUNT, GAME_TICKET_COUNT, TICKET_BOUGHT_AT) values ("
                + "{0}, "
                + "{1}, "
                + "{2}, "
                + "'{3}', "
                + "'{4}', "
                + "{5}, "
                + "{6}, "
                + "{7}, "
                + "{8}, "
                + "{9})",
                DateTimeQuickUtils.makeSqlDateStatement(gameResultDate),
                DateTimeQuickUtils.makeSqlDateStatement(gameCreateDate),
                gamePrincipal,
                GameDescIn.Text,
                GameTitleIn.Text,
                gameIDComp,
                PrizeSlider.Value,
                int.Parse(TotalTicketIn.Text),
                int.Parse(TicketBoughtAtIn.Text)
                );
            //execute the querry
            string retMsg = "";
            if (db.executeQuerry(querry, ref retMsg) == null) {
                setStatus(String.Format("DB Query Failed : {0}", retMsg), StatusState.RED);
                //call complete call back
                if (completeFunction != null)
                    completeFunction(false);
                return;
            }
            rbr_games = true; //set roll back required


            //insert the tickets //is done in the ticket manager now
            //List<string> queries = makeTicketInsertQueries(
            //    idComp,
            //    int.Parse(TotalTicketIn.Text),
            //    int.Parse(TicketSeriesIn.Text),
            //    int.Parse(TicketBoughtAtIn.Text)
            //    );

            //if (queries.Count == 0) {
            //    setStatus("Make Query Statement failed.", StatusState.RED);

            //    //roll back
            //    db.do_BulkDeleteGameRecords(rbr_games, rbr_tickets, false, false, idComp);

            //    //call complete call back
            //    if (completeFunction != null)
            //        completeFunction(false);
            //    return;
            //}

            ////iterate for each of the queries
            //foreach (string qry in queries) {
            //    if (db.executeQuerry(qry, ref retMsg) == null)
            //    {
            //        setStatus(String.Format("DB Query Failed : {0}", retMsg), StatusState.RED);

            //        //roll back
            //        db.do_BulkDeleteGameRecords(rbr_games, rbr_tickets, false, false, idComp);

            //        //call complete call back
            //        if (completeFunction != null)
            //            completeFunction(false);
            //        return;
            //    }
            //    rbr_tickets = true; //first time it will not execute
            //}
            

            //setup tickets

            



            //insert the prize details
            if (insertPrizeDetails(gameIDComp, ref retMsg)) {
                //error
                setStatus(String.Format("DB Query Failed : {0}", retMsg), StatusState.RED);

                //roll back
                db.do_BulkDeleteGameRecords(rbr_games, rbr_tickets, false, false, gameIDComp);

                //call complete call back
                if (completeFunction != null)
                    completeFunction(false);
                return;
            }

            //if successful then it comes here
            //house keeping activities - 

            //close the popup, if insertion was a success
            setStatus("Game Inserted.", StatusState.GREEN); //set the status before closing
            //closePopup(null, null); //close the popUp with delay for the message to be viewed

            //refresh the game list after adding the 
            clearInputData(); //stays, its better this way, instead of making
            //the user click the Add New Game button after every successful entries
            //also gives the time to see the status

            //call complete call back
            if(completeFunction != null)
                completeFunction(true);
        }


        
        private void TotalTicketIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            //text value changed, check for 
            if (TotalTicketIn.Text.Length < 0)
                return;

            if (!IsDigitsOnly(TotalTicketIn.Text)) {
                TotalTicketIn.Text = "";
                return;
            }

            //text value changed, update the stream button text
            SetTicketStreamBtn.Content = "0/" + ((TotalTicketIn.Text.Length > 0) ? TotalTicketIn.Text : "0");

        }



        private int closeTicketManagerPopup() {
            PopupX.Child = null;
            PopupX.IsOpen = false;
            SetTicketStreamBtn.IsEnabled = true;
            return 0;
        }

        private void SetTicketStreamBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupX.Child = new TicketManager(closeTicketManagerPopup, gameIDComp, int.Parse(TotalTicketIn.Text), db);
            PopupX.IsOpen = true;
            PopupX.MinHeight = 650;
            PopupX.MinWidth = 800;
            SetTicketStreamBtn.IsEnabled = false;
        }
    }
}
