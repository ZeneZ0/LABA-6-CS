using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FallingParticlesGame
{
    public class Cart
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int BaseWidth { get; } = 60;
        public int Width { get; set; }
        public int Height { get; set; } = 30;
        public Color Color { get; set; } = Color.OrangeRed;
        public bool IsFrozen { get; set; }
        public float WheelRotation { get; private set; }
        public float LastMovement { get; private set; }

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public Cart()
        {
            Width = BaseWidth;
        }

        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public void Update(int mouseX, int screenWidth)
        {
            int oldX = X;
            if (!IsFrozen)
            {
                X = mouseX - Width / 2;
                X = Clamp(X, 0, screenWidth - Width);
            }
            LastMovement = X - oldX;
            UpdateWheels();
        }

        private void UpdateWheels()
        {
            // Скорость вращения зависит от движения
            WheelRotation += LastMovement * 0.5f;
        }

        public void Draw(Graphics g)
        {
            // Сохраняем оригинальные настройки
            var originalSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Основной цвет вагонетки
            using (var bodyBrush = new SolidBrush(Color))
            {
                // Корпус вагонетки (основная часть)
                g.FillRectangle(bodyBrush, X, Y, Width, Height / 2);

                // Наклонная передняя часть
                Point[] frontShape = {
                    new Point(X, Y + Height/2),
                    new Point(X + Width/4, Y + Height/4),
                    new Point(X + Width/4, Y),
                    new Point(X, Y)
                };
                g.FillPolygon(bodyBrush, frontShape);

                // Наклонная задняя часть
                Point[] backShape = {
                    new Point(X + Width, Y + Height/2),
                    new Point(X + Width*3/4, Y + Height/4),
                    new Point(X + Width*3/4, Y),
                    new Point(X + Width, Y)
                };
                g.FillPolygon(bodyBrush, backShape);
            }

            // Рисуем колёса с анимацией
            DrawWheels(g);

            // Рельсы под вагонеткой
            DrawRails(g);

            // Восстанавливаем настройки
            g.SmoothingMode = originalSmoothing;
        }
    }
}