using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ImaTestLib.ClassExtensions
{
    /// <summary>
    ///     Extensions to Socket class
    /// </summary>
    public static class SocketExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static string GetIPFromHostName(this string hostname)
        {
            var addresslist = Dns.GetHostAddresses(hostname);
            return addresslist[0].ToString();
        }


        /// <summary>
        ///     Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeoutMS"></param>
        public static bool Connect(this Socket socket, string host, string port, int timeoutMS)
        {
            return Connect(socket, host, int.Parse(port), new TimeSpan(0, 0, 0, 0, timeoutMS));
        }

        /// <summary>
        ///     Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeoutMS"></param>
        public static bool Connect(this Socket socket, string host, int port, int timeoutMS)
        {
            return Connect(socket, host, port, new TimeSpan(0, 0, 0, 0, timeoutMS));
        }

        /// <summary>
        ///     Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        public static bool Connect(this Socket socket, string host, int port, TimeSpan timeout)
        {
            return AsyncConnect(socket, (s, a, o) => s.BeginConnect(host, port, a, o), timeout);
        }

        /// <summary>
        ///     Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="addresses">The addresses.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        public static bool Connect(this Socket socket, IPAddress[] addresses, int port, TimeSpan timeout)
        {
            return AsyncConnect(socket, (s, a, o) => s.BeginConnect(addresses, port, a, o), timeout);
        }

        /// <summary>
        ///     Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="endpoint">The IP endpoint.</param>
        /// <param name="timeout">The timeout.</param>
        public static void Connect(this Socket socket, EndPoint endpoint, TimeSpan timeout)
        {
            var result = socket.BeginConnect(endpoint, null, null);

            var success = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (success)
                socket.EndConnect(result);
            else
            {
                socket.Close();
                throw new SocketException(10060); // Connection timed out.
            }
        }

        /// <summary>
        ///     Asynchronous connect.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="connect">The connect.</param>
        /// <param name="timeout">The timeout.</param>
        private static bool AsyncConnect(Socket socket, Func<Socket, AsyncCallback, object, IAsyncResult> connect, TimeSpan timeout)
        {
            try
            {
                var asyncResult = connect(socket, null, null);
                while (!asyncResult.IsCompleted)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                    Thread.Yield();
                }
                if (asyncResult.AsyncWaitHandle.WaitOne(timeout))
                {
                    socket.EndConnect(asyncResult);
                    return true;
                }
                socket.EndConnect(asyncResult);
                return false;
            }
            catch (SocketException)
            {
                //Form1.Console_WriteLine("Connecting to CLI at {0}:{1}", IP, port);
                //Logger.LOG_ERROR("SocketException: {0}", ex.Message);
            }
            return false;
        }
    }
}