using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Collections;

namespace Common
{
    public static class ForHardDesign
    {
        //先声明几个API函数
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(IntPtr HDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);



        //获取网卡地址的方法如下：
        public static string GetNicAddress(string NicId)
        {


            System.IntPtr hDevice = CreateFile("\\\\.\\" + NicId, 0x80000000 | 0x40000000, 0, IntPtr.Zero, 3, 4, IntPtr.Zero);
            if (hDevice.ToInt32() == -1)
            {
                return null;
            }

            uint Len = 0;
            IntPtr Buffer = Marshal.AllocHGlobal(256);

            Marshal.WriteInt32(Buffer, 0x01010101);


            if (!DeviceIoControl(hDevice, 0x170002, Buffer,
            4,
            Buffer,
            256,
            ref Len,
            IntPtr.Zero))
            {

                Marshal.FreeHGlobal(Buffer);
                CloseHandle(hDevice);
                return null;

            }

            byte[] macBytes = new byte[6];
            Marshal.Copy(Buffer, macBytes, 0, 6);

            Marshal.FreeHGlobal(Buffer);
            CloseHandle(hDevice);
            return new System.Net.NetworkInformation.PhysicalAddress(macBytes).ToString();


        }
        /*    该方法的形参是网卡的ID ，形式如：{B50027F7-7A42-4F2D-8935-7620F1DB632F} 这样的字符串。

        调用该函数的代码如下：*/

        //以下函数获取本机所有的以太网卡的ID

        public static ArrayList GetAllNic()
        {
            NetworkInterface[] Nics = NetworkInterface.GetAllNetworkInterfaces();
            System.Collections.ArrayList EtherNics = new ArrayList(20);
            foreach (NetworkInterface nic in Nics)
            {

                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    EtherNics.Add(nic.Id);
                }
            }
            return EtherNics;
        }

        public static string GetJiQiMa()
        {
            ArrayList allNics = ForHardDesign.GetAllNic();
            StringBuilder sb = new StringBuilder();
            foreach (object Nicid in allNics)
            {
                sb.Append(ForHardDesign.GetNicAddress(Nicid.ToString()));
            }
            return sb.ToString();
        }
    }
}
