using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SinGooCMS.FiveStone
{
    /// <summary>
    /// 棋谱录像
    /// </summary>
    public static class ChessRecord
    {
        public static Stack StackHistory { get; set; } = new Stack();

        public static string[] ArrQP { get; set; } = new string[255];

        #region 棋谱

        /// <summary>
        /// 保存棋谱
        /// </summary>
        public static void SaveQP()
        {
            try
            {
                //保存棋谱
                string tempstr = "";
                Array arr = StackHistory.ToArray();
                if (arr.Length > 0)
                {
                    for (int i = arr.Length - 1; i >= 0; i--)
                    {
                        tempstr += arr.GetValue(i).ToString() + "\r\n";
                    }
                    tempstr += "CreateBy admin@sz3w.net http://www.singoo.top \r\n";
                    tempstr += System.DateTime.Now.ToString();
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Filter = "文本文件(*.txt)|*.txt";

                    string saveFolder = Path.Combine(System.Environment.CurrentDirectory, "QiPu");
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);
                    sd.InitialDirectory = saveFolder; //棋谱的默认保存目录

                    sd.FileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    //sd.RestoreDirectory = true;                    

                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        //保存文件
                        FileStream fs = new FileStream(sd.FileName, FileMode.Create, FileAccess.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312"));
                        sw.Write(tempstr);
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }
                else MessageBox.Show("没有可用棋谱", "温馨提示", MessageBoxButtons.OK);
            }
            catch
            {
                MessageBox.Show("程序错误,保存失败", "温馨提示", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 加载棋谱
        /// </summary>
        /// <returns></returns>
        public static IList<StonePoint> LoadQP()
        {
            try
            {
                var lst = new List<StonePoint>();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "文本文件(*.txt)|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //打开文件成功
                    StreamReader sr = new StreamReader(ofd.FileName, Encoding.GetEncoding("gb2312"));
                    string tempStr = "";
                    tempStr = sr.ReadToEnd();
                    tempStr = tempStr.Replace("\r\n", "\r");
                    string[] arrStr = tempStr.Split('\r');

                    //棋谱如：1 黑子 x：H - y：8
                    for (int i = 0; i < arrStr.Length; i++)
                    {
                        var stepNum = Utils.GetInt(arrStr[i].Substring(0, arrStr[i].IndexOf(" ")));
                        if (stepNum == 0) break; //循环输出,直到不是棋谱为止

                        string strPoint = arrStr[i].Substring(arrStr[i].IndexOf("横：") + "横：".Length);
                        string strPointX = strPoint.Substring(0, 1); //H
                        string strPointY = strPoint.Substring(strPoint.IndexOf("纵：") + "纵：".Length); //8

                        ArrQP[i] = arrStr[i];//用于显示在右侧栏

                        lst.Add(new StonePoint()
                        {
                            PointX = (int)Enum.Parse(typeof(ChessBoardX), strPointX),
                            PointY = (int)Enum.Parse(typeof(ChessBoardY), "No" + strPointY),
                            Type = arrStr[i].IndexOf("黑子") != -1 ? StoneType.黑子 : StoneType.白子,
                            StepNum = stepNum
                        });
                    }
                }

                return lst;
            }
            catch
            {
                MessageBox.Show("错误的棋谱格式", "温馨提示", MessageBoxButtons.OK);
            }

            return null;
        }

        #endregion       
    }
}
