﻿using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NeonEngine;
using NeonEngine.Components.CollisionDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Avatar
{
    public class Guard : Component
    {
        #region Properties
        private string _rollAnimation = "";

        public string RollAnimation
        {
            get { return _rollAnimation; }
            set { _rollAnimation = value; }
        }

        private string _dashAnimation = "";

        public string DashAnimation
        {
            get { return _dashAnimation; }
            set { _dashAnimation = value; }
        }

        private string _dashEffectSpriteSheetTag = "";

        public string DashEffectSpriteSheetTag
        {
            get { return _dashEffectSpriteSheetTag; }
            set
            { 
                _dashEffectSpriteSheetTag = value;
                _dashEffectSpritesheet = AssetManager.GetSpriteSheet(value);
            }
        }

        private string _guardAnimation = "";

        public string GuardAnimation
        {
            get { return _guardAnimation; }
            set { _guardAnimation = value; }
        }

        private float _rollDuration = 0.0f;

        public float RollDuration
        {
            get { return _rollDuration; }
            set { _rollDuration = value; }
        }

        private float _dashDuration = 0.0f;

        public float DashDuration
        {
            get { return _dashDuration; }
            set { _dashDuration = value; }
        }

        private float _guardDuration = 0.0f;

        public float GuardDuration
        {
            get { return _guardDuration; }
            set { _guardDuration = value; }
        }

        private float _rollCooldown = 0.0f;

        public float RollCooldown
        {
            get { return _rollCooldown; }
            set { _rollCooldown = value; }
        }

        private float _dashCooldown = 0.0f;

        public float DashCooldown
        {
            get { return _dashCooldown; }
            set { _dashCooldown = value; }
        }

        private float _guardCooldown = 0.0f;

        public float GuardCooldown
        {
            get { return _guardCooldown; }
            set { _guardCooldown = value; }
        }

        private float _rollImpulse = 0.0f;

        public float RollImpulse
        {
            get { return _rollImpulse; }
            set { _rollImpulse = value; }
        }

        private float _dashImpulse = 0.0f;

        public float DashImpulse
        {
            get { return _dashImpulse; }
            set { _dashImpulse = value; }
        }

        private float _guardDamageReduce = 0.0f;

        public float GuardDamageReduce
        {
            get { return _guardDamageReduce; }
            set { _guardDamageReduce = value; }
        }

        private float _guardLockDuration = 1.0f;

        public float GuardLockDuration
        {
            get { return _guardLockDuration; }
            set { _guardLockDuration = value; }
        }

        private string _rollSound = "";

        public string RollSound
        {
            get { return _rollSound; }
            set { _rollSound = value; }
        }

        private string _airDashSound = "";

        public string AirDashSound
        {
            get { return _airDashSound; }
            set { _airDashSound = value; }
        }
        #endregion       

        public AvatarCore AvatarComponent;

        private SpriteSheetInfo _dashEffectSpritesheet;

        private float _rollCooldownTimer = 0.0f;
        private float _guardCooldownTimer = 0.0f;
        private float _durationTimer = 0.0f;

        private bool _isGuarding = false;
        private bool _isAirDashing = false;
        private bool _isRolling = false;
        private bool _alreadyDashed = false;
        private bool _airGuarding = false;

        public Guard(Entity entity)
            :base(entity, "Guard")
        {
            RequiredComponents = new Type[] { typeof(AvatarCore) };
        }

        public override void Init()
        {
            AvatarComponent = entity.GetComponent<AvatarCore>();
            base.Init();
        }

        public override void PreUpdate(GameTime gameTime)
        {
            if (entity.rigidbody != null && entity.rigidbody.isGrounded)
                _alreadyDashed = false;

            if (_durationTimer > 0f)
            {
                AvatarComponent.CanMove = false;
                AvatarComponent.CanTurn = false;
                AvatarComponent.CanAttack = false;
                AvatarComponent.CanUseElement = false;

                _durationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_isAirDashing)
                {
                    if(_durationTimer <= 0.0f)
                        _rollCooldownTimer = _dashCooldown;
                    AvatarComponent.State = AvatarState.AirDashing;
                    entity.rigidbody.body.GravityScale = 0.0f;
                }
                else if (_isGuarding)
                {
                    if(_durationTimer <= 0.0f)
                        _guardCooldownTimer = _guardCooldown;
                    AvatarComponent.State = AvatarState.Guarding;
                    if (_airGuarding)
                        entity.rigidbody.body.GravityScale = 0.0f;

                }
                else if (_isRolling)
                {
                    if(_durationTimer <= 0.0f)
                        _rollCooldownTimer = _rollCooldown;

                    if (entity.rigidbody.body.ContactList != null)
                    {
                        entity.rigidbody.body.ContactList.Contact.Friction = 0.0f;
                        ContactEdge ce = entity.rigidbody.body.ContactList;

                        while(ce.Next != null)
                        {
                            ce = ce.Next;
                            ce.Contact.Friction = 0.0f;
                        }
                    }
                    AvatarComponent.State = AvatarState.Rolling;
                }
            }
            else if (_isAirDashing)
            {
                _isAirDashing = false;
                entity.rigidbody.body.LinearVelocity = Vector2.Zero;
            }
            else if (_isGuarding)
            {
                _isGuarding = false;
                _airGuarding = false;
            }
            else if (_isRolling)
            {
                int offset = 0;

                if (entity.rigidbody.body.ContactList != null)
                {
                    if (entity.rigidbody.body.ContactList.Contact.FixtureA.Body != entity.rigidbody.body)
                    {
                        Entity EntityB = (Entity)entity.rigidbody.body.ContactList.Contact.FixtureA.UserData;
                        if (entity.hitboxes[0] != null && EntityB.transform.Position.Y - EntityB.hitboxes[0].Height / 2 + offset > entity.transform.Position.Y + entity.hitboxes[0].Height / 2)
                        {
                            entity.rigidbody.body.ContactList.Contact.ResetFriction();
                        }
                    }
                    else
                    {
                        Entity EntityB = (Entity)entity.rigidbody.body.ContactList.Contact.FixtureB.UserData;
                        if (entity.hitboxes[0] != null && EntityB.transform.Position.Y - EntityB.hitboxes[0].Height / 2 + offset > entity.transform.Position.Y + entity.hitboxes[0].Height / 2)
                        {
                            entity.rigidbody.body.ContactList.Contact.ResetFriction();
                        }
                    }

                    ContactEdge ce = entity.rigidbody.body.ContactList;

                    while (ce.Next != null)
                    {
                        ce = ce.Next;
                        if (ce.Contact.FixtureA.Body != entity.rigidbody.body)
                        {
                            Entity EntityB = (Entity)ce.Contact.FixtureA.UserData;
                            if (entity.hitboxes[0] != null && EntityB.transform.Position.Y - EntityB.hitboxes[0].Height / 2 + offset > entity.transform.Position.Y + entity.hitboxes[0].Height / 2)
                            {
                                ce.Contact.ResetFriction();
                            }
                        }
                        else
                        {
                            Entity EntityB = (Entity)ce.Contact.FixtureB.UserData;
                            if (entity.hitboxes[0] != null && EntityB.transform.Position.Y - EntityB.hitboxes[0].Height / 2 + offset > entity.transform.Position.Y + entity.hitboxes[0].Height / 2)
                            {
                                ce.Contact.ResetFriction();
                            }
                        }
                    }
                }
                _isRolling = false;
            }

            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_durationTimer <= 0.0f && AvatarComponent.State != AvatarState.UsingElement)
            {
                if (_rollCooldownTimer <= 0.0f)
                {
                    if (entity.rigidbody != null && entity.rigidbody.isGrounded)
                    {
                        _rollCooldownTimer = 0.0f;
                        if (Neon.Input.Pressed(NeonStarInput.Guard) && AvatarComponent.State != AvatarState.Stunlocked)
                        {
                            if (AvatarComponent.CanRoll)
                            {
                                if (Neon.Input.Check(NeonStarInput.MoveLeft))
                                {
                                    AvatarComponent.CurrentSide = Side.Left;

                                    PerformRoll();

                                }
                                else if (Neon.Input.Check(NeonStarInput.MoveRight))
                                {
                                    AvatarComponent.CurrentSide = Side.Right;
                                    PerformRoll();
                                }
                            }
                        }
                    }
                    else
                    {
                        _rollCooldownTimer = 0.0f;
                        if (Neon.Input.Pressed(NeonStarInput.Guard) && AvatarComponent.State != AvatarState.Stunlocked && !_alreadyDashed && AvatarComponent.ThirdPersonController != null /*&& AvatarComponent.ThirdPersonController.NumberOfAirMove > 0*/)
                        {
                            if (AvatarComponent.CanRoll)
                            {
                                if (Neon.Input.Check(NeonStarInput.MoveLeft))
                                {
                                    AvatarComponent.CurrentSide = Side.Left;
                                    AvatarComponent.State = AvatarState.AirDashing;

                                    PerformDash();
                                    _durationTimer = _dashDuration;
                                    _alreadyDashed = true;
                                    _isGuarding = false;
                                    _isAirDashing = true;
                                    _isRolling = false;
                                    AvatarComponent.CanMove = false;
                                    AvatarComponent.CanTurn = false;
                                    AvatarComponent.CanAttack = false;
                                    AvatarComponent.CanUseElement = false;
                                    //if (AvatarComponent.ThirdPersonController != null)
                                        //AvatarComponent.ThirdPersonController.NumberOfAirMove--;
                                }
                                else if (Neon.Input.Check(NeonStarInput.MoveRight))
                                {
                                    AvatarComponent.CurrentSide = Side.Right;
                                    AvatarComponent.State = AvatarState.AirDashing;

                                    PerformDash();
                                    _durationTimer = _dashDuration;
                                    _alreadyDashed = true;
                                    _isGuarding = false;
                                    _isAirDashing = true;
                                    _isRolling = false;
                                    AvatarComponent.CanMove = false;
                                    AvatarComponent.CanTurn = false;
                                    AvatarComponent.CanAttack = false;
                                    AvatarComponent.CanUseElement = false;
                                    //if (AvatarComponent.ThirdPersonController != null)
                                        //AvatarComponent.ThirdPersonController.NumberOfAirMove--;
                                }
                            }
                        }
                    }                  
                }
                else
                {
                    _rollCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (_guardCooldownTimer <= 0.0f && _durationTimer <= 0.0f )
                {
                    _guardCooldownTimer = 0.0f;
                    if (Neon.Input.Pressed(NeonStarInput.Guard) && !Neon.Input.Check(NeonStarInput.MoveRight) && !Neon.Input.Check(NeonStarInput.MoveLeft) && AvatarComponent.State != AvatarState.Stunlocked)
                    {
                        if (AvatarComponent.CanRoll)
                        {
                            AvatarComponent.State = AvatarState.Guarding;

                            PerformGuard();
                            _durationTimer = _guardDuration;
                            _isGuarding = true;
                            _isAirDashing = false;
                            _isRolling = false;
                            AvatarComponent.CanMove = false;
                            AvatarComponent.CanTurn = false;
                            AvatarComponent.CanAttack = false;
                            AvatarComponent.CanUseElement = false;
                        }
                    }
                }
                else
                {
                    _guardCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            base.Update(gameTime);
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);
        }

        public void PerformRoll()
        {
            AvatarComponent.State = AvatarState.Rolling;

            if (AvatarComponent.MeleeFight.CurrentAttack != null)
                AvatarComponent.MeleeFight.CurrentAttack.CancelAttack();

            entity.rigidbody.body.LinearVelocity = Vector2.Zero;
            entity.rigidbody.body.ApplyLinearImpulse(new Vector2(AvatarComponent.CurrentSide == Side.Right ? _rollImpulse : -_rollImpulse, 0));
            AvatarComponent.MeleeFight.CurrentAttack = null;
            AvatarComponent.MeleeFight.ResetComboHit();
            if (AvatarComponent.ElementSystem.CurrentElementEffect != null)
                AvatarComponent.ElementSystem.CurrentElementEffect.End();
            entity.hitboxes[0].SwitchType(HitboxType.Invincible, _rollDuration);
            if (entity.soundEmitter != null) entity.soundEmitter.PlaySound(_rollSound);

            _durationTimer = _rollDuration;
            _isGuarding = false;
            _isAirDashing = false;
            _isRolling = true;
            AvatarComponent.CanMove = false;
            AvatarComponent.CanTurn = false;
            AvatarComponent.CanAttack = false;
            AvatarComponent.CanUseElement = false;
        }

        public void PerformGuard()
        {
            if (AvatarComponent.MeleeFight.CurrentAttack != null)
                AvatarComponent.MeleeFight.CurrentAttack.CancelAttack();

            if (!entity.rigidbody.isGrounded)
            {
                _airGuarding = true;
                entity.rigidbody.GravityScale = 0;
            }
            else
                _airGuarding = false;
            entity.rigidbody.body.LinearVelocity = Vector2.Zero;
            if(AvatarComponent.ElementSystem.CurrentElementEffect != null)
                AvatarComponent.ElementSystem.CurrentElementEffect.End();
            AvatarComponent.MeleeFight.CurrentAttack = null;
            AvatarComponent.MeleeFight.ResetComboHit();
        }

        private void PerformDash()
        {
            if (AvatarComponent.MeleeFight.CurrentAttack != null)
                AvatarComponent.MeleeFight.CurrentAttack.CancelAttack();

            entity.rigidbody.GravityScale = 0;
            entity.rigidbody.body.LinearVelocity = Vector2.Zero;
            entity.rigidbody.body.ApplyLinearImpulse(new Vector2(AvatarComponent.CurrentSide == Side.Right ? _dashImpulse : -_dashImpulse, 0));
            AvatarComponent.MeleeFight.CurrentAttack = null;
            AvatarComponent.MeleeFight.ResetComboHit();
            if (AvatarComponent.ElementSystem.CurrentElementEffect != null)
                AvatarComponent.ElementSystem.CurrentElementEffect.End();
            entity.hitboxes[0].SwitchType(HitboxType.Invincible, _dashDuration);
            EffectsManager.GetEffect(_dashEffectSpritesheet, AvatarComponent.CurrentSide, entity.transform.Position, 0.0f, new Vector2(50, 0), 2.0f, 0.90f);
            if (entity.soundEmitter != null) entity.soundEmitter.PlaySound(_airDashSound);
        }
    }
}
