///////////////////////////////////////////////////////////////
// IO.cs - SatTrax
// Perform scheduling and execution of satellite tracking.
// The program obtains a catalog of satellites and TLE data for a set of satellites.
// It predicts and produces a pass ephemeris script and connects to an MXP with a command set

// Version .01
// Author Michael Ketcham
//
// 01/27/2016 - Initial
//
// Copyright Sea Tel @ 2016 doing business as Cobham SATCOM
//////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ImaTestLib.ClassExtensions;

namespace SatTraxGUI
{
    public sealed class IO
    {
        private readonly Form1 Form1;

        private bool ConnectedCLI { get; set; }
        private bool LoggedInCLI { get; set; }
        private TcpClient TcpClientCLI { get; set; }
        private NetworkStream StreamCLI { get; set; }

        public IO(Form1 form)
        {
            Form1 = form;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        internal bool ConnectAndLoginCLI(string IP = "10.1.1.100", string port = "2003")
        {
            if (ConnectedCLI)
                return true;

            Form1.Console_WriteLine("Connecting to CLI at {0}:{1}", IP, port);

            try
            {
                TcpClientCLI = new TcpClient {SendTimeout = 500};
                if (!TcpClientCLI.Client.Connect(IP, port, 10000))
                {
                    Form1.Console_WriteLine("Failed to connect to CLI {0}:{1}", IP, port);
                    Form1.Console_WriteLine("\nIs the port already open?");
                    ConnectedCLI = false;
                    return ConnectedCLI;
                }
                StreamCLI = TcpClientCLI.GetStream();
                StreamCLI.ReadTimeout = 500;
                ConnectedCLI = true;
            }
            catch (SocketException e)
            {
                Form1.Console_WriteLine("Failed to connect to CLI {0}:{1} {2}", IP, port, e.Message);
                Form1.Console_WriteLine("\nIs the port already open?");
                ConnectedCLI = false;
                return ConnectedCLI;
            }

            if (!LoggedInCLI)
            {
                LoggedInCLI = LogInCLI();
            }

            WaitForPrompt("CLI");
            StreamCLI.Flush();

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        internal string WaitResponseCLI(string interfaceMode = "CLI", int timeout_ms = 2500)
        {
            var timeout = timeout_ms > 0 ? timeout_ms : int.MaxValue;
            var sw = Stopwatch.StartNew();
            string response;
            do
            {
                response = Read(StreamCLI).Trim();
                Console.WriteLine("Read '{0}'", response);

                if (response.Contains(">")) break;

                Write(interfaceMode, "\r");
                Thread.Sleep(1);
            } while (sw.ElapsedMilliseconds < timeout);

            sw.Stop();

            if (response.Contains(">"))
            {
                var response0 = response.Replace('>', ' ').Trim();
                return response0;
            }

            return null; // timeout
        }

        /// <summary>
        /// </summary>
        /// <param name="interfaceMode"></param>
        private void WaitForPrompt(string interfaceMode)
        {
            NetworkStream stream;
            string kick;

            switch (interfaceMode)
            {
                case "CLI":
                    stream = StreamCLI;
                    kick = "\r\n";
                    Write(interfaceMode, kick);
                    break;
                default:
                    Form1.Console_WriteLine("Invalid interface: {0}", interfaceMode);
                    return;
            }

            var pos = -1;
            do
            {
                // wait for ">"
                var response = Read(stream);
                if (string.IsNullOrWhiteSpace(response))
                {
                    Write(interfaceMode, kick);
                    continue;
                }

                pos = response.IndexOf(">");
            } while (pos < 0);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private bool LogInCLI()
        {
            if (!ConnectedCLI)
            {
                return false;
            }

            const string interfaceMode = "CLI";

            Form1.Console_WriteLine("Logging on CLI");

            var stuff = "";
            while (!stuff.Contains("Username:"))
            {
                stuff = Read(StreamCLI);
            }
            StreamCLI.Flush();

            Write(interfaceMode, "Dealer");

            while (!stuff.Contains("Password:"))
            {
                stuff = Read(StreamCLI);
            }
            Write(interfaceMode, "seatel3");
            StreamCLI.Flush();

            WaitForPrompt(interfaceMode);

            Write(interfaceMode, "SET ERROR MESSAGES ALWAYSOFF");

            StreamCLI.Flush();

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// </returns>
        private string Read(NetworkStream stream, int timeout = 1000)
        {
            //            Form1.Console_WriteLine("Reading...");

            var bytes = ReadBytes(stream, timeout);
            var bytes1 = TrimZeros(bytes);
            var response = new ASCIIEncoding().GetString(bytes1, 0, bytes1.Length);
            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// </returns>
        private byte[] ReadBytes(NetworkStream stream, int timeout = 1000)
        {
            if (stream == null)
                return new byte[0];

            var buffer = new byte[1024];
            var message = new StringBuilder();

            var failcount = 0;
            do
            {
                try
                {
                    //blocks until a client sends a message
                    stream.ReadTimeout = 1500;
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    message.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, bytesRead));

                    // LOG.Log(message.ToString());
                }
                catch (SocketException e)
                {
                    Form1.Console_WriteLine("  Read Exception: {0}", e.Message);
                    break;
                }
                catch (IOException e)
                {
                    // Unable to read data from the transport connection:
                    // A connection attempt failed because the connected party did not properly respond
                    // after a period of time,
                    // or established connection failed because connected host has failed to respond.
                    ++failcount;

                    if (failcount < 5)
                    {
                        Form1.Console_WriteLine(" waiting...");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Form1.Console_WriteLine("  Read Exception: {0}", e.Message);
                    Console.WriteLine("Read failed 5 times");

                    break;
                }
            } while (stream.DataAvailable);

            return Encoding.ASCII.GetBytes(message.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="interfaceMode"></param>
        /// <param name="cmd"></param>
        internal void Write(string interfaceMode, string cmd)
        {
            if (StreamCLI == null)
                return;

            //            LOG.Log("{0}", cmd);

            cmd += "\r\n";

            var message = new byte[cmd.ToCharArray().Length].ToArray();
            for (int i = 0, j = 0; i < message.Length; i++, j++)
            {
                if (cmd.Substring(i).StartsWith(@"\x"))
                {
                    try
                    {
                        message[j] = byte.Parse(cmd.Substring(i + 2, 2), NumberStyles.AllowHexSpecifier);
                        i = i + 3;
                    }
                    catch (Exception e)
                    {
                        Form1.Console_WriteLine("Invalid hex conversion cmd {0} {1}", cmd, e);
                        return;
                    }
                }
                else
                {
                    message[j] = (byte) ( cmd.Substring(i, 1)[0] & 0xFF );
                }
            }

            try
            {
                Console.WriteLine("Send '{0}'", cmd.Trim());
                switch (interfaceMode)
                {
                    case "CLI":
                        StreamCLI.Write(message, 0, message.Length);
                        break;

                    default:
                        //Form1.Console_WriteLine("Invalid interface : {0}", interfaceMode);
                        return;
                }
            }
            catch (SocketException e)
            {
                Form1.Console_WriteLine("  Failed write {0} {2}", interfaceMode, e.Message);
            }
            catch (Exception e)
            {
                Form1.Console_WriteLine();
                Form1.Console_WriteLine("  Failed write {0} {1}", interfaceMode, e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// </returns>
        private string StripCRLF(string text)
        {
            return text.Replace('\r', ' ').Replace('\n', ' ').Trim();
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>
        /// </returns>
        private static byte[] TrimZeros(byte[] bytes)
        {
            var lastIndex = Array.FindLastIndex(bytes, b => b != 0);
            Array.Resize(ref bytes, lastIndex + 1);
            return bytes;
        }
    }
}