﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonEngine.Components.VisualFX;
using NeonEngine.Components.Graphics2D;

namespace NeonEngine
{
    public enum MovePattern
    {
        Linear,
        Sinusoidal,
        Curve,
    }

    public class Particle : DrawableComponent
    {
        public Vector2 Direction;

        public float StartingSpeed;
        public float EndingSpeed;
        private float _speed;

        public float AngularVelocity;
        
        public Vector2 Position;
        public float Angle;

        public float Duration;
        private float _duration;

        public float FadeInDelay;
        public float FadeOutDelay;

        public Texture2D Texture;
        public SpriteSheet spriteSheet;
        public MovePattern ParticleMovement;
        
        public Color StartingColor;
        public Color EndingColor;

        public float StartingBrightness;
        public float EndingBrightness;
        private float _brightness;
        
        public float StartingOpacity;
        public float EndingOpacity;
        private float _opacity;
       

        public float StartingScale;
        public float EndingScale;

        public float Scale;

        public ParticleEmitter Emitter;
        public bool IsAnimated = false;

        public Particle()
            : base(1.0f ,null, "Particle")
        {  
        }

        public Particle(Entity entity)
            : base(1.0f, entity, "Particle")
        {
        }

        public override void Init()
        {
            _duration = 0;



            if (FadeInDelay > 0f)
            {
                _opacity = 0.0f;
            }

            base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            if (_duration < Duration)
            {
                ManageOpacity(gameTime);
                ManageColor(gameTime);
                ManageBrightness(gameTime);
                ManageScale(gameTime);
                ManageSpeed(gameTime);

                ManageMovement(gameTime);

                _duration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (spriteSheet != null)
                {
                    spriteSheet.Update(gameTime);
                    spriteSheet.Opacity = _opacity;
                    spriteSheet.TintColor = this.TintColor;
                    spriteSheet.scale = Scale;
                    spriteSheet.TintColor = Color.Lerp(Color.White, TintColor, _brightness);
                }
            }
            else
            {
                this.Remove();
            }
            base.Update(gameTime);
        }

        private void ManageOpacity(GameTime gameTime)
        {
            if (FadeInDelay > 0f && _duration < FadeInDelay)
            {
                _opacity = (_duration / FadeInDelay) * StartingOpacity;
            }
            else if (_duration > Duration - FadeOutDelay)
            {
                _opacity = ((_duration - (Duration - FadeOutDelay) / FadeOutDelay) - 1) * -1 * EndingOpacity;
            }
            else
            {
                _opacity = StartingOpacity + ((_duration - FadeInDelay) / (Duration - FadeOutDelay - FadeInDelay)) * (EndingOpacity - StartingOpacity);
            }
        }

        private void ManageColor(GameTime gameTime)
        {
            TintColor = Color.Lerp(StartingColor, EndingColor, _duration / Duration);
        }

        private void ManageBrightness(GameTime gameTime)
        {
            _brightness = MathHelper.Lerp(StartingBrightness, EndingBrightness, _duration / Duration);
            TintColor = Color.Lerp(Color.White, TintColor, _brightness);
        }

        private void ManageScale(GameTime gameTime)
        {
            Scale = MathHelper.Lerp(StartingScale, EndingScale, _duration / Duration);
        }

        private void ManageSpeed(GameTime gameTime)
        {
            _speed = MathHelper.Lerp(StartingSpeed, EndingSpeed, _duration / Duration);
        }

        private void ManageMovement(GameTime gameTime)
        {
            switch(ParticleMovement)
            {
                case MovePattern.Linear:
                    Position += Direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;

                case MovePattern.Curve:
                    break;

                case MovePattern.Sinusoidal:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (spriteSheet != null)
                spriteSheet.Draw(spriteBatch);
            else if(Texture != null)
                spriteBatch.Draw(Texture, Position, null, Color.Lerp(TintColor, Color.Transparent, _opacity), Angle, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, Layer);
            base.Draw(spriteBatch);
        }

        public override void Remove()
        {
            Emitter.particles.Remove(this);
            Neon.World.ParticlePool.FlagAvailableItem(this);
            base.Remove();
        }
    }
}
