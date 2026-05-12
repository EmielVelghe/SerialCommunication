using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        private SerialPort serialPortArduino;

        public Form1()
        {
            InitializeComponent();
            serialPortArduino = new SerialPort();
            serialPortArduino.ReadTimeout = 1000;
            serialPortArduino.WriteTimeout = 1000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.Close();
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    labelStatus.Text = "Disconnected";
                }
                else
                {
                    // Set serial port properties from UI
                    serialPortArduino.PortName = comboBoxPoort.SelectedItem != null ? comboBoxPoort.SelectedItem.ToString() : comboBoxPoort.Text;
                    int baud;
                    if (!int.TryParse(comboBoxBaudrate.SelectedItem != null ? comboBoxBaudrate.SelectedItem.ToString() : comboBoxBaudrate.Text, out baud)) baud = 115200;
                    serialPortArduino.BaudRate = baud;
                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    // Parity
                    if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
                    else if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

                    // Stop bits
                    if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;
                    else serialPortArduino.StopBits = StopBits.None;

                    // Handshake
                    if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                    else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;

                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    serialPortArduino.NewLine = "\n";

                    serialPortArduino.Open();
                    // Wait ~2s to allow Arduino to reset after opening the port
                    System.Threading.Thread.Sleep(2000);

                    // Ping the Arduino and expect "pong"
                    serialPortArduino.WriteLine("ping");
                    string response = string.Empty;
                    try { response = serialPortArduino.ReadLine().Trim(); } catch { response = string.Empty; }

                    if (response == "pong")
                    {
                        radioButtonVerbonden.Checked = true;
                        buttonConnect.Text = "Disconnect";
                        labelStatus.Text = "Connected to " + serialPortArduino.PortName;
                    }
                    else
                    {
                        labelStatus.Text = "Unexpected response: " + response;
                        serialPortArduino.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void checkBoxDigital2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital2.Checked ? "set d2 high" : "set d2 low";
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }

        private void checkBoxDigital3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital3.Checked ? "set d3 high" : "set d3 low";
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }

        private void checkBoxDigital4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = checkBoxDigital4.Checked ? "set d4 high" : "set d4 low";
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }

        private void trackBarPWM9_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm9 " + trackBarPWM9.Value;
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }

        private void trackBarPWM10_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm10 " + trackBarPWM10.Value;
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }

        private void trackBarPWM11_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string command = "set pwm11 " + trackBarPWM11.Value;
                    serialPortArduino.WriteLine(command);
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error sending command: " + ex.Message;
            }
        }
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl.SelectedTab == tabPageOefening3)
                {
                    timerOefening3.Enabled = true;
                }
                else
                {
                    timerOefening3.Enabled = false;
                }

                if (tabControl.SelectedTab == tabPageOefening4)
                {
                    timerOefening4.Enabled = true;
                }
                else
                {
                    timerOefening4.Enabled = false;
                }

                if (tabControl.SelectedTab == tabPageOefening5)
                {
                    timerOefening5.Enabled = true;
                }
                else
                {
                    timerOefening5.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void timerOefening3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    // Clear any previous data
                    try { serialPortArduino.ReadExisting(); } catch { }

                    // Request and parse digital5
                    serialPortArduino.WriteLine("get d5");
                    string resp = string.Empty;
                    try { resp = serialPortArduino.ReadLine().Trim(); } catch { resp = string.Empty; }
                    string value = resp;
                    int idx = value.IndexOf(':');
                    if (idx >= 0) value = value.Substring(idx + 1).Trim();
                    radioButtonDigital5.Checked = (value == "1");

                    // Request and parse digital6
                    serialPortArduino.WriteLine("get d6");
                    try { resp = serialPortArduino.ReadLine().Trim(); } catch { resp = string.Empty; }
                    value = resp;
                    idx = value.IndexOf(':');
                    if (idx >= 0) value = value.Substring(idx + 1).Trim();
                    radioButtonDigital6.Checked = (value == "1");

                    // Request and parse digital7
                    serialPortArduino.WriteLine("get d7");
                    try { resp = serialPortArduino.ReadLine().Trim(); } catch { resp = string.Empty; }
                    value = resp;
                    idx = value.IndexOf(':');
                    if (idx >= 0) value = value.Substring(idx + 1).Trim();
                    radioButtonDigital7.Checked = (value == "1");
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void timerOefening4_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    // Clear any previous data
                    try { serialPortArduino.ReadExisting(); } catch { }
                    // Request analog 0 value
                    serialPortArduino.WriteLine("get a0");
                    string resp = string.Empty;
                    try { resp = serialPortArduino.ReadLine().Trim(); } catch { resp = string.Empty; }
                    string value = resp;
                    int idx = value.IndexOf(':');
                    if (idx >= 0) value = value.Substring(idx + 1).Trim();
                    // Update UI label
                    labelAnalog0.Text = value;
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void timerOefening5_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino == null || !serialPortArduino.IsOpen)
                {
                    labelStatus.Text = "Not connected";
                    return;
                }

                // Clear previous data
                try { serialPortArduino.ReadExisting(); } catch { }

                // Read desired temperature from analog 0 (0..1023 -> 5..45 °C)
                serialPortArduino.WriteLine("get a0");
                string respA0 = string.Empty;
                try { respA0 = serialPortArduino.ReadLine().Trim(); } catch { respA0 = string.Empty; }
                string valA0 = respA0;
                int idxA0 = valA0.IndexOf(':');
                if (idxA0 >= 0) valA0 = valA0.Substring(idxA0 + 1).Trim();
                int rawA0 = 0; int.TryParse(valA0, out rawA0);
                double mA0 = 40.0 / 1023.0; // slope for 5..45
                double desiredTemp = mA0 * rawA0 + 5.0;
                // Read current temperature from analog 1 (0..1023 -> 0..500 °C)
                serialPortArduino.WriteLine("get a1");
                string respA1 = string.Empty;
                try { respA1 = serialPortArduino.ReadLine().Trim(); } catch { respA1 = string.Empty; }
                string valA1 = respA1;
                int idxA1 = valA1.IndexOf(':');
                if (idxA1 >= 0) valA1 = valA1.Substring(idxA1 + 1).Trim();
                int rawA1 = 0; int.TryParse(valA1, out rawA1);
                double mA1 = 500.0 / 1023.0;
                double currentTemp = mA1 * rawA1;
                // Update UI (rounded to 1 decimal)
                labelGewensteTemp.Text = Math.Round(desiredTemp, 1).ToString("0.0") + " °C";
                labelHuidigeTemp.Text = Math.Round(currentTemp, 1).ToString("0.0") + " °C";
                // Control LED on digital pin 2: ON when current < desired
                if (currentTemp < desiredTemp)
                {
                    serialPortArduino.WriteLine("set d2 high");
                }
                else
                {
                    serialPortArduino.WriteLine("set d2 low");
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }
    }
}
