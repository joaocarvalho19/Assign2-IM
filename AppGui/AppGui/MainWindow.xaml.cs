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
            Actions action = new Actions(driver);
            string class_coord = coordinates[piece];
            IWebElement piece_from = driver.FindElement(By.ClassName(class_coord));
            piece_from.Click();

            System.Threading.Thread.Sleep(1000);

            IWebElement piece_to = driver.FindElement(By.ClassName(_to));
            action.ClickAndHold(piece_from).MoveToElement(piece_to).Release().Build().Perform();
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
        public void change_color(string color)
        {
            // "circle-gearwheel" -- settings

            try{
                click("circle-gearwheel");
                System.Threading.Thread.Sleep(3000);
                // Get all elements with a given ClassName
            
                IList<IWebElement> all = driver.FindElements(By.ClassName("settings-select"));

                //create select element object 
                var selectElement = new SelectElement(all[1]);

                //select by value
                selectElement.SelectByIndex(2);
                System.Threading.Thread.Sleep(3000);
                IList<IWebElement> settingsButtons = driver.FindElements(By.ClassName("settings-modal-container-button"));

                IWebElement saveButtom = settingsButtons[1];
                saveButtom.Click();

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
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            
            start_Browser();
            move("white_pawn_5", "square-54");
            /*change_color("blue");
            System.Threading.Thread.Sleep(5000);
            abort_game();*/
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
                case "ABORT":
                    //_s = rectangle;
                    abort_game();
                    break;
                case "abre definições":
                    //_s = rectangle;
                    open_settings();
                    break;
                case "SQUARE": _s = rectangle;
                    break;
                case "BLUE":
                    change_color("blue");
                    break;
                case "CIRCLE": _s = circle;
                    break;
                case "TRIANGLE": _s = triangle;
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
            json2 += (string)json.recognized[0].ToString()+ " ";
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
