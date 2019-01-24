using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SinGooCMS.FiveStone
{
    #region 棋盘

    /// <summary>
    /// 棋盘
    /// </summary>
    public static class Chessboard
    {
        static Chessboard()
        {
            if (Stones == null)
                Stones = InitStones(); //初始化
        }

        /// <summary>
        /// 棋盘上的所有落子
        /// </summary>
        public static IList<StonePoint> Stones { get; set; }

        /// <summary>
        /// 最大的步数，步数总是+1
        /// </summary>
        public static int MaxStep =>
            Stones.Max(p => p.StepNum);

        /// <summary>
        /// 棋盘上的所有落子数组形式
        /// </summary>
        public static int[,] StonesArray
        {
            get
            {
                int[,] arr = new int[15, 15];
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        var temp = Stones.Where(p => p.PointX == i && p.PointY == j).FirstOrDefault();
                        if (temp != null)
                            arr[i, j] = (int)temp.Type;
                        else
                            arr[i, j] = (int)StoneType.未落子;
                    }
                }

                return arr;
            }
        }

        /// <summary>
        /// 更新棋盘上的落子
        /// </summary>
        /// <param name="point"></param>
        public static void UpdateStones(StonePoint point)
        {
            foreach (var item in Stones)
            {
                if (item.PointX == point.PointX && item.PointY == point.PointY)
                {
                    item.Type = point.Type;
                    item.StepNum = point.StepNum;
                }
            }
        }

        /// <summary>
        /// 重置棋子，一局完了需要重置
        /// </summary>
        public static void Clear()
        {
            Stones.Clear();
            Stones = InitStones();
        }

        /// <summary>
        /// 初始化15X15个空棋位
        /// </summary>
        /// <returns></returns>
        private static IList<StonePoint> InitStones()
        {
            var _stones = new List<StonePoint>();
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    _stones.Add(new StonePoint(i, j, StoneType.未落子, 0));
                }
            }

            return _stones;
        }
    }

    #endregion

    #region 棋盘功能

    public class ChessboardFun
    {
        Stone stone;
        System.Drawing.Graphics mg;
        System.Windows.Forms.ListBox shower;
        private static readonly object lockHelper = new object();
        ComputerAI computer = new ComputerAI(); //电脑AI

        public bool IsRecordPlay { get; set; } = false; //是否播放录像

        public ChessboardFun(Graphics g,ListBox listBox)
        {
            mg = g;
            shower = listBox;
            stone = new Stone(mg);
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Chessboard.Clear();
            ChessRecord.StackHistory.Clear();
        }

        public void Start()
        {
            if (Common.XianShou == Roles.电脑)
                ComputerDownStone(); //电脑先下
        }        

        /// <summary>
        /// 绘制棋盘和棋子
        /// </summary>
        public void Draw()
        {
            lock (lockHelper)
            {
                //棋盘  
                Image imgChessboard = global::SinGooCMS.FiveStone.Properties.Resources.chessboard; //棋盘图片 大小600*600像素
                mg.DrawImage(imgChessboard, 0, 24, imgChessboard.Width, imgChessboard.Height);

                if (IsRecordPlay)
                {
                    lock (lockHelper)
                    {
                        //画出提示字
                        Brush brh = new SolidBrush(Color.Blue);
                        System.Drawing.Font f = new Font("宋体", 30);
                        mg.DrawString("正在查看棋谱录像中...", f, brh, 100, 300);
                    }
                }

                foreach (var item in Chessboard.Stones)
                {
                    if (item.Type != StoneType.未落子)
                        stone.DrawStone(item); //负责把有值（0，1/黑白子）的棋子画在棋盘上
                }                
            }
        }

        public void PlayerDownStone(int x, int y)
        {
            //转化棋盘点
            int m, n;
            m = (int)((double)x / 40);
            n = (int)((double)y / 40);
            if (!Rule.ExistsStone(m, n))
            {
                var result = DownStone(new StonePoint(m, n, Common.PlayerStoneType, Chessboard.MaxStep + 1));
                if (result)
                {
                    System.Threading.Thread.Sleep(1000);
                    ComputerDownStone(); //接着电脑落子
                }
            }
        }

        private void ComputerDownStone()
        {
            var point = computer.DownDot();
            point.StepNum = Chessboard.MaxStep + 1;
            DownStone(point); //电脑落子后提醒玩家落子            
        }

        private bool DownStone(StonePoint point)
        {
            #region 人机对弈,下子

            if (point.Type == StoneType.未落子)
                return false;

            var arr = Chessboard.StonesArray;
            arr[(int)point.PointX, (int)point.PointY] = (int)point.Type;
            var jingShou = Rule.IsJinShou(arr, point.PointX, point.PointY);
            if (point.Type == StoneType.黑子 && jingShou == JinShou.双三禁手)
                MessageBox.Show("黑子双三禁手!", "温馨提示", MessageBoxButtons.OK);
            else if (point.Type == StoneType.黑子 && jingShou == JinShou.双四禁手)
                MessageBox.Show("黑子双四禁手!", "温馨提示", MessageBoxButtons.OK);
            else if (point.Type == StoneType.黑子 && jingShou == JinShou.长联禁手)
                MessageBox.Show("黑子长联禁手!", "温馨提示", MessageBoxButtons.OK);
            else
            {
                Chessboard.UpdateStones(point); //更新Stones
                stone.DrawStone(point);
                Common.MakeStoneSound();

                var txt = point.StepNum.ToString() + " " + point.Type.ToString() + " 横：" + point.PointXValue + " - " + "纵：" + point.PointYValue.Replace("No", "");
                ChessRecord.StackHistory.Push(txt); //StackHistory为了保存棋谱
                shower.DataSource = ChessRecord.StackHistory.ToArray();
                shower.Update();

                if (Rule.IsPing() && MessageBox.Show("平局!", "重新开始", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Chessboard.Clear();
                    ChessRecord.StackHistory.Clear();
                    Draw(); //重绘清除落子
                    Start(); //开新一局
                }
                else if (Rule.IsWin(point.PointX, point.PointY)) //在该位置落子胜
                {
                    string txtStone = point.Type == StoneType.黑子 ? "黑子胜利" : "白子胜利";
                    if (MessageBox.Show(txtStone, "是否保存棋谱", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ChessRecord.SaveQP();
                    }

                    Chessboard.Clear();
                    ChessRecord.StackHistory.Clear();
                    Draw(); //重绘清除落子
                    Start(); //开新一局
                }
                else
                    return true; //可以走下一步
            }

            #endregion

            return false;
        }
    }

    #endregion
}