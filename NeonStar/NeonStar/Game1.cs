using System;
using System.Collections.Generic;
using System.Linq;
using NeonEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NeonStarEditor;
using NeonStarLibrary;
using System.IO;
using System.Xml.Linq;

namespace NeonStar
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            InactiveSleepTime = TimeSpan.Zero;
            IsFixedTimeStep = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Window.Title = "NeonStar " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (Neon.World is EditorScreen)
            {
                EditorScreen editorScreen = Neon.World as EditorScreen;

                XDocument preferenceFile = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                XElement content = new XElement("XnaContent");
                XElement preferences = new XElement("Preferences");

                XElement groupToLoad = new XElement("GroupToLoad");
                groupToLoad.Value = editorScreen.LevelGroupName;
                preferences.Add(groupToLoad);

                XElement levelToLoad = new XElement("LevelToLoad");
                levelToLoad.Value = editorScreen.LevelName;
                preferences.Add(levelToLoad);

                XElement showEditor = new XElement("ShowEditor");
                showEditor.Value = editorScreen.EditorVisible.ToString();

                preferences.Add(showEditor);

                XElement showHitboxes = new XElement("ShowHitboxes");
                showHitboxes.Value = Neon.DrawHitboxes.ToString();

                preferences.Add(showHitboxes);

                XElement showPhysics = new XElement("ShowPhysics");
                showPhysics.Value = Neon.DebugViewEnabled.ToString();

                preferences.Add(showPhysics);

                XElement defaultLayer = new XElement("DefaultLayer");
                defaultLayer.Value = (Neon.World as EditorScreen).DefaultLayer;

                preferences.Add(defaultLayer);

                content.Add(preferences);
                preferenceFile.Add(content);

                preferenceFile.Save(@"../Data/Config/EditorPreferences.xml");

                (Neon.World as GameScreen).SaveProgressionToFile();
            }
            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            base.Initialize();       
            
            Neon.Start(this, graphics, spriteBatch, 1280, 720);
            Neon.Input.AssignCustomControls(typeof(NeonStarInput));
            Neon.NeonScripting.AddAssembly("NeonStarLibrary.dll");
            Neon.NeonScripting.AddAssembly("NeonStarEditor.dll");
            Neon.Scripts = Neon.NeonScripting.CompileScripts().ToList<Type>();

            Neon.ClearColor = Color.Black;
            #if DEBUG
            Neon.World = new NeonStarEditor.LoadingScreen(this, 0, "", "", null, true);
            #else                    
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Neon.World = new NeonStarLibrary.LoadingScreen(this, 0, "PreprodPresentation", "Main");
            
            #endif
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Neon.World.UpdateWorld(gameTime);
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Neon.ClearColor);
            Neon.World.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
