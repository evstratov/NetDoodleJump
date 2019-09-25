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

namespace NetDoodleJump
{
    public partial class GameWindow : Form
    {
        public static int formWidth;
        public static int formHeight;
        public Player player;
        public Edge edge;
        public bool lockJump = false;
        public bool lockMove = false;
        public GameWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            timerPaint.Enabled = true;
            player = new Player(this.Width/2, 500);
            edge = new Edge(this.Width / 2, 600);
        }

        private void GameWindow_Paint(object sender, PaintEventArgs e)
        {
            formWidth = this.Width;
            formHeight = this.Height;
            if (player != null && edge != null)
            {
                player.Draw(e.Graphics);
                edge.Draw(e.Graphics);
                timerGame.Enabled = true;
            }
        }

        private void timerPaint_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            player.Gravity(edge.X, edge.Y, edge.Width);
            edge.Move();
        }

        private async void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (player.isGravityOn)
                lockJump = false;
            if (e.KeyCode == Keys.Up && !lockJump)
            {
                lockJump = true;
                await Task.Run(()=>
                {
                    player.Jump();
                });
            } else if (!lockMove && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))
            {
                lockMove = true;
                await Task.Run(() =>
                {
                    player.Move(e.KeyCode);
                });
            }
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                player.StopMove = true;
                lockMove = false;
            }
        }
    }
}
