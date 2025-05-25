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
    }
}