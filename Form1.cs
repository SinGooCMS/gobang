using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace SinGooCMS.FiveStone
{
    public partial class Form1 : Form
    {
        System.Drawing.Graphics g;
        ChessboardFun chessboard;
        Stone stone;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.TopMost = false;

            g = this.CreateGraphics();
            stone = new Stone(g);
            
            this.axWindowsMediaPlayer1.URL = Common.BackgroundSoundUrl;
            if (toolStripMenuItem4.Checked)
                this.axWindowsMediaPlayer1.Ctlcontrols.play();

            Common.XianShou = 电脑先ToolStripMenuItem.Checked ? Roles.电脑 : Roles.玩家;
            label1.Text= 电脑先ToolStripMenuItem.Checked ? Roles.电脑.ToString() : Roles.玩家.ToString();
            chessboard = new ChessboardFun(g,listBox1); //生成棋盘
            chessboard.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Thread th = new Thread(new ThreadStart(chessboard.Draw));
            th.Start();

            //chessboard.Draw();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 600 && e.Y < 600 + 24 && e.Y > 24)
                this.Cursor = Cursors.Hand; //鼠标的样式为手形
            else
                this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 鼠标点击的时候落子(必定是玩家的操作，电脑不需要鼠标操作)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < 600 && e.Y < 600 + 24 && e.Y > 24 && e.Button == MouseButtons.Left)
            {
                if (!chessboard.IsRecordPlay)
                {
                    chessboard.PlayerDownStone(e.X, (e.Y - 24)); //只有玩家才用鼠标点击，电脑不需要
                }
                else if (chessboard.IsRecordPlay && MessageBox.Show("正在观看下棋录像,是否停止?", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    timer1.Enabled = false;
                    StopRecordAndClearAll();
                }
            }
        }

        /// <summary>
        /// 停止播放录像
        /// </summary>
        private void StopRecordAndClearAll()
        {
            timer1.Stop();
            timer1.Enabled = false;

            lstQP = null;            
            listBox1.DataSource = null;
            chessboard.IsRecordPlay = false;
            
            Chessboard.Clear();
            ChessRecord.StackHistory.Clear();
            Common.XianShou = 电脑先ToolStripMenuItem.Checked ? Roles.电脑 : Roles.玩家;
        }

        #region 菜单功能        

        private void 电脑先ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "电脑";
            电脑先ToolStripMenuItem.Checked = true;
            玩家先ToolStripMenuItem.Checked = false;
        }

        private void 玩家先ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "玩家";
            电脑先ToolStripMenuItem.Checked = false; ;
            玩家先ToolStripMenuItem.Checked = true;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            toolStripMenuItem4.Checked = !toolStripMenuItem4.Checked;
            if (toolStripMenuItem4.Checked)
            {
                //播放背景音乐                
                this.axWindowsMediaPlayer1.URL = Common.BackgroundSoundUrl;
                this.axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else
            {
                //关闭背景音乐
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
        }

        private void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
        {
            if ((int)axWindowsMediaPlayer1.playState == 1 && toolStripMenuItem4.Checked)
            {
                //停顿2秒钟再重新播放
                System.Threading.Thread.Sleep(2000);
                //重新播放
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopRecordAndClearAll();//停止观看录像

            Common.XianShou = 电脑先ToolStripMenuItem.Checked ? Roles.电脑 : Roles.玩家;
            chessboard.Start();
        }

        public static int count = 0;
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUs about = new AboutUs();
            if (count < 1)
            {
                about.Show();
                count++;
            }
        }

        string pathDocx = System.IO.Path.Combine(System.Environment.CurrentDirectory , "说明.docx");
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F1")
            {
                if (System.IO.File.Exists(pathDocx))
                    System.Diagnostics.Process.Start("说明.docx");
                else
                    MessageBox.Show("没有找到文件：说明.docx");
            }
        }

        private void 帮助ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(pathDocx))
                System.Diagnostics.Process.Start("说明.docx");
            else
                MessageBox.Show("没有找到文件：说明.docx");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region 棋谱

        /// <summary>
        /// 保存棋谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChessRecord.SaveQP();            
        }

        IList<StonePoint> stoneSteps = null;
        List<string> lstQP = new List<string>();
        int stepNum = 1;

        /// <summary>
        /// 载入棋谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //导入棋谱,观看录像
            Chessboard.Stones.Clear();
            lstQP.Clear();
            stoneSteps = ChessRecord.LoadQP();
            if (stoneSteps != null && stoneSteps.Count > 0)
            {
                listBox1.DataSource=null;
                chessboard.IsRecordPlay = true;
                chessboard.Draw();
                //自动播放录像,计时器开始工作
                stepNum = 1;
                timer1.Enabled = true;
                timer1.Interval = 3000; //间隔3秒
                timer1.Start();
            }
            else
                MessageBox.Show("无效的棋谱数据");
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {            
            if (stoneSteps != null && stepNum <= stoneSteps.Count)
            {
                lstQP.Add(ChessRecord.ArrQP[stepNum - 1]);
                listBox1.DataSource = null;
                listBox1.DataSource = lstQP;                
                Chessboard.Stones.Add(stoneSteps[stepNum - 1]);
                stone.DrawStone(Chessboard.Stones[stepNum - 1]);
                Common.MakeStoneSound();
                stepNum++;
            }
            else
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
        }

        #endregion        
    }
}