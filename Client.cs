using System;
using System.Net.Sockets;
using System.Text;

namespace RallyServer
{
    internal class Client
    {
        private readonly Socket socket;

        public Socket Socket => socket;
        public bool Connected => socket.Connected;

        public Client(Socket socket)
        {
            this.socket = socket;
        }

        public void Disconnect() => socket.Disconnect(true);

        public int? AcceptInt()
        {
            try
            {
                var size = sizeof(int);
                var buffer = new byte[size];
                socket.Receive(buffer, buffer.Length, SocketFlags.None);
                return BitConverter.ToInt32(buffer, 0);
            }
            catch
            {
                return null;
            }
        }

        public float? AcceptFloat()
        {
            try
            {
                var size = sizeof(float);
                var buffer = new byte[size];
                socket.Receive(buffer, buffer.Length, SocketFlags.None);
                return BitConverter.ToSingle(buffer, 0);
            }
            catch
            {
                return null;
            }
        }

        public string AcceptString()
        {
            var size = AcceptInt();
            if (size != null)
            {
                var buffer = new byte[(int)size];
                socket.Receive(buffer, buffer.Length, SocketFlags.None);
                return Encoding.UTF8.GetString(buffer);
            }
            return null;
        }

        public bool SendInt(int number)
        {
            try
            {
                var buffer = BitConverter.GetBytes(number);
                socket.Send(buffer, buffer.Length, SocketFlags.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendFloat(float number)
        {
            try
            {
                var buffer = BitConverter.GetBytes(number);
                socket.Send(buffer, buffer.Length, SocketFlags.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendString(string line)
        {
            var buffer = Encoding.UTF8.GetBytes(line);
            if (SendInt(buffer.Length))
            {
                try
                {
                    socket.Send(buffer, buffer.Length, SocketFlags.None);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
