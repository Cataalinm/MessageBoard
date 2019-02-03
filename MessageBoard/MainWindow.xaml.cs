using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Windows.Threading;
using Classes;


namespace MessageBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        int logged = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        Person newperson = new Person();
        User newuser = new User();
        
        // When the "Send" button is clicked, the text in the textbox near to the button will be sent on to the Board. 
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (Username.Text.Length == 0)
            {
                Notloggederror.Visibility = Visibility.Visible;
            }
            else
            {
                Board.Text = Username.Text + ": " + Chat.Text + '\n' + Board.Text;
                //Clear the text that was written in the textbox and sent to the board. 
                Chat.Text = "";
            }
        }

        //When an user chooses to Logout out of his account, his information(Username and Password) will be erased from the Login window. 
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Username.Text = "";
            Password.Text = "";
            Login.Visibility = Visibility.Visible;
            Register.Visibility=Visibility.Visible;
            Logout.Visibility = Visibility.Collapsed;
            Board.Text = "";
            logged = 0;
        }

        //The Login Window will be made visible when the user chooses to log in. 
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.Visibility = Visibility.Visible;
            Notloggederror.Visibility = Visibility.Collapsed;
        }

        //The username and password for every user is stored in a text file called "Userdata". When a user wants to log-in, 
        //the username and password he types will be compared to the usernames and passwords in the text file and if
        //a match is found, the user will be allowed to log in, if not, an error message will be displayed. 
        //Also, a different error message will be displayed if there is no registed user and someone tries to log in. 
        private void Logging_Click(object sender, RoutedEventArgs e)
        {
            string line = null;
            string newstring = null;
            //int ok = 0;
            try
            {
                using (StreamReader sr = new StreamReader("Usersdata.txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        newstring = string.Concat(Username.Text, " ", Password.Text);
                        if (line == newstring)
                        {
                            logged = 1;
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                Fileopeningerror.Visibility = Visibility.Visible;
            }
            if (logged==1)
            {
                LoginWindow.Visibility = Visibility.Collapsed;
                Logout.Visibility = Visibility.Visible;
                Login.Visibility = Visibility.Collapsed;
                Register.Visibility = Visibility.Collapsed;
                Wrongdata.Visibility = Visibility.Collapsed;
            }
            else
            {
                FileInfo f = new FileInfo("Usersdata.txt");
                if (f.Exists == true)
                {
                    Wrongdata.Visibility = Visibility.Visible;
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += timer_Tick;
                    timer.Interval = new TimeSpan(0, 0, 2);
                    timer.Start();
                }
            }

        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow.Visibility = Visibility.Visible;
            Fileopeningerror.Visibility = Visibility.Collapsed;
        }

        //When someone tries to create a new account, all fields are mandatory, if any field is left empty an error message will be displayed. 
        //The username and password of the new account will be saved in the text file called "Usersdata". 
        //After a succesfull registration, all fields in the Registration Window will be erased so no user information is exposed. 
        private void CmpReg_Click(object sender, RoutedEventArgs e)
        {
            if (ToSPP.IsChecked == false)
                AgreeToSPP.Visibility = Visibility.Visible;
            else
            {
                AgreeToSPP.Visibility = Visibility.Collapsed;
                if (Passfield.Text != ConfirmPass.Text)
                {
                    Passdontmatch.Visibility = Visibility.Visible;
                }
                else
                {
                    Passdontmatch.Visibility = Visibility.Collapsed;
                    if (FNfield.Text.Length == 0 || LNfield.Text.Length == 0 || Emailfield.Text.Length == 0 || Passfield.Text.Length == 0 || UNfield.Text.Length == 0)
                    {
                        Mandatoryfields.Foreground = Brushes.Red;
                    }
                    else
                    {
                        try
                        {
                            newperson.FirstName = FNfield.Text;
                            newperson.LastName = LNfield.Text;
                            newperson.Email = Emailfield.Text;
                            newperson.birthdate = Convert.ToDateTime(DoBfield.Text);
                            newuser.Username = UNfield.Text;
                            newuser.Password = Passfield.Text;
                            RegistrationWindow.Visibility = Visibility.Collapsed;
                            using (StreamWriter outputfile = new StreamWriter(Path.Combine("Usersdata.txt"), true))
                            {
                                outputfile.WriteLine(newuser.Username + " " + newuser.Password);
                            }
                            FNfield.Text = "";
                            LNfield.Text = "";
                            Emailfield.Text = "";
                            DoBfield.Text = "";
                            UNfield.Text = "";
                            Passfield.Text = "";
                            ConfirmPass.Text = "";
                            ToSPP.IsChecked = false;
                        }
                        catch (Exception ex)
                        {
                            Dateformat.Foreground = Brushes.Red;
                        }

                    }
                }
            }
            
        }

        //Button to close the application 
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //When the button is clicked, the content of the board will be saved into a text file called History.txt
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (logged == 1)
            {
                if (Board.Text.Length != 0)
                {
                    using (StreamWriter outputfile = new StreamWriter(Path.Combine("History.txt"), true))
                    {
                        outputfile.WriteLine(Board.Text);
                    }
                }
            }
            else
            {
                Notloggederror.Visibility = Visibility.Visible;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Start();
            }
        }

        //When the button is clicked, the content that was saved using the "Save" button will be displayed on a different board.
        //If the history file doesn't exist, an error message will be displayed. 
        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (logged == 1)
            {
                try
                {
                    using (StreamReader sr = new StreamReader("History.txt"))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Historyboard.Text = Historyboard.Text + '\n' + line;

                        }
                    }
                    Historyboard.Visibility = Visibility.Visible;
                    Closehistory.Visibility = Visibility.Visible;
                    Fileopeningerror.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Fileopeningerror.Visibility = Visibility.Visible;
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += timer_Tick;
                    timer.Interval = new TimeSpan(0, 0, 2);
                    timer.Start();
                }
            }
            else
            {
                Notloggederror.Visibility = Visibility.Visible;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Start();
            }
        }

        //after 2 seconds the error message will dissapear from the screen
        private void timer_Tick(object sender, EventArgs e)
        {
            Fileopeningerror.Visibility = Visibility.Collapsed;
            Wrongdata.Visibility = Visibility.Collapsed;
            Notloggederror.Visibility = Visibility.Collapsed; 
        }

        //Button for closing the history board.
        //Erase the text from history board every time it's closed. 
        private void Closehistory_Click(object sender, RoutedEventArgs e)
        {
            Historyboard.Visibility = Visibility.Hidden;
            Closehistory.Visibility = Visibility.Hidden;
            Historyboard.Text = "";
        }
        //Button for closing the registration window and erasing all the fields inside. 
        private void Closeregistration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow.Visibility = Visibility.Collapsed;
            FNfield.Text = "";
            LNfield.Text = "";
            Emailfield.Text = "";
            DoBfield.Text = "";
            UNfield.Text = "";
            Passfield.Text = "";
            ConfirmPass.Text = "";
            ToSPP.IsChecked = false;
        }

        //Button for closing the login window and erasing the Username & Password fields. 
        private void Closelogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            Username.Text = "";
            Password.Text = "";
        }

        //Button for deleting the file containing the history saved. 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (logged == 1)
            {
                File.Delete("History.txt");
                Historyboard.Text = "";
            }
            else
            {
                Notloggederror.Visibility = Visibility.Visible;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Start();
            }
        }
    }
}
