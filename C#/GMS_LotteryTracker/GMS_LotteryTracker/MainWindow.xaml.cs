using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
using GMS_LotteryTracker.db_stick;
using System.Windows.Controls.Primitives;

namespace GMS_LotteryTracker
{
    //enum for the Main UI types
    public enum UI_TYPE {
        NO_TYPE,
        MANAGER_GRID,
        NEW_TAB_GRID,
        NEW_GAME_GRID,
        SEARCH_GRID,
        DEBT_GRID
    }

    //base UI Ref class
    public abstract class UI_REF {
        protected UI_TYPE uiType;

        public UI_TYPE getUIType() {
            return uiType;
        }

        public abstract bool refreshUITheme();
        public abstract bool refreshUIElements();

        //function to clear all the inputs from the available input fields
        public abstract bool clearAllInputs();
    }

    //ref class for UI Manager Grid
    public class ManagerGrid_UI_REF : UI_REF{
        public TextBlock statusText = null;
        public Grid managerGrid = null;

        private theme_stick.Themes_Stick themes;

        public ManagerGrid_UI_REF() {
            uiType = UI_TYPE.MANAGER_GRID;
            themes = new theme_stick.Themes_Stick();
        }

        //overrides
        public override bool refreshUITheme()
        {
            //set the ui brushes according to the selected theme
            Brush textBrush = new SolidColorBrush(themes.getSelectedTheme().Value.textColor);

            //background
            if (managerGrid != null)
                managerGrid.Background = themes.getBackgroundGradientBrush();

            //controls
            statusText.Foreground = textBrush;

            return true;
        }

        //overrides
        public override bool refreshUIElements()
        {
            //does nothing for now
            return true;
        }

        //overrides - Does Nothing
        public override bool clearAllInputs()
        {
            //does nothing, theres no input field in the Manager Grid for now
            return true;
        }
    }
    public class NewGameGrid_UI_REF{
        db_stick.db_stick db; //for further purpopses within this control
        
        //refs

        //gameList
        public List<KeyValuePair<long, GameListItem>> gameList;
        public StackPanel gameListPanel = null;

        //buttons
        public StackPanel buttonStackPanel;
        public Button AddGameBtn;
        public Button AddPrizeBtn;

        public Popup popup;

        public NewGameGrid_UI_REF(db_stick.db_stick db) {
            this.db = db;
            gameList = new List<KeyValuePair<long, GameListItem>>();
        }

        public bool refreshGameList(DataTable? newTable) {

            if (gameList == null || gameListPanel == null || newTable == null)
                return false;
            
            //clear out the previous entries if any
            gameList.Clear();
            gameListPanel.Children.Clear();


            //loop to fill out the list again
            foreach (DataRow row in newTable.Rows)
            {
                //fetch the details of the game
                long id = (long)row["ID"];
                string gameName = row["GAME_NAME"].ToString();
                if (gameName == null) gameName = "Personal_Lottery"; //for fail safe
                DateTime createDate = (DateTime)row["CREATE_DATE"];
                DateTime gameDate = (DateTime)row["GAME_DATE"];

                //create the game list item
                GameListItem gameListItem = new GameListItem(db, this, id, gameName, createDate, gameDate, int.Parse(row["GAME_TICKET_COUNT"].ToString()));

                //add to the stack panel
                gameListPanel.Children.Add(gameListItem);
                //add to the uiref
                gameList.Add(new KeyValuePair<long, GameListItem>(id, gameListItem));
            }

            return true;
        }

        public bool showPopup(Control cont) {
            if (cont == null) return false;
            popup.Child = cont;
            popup.IsOpen = true;
            return true;
        }

        public bool closePopup() { 
            popup.IsOpen = false;
            popup.Child = null;
            return true;
        }
    }

    public class SearchGrid_UI_REF{

        
        public SearchGrid_UI_REF() {
            
        }
    }

    public class DebtGrid_UI_REF {


        public DebtGrid_UI_REF() {
        }
    }

    public class NewWindowGrid_UI_REF : UI_REF {

        public UI_TYPE subUIType;

        //self variables
        public Grid newWindowGrid;
        public Button gameBoxBtn;
        public Button debtLookUpBtn;
        public Button searchGridBtn;
        


        //New Game Grid
        public Grid newGameGrid = null;
        public NewGameGrid_UI_REF newGame_uiref;

        //Search Grid
        public Grid searchGrid = null;
        public SearchGrid_UI_REF searchGrid_uiref;

        //debt Grid
        public Grid debtGrid = null;
        public DebtGrid_UI_REF debtGrid_uiref;

        private theme_stick.Themes_Stick themes;
        private db_stick.db_stick db;

        public NewWindowGrid_UI_REF(db_stick.db_stick db) {
            this.uiType = UI_TYPE.NEW_TAB_GRID;
            subUIType = UI_TYPE.NO_TYPE;
            themes = new theme_stick.Themes_Stick();
            this.db = db;
        }


        //function to refresh the Grid Theme according to the UI
        public override bool refreshUITheme()
        {
            //set the ui brushes according to the selected theme
            Brush textBrush = new SolidColorBrush(themes.getSelectedTheme().Value.textColor);
            Brush buttonBrush = new SolidColorBrush(themes.getSelectedTheme().Value.FocusColor);

            //determine the type stored in the ui
            if (subUIType == UI_TYPE.NO_TYPE) { //new window grid

                //controls
                newWindowGrid.Background = themes.getBackgroundGradientBrush(); //background

                //Buttons
                gameBoxBtn.Background = buttonBrush;
                gameBoxBtn.Foreground = textBrush;

                debtLookUpBtn.Background = buttonBrush;
                debtLookUpBtn.Foreground = textBrush;

                searchGridBtn.Foreground = textBrush;
                searchGridBtn.Background = buttonBrush;
            }

            else if (subUIType == UI_TYPE.NEW_GAME_GRID) {
                //Main Grid
                newGameGrid.Background = themes.getBackgroundGradientBrush();

                //get the recent table
                string retMsg = "";
                DataTable? gameList = db.get_All_Games(ref retMsg);

                //controls
                newGame_uiref.refreshGameList(gameList);
                //other controls
                newGame_uiref.AddGameBtn.Background = buttonBrush;
            }

            else if (subUIType == UI_TYPE.DEBT_GRID) {

            }

            else if (subUIType == UI_TYPE.SEARCH_GRID) { 
                
            }

            return true;
        }


        //function to refresh the elements of the tab
        public override bool refreshUIElements()
        {
            //determine the type stored in the ui
            if (subUIType == UI_TYPE.NO_TYPE)
            { //new window grid
              //does not need any refresh of elements right now  
            }

            else if (subUIType == UI_TYPE.NEW_GAME_GRID)
            {
                //get the recent table
                string retMsg = "";
                DataTable gameList = db.get_All_Games(ref retMsg);
                //controls
                newGame_uiref.refreshGameList(gameList);
            }

            else if (subUIType == UI_TYPE.DEBT_GRID)
            {
                //nothing right now
            }

            else if (subUIType == UI_TYPE.SEARCH_GRID)
            {
                //nothing right now
            }

            return true;
        }

        //function to clear all the input fileds available - Does Nothing
        public override bool clearAllInputs()
        {
            //does nothing now as there is no input field in any sub ui
            //determine the type stored in the ui
            if (subUIType == UI_TYPE.NO_TYPE)
            { //new window grid
              //does not need any refresh of elements right now  
            }

            else if (subUIType == UI_TYPE.NEW_GAME_GRID)
            {
                //no input field, skip
            }

            else if (subUIType == UI_TYPE.DEBT_GRID)
            {
                ////no input field, skip
            }

            else if (subUIType == UI_TYPE.SEARCH_GRID)
            {
                ////no input field, skip
            }

            return true;
        }

        //function to show the popup of the according window
        public bool showPopup(Control cont) {

            if (cont == null)
                return false;

            if (subUIType == UI_TYPE.NO_TYPE)
                return false;
            else if (subUIType == UI_TYPE.MANAGER_GRID)
                return false;
            else if (subUIType == UI_TYPE.NEW_TAB_GRID)
                return false;
            else if (subUIType == UI_TYPE.NEW_GAME_GRID)
                return newGame_uiref.showPopup(cont); //only this implemented for now
            else if (subUIType == UI_TYPE.SEARCH_GRID)
                return false;
            else if (subUIType == UI_TYPE.DEBT_GRID)
                return false;

            return false;
        }

        //function to close the popup of the according window
        public bool closePopup() {
            if (subUIType == UI_TYPE.NO_TYPE)
                return false;
            else if (subUIType == UI_TYPE.MANAGER_GRID)
                return false;
            else if (subUIType == UI_TYPE.NEW_TAB_GRID)
                return false;
            else if (subUIType == UI_TYPE.NEW_GAME_GRID)
                return newGame_uiref.closePopup(); //only this implemented for now
            else if (subUIType == UI_TYPE.SEARCH_GRID)
                return false;
            else if (subUIType == UI_TYPE.DEBT_GRID)
                return false;

            return false;
        }
    }



    //factory class for UI Grids
    public class Grid_Factory{
        private List<TabItem> tabs;
        private List<UI_REF> uirefs;
        private db_stick.db_stick db;
        private theme_stick.Themes_Stick themes;

        //Defines
        public const int ROW_MANAGER_GRID       = 3; //NOT-IN-USE Right Now
        public const int COL_MANAGER_GRID       = 3; //NOT-IN-USE Right Now
        public const int ROW_NEW_WINDOW_GRID    = 10;
        public const int COL_NEW_WINDOW_GRID    = 10;
        public const int ROW_NEW_GAME_GRID      = 10;
        public const int COL_NEW_GAME_GRID      = 10;


        public Grid_Factory(List<UI_REF> uirefs, List<TabItem> tabs, db_stick.db_stick db, theme_stick.Themes_Stick themes)
        {
            this.uirefs = uirefs;
            this.tabs = tabs;
            this.db = db;
            this.themes = themes;
        }



        public Grid cerateManagerGrid(string initialStatusText) {
            Grid managerGrid = new Grid();
            ManagerGrid_UI_REF uiref = new ManagerGrid_UI_REF();

            //FAKE USER POP UP
            //Popup fkU = new Popup();
            //fkU.Child = new FAKEUSER.FAKEUSERPOPUP();
            //fkU.IsOpen = true;


            //add the manager grid to the uiref
            uiref.managerGrid = managerGrid;

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

            //refresh theme
            uiref.refreshUITheme();

            return managerGrid;
        }


        //function to fill up the game list and make the game list items for the stack panel
        //accroding to a supplied game table datatable
        //also fills up the new game ui refs list
        private bool fillGameList(DataTable gameTabelSet, NewGameGrid_UI_REF uiref, StackPanel sp) {

            //main loop for each of the rows
            foreach (DataRow row in gameTabelSet.Rows) {
                //fetch the details of the game
                long id = (long)row["ID"];
                string gameName = row["GAME_NAME"].ToString();
                if (gameName == null) gameName = "Personal_Lottery"; //for fail safe
                DateTime createDate = (DateTime)row["CREATE_DATE"];
                DateTime gameDate = (DateTime)row["GAME_DATE"];

                //create the game list item
                GameListItem gameListItem = new GameListItem(
                    db,
                    uiref,
                    id,
                    gameName,
                    createDate,
                    gameDate,
                    int.Parse(row["GAME_TICKET_COUNT"].ToString())
                    );

                //add to the stack panel
                sp.Children.Add(gameListItem);
                //add to the uiref
                uiref.gameList.Add(new KeyValuePair<long, GameListItem>(id, gameListItem));
            }


            return true;
        }



        private bool AddNewGame_Callback(bool isSuccess) {

            //Deprecated - need to think How we would have achieved this though ????
            ////thread to sleep a bit an call out the close function for users to see the status output
            //ThreadStart ts = delegate { closeGameWorker(); };
            //Thread thr = new Thread(ts);

            //thr.Start();


            //if the insert was a success/failure, it will each time call this function
            //passing respectively to the isSuccess argument
            //If the close button is clicked, isSuccess will be false, and will be the last call to this function

            if (isSuccess) {
                //it is success, so refresh the games list and all the other UI Elements in all the possible uirefs
                foreach (UI_REF ui in uirefs) { 
                    ui.refreshUIElements();
                }
            }

            return isSuccess;
        }


        private void AddNewGame(Object sender, RoutedEventArgs args) {
            //show new game add pop up
            int listIndex = getListIndex(((Button)sender).Name);
            ((NewWindowGrid_UI_REF)uirefs[listIndex]).showPopup(new AddGamePopUp(((NewWindowGrid_UI_REF)uirefs[listIndex]).newGame_uiref.popup, db, AddNewGame_Callback));
        }



        private Grid createNewGameGrid(int windowCount) {
            Grid newGameGrid = new Grid();
            NewGameGrid_UI_REF newGameGrid_UI_REF = new NewGameGrid_UI_REF(db);

            //DEBUG ONLY
            newGameGrid.ShowGridLines = true;


            //devide the grid into rows and cols
            for (int i = 0; i < ROW_NEW_GAME_GRID; i++) {
                RowDefinition row = new RowDefinition();
                newGameGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < COL_NEW_GAME_GRID; i++) {
                ColumnDefinition col = new ColumnDefinition();
                newGameGrid.ColumnDefinitions.Add(col);
            }


            //create popup
            Popup popup = new Popup();
            popup.Name = "popup_" + windowCount;
            popup.Placement = PlacementMode.MousePoint;
            popup.HorizontalOffset = -100;
            popup.VerticalOffset = -100;
            popup.AllowsTransparency = false;
            popup.IsOpen = false;

            //make the popup draggable, TO-DO
            //popup.MouseMove += new MouseEventHandler((Object s, MouseEventArgs e) => {
            //    if (e.LeftButton == MouseButtonState.Pressed) {
            //        popup.PlacementRectangle = new Rect(
            //            new Point(e.GetPosition(popup).X, e.GetPosition(popup).Y),
            //            new Point(450, 800));
            //    }
            //});

            //add the popup control to the uiref
            newGameGrid_UI_REF.popup = popup;

            //add the popup control to the new game grid
            newGameGrid.Children.Add(popup);


            //get the table data set
            string retMsg = "";
            DataTable? gameList = db.get_All_Games(ref retMsg);


            //create the scroll view
            ScrollViewer gameListScroll = new ScrollViewer();
            gameListScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; //only visible when needed
            Grid.SetRow(gameListScroll, 1);
            Grid.SetColumn(gameListScroll, 5);
            Grid.SetColumnSpan(gameListScroll, 5);
            Grid.SetRowSpan(gameListScroll, 9);
            gameListScroll.Margin = new Thickness(3);
            //gameListScroll.Height = 100;
            gameListScroll.Width = 700;
            gameListScroll.Visibility = Visibility.Visible;

            //create the Stack Panel for the list
            StackPanel sp = new StackPanel();
            sp.Name = "StackPanel_" + windowCount;
            sp.Margin = new Thickness(3);
            sp.Visibility = Visibility.Visible;

            //add the stack panel to the ref
            newGameGrid_UI_REF.gameListPanel = sp;

            //make and fill the game list
            fillGameList(gameList, newGameGrid_UI_REF, sp);

            //add the stack panel to the scroll viewer
            gameListScroll.Content = sp;

            //add the scroll view to the new game grid
            newGameGrid.Children.Add(gameListScroll);


            //create the button stack Panel
            StackPanel buttonStackPanel = new StackPanel();
            buttonStackPanel.Visibility = Visibility.Visible;
            Grid.SetRow(buttonStackPanel, 2);
            Grid.SetColumn(buttonStackPanel, 2);
            buttonStackPanel.Margin = new Thickness(2);
            Grid.SetRowSpan(buttonStackPanel, 3);

            //add this stackpanel in the gamegrid
            newGameGrid.Children.Add(buttonStackPanel);

            //add to the uiref
            newGameGrid_UI_REF.buttonStackPanel = buttonStackPanel;


            ////create the Add Game Button
            Button AddGameBtn = new Button();
            AddGameBtn.Name = "AddGameBtn_" + windowCount;
            AddGameBtn.Content = "Add New Game";
            AddGameBtn.Height = 50;
            AddGameBtn.Visibility = Visibility.Visible;
            AddGameBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
            AddGameBtn.VerticalContentAlignment = VerticalAlignment.Center;
            AddGameBtn.Margin = new Thickness(2);
            AddGameBtn.Click += new RoutedEventHandler(AddNewGame);

            Button AddPrizeBtn = new Button();
            AddPrizeBtn.Name = "AddPrizeBtn_" + windowCount;
            AddPrizeBtn.Content = "Add New Game";
            AddPrizeBtn.Height = 50;
            AddPrizeBtn.Visibility = Visibility.Visible;
            AddPrizeBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
            AddPrizeBtn.VerticalContentAlignment = VerticalAlignment.Center;
            AddPrizeBtn.Margin = new Thickness(2);
            AddPrizeBtn.Click += new RoutedEventHandler(AddNewGame);

            //add the button to the stack panel
            buttonStackPanel.Children.Add(AddGameBtn);
            buttonStackPanel.Children.Add(AddPrizeBtn);

            //add the Add new Gamebutton to the uirefs
            newGameGrid_UI_REF.AddGameBtn = AddGameBtn;
            newGameGrid_UI_REF.AddPrizeBtn = AddPrizeBtn;



            //add to the uiref
            ((NewWindowGrid_UI_REF)uirefs[windowCount]).subUIType = UI_TYPE.NEW_GAME_GRID;
            ((NewWindowGrid_UI_REF)uirefs[windowCount]).newGameGrid = newGameGrid;
            ((NewWindowGrid_UI_REF)uirefs[windowCount]).newGame_uiref = newGameGrid_UI_REF;

            //refresh UI theme
            ((NewWindowGrid_UI_REF)uirefs[windowCount]).refreshUITheme();

            return newGameGrid;
        }


        private int getListIndex(string name) {
            String index = name.Substring(name.IndexOf('_') + 1, name.Length - (name.IndexOf('_') + 1));
            return int.Parse(index);
        }


        public void newGameBtn_Click(object sender, RoutedEventArgs e) {
            int listIndex = getListIndex(((Button)sender).Name);

            tabs[listIndex].Content = null;

            //create the Game Grid
            Grid newGameGrid = createNewGameGrid(listIndex);


            tabs[listIndex].Content = newGameGrid;
            tabs[listIndex].Header = "GameBox " + (listIndex + 1).ToString();
        }

        public void searchBtn_Click(object sender, RoutedEventArgs e) { 
            
        }

        public void debtBtn_Click(object sender, RoutedEventArgs e) { 
        
        }



        public Grid createNewWindowGrid() {
            Grid newWindowGrid = new Grid();
            NewWindowGrid_UI_REF uiref = new NewWindowGrid_UI_REF(db);

            int windowCount = uirefs.Count;

            //DEBUG ONLY
            newWindowGrid.ShowGridLines = true;

            //add the new grid to the uiref
            uiref.newWindowGrid = newWindowGrid;


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

            //create the ui's----------------------------------------------

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
            uiref.gameBoxBtn = newGameBtn;

            //Search Window Button
            Button searchButton = new Button();
            searchButton.Name = "searchBtn_" + windowCount;
            searchButton.Content = "Deep Search";
            searchButton.Click += new RoutedEventHandler(searchBtn_Click);
            searchButton.Height = 20;
            searchButton.Width = 200;
            searchButton.Margin = new Thickness(2);
            uiref.searchGridBtn = searchButton;

            //Debt Button
            Button debtBtn = new Button();
            debtBtn.Name = "debtBtn_" + windowCount;
            debtBtn.Content = "Debt Lookup";
            debtBtn.Click += new RoutedEventHandler(debtBtn_Click);
            debtBtn.Height = 20;
            debtBtn.Width = 200;
            debtBtn.Margin = new Thickness(2);
            uiref.debtLookUpBtn = debtBtn;

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

            //refresh the ui theme
            uiref.refreshUITheme();

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


        //connection string for now
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Work\\Codes\\C#\\GMS_LotteryTracker\\GMS_LotteryTracker\\data\\mob_db_01.mdf;Integrated Security=True";

        

        //class scope control items
        private List<TabItem> tabs;
        private List<UI_REF> uirefs;
        private TabItem mainTab;
        private Grid_Factory gridFactory;
        private db_stick.db_stick db;
        private theme_stick.Themes_Stick themes;

        

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
            db = new db_stick.db_stick(connectionString); //initialize the db connection
            themes = new theme_stick.Themes_Stick();

            //create the grid factory            
            gridFactory = new Grid_Factory(uirefs, tabs, db, themes);

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
            //updateManagerTabStatus("Welcome... \nGMS_LotteryTracker V 0.1");

            //FAKE USER
            updateManagerTabStatus("GMS_LotteryTracker V 0.1 \n SYSTEM ERROR : FATAL ISSUES - \n .NET Framework 6.0 Not compatible with architecture.");

            
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
