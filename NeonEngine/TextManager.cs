﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonEngine
{
    public static class TextManager
    {
        public static Dictionary<string, SpriteFont> FontList = new Dictionary<string, SpriteFont>();

        public static void LoadFontList()
        {
            FontList.Add("TimerFont", AssetManager.Content.Load<SpriteFont>("Timer"));
        }
    }
}
