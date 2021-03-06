﻿using Microsoft.Xna.Framework;
using NeonEngine;
using NeonStarLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Enemies
{
    public class FollowNodes : Component
    {

        #region Properties
        private PathNodeList _currentNodeList;

        public PathNodeList CurrentNodeList
        {
            get { return _currentNodeList; }
            set
            {
                _currentNodeList = value;
                Init();
            }
        }

        private float _speed = 1.5f;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        private bool _roundtrip = true;

        public bool Roundtrip
        {
            get { return _roundtrip; }
            set { _roundtrip = value; }
        }

        private bool _reverseStart;

        public bool ReverseStart
        {
            get { return _reverseStart; }
            set { _reverseStart = value; }
        }

        private float _pathPrecisionTreshold = 50.0f;

        public float PathPrecisionTreshold
        {
            get { return _pathPrecisionTreshold; }
            set { _pathPrecisionTreshold = value; }
        }
        #endregion

        public EnemyCore EnemyComponent;

        public bool Active = true;
        public bool TookDamageLastFrame = false;

        private Node _nextNode;
        private Node _previousNode;
        private bool _reverse;
        private float _currentNodeDelay = 0.0f;
        
        public FollowNodes(Entity entity)
            :base(entity, "FollowNodes")
        {
            RequiredComponents = new Type[] { typeof(EnemyCore) };
        }

        public override void Init()
        {
            this.EnemyComponent = entity.GetComponent<EnemyCore>();
            if (_currentNodeList != null)
            {

                Node CloserNode = CurrentNodeList.Nodes[0];

                for (int i = 1; i < CurrentNodeList.Nodes.Count; i++)
                {
                    if (Math.Sqrt(Math.Pow(CurrentNodeList.Nodes[i].Position.X - entity.transform.Position.X, 2)) < Math.Sqrt(Math.Pow(CloserNode.Position.X - entity.transform.Position.X, 2)))
                    {
                        CloserNode = CurrentNodeList.Nodes[i];
                    }
                }
                _nextNode = CloserNode;
 
                _reverseStart = _reverse;
            }  
            base.Init();
        }

        public override void PreUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {   
            if (entity.rigidbody.isGrounded || EnemyComponent.Type == EnemyType.Flying)
            {
                if (EnemyComponent.State == EnemyState.Idle)
                {
                    EnemyComponent.State = EnemyState.Patrol;
                }

                if (_currentNodeList != null)
                {
                    switch (EnemyComponent.State)
                    {
                        case EnemyState.WaitNode:
                            entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                            _currentNodeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (_currentNodeDelay <= 0.0f)
                            {
                                _currentNodeDelay = 0.0f;
                                EnemyComponent.State = EnemyState.Patrol;
                                SearchNextNode();
                            }
                            break;

                        case EnemyState.Patrol:
                                if (this._nextNode.Position.X + _pathPrecisionTreshold > entity.transform.Position.X && this._nextNode.Position.X - _pathPrecisionTreshold < entity.transform.Position.X)
                                {
                                    switch (_nextNode.Type)
                                    {
                                        case NodeType.Move:
                                            SearchNextNode();
                                            break;

                                        case NodeType.DelayedMove:
                                            _currentNodeDelay = _nextNode.NodeDelay;
                                            this.EnemyComponent.State = EnemyState.WaitNode;
                                            break;
                                    }
                                }
                            break;
                    }
                }
            }
            
            
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (entity.rigidbody.isGrounded || EnemyComponent.Type == EnemyType.Flying)
            {
                if (EnemyComponent.State == EnemyState.Patrol)
                {
                    if (_nextNode != null)
                    {
                        if (this._nextNode.Position.X < this.entity.transform.Position.X)
                        {
                            EnemyComponent.CurrentSide = Side.Left;
                            if (EnemyComponent.Type == EnemyType.Ground && entity.rigidbody.isGrounded || EnemyComponent.Type == EnemyType.Flying)
                                this.entity.rigidbody.body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(-_speed, this.entity.rigidbody.body.LinearVelocity.Y);
                        }
                        else
                        {
                            EnemyComponent.CurrentSide = Side.Right;
                            if (EnemyComponent.Type == EnemyType.Ground && entity.rigidbody.isGrounded || EnemyComponent.Type == EnemyType.Flying)
                                this.entity.rigidbody.body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(_speed, this.entity.rigidbody.body.LinearVelocity.Y);
                        }
                    }
                }  
            }
                  
                                                         
            base.Update(gameTime);
        }

        private void SearchNextNode()
        {
            this._previousNode = this._nextNode;

            int newIndex = 0;
            int oldIndex = _currentNodeList.Nodes.IndexOf(_previousNode);
            if (_roundtrip)
            {
                if (oldIndex == 0)
                {
                    newIndex = 1;
                    _reverse = false;
                }
                else if (oldIndex == _currentNodeList.Nodes.Count - 1)
                {
                    newIndex = _currentNodeList.Nodes.Count - 2;
                    _reverse = true;
                }
                else
                {
                    newIndex = oldIndex + (_reverse ? -1 : 1);
                }

            }
            this._nextNode = _currentNodeList.Nodes[newIndex];
        }

    }
}
