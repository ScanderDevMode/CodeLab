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




namespace GMS_LotteryTracker
{
    /// <summary>
    /// Interaction logic for GameListItem.xaml
    /// </summary>
    public partial class GameListItem : UserControl
    {
        bool isSelected; //if this item is selected

        private theme_stick.Themes_Stick themes = new Themes_Stick();




        public enum GAME_STATE { 
            ACTIVE = 0,
            RESULT_CLOSE = 1,
            RESULT_DAY = 2,
            INACTIVE = 3
        }



        public LinearGradientBrush getBackGroundGradientBrush(GAME_STATE state) {
            GradientStop primaryColorStop = new GradientStop(themes.getSelectedTheme().Value.primaryColor, 0.0);
            GradientStop stateColorStop = null;
            
            if (state == GAME_STATE.ACTIVE)
                stateColorStop = new GradientStop(Colors.AliceBlue, 0.7);
            else if (state == GAME_STATE.RESULT_CLOSE)
                stateColorStop = new GradientStop(Colors.YellowGreen, 0.7);
            else if (state == GAME_STATE.RESULT_DAY)
                stateColorStop = new GradientStop(Colors.Green, 0.7);
            else if (state == GAME_STATE.INACTIVE)
                stateColorStop = new GradientStop(themes.getSelectedTheme().Value.primaryColor, 0.7);

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
        public GameListItem(string gameName, Date createDate, Date gameDate)
        {
            InitializeComponent();



        }
    }
}
