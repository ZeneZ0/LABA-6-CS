using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FallingParticlesGame
{
    public class Particle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; } = 10;
        public Color Color { get; set; }
        public int Speed { get; set; }
        public bool IsCollected { get; set; }
        public ParticleEffect Effect { get; set; }
        public bool IsDangerous => Color == Color.Red;

        private readonly Point[] trail = new Point[5];
        private int trailIndex;

        public void Update()
        {
            trail[trailIndex] = new Point(X + Size / 2, Y + Size / 2);
            trailIndex = (trailIndex + 1) % trail.Length;
        }

        public void Draw(Graphics g)
        {
            if (Effect == ParticleEffect.Steam)
            {
                DrawSteam(g);
                return;
            }

 
            

            // Рисуем саму частицу
            using (var brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, X, Y, Size, Size);
            }
        }

        private void DrawSteam(Graphics g)
        {
            var originalSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;

            int alpha = 60 + (int)(Math.Sin(DateTime.Now.Millisecond * 0.01) * 40);
            using (var steamBrush = new SolidBrush(Color.FromArgb(alpha, Color.WhiteSmoke)))
            {
                for (int i = 0; i < 3; i++)
                {
                    int puffSize = Size + i * 3;
                    int offsetX = (int)(Math.Sin(DateTime.Now.Millisecond * 0.005 + i) * 5);
                    g.FillEllipse(steamBrush,
                        X - puffSize / 2 + offsetX,
                        Y - i * 5 - (DateTime.Now.Millisecond % 1000) / 50,
                        puffSize, puffSize);
                }
            }

            g.SmoothingMode = originalSmoothing;
        }
    }
}