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
        NO_TYPE,
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


    public class NewGameGrid_UI_REF : UI_REF {


        public NewGameGrid_UI_REF() {
            this.uiType = UI_TYPE.NEW_GAME_GRID;
        }
    }


    public class SearchGrid_UI_REF : UI_REF {


        public SearchGrid_UI_REF() {
            this.uiType = UI_TYPE.SEARCH_GRID;
        }
    }




    public class DebtGrid_UI_REF : UI_REF {


        public DebtGrid_UI_REF() {
            this.uiType = UI_TYPE.DEBT_GRID;
        }
    }


    public class NewWindowGrid_UI_REF : UI_REF {

        public UI_TYPE subUIType;

        //New Game Grid
        public Grid newGameGrid;
        public NewGameGrid_UI_REF newGame_uiref;

        //Search Grid
        public Grid searchGrid;
        public SearchGrid_UI_REF searchGrid_uiref;

        //debt Grid
        public Grid debtGrid;
        public DebtGrid_UI_REF debtGrid_uiref;


        public NewWindowGrid_UI_REF() {
            this.uiType = UI_TYPE.NEW_TAB_GRID;
            subUIType = UI_TYPE.NO_TYPE;
        }

    }



    //factory class for UI Grids
    public class Grid_Factory{

        private List<UI_REF> uirefs;

        //Defines
        public const int ROW_MANAGER_GRID       = 3; //NOT-IN-USE Right Now
        public const int COL_MANAGER_GRID       = 3; //NOT-IN-USE Right Now
        public const int ROW_NEW_WINDOW_GRID    = 10;
        public const int COL_NEW_WINDOW_GRID    = 10;


        public Grid_Factory(List<UI_REF> uirefs)
        {
            this.uirefs = uirefs;
        }



        public Grid cerateManagerGrid(string initialStatusText) {
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


        public Grid createNewGameGrid(int windowCount, List<UI_REF> uiref) {
            Grid newGameGrid = new Grid();
            NewGameGrid_UI_REF newGameGrid_UI_REF = new NewGameGrid_UI_REF();






            //add to the uiref
            ((NewWindowGrid_UI_REF)uiref[windowCount]).newGameGrid = newGameGrid;
            ((NewWindowGrid_UI_REF)uiref[windowCount]).newGame_uiref = newGameGrid_UI_REF;

            return newGameGrid;
        }


        private int getListIndex(string name) {
            String index = name.Substring(name.IndexOf('_') + 1, name.Length - (name.IndexOf('_') + 1));
            return int.Parse(index);
        }


        public void newGameBtn_Click(object sender, RoutedEventArgs e) {
            int listIndex = getListIndex(((Button)sender).Name);





        }

        public void searchBtn_Click(object sender, RoutedEventArgs e) { 
            
        }

        public void debtBtn_Click(object sender, RoutedEventArgs e) { 
        
        }



        public Grid createNewWindowGrid() {
            Grid newWindowGrid = new Grid();
            NewWindowGrid_UI_REF uiref = new NewWindowGrid_UI_REF();

            int windowCount = uirefs.Count;

            //DEBUG ONLY
            newWindowGrid.ShowGridLines = true;


            //create the rows and cols
            for (int i = 0; i < ROW_NEW_WINDOW_GRID; i++)
            {
                RowDefinition row = new RowDefinition();
                newWindowGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < COL_NEW_WINDOW_GRID; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                newWindowGrid.ColumnDefinitions.Add(col);
            }

            //create the uis----------------------------------------------

            //stack Panel for the buttons
            StackPanel stackPanel = new StackPanel();
            
            //New Game Button
            Button newGameBtn = new Button();
            newGameBtn.Name = "newGameBtn_" + windowCount;
            newGameBtn.Content = "Game Box";
            newGameBtn.Click += new RoutedEventHandler(newGameBtn_Click);
            newGameBtn.Height = 20;
            newGameBtn.Width = 200;
            newGameBtn.Margin = new Thickness(2);

            //Search Window Button
            Button searchButton = new Button();
            searchButton.Name = "searchBtn_" + windowCount;
            searchButton.Content = "Deep Search";
            searchButton.Click += new RoutedEventHandler(searchBtn_Click);
            searchButton.Height = 20;
            searchButton.Width = 200;
            searchButton.Margin = new Thickness(2);


            //Debt Button
            Button debtBtn = new Button();
            debtBtn.Name = "debtBtn_" + windowCount;
            debtBtn.Content = "Debt Lookup";
            debtBtn.Click += new RoutedEventHandler(debtBtn_Click);
            debtBtn.Height = 20;
            debtBtn.Width = 200;
            debtBtn.Margin = new Thickness(2);


            //add the buttons to the stack panel
            stackPanel.Children.Add(newGameBtn);
            stackPanel.Children.Add(searchButton);
            stackPanel.Children.Add(debtBtn);
            Grid.SetColumnSpan(stackPanel, 2);
            Grid.SetRow(stackPanel, 1);
            Grid.SetColumn(stackPanel, 0);


            //add all to the uirefs
            //ui refs for the new game
            uirefs.Add(uiref);

            //add the controls to the grid
            newWindowGrid.Children.Add(stackPanel);

            //make the grid visible
            newWindowGrid.Visibility = Visibility.Visible;

            return newWindowGrid;
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
        private Grid_Factory gridFactory;



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
            
            //content of the manager tab
            mainTab.Content = gridFactory.cerateManagerGrid("Welcome... \nGMS_LOTTERY V 0.1");
            mainTab.Visibility = Visibility.Visible;

            //add to the tab list and ui ref list
            tabs.Add(mainTab);


            //create the + button for the tabs
            TabItem addTab = new TabItem();
            addTab.Name = "addTab";
            addTab.Header = "+";
            addTab.Visibility = Visibility.Visible;

            //add the second tab to the tabs list, to get included
            tabs.Add(addTab);


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
            gridFactory = new Grid_Factory(uirefs);

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
        private void TabItem_MouseLeftButtonDown(object sender, SelectionChangedEventArgs e)
        {

            if (MainControl.SelectedIndex != tabs.Count - 1)
                return;

            //clear the data binding
            MainControl.DataContext = null;


            //create a new tab
            TabItem newTab = new TabItem();
            newTab.Name = "tab" + (tabs.Count - 1);
            newTab.Header = "NewTab " + (tabs.Count - 1);
            newTab.Content = gridFactory.createNewWindowGrid();
            newTab.Visibility = Visibility.Visible;


            //add it right before the sender
            tabs.Insert(tabs.Count - 1, newTab);

            //change the selection to be the last added one
            MainControl.DataContext = tabs;
            MainControl.Visibility = Visibility.Visible;
            MainControl.SelectedIndex = tabs.Count - 2;
            
        }
        
    }
}
