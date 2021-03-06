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
    public partial class StoreForm : Form
    {
        public string MySQLConnectionStringValue;

        public StoreForm(string login, string password, string MySQLConnectionString)
        {
            //creates new Forms and writes individual user data to labels

            MySQLConnectionStringValue = MySQLConnectionString;

            InitializeComponent();
            
            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                // writes client's ID to label

                sqlCon.Open();

                string queryID = "SELECT idklienta FROM users WHERE login='" + login + "' AND haslo='" + password + "'";
                
                MySqlCommand commandDatabaseID = new MySqlCommand(queryID, sqlCon);
                MySqlDataReader myReaderID = commandDatabaseID.ExecuteReader();

                if (myReaderID.Read())
                {
                    int userID = myReaderID.GetInt32(0);
                    userIDLabel.Text = userID.ToString();
                }      
            }
            

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                // writes client's vorname to label

                sqlCon.Open();

                string queryVorname = "SELECT imie FROM users WHERE login='" + login + "' AND haslo='" + password + "'";

                MySqlCommand commandDatabaseVorname = new MySqlCommand(queryVorname, sqlCon);
                MySqlDataReader myReaderVorname = commandDatabaseVorname.ExecuteReader();

                if (myReaderVorname.Read())
                {
                    string userVorname = myReaderVorname.GetString(0);
                    vorNameLabel.Text = userVorname.ToString();
                }
            }

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                // writes name's client to label

                sqlCon.Open();

                string queryName = "SELECT nazwisko FROM users WHERE login='" + login + "' AND haslo='" + password + "'";

                MySqlCommand commandDatabaseName = new MySqlCommand(queryName, sqlCon);
                MySqlDataReader myReaderName = commandDatabaseName.ExecuteReader();

                if (myReaderName.Read())
                {
                    string userName = myReaderName.GetString(0);
                    nameLabel.Text = userName.ToString();
                }
            }

            orderTable(MySQLConnectionStringValue);

            ordersDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            searchResultDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
           

        }

        private void searchQuery(string MySQLConnectionString)
        {
            // searches for books by name and writes into dataGridView

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                sqlCon.Open();

                string querySearch = "SELECT idksiazki AS 'ID ksiazki', imieautora AS 'Imie autora'," +
                    "nazwiskoautora AS 'Nazwisko autora',tytul AS 'Tytul', " +
                    "cena AS 'Cena' FROM ksiazki WHERE tytul LIKE" + "'%" + searchTextBox.Text + "%'";

                MySqlCommand commandDatabaseQuerySearch = new MySqlCommand(querySearch, sqlCon);
                MySqlDataReader myReaderQuerySearch = commandDatabaseQuerySearch.ExecuteReader();

                DataTable dtbl = new DataTable();

                dtbl.Load(myReaderQuerySearch);
                searchResultDataGridView.DataSource = dtbl;
            }
        }

        private void AddOrder(string MySQLConnectionString)
        {
            // adds the selected order 

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                sqlCon.Open();
                try
                {
                    int bookID = Convert.ToInt32(searchResultDataGridView.SelectedRows[0].Cells[0].Value);
                    string queryAddOrder = "INSERT INTO zamowienia (idklienta, idksiazki, status) VALUES (" + userIDLabel.Text + "," + bookID + "," + "''" + ")";

                    MySqlCommand commandDatabaseQueryAddOrder = new MySqlCommand(queryAddOrder, sqlCon);
                    MySqlDataReader myReaderqueryAddOrder = commandDatabaseQueryAddOrder.ExecuteReader();

                    MessageBox.Show("Dokonano zakupu", "Komunikat");
                }
                catch(Exception exc)
                {
                    MessageBox.Show("Nie wybrano ksiązki",exc.Message);
                }
            }
        }

        private void deleteOrder(string MySQLConnectionString)
        {
            // deletes the selected order

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                sqlCon.Open();

                int orderID = Convert.ToInt32(ordersDataGridView.SelectedRows[0].Cells[0].Value);
                string queryDeleteOrder = "DELETE FROM zamowienia WHERE idzamowienia ="+orderID;

                MySqlCommand commandDatabaseQueryDeleteOrder = new MySqlCommand(queryDeleteOrder, sqlCon);
                MySqlDataReader myReaderqueryDeleteOrder = commandDatabaseQueryDeleteOrder.ExecuteReader();
            }
        }



        private void orderTable(string MySQLConnectionString)
        {
            // writes to the table all orders that the user has

            using (MySqlConnection sqlCon = new MySqlConnection(MySQLConnectionString))
            {
                sqlCon.Open();

                string queryOrders = "SELECT zamowienia.idzamowienia, ksiazki.tytul AS 'Tytuł', ksiazki.imieautora AS" +
                    " 'Imie autora', ksiazki.nazwiskoautora AS 'Nazwisko autora', ksiazki.cena AS 'Cena'" +
                    " FROM ksiazki INNER JOIN zamowienia ON " +
                    "zamowienia.idksiazki = ksiazki.idksiazki WHERE zamowienia.idklienta=" + userIDLabel.Text;

                MySqlCommand commandDatabaseOrders = new MySqlCommand(queryOrders, sqlCon);
                MySqlDataReader myReaderOrders = commandDatabaseOrders.ExecuteReader();

                DataTable dtbl = new DataTable();

                dtbl.Load(myReaderOrders);
                ordersDataGridView.DataSource = dtbl;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            searchQuery(MySQLConnectionStringValue);
        }

        private void buyButton_Click(object sender, EventArgs e)
        {
            AddOrder(MySQLConnectionStringValue);
            orderTable(MySQLConnectionStringValue);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteOrder(MySQLConnectionStringValue);
            orderTable(MySQLConnectionStringValue);
            MessageBox.Show("Usunięto zamówienie","Komunikat");
        }
    }
}
