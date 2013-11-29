﻿using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public enum Element
    {
        Neutral,
        Fire,
        Thunder
    }

    public class ElementSystem : Component
    {
        #region Properties
        private string _fireLaunchAnimation = "";

        public string FireLaunchAnimation
        {
            get { return _fireLaunchAnimation; }
            set { _fireLaunchAnimation = value; }
        }

        private string _thunderLaunchAnimation = "";

        public string ThunderLaunchAnimation
        {
            get { return _thunderLaunchAnimation; }
            set { _thunderLaunchAnimation = value; }
        }

        private Element _leftSlotElement = Element.Neutral;

        public Element LeftSlotElement
        {
            get { return _leftSlotElement; }
            set { _leftSlotElement = value; }
        }

        private float _leftSlotLevel = 1;

        public float LeftSlotLevel
        {
            get { return _leftSlotLevel; }
            set { _leftSlotLevel = value; }
        }

        private float _leftSlotCooldownTimer = 0.0f;

        public float LeftSlotCooldownTimer
        {
            get { return _leftSlotCooldownTimer; }
            set { _leftSlotCooldownTimer = value; }
        }

        private float _rightSlotCooldownTimer = 0.0f;

        public float RightSlotCooldownTimer
        {
            get { return _rightSlotCooldownTimer; }
            set { _rightSlotCooldownTimer = value; }
        }

        private Element _rightSlotElement = Element.Neutral;

        public Element RightSlotElement
        {
            get { return _rightSlotElement; }
            set { _rightSlotElement = value; }
        }

        private float _rightSlotLevel = 1;

        public float RightSlotLevel
        {
            get { return _rightSlotLevel; }
            set { _rightSlotLevel = value; }
        }

        private float _maxLevel = 3;

        public float MaxLevel
        {
            get { return _maxLevel; }
            set { _maxLevel = value; }
        }
        #endregion

        public Avatar AvatarComponent = null;

        public ElementEffect CurrentElementEffect = null;

        public ElementSystem(Entity entity)
            :base(entity, "ElementSystem")
        {
        }

        public override void Init()
        {
            AvatarComponent = entity.GetComponent<Avatar>();
            base.Init();
        }

        public override void PreUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_leftSlotCooldownTimer > 0.0f)
            {
                _leftSlotCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_leftSlotCooldownTimer < 0.0f)
                    _leftSlotCooldownTimer = 0.0f;
            }

            if (_rightSlotCooldownTimer > 0.0f)
            {
                _rightSlotCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_rightSlotCooldownTimer < 0.0f)
                    _rightSlotCooldownTimer = 0.0f;
            }

            if (CurrentElementEffect != null)
            {
                AvatarComponent.State = AvatarState.UsingElement;
                CurrentElementEffect.PreUpdate(gameTime);
                AvatarComponent.CanUseElement = false;
            }
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (AvatarComponent.CanAttack && AvatarComponent.CanMove && AvatarComponent.CanTurn && AvatarComponent.CanUseElement)
            {
                if (Neon.Input.Pressed(NeonStarInput.UseLeftSlotElement) && _leftSlotCooldownTimer <= 0.0f)
                {
                    if (_leftSlotElement != Element.Neutral)
                    {
                        Console.WriteLine("Use Element -> " + LeftSlotElement);
                        UseElement(_leftSlotElement, (int)_leftSlotLevel, NeonStarInput.UseLeftSlotElement);
                    }
                }
                else if (Neon.Input.Pressed(NeonStarInput.UseRightSlotElement) && _rightSlotCooldownTimer <= 0.0f)
                {
                    if (_rightSlotElement != Element.Neutral)
                    {
                        Console.WriteLine("Use Element -> " + RightSlotElement);
                        UseElement(_rightSlotElement, (int)_rightSlotLevel, NeonStarInput.UseRightSlotElement);
                    }
                }

                if (Neon.Input.Pressed(NeonStarInput.DropLeftSlotElement))
                {
                    if (_leftSlotElement != Element.Neutral)
                    {
                        Console.WriteLine("Drop Element -> " + LeftSlotElement);
                        _leftSlotElement = Element.Neutral;
                        _leftSlotLevel = 1;
                    }
                }
                if (Neon.Input.Pressed(NeonStarInput.DropRightSlotElement))
                {
                    if (_rightSlotElement != Element.Neutral)
                    {
                        Console.WriteLine("Drop Element -> " + RightSlotElement);
                        _rightSlotElement = Element.Neutral;
                        _rightSlotLevel = 1;
                    }
                }
            }
            else if (CurrentElementEffect != null)
            {
                CurrentElementEffect.Update(gameTime);
            }
        }

        public override void PostUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (CurrentElementEffect != null)
            {
                CurrentElementEffect.PostUpdate(gameTime);
                if (CurrentElementEffect.State == ElementState.End)
                {
                    CurrentElementEffect = null;
                    AvatarComponent.State = AvatarState.Idle;
                }
            }
            base.PostUpdate(gameTime);
        }

        public void UseElement(Element element, int level, NeonStarInput input)
        {
            switch(element)
            {
                case Element.Fire:
                    CurrentElementEffect = new Fire(this, level, entity, input, (GameScreen)entity.containerWorld);
                    break;

                case Element.Thunder:
                    CurrentElementEffect = new Thunder(this, level, entity, input, (GameScreen)entity.containerWorld);
                    break;
            }
        }

        public void GetElement(Element element)
        {
            if (_leftSlotElement == element)
            {
                if (_leftSlotLevel < _maxLevel)
                    _leftSlotLevel++;
                Console.WriteLine("Left Slot Level Up -> " + _leftSlotLevel);
            }
            else if (_rightSlotElement == element)
            {
                if (_rightSlotLevel < _maxLevel)
                    _rightSlotLevel++;
                Console.WriteLine("Right Slot Level Up -> " + _rightSlotLevel);
            }
            else if (_leftSlotElement == Element.Neutral)
            {
                _leftSlotElement = element;
                Console.WriteLine("Got " + element + " in Left Slot");
            }
            else if (_rightSlotElement == Element.Neutral)
            {
                _rightSlotElement = element;
                Console.WriteLine("Got " + element + " in Right Slot");
            }
            else
            {
                Console.WriteLine("Fizzle");
            }
        }
    }
}
