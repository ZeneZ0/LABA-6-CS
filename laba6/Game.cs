using System;
using System.Collections.Generic;
using System.Drawing;

namespace FallingParticlesGame
{

    public class Game
    {
        public enum ParticleEffect
        {
            None,
            DoubleSize,
            Freeze,
            HalfSize,
            BonusPoints,
            Steam,
            Invulnerability
        }

        public class Game
        {
            public List<Particle> Particles { get; } = new List<Particle>();
            public List<Particle> SteamParticles { get; } = new List<Particle>();
            public List<Explosion> Explosions { get; } = new List<Explosion>();
            public Cart Cart { get; } = new Cart();
            public int Score { get; private set; }
            public bool IsGameOver { get; private set; }
            public int Lives { get; private set; } = 3;
            public bool IsInvulnerable { get; private set; }
            public int SpawnRate { get; set; } = 50;
            public int ParticleSpeed { get; set; } = 3;
            public int ParticleSize { get; set; } = 10;
            public ParticleEffect CurrentEffect { get; private set; } = ParticleEffect.None;
            public DateTime EffectEndTime { get; private set; }
            public event Action OnHit;

            private readonly Random random = new Random();
            private int spawnCounter;
            private readonly int screenWidth;
            private readonly int originalCartWidth;
            private DateTime invulnerabilityEndTime;

            public Game(int screenWidth, int screenHeight)
            {
                this.screenWidth = screenWidth;
                Cart.Y = screenHeight - 50;
                originalCartWidth = Cart.BaseWidth;
            }

            public void Update()
            {
                if (IsGameOver) return;

                CheckEffects();
                SpawnParticles();
                UpdateParticles();
                GenerateSteam();
                UpdateSteam();
                UpdateExplosions();
            }

            private void CheckEffects()
            {
                if (CurrentEffect != ParticleEffect.None && DateTime.Now >= EffectEndTime)
                {
                    ResetCurrentEffect();
                }

                if (IsInvulnerable && DateTime.Now >= invulnerabilityEndTime)
                {
                    IsInvulnerable = false;
                }
            }

            private void SpawnParticles()
            {
                spawnCounter++;
                if (spawnCounter >= 100 - SpawnRate)
                {
                    CreateParticle();
                    spawnCounter = 0;
                }
            }

            private void CreateParticle()
            {
                var particle = new Particle
                {
                    X = random.Next(0, screenWidth),
                    Y = -20,
                    Size = ParticleSize,
                    Speed = ParticleSpeed
                };

                int rnd = random.Next(100);
                if (rnd < 15) // 15% красные (опасные)
                {
                    particle.Color = Color.Red;
                    particle.Effect = ParticleEffect.None;
                }
                else if (rnd < 30) // 15% фиолетовые (неуязвимость)
                {
                    particle.Color = Color.Purple;
                    particle.Effect = ParticleEffect.Invulnerability;
                }
                else if (rnd < 45) // 15% зелёные
                {
                    particle.Color = Color.Green;
                    particle.Effect = ParticleEffect.None;
                }
                else if (rnd < 60) // 15% жёлтые
                {
                    particle.Color = Color.Yellow;
                    particle.Effect = ParticleEffect.DoubleSize;
                }
                else if (rnd < 75) // 15% синие
                {
                    particle.Color = Color.Blue;
                    particle.Effect = ParticleEffect.Freeze;
                }
                else if (rnd < 90) // 15% розовые
                {
                    particle.Color = Color.Pink;
                    particle.Effect = ParticleEffect.HalfSize;
                }
                else // 10% бирюзовые
                {
                    particle.Color = Color.Turquoise;
                    particle.Effect = ParticleEffect.BonusPoints;
                }

                Particles.Add(particle);
            }
        }

        private void UpdateParticles()
        {
            foreach (var particle in Particles.ToArray())
            {
                particle.Y += particle.Speed;

                if (Cart.Bounds.IntersectsWith(new Rectangle(
                    (int)particle.X, (int)particle.Y,
                    (int)particle.Size, (int)particle.Size)))
                {
                    HandleParticleCollision(particle);
                }

                if (particle.IsCollected || particle.Y > Cart.Y + 100)
                {
                    Particles.Remove(particle);
                }
            }
        }

        private void HandleParticleCollision(Particle particle)
        {
            particle.IsCollected = true;
            CreateExplosion(particle);

            if (particle.IsDangerous && !IsInvulnerable)
            {
                Lives--;
                OnHit?.Invoke();

                if (Lives <= 0)
                {
                    IsGameOver = true;
                    return;
                }
            }

            ApplyParticleEffect(particle);
        }

        private void CreateExplosion(Particle particle)
        {
            Explosions.Add(new Explosion
            {
                Location = new Point(particle.X + particle.Size / 2, particle.Y + particle.Size / 2),
                Color = particle.Color,
                Radius = particle.Size,
                LifeTime = 10
            });
        }

        private void UpdateExplosions()
        {
            foreach (var explosion in Explosions.ToArray())
            {
                explosion.LifeTime--;
                if (explosion.LifeTime <= 0)
                {
                    Explosions.Remove(explosion);
                }
            }
        }

        private void ApplyParticleEffect(Particle particle)
        {
            ResetCurrentEffect();
            CurrentEffect = particle.Effect;

            switch (particle.Effect)
            {
                case ParticleEffect.DoubleSize:
                    Cart.Width = originalCartWidth * 2;
                    EffectEndTime = DateTime.Now.AddSeconds(5);
                    break;

                case ParticleEffect.Freeze:
                    Cart.IsFrozen = true;
                    EffectEndTime = DateTime.Now.AddSeconds(2);
                    break;

                case ParticleEffect.HalfSize:
                    Cart.Width = originalCartWidth / 2;
                    EffectEndTime = DateTime.Now.AddSeconds(5);
                    break;

                case ParticleEffect.BonusPoints:
                    Score += 5;
                    CurrentEffect = ParticleEffect.None;
                    break;

                case ParticleEffect.Invulnerability:
                    ActivateInvulnerability(5);
                    CurrentEffect = ParticleEffect.None;
                    break;

                case ParticleEffect.None:
                    Score += 1;
                    break;
            }
        }

        public void ActivateInvulnerability(float seconds)
        {
            IsInvulnerable = true;
            invulnerabilityEndTime = DateTime.Now.AddSeconds(seconds);
        }

        private void GenerateSteam()
        {
            if (Math.Abs(Cart.LastMovement) > 0.1f && random.Next(10) < 3)
            {
                SteamParticles.Add(new Particle
                {
                    X = Cart.X + Cart.Width / 2 + random.Next(-10, 10),
                    Y = Cart.Y,
                    Size = 5 + random.Next(10),
                    Color = Color.WhiteSmoke,
                    Effect = ParticleEffect.Steam,
                    Speed = -1 - random.Next(3)
                });
            }
        }

        private void UpdateSteam()
        {
            foreach (var steam in SteamParticles.ToArray())
            {
                steam.Y += steam.Speed;
                steam.Size = (int)Math.Max(0, steam.Size - 0.1f);

                if (steam.Size <= 0)
                {
                    SteamParticles.Remove(steam);
                }
            }
        }

        private void ResetCurrentEffect()
        {
            Cart.ResetSize();
            CurrentEffect = ParticleEffect.None;
        }

        public void Draw(Graphics g)
        {
            foreach (var steam in SteamParticles)
            {
                steam.Draw(g);
            }

            foreach (var explosion in Explosions)
            {
                explosion.Draw(g);
            }
        }

        public void Reset()
        {
            Particles.Clear();
            SteamParticles.Clear();
            Explosions.Clear();
            Score = 0;
            Lives = 3;
            IsGameOver = false;
            CurrentEffect = ParticleEffect.None;
            IsInvulnerable = false;
            Cart.ResetSize();
        }
    }

    public class Explosion
    {
        public Point Location;
        public int Radius;
        public int LifeTime;
        public Color Color;

        public void Draw(Graphics g)
        {
            var alpha = (int)(255 * (LifeTime / 10f));
            using (var brush = new SolidBrush(Color.FromArgb(alpha, Color)))
            {
                g.FillEllipse(brush,
                    Location.X - Radius, Location.Y - Radius,
                    Radius * 2, Radius * 2);
            }
        }
    }
}