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

namespace PhoneBook
{
    public partial class Form2 : Form
    {
        ObjectId id;
        bool change_contact;
        static string connectionString = "mongodb://localhost:27017";
        static MongoClient client = new MongoClient(connectionString);
        static IMongoDatabase database = client.GetDatabase("PhoneBook");
        static IMongoCollection<Contact> collection = database.GetCollection<Contact>("Contact");

        public Form2(Contact contact, bool change_contact = false)
        {
            InitializeComponent();
            
            this.id = contact.id;
            textBox1.Text = contact.name;
            textBox2.Text = contact.surname;
            textBox3.Text = contact.number;
            textBox4.Text = contact.address;
            textBox5.Text = contact.email;
            
            this.change_contact = change_contact;

            button2.Visible = change_contact;
        }
        
        public const string motif = @"^\(?([0-9]{3})\)?[-\.]?([0-9]{3})[-\.]?([0-9]{4})$";
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                textBox1.Text = "";
                MessageBox.Show("Введіть ім'я");
                return;
            }

            bool number = IsPhoneNbr(textBox3.Text);
            if (!number)
            {
                MessageBox.Show("Не вірний формат номера телефону\n((123)4567890)");
                return;
            }

            Contact con = new Contact();
            con.name = textBox1.Text;
            con.surname = textBox2.Text;
            con.number = textBox3.Text;
            con.address = textBox4.Text;
            con.email = textBox5.Text;

            if (change_contact == false)
                SaveContact(con).GetAwaiter();
            else
                con.id = this.id;
                UpdateContact(con, this.id).GetAwaiter();
            
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteContact(this.id);
            this.Close();
        }

        public static bool IsPhoneNbr(string number)
        {
            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }

        private static async Task SaveContact(Contact Doc)
        {
            await collection.InsertOneAsync(Doc);
        }

        private static async Task UpdateContact(Contact Doc, ObjectId Id)
        {
            await collection.ReplaceOneAsync(new BsonDocument("_id", Id), Doc);
        }

        private static async Task DeleteContact(ObjectId Id)
        {
            await collection.DeleteOneAsync(new BsonDocument("_id", Id));
        }
    }
}
