using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetDoodleJump.ServiceGame;

namespace NetDoodleJump
{
    public partial class GameWindow : Form, IServiceGameCallback
    {
        public static int formWidth;
        public static int formHeight;
        public int id;
        public Player player;
        public Edge[] edges;
        public bool lockJump = false;
        public bool lockMove = false;
        public bool isConnected = false;
        Random rnd = new Random();
        public LoggerClass logger;
        public ServiceGameClient client;
        public GameWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);
            UpdateStyles();
            logger = new LoggerClass();
        }

        public void ConnectPlayer()
        {
            if (!isConnected)
            {
                id = client.Connect(name:"");
                isConnected = true;
            }
        }
        public void DisconnectPlayer()
        {
            if (isConnected)
            {
                client.Disconnect(id: id);
                isConnected = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            client = new ServiceGameClient(new System.ServiceModel.InstanceContext(this));
            ConnectPlayer();
            logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Старт игры");
            //this.Controls.Clear();
            btnStart.Enabled = false;
            btnStart.Visible = false;
            timerPaint.Enabled = true;
            timerGame.Enabled = true;
            player = new Player(logger, this.Width/2, 100);
            edges = CreateEdges();  //new Edge(this.Width / 2, 300);
            
            
        }
        public Edge[] CreateEdges()
        {
            logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Создание граней");
            Edge[] arr = new Edge[6];
            Edge edge;
            int x = GameWindow.formWidth / 2;
            int y = GameWindow.formHeight / 2;
            for (int i = 0; i < 6; i++)
            {
                edge = new Edge(x, y);
                edge.Counted = false;
                arr[i] = edge;
                if (x - Edge.Width > 100 && (GameWindow.formWidth - Edge.Width) - (x + Edge.Width) > 100)
                {
                    if (rnd.Next(0,1) == 0)
                        x = rnd.Next(0, x - Edge.Width);
                    else
                        x = rnd.Next(x + Edge.Width, GameWindow.formWidth - Edge.Width);

                } else
                {
                    if (x  - Edge.Width > 100)
                        x = rnd.Next(0, x - Edge.Width);
                    else if ((GameWindow.formWidth - Edge.Width) - (x + Edge.Width) > 100)
                        x = rnd.Next(x + Edge.Width, GameWindow.formWidth - Edge.Width);
                }
                y = y - 150;
            }
            return arr;
        }

        public Point GetNewPoint(int x, int y)
        {
            if (x - Edge.Width > 100 && (GameWindow.formWidth - Edge.Width) - (x + Edge.Width) > 100)
            {
                if (rnd.Next(0, 1) == 0)
                    x = rnd.Next(0, x - Edge.Width);
                else
                    x = rnd.Next(x + Edge.Width, GameWindow.formWidth - Edge.Width);

            }
            else
            {
                if (x - Edge.Width > 100)
                    x = rnd.Next(0, x - Edge.Width);
                else if ((GameWindow.formWidth - Edge.Width) - (x + Edge.Width) > 100)
                    x = rnd.Next(x + Edge.Width, GameWindow.formWidth - Edge.Width);
            }
            y = 0 - Edge.Height;
            return new Point(x, y);
        }

        private void GameWindow_Paint(object sender, PaintEventArgs e)
        {
            formWidth = this.Width;
            formHeight = this.Height;
            if (player != null && edges != null)
            {
                player.Draw(e.Graphics);
                edges.ToList<Edge>().ForEach(p => p.Draw(e.Graphics));
                timerGame.Enabled = true;
            } else
            {
                e.Graphics.Clear(Color.White);
            }
        }

        private void TimerPaint_Tick(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.SendPlayerInfo();
            }
            label1.Text = $"Score: {player.Score}";
            /*if (player.IsGameOver)
            {
                timerGame.Enabled = false;
                timerPaint.Enabled = false;
                MessageBox.Show("Игра окончена. Набрано: (" + player.Score + ") очков.");
                label1.Text = $"Score: {0}";
                btnStart.Visible = true;
                btnStart.Enabled = true;
                player = null;
                edges = null;
            }*/
            Refresh();
        }

        private void TimerGame_Tick(object sender, EventArgs e)
        {
            if (player == null)
                return;
            player.Gravity(edges, Edge.Width);

            Point p;
            foreach (Edge edge in edges)
            {
                edge.Move();
                if (edge.Y >= GameWindow.formHeight)
                {
                    p = GetNewPoint(edge.X, edge.Y);
                    edge.Counted = false;
                    edge.X = p.X;
                    edge.Y = p.Y;
                }
            }
        }

        private async void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (player == null)
                return;
            if (e.KeyCode == Keys.Up && player.isGravityOn)
            {
                logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Нажата клавиша прыжка");
                await Task.Run(()=>
                {
                    player.Jump(edges, Edge.Width);
                });
            } else if (!lockMove && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))
            {
                logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Нажата клавиша вбок");
                lockMove = true;
                await Task.Run(() =>
                {
                    player.Move(e.KeyCode);
                });
            }
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (player == null)
                return;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Клавиша движения вбок отпущена");
                player.StopMove = true;
                lockMove = false;
            }
        }

        private void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectPlayer();
            logger.CloseLog();
        }

        public void PlayerInfoCallback(object info)
        {
            throw new NotImplementedException();
        }
    }
}
