using System;

namespace SinGooCMS.FiveStone
{
    /// <summary>
    /// 电脑的简单人工智能
    /// </summary>
    public class ComputerAI
    {
        public ComputerAI()
        {
            //
        }

        /// <summary>
        /// 电脑落子，返回落子的信息
        /// </summary>
        /// <param name="arrchessboard"></param>
        /// <returns></returns>
        public StonePoint DownDot()
        {
            int[,] qz = new int[15, 15];
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (Chessboard.StonesArray[i, j] != (int)StoneType.未落子) qz[i, j] = -1; //在该点上已经有棋子,权重为-1
                    else qz[i, j] = CheckQZ(Chessboard.StonesArray, i, j);
                }
            }

            int pointX = 0, pointY = 0;
            MaxQZ(qz, ref pointX, ref pointY);

            return new StonePoint(pointX, pointY, Common.ComputerStoneType);
        }
        
        #region 计算权重值

        /// <summary>
        /// 棋盘格里的最大权重值（最有利的位置）
        /// </summary>
        /// <param name="qz"></param>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        private void MaxQZ(int[,] qz, ref int pointX, ref int pointY)
        {
            //计算最大的权重值.这样也得到了该点的坐标x,y
            int max = 0;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (qz[i, j] > max)
                    {
                        pointX = i;
                        pointY = j;
                        max = qz[i, j];
                    }
                }
            }
        }

        /// <summary>
        /// 计算落子位置的权重值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="arrchessboard"></param>
        /// <returns></returns>
        private int CheckQZ(int[,] arr, int m, int n)
        {
            //权重加入活和死的分别,活的表示两头没有落子即为活,两头只要有一头有子即为死,h表示活,s表示死
            int qz = 0;

            //我方获胜点 连成5个
            int w1 = 100000;
            //对方获胜点,连成5个
            int w2 = 50000;

            //4个
            int w3_h = 10000;
            int w3_s = 3000;
            int w4_h = 8000;
            int w4_s = 2000;

            //3个
            int w5_h = 1000;
            int w5_s = 500;
            int w6_h = 800;
            int w6_s = 300;

            //2个
            int w7_h = 100;
            int w7_s = 50;
            int w8_h = 80;
            int w8_s = 30;

            //黑子的禁手
            int w9 = -10000000;

            if (m == 7 && n == 7) qz += 1; //最中间的位置

            arr[m, n] = (int)Common.ComputerStoneType;
            var dict = Rule.GetDirect4Num(arr, m, n);

            foreach (var item in dict)
            {
                if (Math.Abs(item.LianziNum) == 5) qz += w1;
                if (item.LianziNum == 4 && item.IsActive) qz += w3_h;
                if (item.LianziNum == 4) qz += w3_s;
                if (item.LianziNum == 3 && item.IsActive) qz += w5_h;
                if (item.LianziNum == 3) qz += w5_s;
                if (item.LianziNum == 2 && item.IsActive) qz += w7_h;
                if (item.LianziNum == 2) qz += w7_s;

                if (Common.ComputerStoneType == StoneType.黑子)
                {
                    if (Rule.IsJinShou(arr, m, n) != 0)
                        qz += w9;//黑子在判断是否禁手
                }
            }

            //如果该点下对方的子(对方相对电脑来说是人)
            arr[m, n] = Common.ComputerStoneType == StoneType.黑子 ?
                (int)StoneType.白子 :
                (int)StoneType.黑子;

            dict = Rule.GetDirect4Num(arr, m, n);
            foreach (var item in dict)
            {
                if (Math.Abs(item.LianziNum) == 5) qz += w2;
                if (item.LianziNum == 4 && item.IsActive) qz += w4_h;
                if (item.LianziNum == 4) qz += w4_s;
                if (item.LianziNum == 3 && item.IsActive) qz += w6_h;
                if (item.LianziNum == 3) qz += w6_s;
                if (item.LianziNum == 2 && item.IsActive) qz += w8_h;
                if (item.LianziNum == 2) qz += w8_s;
            }

            return qz;
        }

        #endregion
    }
}
