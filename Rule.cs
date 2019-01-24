using System.Collections.Generic;

namespace SinGooCMS.FiveStone
{
    /// <summary>
    /// 禁手，黑子才有禁手
    /// </summary>
    public enum JinShou
    {
        无禁手,
        双三禁手,
        双四禁手,
        长联禁手
    }

    /// <summary>
    /// 五子棋的规则
    /// </summary>
    public class Rule
    {
        #region 判断结果

        /// <summary>
        /// 判断该位置是否有落子
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="ChessBoard"></param>
        /// <returns></returns>
        public static bool ExistsStone(int m, int n) =>
            Chessboard.StonesArray[m, n] < 2;

        /// <summary>
        /// 是否胜利
        /// </summary>
        /// <param name="arr">4个方向的连子数</param>
        /// <returns></returns>
        public static bool IsWin(int m, int n)
        {
            var lst = GetDirect4Num(Chessboard.StonesArray, m, n);
            foreach (var item in lst)
                if (item.LianziNum == 5) return true;

            return false;
        }

        /// <summary>
        /// 平局即所有的落子点都下满了.落子为1,0.空着表示2
        /// </summary>
        /// <param name="arrchessboard"></param>
        /// <returns></returns>
        public static bool IsPing()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (Chessboard.StonesArray[i, j] == 2) return false;
                }
            }

            return true;
        }

        #endregion

        #region 是否禁手（针对电脑）

        /// <summary>
        /// 判断在该点下子会不会犯规.因为黑子先手下棋会有很大的优势,所以需要一些规则来限制先手下棋
        /// 即黑子不能走双三,双四,不能长联,只能走四冲三
        /// </summary>
        /// <param name="arr">四个方向的连数</param>
        /// <param name="stoneflag">落子类型</param>
        /// <returns></returns>
        public static JinShou IsJinShou(int[,] arr, int m, int n)
        {
            var lst = GetDirect4Num(arr, m, n);
            int shuang3 = 0;
            int shuang4 = 0;
            foreach (var item in lst)
            {
                if (item.IsActive && item.LianziNum == 3) shuang3++;
                if (item.IsActive && item.LianziNum == 4) shuang4++;
            }

            if (shuang3 > 1) return JinShou.双三禁手;
            else if (shuang4 > 1) return JinShou.双四禁手;
            
            return JinShou.无禁手;
        }

        #endregion

        #region 返回四个方向的连续棋子数

        /*
         * 五子棋的核心就是计算是否五连。4个方向的五连。
         * 如果两边都堵住，而且没有达到5，可以认为是没有发展前途的，返回负数
         * flag=1 表示一头堵住 flag=2 表示两头都堵住，只要有一边被堵住就不是活棋
         */

        public static List<StoneDirect> GetDirect4Num(int[,] arr, int m, int n)
        {
            var directs = new List<StoneDirect>();
            directs.Add(XNum(arr, m, n));
            directs.Add(YNum(arr, m, n));
            directs.Add(YXNum(arr, m, n));
            directs.Add(XYNum(arr, m, n));
            return directs;
        }

        public static StoneDirect XNum(int[,] arr, int m, int n)
        {
            //正东方向
            int flag = 0, num = 1;
            int i = m + 1;
            while (i < 15)
            {
                if (arr[m, n] == arr[i, n])
                {
                    num++;
                    i++;
                }
                else break;
            }
            if (i == 15) flag++;
            else
            {
                if (arr[i, n] != 2) flag++;
            }

            //正西方向
            i = m - 1;
            while (i >= 0)
            {
                if (arr[m, n] == arr[i, n])
                {
                    num++;
                    i--;
                }
                else break;
            }
            if (i == -1) flag++;
            else
            {
                if (arr[i, n] != 2) flag++;
            }

            return new StoneDirect()
            {
                DirectName = "X",
                LianziNum = num,
                IsActive = flag == 0
            };
        }

        public static StoneDirect YNum(int[,] arr, int m, int n)
        {
            int num = 1, flag = 0;
            //正南方向
            int i = n + 1;
            while (i < 15)
            {
                if (arr[m, n] == arr[m, i])
                {
                    num++;
                    i++;
                }
                else break;
            }
            if (i == 15) flag++;
            else
            {
                if (arr[m, i] != 2) flag++;
            }
            //正北方向
            i = n - 1;
            while (i >= 0)
            {
                if (arr[m, n] == arr[m, i])
                {
                    num++;
                    i--;
                }
                else break;
            }
            if (i == -1) flag++;
            else
            {
                if (arr[m, i] != 2) flag++;
            }

            return new StoneDirect()
            {
                DirectName = "Y",
                LianziNum = num,
                IsActive = flag == 0
            };
        }

        public static StoneDirect YXNum(int[,] arr, int m, int n)
        {
            int num = 1, flag = 0;
            //东南方向
            int i = m + 1, j = n + 1;
            while (i < 15 && j < 15)
            {
                if (arr[m, n] == arr[i, j])
                {
                    num++;
                    i++;
                    j++;
                }
                else break;
            }
            if (i == 15 || j == 15) flag++;
            else
            {
                if (arr[i, j] != 2) flag++;
            }
            //西北方向
            i = m - 1;
            j = n - 1;
            while (i >= 0 && j >= 0)
            {
                if (arr[m, n] == arr[i, j])
                {
                    num++;
                    i--;
                    j--;
                }
                else break;
            }
            if (i == -1 || j == -1) flag++;
            else
            {
                if (arr[i, j] != 2) flag++;
            }

            return new StoneDirect()
            {
                DirectName = "YX",
                LianziNum = num,
                IsActive = flag == 0
            };
        }

        public static StoneDirect XYNum(int[,] arr, int m, int n)
        {
            int num = 1, flag = 0;
            //东北方向
            int i = m + 1, j = n - 1;
            while (i < 15 && j >= 0)
            {
                if (arr[i, j] == arr[m, n])
                {
                    num++;
                    i++;
                    j--;
                }
                else break;
            }
            if (i == 15 || j == -1) flag++;
            else
            {
                if (arr[i, j] != 2) flag++;
            }
            //西南方向
            i = m - 1; j = n + 1;
            while (i >= 0 && j < 15)
            {
                if (arr[i, j] == arr[m, n])
                {
                    num++;
                    i--;
                    j++;
                }
                else break;
            }
            if (i == -1 || j == 15) flag++;
            else
            {
                if (arr[i, j] != 2) flag++;
            }

            return new StoneDirect()
            {
                DirectName = "XY",
                LianziNum = num,
                IsActive = flag == 0
            };
        }

        #endregion
    }
}
