using BarWinInventory.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BarWinInventory.Utility
{
    public class SerialControl
    {
        public List<string> log { get; private set; }
        private SerialPort serialPort;
        private string buffer = "";
        public delegate void EventReceiveCommand(SerialControl serialControl);
        public EventReceiveCommand OnReceiveCommand { get; set; }
        public string RetrieveBuffer()
        {
            string value = (string)buffer.Clone();
            buffer = "";
            return value;

        }


        public SerialControl()
        {
            log = new List<string>();
            serialPort = new SerialPort();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0 || serialPort.IsOpen) throw new Exception("COM port not available");
            serialPort.BaudRate = 9600;
            serialPort.PortName = ports[0];
            serialPort.DataReceived += OnRead;
            serialPort.Open();

        }
        ~SerialControl()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private void OnRead(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead > 0)
            {
                char received = (char)serialPort.ReadChar();
                if(received == '\r')
                {
                    log.Add(buffer);
                    OnReceiveCommand.Invoke(this);
                }
                else
                {
                    buffer += received;
                }
            }

        }

        public void SendSerial(char[] data)
        {
            serialPort.Write(data,0,data.Length);
        }

        public void WriteLog(string text)
        {
            //hubContext.Clients.All.SendAsync("ReceiveLog", buffer);
            log.Add(text);
        }

        
    }
}
