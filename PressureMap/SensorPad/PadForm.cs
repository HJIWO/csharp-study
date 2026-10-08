using PressureMap.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SensorPad
{
    public partial class PadForm : Form
    {
        private const int CellSize = 30;
        private const int SpreadRadius = 3;
        private const int RampMs = 1000;

        private bool isPressing;
        private int pressRow, pressCol;
        private readonly Stopwatch pressTimer = new Stopwatch();

        private readonly System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer();
        private int frameNo = 0;
        private Frame currentFrame = null;  
        public PadForm()
        {
            InitializeComponent();
            Text = "SensorPad (가상 압력 센서)";
            ClientSize = new Size(Config.Cols * CellSize, Config.Rows * CellSize);
            DoubleBuffered = true;

            frameTimer.Interval = Config.IntervalMs;
            frameTimer.Tick += OnFrameTick;
            frameTimer.Start();
        }

        private void OnFrameTick(object sender, EventArgs e)
        {
            ushort[,] values = CalculatePressure();
            frameNo++;
            currentFrame = new Frame(frameNo, DateTime.Now, values);

            Text = $"SensorPad - Frame {frameNo} / 누른 칸 압력 {values[pressRow, pressCol]}";
            Invalidate();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int r = 0; r < Config.Rows; r++)
                for (int c = 0; c < Config.Cols; c++)
                {
                    if (currentFrame != null)
                    {
                        using (var brush = new SolidBrush(ToColor(currentFrame.Values[r, c])))
                            e.Graphics.FillRectangle(brush,
                                c * CellSize, r * CellSize, CellSize, CellSize);
                    }
                    e.Graphics.DrawRectangle(Pens.LightGray,
                        c * CellSize, r * CellSize, CellSize, CellSize);
                }
        }

        private static Color ToColor(ushort value)
        {
            int level = value * 255 / Config.MaxPressure; 
            return Color.FromArgb(255, 255 - level, 255 - level);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            isPressing = true;
            pressTimer.Restart();
            UpdatePressCell(e.X, e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isPressing)
                UpdatePressCell(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left) return;
            isPressing = false;
            pressTimer.Stop();  
           
        }

        private void UpdatePressCell(int x, int y)
        {
            int col = x / CellSize;
            int row = y / CellSize;
            pressCol = Math.Max(0, Math.Min(Config.Cols - 1, col));
            pressRow = Math.Max(0, Math.Min(Config.Rows - 1, row));
            
        }
        private ushort[,] CalculatePressure()
        {
            var values = new ushort[Config.Rows, Config.Cols];   // 처음엔 전부 0
            if (!isPressing) return values;

            // 누른 시간 비율: 0.0 ~ 1.0
            double ratio = Math.Min(1.0, pressTimer.ElapsedMilliseconds / (double)RampMs);
            double peak = Config.MaxPressure * ratio;            // 누른 칸의 압력

            for (int r = 0; r < Config.Rows; r++)
                for (int c = 0; c < Config.Cols; c++)
                {
                    int dr = r - pressRow;
                    int dc = c - pressCol;
                    double distance = Math.Sqrt(dr * dr + dc * dc);    // 누른 칸까지 거리
                    double falloff = 1.0 - distance / SpreadRadius;    // 가까울수록 1, 3칸이면 0
                    if (falloff > 0)
                        values[r, c] = (ushort)(peak * falloff);
                }
            return values;
        }

    }
}
