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
    }
}