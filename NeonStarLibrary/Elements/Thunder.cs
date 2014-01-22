﻿using Microsoft.Xna.Framework;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public class Thunder : ElementEffect
    {
        private SpriteSheetInfo _thunderEffectSpritesheetInfo = null;
        private SpriteSheetInfo _thunderFinishSpritesheetInfo = null;
    
        private AnimatedSpecialEffect effect;
        private AnimatedSpecialEffect finishEffect;
        
        private Vector2 _dashImpulse = Vector2.Zero;
        private float _dashDuration = 0.15f;
        private string _attackToLaunch = "";
        private Attack ThunderAttack;

        public Thunder(ElementSystem elementSystem, int elementLevel, Entity entity, NeonStarInput input, GameScreen world)
            :base(elementSystem, elementLevel, entity, input, world)
        {           
        }

        public override void InitializeLevelParameters()
        {
            _thunderEffectSpritesheetInfo = AssetManager.GetSpriteSheet("LiOnThunderDashLink");
            _thunderFinishSpritesheetInfo = AssetManager.GetSpriteSheet("LiOnThunderDashFinish");
            
            this.EffectElement = Element.Thunder;
            switch (_elementLevel)
            {
                case 1:
                    _gaugeCost = (float)ElementManager.ThunderParameters[0][0];
                    _dashImpulse = new Vector2(_entity.spritesheets.CurrentSide == Side.Right ? (float)ElementManager.ThunderParameters[0][1] : -(float)ElementManager.ThunderParameters[0][1], 0);
                    _dashDuration = (float)ElementManager.ThunderParameters[0][2];
                    _attackToLaunch = (string)ElementManager.ThunderParameters[0][3];
                    break;

                case 2:
                    _gaugeCost = (float)ElementManager.ThunderParameters[1][0];
                    if (Neon.Input.Check(NeonStarInput.MoveUp))
                        _dashImpulse = new Vector2(0, -(float)ElementManager.ThunderParameters[1][4]);
                    else if (Neon.Input.Check(NeonStarInput.MoveDown))
                        _dashImpulse = new Vector2(0, (float)ElementManager.ThunderParameters[1][5]);
                    else
                        _dashImpulse = new Vector2(_entity.spritesheets.CurrentSide == Side.Right ? (float)ElementManager.ThunderParameters[1][1] : -(float)ElementManager.ThunderParameters[1][1], 0);
                    _dashDuration = (float)ElementManager.ThunderParameters[1][2];
                    _attackToLaunch = (string)ElementManager.ThunderParameters[1][3];
                    break;

                case 3:
                    _gaugeCost = (float)ElementManager.ThunderParameters[2][0];
                    if (Neon.Input.Check(NeonStarInput.MoveUp))
                        _dashImpulse = new Vector2(0, -(float)ElementManager.ThunderParameters[2][4]);
                    else if (Neon.Input.Check(NeonStarInput.MoveDown))
                        _dashImpulse = new Vector2(0, (float)ElementManager.ThunderParameters[2][5]);
                    else
                        _dashImpulse = new Vector2(_entity.spritesheets.CurrentSide == Side.Right ? (float)ElementManager.ThunderParameters[2][1] : -(float)ElementManager.ThunderParameters[2][1], 0);
                    _dashDuration = (float)ElementManager.ThunderParameters[2][2];
                    _attackToLaunch = (string)ElementManager.ThunderParameters[2][3];
                    break;
            }
            base.InitializeLevelParameters();
        }

        public override void PreUpdate(GameTime gameTime)
        {
            switch (State)
            {
                case ElementState.Initialization:
                case ElementState.Charge:
                case ElementState.Effect:
                    _elementSystem.AvatarComponent.CanMove = false;
                    _elementSystem.AvatarComponent.CanTurn = false;
                    _elementSystem.AvatarComponent.CanAttack = false;
                    break;
            }
            base.PreUpdate(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch(State)
            {
                case ElementState.Initialization:
                    _entity.spritesheets.ChangeAnimation(_elementSystem.ThunderLaunchAnimation, true, 0, true, false, false);
                    _entity.containerWorld.Camera.ChaseStrength = 0.0f;
                    _entity.rigidbody.body.LinearVelocity = Vector2.Zero;  
                    _entity.rigidbody.GravityScale = 0.0f;
                    _entity.hitboxes[0].SwitchType(HitboxType.Invincible, _dashDuration);
                    if (_entity.spritesheets.IsFinished())
                    {
                        State = ElementState.Charge;
                        if (effect == null)
                        {
                            float angle = _dashImpulse.X == 0 ? 0.0f : (float)Math.PI / 2;
                            Vector2 offset;
                            if (_dashImpulse.X == 0)
                            {
                                offset = Vector2.Zero;
                            }
                            else
                            {
                                offset = new Vector2(240, 0);
                            }
                            effect = EffectsManager.GetEffect(_thunderEffectSpritesheetInfo, _dashImpulse.X != 0 ? _elementSystem.AvatarComponent.CurrentSide : Side.Right, _elementSystem.entity.transform.Position, angle, offset, 2.0f, 0.9f);
                        }
                    }
                    
                    break;

                case ElementState.Charge:                                                         
                    State = ElementState.Effect;
                    
                    _entity.rigidbody.body.ApplyLinearImpulse(_dashImpulse);
                    _entity.spritesheets.Active = false;
                    break;

                case ElementState.Effect:
                    if (finishEffect == null)
                    {
                        if (_dashDuration > 0.0f)
                        {

                            _entity.rigidbody.GravityScale = 0.0f;
                            _dashDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (ThunderAttack != null) ThunderAttack.Update(gameTime);
                        }

                        if (_dashDuration <= 0.0f)
                        {
                            _dashDuration = 0.0f;
                            _entity.rigidbody.body.LinearVelocity = Vector2.Zero;
                            if (ThunderAttack != null) ThunderAttack.CancelAttack();
                            ThunderAttack = null;
                            State = ElementState.End;
                            finishEffect = EffectsManager.GetEffect(_thunderFinishSpritesheetInfo, _elementSystem.AvatarComponent.CurrentSide, _entity.transform.Position, 0.0f, Vector2.Zero, 2.0f, 0.8f);
                            switch (_input)
                            {
                                case NeonStarInput.UseLeftSlotElement:
                                    _elementSystem.LeftSlotEnergy -= _gaugeCost;
                                    break;

                                case NeonStarInput.UseRightSlotElement:
                                    _elementSystem.RightSlotEnergy -= _gaugeCost;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (finishEffect.spriteSheet.currentFrame == finishEffect.spriteSheet.spriteSheetInfo.FrameCount - 1)
                        {
                            State = ElementState.End;
                        }
                    }
                                   


                    break;

                case ElementState.End:                
                    ThunderAttack.CancelAttack();
                    ThunderAttack = null;
                    break;
            }
            base.Update(gameTime);
        }
    }
}
