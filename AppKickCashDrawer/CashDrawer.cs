using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Drawer
{
    public class CashDrawer
    {
        public SerialPort ComPort;
        private readonly string _name;

        public static string GS = Convert.ToString((char)29);
        public static string ESC = Convert.ToString((char)27);

        #region Mechanism Control Command

        //Kick Drawer
        // Different depending on the type of printer, hardcode for now, for more visit http://keyhut.com/popopen.htm
        public static string OPENDRAWER = ESC + Encoding.ASCII.GetString(new byte[] { 112, 0, 25, 250 });

        public static string OPENDRAWERBYCOMPORT = Encoding.ASCII.GetString(new byte[] { 23, 112, 0, 25, 250 });

        // DLE EOT 1
        public static string CASHDRAWERSTATUS = Encoding.ASCII.GetString(new byte[] { 16, 4, 1 });

        public static string CASHDRAWEROPENED = Encoding.ASCII.GetString(new byte[] { 50 });

        public static string CASHDRAWERCLOSED = Encoding.ASCII.GetString(new byte[] { 54 });

        #endregion Mechanism Control Command

        #region Miscellaneous Command

        public static string INITIALIZEPRINTER = ESC + "@";

        #endregion Miscellaneous Command

        public CashDrawer(string name)
        {
            _name = name;
            OpenDeviceWithCOMPort(_name);
        }

        public bool OpenCashdrawer()
        {
            try
            {
                if (string.IsNullOrEmpty(_name)) return false;
                var listCommandCashDrawer =
                    new List<string>
                    {
                        OPENDRAWERBYCOMPORT,
                        OPENDRAWER
                    };
                return OpenCashDrawer(listCommandCashDrawer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open Cashd rawer Failed" + ex);
                return false;
            }
        }

        public bool IsCashdrawerOpening()
        {
            return GetCashdrawerStatus() == 0;
        }

        public void OpenDeviceWithCOMPort(string comPortName)
        {
            if (string.IsNullOrEmpty(comPortName)) return;

            try
            {
                if (ComPort == null)
                {
                    ComPort = new SerialPort
                    {
                        PortName = comPortName,
                        BaudRate = 9600,
                        StopBits = StopBits.One,
                        DataBits = 8,
                        Parity = Parity.None
                    };
                }
                else
                {
                    ComPort.PortName = comPortName;
                }

                ComPort?.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open COM Port Failed: " + ex);
            }
        }

        public void CloseDeviceWithCOMPort()
        {
            try
            {
                if (ComPort?.IsOpen == true) ComPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Close COM Port Failed: " + ex);
            }
            finally
            {
                ComPort.Dispose();
            }
        }

        private int GetCashdrawerStatus()
        {
            if (string.IsNullOrEmpty(_name)) return -1;
            return GetCashDrawerByComPortStatus();
        }

        private int GetCashDrawerByComPortStatus()
        {
            try
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("### Get Status Cash Drawer ###");
                if (!ComPort.IsOpen) ComPort.Open();

                if (ComPort.IsOpen)
                {
                    var cmd = CASHDRAWERSTATUS;
                    messageBuilder.AppendLine("### Request ###");
                    messageBuilder.AppendLine($"Details: '{cmd}'");
                    messageBuilder.AppendLine();
                    ComPort.WriteLine(cmd);
                }

                //Need to add deplay time to make sure can receive data from device
                var taskDelay = Task.Run(async delegate
                {
                    await Task.Delay(100);
                });
                taskDelay.Wait();

                messageBuilder.AppendLine("### Response ###");
                var bytes = ComPort.BytesToRead;
                messageBuilder.AppendLine($"BytesToRead: '{bytes}'");
                var comBuffer = new byte[bytes];
                messageBuilder.AppendLine($"ComBuffer: '{string.Join(", ", comBuffer)}'");
                var readRespone = ComPort.Read(comBuffer, 0, bytes);
                messageBuilder.AppendLine($"ComPortRead: '{readRespone}'");
                var getString = Encoding.ASCII.GetString(comBuffer);
                messageBuilder.AppendLine($"GetString: '{(getString == null ? "NULL" : getString == "" ? "EMPTY" : getString)}'");
                MessageBox.Show(messageBuilder.ToString());
                var cashDrawerStatus = getString?.Equals(CASHDRAWEROPENED) == true ? 0 : 1;
                return cashDrawerStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Get CashDrawer Status Failed" + ex);
                return -1;
            }
            finally
            {
                CloseDeviceWithCOMPort();
            }
        }

        private bool OpenCashDrawer(List<string> cmds)
        {
            try
            {
                if (ComPort == null) OpenDeviceWithCOMPort(_name);
                if (ComPort.IsOpen == false) ComPort.Open();
                if (ComPort.IsOpen)
                {
                    foreach (var cmd in cmds)
                    {
                        if (string.IsNullOrWhiteSpace(cmd)) continue;
                        ComPort.WriteLine(cmd);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open Cash Drawer Failed: " + ex);
                return false;
            }
            finally
            {
                CloseDeviceWithCOMPort();
            }
        }
    }
}
