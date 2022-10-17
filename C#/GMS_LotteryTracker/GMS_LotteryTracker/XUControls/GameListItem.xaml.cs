using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using GMS_LotteryTracker.theme_stick;
using GMS_LotteryTracker.XUControls;
using GMS_LotteryTracker.Utilities.QuickUtil.DateTimeQuickUtils;
using System.Data;

namespace GMS_LotteryTracker
{
    /// <summary>
    /// Interaction logic for GameListItem.xaml
    /// </summary>
    public partial class GameListItem : UserControl
    {
        GAME_STATE gameState;

        private Themes_Stick themes = new Themes_Stick();
        private db_stick.db_stick db;
        private long gameId;
        private NewGameGrid_UI_REF contextUiref;
        private Popup pop;

        public enum GAME_STATE {

            INACTIVE = -1,
            RESULT_DAY = 0,
            RESULT_CLOSE = 1,
            ACTIVE = 4
            
        }


        private string getStateToString(GAME_STATE state) {
            switch (state) {
                case GAME_STATE.INACTIVE:
                    return "INACTIVE";
                case GAME_STATE.ACTIVE:
                    return "ACTIVE";
                case GAME_STATE.RESULT_CLOSE:
                    return "RESULT_CLOSE";
                case GAME_STATE.RESULT_DAY:
                    return "RESULT_DAY";
                default:
                    return "INACTIVE";
            }
        }


        public LinearGradientBrush getBackGroundGradientBrush(GAME_STATE state) {
            GradientStop primaryColorStop = new GradientStop(themes.getSelectedTheme().Value.FocusColor, 0.0);
            GradientStop stateColorStop = null;
            
            if (state == GAME_STATE.ACTIVE)
                stateColorStop = new GradientStop(Colors.Blue, 0.7);
            else if (state == GAME_STATE.RESULT_CLOSE)
                stateColorStop = new GradientStop(Colors.Green, 0.7);
            else if (state == GAME_STATE.RESULT_DAY)
                stateColorStop = new GradientStop(Colors.YellowGreen, 0.7);
            else if (state == GAME_STATE.INACTIVE)
                stateColorStop = new GradientStop(Colors.Red, 0.7);

            GradientStopCollection stopColl = new GradientStopCollection();
            stopColl.Add(primaryColorStop);
            stopColl.Add(stateColorStop);

            LinearGradientBrush lgb = new LinearGradientBrush(stopColl);
            lgb.StartPoint = new Point(1, 0);
            lgb.EndPoint = new Point(1, 1);
            lgb.Opacity = 0.5;

            return lgb;
        }


        private void showPopup(Control cont) {
            pop.IsOpen = false;
            pop.Child = cont;
            pop.IsOpen = true;
        }

        private int getTicketsSold() {
            //find how many tickets are sold, to display
            string query = string.Format("SELECT COUNT(TICKET_NUMBER) as TOTAL_COUNT from tickets where GAME_ID = {0} AND IS_TICKET_SOLD = 1", gameId);
            string retMsg = "";
            DataTable dt = db.executeQuerry(query, ref retMsg);
            if (dt != null) return int.Parse(dt.Rows[0]["TOTAL_COUNT"].ToString());
            return 0;
        }

        //constructor
        public GameListItem(db_stick.db_stick db, NewGameGrid_UI_REF uiref, long gameID, string gameName, DateTime createDate, DateTime gameDate, int totalTickets)
        {
            InitializeComponent();

            //set up the item
            this.db = db;
            gameId = gameID;
            contextUiref = uiref;

            //set the popup
            //create popup
            pop = new Popup();
            pop.Name = "Popup_box";
            pop.Placement = PlacementMode.MousePoint;
            pop.HorizontalOffset = -100;
            pop.VerticalOffset = -100;
            pop.AllowsTransparency = false;
            pop.IsOpen = false;

            //add to the main grid
            MainGrid.Children.Add(pop);


            //set the back ground as per the state
            //get state
            TimeSpan intervalDays = gameDate - DateTime.Now;
            gameState = GAME_STATE.INACTIVE;


            if (DateTime.Compare(gameDate, DateTime.Now) >= 0)
            {
                if (intervalDays.Days >= (int)GAME_STATE.ACTIVE)
                    gameState = GAME_STATE.ACTIVE;

                else if (intervalDays.Days < (int)GAME_STATE.ACTIVE &&
                    intervalDays.Days >= (int)GAME_STATE.RESULT_CLOSE)
                    gameState = GAME_STATE.RESULT_CLOSE;

                else if (intervalDays.Days < (int)(GAME_STATE.RESULT_CLOSE) &&
                    intervalDays.Days >= (int)GAME_STATE.RESULT_DAY)
                    gameState = GAME_STATE.RESULT_DAY;
            }
            else {
                gameState = GAME_STATE.INACTIVE;
            }


            //get the background
            LinearGradientBrush brush = getBackGroundGradientBrush(gameState);
            SolidColorBrush textBrush = new SolidColorBrush(themes.getSelectedTheme().Value.textColor);


            //set the background
            MainGrid.Background = brush;

            //set the text color accroding to the themes
            gameNameOut.Foreground = textBrush;
            gameCreateDateOut.Foreground = textBrush;
            gameResultDayOut.Foreground = textBrush;
            statusOut.Foreground = textBrush;
            tikectsOut.Foreground = textBrush;
            gameCreateTitle.Foreground = textBrush;
            gameResultTitle.Foreground = textBrush;
            StatusTitle.Foreground = textBrush;
            TicketTitle.Foreground = textBrush;
            editGameBtn.Foreground = textBrush;
            deleteGameBtn.Foreground = textBrush;
            manageTicketBtn.Foreground = textBrush;
            managePrizeBtn.Foreground = textBrush;
            
            //set the other values of the item
            gameNameOut.Content = "Game : " + gameID + " - " + gameName;
            gameCreateDateOut.Content = DateTimeQuickUtils.getDateTimeTo12String(createDate);
            gameResultDayOut.Content = DateTimeQuickUtils.getDateTimeTo12String(gameDate);
            statusOut.Content = getStateToString(gameState);
            tikectsOut.Content = string.Format("{0}/{1}", getTicketsSold(), totalTickets);
        }


        private bool performDelete(bool confirmDelete) {
            if (confirmDelete) {
                string retMsg = "";

                //delete the game along with other tings added for the game
                //games table
                //execute query
                db.executeQuerry(string.Format("DELETE FROM games where ID = {0}", gameId), ref retMsg);
                //tickets table
                db.executeQuerry(string.Format("DELETE FROM tickets where ID = {0}", gameId), ref retMsg);

                //refresh the game list
                contextUiref.refreshGameList(db.get_All_Games(ref retMsg));
            }
            return true;
        }

        private void deleteGameBtn_Click(object sender, RoutedEventArgs e)
        {
            showPopup(new WarningPopup(
                pop, 
                string.Format("Are you sure you want to delete this Game : {0}", ((string)gameNameOut.Content)),
                performDelete
                ));
        }

        private void manageTicketBtn_Click(object sender, RoutedEventArgs e)
        {
            //showPopup(
                
            //    );
        }

        private void editGameBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
