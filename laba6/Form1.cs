using System;
using System.Drawing;
using System.Windows.Forms;
using static FallingParticlesGame.Game;

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

        private void InitializeUITimer()
        {
            uiTimer = new Timer { Interval = 30 };
            uiTimer.Tick += (s, e) => picDisplay.Invalidate();
            uiTimer.Start();
        }

        private void InitializeControls()
        {
            var controlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 200,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };
            Controls.Add(controlPanel);

            int verticalPos = 15;
            int elementHeight = 70;

            // Настройка частоты появления
            var spawnLabel = new Label
            {
                Text = "Частота:",
                Location = new Point(10, verticalPos),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(spawnLabel);

            var spawnTrack = new TrackBar
            {
                Location = new Point(10, verticalPos + 20),
                Width = 180,
                Minimum = 1,
                Maximum = 100,
                Value = game.SpawnRate,
                TickFrequency = 10
            };
            spawnTrack.ValueChanged += (s, e) => game.SpawnRate = spawnTrack.Value;
            controlPanel.Controls.Add(spawnTrack);

            verticalPos += elementHeight;

            // Настройка скорости
            var speedLabel = new Label
            {
                Text = "Скорость:",
                Location = new Point(10, verticalPos),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(speedLabel);

            var speedTrack = new TrackBar
            {
                Location = new Point(10, verticalPos + 20),
                Width = 180,
                Minimum = 1,
                Maximum = 15,
                Value = game.ParticleSpeed,
                TickFrequency = 2
            };
            speedTrack.ValueChanged += (s, e) => game.ParticleSpeed = speedTrack.Value;
            controlPanel.Controls.Add(speedTrack);

            verticalPos += elementHeight;

            // Настройка размера
            var sizeLabel = new Label
            {
                Text = "Размер частиц:",
                Location = new Point(10, verticalPos),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(sizeLabel);

            var sizeTrack = new TrackBar
            {
                Location = new Point(10, verticalPos + 20),
                Width = 180,
                Minimum = 5,
                Maximum = 30,
                Value = game.ParticleSize,
                TickFrequency = 5
            };
            sizeTrack.ValueChanged += (s, e) =>
            {
                game.ParticleSize = Math.Min(sizeTrack.Value, game.Cart.BaseWidth / 2);
                sizeTrack.Value = game.ParticleSize;
            };
            controlPanel.Controls.Add(sizeTrack);

            verticalPos += elementHeight;

            // Выбор цвета тележки
            var colorLabel = new Label
            {
                Text = "Цвет тележки:",
                Location = new Point(10, verticalPos),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(colorLabel);

            verticalPos += 25;

            // Таблица цветов 3x3
            var colorTable = new TableLayoutPanel
            {
                Location = new Point(10, verticalPos),
                ColumnCount = 3,
                RowCount = 3,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            for (int i = 0; i < 3; i++)
            {
                colorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
                colorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }

            for (int i = 0; i < cartColors.Length; i++)
            {
                var btn = new Button
                {
                    BackColor = cartColors[i],
                    Size = new Size(30, 30),
                    Margin = new Padding(3),
                    FlatStyle = FlatStyle.Flat,
                    Tag = i,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.Gray;
                btn.Click += (s, e) => game.Cart.Color = cartColors[(int)((Button)s).Tag];

                colorTable.Controls.Add(btn, i % 3, i / 3);
            }

            controlPanel.Controls.Add(colorTable);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            game.Update();
            UpdateScreenShake();

            if (game.IsGameOver)
            {
                gameTimer.Stop();
                uiTimer.Stop();

                var result = MessageBox.Show($"Игра окончена! Счёт: {game.Score}\nИграть снова?",
                    "Конец игры",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    game.Reset();
                    gameTimer.Start();
                    uiTimer.Start();
                }
                else
                {
                    Close();
                }
            }
        }

        private void picDisplay_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Отрисовка фона (используем заранее загруженное изображение)
            if (backgroundImage != null)
            {
                g.DrawImage(backgroundImage, 0, 0, picDisplay.Width, picDisplay.Height);
            }
            else
            {
                g.Clear(Color.Black);
            }

            // Остальная отрисовка без изменений
            game.Draw(g);

            foreach (var particle in game.Particles)
            {
                particle.Draw(g);
            }

            game.Cart.Draw(g);

            // Тень под тележкой
            using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush,
                    game.Cart.X + game.Cart.Width / 4,
                    game.Cart.Y + game.Cart.Height - 5,
                    game.Cart.Width / 2, 10);
            }

            DrawScore(g);
            DrawLives(g);
            DrawEffectTimer(g);
            DrawInvulnerability(g);
        }

        private void DrawScore(Graphics g)
        {
            string scoreText = $"Счёт: {game.Score}";
            var scoreFont = new Font("Arial", 14, FontStyle.Bold);
            var scoreSize = g.MeasureString(scoreText, scoreFont);

            using (var bgBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            {
                g.FillRectangle(bgBrush,
                    10, 8,
                    scoreSize.Width + 10,
                    scoreSize.Height + 4);
            }

            g.DrawString(scoreText, scoreFont, Brushes.White, 10, 10);
        }

        private void DrawLives(Graphics g)
        {
            string livesText = $"Жизни: {game.Lives}";
            var livesFont = new Font("Arial", 14, FontStyle.Bold);
            var livesSize = g.MeasureString(livesText, livesFont);

            using (var bgBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            {
                g.FillRectangle(bgBrush,
                    10, 40,
                    livesSize.Width + 10,
                    livesSize.Height + 4);
            }

            Brush livesBrush = game.Lives < 2 ? Brushes.Red : Brushes.White;
            g.DrawString(livesText, livesFont, livesBrush, 10, 42);
        }

        private void DrawEffectTimer(Graphics g)
        {
            if (game.CurrentEffect != ParticleEffect.None)
            {
                var timeLeft = (game.EffectEndTime - DateTime.Now).TotalSeconds;
                if (timeLeft > 0)
                {
                    string effectText = GetEffectText(game.CurrentEffect);
                    var timerText = $"{effectText}: {timeLeft:F1}с";
                    var timerFont = new Font("Arial", 12, FontStyle.Bold);
                    var timerSize = g.MeasureString(timerText, timerFont);

                    using (var bgBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    {
                        g.FillRectangle(bgBrush,
                            picDisplay.Width - timerSize.Width - 15,
                            8,
                            timerSize.Width + 10,
                            timerSize.Height + 4);
                    }

                    g.DrawString(timerText, timerFont, Brushes.White,
                        picDisplay.Width - timerSize.Width - 10, 10);
                }
            }
        }

        private void DrawInvulnerability(Graphics g)
        {
            if (game.IsInvulnerable)
            {
                string invText = "НЕУЯЗВИМОСТЬ!";
                var invFont = new Font("Arial", 16, FontStyle.Bold);
                var invSize = g.MeasureString(invText, invFont);

                float pulse = (float)Math.Abs(Math.Sin(DateTime.Now.Millisecond * 0.01));
                var pulseColor = Color.FromArgb(255, (int)(255 * pulse), (int)(255 * pulse));

                using (var brush = new SolidBrush(pulseColor))
                {
                    g.DrawString(invText, invFont, brush,
                        picDisplay.Width / 2 - invSize.Width / 2,
                        20);
                }
            }
        }

        private string GetEffectText(ParticleEffect effect)
        {
            switch (effect)
            {
                case ParticleEffect.DoubleSize: return "Увеличение";
                case ParticleEffect.Freeze: return "Заморозка";
                case ParticleEffect.HalfSize: return "Уменьшение";
                default: return "";
            }
        }

        private void picDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            game.Cart.Update(e.X, picDisplay.Width);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            uiTimer?.Stop();
            backgroundImage?.Dispose(); // Освобождаем ресурсы фона
            base.OnFormClosing(e);
        }
    }
}
    
