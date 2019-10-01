namespace NetDoodleJump
{
    partial class GameWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.timerPaint = new System.Windows.Forms.Timer(this.components);
            this.timerGame = new System.Windows.Forms.Timer(this.components);
            this.lb_score1 = new System.Windows.Forms.Label();
            this.lb_score2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(225, 284);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timerPaint
            // 
            this.timerPaint.Interval = 15;
            this.timerPaint.Tick += new System.EventHandler(this.TimerPaint_Tick);
            // 
            // timerGame
            // 
            this.timerGame.Interval = 10;
            this.timerGame.Tick += new System.EventHandler(this.TimerGame_Tick);
            // 
            // lb_score1
            // 
            this.lb_score1.AutoSize = true;
            this.lb_score1.Location = new System.Drawing.Point(25, 22);
            this.lb_score1.Name = "lb_score1";
            this.lb_score1.Size = new System.Drawing.Size(0, 13);
            this.lb_score1.TabIndex = 1;
            // 
            // lb_score2
            // 
            this.lb_score2.AutoSize = true;
            this.lb_score2.Location = new System.Drawing.Point(395, 22);
            this.lb_score2.Name = "lb_score2";
            this.lb_score2.Size = new System.Drawing.Size(0, 13);
            this.lb_score2.TabIndex = 2;
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(530, 658);
            this.Controls.Add(this.lb_score2);
            this.Controls.Add(this.lb_score1);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "GameWindow";
            this.Text = "Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameWindow_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameWindow_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameWindow_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameWindow_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timerPaint;
        private System.Windows.Forms.Timer timerGame;
        private System.Windows.Forms.Label lb_score1;
        private System.Windows.Forms.Label lb_score2;
    }
}

