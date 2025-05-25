using System;
using System.Drawing;
using System.Windows.Forms;

namespace FallingParticlesGame
{
    public partial class Form1 : Form
    {
        private Game game;
        private Timer gameTimer;
        private Timer uiTimer;
        private readonly Color[] cartColors = {
            Color.OrangeRed, Color.SteelBlue, Color.ForestGreen,
            Color.Goldenrod, Color.MediumPurple, Color.Teal,
            Color.Coral, Color.HotPink, Color.LimeGreen
        };

        private Point originalPicDisplayLocation;
        private DateTime shakeEndTime;
        private bool isShaking;
        private Image backgroundImage; // Добавлено для хранения фона

        public Form1()
        {
            InitializeComponent();

            // Загрузка фона при создании формы
            try
            {
                backgroundImage = Image.FromFile("sky.jpg");
            }
            catch
            {
                backgroundImage = null;
            }

            originalPicDisplayLocation = picDisplay.Location;
            InitializeGame();
            InitializeControls();
            InitializeUITimer();
        }

        private void InitializeGame()
        {
            game = new Game(picDisplay.Width, picDisplay.Height);
            game.OnHit += StartScreenShake;

            gameTimer = new Timer { Interval = 16 };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void StartScreenShake()
        {
            isShaking = true;
            shakeEndTime = DateTime.Now.AddMilliseconds(500);
        }

        private void UpdateScreenShake()
        {
            if (isShaking)
            {
                if (DateTime.Now >= shakeEndTime)
                {
                    isShaking = false;
                    picDisplay.Location = originalPicDisplayLocation;
                }
                else
                {
                    int shakeOffset = 5;
                    var rnd = new Random();
                    picDisplay.Location = new Point(
                        originalPicDisplayLocation.X + rnd.Next(-shakeOffset, shakeOffset),
                        originalPicDisplayLocation.Y + rnd.Next(-shakeOffset, shakeOffset));
                }
            }
        }
    }
}