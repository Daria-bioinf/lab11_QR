using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace lab11_QR
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=Samples.db;Version=3;";
        public Form1()
        {
            InitializeComponent();

            CreateTable();
        }
        private void CreateTable() {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString)) { 
                connection.Open();
                string query = @"CREATE TABLE IF NOT EXISTS Samples (
                                Id TEXT PRIMARY KEY,
                                Name TEXT, 
                                Type TEXT,
                                SampleDate TEXT,
                                Description TEXT)";
             SQLiteCommand command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        
        }

        private void richTextBoxDesc_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query =@"INSERT INTO Samples( ID,Name,Type,SampleDate,Description) VALUES(@id,@name,@type,@date,@desc)";

                SQLiteCommand command =
                new SQLiteCommand(query, connection);

                command.Parameters.AddWithValue(
                "@id",
                textBoxID.Text);

                command.Parameters.AddWithValue(
                "@name",
                textBoxName.Text);

                command.Parameters.AddWithValue(
                "@type",
                comboBoxType.Text);

                command.Parameters.AddWithValue(
                "@date",
                dateTimePicker1.Value.ToShortDateString());

                command.Parameters.AddWithValue(
                "@desc",
                richTextBoxDesc.Text);

                command.ExecuteNonQuery();

                MessageBox.Show(
                "Próbka została zapisana");
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Samples";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter( query, connection);

                DataTable table = new DataTable();

                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }
        }

        private void buttonQR_Click(object sender, EventArgs e)
        {
            string qrText =$@"ID: {textBoxID.Text} Nazwa: {textBoxName.Text} Typ: {comboBoxType.Text} Data: {dateTimePicker1.Value.ToShortDateString()}";

            QRCodeGenerator generator = new QRCodeGenerator();

            QRCodeData data =
            generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);

            QRCode qr = new QRCode(data);

            Bitmap image = qr.GetGraphic(10);

            pictureBoxQR.Image = image;
        }

        private void buttonPNG_Click(object sender, EventArgs e)
        {
            if (pictureBoxQR.Image == null)
            {
                MessageBox.Show("Najpierw wygeneruj QR");

                return;
            }

            SaveFileDialog save =new SaveFileDialog();

            save.Filter ="PNG (*.png)|*.png";

            if (save.ShowDialog()== DialogResult.OK)
            {
                pictureBoxQR.Image.Save(save.FileName);

                MessageBox.Show("PNG zapisany");
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query =@"SELECT *FROM Samples WHERE ID LIKE @search OR Name LIKE @search OR Type LIKE @search";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);

                adapter.SelectCommand.Parameters.AddWithValue( "@search","%" +textBoxSearch.Text +"%");

                DataTable table = new DataTable();

                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }
        }
    }
}
