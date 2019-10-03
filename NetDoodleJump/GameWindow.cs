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
            if (isConnected)
            {
                client.Disconnect(id: id);
                client.Close();
                isConnected = false;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            client = new ServiceGameClient(new System.ServiceModel.InstanceContext(this));
            ConnectPlayer(); 
            // ожидание второго игрока
            await Task.Run(() =>{while (!client.StartGame()) { }});
            player = new Player(logger, this.Width / 2, 100);
            opponent = new Player(logger, this.Width / 2, 100);
            edges = CreateEdges();
            //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Старт игры");
            //this.Controls.Clear();
            btnStart.Enabled = false;
            btnStart.Visible = false;
            timerPaint.Enabled = true;
            timerGame.Enabled = true;
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
                    x =client.GetXcoordinate(id, x, GameWindow.formWidth, Edge.Width);
                    y = y - 150;
                }
                return arr;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //logger.CloseLog();
            }
            return null;
        }

        public Point GetNewPoint(int x, int y)
        {
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
            if (client != null)
            {
                object[] info = new object[] { player.X, player.Y, player.Score};
                client.SendPlayerInfo(info, id);
            }
            lb_score1.Text = $"You score: {player.Score}";
            lb_score2.Text = $"Opponent score: {opponent.Score}";
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
                //logger.WriteLog($"{DateTime.Now.ToString("H.mm.ss.fff")} Нажата клавиша прыжка");
                await Task.Run(()=>
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
            opponent.X = (int)info[0];
            opponent.Y = (int)info[1];
            opponent.Score = (int)info[2];
        }
        public void GameOverCallback()
        {
            timerGame.Enabled = false;
            timerPaint.Enabled = false;
            if (player.Score > opponent.Score)
                MessageBox.Show("Вы выиграли! Набрано: (" + player.Score + ") очков.");
            else
                MessageBox.Show("Вы проиграли! До победы не хватило: (" + (opponent.Score - player.Score) + ") очков.");
            lb_score1.Text = $"You score: {0}";
            lb_score2.Text = $"Opponent score: {0}";
            btnStart.Visible = true;
            btnStart.Enabled = true;
            player = null;
            opponent = null;
            edges = null;
        }

        //public void EdgeXCoordCallback(int x)
        //{
        //    x_callback = x;
        //}
    }
}
