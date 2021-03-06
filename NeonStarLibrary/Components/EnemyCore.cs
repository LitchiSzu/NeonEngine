﻿using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using NeonEngine;
using NeonEngine.Components.CollisionDetection;
using NeonEngine.Components.Graphics2D;
using NeonStarLibrary.Components.Avatar;
using NeonStarLibrary.Components.Graphics2D;
using NeonStarLibrary.Private;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Enemies
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        MustFinishChase,
        FinishChase,
        Wait,
        WaitNode,
        WaitThreat,
        Attacking,
        StunLocked,
        StunLockEnd,
        Dying,
        Dead,
        ScriptControlled,
    }

    public enum EnemyType
    {
        Ground,
        Flying
    }

    public class EnemyCore : Component
    {
        #region Properties
        private bool _debug;
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }
        private float _startingHealthPoints = 10.0f;
        public float StartingHealthPoints
        {
            get { return _startingHealthPoints; }
            set { _startingHealthPoints = value; }
        }

        private Element _coreElement = Element.Neutral;

        public Element CoreElement
        {
            get { return _coreElement; }
            set { _coreElement = value; }
        }

        private float _currentHealthPoints;

        public float CurrentHealthPoints
        {
            get { return _currentHealthPoints; }
            set { _currentHealthPoints = value; }
        }

        private float _chanceToDropHealth = 0.6f;

        public float ChanceToDropHealth
        {
            get { return _chanceToDropHealth; }
            set { _chanceToDropHealth = value; }
        }

        private float _healthItemsToDrop = 1.0f;

        public float HealthItemsToDrop
        {
            get { return _healthItemsToDrop; }
            set { _healthItemsToDrop = value; }
        }

        private float _invincibilityDuration = 0.5f;

        public float InvincibilityDuration
        {
            get { return _invincibilityDuration; }
            set { _invincibilityDuration = value; }
        }

        private bool _airImpulsedInAir = true;

        public bool AirImpulsedInAir
        {
            get { return _airImpulsedInAir; }
            set { _airImpulsedInAir = value; }
        }


        private string _runAnim = "";

        public string RunAnim
        {
            get { return _runAnim; }
            set { _runAnim = value; }
        }

        private string _idleAnim = "";

        public string IdleAnim
        {
            get { return _idleAnim; }
            set { _idleAnim = value; }
        }

        private string _dyingAnim = "";

        public string DyingAnim
        {
            get { return _dyingAnim; }
            set { _dyingAnim = value; }
        }

        private string _hitAnim = "";

        public string HitAnim
        {
            get { return _hitAnim; }
            set { _hitAnim = value; }
        }

        private string _stunlockAnim = "";

        public string StunlockAnim
        {
            get { return _stunlockAnim; }
            set { _stunlockAnim = value; }
        }

        private bool _immuneToStunLock = false;

        public bool ImmuneToStunLock
        {
            get { return _immuneToStunLock; }
            set { _immuneToStunLock = value; }
        }

        private bool _immuneToImpulse = false;

        public bool ImmuneToImpulse
        {
            get { return _immuneToImpulse; }
            set { _immuneToImpulse = value; }
        }

        private bool _immuneToDeath = false;

        public bool ImmuneToDeath
        {
            get { return _immuneToDeath; }
            set { _immuneToDeath = value; }
        }

        private bool _immuneToDamage = false;

        public bool ImmuneToDamage
        {
            get { return _immuneToDamage; }
            set { _immuneToDamage = value; }
        }

        private bool _alwaysTakeUppercut = false;

        public bool AlwaysTakeUppercut
        {
            get { return _alwaysTakeUppercut; }
            set { _alwaysTakeUppercut = value; }
        }

        private EnemyType _type = EnemyType.Ground;

        public EnemyType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _triggerOnDamage = false;

        public bool TriggerOnDamage
        {
            get { return _triggerOnDamage; }
            set { _triggerOnDamage = value; }
        }

        private string _componentToTriggerName = "";

        public string ComponentToTriggerName
        {
            get { return _componentToTriggerName; }
            set { _componentToTriggerName = value; }
        }

        private float _waitThreatDuration = 1.0f;

        public float WaitThreatDuration
        {
            get { return _waitThreatDuration; }
            set { _waitThreatDuration = value; }
        }

        private Side _currentSide = Side.Right;

        public Side CurrentSide
        {
            get { return _currentSide; }
            set { _currentSide = value; }
        }

        private float _maximumStunlockTime = 3.0f;

        public float MaximumStunlockTime
        {
            get { return _maximumStunlockTime; }
            set { _maximumStunlockTime = value; }
        }
        #endregion

        public EnemyState State = EnemyState.Idle;

        public FollowNodes FollowNodes;
        public Chase Chase;
        public EnemyAttack Attack;
        public DamageDisplayer DamageDisplayer;
        public WeaknessDuringAttack WeaknessDuringAttack;

        private List<DrawableComponent> _drawableComponents;

        public bool CanMove = true;
        public bool CanTurn = true;
        public bool CanAttack = true;

        public bool IsInvincible = false;
        public bool IsAirLocked = false;

        public float WaitThreatTimer = 0.0f;
        
        private float _stunLockDuration = 0.0f;
        private float _airLockDuration = 0.0f;
        private float _invincibilityTimer = 0.0f;

        private Attack _damageOverTimeSource = null;
        private float _damageOverTimeValue = 0.0f;
        private float _damageOverTimeTimer = 0.0f;
        private float _damageOverTimeSpeed = 0.0f;
        private float _damageOverTimeTickTimer = 0.0f;

        private bool _damageBlinking = false;
        private float _blinkDuration = 0.2f;
        private float _blinkTimer = 0.0f;

        private bool _opacityGoingDown = true;
        private Component _componentToTrigger = null;
        private Entity[] _raycastHits;

        public bool TookDamageThisFrame = false;
        public string LastAttackTook = "";

        private float _totalStunlockTime = 0.0f;

        public EnemyCore(Entity entity)
            :base(entity, "EnemyCore")
        {
        }

        public override void Init()
        {
            if (FollowNodes == null)
                FollowNodes = entity.GetComponent<FollowNodes>();
            if (Attack == null)
                Attack = entity.GetComponent<EnemyAttack>();
            if (Chase == null)
                Chase = entity.GetComponent<Chase>();
            if (DamageDisplayer == null)
                DamageDisplayer = entity.GetComponent<DamageDisplayer>();
            if (WeaknessDuringAttack == null)
                WeaknessDuringAttack = entity.GetComponent<WeaknessDuringAttack>();

            if (_componentToTrigger == null && _componentToTriggerName != "")
            {
                _componentToTrigger = entity.GetComponentByName(_componentToTriggerName);
            }
            _currentHealthPoints = _startingHealthPoints;

            _drawableComponents = entity.GetComponentsByInheritance<DrawableComponent>();
            base.Init();
        }

        public DamageResult TakeDamage(Attack attack)
        {
            LastAttackTook = attack.Name;
            DamageResult tookDamage;
            if(WeaknessDuringAttack != null && WeaknessDuringAttack.CurrentlyWeak && WeaknessDuringAttack.AttackWeakTo == attack.Name)
                tookDamage = TakeDamage(attack.DamageOnHit * WeaknessDuringAttack.DamageModifier, attack.StunLock, attack.AlwaysStunlock, attack.TargetAirLock, attack.CurrentSide);
            else
                tookDamage = TakeDamage(attack.DamageOnHit, attack.StunLock, attack.AlwaysStunlock, attack.TargetAirLock, attack.CurrentSide);

            if (_triggerOnDamage)
            {
                if (_componentToTrigger != null)
                    _componentToTrigger.OnTrigger(this.entity, attack.Launcher != null ? attack.Launcher : attack._entity, new object[] { attack });
            }
            else if (tookDamage == DamageResult.Missed && _currentHealthPoints <= 0.0f)
            {
                if (attack.Launcher != null && CoreElement != Element.Neutral)
                {
                    AvatarCore ac = attack.Launcher.GetComponent<AvatarCore>();
                    if(ac != null)
                        ac.ElementSystem.GetElement(CoreElement);
                }
                else if (attack._entity != null && CoreElement != Element.Neutral)
                {
                    AvatarCore ac = attack._entity.GetComponent<AvatarCore>();
                    if(ac != null)
                        ac.ElementSystem.GetElement(CoreElement);
                }

                entity.hitboxes[0].Type = HitboxType.Invincible;
                State = EnemyState.Dying;
                if (Attack != null)
                {
                    if (Attack.CurrentAttack != null)
                    {
                        Attack.CurrentAttack.CancelAttack();
                        Attack.CurrentAttack = null;
                    }
                }
                if(entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(DyingAnim, true, 0, true, false, false);
            }

            if (tookDamage == DamageResult.Effective)
            {
                _damageBlinking = true;
                TookDamageThisFrame = true;
                //entity.spritesheets.ChangeAnimation(HitAnim, true, 0, true, true, false);
            }

            return tookDamage;
        }

        public DamageResult TakeDamage(Bullet bullet)
        {
            DamageResult tookDamage = TakeDamage(bullet.DamageOnHit, bullet.StunLock, true, 0.0f, bullet.entity.spritesheets.CurrentSide);
            if (tookDamage == DamageResult.Effective && _triggerOnDamage)
            {
                if (_componentToTrigger != null)
                    _componentToTrigger.OnTrigger(this.entity, bullet.launcher != null ? bullet.launcher : bullet.launcher, new object[] { bullet });
            }
            else if (tookDamage == DamageResult.Missed && _currentHealthPoints <= 0.0f)
            {
                if (bullet.launcher != null && CoreElement != Element.Neutral)
                    bullet.launcher.GetComponent<AvatarCore>().ElementSystem.GetElement(CoreElement);

                entity.hitboxes[0].Type = HitboxType.Invincible;
                State = EnemyState.Dying;
                if (Attack != null)
                {
                    if(Attack.CurrentAttack != null)
                        Attack.CurrentAttack.CancelAttack();
                    Attack.CurrentAttack = null;
                }
                entity.spritesheets.ChangeAnimation(DyingAnim, true, 0, true, false, false);
            }

            if (tookDamage == DamageResult.Effective)
            {
                _damageBlinking = true;
                //entity.spritesheets.ChangeAnimation(HitAnim, true, 0, true, true, false);
                TookDamageThisFrame = true;
            }

            return tookDamage;
        }

        public DamageResult TakeDamage(float damageValue, float stunLockDuration, bool alwaysStunlock, float airLockDuration, Side side)
        {
            if (_immuneToDamage)
                return DamageResult.Missed;

            if (IsInvincible)
                return DamageResult.Missed;

            if (damageValue >= 0.0f)
                return DamageResult.Missed;

            _currentHealthPoints += damageValue;

            foreach (DrawableComponent dc in _drawableComponents)
            {
                if (dc.GetType() == typeof(SpritesheetManager))
                {
                    (dc as SpritesheetManager).CurrentSpritesheet.MainColor = Color.Lerp(Color.Red, Color.White, (_currentHealthPoints / _startingHealthPoints) + 0.25f);
                }            
            }

            if (_currentHealthPoints <= 0.0f && !_immuneToDeath)
            {
                if(entity.hitboxes.Count > 0)
                    entity.hitboxes[0].Type = HitboxType.Invincible;
                IsAirLocked = false;
                _airLockDuration = 0.0f;
                if (DamageDisplayer != null)
                {
                    DamageDisplayer.DisplayDamage(damageValue, side);
                }

                foreach (DrawableComponent dc in _drawableComponents)
                {
                    if (dc.GetType() == typeof(SpritesheetManager))
                    {
                        (dc as SpritesheetManager).CurrentSpritesheet.MainColor = Color.White;
                    }
                }

                State = EnemyState.Dying;

                if (entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(DyingAnim, true, 0, true, false, false);

                return DamageResult.Missed;
            }
            else
            {
                foreach (DrawableComponent dc in _drawableComponents)
                {
                    if (dc.GetType() == typeof(SpritesheetManager))
                    {
                        (dc as SpritesheetManager).CurrentSpritesheet.MainColor = Color.Lerp(Color.Red, Color.White, (_currentHealthPoints / _startingHealthPoints) + 0.25f);
                    }
                }
            }

            if (Debug)
            {
                Console.WriteLine(entity.Name + " have lost " + damageValue + " HP(s) -> Now at " + _currentHealthPoints + " HP(s).");
            }

            if((alwaysStunlock || State == EnemyState.StunLocked) && _totalStunlockTime < _maximumStunlockTime)
                StunLock(stunLockDuration);
            
            if (entity.rigidbody != null)
            {
                if (!entity.rigidbody.isGrounded)
                {
                    AirLock(airLockDuration);
                }
            }
            TookDamageThisFrame = true;

            if (DamageDisplayer != null)
            {
                DamageDisplayer.DisplayDamage(damageValue, side);
            }

            return DamageResult.Effective;
        }

        public void DropHealthItems()
        {
            double random = Neon.Utils.CommonRandom.NextDouble();
            if (random <= _chanceToDropHealth)
            {
                for(int i = 0; i < _healthItemsToDrop; i ++)
                {
                    if (File.Exists(@"../Data/Prefabs/HealthCollectible.prefab"))
                    {
                        Entity e = DataManager.LoadPrefab(@"../Data/Prefabs/HealthCollectible.prefab", entity.GameWorld);
                        e.HasToBeSaved = false;
                        e.transform.Position = entity.transform.Position;
                        random = (Neon.Utils.CommonRandom.NextDouble() * 2) - 1;
                        if (e.rigidbody != null)
                            e.rigidbody.body.ApplyLinearImpulse(new Vector2((float)random, -1));
                    }
                }
            }
            else
                Console.WriteLine("No Health ! ");
        }

        public void StunLock(float duration)
        {
            if (duration > 0.0f && !_immuneToStunLock)
            {
                _totalStunlockTime -= _stunLockDuration;
                _stunLockDuration = duration;
                _totalStunlockTime += duration;
                if(entity.rigidbody != null)
                    entity.rigidbody.body.LinearVelocity = Vector2.Zero;

                if (State == EnemyState.Attacking && Attack != null && Attack.CurrentAttack != null)
                {
                    Attack.CurrentAttack.CancelAttack();
                    Attack.CurrentAttack = null;
                }

                State = EnemyState.StunLocked;
            }
        }

        public void AirLock(float duration)
        {
            if (duration > 0.0f)
            {
                IsAirLocked = true;
                _airLockDuration = duration;
                entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                entity.rigidbody.GravityScale = 0.0f;
            }
        }

        public override void PreUpdate(GameTime gameTime)
        {
            if (State != EnemyState.StunLocked)
            {
                _totalStunlockTime = 0.0f;
            }
            if (State != EnemyState.Dying && State != EnemyState.Dead)
            {
                for (int i = Fire.FirePlatforms.Count - 1; i >= 0; i--)
                {
                    Rigidbody rg = Fire.FirePlatforms[i];
                    if (entity.GameWorld.PhysicWorld.BodyList.Contains(rg.body))
                    {
                        if (entity.rigidbody != null)
                            entity.rigidbody.body.IgnoreCollisionWith(rg.body);
                    }
                    else
                        Fire.FirePlatforms.Remove(rg);
                }
            }
            

            LastAttackTook = "";

            if (State == EnemyState.Dying)
            {
                
                if (entity.spritesheets != null && entity.spritesheets.CurrentSpritesheetName == _dyingAnim && entity.spritesheets.IsFinished())
                    State = EnemyState.Dead;
                else if (entity.spritesheets == null || (entity.spritesheets != null && _dyingAnim == ""))
                    State = EnemyState.Dead;
                if(entity.rigidbody != null)
                    entity.rigidbody.Remove();
            }
            else if (State != EnemyState.Dead)
            {
                if (_airLockDuration > 0.0f && IsAirLocked)
                {
                    _airLockDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    entity.rigidbody.GravityScale = 0.0f;
                    CanMove = false;
                }
                else
                {
                    _airLockDuration = 0.0f;
                    IsAirLocked = true;
                }

                if (_stunLockDuration > 0.0f)
                {
                    _stunLockDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    State = EnemyState.StunLocked;
                    CanMove = false;
                    CanTurn = false;
                    CanAttack = false;
                }
                else if (State == EnemyState.StunLocked)
                    State = EnemyState.Wait;
            }
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (State != EnemyState.Dying && State != EnemyState.Dead)
            {
                if (entity.spritesheets != null)
                {
                    if (_invincibilityTimer > 0.0f)
                    {
                        _invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        OpacityBlink(15 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        _invincibilityTimer = 0.0f;
                        entity.spritesheets.CurrentSpritesheet.Opacity = 1.0f;
                    }
                }           

                if (_damageOverTimeTimer > 0.0f)
                {
                    _damageOverTimeTickTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_damageOverTimeTickTimer > _damageOverTimeSpeed)
                    {
                        _damageOverTimeTickTimer = 0.0f;
                        TakeDamage(_damageOverTimeValue, 0.0f, true, 0.0f, _damageOverTimeSource.CurrentSide);
                    }
                    _damageOverTimeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    _damageOverTimeTickTimer = 0.0f;
                    _damageOverTimeTimer = 0.0f;
                    _damageOverTimeSpeed = 0.0f;
                    _damageOverTimeValue = 0.0f;
                    _damageOverTimeSource = null;
                }
            }

            if (_damageBlinking)
            {
                if (_blinkTimer < _blinkDuration)
                {
                    if (entity.spritesheets != null)
                    {
                        entity.spritesheets.CurrentEffect = AssetManager.GetEffect("WhiteBlink");
                        _blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else
                {
                    if (entity.spritesheets != null)
                    {
                        entity.spritesheets.CurrentEffect = AssetManager.GetEffect("BasicRender");
                        _damageBlinking = false;
                        _blinkTimer = 0.0f;
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {                   
            base.PostUpdate(gameTime);
        }

        public override void FinalUpdate(GameTime gameTime)
        {
            TookDamageThisFrame = false;
            if (State == EnemyState.Dead)
            {
                this.entity.Destroy();
                DropHealthItems();
                return;
            }
            else if (State != EnemyState.Dying && State != EnemyState.StunLocked)
            {
                CanMove = true;
                CanTurn = true;
                CanAttack = true;
            }
            _raycastHits = null;

            if (WaitThreatTimer > WaitThreatDuration)
                WaitThreatTimer = 0.0f;

            switch (State)
            {
                case EnemyState.WaitThreat:
                    WaitThreatTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }

            if (Debug)
                Console.WriteLine(entity.Name + " current state is : " + State);
            base.FinalUpdate(gameTime);
        }

        public void AfflictDamageOverTime(float damageValue, float damageTimer, float damageSpeed, Attack source)
        {
            _damageOverTimeValue = damageValue;
            _damageOverTimeTimer = damageTimer;
            _damageOverTimeSpeed = damageSpeed;
            _damageOverTimeSource = source;
        }

        public void OpacityBlink(float value)
        {
            if (_opacityGoingDown)
            {
                if (entity.spritesheets.CurrentSpritesheet.Opacity > 0)
                {
                    entity.spritesheets.ChangeOpacity(-value);
                }
                else
                {
                    _opacityGoingDown = false;
                    entity.spritesheets.CurrentSpritesheet.Opacity = 0.0f;
                }
            }
            else
            {
                if (entity.spritesheets.CurrentSpritesheet.Opacity < 1)
                {
                    entity.spritesheets.ChangeOpacity(value);
                }
                else
                {
                    _opacityGoingDown = true;
                    entity.spritesheets.CurrentSpritesheet.Opacity = 1.0f;
                }
            }
        }

        public Entity[] UniqueRaycast(Vector2 offset, float distance, bool bothSide)
        {
            if (_raycastHits == null)
            {
                Entity[] hitEntities = new Entity[2];
                if (bothSide)
                {
                    if (entity.rigidbody != null)
                    {
                        if (entity.rigidbody.beacon != null)
                        {
                            hitEntities[0] = entity.rigidbody.beacon.Raycast(entity.transform.Position + offset, entity.transform.Position + offset + new Vector2(distance, 0));
                            hitEntities[1] = entity.rigidbody.beacon.Raycast(entity.transform.Position + offset, entity.transform.Position + offset + new Vector2(-distance, 0));
                        }
                    }
                }
                else
                {
                    hitEntities[0] = entity.rigidbody.beacon.Raycast(entity.transform.Position + offset, entity.transform.Position + offset + new Vector2(distance, 0) * (CurrentSide == Side.Right ? 1 : -1));
                }
                _raycastHits = hitEntities;
            }
            
            return _raycastHits;
        }
    }
}
