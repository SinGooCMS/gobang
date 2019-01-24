using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SinGooCMS.FiveStone
{
    class HotKey
    {
        /*
         * RegisterHotKey函数原型及说明：
         * BOOL RegisterHotKey(
         * HWND hWnd,         // window to receive hot-key notification
         * int id,            // identifier of hot key
         * UINT fsModifiers, // key-modifier flags
         * UINT vk            // virtual-key code);
         * 参数 id为你自己定义的一个ID值
         * 对一个线程来讲其值必需在0x0000 - 0xBFFF范围之内,十进制为0~49151
         * 对DLL来讲其值必需在0xC000 - 0xFFFF 范围之内,十进制为49152~65535
         * 在同一进程内该值必须唯一参数 fsModifiers指明与热键联合使用按键
         * 可取值为：MOD_ALT MOD_CONTROL MOD_WIN MOD_SHIFT参数，或数字0为无，1为Alt,2为Control，4为Shift，8为Windows
         * vk指明热键的虚拟键码
         */

        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数
        public static extern bool RegisterHotKey(
         IntPtr hWnd, // handle to window 
         int id, // hot key identifier 
         uint fsModifiers, // key-modifier options 
         Keys vk // virtual-key code 
        );

        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数
        public static extern bool UnregisterHotKey(
         IntPtr hWnd, // handle to window 
         int id // hot key identifier 
        );
    }


}
