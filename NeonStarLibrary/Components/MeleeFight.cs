﻿using Microsoft.Xna.Framework;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public enum ComboSequence
    {
        None,
        Starter,
        Link,
        Finish
    }

    public class MeleeFight : Component
    {
        private bool _Debug = false;

        public float DamageModifier = 1.0f;
        public float DamageModifierTimer = 0.0f;

        public bool Debug
        {
            get { return _Debug; }
            set { _Debug = value; }
        }

        public Attack CurrentAttack;

        private string _lightAttackName;

        public string LightAttackName
        {
            get { return _lightAttackName; }
            set { _lightAttackName = value; }
        }

        private string _rushAttackName;

        public string RushAttackName
        {
            get { return _rushAttackName; }
            set { _rushAttackName = value; }
        }

        private string _uppercutName;

        public string UppercutName
        {
            get { return _uppercutName; }
            set { _uppercutName = value; }
        }

        private string _diveAttackName;

        public string DiveAttackName
        {
            get { return _diveAttackName; }
            set { _diveAttackName = value; }
        }

        private float _comboDelayMax = 1.0f;
        public float ComboDelayMax
        {
            get { return _comboDelayMax; }
            set { _comboDelayMax = value; }
        }

        private string _lightAttackAnimation;
        public string LightAttackAnimation
        {
            get { return _lightAttackAnimation; }
            set { _lightAttackAnimation = value; }
        }

        private string _uppercutAnimation;
        public string UppercutAnimation
        {
            get { return _uppercutAnimation; }
            set { _uppercutAnimation = value; }
        }

        private string _rushAttackAnimation;
        public string RushAttackAnimation
        {
            get { return _rushAttackAnimation; }
            set { _rushAttackAnimation = value; }
        }

        private string _diveAttackStartAnimation;
        public string DiveAttackStartAnimation
        {
            get { return _diveAttackStartAnimation; }
            set { _diveAttackStartAnimation = value; }
        }

        private string _diveAttackLoopAnimation;
        public string DiveAttackLoopAnimation
        {
            get { return _diveAttackLoopAnimation; }
            set { _diveAttackLoopAnimation = value; }
        }

        private string _diveAttackLandAnimation;

        public string DiveAttackLandAnimation
        {
            get { return _diveAttackLandAnimation; }
            set { _diveAttackLandAnimation = value; }
        }

        private float _rushAttackSideDelay = 1.0f;

        public float RushAttackSideDelay
        {
            get { return _rushAttackSideDelay; }
            set { _rushAttackSideDelay = value; }
        }

        private float _lastHitDelay = 0.0f;
        private bool ReleasedAttackButton = true;

        private float _chainDelay = 0.3f;

        public float ChainDelay
        {
            get { return _chainDelay; }
            set { _chainDelay = value; }
        }

        private float _rushAttackStickDelay = 0.3f;

        public float RushAttackStickDelay
        {
            get { return _rushAttackStickDelay; }
            set { _rushAttackStickDelay = value; }
        }

        public List<string> AttacksWhileInAir = new List<string>();

        private float _chainDelayTimer = 0.0f;
        private string NextAttack = "";


        private ThirdPersonController _thirdPersonController;
        public ThirdPersonController ThirdPersonController
        {
            get { return _thirdPersonController; }
            set { _thirdPersonController = value; }
        }

        public ComboSequence LastComboHit = ComboSequence.None;

        private bool _triedAttacking = false;
        private ComboSequence _currentComboHit = ComboSequence.None;

        public ComboSequence CurrentComboHit
        {
            get { return _currentComboHit; }
            set { _currentComboHit = value; }
        }

        private Avatar _avatar;
        public bool CanAttack = true;

        public MeleeFight(Entity entity)
            :base(entity, "MeleeFight")
        {
        }

        public override void Init()
        {
            _avatar = entity.GetComponent<Avatar>();
            base.Init();
        }

        public override void PreUpdate(GameTime gameTime)
        {
            if (DamageModifierTimer > 0.0f)
                DamageModifierTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                DamageModifier = 1.0f;

            if (CurrentAttack != null && (CurrentAttack.AttackInfo.Name == _diveAttackName || CurrentAttack.AttackInfo.Name == _diveAttackName + "Finish"))
            {
                if (entity.spritesheets.CurrentSpritesheetName == DiveAttackStartAnimation && entity.spritesheets.IsFinished())
                {
                    entity.spritesheets.ChangeAnimation(DiveAttackLoopAnimation, 1, true, false, true);
                }
                else if (entity.spritesheets.CurrentSpritesheetName == DiveAttackLoopAnimation && entity.rigidbody.isGrounded)
                {
                    entity.spritesheets.ChangeAnimation(DiveAttackLandAnimation, 1, true, false, false);
                }
            }

            if (CurrentAttack == null)
            {
                if (_avatar != null && _avatar.StunLockDuration > 0)
                {
                    ThirdPersonController.CanMove = false;
                    ThirdPersonController.CanTurn = false;
                }
            }
            else
            {
                if (((CurrentAttack.DelayStarted && !CurrentAttack.DelayFinished) || (CurrentAttack.DurationStarted && !CurrentAttack.DurationFinished) || (CurrentAttack.CooldownStarted && !CurrentAttack.CooldownFinished)))
                {
                    ThirdPersonController.CanMove = false;
                    ThirdPersonController.CanTurn = false;
                }
                else
                {
                    if (_avatar != null && _avatar.StunLockDuration > 0)
                    {
                        CurrentAttack.AirLock = 0.0f;
                        ThirdPersonController.CanMove = false;
                        ThirdPersonController.CanTurn = false;
                    }
                    else if (CurrentAttack.AirLocked)
                    {
                        ThirdPersonController.CanMove = false;
                    }
                }

            }
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {  
            if (!ReleasedAttackButton)
                if (!Neon.Input.Check(NeonStarInput.Attack))
                    ReleasedAttackButton = true;

            if (entity.rigidbody.isGrounded)
            {
                AttacksWhileInAir.Clear();
            }

            

            if (CurrentAttack != null)
            {
                if (CurrentAttack.CooldownFinished && CurrentAttack.AirLockFinished && (entity.spritesheets.CurrentSpritesheet.IsLooped || !entity.spritesheets.CurrentSpritesheet.IsLooped && entity.spritesheets.IsFinished()))
                {
                    CurrentAttack = null;
                    entity.spritesheets.CurrentPriority = 0;
                }
                else
                    CurrentAttack.Update(gameTime);
            }        

            if (CanAttack && (CurrentAttack == null || (CurrentAttack != null && CurrentAttack.CooldownFinished)) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
            {
                if (NextAttack != "")
                {
                    if (Neon.Input.Check(NeonStarInput.MoveLeft))
                    {
                        if (ThirdPersonController.CurrentSide != Side.Left)
                        {
                            ThirdPersonController.CurrentSide = Side.Left;
                            ThirdPersonController.LastSideChangedDelay = 0.0f;
                            entity.spritesheets.ChangeSide(ThirdPersonController.CurrentSide);
                        }
                    }
                    else if (Neon.Input.Check(NeonStarInput.MoveRight))
                    {
                        if (ThirdPersonController.CurrentSide != Side.Right)
                        {
                            ThirdPersonController.CurrentSide = Side.Right;
                            ThirdPersonController.LastSideChangedDelay = 0.0f;
                            entity.spritesheets.ChangeSide(ThirdPersonController.CurrentSide);
                        }
                    }

                    if (_chainDelayTimer < _chainDelay)
                    {
                        switch (NextAttack)
                        {
                            case "Uppercut":
                                PerformUppercut();
                                break;

                            case "DiveAttack":
                                PerformDiveAttack();
                                break;

                            case "LeftRushAttack": 
                                if(ThirdPersonController.CurrentSide == Side.Left && _rushAttackSideDelay <= ThirdPersonController.LastSideChangedDelay)
                                    PerformLeftRushAttack();
                                else
                                    PerformLightAttack();
                                break;

                            case "RightRushAttack":
                                if (ThirdPersonController.CurrentSide == Side.Right && _rushAttackSideDelay <= ThirdPersonController.LastSideChangedDelay)
                                    PerformRightRushAttack();
                                else
                                    PerformLightAttack();
                                break;

                            case "LightAttack":
                                PerformLightAttack();
                                break;
                        }
                        NextAttack = "";
                        _chainDelayTimer = 0.0f;
                    }
                    else
                    {
                        NextAttack = "";
                        _chainDelayTimer = 0.0f;
                    }
                }
                
            }

            if (NextAttack != "")
            {
                _chainDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (ReleasedAttackButton && CanAttack)
            {
                if (Neon.Input.PressedComboInput(NeonStarInput.Attack, 0.0f, NeonStarInput.MoveUp))
                {
                    if ((CurrentAttack == null || CurrentAttack != null && (CurrentAttack.CooldownFinished || (CurrentAttack.Type == AttackType.MeleeLight && CurrentAttack.DurationFinished))) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
                    {
                        PerformUppercut();
                    }
                    else if(NextAttack == "")
                    {
                        NextAttack = "Uppercut";
                        _chainDelayTimer = 0;
                    }
                }
                else if (Neon.Input.PressedComboInput(NeonStarInput.Attack, 0.0f, NeonStarInput.MoveLeft) && Neon.Input.CheckPressedDelay(NeonStarInput.MoveLeft, _rushAttackStickDelay) == DelayStatus.Valid)
                {
                    if (_rushAttackSideDelay <= ThirdPersonController.LastSideChangedDelay)
                    {
                        if ((CurrentAttack == null || CurrentAttack != null && (CurrentAttack.CooldownFinished || (CurrentAttack.Type == AttackType.MeleeLight && CurrentAttack.DurationFinished))) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
                        {
                            PerformLeftRushAttack();
                        }
                        else if(NextAttack == "")
                        {
                            NextAttack = "LeftRushAttack";
                            _chainDelayTimer = 0;
                        }
                    }
                    else
                    {
                        if (CurrentAttack == null || CurrentAttack != null && CurrentAttack.CooldownFinished)
                        {
                            PerformLightAttack();
                        }
                    }                                        
                }
                else if (Neon.Input.PressedComboInput(NeonStarInput.Attack, 0.0f, NeonStarInput.MoveRight) && Neon.Input.CheckPressedDelay(NeonStarInput.MoveRight, _rushAttackStickDelay) == DelayStatus.Valid && entity.spritesheets.CurrentSide == Side.Right)
                {
                    if (_rushAttackSideDelay <= ThirdPersonController.LastSideChangedDelay)
                    {
                        if ((CurrentAttack == null || CurrentAttack != null && (CurrentAttack.CooldownFinished || (CurrentAttack.Type == AttackType.MeleeLight && CurrentAttack.DurationFinished))) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
                        {
                            PerformRightRushAttack();
                        }
                        else if (NextAttack == "")
                        {
                            NextAttack = "RightRushAttack";
                            _chainDelayTimer = 0;
                        }
                    }
                    else
                    {
                        if (CurrentAttack == null || CurrentAttack != null && CurrentAttack.CooldownFinished)
                        {
                            PerformLightAttack();
                        }
                    }                    
                }
                else if (Neon.Input.PressedComboInput(NeonStarInput.Attack, 0.0f, NeonStarInput.MoveDown))                  
                {
                    if ((CurrentAttack == null || CurrentAttack != null && (CurrentAttack.CooldownFinished || (CurrentAttack.Type == AttackType.MeleeLight && CurrentAttack.DurationFinished))) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
                    {
                        PerformDiveAttack();
                        if (CurrentAttack == null)
                        {
                            PerformLightAttack();
                        }
                    }
                    else if (NextAttack == "")
                    {
                        NextAttack = "DiveAttack";
                        _chainDelayTimer = 0;
                    }                
                }
                else if (Neon.Input.Pressed(NeonStarInput.Attack) && !_triedAttacking)
                {
                    if ((CurrentAttack == null || (CurrentAttack != null && CurrentAttack.CooldownFinished)) && (_avatar != null && _avatar.StunLockDuration <= 0.0f))
                        PerformLightAttack();
                    else if (NextAttack == "")
                    {
                        NextAttack = "LightAttack";
                        _chainDelayTimer = 0;
                    }
                }
                else if(Neon.Input.Pressed(NeonStarInput.Jump) && !ThirdPersonController.StartJumping)
                {
                    ThirdPersonController._mustJumpAsSoonAsPossible = true;
                }
                
            }

            if (_currentComboHit != ComboSequence.None && (CurrentAttack != null && CurrentAttack.DurationFinished) || CurrentAttack == null)
            {
                _lastHitDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(_lastHitDelay >= ComboDelayMax)
                {
                    _currentComboHit = ComboSequence.None;
                    LastComboHit = ComboSequence.None;
                    _lastHitDelay = 0.0f;
                }    
            }

            if (_currentComboHit == ComboSequence.None)
            {
                _lastHitDelay = 0.0f;
            }

            

            if (CurrentAttack != null)
            {
                if (CurrentAttack.AirLocked)
                {
                    entity.rigidbody.body.GravityScale = 0.0f;                    
                }

                if (CurrentAttack.DurationFinished && entity.spritesheets.IsFinished())
                {
                    
                    entity.spritesheets.CurrentPriority = 0;
                    entity.spritesheets.ChangeAnimation(ThirdPersonController.IdleAnimation);
                }
            }

            
            _triedAttacking = false;
            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {

            CanAttack = true;
            base.PostUpdate(gameTime);
        }

        private void PerformUppercut()
        {
            CheckComboHit();
            if (_currentComboHit == ComboSequence.Finish)
            {
                CurrentAttack = AttacksManager.GetAttack(_uppercutName+"Finish", entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeAnimation(UppercutAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
            }
            else
            {
                CurrentAttack = AttacksManager.GetAttack(_uppercutName, entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeAnimation(UppercutAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }

            }
            ReleasedAttackButton = false;
            _triedAttacking = true;
        }

        private void PerformLeftRushAttack()
        {
            CheckComboHit();
            if (_currentComboHit == ComboSequence.Finish)
            {               
                CurrentAttack = AttacksManager.GetAttack(_rushAttackName+"Finish", Side.Left, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeSide(Side.Left);
                    entity.spritesheets.ChangeAnimation(RushAttackAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
            }
            else
            {
                CurrentAttack = AttacksManager.GetAttack(_rushAttackName, Side.Left, entity);
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeSide(Side.Left);
                    entity.spritesheets.ChangeAnimation(RushAttackAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }                
            }

            ReleasedAttackButton = false;
            _triedAttacking = true;
        }

        private void PerformRightRushAttack()
        {
            CheckComboHit();
            if (_currentComboHit == ComboSequence.Finish)
            {
                CurrentAttack = AttacksManager.GetAttack(_rushAttackName + "Finish", Side.Right, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeSide(Side.Right);
                    entity.spritesheets.ChangeAnimation(RushAttackAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
            }
            else
            {
                CurrentAttack = AttacksManager.GetAttack(_rushAttackName, Side.Right, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeSide(Side.Right);
                    entity.spritesheets.ChangeAnimation(RushAttackAnimation, 1, true, true, false);
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
            }
            ReleasedAttackButton = false;
            _triedAttacking = true;
        }

        private void PerformDiveAttack()
        {
            CheckComboHit();
            if (_currentComboHit == ComboSequence.Finish)
            {
                CurrentAttack = AttacksManager.GetAttack(_diveAttackName+"Finish", entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                    entity.spritesheets.ChangeAnimation(DiveAttackStartAnimation, 1, true, true, false);
            }
            else
            {
                CurrentAttack = AttacksManager.GetAttack(_diveAttackName, entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                    entity.spritesheets.ChangeAnimation(DiveAttackStartAnimation, 1, true, true, false);
            }
            ReleasedAttackButton = false;
        }

        private void PerformLightAttack()
        {
            CheckComboHit();
            if (_currentComboHit == ComboSequence.Finish)
            {
                CurrentAttack = AttacksManager.GetAttack(_lightAttackName+"Finish", entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    entity.spritesheets.ChangeAnimation(LightAttackAnimation + "Finish", 1, true, true, false);
                    //entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
            }
            else
            {
                CurrentAttack = AttacksManager.GetAttack(_lightAttackName, entity.spritesheets.CurrentSide, entity);
                if (DamageModifierTimer > 0.0f)
                {
                    CurrentAttack.DamageOnHit *= DamageModifier;
                }
                if (!CurrentAttack.Canceled)
                {
                    if (_currentComboHit == ComboSequence.Starter)
                        entity.spritesheets.ChangeAnimation(LightAttackAnimation + "Starter", 1, true, true, false);
                    else
                        entity.spritesheets.ChangeAnimation(LightAttackAnimation + "Link", 1, true, true, false);

                    //entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                }
                           
            }
            ReleasedAttackButton = false;
        }

        

        private void CheckComboHit()
        {
            LastComboHit = _currentComboHit;
            if (_currentComboHit != ComboSequence.None)
            {
                if (_lastHitDelay < ComboDelayMax)
                {
                    switch (_currentComboHit)
                    {
                        case ComboSequence.Starter:
                            _currentComboHit = ComboSequence.Link;
                            break;

                        case ComboSequence.Link:
                            _currentComboHit = ComboSequence.Finish;
                            break;

                        case ComboSequence.Finish:
                            _currentComboHit = ComboSequence.Starter;
                            break;
                    }
                }
                else
                {
                    _currentComboHit = ComboSequence.Starter;
                }
            }
            else
            {
                _currentComboHit = ComboSequence.Starter;
            }

            _lastHitDelay = 0.0f;
        }

        public void ResetComboHit()
        {
            _currentComboHit = ComboSequence.None;
        }
    }
}
