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
        private int width, height;
        protected Bitmap texture;

        public int Width
        {
            get { return width; }
            protected set { width = value; }
        }
        public int Height
        {
            get { return height; }
            protected set { height = value; }
        }

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


        public virtual void Draw(Graphics graphics)
        {
            graphics.DrawImage(texture, new Rectangle(X, Y, Width, Height));
        }
    }

    public class Player:Game
    {
        public bool isGravityOn = true;
        public Player (int x, int y)
        {
            X = x;
            Y = y;
            texture = Resources.gamerTexture;
            Width = texture.Width;
            Height = texture.Height;

        }
        public void Jump(int x_edge, int y_edge, int length)
        {
            isGravityOn = false;
            for (int i = 1; i <= 300; i++)
            {
                if (Y == y_edge + 1 && ((Math.Abs(X - x_edge) < 40) || (Math.Abs((X + Width) - (x_edge + length)) < 40)))
                    break;
                Y--;
                i++;
                Thread.Sleep(2);
            }
            isGravityOn = true;  
        }
        public void Move(Keys key)
        {
            StopMove = false;
            switch (key)
            {
                case Keys.Right:
                    
                    do
                    {
                        X += 1;
                        Thread.Sleep(3);
                    } while (!StopMove && ((X + Width) < GameWindow.formWidth));
                    StopMove = false;

                    break;
                case Keys.Left:
                    do
                    {
                        X -= 1;
                        Thread.Sleep(3);
                    } while (!StopMove && X > 0);
                    StopMove = false;

                    break;
            }
        }
        public void Gravity(int x_edge, int y_edge, int length)
        {
            for(int i = 0; i < 3; i++) 
            {
                if (Y + 2 * Height >= GameWindow.formHeight)
                    return;
                if (Y + Height == y_edge && ((Math.Abs(X - x_edge) < 40) || (Math.Abs((X + Width) - (x_edge + length)) < 40)))
                    return;
                Y++;
            }
        }
    }
    public class Edge:Game
    {
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
    }
}
