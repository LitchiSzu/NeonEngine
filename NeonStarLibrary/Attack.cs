﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NeonEngine;
using NeonEngine.Components.CollisionDetection;
using NeonStarLibrary.Components.Avatar;
using NeonStarLibrary.Components.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public class AttackEffect
    {
        public SpecialEffect specialEffect = SpecialEffect.Impulse;
        public string NameType
        {
            get { return specialEffect.ToString(); }
        }
        public object[] Parameters;

        public AttackEffect(SpecialEffect specialEffect, object[] Parameters)
        {
            this.specialEffect = specialEffect;
            this.Parameters = Parameters;
        }
    }

    public class AnimationDelayed
    {
        public float Delay;
        public object[] Parameters;
    }

    public class AttackDelayed
    {
        public float Delay;
        public object[] Parameters;
    }

    public class Attack
    {
        private bool _hit = false;
        private bool _mustStopAtTargetSight = false;
        private bool _alreadyLocked = false;

        private bool _alwaysStunlock = false;

        public bool AlwaysStunlock
        {
            get { return _alwaysStunlock; }
            set { _alwaysStunlock = value; }
        }

        public Element AttackElement = Element.Neutral;      

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        AttackType _type;
        public AttackType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _onlyOnceInAir = false;

        public bool OnlyOnceInAir
        {
            get { return _onlyOnceInAir; }
            set { _onlyOnceInAir = value; }
        }

        private bool _airOnly = false;

        public bool AirOnly
        {
            get { return _airOnly; }
            set { _airOnly = value; }
        }

        private bool _cancelOnGround = false;

        public bool CancelOnGround
        {
            get { return _cancelOnGround; }
            set { _cancelOnGround = value; }
        }


        private float _damageOnHit = 1.0f;
        public float DamageOnHit
        {
            get { return _damageOnHit; }
            set { _damageOnHit = value; }
        }

        private float _delay = 1.0f;
        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        private float _cooldown = 1.0f;
        public float Cooldown
        {
            get { return _cooldown; }
            set { _cooldown = value; }
        }

        private float _localCooldown = 0.0f;

        public float LocalCooldown
        {
            get { return _localCooldown; }
            set { _localCooldown = value; }
        }

        private float _duration = 1.0f;
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private float _airLock = 0.0f;
        public float AirLock
        {
            get { return _airLock; }
            set { _airLock = value; }
        }

        private float _stunLock = 0.0f;

        public float StunLock
        {
            get { return _stunLock; }
            set { _stunLock = value; }
        }

        private Side _side = Side.Right;
        public Side CurrentSide
        {
          get { return _side; }
          set { _side = value; }
        }

        private float _targetAirLock = 0.0f;
        public float TargetAirLock
        {
            get { return _targetAirLock; }
            set { _targetAirLock = value; }
        }

        private bool _delayStarted = false;

        public bool DelayStarted
        {
            get { return _delayStarted; }
            set { _delayStarted = value; }
        }

        private bool _durationStarted = false;

        public bool DurationStarted
        {
            get { return _durationStarted; }
            set { _durationStarted = value; }
        }

        private bool _cooldownStarted = false;

        public bool CooldownStarted
        {
            get { return _cooldownStarted; }
            set { _cooldownStarted = value; }
        }

        private bool _delayFinished = false;

        public bool DelayFinished
        {
            get { return _delayFinished; }
            set { _delayFinished = value; }
        }

        private bool _durationFinished = false;

        public bool DurationFinished
        {
            get { return _durationFinished; }
            set { _durationFinished = value; }
        }

        private bool _cooldownFinished = false;

        public bool CooldownFinished
        {
            get { return _cooldownFinished; }
            set { _cooldownFinished = value; }
        }

        private bool _airLockFinished = false;

        private bool _airLocked = false;

        public bool AirLocked
        {
          get { return _airLocked; }
          set { _airLocked = value; }
        }

        public bool AirLockFinished
        {
            get { return _airLockFinished; }
            set { _airLockFinished = value; }
        }

        private float _airFactor = 1.0f;

        public float AirFactor
        {
            get { return _airFactor; }
            set { _airFactor = value; }
        }

        private float _multiHitDelay = 0.0f;

        public float MultiHitDelay
        {
            get { return _multiHitDelay; }
            set { _multiHitDelay = value; }
        }

        private Vector2 _airImpulse;

        public Vector2 AirImpulse
        {
            get { return _airImpulse; }
            set { _airImpulse = value; }
        }

        private List<AttackEffect> _onDelaySpecialEffects = new List<AttackEffect>();
        private List<AttackEffect> _onDurationSpecialEffects = new List<AttackEffect>();
        private List<AttackEffect> _onHitSpecialEffects  = new List<AttackEffect>();
        private List<AttackEffect> _onGroundCancelSpecialEffects = new List<AttackEffect>();
        private List<AttackEffect> _onFinishSpecialEffects = new List<AttackEffect>();

        private Dictionary<Hitbox, float> _alreadyTouched = new Dictionary<Hitbox, float>();

        private List<AnimationDelayed> _delayedEffect = new List<AnimationDelayed>();
        private List<AttackDelayed> _delayedAttacks = new List<AttackDelayed>();
        private List<SoundEffectInstance> _soundEffectsToStop = new List<SoundEffectInstance>();

        private List<AnimatedSpecialEffect> _delayLoopedAnimation = new List<AnimatedSpecialEffect>();
        private List<AnimatedSpecialEffect> _durationLoopedAnimation = new List<AnimatedSpecialEffect>();
        private List<AnimatedSpecialEffect> _cooldownLoopedAnimation = new List<AnimatedSpecialEffect>();
        private List<AnimatedSpecialEffect> _specialEffectsToStopWithAttack = new List<AnimatedSpecialEffect>();


        public List<Hitbox> Hitboxes;
        public bool Canceled = false;
        public Entity _entity;
        public Entity Launcher;
        public AttackInfo AttackInfo;
        private MeleeFight _meleeFight;
        public bool FromEnemy = false;
        private Entity _target;
        private bool _isMoving = false;
        private float _movingSpeed = 0.0f;
        private bool _shouldMultiHit = true;

        public Attack()
        {
        }

        public Attack(AttackInfo attackInfo, Side side, Entity launcher, Entity target, bool FromEnemy = false)
            : this(attackInfo, side, launcher, FromEnemy)
        {
            this._target = target;
        }

        public Attack(AttackInfo attackInfo, Side side, Vector2 Position, bool FromEnemy)
            :this(attackInfo, side, null, FromEnemy)
        {
            _entity = new Entity(Neon.World);
            _entity.Name = "AttackHolder";
            _entity.transform.Position = Position;
        }

        public Attack(AttackInfo attackInfo, Side side, Entity launcher, bool FromEnemy = false)
        {           
            AttackInfo = attackInfo;
            Hitboxes = new List<Hitbox>();
            this._side = side;
            if(launcher != null) this._entity = launcher;
            this.Name = attackInfo.Name;
            this.Type = attackInfo.Type;
            this.Delay = attackInfo.Delay;
            this.DamageOnHit = attackInfo.DamageOnHit;
            this.Cooldown = attackInfo.Cooldown;
            this.LocalCooldown = attackInfo.LocalCooldown;
            this.Duration = attackInfo.Duration;
            this.AirLock = attackInfo.AirLock;
            this.TargetAirLock = attackInfo.TargetAirLock;
            this.StunLock = attackInfo.StunLock;
            this.AirOnly = attackInfo.AirOnly;
            this.CancelOnGround = attackInfo.CancelOnGround;
            this.OnlyOnceInAir = attackInfo.OnlyOnceInAir;
            this.AirFactor = attackInfo.AirFactor;
            this.FromEnemy = FromEnemy;
            this.AttackElement = attackInfo.AttackElement;
            this.MultiHitDelay = attackInfo.MultiHitDelay;
            this.AlwaysStunlock = attackInfo.AlwaysStunlock;
            if (MultiHitDelay <= 0.0f)
                this._shouldMultiHit = false;
            this.AirImpulse = attackInfo.AirImpulse;

            foreach (AttackEffect ae in attackInfo.OnDelaySpecialEffects)
                this._onDelaySpecialEffects.Add(ae);

            foreach (AttackEffect ae in attackInfo.OnDurationSpecialEffects)
                this._onDurationSpecialEffects.Add(ae);

            foreach (AttackEffect ae in attackInfo.OnHitSpecialEffects)
                this._onHitSpecialEffects.Add(ae);

            foreach (AttackEffect ae in attackInfo.OnGroundCancelSpecialEffects)
                this._onGroundCancelSpecialEffects.Add(ae);

            foreach (AttackEffect ae in attackInfo.OnFinishSpecialEffects)
                this._onFinishSpecialEffects.Add(ae);

            DelayStarted = true;

            if (_entity != null && _entity.Name != "AttackHolder")
            {
                _meleeFight = _entity.GetComponent<MeleeFight>();

                if (OnlyOnceInAir && !_entity.rigidbody.isGrounded)
                {
                    if (_meleeFight != null)
                    {
                        if (_meleeFight.AttacksWhileInAir.Contains(attackInfo.Name) || _meleeFight.AttacksWhileInAir.Contains(attackInfo.Name.Remove(attackInfo.Name.Length - 6)))
                        {
                            _meleeFight.CurrentComboHit = _meleeFight.LastComboHit;
                            this.CancelAttack();
                            Canceled = true;
                            return;
                        }
                        else
                        {
                            _meleeFight.AttacksWhileInAir.Add(attackInfo.Name);
                            if (attackInfo.Name.Substring(attackInfo.Name.Length - 6) == "Finish")
                            {
                                _meleeFight.AttacksWhileInAir.Add(attackInfo.Name);
                                _meleeFight.AttacksWhileInAir.Add(attackInfo.Name.Remove(attackInfo.Name.Length - 6));
                            }
                            else
                            {
                                _meleeFight.AttacksWhileInAir.Add(attackInfo.Name);
                                _meleeFight.AttacksWhileInAir.Add(attackInfo.Name + "Finish");
                            }
                        }
                    }
                }
            }

            if (_entity != null && _entity.Name != "AttackHolder")
            {
                if (AirOnly)
                    if (this._entity.rigidbody.isGrounded && _meleeFight != null)
                    {
                        _meleeFight.CurrentComboHit = _meleeFight.LastComboHit;
                        this.CancelAttack();
                        Canceled = true;
                        return;
                    }
            }
            if (_type == AttackType.MeleeSpecial && _meleeFight != null)
            {
                _meleeFight.CurrentComboHit = ComboSequence.None;
                if (AirLock > 0.0f) 
                    _meleeFight.AvatarComponent.AirLock(this.AirLock);
            }
            else if (_meleeFight == null && _entity != null)
            {
                EnemyCore ec = _entity.GetComponent<EnemyCore>();
                if (ec != null)
                {
                    if (AirLock > 0.0f)
                        ec.AirLock(AirLock);
                }
            }       
        }

        public void Init()
        {
            foreach (Rectangle hitbox in AttackInfo.Hitboxes)
            {
                Hitbox hb = _entity.GameWorld.HitboxPool.GetAvailableItem();
                hb.Type = HitboxType.Hit;
                hb.PoolInit(_entity);
                hb.Width = hitbox.Width;
                hb.Height = hitbox.Height;
                hb.Center = _entity.transform.Position;
                hb.OffsetX = this._side == Side.Right ? hitbox.X : -hitbox.X;
                hb.OffsetY = hitbox.Y;
                hb.HasToBeSaved = false;
                _entity.AddComponent(hb);
                Hitboxes.Add(hb);
            }

            if(_entity.Name != "AttackHolder" && (_meleeFight != null && _meleeFight.Debug))
                Console.WriteLine(AttackInfo.Name + " launched ! Current Combo -> " + _meleeFight.CurrentComboHit.ToString());

            this.DelayFinished = true;
            this.DurationStarted = true;
        }

        public void Update(GameTime gameTime)
        {
            for(int i = _delayedEffect.Count - 1; i >= 0; i --)
            {
                _delayedEffect[i].Delay = _delayedEffect[i].Delay - (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (_delayedEffect[i].Delay <= 0.0f)
                {
                    SpriteSheetInfo ssi = (SpriteSheetInfo)_delayedEffect[i].Parameters[0];
                    Entity entityToFollow = null;
                    AnimatedSpecialEffect ase = null;
                    if (ssi != null)
                    {               
                            if ((bool)_delayedEffect[i].Parameters[3])
                                entityToFollow = _entity;
                            ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(_delayedEffect[i].Parameters[1]), (Vector2)(_delayedEffect[i].Parameters[2]), (float)(_delayedEffect[i].Parameters[4]), (float)(_delayedEffect[i].Parameters[6]), entityToFollow);
                    }

                    if (ase != null && (bool)_delayedEffect[i].Parameters[8])
                        _specialEffectsToStopWithAttack.Add(ase);

                    _delayedEffect.RemoveAt(i);
                    
                }
            }

            for (int i = _delayedAttacks.Count - 1; i >= 0; i--)
            {
                _delayedAttacks[i].Delay = _delayedAttacks[i].Delay - (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_delayedAttacks[i].Delay <= 0.0f)
                {
                    string attackName = (string)(_delayedAttacks[i].Parameters[0]);
                    if ((bool)_delayedAttacks[i].Parameters[1])
                    {
                        if (FromEnemy)
                        {
                            EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                            if (ea != null && ea.CurrentAttack != null)
                            {
                                ea.CurrentAttack.CancelAttack();
                                ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                            }
                        }
                        else
                        {
                            MeleeFight mf = _entity.GetComponent<MeleeFight>();
                            if (mf != null && mf.CurrentAttack != null)
                            {
                                mf.CurrentAttack.CancelAttack();
                                mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                            }
                        }
                    }
                    else
                        AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                    _delayedAttacks.RemoveAt(i);
                }
                
            }

                if (_shouldMultiHit && _alreadyTouched.Count > 0)
                {
                    Hitbox[] keys = _alreadyTouched.Keys.ToArray();
                    for (int i = keys.Length - 1; i >= 0; i--)
                    {
                        Hitbox key = keys[i];
                        _alreadyTouched[key] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (_alreadyTouched[key] <= 0.0f)
                            _alreadyTouched.Remove(key);
                    }
                }

            if (DelayStarted && !DelayFinished)
            {
                for (int i = _onDelaySpecialEffects.Count - 1; i >= 0; i--)
                {
                    AttackEffect ae = _onDelaySpecialEffects.ElementAt(i);
                    switch (ae.specialEffect)
                    {
                        case SpecialEffect.Impulse:
                            if (_entity != null)
                            {
                                if (_entity.rigidbody.isGrounded || !(bool)ae.Parameters[2])
                                {
                                    Vector2 impulseForce = (Vector2)ae.Parameters[0] * (_entity.rigidbody.isGrounded ? 1 : AirFactor);
                                    _mustStopAtTargetSight = (bool)ae.Parameters[1];
                                    if (Launcher != null)
                                    {
                                        Launcher.rigidbody.body.LinearVelocity = Vector2.Zero;
                                        Launcher.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y));
                                    }
                                    else
                                    {
                                        _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                        _entity.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y));
                                    }
                                }
                            }
                            break;

                        case SpecialEffect.PercentageDamageBoost:
                            if (_meleeFight != null)
                            {
                                _meleeFight.DamageModifier = (float)(ae.Parameters[1]);
                                _meleeFight.DamageModifierTimer = (float)(ae.Parameters[0]);
                            }
                            break;

                        case SpecialEffect.DamageOverTime:
                            break;

                        case SpecialEffect.StartAttack:                            
                            if ((float)ae.Parameters[2] > 0.0f)
                            {
                                AttackDelayed ad = new AttackDelayed();
                                ad.Delay = (float)ae.Parameters[2];
                                ad.Parameters = ae.Parameters;
                                _delayedAttacks.Add(ad);
                            }
                            else
                            {
                                string attackName = (string)(ae.Parameters[0]);
                                if ((bool)ae.Parameters[1])
                                {
                                    if (FromEnemy)
                                    {
                                        EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                                        if (ea != null && ea.CurrentAttack != null)
                                        {
                                            ea.CurrentAttack.CancelAttack();
                                            ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                                        }
                                    }
                                    else
                                    {
                                        MeleeFight mf = _entity.GetComponent<MeleeFight>();
                                        if (mf != null && mf.CurrentAttack != null)
                                        {
                                            mf.CurrentAttack.CancelAttack();
                                            mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                                        }
                                    }
                                }
                                else
                                    AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                            }
                            break;

                        case SpecialEffect.ShootBullet:
                            BulletInfo bi = (BulletInfo)ae.Parameters[0];
                            BulletsManager.CreateBullet(bi, _side, Vector2.Zero, Launcher != null ? Launcher : _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.ShootBulletAtTarget:
                            BulletInfo bi2 = (BulletInfo)ae.Parameters[0];
                            if (_target != null)
                                BulletsManager.CreateBullet(bi2, _side, Vector2.Normalize(_target.transform.Position - _entity.transform.Position), _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.Invincible:
                            if (Launcher != null) Launcher.hitboxes[0].SwitchType(HitboxType.Invincible, (float)(ae.Parameters[0]));
                            else _entity.hitboxes[0].SwitchType(HitboxType.Invincible, (float)(ae.Parameters[0]));
                            break;

                        case SpecialEffect.EffectAnimation:
                            if ((float)ae.Parameters[5] != 0.0f)
                            {
                                AnimationDelayed ad = new AnimationDelayed();
                                ad.Delay = (float)ae.Parameters[5];
                                ad.Parameters = ae.Parameters;
                                _delayedEffect.Add(ad);
                            }
                            else
                            {
                                SpriteSheetInfo ssi = (SpriteSheetInfo)ae.Parameters[0];
                                Entity entityToFollow = null;
                                if (ssi != null)
                                {
                                    AnimatedSpecialEffect ase = null;
                                    if ((bool)ae.Parameters[3])
                                        entityToFollow = _entity;
                                    if((bool)ae.Parameters[7])
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow, AttackInfo.Delay, true);
                                    else
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow);

                                    if (ase != null && (bool)ae.Parameters[8])
                                        _specialEffectsToStopWithAttack.Add(ase);
                                }   

                            }                                                 
                            
                            break;

                        case SpecialEffect.MoveWhileAttacking:
                            _isMoving = true;
                            _movingSpeed = (float)(ae.Parameters[0]);
                            _mustStopAtTargetSight = (bool)(ae.Parameters[1]);
                            break;

                        case SpecialEffect.InstantiatePrefab:
                            Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/" + ae.Parameters[0] + ".prefab", Neon.World);
                            Vector2 offsetPrefab = Neon.Utils.ParseVector2(ae.Parameters[2].ToString());
                            e.transform.Position = (Launcher != null ? Launcher : _entity).transform.Position + (this.CurrentSide == Side.Right ? offsetPrefab : new Vector2(-offsetPrefab.X, offsetPrefab.Y));
                            
                            if (e.rigidbody != null)
                            {
                                Vector2 impulse = Neon.Utils.ParseVector2(ae.Parameters[1].ToString());
                                e.rigidbody.body.LinearVelocity = Vector2.Zero;
                                e.rigidbody.body.ApplyLinearImpulse((this.CurrentSide == Side.Right ? impulse : new Vector2(-impulse.X, impulse.Y)));
                            }
                            
                            break;

                        case SpecialEffect.PlaySound:
                            if(ae.Parameters[0] != null && (string)ae.Parameters[0] != "")
                            {
                                if ((bool)ae.Parameters[2])
                                {
                                    SoundEffect se = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if (se != null)
                                    {
                                        SoundEffectInstance sei = se.CreateInstance();
                                        sei.Volume = Math.Min((float)ae.Parameters[1], 1.0f);
                                        sei.Play();
                                        _soundEffectsToStop.Add(sei);
                                    }
                                }
                                else
                                {
                                    SoundEffect sei = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if (sei != null)
                                        sei.Play(Math.Min((float)ae.Parameters[1], 1.0f), 0.0f, 0.0f);
                                }
                            }
                            break;

                    }
                    _onDelaySpecialEffects.Remove(ae);
                }

                if (Delay > 0.0f)
                    Delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                {
                    Delay = 0.0f;
                    Init();
                }
            }
            else if (DurationStarted && !DurationFinished)
            {
                for(int i = _onDurationSpecialEffects.Count - 1; i >= 0; i--)
                {
                    AttackEffect ae = _onDurationSpecialEffects.ElementAt(i);
                    switch (ae.specialEffect)
                    {
                        case SpecialEffect.Impulse:
                            if (_entity != null)
                            {
                                if (_entity.rigidbody != null && (_entity.rigidbody.isGrounded || !(bool)ae.Parameters[2]))
                                {
                                    Vector2 impulseForce = (Vector2)ae.Parameters[0] * (_entity.rigidbody.isGrounded ? 1 : AirFactor);
                                    _mustStopAtTargetSight = (bool)ae.Parameters[1];
                                    _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                    _entity.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y));   
                                }
                            }
                            break;

                        case SpecialEffect.PercentageDamageBoost:
                            if (_meleeFight != null)
                            {
                                _meleeFight.DamageModifier = (float)(ae.Parameters[1]);
                                _meleeFight.DamageModifierTimer = (float)(ae.Parameters[0]);
                            }
                            break;

                        case SpecialEffect.DamageOverTime:
                            break;

                        case SpecialEffect.StartAttack:
                            if ((float)ae.Parameters[2] > 0.0f)
                            {
                                AttackDelayed ad = new AttackDelayed();
                                ad.Delay = (float)ae.Parameters[2];
                                ad.Parameters = ae.Parameters;
                                _delayedAttacks.Add(ad);
                            }
                            else
                            {
                                string attackName = (string)(ae.Parameters[0]);
                                if ((bool)ae.Parameters[1])
                                {
                                    if (FromEnemy)
                                    {
                                        EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                                        if (ea != null && ea.CurrentAttack != null)
                                        {
                                            ea.CurrentAttack.CancelAttack();
                                            ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                                        }
                                    }
                                    else
                                    {
                                        MeleeFight mf = _entity.GetComponent<MeleeFight>();
                                        if (mf != null && mf.CurrentAttack != null)
                                        {
                                            mf.CurrentAttack.CancelAttack();
                                            mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                                        }
                                    }
                                }
                                else
                                    AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                            }
                            break;

                        case SpecialEffect.ShootBullet:
                            BulletInfo bi = (BulletInfo)ae.Parameters[0];
                            BulletsManager.CreateBullet(bi, _side, Vector2.Zero, Launcher != null ? Launcher : _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.ShootBulletAtTarget:
                            BulletInfo bi2 = (BulletInfo)ae.Parameters[0];
                            if(_target != null)
                                BulletsManager.CreateBullet(bi2, _side, Vector2.Normalize(_target.transform.Position - _entity.transform.Position), _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.Invincible:
                            if (Launcher != null) Launcher.hitboxes[0].SwitchType(HitboxType.Invincible, (float)(ae.Parameters[0]));
                            else _entity.hitboxes[0].SwitchType(HitboxType.Invincible, (float)(ae.Parameters[0]));
                            break;

                        case SpecialEffect.EffectAnimation:
                            if ((float)ae.Parameters[5] != 0.0f)
                            {
                                AnimationDelayed ad = new AnimationDelayed();
                                ad.Delay = (float)ae.Parameters[5];
                                ad.Parameters = ae.Parameters;
                                _delayedEffect.Add(ad);
                            }
                            else
                            {
                                SpriteSheetInfo ssi = (SpriteSheetInfo)ae.Parameters[0];
                                Entity entityToFollow = null;
                                if (ssi != null)
                                {
                                    AnimatedSpecialEffect ase = null;
                                    if ((bool)ae.Parameters[3])
                                        entityToFollow = _entity;
                                    if ((bool)ae.Parameters[7])
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow, AttackInfo.Duration, true);
                                    else
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow);

                                    if (ase != null && (bool)ae.Parameters[8])
                                        _specialEffectsToStopWithAttack.Add(ase);
                                }
                            }
                           
                            break;

                        case SpecialEffect.MoveWhileAttacking:
                            _isMoving = true;
                            _movingSpeed = (float)(ae.Parameters[0]);
                            _mustStopAtTargetSight = (bool)(ae.Parameters[1]);
                            break;

                        case SpecialEffect.InstantiatePrefab:
                            Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/" + ae.Parameters[0] + ".prefab", Neon.World);
                            Vector2 offsetPrefab = Neon.Utils.ParseVector2(ae.Parameters[2].ToString());
                            e.transform.Position = (Launcher != null ? Launcher : _entity).transform.Position + (this.CurrentSide == Side.Right ? offsetPrefab : new Vector2(-offsetPrefab.X, offsetPrefab.Y));

                            if (e.rigidbody != null)
                            {
                                Vector2 impulse = Neon.Utils.ParseVector2(ae.Parameters[1].ToString());
                                e.rigidbody.body.LinearVelocity = Vector2.Zero;
                                e.rigidbody.body.ApplyLinearImpulse((this.CurrentSide == Side.Right ? impulse : new Vector2(-impulse.X, impulse.Y)));
                            }
                            break;

                        case SpecialEffect.PlaySound:
                            if (ae.Parameters[0] != null && (string)ae.Parameters[0] != "")
                            {
                                if ((bool)ae.Parameters[2])
                                {
                                    SoundEffect se = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if (se != null)
                                    {
                                        SoundEffectInstance sei = se.CreateInstance();
                                        sei.Volume = Math.Min((float)ae.Parameters[1], 1.0f);
                                        sei.Play();
                                        _soundEffectsToStop.Add(sei);
                                    }
                                }
                                else
                                {
                                    SoundEffect sei = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if(sei != null)
                                        sei.Play(Math.Min((float)ae.Parameters[1], 1.0f), 0.0f, 0.0f);
                                }
                            }
                            break;

                    }
                    _onDurationSpecialEffects.Remove(ae);
                }
                if (Hitboxes.Count > 0)
                {
                    for (int i = Hitboxes.Count - 1; i >= 0; i--)
                    {
                        Hitbox hitbox = Hitboxes[i];
                        for (int j = _entity.GameWorld.Hitboxes.Count - 1; j >= 0; j--)
                        {
                            Hitbox hb = _entity.GameWorld.Hitboxes[j];
                            if ((hb.Type == HitboxType.Main || hb.Type == HitboxType.Invincible) && hb.entity != this._entity)
                            {
                                if (hb.hitboxRectangle.Intersects(hitbox.hitboxRectangle) && !_alreadyTouched.ContainsKey(hb))
                                {
                                    Effect(hb.entity, hb);
                                    if (hb.Type != HitboxType.Invincible)
                                        _alreadyTouched.Add(hb, _multiHitDelay);
                                }
                            }
                        }

                        if (i - 1 > Hitboxes.Count - 1)
                            break;
                    }
                }

                Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Duration <= 0)
                {
                    DurationFinished = true;
                    CooldownStarted = true;
                    
                    if (LocalCooldown > 0.0f)
                    {
                        if (FromEnemy)
                        {
                            _entity.GetComponent<EnemyCore>().Attack.LocalAttacksInCooldown.Add(this);
                        }
                    }

                    for (int i = Hitboxes.Count - 1; i >= 0; i--)
                    {
                        Hitboxes[i].Remove();
                    }
                    this.Hitboxes.Clear();
                }
            }
            else if(CooldownStarted && !CooldownFinished)
            {
                _isMoving = false;
                if (Cooldown > 0.0f)
                {
                    Cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Cooldown = 0.0f;
                    CooldownFinished = true;
                    if (!Canceled)
                    {
                        for (int i = _onFinishSpecialEffects.Count - 1; i >= 0; i--)
                        {
                            AttackEffect ae = _onFinishSpecialEffects.ElementAt(i);
                            switch (ae.specialEffect)
                            {
                                case SpecialEffect.Impulse:
                                    if (_entity.rigidbody.isGrounded || !(bool)ae.Parameters[2])
                                    {
                                        Vector2 impulseForce = (Vector2)ae.Parameters[0] * (_entity.rigidbody.isGrounded ? 1 : AirFactor);
                                        _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                        _entity.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y));
                                    }
                                    break;

                                case SpecialEffect.PercentageDamageBoost:
                                    break;

                                case SpecialEffect.DamageOverTime:
                                    break;

                                case SpecialEffect.StartAttack:
                                    if ((float)ae.Parameters[2] > 0.0f)
                                    {
                                        AttackDelayed ad = new AttackDelayed();
                                        ad.Delay = (float)ae.Parameters[2];
                                        ad.Parameters = ae.Parameters;
                                        _delayedAttacks.Add(ad);
                                    }
                                    else
                                    {
                                        string attackName = (string)ae.Parameters[0];
                                        if ((bool)ae.Parameters[1])
                                        {
                                            if (FromEnemy)
                                            {
                                                EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                                                if (ea != null && ea.CurrentAttack != null)
                                                {
                                                    ea.CurrentAttack.CancelAttack();
                                                    ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                                                }
                                            }
                                            else
                                            {
                                                MeleeFight mf = _entity.GetComponent<MeleeFight>();
                                                if (mf != null && mf.CurrentAttack != null)
                                                {
                                                    mf.CurrentAttack.CancelAttack();
                                                    mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                                                }
                                            }
                                        }
                                        else
                                            AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                                    }
                                    break;

                                case SpecialEffect.ShootBullet:
                                    BulletInfo bi = (BulletInfo)ae.Parameters[0];
                                    BulletsManager.CreateBullet(bi, _side, Vector2.Zero, _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                                    break;

                                case SpecialEffect.ShootBulletAtTarget:
                                    BulletInfo bi2 = (BulletInfo)ae.Parameters[0];
                                    if (_target != null)
                                        BulletsManager.CreateBullet(bi2, _side, Vector2.Normalize(_target.transform.Position - _entity.transform.Position), _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                                    break;

                                case SpecialEffect.Invincible:
                                    _entity.hitboxes[0].SwitchType(HitboxType.Invincible, (float)ae.Parameters[0]);
                                    break;

                                case SpecialEffect.EffectAnimation:
                                    if ((float)ae.Parameters[5] != 0.0f)
                                    {
                                        AnimationDelayed ad = new AnimationDelayed();
                                        ad.Delay = (float)ae.Parameters[5];
                                        ad.Parameters = ae.Parameters;
                                        _delayedEffect.Add(ad);
                                    }
                                    else
                                    {
                                        SpriteSheetInfo ssi = (SpriteSheetInfo)ae.Parameters[0];
                                        Entity entityToFollow = null;
                                        if (ssi != null)
                                        {
                                            AnimatedSpecialEffect ase = null;

                                            if ((bool)ae.Parameters[3])
                                                entityToFollow = _entity;
                                            if ((bool)ae.Parameters[7])
                                                EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow, AttackInfo.Cooldown, true);
                                            else
                                                EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow);

                                            if (ase != null && (bool)ae.Parameters[8])
                                                _specialEffectsToStopWithAttack.Add(ase);
                                        }
                                    }

                                    break;

                                case SpecialEffect.InstantiatePrefab:
                                    Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/" + ae.Parameters[0] + ".prefab", Neon.World);
                                    Vector2 offsetPrefab = Neon.Utils.ParseVector2(ae.Parameters[2].ToString());
                                    e.transform.Position = (Launcher != null ? Launcher : _entity).transform.Position + (this.CurrentSide == Side.Right ? offsetPrefab : new Vector2(-offsetPrefab.X, offsetPrefab.Y));

                                    if (e.rigidbody != null)
                                    {
                                        Vector2 impulse = Neon.Utils.ParseVector2(ae.Parameters[1].ToString());
                                        e.rigidbody.body.LinearVelocity = Vector2.Zero;
                                        e.rigidbody.body.ApplyLinearImpulse((this.CurrentSide == Side.Right ? impulse : new Vector2(-impulse.X, impulse.Y)));
                                    }

                                    break;

                                case SpecialEffect.PlaySound:
                                    if (ae.Parameters[0] != null && (string)ae.Parameters[0] != "")
                                    {
                                        if ((bool)ae.Parameters[2])
                                        {
                                            SoundEffect se = SoundManager.GetSound((string)ae.Parameters[0]);
                                            if (se != null)
                                            {
                                                SoundEffectInstance sei = se.CreateInstance();
                                                sei.Volume = Math.Min((float)ae.Parameters[1], 1.0f);
                                                sei.Play();
                                                _soundEffectsToStop.Add(sei);
                                            }
                                        }
                                        else
                                        {
                                            SoundEffect sei = SoundManager.GetSound((string)ae.Parameters[0]);
                                            if (sei != null)
                                                sei.Play(Math.Min((float)ae.Parameters[1], 1.0f), 0.0f, 0.0f);
                                        }
                                    }
                                    break;
                            }
                            _onFinishSpecialEffects.Remove(ae);
                        }
                    }
                    
                }
            }

            if (_isMoving)
            {
                _entity.rigidbody.body.LinearVelocity = CurrentSide == Side.Right ? new Vector2(_movingSpeed, 0) : new Vector2(-_movingSpeed, 0);
            }         

            if (_mustStopAtTargetSight)
            {
                if(_side == Side.Left)
                    if (_target.hitboxes[0].Type != HitboxType.Invincible && _entity.rigidbody.beacon.CheckLeftSide(Math.Abs(_entity.rigidbody.body.LinearVelocity.X) * 2, true) == _target)
                    {
                        _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                        _isMoving = false;
                        Duration = 0.0f;
                        _delayedAttacks.Clear();
                        foreach (AnimatedSpecialEffect ase in _specialEffectsToStopWithAttack)
                            ase.Destroy();
                        
                    }

                if (_side == Side.Right)
                    if (_target.hitboxes[0].Type != HitboxType.Invincible && _entity.rigidbody.beacon.CheckRightSide(Math.Abs(_entity.rigidbody.body.LinearVelocity.X) * 2, true) == _target)
                    {
                        _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                        _isMoving = false;
                        Duration = 0.0f;
                        _delayedAttacks.Clear();
                        foreach (AnimatedSpecialEffect ase in _specialEffectsToStopWithAttack)
                            ase.Destroy();
                    }
            }
                
        }

        public void FinalUpdate(GameTime gameTime)
        {
            if (_entity.Name != "AttackHolder")
            {
                if (CancelOnGround && _entity.rigidbody.isGrounded && !Canceled)
                {
                    for (int i = _onGroundCancelSpecialEffects.Count - 1; i >= 0; i--)
                    {
                        AttackEffect ae = _onGroundCancelSpecialEffects.ElementAt(i);
                        switch (ae.specialEffect)
                        {
                            case SpecialEffect.Impulse:
                                if (_entity.rigidbody.isGrounded || !(bool)ae.Parameters[2])
                                {
                                    Vector2 impulseForce = (Vector2)ae.Parameters[0] * (_entity.rigidbody.isGrounded ? 1 : AirFactor);
                                    _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                    _entity.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y));
                                }
                                break;

                            case SpecialEffect.PercentageDamageBoost:
                                break;

                            case SpecialEffect.DamageOverTime:
                                break;

                            case SpecialEffect.StartAttack:
                                if ((float)ae.Parameters[2] > 0.0f)
                                {
                                    AttackDelayed ad = new AttackDelayed();
                                    ad.Delay = (float)ae.Parameters[2];
                                    ad.Parameters = ae.Parameters;
                                    _delayedAttacks.Add(ad);
                                }
                                else
                                {
                                    string attackName = (string)ae.Parameters[0];
                                    if ((bool)ae.Parameters[1])
                                    {
                                        if (FromEnemy)
                                        {
                                            EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                                            if (ea != null && ea.CurrentAttack != null)
                                            {
                                                ea.CurrentAttack.CancelAttack();
                                                ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                                            }
                                        }
                                        else
                                        {
                                            MeleeFight mf = _entity.GetComponent<MeleeFight>();
                                            if (mf != null && mf.CurrentAttack != null)
                                            {
                                                mf.CurrentAttack.CancelAttack();
                                                mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                                            }
                                        }
                                    }
                                    else
                                        AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                                }
                                break;

                            case SpecialEffect.ShootBullet:
                                BulletInfo bi = (BulletInfo)ae.Parameters[0];
                                BulletsManager.CreateBullet(bi, _side, Vector2.Zero, _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                                break;

                            case SpecialEffect.ShootBulletAtTarget:
                                BulletInfo bi2 = (BulletInfo)ae.Parameters[0];
                                if (_target != null)
                                    BulletsManager.CreateBullet(bi2, _side, Vector2.Normalize(_target.transform.Position - _entity.transform.Position), _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                                break;

                            case SpecialEffect.Invincible:
                                _entity.hitboxes[0].SwitchType(HitboxType.Invincible, (float)ae.Parameters[0]);
                                break;

                            case SpecialEffect.EffectAnimation:
                                if ((float)ae.Parameters[5] != 0.0f)
                                {
                                    AnimationDelayed ad = new AnimationDelayed();
                                    ad.Delay = (float)ae.Parameters[5];
                                    ad.Parameters = ae.Parameters;
                                    _delayedEffect.Add(ad);
                                }
                                else
                                {
                                    SpriteSheetInfo ssi = (SpriteSheetInfo)ae.Parameters[0];
                                    Entity entityToFollow = null;
                                    if (ssi != null)
                                    {
                                        AnimatedSpecialEffect ase = null;
                                        if ((bool)ae.Parameters[3])
                                            entityToFollow = _entity;
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, _entity.transform.Position, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow);

                                        if (ase != null && (bool)ae.Parameters[8])
                                            _specialEffectsToStopWithAttack.Add(ase);
                                    }
                                }
                                
                                break;

                            case SpecialEffect.InstantiatePrefab:
                                Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/" + ae.Parameters[0] + ".prefab", Neon.World);
                                Vector2 offsetPrefab = Neon.Utils.ParseVector2(ae.Parameters[2].ToString());
                                e.transform.Position = (Launcher != null ? Launcher : _entity).transform.Position + (this.CurrentSide == Side.Right ? offsetPrefab : new Vector2(-offsetPrefab.X, offsetPrefab.Y));

                                if (e.rigidbody != null)
                                {
                                    Vector2 impulse = Neon.Utils.ParseVector2(ae.Parameters[1].ToString());
                                    e.rigidbody.body.LinearVelocity = Vector2.Zero;
                                    e.rigidbody.body.ApplyLinearImpulse((this.CurrentSide == Side.Right ? impulse : new Vector2(-impulse.X, impulse.Y)));
                                }

                                break;

                            case SpecialEffect.PlaySound:
                                if (ae.Parameters[0] != null && (string)ae.Parameters[0] != "")
                                {
                                    if ((bool)ae.Parameters[2])
                                    {
                                        SoundEffect se = SoundManager.GetSound((string)ae.Parameters[0]);
                                        if (se != null)
                                        {
                                            SoundEffectInstance sei = se.CreateInstance();
                                            sei.Volume = Math.Min((float)ae.Parameters[1], 1.0f);
                                            sei.Play();
                                            _soundEffectsToStop.Add(sei);
                                        }
                                    }
                                    else
                                    {
                                        SoundEffect sei = SoundManager.GetSound((string)ae.Parameters[0]);
                                        if (sei != null)
                                            sei.Play(Math.Min((float)ae.Parameters[1], 1.0f), 0.0f, 0.0f);
                                    }
                                }
                                break;
                        }
                        _onGroundCancelSpecialEffects.Remove(ae);
                    }

                    for (int i = Hitboxes.Count - 1; i >= 0; i--)
                    {
                        Hitboxes[i].Remove();
                    }
                    Hitboxes.Clear();
                    foreach (SoundEffectInstance se in _soundEffectsToStop)
                        se.Stop();
                    this.DurationStarted = true;
                    this.DurationFinished = true;
                    this.DelayStarted = true;
                    this.DelayStarted = true;
                    this.CooldownStarted = true;
                    this.AirLocked = false;
                    this.AirLockFinished = true;
                    this.Canceled = true;
                    foreach (AnimatedSpecialEffect ase in _specialEffectsToStopWithAttack)
                        ase.Destroy();
                }
            }
        }

        private void Effect(Entity entity, Hitbox collidedHitbox)
        {
            bool validTarget = false;
            DamageResult damageResult;
            AvatarCore avatar = null;
            EnemyCore enemy = null;

            if (!FromEnemy)
            {
                enemy = entity.GetComponent<EnemyCore>();
                if (enemy != null)
                {
                    validTarget = true;
                    if (collidedHitbox.Type != HitboxType.Invincible)
                    {
                        damageResult = enemy.TakeDamage(this);
                        _hit = damageResult == DamageResult.Effective ? true : false;
                    }
                }

                if (!_alreadyLocked && AirLock >= 0 && Type == AttackType.MeleeLight && _meleeFight != null)
                {
                    _meleeFight.AvatarComponent.AirLock(AirLock);
                    _alreadyLocked = true;
                }
            }
            else
            {
                avatar = entity.GetComponent<AvatarCore>();
                if(avatar != null)
                {
                    validTarget = true;
                    if (collidedHitbox.Type != HitboxType.Invincible)
                    {
                        damageResult = avatar.TakeDamage(this);
                        _hit = damageResult == DamageResult.Effective ? true : false;
                    }
                }
            }

            if (validTarget || (enemy != null && enemy.AlwaysTakeUppercut && (this.Name == "LiOnUppercut" || this.Name == "LiOnUppercutFinish")))
            {
                bool velocityReset = false;

                foreach (AttackEffect ae in _onHitSpecialEffects)
                {
                    switch (ae.specialEffect)
                    {
                        case SpecialEffect.Impulse:
                            if (avatar != null && _hit || (enemy != null && !enemy.ImmuneToImpulse && enemy.State != EnemyState.Dying))
                            {
                                if (entity.rigidbody.isGrounded || !(bool)ae.Parameters[2])
                                {
                                    Vector2 impulseForce = (Vector2)ae.Parameters[0];
                                    if (!velocityReset)
                                    {
                                        entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                        entity.rigidbody.body.ResetDynamics();
                                    }

                                    entity.rigidbody.body.ApplyLinearImpulse(new Vector2(_side == Side.Right ? impulseForce.X : -impulseForce.X, impulseForce.Y) * (entity.rigidbody.isGrounded ? 1 : AirFactor));
                                    velocityReset = true;
                                }
                            }
                            break;

                        case SpecialEffect.PositionalPulse:
                            if ((avatar != null && _hit) || (enemy != null && !enemy.ImmuneToImpulse))
                            {
                                Vector2 pulseForce = (Vector2)ae.Parameters[0];
                                if (!velocityReset) entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                                entity.rigidbody.body.ApplyLinearImpulse(new Vector2(pulseForce.X * (_entity.transform.Position.X < entity.transform.Position.X ? 1 : -1), pulseForce.Y * (_entity.transform.Position.Y < entity.transform.Position.Y ? 1 : -1)));
                                velocityReset = true;
                            }
                            break;

                        case SpecialEffect.PercentageDamageBoost:
                            break;

                        case SpecialEffect.DamageOverTime:
                            if (enemy != null)
                                enemy.AfflictDamageOverTime((float)ae.Parameters[1], (float)ae.Parameters[0], (float)ae.Parameters[2], this);
                            break;

                        case SpecialEffect.StartAttack:
                            if ((float)ae.Parameters[2] > 0.0f)
                            {
                                AttackDelayed ad = new AttackDelayed();
                                ad.Delay = (float)ae.Parameters[2];
                                ad.Parameters = ae.Parameters;
                                _delayedAttacks.Add(ad);
                            }
                            else
                            {
                                string attackName = (string)ae.Parameters[0];
                                if ((bool)ae.Parameters[1])
                                {
                                    if (FromEnemy)
                                    {
                                        EnemyAttack ea = _entity.GetComponent<EnemyAttack>();
                                        if (ea != null && ea.CurrentAttack != null)
                                        {
                                            ea.CurrentAttack.CancelAttack();
                                            ea.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity, _target, true);
                                        }
                                    }
                                    else
                                    {
                                        MeleeFight mf = _entity.GetComponent<MeleeFight>();
                                        if (mf != null && mf.CurrentAttack != null)
                                        {
                                            mf.CurrentAttack.CancelAttack();
                                            mf.CurrentAttack = AttacksManager.GetAttack(attackName, CurrentSide, _entity);
                                        }
                                    }
                                }
                                else
                                    AttacksManager.StartFreeAttack(attackName, _side, _entity.transform.Position, FromEnemy).Launcher = _entity;
                            }
                            break;

                        case SpecialEffect.ShootBullet:
                            BulletInfo bi = (BulletInfo)ae.Parameters[0];
                            BulletsManager.CreateBullet(bi, _side, Vector2.Zero, _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.ShootBulletAtTarget:
                            BulletInfo bi2 = (BulletInfo)ae.Parameters[0];
                            if (_target != null)
                                BulletsManager.CreateBullet(bi2, _side, Vector2.Normalize(_target.transform.Position - _entity.transform.Position), _entity, (GameScreen)_entity.GameWorld, FromEnemy);
                            break;

                        case SpecialEffect.Invincible:
                            entity.hitboxes[0].SwitchType(HitboxType.Invincible, (float)ae.Parameters[0]);
                            break;

                        case SpecialEffect.EffectAnimation:
                            if (_hit)
                            {
                                if ((float)ae.Parameters[5] != 0.0f)
                                {
                                    AnimationDelayed ad = new AnimationDelayed();
                                    ad.Delay = (float)ae.Parameters[5];
                                    ad.Parameters = ae.Parameters;
                                    _delayedEffect.Add(ad);
                                }
                                else
                                {
                                    SpriteSheetInfo ssi = (SpriteSheetInfo)ae.Parameters[0];
                                    if (ssi != null)
                                    {
                                        Rectangle intersectionRectangle = Rectangle.Intersect(collidedHitbox.hitboxRectangle, entity.hitboxes[0].hitboxRectangle);

                                        AnimatedSpecialEffect ase = null;
                                        Entity entityToFollow = null;
                                        if ((bool)ae.Parameters[3])
                                            entityToFollow = _entity;
                                        Vector2 hitPosition = new Vector2(CurrentSide == Side.Right ? collidedHitbox.hitboxRectangle.Right : collidedHitbox.hitboxRectangle.Left, collidedHitbox.hitboxRectangle.Center.Y);
                                        ase = EffectsManager.GetEffect(ssi, CurrentSide, hitPosition, (float)(ae.Parameters[1]), (Vector2)(ae.Parameters[2]), (float)(ae.Parameters[4]), (float)(ae.Parameters[6]), entityToFollow);

                                        if (ase != null && (bool)ae.Parameters[8])
                                            _specialEffectsToStopWithAttack.Add(ase);
                                    }
                                }
                            }
                            break;

                        case SpecialEffect.InstantiatePrefab:
                            Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/" + ae.Parameters[0] + ".prefab", Neon.World);
                            Vector2 offsetPrefab = Neon.Utils.ParseVector2(ae.Parameters[2].ToString());
                            e.transform.Position = (Launcher != null ? Launcher : _entity).transform.Position + (this.CurrentSide == Side.Right ? offsetPrefab : new Vector2(-offsetPrefab.X, offsetPrefab.Y));

                            if (e.rigidbody != null)
                            {
                                Vector2 impulse = Neon.Utils.ParseVector2(ae.Parameters[1].ToString());
                                e.rigidbody.body.LinearVelocity = Vector2.Zero;
                                e.rigidbody.body.ApplyLinearImpulse((this.CurrentSide == Side.Right ? impulse : new Vector2(-impulse.X, impulse.Y)));
                            }
                            break;

                        case SpecialEffect.PlaySound:
                            if (!_hit)
                                break;
                            if (ae.Parameters[0] != null && (string)ae.Parameters[0] != "")
                            {
                                if ((bool)ae.Parameters[2])
                                {
                                    SoundEffect se = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if (se != null)
                                    {
                                        SoundEffectInstance sei = se.CreateInstance();
                                        sei.Volume = Math.Min((float)ae.Parameters[1], 1.0f);
                                        sei.Play();
                                        _soundEffectsToStop.Add(sei);
                                    }
                                }
                                else
                                {
                                    SoundEffect sei = SoundManager.GetSound((string)ae.Parameters[0]);
                                    if (sei != null)
                                        sei.Play(Math.Min((float)ae.Parameters[1], 1.0f), 0.0f, 0.0f);
                                }
                            }
                            break;
                    }
                }

                if (AirImpulse != Vector2.Zero)
                {
                    if (entity.rigidbody != null)
                    {
                        if (!entity.rigidbody.isGrounded)
                        {
                            if (!velocityReset)
                                entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                            if(enemy != null && enemy.AirImpulsedInAir)
                                entity.rigidbody.body.ApplyLinearImpulse(CurrentSide == Side.Right ? AirImpulse : new Vector2(-AirImpulse.X, AirImpulse.Y));
                        }
                    }
                }
            }        
        }

        public void CancelAttack()
        {
            for (int i = Hitboxes.Count - 1; i >= 0; i--)
            {
                Hitboxes[i].Remove();
            }
            foreach (SoundEffectInstance se in _soundEffectsToStop)
                se.Stop();
            this.Hitboxes.Clear();
            this.CooldownFinished = true;
            this.CooldownStarted = true;
            this.DurationStarted = true;
            this.DurationFinished =true;
            this.DelayStarted = true;
            this.DelayFinished = true;
            this.AirLocked = false;
            this.AirLockFinished = true;
            foreach (AnimatedSpecialEffect ase in _specialEffectsToStopWithAttack)
                ase.Destroy();
        }
    }
}
