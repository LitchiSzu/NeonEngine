﻿using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
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
        Attack,
        StunLock
    }

    public enum EnemyType
    {
        Ground,
        Flying
    }

    public class Enemy : Component
    {
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

        public EnemyState State;

        private float _currentHealthPoints;
        private float _airLockDuration = 0.0f;

        public FollowNodes _followNodes;
        public ThreatArea _threatArea;
        public Chase _chase;
        public EnemyAttack _attack;
        public bool CanMove = true;
        private float _stunLockDuration = 0.0f;

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

        private string _attackAnim = "";

        public string AttackAnim
        {
            get { return _attackAnim; }
            set { _attackAnim = value; }
        }

        private EnemyType _type = EnemyType.Ground;

        public EnemyType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Enemy(Entity entity)
            :base(entity, "Enemy")
        {
        }

        public override void Init()
        {
            if (_threatArea == null)
                _threatArea = entity.GetComponent<ThreatArea>();
            if (_followNodes == null)
                _followNodes = entity.GetComponent<FollowNodes>();
            if (_chase == null)
                _chase = entity.GetComponent<Chase>();
            if (_attack == null)
                _attack = entity.GetComponent<EnemyAttack>();
            _currentHealthPoints = _startingHealthPoints;
            base.Init();
        }

        public void ChangeHealthPoints(float value)
        {
            _currentHealthPoints += value;
            if (Debug)
            {
                Console.WriteLine(entity.Name + " have lost " + value + " HP(s) -> Now at " + _currentHealthPoints + " HP(s).");
            }
        }

        public void StunLockEffect(float duration)
        {
            _stunLockDuration = duration;
            if (_stunLockDuration > 0)
            {
                entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                State = EnemyState.StunLock;
            }         
        }

        public void AirLock(float duration)
        {
            _airLockDuration = duration;
            if (_airLockDuration > 0)
            {
                entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                entity.rigidbody.GravityScale = 0.0f;
            }
        }

        public override void PreUpdate(GameTime gameTime)
        {
            if (_airLockDuration > 0.0f)
            {
                _airLockDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                entity.rigidbody.GravityScale = 0.0f;
            }
            else
            {
                _airLockDuration = 0.0f;
            }

            if (_stunLockDuration > 0)
            {
                _stunLockDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                State = EnemyState.StunLock;
            }
            else
            {
                if (State == EnemyState.Idle && _followNodes != null)
                {
                    State = EnemyState.Patrol;
                }
                else if (State == EnemyState.StunLock)
                {
                    State = EnemyState.Idle;
                }
            }

            if (entity.spritesheets != null)
            {
                switch (State)
                {
                    case EnemyState.Patrol:
                        entity.spritesheets.ChangeAnimation(_runAnim);
                        break;

                    case EnemyState.Chase:
                        entity.spritesheets.ChangeAnimation(_runAnim);
                        break;

                    case EnemyState.FinishChase:
                        entity.spritesheets.ChangeAnimation(_runAnim);
                        break;

                    case EnemyState.Wait:
                    case EnemyState.WaitNode:
                    case EnemyState.WaitThreat:
                        entity.spritesheets.ChangeAnimation(_idleAnim);
                        break;

                    case EnemyState.Idle:
                        entity.spritesheets.ChangeAnimation(_idleAnim);
                        break;

                    case EnemyState.Attack:
                        entity.spritesheets.ChangeAnimation(_attackAnim);
                        break;
                }
            } 
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
                    
            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);
        }
    }
}
