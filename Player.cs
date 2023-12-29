using System.Drawing;
using System.Net.Sockets;

namespace RallyServer
{
    internal class Player : Car
    {
        private readonly Client client;

        public PlayerState State { get; set; }

        public string Nick { private set; get; }
        public bool Connected => client.Connected;

        public Player(Socket socket)
        {
            client = new Client(socket);
            State = PlayerState.Waiting;
        }

        public virtual string GetNick()
        {
            if (!client.SendInt((int)Command.GetNick))
                return null;
            Nick = client.AcceptString();
            return Nick;
        }

        public virtual PointF? GetPosition()
        {
            if (!client.SendInt((int)Command.GetPosition))
                return null;
            var x = client.AcceptFloat();
            if (x == null)
                return null;
            var y = client.AcceptFloat();
            if (y == null)
                return null;
            SetPosition((float)x, (float)y);
            return new PointF((float)x, (float)y);
        }

        public virtual bool SetCarPosition(int index, PointF position)
        {
            if (!client.SendInt((int)Command.SetCarPosition))
                return false;
            if (!client.SendInt(index))
                return false;
            if (!client.SendFloat(position.X))
                return false;
            if (!client.SendFloat(position.Y))
                return false;
            return true;
        }

        public virtual bool SetCarColor(int index, int color)
        {
            if (!client.SendInt((int)Command.SetCarColor))
                return false;
            if (!client.SendInt(index))
                return false;
            if (!client.SendInt(color))
                return false;
            return true;
        }

        public virtual bool Start()
        {
            if (!client.SendInt((int)Command.Start))
                return false;
            return true;
        }

        public virtual bool SetOpponentPosition(PointF position)
        {
            if (!client.SendInt((int)Command.SetOpponentPosition))
                return false;
            if (!client.SendFloat(position.X))
                return false;
            if (!client.SendFloat(position.Y))
                return false;
            return true;
        }

        public virtual bool SetPosition(float y)
        {
            if (!client.SendInt((int)Command.SetPosition))
                return false;
            if (!client.SendFloat(y))
                return false;
            return true;
        }

        public virtual bool SGame()
        {
            if (!client.SendInt((int)Command.SGame))
                return false;
            return client.AcceptInt() == 1;
        }

        public virtual bool SetResultMP(bool win)
        {
            if (!client.SendInt((int)Command.SetResult))
                return false;
            if (!client.SendInt(win ? 1 : 0))
                return false;
            return true;
        }

        public virtual bool SetResult()
        {
            if (!client.SendInt((int)Command.SetResult))
                return false;
            return true;
        }
        public virtual bool SetScore(float score)
        {
            if (!client.SendInt((int)Command.SetScore))
                return false;
            if (!client.SendFloat(score))
                return false;
            return true;
        }

        public virtual bool Disconnect()
        {
            if (!client.SendInt((int)Command.Disconnect))
                return false;
            client.Disconnect();
            return true;
        }
    }

    public enum PlayerState
    { 
        Waiting,
        Playing
    }
}
