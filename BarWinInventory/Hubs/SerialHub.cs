using BarWinInventory.Models;
using BarWinInventory.Utility;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace BarWinInventory.Hubs
{
    public class SerialHub : Hub
    {
        private SerialControl serialControl;
        private IHubContext<SerialHub> hubContext;
        private EventBusService eventBusService;

        public SerialHub(IHubContext<SerialHub> hubContext,SerialControl serialControl, EventBusService eventBusService)
        {
            this.serialControl = serialControl;
            this.hubContext = hubContext;
            this.eventBusService = eventBusService;
            serialControl.OnReceiveCommand += OnCommand;
        }

        ~SerialHub()
        {
            serialControl.OnReceiveCommand -= OnCommand;
        }

        private char ConvertASCII(int value)
        {
            return (char)value;
        }

        public void OnCommand(SerialControl serialControl)
        {
            string value = serialControl.RetrieveBuffer();
            if (value.Length > 0)
            {
                hubContext.Clients.All.SendAsync("ReceiveLog", value);
                // create eventbus msg
                var msg = new ScannerResultMessage(value, ScannerResult.Scanned);
                char[] s = new char[5];
                s[0] = 'c';
                s[1] = (char)0;
                s[2] = (char)0;
                s[3] = (char)255;
                s[4] = '\r';
                serialControl.WriteLog("Received barcode scan: " + value);
                serialControl.SendSerial(s);
                eventBusService.Publish("scanner", JsonConvert.SerializeObject(msg));
                serialControl.WriteLog("Send to event bus barcode: " + value);
            }

        }

        public async Task StartScan()
        {
            char[] bs = new char[2];
            char[] s = new char[5];
            s[0] = 'c';
            s[1] = (char)0;
            s[2] = (char)0;
            s[3] = (char)0;
            s[4] = '\r';
            bs[0] = 's';
            bs[1] = '\r';

            serialControl.SendSerial(s);
            serialControl.SendSerial(bs);
            serialControl.WriteLog("Start barcode scanner instruction");
        }

        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("ReceiveMessage", "system", "new connection");
            foreach(String entry in serialControl.log)
            {
                Clients.Caller.SendAsync("ReceiveLog", entry);
            }
            return base.OnConnectedAsync();
        }
    }
}
