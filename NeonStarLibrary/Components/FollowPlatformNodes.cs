﻿using Microsoft.Xna.Framework;
using NeonEngine;
using NeonEngine.Components.CollisionDetection;
using NeonEngine.Components.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Enemies
{
    class FollowPlatformNodes : FollowNodes
    {
        public LinkedToPath CurrentPlatform;
        private Rigidbody _lastRigidBody;
        private Entity _lastEntityHit;

        public FollowPlatformNodes(Entity entity)
            :base(entity)
        {
            this.Name = "FollowPlatformNodes";
        }

        public override void Init()
        {
            if (entity.rigidbody != null && entity.rigidbody.beacon != null)
            {
                _lastRigidBody = entity.rigidbody.beacon.CheckGround(Vector2.Zero);
                if (_lastRigidBody != null)
                {
                    _lastEntityHit = _lastRigidBody.entity;
                    CurrentPlatform = _lastEntityHit.GetComponent<LinkedToPath>();
                }
            }
            base.Init();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (CurrentPlatform != null && this.CurrentNodeList != CurrentPlatform.LinkedPathNodeList)
                this.CurrentNodeList = CurrentPlatform.LinkedPathNodeList;
            if (entity.rigidbody.beacon != null)
            {
                Rigidbody rg = entity.rigidbody.beacon.CheckGround(Vector2.Zero);

                if (rg != null && rg != _lastRigidBody)
                {
                    SearchForNewPath(rg);
                }
            }
            base.Update(gameTime);
        }

        private void SearchForNewPath(Rigidbody rg)
        {
            LinkedToPath linkedToPath = rg.entity.GetComponent<LinkedToPath>();
            if (linkedToPath != null)
            {
                CurrentPlatform = linkedToPath;
                _lastRigidBody = rg;
                _lastEntityHit = rg.entity;
            }
            else
                CurrentNodeList = null;
        }
    }
}
