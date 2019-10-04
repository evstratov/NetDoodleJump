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
        public Player opponent;
        public Edge[] edges;
        public bool lockJump = false;
        public bool lockMove = false;
        public bool isConnected = false;
        Random rnd = new Random();
        public wcf_service.LoggerClass logger;
        public ServiceGameClient client;
        public GameWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);
            UpdateStyles();
            //logger = new wcf_service.LoggerClass();
        }

        public void ConnectPlayer()
        {
            if (!isConnected)
            {
                id = client.Connect("");
                isConnected = true;
            }
        }
        public void DisconnectPlayer()
        {
            try
            {
                if (isConnected)
                {
                    timerPaint.Enabled = false;
                    timerGame.Enabled = false;
                    client.Disconnect(id: id);
                    client.Close();
                }
            } catch
            {
                client.Abort();
            }
            finally
            {
                client = null;
                isConnected = false;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                client = new ServiceGameClient(new System.ServiceModel.InstanceContext(this));
                ConnectPlayer();
                // ожидание второго игрока
                await Task.Run(() => { while (!client.StartGame()) { } });
                player = new Player(this.Width / 2, 100, Resources.gamerTexture);
                opponent = new Player(this.Width / 2, 100, Resources.opponentTexture);
                edges = CreateEdges();
                //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Старт игры");
                //this.Controls.Clear();
                btnStart.Enabled = false;
                btnStart.Visible = false;
                timerPaint.Enabled = true;
                timerGame.Enabled = true;
            } catch
            {
                DisconnectPlayer();
            }
        }
        public Edge[] CreateEdges()
        {
            try {
                //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Создание граней");
                Edge[] arr = new Edge[6];
                Edge edge;
                int x = GameWindow.formWidth / 2;
                int y = GameWindow.formHeight / 2;
                for (int i = 0; i < 6; i++)
                {
                    edge = new Edge(x, y);
                    edge.Counted = false;
                    arr[i] = edge;
                    //if (x - Edge.Width > 100 && (formWidth - Edge.Width) - (x + Edge.Width) > 100)
                    //{
                    //    if (rnd.Next(0, 1) == 0)
                    //        x = rnd.Next(0, x - Edge.Width);
                    //    else
                    //        x = rnd.Next(x + Edge.Width, formWidth - Edge.Width);

                    //}
                    //else
                    //{
                    //    if (x - Edge.Width > 100)
                    //        x = rnd.Next(0, x - Edge.Width);
                    //    else if ((formWidth - Edge.Width) - (x + Edge.Width) > 100)
                    //        x = rnd.Next(x + Edge.Width, formWidth - Edge.Width);
                    //}
                    x = client.GetXcoordinate(id, x, GameWindow.formWidth, Edge.Width);
                    y = y - 150;
                }
                return arr;
            }
            catch (Exception ex)
            {
                DisconnectPlayer();
                return null;
            }

        }

        public Point GetNewPoint(int x, int y)
        {
            //if (x - Edge.Width > 100 && (formWidth - Edge.Width) - (x + Edge.Width) > 100)
            //{
            //    if (rnd.Next(0, 1) == 0)
            //        x = rnd.Next(0, x - Edge.Width);
            //    else
            //        x = rnd.Next(x + Edge.Width, formWidth - Edge.Width);

            //}
            //else
            //{
            //    if (x - Edge.Width > 100)
            //        x = rnd.Next(0, x - Edge.Width);
            //    else if ((formWidth - Edge.Width) - (x + Edge.Width) > 100)
            //        x = rnd.Next(x + Edge.Width, formWidth - Edge.Width);
            //}
            x = client.GetXcoordinate(id, x, GameWindow.formWidth, Edge.Width);
            y = 0 - Edge.Height;
            return new Point(x, y);
        }

        private void GameWindow_Paint(object sender, PaintEventArgs e)
        {
            formWidth = this.Width;
            formHeight = this.Height;
            if (opponent != null && player != null && edges != null)
            {
                opponent.Draw(e.Graphics);
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
            try {
                if (client != null)
                {
                    object[] info = new object[] { player.X, player.Y, player.Score };
                    client.SendPlayerInfo(info, id);
                }
                lb_score1.Text = $"You score: {player.Score}";
                lb_score2.Text = $"Opponent score: {opponent.Score}";
                if (player.IsGameOver)
                {
                    DisconnectPlayer();
                    MessageBox.Show("Поражение, вы разбились. Набрано: (" + player.Score + ") очков.");
                    btnStart.Visible = true;
                    btnStart.Enabled = true;
                    player = null;
                    opponent = null;
                    edges = null;
                }
                Refresh();
            } catch
            {
                DisconnectPlayer();
            }
        }

        private void TimerGame_Tick(object sender, EventArgs e)
        {
            try {
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
            } catch
            {
                DisconnectPlayer();
            }
        }

        private async void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try {
                if (player == null)
                    return;
                if (e.KeyCode == Keys.Up && player.isGravityOn)
                {
                    //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Нажата клавиша прыжка");
                    await Task.Run(() =>
                    {
                        player.Jump(edges, Edge.Width);
                    });
                } else if (!lockMove && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))
                {
                    //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Нажата клавиша вбок");
                    lockMove = true;
                    await Task.Run(() =>
                    {
                        player.Move(e.KeyCode);
                    });
                }
            }
            catch {
                DisconnectPlayer();
            }
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (player == null)
                return;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Клавиша движения вбок отпущена");
                player.StopMove = true;
                lockMove = false;
            }
        }

        private void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

            DisconnectPlayer();
            //GameOverCallback();
            //logger.CloseLog();
        }

        public void PlayerInfoCallback(object[] info)
        {
            try
            {
                opponent.X = (int)info[0];
                opponent.Y = (int)info[1];
                opponent.Score = (int)info[2];
            }
            catch
            {
                DisconnectPlayer();
            }
        }
        public void GameOverCallback()
        {
            try {
                timerGame.Enabled = false;
                timerPaint.Enabled = false;
                if (player.Score > opponent.Score)
                    MessageBox.Show("Вы выиграли! Набрано: (" + player.Score + ") очков.");
                else
                    MessageBox.Show("Вы проиграли! До победы не хватило: (" + (opponent.Score - player.Score) + ") очков.");
                //lb_score1.Text = $"You score: {0}";
                //lb_score2.Text = $"Opponent score: {0}";
                btnStart.Visible = true;
                btnStart.Enabled = true;
                player = null;
                opponent = null;
                edges = null;
                Refresh();
            }
            catch
            {
                DisconnectPlayer();
            }
        }

        //public void EdgeXCoordCallback(int x)
        //{
        //    x_callback = x;
        //}
    }
}
