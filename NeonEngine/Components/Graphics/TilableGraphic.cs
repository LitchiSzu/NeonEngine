﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonEngine.Components.Graphics2D
{
    public class TilableGraphic : DrawableComponent
    {
        #region Properties
        private bool _useTextureWidth = true;
        public bool UseTextureWidth
        {
            get { return _useTextureWidth; }
            set { _useTextureWidth = value; }
        }

        private float _tilingWidth = 100.0f;
        public float TilingWidth
        {
            get { return _tilingWidth; }
            set { _tilingWidth = value; }
        }

        private bool _useTextureHeight = true;

        public bool UseTextureHeight
        {
            get { return _useTextureHeight; }
            set { _useTextureHeight = value; }
        }

        private float _tilingHeight = 100.0f;
        public float TilingHeight
        {
            get { return _tilingHeight; }
            set { _tilingHeight = value; }
        }

        public float DrawLayer
        {
            get { return Layer; }
            set { Layer = value; }
        }

        public string graphicTag = "";
        public string GraphicTag
        {
            get
            {
                return graphicTag;
            }
            set
            {
                graphicTag = value;
                _texture = AssetManager.GetTexture(value);
            }
        }

        #endregion

        private Texture2D _texture;
        public bool Active = true;

        public TilableGraphic(Entity entity)
            :base(1.0f, entity, "TilableGraphic")
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (entity != null && _texture != null && Active)
            {
                CurrentEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(_texture, entity.transform.Position + this._parallaxPosition + Offset - new Vector2((_useTextureWidth ? (int)_texture.Width : (int)_tilingWidth), (int)(_useTextureHeight ? (int)_texture.Height : (int)_tilingHeight)) / 2, new Rectangle(0, 0, (int)((_useTextureWidth ? (int)_texture.Width : (int)_tilingWidth / entity.transform.Scale)), (int)((_useTextureHeight ? (int)_texture.Height : (int)_tilingHeight / entity.transform.Scale))), MainColor, entity.transform.rotation + RotationOffset, Vector2.Zero, entity.transform.Scale, this._currentSide == Side.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally, Layer);
            }
        }
    }
}
