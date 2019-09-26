using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetDoodleJump
{
    public class Game
    {
        protected Bitmap texture;

        public int X
        {
            get; set;
        }
        public int Y
        {
            get; set;
        }
        public bool StopMove
        {
            get;
            set;
        }
    }

    public class Player:Game
    {
        private static int width, height;

        public static int Width
        {
            get { return width; }
            private set { width = value; }
        }
        public static int Height
        {
            get { return height; }
            private set { height = value; }
        }
        public bool isGravityOn = true;
        public Player (int x, int y)
        {
            X = x;
            Y = y;
            texture = Resources.gamerTexture;
            Width = texture.Width;
            Height = texture.Height;

        }
        public void Jump(Edge[] edges, int length)
        {
            isGravityOn = false;
            for (int i = 1; i <= 500; i++)
            {
                if (IsStayOrHitOnEdge(edges, length, true))
                    break;
                Y--;
                i++;
                Thread.Sleep(1);
            }
            //isGravityOn = true;
        }
        public void Move(Keys key)
        {
            StopMove = false;
            switch (key)
            {
                case Keys.Right:
                    
                    do
                    {
                        X += 2;
                        Thread.Sleep(3);
                    } while (!StopMove && ((X + Width) < GameWindow.formWidth));
                    StopMove = false;

                    break;
                case Keys.Left:
                    do
                    {
                        X -= 2;
                        Thread.Sleep(3);
                    } while (!StopMove && X > 0);
                    StopMove = false;

                    break;
            }
        }
        public void Gravity(Edge[] edges, int length)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Y + 2 * Height >= GameWindow.formHeight)
                {
                    isGravityOn = true;
                    return;
                }
                if (IsStayOrHitOnEdge(edges, length, false))
                {
                    isGravityOn = true;
                    return;
                }
                Y++;
            }
        }
        private bool IsStayOrHitOnEdge(Edge[] edges, int length, bool isJump)
        {
            foreach(Edge e in edges)
            {
                if (!isJump)
                {
                    if (Y + Height == e.Y && ((Math.Abs(X - e.X) < 40) || (Math.Abs((X + Width) - (e.X + length)) < 40)))
                        return true;
                } else
                {
                    if (Y == e.Y + 1 && ((Math.Abs(X - e.X) < 40) || (Math.Abs((X + Width) - (e.X + length)) < 40)))
                        return true;
                }
            }
            return false;
        }
        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(texture, new Rectangle(X, Y, Width, Height));
        }
    }
    public class Edge:Game
    {
        private static int width, height;

        public static int Width
        {
            get { return width; }
            private set { width = value; }
        }
        public static int Height
        {
            get { return height; }
            private set { height = value; }
        }
        public Edge(int x, int y)
        {
            X = x;
            Y = y;
            texture = Resources.edgeTexture;
            Width = texture.Width;
            Height = texture.Height;
        }
        public void Move()
        { 
            Y++;
        }
        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(texture, new Rectangle(X, Y, Width, Height));
        }
    }
}
