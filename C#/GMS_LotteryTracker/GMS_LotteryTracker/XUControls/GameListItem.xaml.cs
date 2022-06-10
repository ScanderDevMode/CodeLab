using System;
using System.Collections.Generic;
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
using GMS_LotteryTracker.theme_stick;
using GMS_LotteryTracker.Utilities.QuickUtil.DateTimeQuickUtils;




namespace GMS_LotteryTracker
{
    /// <summary>
    /// Interaction logic for GameListItem.xaml
    /// </summary>
    public partial class GameListItem : UserControl
    {
        bool isSelected; //if this item is selected

        GAME_STATE gameState;

        private Themes_Stick themes = new Themes_Stick();

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
                stateColorStop = new GradientStop(Colors.YellowGreen, 0.7);
            else if (state == GAME_STATE.RESULT_DAY)
                stateColorStop = new GradientStop(Colors.Green, 0.7);
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


        //constructor
        public GameListItem(long gameID, string gameName, DateTime createDate, DateTime gameDate)
        {
            InitializeComponent();

            //set up the item
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

            //set the other values of the item
            gameNameOut.Content = "Game : " + gameID + " - " + gameName;
            gameCreateDateOut.Content = DateTimeQuickUtils.getDateTimeTo12String(createDate);
            gameResultDayOut.Content = DateTimeQuickUtils.getDateTimeTo12String(gameDate);
            statusOut.Content = getStateToString(gameState);

        }
    }
}
