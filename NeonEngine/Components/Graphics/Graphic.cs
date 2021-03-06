﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NeonEngine.Components.Graphics2D
{
    public class Graphic : DrawableComponent
    {
        public Texture2D texture;

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
                texture = AssetManager.GetTexture(value);
            }
        }

        public float DrawLayer
        {
            get
            {
                return Layer;
            }
            set
            {
                Layer = value;
            }
        }

        public bool Active = true;


        public Graphic(Entity entity)
            : base(0.5f, entity, "Graphic")
        {
        }

        public Graphic(Texture2D texture, float Layer, Entity entity)
            : base(Layer, entity, "Graphic")
        {
            this.texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (entity != null && texture != null && Active)
            {
                CurrentEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(texture, entity.transform.Position + this._parallaxPosition + Offset, null, Color.Lerp(Color.Transparent, MainColor, Opacity), entity.transform.rotation + RotationOffset, new Vector2(texture.Width / 2, texture.Height / 2), entity.transform.Scale, _currentSide == Side.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally, Layer);
            }
        }
    }
}
