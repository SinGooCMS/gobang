using System;
using System.Collections.Generic;
using System.Drawing;

namespace SinGooCMS.FiveStone
{
    #region 棋子

    /// <summary>
    /// 棋子类
    /// </summary>
    public class Stone
    {
        System.Drawing.Graphics g;
        private static readonly object lockHelper = new object();

        public Stone(System.Drawing.Graphics graphiic) =>
                    g = graphiic;

        #region 黑白棋子图片

        //白子
        public System.Drawing.Image WhiteStone =>
            global::SinGooCMS.FiveStone.Properties.Resources.whitestone;

        //黑子
        public System.Drawing.Image BlackStone =>
            global::SinGooCMS.FiveStone.Properties.Resources.blackstone;

        #endregion

        #region 画棋子

        /// <summary>
        /// 画棋子 x,y为计算后的棋子的位置,+24为菜单24高度，棋子上还包括数字（步数）
        /// </summary>
        /// <param name="point"></param>
        public void DrawStone(StonePoint point)
        {
            lock(lockHelper)
            {
                Brush bh;
                Font f;

                //数字的位置
                int numX = 0, numY = 0;
                if (point.StepNum < 10)
                {
                    numX = 14; numY = 13;
                }
                if (point.StepNum >= 10 && point.StepNum < 100)
                {
                    numX = 11; numY = 13;
                }
                if (point.StepNum >= 100)
                {
                    numX = 8; numY = 13;
                }

                switch (point.Type)
                {
                    case StoneType.黑子:
                        {
                            g.DrawImage(BlackStone, point.PointX * 40, (point.PointY * 40) + 24, BlackStone.Width, BlackStone.Height);
                            bh = new SolidBrush(Color.White);
                            f = new Font("宋体", 10);
                            g.DrawString(point.StepNum.ToString(), f, bh, point.PointX * 40 + numX, (point.PointY * 40) + 24 + numY);                            
                        }
                        break;
                    case StoneType.白子:
                        {
                            g.DrawImage(WhiteStone, point.PointX * 40, (point.PointY * 40) + 24, WhiteStone.Width, WhiteStone.Height);
                            bh = new SolidBrush(Color.Black);
                            f = new Font("宋体", 10);
                            g.DrawString(point.StepNum.ToString(), f, bh, point.PointX * 40 + numX, (point.PointY * 40) + 24 + numY);
                        }
                        break;
                }

                /*
                //给最后的落子加外框
                if(point.StepNum==Chessboard.MaxStep)
                {
                    Pen myPen = new Pen(Color.Red, 1);
                    g.DrawRectangle(myPen, new Rectangle(point.PointX * 40 - 1, (point.PointY * 40) + 23, BlackStone.Width + 1, BlackStone.Height + 1));
                }*/                
            }                       
        }

        #endregion
    }

    #endregion

    #region 在棋盘上的落子

    /// <summary>
    /// 在棋盘上的落子
    /// </summary>
    public class StonePoint
    {
        public StonePoint(int x, int y, StoneType stoneType = StoneType.黑子, int stepNum = 0)
        {
            this.PointX = x;
            this.PointY = y;
            this.Type = stoneType;
            this.StepNum = stepNum;
        }

        public StonePoint()
        {
            //
        }

        /// <summary>
        /// 棋子类型：黑子、白子
        /// </summary>
        public StoneType Type { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public int PointX { get; set; }

        /// <summary>
        /// 坐标的值
        /// </summary>
        public string PointXValue
        {
            get
            {
                return Enum.GetName(typeof(ChessBoardX), PointX);
            }
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int PointY { get; set; }

        /// <summary>
        /// 坐标的值
        /// </summary>
        public string PointYValue
        {
            get
            {
                return Enum.GetName(typeof(ChessBoardY), PointY);
            }
        }

        /// <summary>
        /// 步数
        /// </summary>
        public int StepNum { get; set; }
    }

    #endregion

    #region 棋子的方位数量

    public class StoneDirect
    {
        /// <summary>
        /// 方位，共4个方位
        /// </summary>
        public string DirectName { get; set; }

        /// <summary>
        /// 连子数
        /// </summary>
        public int LianziNum { get; set; }

        /// <summary>
        /// 是否活的，两边都没堵住就是活的
        /// </summary>
        public bool IsActive { get; set; }
    }

    #endregion

    public static class StoneExtention
    {
        /// <summary>
        /// 该落子点位置上4个方向的连续子统计
        /// </summary>
        /// <param name="stonePoint"></param>
        /// <returns></returns>
        public static List<StoneDirect> GetDirect4Num(this StonePoint stonePoint)
        {
            return Rule.GetDirect4Num(Chessboard.StonesArray, stonePoint.PointX, stonePoint.PointY);
        }
    }
}
