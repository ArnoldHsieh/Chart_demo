using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.Threading;


namespace Chart_demo
{
    public partial class Form1 : Form
    {
        static bool _continue;
        static SerialPort mySerialPort;

        // 委派(方法類別)
        public delegate void UART_Handler(string message);
        // 事件(方法變數)
        public static event UART_Handler UART_Event;

        int[] value = new int[200];


        public Form1()
        {
            InitializeComponent();
            UART_Event += new UART_Handler(txt_update);


            seril_ini();
            // 設定ChartArea
            ChartArea chtArea = new ChartArea("ViewArea");
            chtArea.AxisX.Minimum = 0; //X軸數值從0開始
            chtArea.AxisX.ScaleView.Size = 520; //設定視窗範圍內一開始顯示多少點
            chtArea.AxisX.Interval = 4;
            //chtArea.AxisX.LineWidth = 1;
            chtArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chtArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All; //設定scrollbar
            //chtArea.AxisY.Interval = 2000;
            chtArea.InnerPlotPosition.Auto = false;//(false,90,99,1,2);
            chtArea.InnerPlotPosition.Height = 90;
            chtArea.InnerPlotPosition.Width = 99;
            chtArea.InnerPlotPosition.X = 1;
            chtArea.InnerPlotPosition.Y = 2;
            chart1.Series[0].Color = Color.DarkBlue;
            chart1.Series[0].BorderWidth = 3;

            chart1.ChartAreas[0] = chtArea; // chart new 出來時就有內建第一個chartarea

            ChartArea chtArea2 = new ChartArea("ViewArea");
            chtArea2.AxisX.Minimum = 0; //X軸數值從0開始
           // chtArea2.AxisX.ScaleView.Size = 520; //設定視窗範圍內一開始顯示多少點
            chtArea2.AxisX.Interval = 5;
            //chtArea.AxisX.LineWidth = 1;
            chtArea2.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chtArea2.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All; //設定scrollbar
            //chtArea.AxisY.Interval = 2000;
            chtArea2.InnerPlotPosition.Auto = false;//(false,90,99,1,2);
            chtArea2.InnerPlotPosition.Height = 90;
            chtArea2.InnerPlotPosition.Width = 99;
            chtArea2.InnerPlotPosition.X = 1;
            chtArea2.InnerPlotPosition.Y = 2;
            chart2.Series[0].Color = Color.DarkBlue;
            chart2.Series[0].BorderWidth = 2;

            chart2.ChartAreas[0] = chtArea2;
        }
        private void txt_update(string message)
        {
            string[] data = message.Split(',');

            if (data[0] == "FFT")
            {
                this.Invoke(new MethodInvoker(delegate () { textBox1.Text += data[1]; }));
            }
            if (data[0] == "SIG")//signal
            {
                this.Invoke(new MethodInvoker(delegate () { txt_signal.Text += data[1]; }));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in chart2.Series)
            {
                series.Points.Clear();
            }


            for (int i = 0; i < 720; i++)
            {

                chart2.Series[0].Points.AddXY(i, Math.Sin(i * 2 * Math.PI / 360) * 2000);

                //  chart1.Series[1].Points.AddXY(i, Math.Cos(i * 2 * Math.PI / 360));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            chart1.Series[0].Points.Clear();

            if (string.IsNullOrEmpty(textBox1.Text))
                return;

            string[] ContentLines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //忽略空行



            //chart1.Series[0].
            for (int i = 0; i < ContentLines.Length; i++)
            {
                //ContentLines[i];
                chart1.Series[0].Points.AddXY(i * 4, ContentLines[i]);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // textBox1.Text = "93.351288\r\n245.811234\r\n167.057419\r\n147.193054\r\n265.608948\r\n237.697052\r\n398.920227\r\n135.341751\r\n87.22493";
        }
        private void seril_ini()
        {

            _continue = true;
            Thread readThread = new Thread(Read);

            mySerialPort = new SerialPort("COM4");

            mySerialPort.BaudRate = 115200;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            try
            {
                mySerialPort.Open();
                readThread.Start();
            }
            catch
            {
            }
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = mySerialPort.ReadLine() + "\n";
                    Console.Write(message);
                    UART_Event(message);
                }
                catch (TimeoutException) { }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            chart2.Series[0].Points.Clear();

            if (string.IsNullOrEmpty(txt_signal.Text))
                return;

            string[] ContentLines = txt_signal.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //忽略空行



            //chart1.Series[0]
            for (int i = 0; i < ContentLines.Length; i++)
            {
                //ContentLines[i];
                chart2.Series[0].Points.AddXY(i, ContentLines[i]);
            }
        }
    }
}
