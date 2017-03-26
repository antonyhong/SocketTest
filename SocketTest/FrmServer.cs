using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServer
{
    public partial class FrmServer : Form
    {
        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            

            Task.Factory.StartNew(() => {
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 0);

                listener.Bind(new IPEndPoint(IPAddress.Any,2112));
                listener.Listen(Int32.MaxValue);
                

                while (true) {
                    
                    byte[] receiveBytes = new byte[1024];
                    Socket clientSocket = listener.Accept();
                    int numBytes = clientSocket.Receive(receiveBytes);

                    SocketAsyncEventArgs asynEvent = new SocketAsyncEventArgs();
                    //asynEvent.Buffer = new byte[1024];
                    
                    clientSocket.ReceiveAsync(asynEvent);

                    

                    string receiveStr = null;

                    while(true){
                        receiveStr += Encoding.UTF8.GetString(receiveBytes,0,numBytes);
                        if (receiveStr.IndexOf("[FINAL]") >-1) {
                            break;
                        }
                    }

                    Console.WriteLine("ReceivData:{0}",receiveStr);

                    string replayValue = "reply from server: " + receiveStr;
                    byte[] replayBytes = Encoding.UTF8.GetBytes(replayValue);
                    clientSocket.Send(replayBytes);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            });
            
 
            //Task.WaitAll
        }
       
    }
}
