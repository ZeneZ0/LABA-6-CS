using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static FallingParticlesGame.Game;

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

            // Рисуем трейл
            for (int i = 0; i < trail.Length; i++)
            {
                if (trail[i] != Point.Empty)
                {
                    int alpha = 50 + i * 40;
                    int size = Size - i * 2;
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, Color)))
                        g.FillEllipse(brush,
                            trail[i].X - size / 2, trail[i].Y - size / 2,
                            size, size);
                }
            }

            // Рисуем саму частицу
            using (var brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, X, Y, Size, Size);
            }
        }
    }
}