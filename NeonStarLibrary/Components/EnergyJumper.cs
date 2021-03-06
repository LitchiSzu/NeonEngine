﻿using Microsoft.Xna.Framework;
using NeonEngine;
using NeonEngine.Components.CollisionDetection;
using NeonStarLibrary.Components.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.EnergyObjects
{
    public class EnergyJumper : EnergyComponent
    {
        #region Properties
        private Vector2 _jumperImpulse;

        public Vector2 JumperImpulse
        {
            get { return _jumperImpulse; }
            set { _jumperImpulse = value; }
        }

        private string _avatarName = "LiOn";

        public string AvatarName
        {
            get { return _avatarName; }
            set { _avatarName = value; }
        }

        private string _jumperOffAnimation = "";

        public string JumperOffAnimation
        {
            get { return _jumperOffAnimation; }
            set { _jumperOffAnimation = value; }
        }

        private string _jumperOnAnimation = "";

        public string JumperOnAnimation
        {
            get { return _jumperOnAnimation; }
            set { _jumperOnAnimation = value; }
        }

        private string _jumperImpulseAnimation = "";

        public string JumperImpulseAnimation
        {
            get { return _jumperImpulseAnimation; }
            set { _jumperImpulseAnimation = value; }
        }
        #endregion

        private Entity _avatarEntity;

        public EnergyJumper(Entity entity)
            :base(entity)
        {
            Name = "EnergyJumper";
            RequiredComponents = new Type[] { typeof(Hitbox) };
        }

        public override void Init()
        {
            _avatarEntity = entity.GameWorld.GetEntityByName(_avatarName);
            
            base.Init();

            if (_powered)
            {
                if (entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(_jumperOnAnimation, true, 0, true, false, true);
            }
            else
            {
                if (entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(_jumperOffAnimation, true, 0, true, false, true);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_powered)
            {
                if (entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(_jumperOnAnimation, false, 0, true, false, true);
                if (_avatarEntity != null && _avatarEntity.hitboxes.Count > 0)
                {
                    if (_avatarEntity.hitboxes[0].hitboxRectangle.Intersects(entity.hitboxes[0].hitboxRectangle))
                    {
                        if (_avatarEntity.rigidbody != null)
                        {
                            ElementSystem es = _avatarEntity.GetComponent<ElementSystem>();
                            if (es != null && es.CurrentElementEffect != null && es.CurrentElementEffect.EffectElement == Element.Thunder)
                                es.CurrentElementEffect.State = ElementState.End; 
                            _avatarEntity.rigidbody.body.LinearVelocity = Vector2.Zero;
                            _avatarEntity.rigidbody.body.ApplyLinearImpulse(_jumperImpulse);
                            if (entity.spritesheets != null)
                                entity.spritesheets.ChangeAnimation(_jumperImpulseAnimation, true, 0, true, false, false);
                        }
                    }
                }
            }
            else
            {
                if (entity.spritesheets != null)
                    entity.spritesheets.ChangeAnimation(_jumperOffAnimation, false, 0, true, false, true);
            }
            base.Update(gameTime);
        }

    }
}
