using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Globalization;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

using System.Windows.Forms.DataVisualization.Charting;


namespace Temperature
{
    public partial class Form1 : Form
    {
        private SerialPort myPort;
        private String in_data;
        int duration = 0;
        String currentDate;
        float temp;
        float hum;

     

        public Form1()
        {
            InitializeComponent();
            DateTime localDate = DateTime.Now;
            portStatus.Text = "Not connected";
            chart1.ChartAreas[0].AxisY.Maximum = 40;
            chart1.ChartAreas[0].AxisY.Minimum = 10;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            currentDate = localDate.ToString();
            label4.Text += currentDate;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = "COM4";
            myPort.Parity = Parity.None;
            myPort.DataBits = 8;
            myPort.StopBits = StopBits.One;
            myPort.DataReceived += myPort_DataReceived;
            try
            {
                myPort.Open();
                portStatus.Text = "Connected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }

        }

        void myPort_DataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            in_data = myPort.ReadLine();
            Console.WriteLine(in_data);
            this.Invoke(new EventHandler(displaydata_event));
        }

        private void displaydata_event(Object sender, EventArgs e)
        {
            String result = in_data;
            string[] res = result.Split(null);
            temperatura.Text = res[0];
            humidity.Text = res[1]+ "%";
           
           temp = float.Parse(res[0], CultureInfo.InvariantCulture.NumberFormat);
           hum = float.Parse(res[1], CultureInfo.InvariantCulture.NumberFormat);
           progressBar1.Value = (int)Math.Round(temp);
           progressBar2.Value = (int)Math.Round(hum);
           timer1.Start();

           this.chart1.Series["Temperature"].Points.AddXY(duration, temp);
           this.chart1.Series["Humidity"].Points.AddXY(duration, hum); 

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            duration++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                myPort.Close();
                portStatus.Text = "Not connected";
                temperatura.Text = " ";
                humidity.Text = " ";
                progressBar1.Value = 0;
                progressBar2.Value = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {

            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("Test.pdf", FileMode.Create));
            doc.Open();
            Paragraph p = new Paragraph(currentDate);
            p.Alignment = Element.ALIGN_RIGHT;
            Paragraph p2 = new Paragraph("\n");
            Paragraph p3 = new Paragraph("Current situation");
            Paragraph p4 = new Paragraph("Temperature: " + temp);
            Paragraph p5 = new Paragraph("Humidity: " + hum + "%");
            Paragraph p6 = new Paragraph("Graphics:");
            p3.Alignment = Element.ALIGN_CENTER;
            doc.Add(p);
            doc.Add(p2);
            doc.Add(p3);
            doc.Add(p4);
            doc.Add(p5);

            //add graphic

            var chartImage = new MemoryStream();
            chart1.SaveImage(chartImage, ChartImageFormat.Png);
            iTextSharp.text.Image Chart_image = iTextSharp.text.Image.GetInstance(chartImage.GetBuffer());
            doc.Add(Chart_image);
    
            doc.Close();

        }

    }
}
