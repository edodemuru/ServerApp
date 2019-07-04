using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerApp
{
    
    class SnifferServer:BackgroundWorker
    {

        private int numEsp32;
        private List<Device> esp32;

        //Property to modify number of Esp32
        public int NumEsp32 { get => numEsp32; set => numEsp32 = value; }
        public List<Device> Esp32 { get => esp32; set => esp32 = value; }

        public SnifferServer(List<Device> esp32List)
        {
            Esp32 = esp32List;
            numEsp32 = esp32List.Count;
        }

       public void Run()
        {
            try
            {
                //Set ip endpoint for socket
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.137.1"), 8888);
                // Create a TCP/IP socket.  
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and   
                // listen for incoming connections.  
                listener.Bind(endPoint);
                listener.Listen(10);

                //Data received through socket
                string dataReceived = "";
                //Data to send to esp 32
                string dataToSend = "";
                //Data parsed
                string[] dataReceivedParsed;
                //Number of esp32 devices
                int numEspTemp = 0;
                //Timestamp necessary for sync
                DateTime timestamp = new DateTime();
                DateTime timestampModified = new DateTime();
                //Mac of esp32 connected
                List<string> macEsp32 = new List<string>();

                //Get last id from db
                /*int firstIdDB = PacketFactory.Instance.GetPacketMaxId() + 1;
                int lastIdDB = firstIdDB;
                PacketFactory.Instance.NumEsp32 = numEsp32;*/

                //List of esp32
                //List<Device> esp32 = new List<Device>();
                /*Device esp0 = new Device(5, 5);
                Device esp1 = new Device(-5, 5);
                Device esp2 = new Device(-5, -5);
                Device esp3 = new Device(5, -5);
                esp32.Add(esp0);
                esp32.Add(esp1);
                esp32.Add(esp2);
                esp32.Add(esp3);*/
                //Define esp32 for position
               /* Device esp0 = new Device(10, 0);
                Device esp1 = new Device(-10, 0);
                Device esp2 = new Device(0, 10);
                esp32.Add(esp0);
                esp32.Add(esp1);
                esp32.Add(esp2);*/



                //Enter the listening loop
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    Socket handler = listener.Accept();
                    Console.WriteLine("Connected!");

                    int numberOfBytesRead = 0;
                    byte[] myReadBuffer = new byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int totalNum = 0;


                    // An incoming connection needs to be processed.  
                    while ((numberOfBytesRead = handler.Receive(myReadBuffer, myReadBuffer.Length, 0)) > 0)
                    {
                        totalNum += numberOfBytesRead;
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                        if (myCompleteMessage.ToString().EndsWith("EOF"))
                        {
                            break;
                        }
                    }

                    dataReceived = myCompleteMessage.ToString();
                    Console.WriteLine("Dimensione dati: " + totalNum);

                    if (dataReceived != null)
                    {
                        //Parse packets
                        dataReceivedParsed = dataReceived.Split(';');

                        Console.WriteLine(String.Format("Received: {0}", dataReceived));

                        //First Connection
                        if (dataReceivedParsed[1] == "R")
                        {
                            //No esp32 connected
                            if (macEsp32.Count == 0)
                            {
                                //This is the first esp32 to connect to Server, so I calculate the timestamp
                                Console.WriteLine("First Connection");
                                timestamp = GetNetworkTime();
                                Console.WriteLine("Current time " + timestamp.ToString());
                                timestampModified = timestamp.AddMinutes(0.2);
                                Console.WriteLine("Time when esp32 will start working " + timestampModified.ToString());

                                macEsp32.Add(dataReceivedParsed[0]);
                                Esp32[macEsp32.Count - 1].Mac = dataReceivedParsed[0];


                            }
                            //List has at least one element
                            else
                            {
                                Console.WriteLine("Other Connection");
                                //Mac is not inside list
                                if (!macEsp32.Contains(dataReceivedParsed[0]))
                                {
                                    macEsp32.Add(dataReceivedParsed[0]);

                                    Esp32[macEsp32.Count - 1].Mac = dataReceivedParsed[0];
                                    if (macEsp32.Count == numEsp32)
                                    {
                                        //Sort all esp32 devices based on mac
                                        Esp32.Sort();
                                        //Report that all esp32 connected to server
                                        ReportProgress(1);
                                        
                                    }

                                }
                                //Mac is inside list
                                else
                                {
                                    Console.WriteLine("Esp32 riconnected");
                                    timestamp = GetNetworkTime();
                                    timestampModified = timestamp.AddMinutes(1.5);
                                    numEspTemp = 1;
                                }
                            }
                            dataToSend = timestampModified.ToString();
                            Console.WriteLine("Esp32 Connected:");
                            foreach (var item in macEsp32)
                            {
                                Console.WriteLine(item.ToString());
                            }

                        }
                        

                    }

                 }


            } catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }

        }
            

        public static DateTime GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var attempts = 0;
                var times = 4;
                var delay = 1000;
                do
                {
                    try
                    {
                        attempts++;
                        socket.Connect(ipEndPoint);

                        //Stops code hang if NTP is blocked
                        socket.ReceiveTimeout = 3000;

                        socket.Send(ntpData);
                        socket.Receive(ntpData);
                        socket.Close();
                        break;
                    }
                    catch (SocketException ex)
                    {
                        if (attempts == times)
                            throw;
                        Console.WriteLine("Exception" + ex.Message + " caught on attempt " + attempts + " will retry after delay");

                        Task.Delay(delay).Wait();
                    }
                } while (true);
                /* try
                 {
                     socket.Connect(ipEndPoint);

                     //Stops code hang if NTP is blocked
                     socket.ReceiveTimeout = 3000;

                     socket.Send(ntpData);
                     socket.Receive(ntpData);
                     socket.Close();
                 }
                 catch
                 {

                 }*/
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        static String ReadData(NetworkStream stream)
        {
            byte[] msgReceivedBytes = new byte[1024];
            MemoryStream ms = new MemoryStream();
            StringBuilder dataComplete = new StringBuilder();
            int numBytesRead;
            while ((numBytesRead = stream.Read(msgReceivedBytes, 0, msgReceivedBytes.Length)) > 0)
            {
                ms.Write(msgReceivedBytes, 0, numBytesRead);


            }

            String data = System.Text.Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);

            Console.WriteLine("Lunghezza dati: " + data.Length);
            return data;
        }

        static double GetDistanceFromRssi(double rssi)
        {
            //double measuredPower = -39 -45 -51 -69 -42;
            double measuredPower = -42;
            double n = 2;
            //double n = 4;
            return Math.Pow(10, (measuredPower - rssi) / (10 * n));
        }

      

       

        
    }

    
}
