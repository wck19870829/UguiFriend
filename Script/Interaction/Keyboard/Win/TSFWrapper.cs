using System;
using System.Runtime.InteropServices;

namespace RedScarf.UguiFriend
{
    public class TSFWrapper
    {
        [DllImport("user32.dll")]
        static extern void Test();
    }
}