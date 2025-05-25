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
        }
}