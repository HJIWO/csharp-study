using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMap.Common
{
    public class Config
    {
        // 통신
        public const int Port = 5000;
        public const string Host = "127.0.0.1";

        //격자
        public const int Rows = 16;
        public const int Cols =16;
        public const int CellCount = Rows * Cols;

        //주기
        public const int Fps = 10;
        public const int IntervalMs = 1000 / Fps;

        public const int MaxPressure = 1023;
    
    }
}
