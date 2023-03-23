using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        IMongoQueryable<Contact> all_contacts;
        IMongoQueryable<Contact> search_result;
        static string connectionString = "mongodb://localhost:27017";
        static MongoClient client = new MongoClient(connectionString);
        static IMongoDatabase database = client.GetDatabase("PhoneBook");
        static IMongoCollection<Contact> collection = database.GetCollection<Contact>("Contact");
        
        public Form1()
        {
            InitializeComponent();
            
            this.all_contacts = from x in collection.AsQueryable<Contact>()
                                orderby x.name
                                select x;
            
            this.search_result = this.all_contacts;

            UpdateList(this.search_result);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Contact con = new Contact();
                var arr = search_result.ToArray();
                con.id = arr[listBox1.SelectedIndex].id;
                con.name = arr[listBox1.SelectedIndex].name;
                con.surname = arr[listBox1.SelectedIndex].surname;
                con.number = arr[listBox1.SelectedIndex].number;
                con.email = arr[listBox1.SelectedIndex].email;
                con.address = arr[listBox1.SelectedIndex].address;

                Form2 form2 = new Form2(con, true);
                form2.ShowDialog();

                UpdateList(this.search_result);
            }
        }

        private void AddContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(new Contact());
            form2.ShowDialog();
            UpdateList(this.all_contacts);
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Телефонна книга©\nПросянніков А.В., Явгусішин Б.А.\n122-Б20");
        }

        private void UpdateList(IMongoQueryable<Contact> all_contacts)
        {
            listBox1.Items.Clear();

            foreach (var item in all_contacts)
            {
                listBox1.Items.Add($"{item.name} {item.surname}".PadLeft(listBox1.Width/8));
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {

            if (textBoxSearch.Text == "")
            { 
                this.search_result = all_contacts; 
            }
            else
            {
                this.search_result = from x in all_contacts
                                     where x.name.ToLower().StartsWith(textBoxSearch.Text) || x.surname.ToLower().StartsWith(textBoxSearch.Text) || x.number.Contains(textBoxSearch.Text)
                                     orderby x.name
                                     select x;
            }
            
            UpdateList(this.search_result);
        }
    }
    public class Contact
    {
        private ObjectId _id;
        private string Name = "";
        private string Surname = "";
        private string Number = "";
        private string Email = "";
        private string Address = "";
        public ObjectId id { get => _id; set => _id = value; }
        public string name { get => Name; set => Name = value; }
        public string surname { get => Surname; set => Surname = value; }
        public string number { get => Number; set => Number = value; }
        public string email { get => Email; set => Email = value; }
        public string address { get => Address; set => Address = value; }
    }
}
