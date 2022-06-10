using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GMS_LotteryTracker.theme_stick
{
    public class Theme_Stick
    {
        public bool useBackGroundImage = false;


        public Color primaryColor;
        public Color SecondaryColor;
        public Color FocusColor;
        public Color textColor;

        public Color highLightColor; //not in use - high light color for special purposes

        public Image backGroundImage; //not in use - Image for the background of the application


        public Theme_Stick()
        {
            primaryColor = Brushes.AntiqueWhite.Color;
            SecondaryColor = Brushes.GhostWhite.Color;
            FocusColor = Brushes.LightGray.Color;
            textColor = Brushes.Black.Color;
            highLightColor = Brushes.LimeGreen.Color;
            useBackGroundImage = false;
            backGroundImage = null;
        }

        public Theme_Stick(Color primaryColor, Color secondaryColor, Color FocusColor, Color textColor, Color highLightColor, Image backGroundImage = null) { 
            this.primaryColor =  primaryColor;
            this.SecondaryColor = secondaryColor;
            this.FocusColor = FocusColor;
            this.textColor = textColor;
            this.highLightColor = highLightColor;
            this.useBackGroundImage = (backGroundImage == null) ? false : true;
            this.backGroundImage = (backGroundImage == null) ? null : backGroundImage;
        }

    }

    public class Themes_Stick {
        public static int selectedIndex;
        public static List<KeyValuePair<string, Theme_Stick>> themes = new List<KeyValuePair<string, Theme_Stick>>();


        //adds up the default themes into the list
        private void addThemes() {
            selectedIndex = 0;

            //add all the themes here
            themes.Add(new KeyValuePair<string, Theme_Stick>("Default White", new Theme_Stick()));
            themes.Add(new KeyValuePair<string, Theme_Stick>("Dark Mode", new Theme_Stick(Brushes.Black.Color, Brushes.DarkGray.Color, Brushes.Blue.Color, Brushes.Gray.Color, Brushes.White.Color)));
            
        }
        
        public Themes_Stick() {
            //add the themes here
            addThemes();
        }

        public KeyValuePair<string, Theme_Stick> getSelectedTheme() {
            return themes[selectedIndex];
        }


        //returns the background brush for the currently selected theme
        public LinearGradientBrush getBackgroundGradientBrush() {

            Theme_Stick theme = themes[selectedIndex].Value;

            GradientStop primaryColorStop = new GradientStop(theme.primaryColor, 0.0);
            GradientStop stateColorStop = new GradientStop(theme.SecondaryColor, 0.7);

            GradientStopCollection stopColl = new GradientStopCollection();
            stopColl.Add(primaryColorStop);
            stopColl.Add(stateColorStop);

            LinearGradientBrush lgb = new LinearGradientBrush(stopColl);
            lgb.StartPoint = new Point(1, 0);
            lgb.EndPoint = new Point(1, 1);

            return lgb;
        }



        public bool selectThemeByIndex(int index) {
            if (index > themes.Count - 1) return false;
            selectedIndex = index;
            return true;
        }

        public bool selectThemeByName(string name) {
            if (name == null) return false;

            for (int i = 0; i < themes.Count; i++) {
                if (name.ToLower() == themes[i].Key.ToLower()) {
                    selectedIndex = i;
                    return true;
                }
            }
            return false;
        }
    }
}
