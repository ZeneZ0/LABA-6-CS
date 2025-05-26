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

        

        public void Update(int mouseX, int screenWidth)
        {
            int oldX = X;
            if (!IsFrozen)
            {
                X = mouseX - Width / 2;
                
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

        

            // Рельсы под вагонеткой
            DrawRails(g);

            // Восстанавливаем настройки
            g.SmoothingMode = originalSmoothing;
        }

       

        private void DrawWheel(Graphics g, Brush brush, Pen pen, int centerX, int centerY, int size)
        {
            // Сохраняем трансформацию
            var originalTransform = g.Transform;

            // Применяем вращение
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(WheelRotation);

            // Рисуем колесо
            g.FillEllipse(brush, -size / 2, -size / 2, size, size);

            // Спицы колеса
            for (int i = 0; i < 6; i++)
            {
                g.DrawLine(pen, 0, 0, 0, -size / 2);
                g.RotateTransform(60);
            }

            // Восстанавливаем трансформацию
            g.Transform = originalTransform;
        }

        private void DrawRails(Graphics g)
        {
            using (var railPen = new Pen(Color.DarkGray, 4))
            {
                g.DrawLine(railPen,
                    X - 10, Y + Height / 2 + 5,
                    X + Width + 10, Y + Height / 2 + 5);
            }
        }

        public void ResetSize()
        {
            Width = BaseWidth;
            IsFrozen = false;
        }
    }
}