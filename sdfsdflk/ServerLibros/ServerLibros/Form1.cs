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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;

namespace ServerLibros
{
    public partial class Form1 : Form
    {
        private const string DBName = "database.sqlite";
        private const string SQLScript = @"..\..\Util\database.sql";
        UdpClient udpClient = new UdpClient(8070);
        UdpClient udpClient2 = new UdpClient(8071);
        private static bool IsDbRecentlyCreated = false;
        public Form1()
        {
            ThreadStart del = new ThreadStart(Listen);
            Thread hilo = new Thread(del);
            hilo.Start();
            ThreadStart del2 = new ThreadStart(ListenBooks);
            Thread hilo2 = new Thread(del2);
            hilo2.Start();
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            agregar a = new agregar(this);
            a.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var result = new List<User>();

            using (var ctx = GetInstance())
            {
                var query = "SELECT * FROM Books3";
                try
                {
                    using (var command = new SQLiteCommand(query, ctx))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new User
                                {
                                    nombre = (reader["nombre"].ToString()),
                                    autor = reader["autor"].ToString(),
                                    usuario = reader["usuario"].ToString(),
                                    fechasalida = reader["fechasalida"].ToString(),
                                    fechaentrada = reader["fechaingreso"].ToString(),
                                    disponible = reader["disponible"].ToString(),
                                    imagen = reader["imagen"].ToString()
                                });
                            }
                        }
                        Console.WriteLine("exito");
                    }
                }
                catch
                {
                    Console.WriteLine("no hay bd");
                }
            }
            listView1.Items.Clear();
            foreach(var book in result)
            {
                var imageview = new ImageList();
                if(book.imagen != ""){
                    imageview.Images.Add(Image.FromFile(book.imagen));
                    imageview.ImageSize = new Size(32, 32);
                    listView1.LargeImageList = imageview;
                }
                
                var row = new string[] { book.nombre, book.fechaentrada, book.fechaentrada,book.usuario,book.disponible,book.autor};
                var lvi = new ListViewItem(row);
                lvi.Tag = book;
                listView1.Items.Add(lvi);
            }
        }
        public static SQLiteConnection GetInstance()
        {
            var db = new SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = True; ");

            db.Open();

            return db;
        }

        public void Listen()
        {
           
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                Console.WriteLine(returnData);
                var ite = items(returnData);
                var envio = JsonConvert.SerializeObject(ite);
                UdpClient udpClient4 = new UdpClient();
                udpClient4.Connect("localhost", 8000);
                Byte[] senddata = Encoding.ASCII.GetBytes(envio);
                udpClient4.Send(senddata, senddata.Length);
                udpClient4.Close();
            }
            udpClient.Close();
        }
        public List<User> items(string q)
        {
            var result = new List<User>();

            using (var ctx = GetInstance())
            {
                var query = q;
                try
                {
                    using (var command = new SQLiteCommand(query, ctx))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new User
                                {
                                    nombre = (reader["nombre"].ToString()),
                                    autor = reader["autor"].ToString(),
                                    usuario = reader["usuario"].ToString(),
                                    fechasalida = reader["fechasalida"].ToString(),
                                    fechaentrada = reader["fechaingreso"].ToString(),
                                    disponible = reader["disponible"].ToString(),
                                    imagen = reader["imagen"].ToString()
                                });
                            }
                        }
                        Console.WriteLine("exito");
                        var results = JsonConvert.SerializeObject(result);
                        Console.WriteLine(results);
                        return result;
                    }
                }
                catch
                {
                    Console.WriteLine("no hay bd");
                    return null;
                }
            }
        }
        
        public void ListenBooks()
        {
            
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient2.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                
                Console.WriteLine(returnData);

                var data = getBooksYes(returnData);
                UdpClient udpClient = new UdpClient();
                udpClient.Connect("localhost", 8061);
                Byte[] senddata = Encoding.ASCII.GetBytes(data);
                udpClient.Send(senddata, senddata.Length);
                udpClient.Close();

                
                receiveBytes = udpClient2.Receive(ref RemoteIpEndPoint);
                returnData = Encoding.ASCII.GetString(receiveBytes);
                var datos = JsonConvert.DeserializeObject<List<User>>(returnData);
                Console.WriteLine(returnData);
                Update(datos);
                break;
            }
            
        }

        public void Update(List<User> t)
        {
            using (var ctx = GetInstance())
            {
                foreach(var x in t)
                {
                    var query = $"Update Books3 set usuario = 'omar', disponible = 'no', fechasalida = '{DateTime.Today.ToString()}' where nombre = '{x.nombre}'";
                    var command = new SQLiteCommand(query,ctx);
                    command.ExecuteNonQuery();

                }
            }
        }

        public string getBooksYes(string q)
        {
            var result = new List<User>();

            using (var ctx = GetInstance())
            {
                var query = q;
                try
                {
                    using (var command = new SQLiteCommand(query, ctx))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new User
                                {
                                    nombre = (reader["nombre"].ToString()),
                                    autor = reader["autor"].ToString(),
                                    usuario = reader["usuario"].ToString(),
                                    fechasalida = reader["fechasalida"].ToString(),
                                    fechaentrada = reader["fechaingreso"].ToString(),
                                    disponible = reader["disponible"].ToString(),
                                    imagen = reader["imagen"].ToString()
                                });
                            }
                        }
                        Console.WriteLine("exito");
                        var results = JsonConvert.SerializeObject(result);
                        Console.WriteLine(results);
                        return results;
                    }
                }
                catch
                {
                    Console.WriteLine("no hay bd");
                    return "";
                }
            }
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            udpClient.Close();
            udpClient2.Close();
        }
    }
}
