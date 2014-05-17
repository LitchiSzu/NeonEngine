﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeonEngine;
using NeonEngine.Components.Graphics2D;
using NeonStarLibrary.Components.EnergyObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Menu
{
    public enum MenuState
    {
        PressStart,
        ModeSelect,
        ChapterSelect
    }

    class MainMenu : Graphic
    {
        #region Properties
        private float _opacityChangeValue = 0.5f;

        public float OpacityChangeValue
        {
            get { return _opacityChangeValue; }
            set { _opacityChangeValue = value; }
        }
        #endregion
        
        private bool _opacityGoingDown = true;

        private MenuState _menuState = MenuState.PressStart;
        List<DrawableComponent> _drawableComponents = new List<DrawableComponent>();

        private Graphic _selectBar = null;

        private Graphic _firstGraphic = null;
        private Graphic _secondGraphic = null;

        private Entity _firstChapterEntity;
        private Entity _secondChapterEntity;

        private bool _firstUpdateDone = false;
        private int _currentSelection = 0;

        public MainMenu(Entity entity)
            :base(entity)
        {
            Name = "MainMenu";
        }

        public override void Init()
        {
            ChangeState(MenuState.PressStart);
            this.Opacity = 1.0f;
            _firstChapterEntity = entity.GameWorld.GetEntityByName("FirstChapter");
            _secondChapterEntity = entity.GameWorld.GetEntityByName("SecondChapter");
            if (_firstChapterEntity != null)
                _firstChapterEntity.spritesheets.Active = false;
            if (_secondChapterEntity != null)
                _secondChapterEntity.spritesheets.Active = false;

            DeviceManager.AlreadyLoaded = false;
            DeviceManager.LoadDevicesInformation();
            GameScreen.CheckPointsData.Clear();
            CollectibleManager.ResetCollectibles();
            CollectibleManager.InitializeCollectibles(entity.GameWorld as GameScreen);
 	        base.Init();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!_firstUpdateDone)
            {
                DeviceManager.AlreadyLoaded = false;
                DeviceManager.LoadDevicesInformation();
                GameScreen.CheckPointsData.Clear();
                HealStation._usedHealStations.Clear();
                CollectibleManager.ResetCollectibles();
                CollectibleManager.InitializeCollectibles(entity.GameWorld as GameScreen);
                _firstUpdateDone = true;
            }
            switch(_menuState)
            {
                case MenuState.PressStart:
                    
                    if (_opacityGoingDown)
                    {
                        if (Opacity > 0)
                        {
                            Opacity -= _opacityChangeValue * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            _opacityGoingDown = false;
                            Opacity = 0.0f;
                        }
                    }
                    else
                    {
                        if (Opacity < 1)
                        {
                            Opacity += _opacityChangeValue * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            _opacityGoingDown = true;
                            Opacity = 1.0f;
                        }
                    }

                    foreach (DrawableComponent dc in _drawableComponents)
                        (dc as Graphic).Opacity = Opacity;                 

                    if (Neon.Input.Pressed(Buttons.Start))
                        ChangeState(MenuState.ModeSelect);
                    break;    

                case MenuState.ModeSelect:

                    if (Neon.Input.Pressed(NeonStarInput.MoveDown))
                    {
                        _currentSelection = (_currentSelection + 1) % 2;
						SoundManager.GetSound("menuTemp").Play();					//UP Sfx
                    }
                    else if (Neon.Input.Pressed(NeonStarInput.MoveUp))
                    {
                        _currentSelection = Math.Abs((_currentSelection - 1)) % 2;
						SoundManager.GetSound("menuTemp").Play();					//DOWN Sfx
                    }

                    if (_currentSelection == 0)
                    {
                        _firstGraphic.GraphicTag = "NewGameOn";
                        _secondGraphic.GraphicTag = "ChaptersOff";
                        _selectBar.Offset = Vector2.Zero;
                    }
                    else
                    {
                        _firstGraphic.GraphicTag = "NewGameOff";
                        _secondGraphic.GraphicTag = "ChaptersOn";
                        _selectBar.Offset = new Vector2(0, 40);
                    }
                    if (Neon.Input.Pressed(NeonStarInput.Guard))
                    {
                        ChangeState(MenuState.PressStart);
                    }


                    if (Neon.Input.Pressed(NeonStarInput.Jump))
                    {
                        if (_currentSelection == 0)
                            entity.GameWorld.ChangeLevel("01TrainingLevel", "00TrainingOpening", 0);
                        else
                            ChangeState(MenuState.ChapterSelect);
                    }
                    break;

                case MenuState.ChapterSelect:
                    if (_selectBar != null)
                    {
                        if (_opacityGoingDown)
                        {
                            if (this._selectBar.Opacity > 0)
                            {
                                this._selectBar.Opacity -= _opacityChangeValue * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else
                            {
                                _opacityGoingDown = false;
                                this._selectBar.Opacity = 0.0f;
                            }
                        }
                        else
                        {
                            if (this._selectBar.Opacity < 1)
                            {
                                this._selectBar.Opacity += _opacityChangeValue * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else
                            {
                                _opacityGoingDown = true;
                                this._selectBar.Opacity = 1.0f;
                            }
                        }

                        if (( _firstChapterEntity.spritesheets.IsFinished() || _firstChapterEntity.spritesheets.CurrentSpritesheet.IsLooped) && (_secondChapterEntity.spritesheets.IsFinished() || _secondChapterEntity.spritesheets.CurrentSpritesheet.IsLooped))
                        {
                            if (Neon.Input.Pressed(NeonStarInput.MoveRight))
                            {
                                _currentSelection = (_currentSelection + 1) % 1;
                            }
                            else if (Neon.Input.Pressed(NeonStarInput.MoveLeft))
                            {
                                _currentSelection = Math.Abs((_currentSelection - 1)) % 1;
                            }
                        }
                        

                        if (_currentSelection == 0)
                        {
                            if (_firstChapterEntity.spritesheets.IsFinished() && _firstChapterEntity.spritesheets.CurrentSpritesheetName == "Opening")
                                _firstChapterEntity.spritesheets.ChangeAnimation("Opened");
                            if (_secondChapterEntity.spritesheets.IsFinished() && _secondChapterEntity.spritesheets.CurrentSpritesheetName == "Opening")
                            {
                                _secondChapterEntity.spritesheets.ChangeAnimation("Closed");
                                _secondChapterEntity.spritesheets.CurrentSpritesheet.Reverse = false;
                            }
                            else if (_firstChapterEntity.spritesheets.CurrentSpritesheetName == "Closed")
                            {
                                _firstChapterEntity.spritesheets.ChangeAnimation("Opening", 0, true, false, false);
                                _secondChapterEntity.spritesheets.ChangeAnimation("Opening", 0, true, false, false);
                                _secondChapterEntity.spritesheets.CurrentSpritesheet.Reverse = true;
                                _secondChapterEntity.spritesheets.CurrentSpritesheet.currentFrame = _secondChapterEntity.spritesheets.CurrentSpritesheet.spriteSheetInfo.FrameCount - 1;
                            }
                            
                            _selectBar.Offset = Vector2.Zero;
                        }
                        else
                        {
                            if (_secondChapterEntity.spritesheets.IsFinished() && _secondChapterEntity.spritesheets.CurrentSpritesheetName == "Opening")
                                _secondChapterEntity.spritesheets.ChangeAnimation("Opened");
                            if (_firstChapterEntity.spritesheets.IsFinished() && _firstChapterEntity.spritesheets.CurrentSpritesheetName == "Opening")
                            {
                                _firstChapterEntity.spritesheets.ChangeAnimation("Closed");
                                _firstChapterEntity.spritesheets.CurrentSpritesheet.Reverse = false;
                            }
                            else if (_secondChapterEntity.spritesheets.CurrentSpritesheetName == "Closed")
                            {
                                _secondChapterEntity.spritesheets.ChangeAnimation("Opening", 0, true, false, false);
                                _firstChapterEntity.spritesheets.ChangeAnimation("Opening", 0, true, false, false);
                                _firstChapterEntity.spritesheets.CurrentSpritesheet.Reverse = true;
                                _firstChapterEntity.spritesheets.CurrentSpritesheet.currentFrame = _firstChapterEntity.spritesheets.CurrentSpritesheet.spriteSheetInfo.FrameCount - 1;
                            }
                            _selectBar.Offset = new Vector2(0, 40);
                        }
                        
                        if (Neon.Input.Pressed(NeonStarInput.Guard))
                        {
                            ChangeState(MenuState.ModeSelect);
                            _firstChapterEntity.spritesheets.Active = false;
                            _secondChapterEntity.spritesheets.Active = false;
                        }

                        if (Neon.Input.Pressed(NeonStarInput.Jump))
                        {
                            if (_currentSelection == 0)
                                entity.GameWorld.ChangeLevel("01TrainingLevel", "01TrainingEntrance", 0);
                            /*else
                                entity.GameWorld.ChangeLevel("SequenceOneCity", "CityOne", 0);*/
                        }

                        
                    }
                    break;
            }
            
            base.Update(gameTime);
        }

        public void ChangeState(MenuState newState)
        {
            _menuState = newState;
            for (int i = _drawableComponents.Count - 1; i >= 0; i--)
            {
                DrawableComponent dc = _drawableComponents[i];
                dc.Remove();
            }
            _drawableComponents.Clear();

            switch(newState)
            {
                case MenuState.PressStart:
                    Graphic g = new Graphic(this.entity);
                    g.GraphicTag = "PressStart";
                    g.ParallaxForce = Vector2.One;
                    entity.AddComponent(g);
                    g.Layer = 0.7f;
                    _drawableComponents.Add(g);
                    g = new Graphic(this.entity);
                    g.GraphicTag = "SelectBar";
                    g.ParallaxForce = Vector2.One;
                    g.Layer = 0.7f;
                    entity.AddComponent(g);
                    _drawableComponents.Add(g);
                    break;

                case MenuState.ModeSelect:
                    _selectBar = new Graphic(this.entity);
                    _selectBar.GraphicTag = "SelectBar";
                    _selectBar.ParallaxForce = Vector2.One;
                    _selectBar.Layer = 0.7f;
                    entity.AddComponent(_selectBar);
                    _drawableComponents.Add(_selectBar);
                    _firstGraphic = new Graphic(this.entity);
                    _firstGraphic.GraphicTag = "NewGameOn";
                    _firstGraphic.ParallaxForce = Vector2.One;
                    _firstGraphic.Layer = 0.7f;
                    entity.AddComponent(_firstGraphic);
                    _drawableComponents.Add(_firstGraphic);
                    _secondGraphic = new Graphic(this.entity);
                    _secondGraphic.GraphicTag = "ChaptersOff";
                    _secondGraphic.ParallaxForce = Vector2.One;
                    _secondGraphic.Layer = 0.7f;
                    _secondGraphic.Offset = new Vector2(0, 40);
                    entity.AddComponent(_secondGraphic);
                    _drawableComponents.Add(_secondGraphic);
                    break;

                case MenuState.ChapterSelect:
                        _currentSelection = 0;
                        _firstChapterEntity.spritesheets.Active = true;
                        _firstChapterEntity.spritesheets.ChangeAnimation("Opening", 0, true, false, false);
                        _secondChapterEntity.spritesheets.Active = true;
                        _secondChapterEntity.spritesheets.ChangeAnimation("Closed", 0, true, false, false);
                    break;
            }
        
        }
       

    }
}
