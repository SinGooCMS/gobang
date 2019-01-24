using System.Media;
using System.IO;
using System.Text;

namespace SinGooCMS.FiveStone
{
    /// <summary>
    /// 黑白子
    /// </summary>
    public enum StoneType
    {
        黑子,
        白子,
        未落子
    }

    /// <summary>
    /// 棋盘横坐标
    /// </summary>
    public enum ChessBoardX
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O
    };

    /// <summary>
    /// 棋盘纵坐标
    /// </summary>
    public enum ChessBoardY
    {
        No15, No14, No13, No12, No11, No10, No9, No8, No7, No6, No5, No4, No3, No2, No1
    }

    /// <summary>
    /// 游戏角色
    /// </summary>
    public enum Roles
    {
        电脑,
        玩家
    }

    public static class Common
    {
        /// <summary>
        /// 先手（电脑还是玩家先手，先手执黑）
        /// </summary>
        public static Roles XianShou { get; set; }

        /// <summary>
        /// 电脑执（黑/白）子
        /// </summary>
        public static StoneType ComputerStoneType
        {
            get
            {
                return XianShou == Roles.电脑 ? StoneType.黑子 : StoneType.白子;
            }
        }
        /// <summary>
        /// 玩家执（黑/白）子
        /// </summary>
        public static StoneType PlayerStoneType
        {
            get
            {
                return XianShou == Roles.电脑 ? StoneType.白子 : StoneType.黑子;
            }
        }             

        /// <summary>
        /// 背景音乐路径
        /// </summary>
        public static string BackgroundSoundUrl =>
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "骆集益-玉满堂.mp3");        

        /// <summary>
        /// 播放落子的音效
        /// </summary>
        public static void MakeStoneSound()
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(global::SinGooCMS.FiveStone.Properties.Resources.click);
            sp.Play();
        }
    }

    public class Utils
    {
        /// <summary>
        /// 字符串转数字
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        public static int GetInt(string numStr)
        {
            if (int.TryParse(numStr, out int outParm))
                return outParm;

            return 0;
        }           
    }
}
