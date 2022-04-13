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
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GMS_LotteryTracker
{

    public enum UI_TYPE {
        MANAGER_GRID,
        NEW_TAB_GRID,
        NEW_GAME_GRID,
        SEARCH_GRID,
        DEBT_GRID
    }


    //base UI Ref class
    public class UI_REF {
        protected UI_TYPE uiType;

        public UI_TYPE getUIType() {
            return uiType;
        }
    }

    //ref class for UI Manager Grid
    public class ManagerGrid_UI_REF : UI_REF {
        public TextBlock statusText;

        public ManagerGrid_UI_REF() {
            this.uiType = UI_TYPE.MANAGER_GRID;
        }
    }



    //factory class for UI Grids
    public class Grid_Factory{

        //Defines
        public const int ROW_MANAGER_GRID = 3; //NOT-IN-USE Right Now
        public const int COL_MANAGER_GRID = 3; //NOT-IN-USE Right Now





        public static Grid cerateManagerGrid(string initialStatusText, List<UI_REF> uirefs) {
            Grid managerGrid = new Grid();
            ManagerGrid_UI_REF uiref = new ManagerGrid_UI_REF();

            //for now
            managerGrid.Background = Brushes.Beige;


            //we may need to calculate row and col size here
            

            //clear the grid
            managerGrid.Children.Clear();


            //create the rows and cols
            for (int i = 0; i < ROW_MANAGER_GRID; i++)
            {
                RowDefinition row = new RowDefinition();
                managerGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < COL_MANAGER_GRID; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                
                managerGrid.ColumnDefinitions.Add(col);
            }


            //create the status text block
            TextBlock statusText = new TextBlock();
            uiref.statusText = statusText;
            statusText.Text = initialStatusText;
            //middle cell
            Grid.SetRow(statusText, 1);
            Grid.SetColumn(statusText, 1);
                                 
            //add the ui reference to the list
            uirefs.Add(uiref);

            //add the controls to the Grid
            managerGrid.Children.Add(statusText);

            return managerGrid;
        }
    
    }











    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool systemActivated = false;
        private string keyFIleRoot = "./biglock.ls";


        //class scope control items
        private List<TabItem> tabs;
        private List<UI_REF> uirefs;
        private TabItem mainTab;



        //function to update the status of the Manager Tab
        private void updateManagerTabStatus(string status) {
            Thread.Sleep(750); //wait a bit for the user to read the message
            ((ManagerGrid_UI_REF)uirefs[0]).statusText.Text = status;
        }



        private void initMainControl() { 
            
            //create the first tab [Manager Tab]
            mainTab = new TabItem();
            mainTab.Name = $"Tab1";
            mainTab.Header = $"1. Manager";

            //status text block
            mainTab.Content = Grid_Factory.cerateManagerGrid("Welcome... \nGMS_LOTTERY V 0.1", uirefs);
            mainTab.Visibility = Visibility.Visible;


            //add to the tab list and ui ref list
            tabs.Add(mainTab);
            

            //add the tab list as context to main grid
            MainControl.DataContext = tabs;
            MainControl.Visibility = Visibility.Visible;
            MainControl.SelectedIndex = 0;
        }


        //function to set the window size to fit the screen with padding
        private void setWindowToScreenFit(double screenX, double screenY) {
            int windowXPadding = 200;
            int windowYPadding = 100;
            Application.Current.MainWindow.Height = screenY - (windowYPadding * 2);
            Application.Current.MainWindow.Width = screenX - (windowXPadding * 2);
        }



        private void initializeSystem() {
            //init locals
            tabs = new List<TabItem>(); //init the tab item for the first time
            uirefs = new List<UI_REF>(); //inii the list for ui refs

            //init the main Control
            initMainControl();

            //set the window size according to the screen size
            updateManagerTabStatus("Please Wait... \nSetting the Window Size...");
            setWindowToScreenFit(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            //check and read key
            

            //fetch date from the key


            //activate, connect to db and read data if date valid

        }

        
        public MainWindow()
        {
            InitializeComponent();


            //initialize the system
            initializeSystem();

            //update status
            updateManagerTabStatus("Welcome... \nGMS_LotteryTracker V 0.1");
        }




        //private function to create a new tab
        private void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //create a new tab
            
            
            //add it right before the sender
            
            
            //change the content of the current last tab to a new tab header
            

            //add another tab in the tab control and make it 
        }



        
    }
}
