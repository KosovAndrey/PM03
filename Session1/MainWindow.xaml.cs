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
using System.Timers;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Uchet
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Code codewin;
        public MainWindow()
        {
            InitializeComponent();
             
        }
        public char[] randomWord = new char[8];
        public string code;
        DispatcherTimer timer;
        int i = 1;
        int k = 10;
        public bool opened=false;
        private void btnUpdate_Click(object sender, RoutedEventArgs e) //генерация кода, открытие диалогового окна, ожидается его закрытие
        {
            if (opened == false)
            {
                Random random = new Random();
                for (int i = 0; i < 8; i++)
                {
                    randomWord[i] = (char)random.Next(48, 123);
                }
                code = new string(randomWord);
                codewin = new Code(code);
                codewin.Show();
                codewin.Closing += codewin_Closing;
                opened = true;
            }
            else
            {
                codewin.Close();
                btnUpdate_Click(sender, e);
            }
        }
        private void codewin_Closing(object sender, System.ComponentModel.CancelEventArgs e)//таймер
        {
            opened = false;
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)//сброс кода
        {
            i++;
            if (i > k)
            {
                timer.Stop();
                code = null;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)//очистка полей
        {
            txbCode.Text = "";
            txbLogin.Text = "";
            txbPassword.Password = "";
        }
        private string connstring= @"data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\Uchet.mdf;integrated security=True";//подключение базы данных
        private SqlConnection conn;
        private void btnSignIn_Click(object sender, RoutedEventArgs e)//валидация данных
        {
            try 
            {
                conn = new SqlConnection(connstring);
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User] " +
                    "WHERE Login = @login AND Password =@password");
                cmd.Connection = conn;
                conn.Open();
                cmd.Parameters.AddWithValue("@login", txbLogin.Text);
                cmd.Parameters.AddWithValue("@password", txbPassword.Password);
                SqlDataReader Dr = cmd.ExecuteReader();
                int i1 = 0;
                while (Dr.Read())
                {
                    i1++;
                }
                if (i1 == 1)
                {
                    if (txbCode.Text == code)
                    {
                        UserWindow user = new UserWindow();
                        user.Show();
                        this.Close();
                    }
                    else 
                    {
                        MessageBox.Show("Введён неправильный код, повторите попытку");    
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
                conn.Close();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка"); 
            }
        }
        private void txbLogin_KeyUp(object sender, KeyEventArgs e)//валидация номера
        {
            if (e.Key == Key.Enter)
            {
                conn = new SqlConnection(connstring);
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User] " +
                    "WHERE Login = @login");
                cmd.Connection = conn;
                conn.Open();
                cmd.Parameters.AddWithValue("@login", txbLogin.Text);
                SqlDataReader Dr = cmd.ExecuteReader();
                int i1 = 0;
                while (Dr.Read())
                {
                    i1++;
                }
                if (i1 == 1)
                {
                    txbPassword.IsEnabled = true;
                }
                else 
                {
                    MessageBox.Show("Введённый номер - неправильный!");
                }
                conn.Close();
            }
        }
        private void txbPassword_KeyUp(object sender, KeyEventArgs e)//валидация пароля
        {
            if (e.Key == Key.Enter)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User] " +
                    "WHERE Login = @login AND Password =@password");
                cmd.Connection = conn;
                conn.Open();
                cmd.Parameters.AddWithValue("@login", txbLogin.Text);
                cmd.Parameters.AddWithValue("@password", txbPassword.Password);
                SqlDataReader Dr = cmd.ExecuteReader();
                int i1 = 0;
                while (Dr.Read())
                {
                    i1++;
                }
                if (i1 == 1)
                {
                    txbCode.IsEnabled = true;
                    btnUpdate.IsEnabled = true;
                    Random random = new Random();
                    for (int i = 0; i < 8; i++)
                    {
                        randomWord[i] = (char)random.Next(48, 123);
                    }
                    if (opened == false)
                    {
                        code = new string(randomWord);
                        codewin = new Code(code);
                        codewin.Show();
                        codewin.Closing += codewin_Closing;
                        opened = true;
                    }
                    else
                    {
                        codewin.Close();
                        btnUpdate_Click(sender, e);
                    }
                }
                else 
                {
                    MessageBox.Show("Введённый пароль - неправильный!");
                }
                conn.Close();
            }
        }
        private void txbCode_KeyUp(object sender, KeyEventArgs e)//валидация кода
        {
            if (e.Key == Key.Enter)
            {
                btnSignIn.IsEnabled = true;
            }
        }
    }
}
