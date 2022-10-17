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

namespace GMS_LotteryTracker.XUControls
{
    /// <summary>
    /// Interaction logic for WarningPopup.xaml
    /// </summary>
    public partial class WarningPopup : UserControl
    {

        Func<bool, bool>? completeCallback;
        Popup thisPopup;
        theme_stick.Themes_Stick themes;


        public void refreshUITheme() { 
            //set up the back ground
            MainGrid.Background = themes.getBackgroundGradientBrush();

            //set up the controls
            SolidColorBrush btnBrush = new SolidColorBrush(themes.getSelectedTheme().Value.FocusColor);
            SolidColorBrush textBrush = new SolidColorBrush(themes.getSelectedTheme().Value.textColor);
            OK_Btn.Background = btnBrush;
            OK_Btn.Foreground = textBrush;
            Cancel_Btn.Background = btnBrush;
            Cancel_Btn.Foreground = textBrush;
            Message_Text.Foreground = textBrush;
        }


        public string devideMessage(string message) {
            string ret = "";
            int breakPoint = 25; //assuming 25 for now, for breaking the line
            if (message.Length > breakPoint) {
                for (int i = 1; i <= message.Length / breakPoint; i++)
                {
                    ret += message.Substring((i - 1) * breakPoint, breakPoint - 1) + "\n";
                }
                ret += message.Substring((message.Length / breakPoint) * breakPoint - 1,
                    (message.Length - ((message.Length / breakPoint) * breakPoint) + 1)
                    );
                return ret;
            }
            return message;
        }

        public WarningPopup(Popup thisPopup, string message, Func<bool, bool>? completeCallback)
        {
            InitializeComponent();

            themes = new theme_stick.Themes_Stick();

            this.completeCallback = completeCallback;
            this.thisPopup = thisPopup;

            Message_Text.Content = devideMessage(message);

            //setup the background
            refreshUITheme();
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (completeCallback != null)
                completeCallback(true);

            thisPopup.IsOpen = false;
            thisPopup.Child = null;
        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (completeCallback != null)
                completeCallback(false);

            thisPopup.IsOpen = false;
            thisPopup.Child = null;
        }
    }
}
