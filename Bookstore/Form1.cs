﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Bookstore
{
    public partial class LoginForm : Form
    {
        string MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=ksiegarnia";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void logInButton_Click(object sender, EventArgs e)
        {
            query();
        }

        private void query()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                sqlCon.Open();

                string login = loginTextBox.Text;
                string password = passwordTextBox.Text;

                string query = "SELECT * FROM users WHERE login='" + login + "' AND haslo='" + password + "'";

                MySqlCommand commandDatabase = new MySqlCommand(query, sqlCon);

                try
                {
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    if (myReader.HasRows)
                    {
                        MessageBox.Show("Dziala");
                        StoreForm store = new StoreForm();
                        store.Show();
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show("Nie poprawny login lub hasło");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Query error: " + e.Message);
                }




            }

        }
    }
}
