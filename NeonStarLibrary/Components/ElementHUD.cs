﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonEngine;
using NeonEngine.Components.Graphics2D;
using NeonStarLibrary.Components.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.HUD
{
    public class ElementHUD : DrawableComponent
    {
        #region Properties
        private string _avatarName = "";

        public string AvatarName
        {
          get { return _avatarName; }
          set { _avatarName = value; }
        }

        private string _elementCooldownFinishedSpriteSheetTag = "";

        public string ElementCooldownFinishedSpriteSheetTag
        {
            get { return _elementCooldownFinishedSpriteSheetTag; }
            set { _elementCooldownFinishedSpriteSheetTag = value; }
        }
        #endregion

        private Entity _avatar = null;

        private ElementSystem _elementSystem = null;

        private Texture2D _leftCharacterHUD = null;
        private Texture2D _rightCharacterHUD = null;

        private Texture2D _emptySlotHUD = null;

        private Texture2D _fireElementHUD = null;
        private Texture2D _thunderElementHUD = null;

        private SpriteSheetInfo _elementCooldownFinished;

        public ElementHUD(Entity entity)
            :base(0.99f, entity, "ElementHUD")
        {
            IsHUD = true;
        }

        public override void Init()
        {
            _leftCharacterHUD = AssetManager.GetTexture("HUDLeftCharacter");
            _rightCharacterHUD = AssetManager.GetTexture("HUDRightCharacter");

            _emptySlotHUD = AssetManager.GetTexture("HUDElementNeutral");

            _fireElementHUD = AssetManager.GetTexture("HUDElementCrystalGauge");
            _thunderElementHUD = AssetManager.GetTexture("HUDElementThunderGauge");

            _avatar = entity.GameWorld.GetEntityByName(_avatarName);
            if (_avatar != null)
                _elementSystem = _avatar.GetComponent<ElementSystem>();
            _elementCooldownFinished = AssetManager.GetSpriteSheet(_elementCooldownFinishedSpriteSheetTag);
            base.Init();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_elementSystem != null)
            {
                Texture2D leftTextureToUse = null;
                Texture2D rightTextureToUse = null;
                for (int i = 0; i < _elementSystem.ElementSlots[0].Length; i++)
                {
                    switch(_elementSystem.ElementSlots[0][i].Type)
                    {
                        case Element.Thunder:
                            leftTextureToUse = _thunderElementHUD;
                            break;

                        case Element.Fire:
                            leftTextureToUse = _fireElementHUD;
                            break;

                    }
                   //spriteBatch.Draw(leftTextureToUse, entity.transform.Position + Offset + new Vector2(0, i * (leftTextureToUse.Height * 2) + i * 4), null, leftTextureToUse == _emptySlotHUD ? Color.White : Color.Lerp(Color.Black, Color.White, 0.3f), entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);    
                    if(_elementSystem.ElementSlots[0][i].Type != Element.Neutral)
                        spriteBatch.Draw(leftTextureToUse, entity.transform.Position + Offset + new Vector2(0, i * (leftTextureToUse.Height * 2) + i * 4) + new Vector2(1, 5), new Rectangle(0, 0, (int)Math.Round(leftTextureToUse.Width * (1 - _elementSystem.ElementSlots[0][i].Cooldown / Math.Max(_elementSystem.ElementSlotCooldownDuration, 0.1f))), leftTextureToUse.Height), Color.White, entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);    
                }

                for (int i = 0; i < _elementSystem.ElementSlots[1].Length; i++)
                {
                        switch (_elementSystem.ElementSlots[1][i].Type)
                        {
                            case Element.Thunder:
                                rightTextureToUse = _thunderElementHUD;
                                break;

                            case Element.Fire:
                                rightTextureToUse = _fireElementHUD;
                                break;
                        }
                    //spriteBatch.Draw(rightTextureToUse, entity.transform.Position + Offset + new Vector2(leftTextureToUse.Width * entity.transform.Scale + 10, i * (rightTextureToUse.Height * 2) + i * 4), null, rightTextureToUse == _emptySlotHUD ? Color.White : Color.Lerp(Color.Black, Color.White, 0.3f), entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);
                    if (_elementSystem.ElementSlots[1][i].Type != Element.Neutral)
                        spriteBatch.Draw(rightTextureToUse, entity.transform.Position + Offset + new Vector2(_fireElementHUD.Width * entity.transform.Scale + 10, i * (_fireElementHUD.Height * 2) + i * 4) + new Vector2(23, 5), new Rectangle(0, 0, (int)Math.Round(_fireElementHUD.Width * (1 - _elementSystem.ElementSlots[1][i].Cooldown / Math.Max(_elementSystem.ElementSlotCooldownDuration, 0.1f))), rightTextureToUse.Height), Color.White, entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);    
                }

                if (_leftCharacterHUD != null)
                    spriteBatch.Draw(_leftCharacterHUD, entity.transform.Position + Offset + new Vector2(_fireElementHUD.Width / 2 * entity.transform.Scale - _leftCharacterHUD.Width + 2, (_elementSystem.MaxLevel) * (_fireElementHUD.Height * 2) + _elementSystem.MaxLevel * 4 + 5), null, Color.White, entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);
                if (_rightCharacterHUD != null)
                    spriteBatch.Draw(_rightCharacterHUD, entity.transform.Position + Offset + new Vector2(_fireElementHUD.Width * entity.transform.Scale + _fireElementHUD.Width / 2 * entity.transform.Scale + 35 - _rightCharacterHUD.Width, (_elementSystem.MaxLevel) * (_fireElementHUD.Height * 2) + _elementSystem.MaxLevel * 4 + 5), null, Color.White, entity.transform.rotation, Vector2.Zero, entity.transform.Scale, SpriteEffects.None, Layer);
            }
            base.Draw(spriteBatch);
        }

        public void CooldownFinished(int row, int column)
        {
            //ROW TO MANAGE
            EffectsManager.GetEffect(_elementCooldownFinished, Side.Right, entity.transform.Position + Offset + new Vector2(_elementCooldownFinished.FrameWidth / 2, 0 * (_fireElementHUD.Height * 2) + 0 * 4) + new Vector2(22, 7) + new Vector2(column * 120, 0), 0.0f, Vector2.Zero, 2.0f, 1.0f, null, 50.0f, true, true);
        }
    }
}
