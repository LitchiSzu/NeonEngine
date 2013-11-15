﻿using System;
using NeonEngine;
using NeonStarLibrary;
using NeonEngine.Private;
using Microsoft.Xna.Framework;

namespace NeonScripts
{
    public class ButtonsScript : ScriptComponent
    {
        private string _avatarName = "";

        public string AvatarName
        {
            get { return _avatarName; }
            set { _avatarName = value; }
        }

        private string _firstButtonName = "";

        public string FirstButtonName
        {
            get { return _firstButtonName; }
            set { _firstButtonName = value; }
        }

        private string _firstButtonNeededAttack = "";

        public string FirstButtonNeededAttack
        {
            get { return _firstButtonNeededAttack; }
            set { _firstButtonNeededAttack = value; }
        }

        private string _firstButtonSpritesheetName = "";

        public string FirstButtonSpritesheetName
        {
            get { return _firstButtonSpritesheetName; }
            set { _firstButtonSpritesheetName = value; }
        }

        private bool _firstButtonNeedsToSynchronise = false;

        public bool FirstButtonNeedsToSynchronise
        {
            get { return _firstButtonNeedsToSynchronise; }
            set { _firstButtonNeedsToSynchronise = value; }
        }

        private string _secondButtonName = "";

        public string SecondButtonName
        {
            get { return _secondButtonName; }
            set { _secondButtonName = value; }
        }

        private string _secondButtonNeededAttack = "";

        public string SecondButtonNeededAttack
        {
            get { return _secondButtonNeededAttack; }
            set { _secondButtonNeededAttack = value; }
        }

        private string _secondButtonSpritesheetName = "";

        public string SecondButtonSpritesheetName
        {
            get { return _secondButtonSpritesheetName; }
            set { _secondButtonSpritesheetName = value; }
        }

        private bool _secondButtonNeedsToSynchronize = false;

        public bool SecondButtonNeedsToSynchronize
        {
            get { return _secondButtonNeedsToSynchronize; }
            set { _secondButtonNeedsToSynchronize = value; }
        }

        private string _thirdButtonName = "";

        public string ThirdButtonName
        {
            get { return _thirdButtonName; }
            set { _thirdButtonName = value; }
        }

        private string _thirdButtonNeededAttack = "";

        public string ThirdButtonNeededAttack
        {
            get { return _thirdButtonNeededAttack; }
            set { _thirdButtonNeededAttack = value; }
        }

        private string _thirdButtonSpritesheetName = "";

        public string ThirdButtonSpritesheetName
        {
            get { return _thirdButtonSpritesheetName; }
            set { _thirdButtonSpritesheetName = value; }
        }

        private bool _thirdButtonNeedsToSynchronize = false;

        public bool ThirdButtonNeedsToSynchronize
        {
            get { return _thirdButtonNeedsToSynchronize; }
            set { _thirdButtonNeedsToSynchronize = value; }
        }

        private string _doorToOpenName = "";

        public string DoorToOpenName
        {
            get { return _doorToOpenName; }
            set { _doorToOpenName = value; }
        }

        private string _doorAnimation = "";

        public string DoorAnimation
        {
            get { return _doorAnimation; }
            set { _doorAnimation = value; }
        }

        private float _delay = 0.0f;

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        private Avatar _avatar = null;

        private Entity _firstButton = null;
        private Entity _secondButton = null;
        private Entity _thirdButton = null;
        private Entity _doorToOpen = null;

        private bool _firstButtonActivated = false;
        private bool _secondButtonActivated = false;
        private bool _thirdButtonActivated = false;

        private float _timer = 0.0f;

        public ButtonsScript(Entity entity)
            :base(entity, "ButtonsScript")
        {
        }
		
		public override void Init()
		{
            if (_avatarName != "")
                _avatar = Neon.world.GetEntityByName(_avatarName).GetComponent<Avatar>();

            if (_firstButtonName != "")
            {
                _firstButton = Neon.world.GetEntityByName(_firstButtonName);
                if (_firstButton != null)
                    _firstButton.spritesheets.ChangeAnimation(_firstButtonSpritesheetName, 0, false, true, false, 0);
            }
            if (_secondButtonName != "")
            {
                _secondButton = Neon.world.GetEntityByName(_secondButtonName);
                if (_secondButton != null)
                    _secondButton.spritesheets.ChangeAnimation(_secondButtonSpritesheetName, 0, false, true, false, 0);
            }
            if (_thirdButtonName != "")
            {
                _thirdButton = Neon.world.GetEntityByName(_thirdButtonName);
                if (_thirdButton != null)
                    _thirdButton.spritesheets.ChangeAnimation(_thirdButtonSpritesheetName, 0, false, true, false, 0);
            }
            if (_doorToOpenName != "")
            {
                _doorToOpen = Neon.world.GetEntityByName(_doorToOpenName);
                if (_doorToOpen != null)
                    _doorToOpen.spritesheets.ChangeAnimation(_doorAnimation, 0, false, true, false, 0);
            }
		}

        public override void Update(GameTime gameTime)
        {
            if (_timer > 0.0f)
            {
                _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer < 0.0f)
                {
                    if (_secondButtonActivated == true && _thirdButtonActivated == true)
                    {
                        _timer = 0.0f;
                    }
                    else
                    {
                        if (_secondButtonActivated == true)
                        {
                            _secondButtonActivated = false;
                            _secondButton.spritesheets.ChangeAnimation(_secondButtonSpritesheetName, 0, true, true, true, _secondButton.spritesheets.SpritesheetList[_secondButtonSpritesheetName].FrameCount - 1);
                            _secondButton.spritesheets.CurrentSpritesheet.ReverseLoop = true;
                        }
                        if (_thirdButtonActivated == true)
                        {
                            _thirdButtonActivated = false;
                            _thirdButton.spritesheets.ChangeAnimation(_thirdButtonSpritesheetName, 0, true, true, true, _thirdButton.spritesheets.SpritesheetList[_thirdButtonSpritesheetName].FrameCount - 1);
                            _thirdButton.spritesheets.CurrentSpritesheet.ReverseLoop = true;
                        }
                        _timer = 0.0f;
                    }
                }
            }

            if (_secondButton.spritesheets.CurrentSpritesheet.ReverseLoop == true && _secondButton.spritesheets.CurrentSpritesheet.currentFrame == 0)
            {
                _secondButton.spritesheets.CurrentSpritesheet.ReverseLoop = false;
                _secondButton.spritesheets.ChangeAnimation(_secondButtonSpritesheetName, 0, false, true, false, 0);
            }
            if (_thirdButton.spritesheets.CurrentSpritesheet.ReverseLoop == true && _thirdButton.spritesheets.CurrentSpritesheet.currentFrame == 0)
            {
                _thirdButton.spritesheets.CurrentSpritesheet.ReverseLoop = false;
                _thirdButton.spritesheets.ChangeAnimation(_thirdButtonSpritesheetName, 0, false, true, false, 0);
            }
        }
		
		public override void OnTrigger(Entity trigger, Entity triggeringEntity, object[] parameters = null)
		{
            if (trigger.Name == _firstButtonName && _firstButtonActivated == false && _avatar.meleeFight.CurrentAttack.Name == _firstButtonNeededAttack)
            {
                _firstButton.spritesheets.ChangeAnimation(_firstButtonSpritesheetName, 0, true, false, false, 0);
                _firstButtonActivated = true;
                if (_firstButtonNeedsToSynchronise == true)
                {
                    if (_timer > 0.0f)
                        Console.WriteLine("Two Synchronized Buttons Are Activated");
                    else
                        _timer = _delay;
                }
                if (_secondButtonActivated == true && _thirdButtonActivated == true)
                {
                    _doorToOpen.rigidbody.Remove();
                    _doorToOpen.spritesheets.ChangeAnimation(_doorAnimation, 0, true, false, false, 0);
                }
            }
            if (trigger.Name == _secondButtonName && _secondButtonActivated == false && _avatar.meleeFight.CurrentAttack.Name == _secondButtonNeededAttack)
            {
                _secondButton.spritesheets.ChangeAnimation(_secondButtonSpritesheetName, 0, true, false, false, 0);
                _secondButtonActivated = true;
                if (_secondButtonNeedsToSynchronize == true)
                {
                    if (_timer > 0.0f && _doorToOpen != null)
                    {
                        if (_firstButtonActivated == true)
                        {
                            _doorToOpen.rigidbody.Remove();
                            _doorToOpen.spritesheets.ChangeAnimation(_doorAnimation, 0, true, false, false, 0);
                        }
                    }
                    else
                        _timer = _delay;
                }
            }
            if (trigger.Name == _thirdButtonName && _thirdButtonActivated == false && _avatar.meleeFight.CurrentAttack.Name == _thirdButtonNeededAttack)
            {
                _thirdButton.spritesheets.ChangeAnimation(_thirdButtonSpritesheetName, 0, true, false, false, 0);
                _thirdButtonActivated = true;
                if (_thirdButtonNeedsToSynchronize == true)
                {
                    if (_timer > 0.0f && _doorToOpen != null)
                    {
                        if (_firstButtonActivated == true)
                        {
                            _doorToOpen.rigidbody.Remove();
                            _doorToOpen.spritesheets.ChangeAnimation(_doorAnimation, 0, true, false, false, 0);
                        }
                    }
                    else
                        _timer = _delay;
                }
            }
		}
    }
}
