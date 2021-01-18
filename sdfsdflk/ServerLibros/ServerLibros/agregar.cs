using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace ServerLibros
{
    public partial class agregar : Form
    {
        private const string DBName = "database.sqlite";
        private const string SQLScript = @"..\..\Util\database.sql";
        private static bool IsDbRecentlyCreated = false;
        private string imgPath = "";
        Form1 f;
        public agregar(Form1 t)
        {
            f = t;
            InitializeComponent();
            if (!File.Exists(Path.GetFullPath(DBName)))
            {
                SQLiteConnection.CreateFile(DBName);
                IsDbRecentlyCreated = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                var img = new Bitmap(open.FileName);
                pictureBox1.Image = new Bitmap(img, new Size(100, 144));
                // image file path  
                Console.WriteLine(open.FileName);
                imgPath = open.FileName;
            }
        }
        public static SQLiteConnection GetInstance()
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );

            db.Open();

            return db;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && imgPath != "")
            {
                create();
            }
            else
            {
                MessageBox.Show("LLene los espacios restantes");
            }
        }

        public void create()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
            enviar(sqlite_conn);
        }
        public void enviar(SQLiteConnection ctx)
        {
            var query = "INSERT INTO Books3 (nombre, autor, fechaingreso, fechasalida, usuario, disponible, imagen) VALUES (?, ?, ?, ?,?,?,?)";

            using (var command = new SQLiteCommand(query, ctx))
            {
                command.Parameters.Add(new SQLiteParameter("nombre", textBox1.Text));
                command.Parameters.Add(new SQLiteParameter("autor", textBox2.Text));
                command.Parameters.Add(new SQLiteParameter("fechaingreso", DateTime.Today.ToString()));
                command.Parameters.Add(new SQLiteParameter("fechasalida", ""));
                command.Parameters.Add(new SQLiteParameter("usuario", ""));
                command.Parameters.Add(new SQLiteParameter("disponible", "si"));
                command.Parameters.Add(new SQLiteParameter("imagen", imgPath));

                command.ExecuteNonQuery();
                MessageBox.Show("Agregado correctamente");
                Close();
            }

        }
        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = True; ");
         // Open the connection:
         try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }
        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS Books3 (Id INTEGER PRIMARY KEY AUTOINCREMENT,nombre VARCHAR(100) NOT NULL,fechaingreso VARCHAR(100) NOT NULL, fechasalida VARCHAR(100) ,autor VARCHAR(100) NOT NULL,disponible VARCHAR(100) NOT NULL,usuario VARCHAR(100) ,imagen VARCHAR(100) NOT NULL); ";
         
         sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            

        }
    }
    }

