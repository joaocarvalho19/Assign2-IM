using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;


namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;

        //  new 16 april 2020
        private MmiCommunication mmiSender;
        private LifeCycleEvents lce;
        private MmiCommunication mmic;

        String chess_url = "https://www.chess.com/play/computer";

        IWebDriver driver;

        // Keep the coordinates of pieces
        Dictionary<string, string> coordinates =
                       new Dictionary<string, string>();

        public void get_coordinates()
        {
            coordinates.Add("white_rook_1", "square-11");
            coordinates.Add("white_rook_2", "square-81");
            coordinates.Add("white_knight_1", "square-21");
            coordinates.Add("white_knight_2", "square-71");
            coordinates.Add("white_bishop_1", "square-31");
            coordinates.Add("white_bishop_2", "square-61");
            coordinates.Add("white_queen", "square-41");
            coordinates.Add("white_king", "square-51");

            for(int i = 1; i<=8; i++)
            {
                coordinates.Add("white_pawn_"+i, "square-"+i+"2");
            }
            coordinates.Add("black_rook_1", "square-18");
            coordinates.Add("black_rook_2", "square-88");
            coordinates.Add("black_knight_1", "square-28");
            coordinates.Add("black_knight_2", "square-78");
            coordinates.Add("black_bishop_1", "square-38");
            coordinates.Add("black_bishop_2", "square-68");
            coordinates.Add("black_queen", "square-48");
            coordinates.Add("black_king", "square-58");

            for (int i = 1; i <= 8; i++)
            {
                coordinates.Add("black_pawn_" + i, "square-" + i + "7");
            }
        }
         // Move a Piece
        public void move(string piece, string _to)
        {
            try
            {
                Actions action = new Actions(driver);
                string class_coord = coordinates[piece];
                IWebElement piece_from = driver.FindElement(By.ClassName(class_coord));
                piece_from.Click();

                System.Threading.Thread.Sleep(1000);

                IWebElement piece_to = driver.FindElement(By.ClassName(_to));
                action.ClickAndHold(piece_from).MoveToElement(piece_to).Release().Build().Perform();
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR moving a piece {0}", e);
            }

        }

        // Abort the game
        public void abort_game()
        {
            try { 
                IList<IWebElement> controlsButtoms = driver.FindElements(By.ClassName("primary-controls-button"));
                IWebElement new_gameButtom = controlsButtoms[0];
                new_gameButtom.Click();

                IWebElement yesButton = driver.FindElement(By.XPath("//button[.='Yes']"));
                yesButton.Click();
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }
    
        }
        public void start_new_game()
        {
            try
            {
                IList<IWebElement> controlsButtoms = driver.FindElements(By.ClassName("game-over-controls-buttonlg"));
                IWebElement new_gameButtom = controlsButtoms[1];
                new_gameButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR Starting new game {0}", e);
            }

        }

        public void undo_move()
        {
            try
            {
                IList<IWebElement> controlsButtoms = driver.FindElements(By.ClassName("primary-controls-button"));
                IWebElement undoButtom = controlsButtoms[1];
                undoButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR undo a move {0}", e);
            }

        }

        public void redo_move()
        {
            try
            {
                IList<IWebElement> controlsButtoms = driver.FindElements(By.ClassName("primary-controls-button"));
                IWebElement undoButtom = controlsButtoms[2];
                undoButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR redo a move {0}", e);
            }

        }

        public void show_clue()
        {
            try
            {
                IList<IWebElement> controlsButtoms = driver.FindElements(By.ClassName("primary-controls-button"));
                IWebElement undoButtom = controlsButtoms[3];
                undoButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR show a clue {0}", e);
            }

        }

        // Simple click on the page
        public void click(string _class)
        {
            IWebElement buttom = driver.FindElement(By.ClassName(_class));
            buttom.Click();
        }

        // Open settings
        public void open_settings()
        {
            try { 
                click("circle-gearwheel");
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }
        }

        // Change the color of the board
        public void change_color()
        {
            try {
                // Get all elements with a given ClassName

                IList<IWebElement> all = driver.FindElements(By.ClassName("settings-select"));

                //Get a random index
                Random r = new Random();
                int index = r.Next(1, 29); //for ints
                //create select element object 
                var selectElement = new SelectElement(all[1]);

                //select by value
                selectElement.SelectByIndex(index);

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }

        }

        public void change_pieces()
        {
            try
            {
                // Get all elements with a given ClassName

                IList<IWebElement> all = driver.FindElements(By.ClassName("settings-select"));

                //Get a random index
                Random r = new Random();
                int index = r.Next(1, 38); //for ints
                //create select element object 
                var selectElement = new SelectElement(all[0]);

                //select by value
                selectElement.SelectByIndex(index);

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }

        }
        public void save_options()
        {
            try
            {
                //Save
                IList<IWebElement> settingsButtons = driver.FindElements(By.ClassName("settings-modal-container-button"));

                IWebElement saveButtom = settingsButtons[1];
                saveButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR saving {0}", e);
            }
        }

        public void cancel_options()
        {
            // "circle-gearwheel" -- settings

            try
            {
                //Cancel
                IList<IWebElement> settingsButtons = driver.FindElements(By.ClassName("settings-modal-container-button"));

                IWebElement cancelButtom = settingsButtons[0];
                cancelButtom.Click();

            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR cancel {0}", e);
            }
        }

        public void sounds()
        {
            // "circle-gearwheel" -- settings
            
            try
            {
                IList<IWebElement> all = driver.FindElements(By.ClassName("ui_v5-switch-button"));

                // Get all elements with a given ClassName
                IWebElement soundsButton = all[1];
                soundsButton.Click();

                
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }

        }

        public void scroll_options()
        {
            // "circle-gearwheel" -- settings

            try
            {
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                System.Threading.Thread.Sleep(500);
                IWebElement Element = driver.FindElement(By.XPath("//*[@id='board-layout-chessboard']/div[4]/div[2]/div[1]/div/div[12]/div[1]"));
                js.ExecuteScript("arguments[0].scrollIntoView();", Element);
                Console.Read();
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("ERROR {0}", e);
            }

        }


        // Start de browser
        [SetUp]
        public void start_Browser()
        {
            // Local Selenium WebDriver
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(chess_url);

            IWebElement okButton = driver.FindElement(By.XPath("//button[.='Ok']"));
            okButton.Click();

        }


        public MainWindow()
        {
            InitializeComponent();


            mmiC = new MmiCommunication("localhost",8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

            //init LifeCycleEvents..
            lce = new LifeCycleEvents("APP", "TTS", "User1", "na", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode
            // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)
            mmic = new MmiCommunication("localhost", 8000, "User1", "GUI");


            // CHESS APP -----------------------------------------------------------
            get_coordinates();
            foreach (KeyValuePair<string, string> kvp in coordinates)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            
            start_Browser();
            move("white_pawn_5", "square-54");
            /*System.Threading.Thread.Sleep(3000);
            show_clue();
            System.Threading.Thread.Sleep(3000);
            open_settings();
            System.Threading.Thread.Sleep(3000);
            change_color();*/

        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

            // Print income msg
            Console.WriteLine("INCOME MSG: "+(string)json.recognized[0].ToString());

            Shape _s = null;
            //switch (obj)
            switch ((string)json.recognized[0].ToString())
            {
                case "GIVEUP":
                    abort_game();
                    break;
                case "NEWGAME":
                    start_new_game();
                    break;
                case "OPTIONS":
                    open_settings();
                    break;
                case "UNDO":
                    undo_move();
                    break;
                case "REDO":
                    redo_move();
                    break;
                case "CLUE":
                    show_clue();
                    break;
                case "BOARD_COLOR":
                    change_color();
                    break;
                case "PIECE_APPEARANCE":
                    change_pieces();
                    break;
                case "SAVE":
                    save_options();
                    break;
                case "CANCEL":
                    cancel_options();
                    break;
                case "SCROLLDOWN":
                    scroll_options();
                    break;
                case "NOSOUND":
                    sounds();
                    break;
                case "SOUND":
                    sounds();
                    break;
            }

            /*App.Current.Dispatcher.Invoke(() =>
            {
                switch ((string)json.recognized[1].ToString())
                {
                    case "GREEN":
                        _s.Fill = Brushes.Green;
                        break;
                    case "BLUE":
                        abort_game();
                        _s.Fill = Brushes.Blue;
                        break;
                    case "RED":
                        _s.Fill = Brushes.Red;
                        break;
                }
            });*/

            mmic.Send(lce.NewContextRequest());

            string json2 = ""; // "{ \"synthesize\": [";
            json2 += (string)json.recognized[0].ToString();
            //json2 += (string)json.recognized[1].ToString() + " DONE." ;
            //json2 += "] }";
            /*
             foreach (var resultSemantic in e.Result.Semantics)
            {
                json += "\"" + resultSemantic.Value.Value + "\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";
            */
            var exNot = lce.ExtensionNotification(0 + "", 0 + "", 1, json2);
            mmic.Send(exNot);


        }
    }
}
