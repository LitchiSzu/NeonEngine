﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonEngine
{
    static public class Neon
    {
        static public Game Game;
        static public World World;
        static public GraphicsDevice GraphicsDevice;
        static public SpriteBatch SpriteBatch;

        static public int ScreenWidth, ScreenHeight;
        static public Vector2 HalfScreen;

        static public Color FadeColor = Color.Black;
        static public Color ClearColor = new Color(40, 40, 40);

        static public ScriptingEngine NeonScripting;
        static public List<Type> Scripts;

        static public bool WaterSplash = true;

        static public bool DebugViewEnabled = false;
        static public bool DrawHitboxes = false;
        static public GraphicsDeviceManager GraphicsDeviceManager;
        static public int ElapsedTime;

        static public Input Input;

        static public Utils Utils;

        static public void Start(Game game1, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, int screenWidth, int screenheight)
        {
            

            Console.Title = "Neon Engine Console";

            Console.WriteLine(@"");
            Console.WriteLine(@"");
            Console.WriteLine(@"--------------------------------------------------------------------------------");
            Console.WriteLine(@"");
            Console.WriteLine(@"");
            Console.WriteLine(@"               _   __                    ______            _          ");
            Console.WriteLine(@"              / | / /__  ____  ____     / ____/___  ____ _(_)___  ___");
            Console.WriteLine(@"             /  |/ / _ \/ __ \/ __ \   / __/ / __ \/ __ `/ / __ \/ _ \");
            Console.WriteLine(@"            / /|  /  __/ /_/ / / / /  / /___/ / / / /_/ / / / / /  __/");
            Console.WriteLine(@"           /_/ |_/\___/\____/_/ /_/  /_____/_/ /_/\__, /_/_/ /_/\___/ ");
            Console.WriteLine(@"                                                 /____/               ");
            Console.WriteLine(@"");
            Console.WriteLine(@"");

            Console.WriteLine(@"  ___ ____ _  _ _ ____    ____ ____ ____ ____ ____ _  _    ___ ____ ____ _  _ ");
            Console.WriteLine(@"   |  |  |  \/  | |       |__/ |__| |    |  | |  | |\ |     |  |___ |__| |\/| ");
            Console.WriteLine(@"   |  |__| _/\_ | |___    |  \ |  | |___ |__| |__| | \|     |  |___ |  | |  | ");
            Console.WriteLine(@"                                                                              ");

            Console.WriteLine(@"");
            Console.WriteLine(@"");

            Console.WriteLine(@"--------------------------------------------------------------------------------");


            Console.WriteLine("Neon Engine is starting...");
            Console.WriteLine(@"");
            Console.WriteLine(@"");

            Game = game1;

            ScreenWidth = screenWidth;
            ScreenHeight = screenheight;
            HalfScreen = new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            GraphicsDevice = Game.GraphicsDevice;
            Neon.SpriteBatch = spriteBatch;
            AssetManager.Initialize(GraphicsDevice);

            SoundManager.InitializeSounds();
            SoundManager.Load(Game.Content);
            TextManager.LoadFontList();

            Input = Input.Instance;
            Utils = new Utils();
            NeonScripting = new ScriptingEngine();
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.ApplyChanges();
            GraphicsDeviceManager = graphics;

            Console.WriteLine("Neon Engine started !");
            Console.WriteLine(@"");
            Console.WriteLine(@"");
        }
    }
}
