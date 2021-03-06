﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NeonEngine;
using NeonEngine.Components.Graphics2D;
using NeonEngine.Private;
using NeonStarLibrary.Components.Enemies;
using NeonStarLibrary.Components.EnergyObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Scripts
{
    public class TrainingScanAlarmScript : ScriptComponent
    {
        private string _scanDoorName = "";
        public string ScanDoorName
        {
            get { return _scanDoorName; }
            set { _scanDoorName = value; }
        }

        private string _importantRobotName = "ImportantRobot";
        public string ImportantRobotName
        {
            get { return _importantRobotName; }
            set { _importantRobotName = value; }
        }

        private string _secondImportantRobotName = "ImportantRobot2";

        public string SecondImportantRobotName
        {
            get { return _secondImportantRobotName; }
            set { _secondImportantRobotName = value; }
        }

        private string _thirdImportantRobotName = "ImportantRobot3";

        public string ThirdImportantRobotName
        {
            get { return _thirdImportantRobotName; }
            set { _thirdImportantRobotName = value; }
        }

        private Entity _scanDoor;
        private Entity _importantRobot;
        private Entity _secondImportantRobot;
        private Entity _thirdImportantRobot;
        private EnemyCore _importantRobotEnemyCore;

        private List<DrawableComponent> _guardTutorialComponents;
        private List<DrawableComponent> _comboTutorialComponents;
        private bool _fadingTutorials = false;

        private QuotaEnergyDoor _arenaDoor;

        public TrainingScanAlarmScript(Entity entity)
            : base(entity, "TrainingScanAlarmScript")
        {
        }

        public override void Init()
        {
            Entity e = entity.GameWorld.GetEntityByName("ExitDoor");
            if (e != null)
                _arenaDoor = e.GetComponent<QuotaEnergyDoor>();
            
            this.entity.spritesheets.ChangeAnimation("Opened", 0, true, false, false);

            if (_scanDoorName != "")
                _scanDoor = entity.GameWorld.GetEntityByName(_scanDoorName);
            if (_scanDoor != null)
            {
                _scanDoor.rigidbody.IsGround = false;
                _scanDoor.rigidbody.Init();
            }

            _importantRobot = entity.GameWorld.GetEntityByName(_importantRobotName);
            if (_importantRobot != null)
                _importantRobotEnemyCore = _importantRobot.GetComponent<EnemyCore>();
            _importantRobot = null;

            _secondImportantRobot = entity.GameWorld.GetEntityByName(_secondImportantRobotName);
            _thirdImportantRobot = entity.GameWorld.GetEntityByName(_thirdImportantRobotName);

            e = entity.GameWorld.GetEntityByName("GuardTutorial");
            if (e != null)
                _guardTutorialComponents = e.GetComponentsByInheritance<DrawableComponent>();

            e = entity.GameWorld.GetEntityByName("ComboTutorial");
            if (e != null)
                _comboTutorialComponents = e.GetComponentsByInheritance<DrawableComponent>();

            if (_comboTutorialComponents != null)
                foreach (DrawableComponent dc in _comboTutorialComponents)
                    dc.Opacity = 0.0f;

            base.Init();
        }

        public override void OnTrigger(Entity trigger, Entity triggeringEntity, object[] parameters = null)
        {
            this.entity.spritesheets.ChangeAnimation("Closing", 0, true, false, false);
            
            if(SoundManager.MusicLock)  
                SoundManager.PrepareTrack("BattleTransition");
            if (_scanDoor != null)
            {
                _scanDoor.rigidbody.IsGround = true;
                _scanDoor.rigidbody.Init();
            }

            base.OnTrigger(trigger, triggeringEntity, parameters);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(!SoundManager.MusicLock && _arenaDoor != null && _arenaDoor.Closed && entity.GameWorld.NextScreen == null)
            {
                SoundManager.MusicLock = true;
                SoundManager.CrossFadeLoopTrack("BattleFirstLoop");
            }

            if (SoundManager.CurrentTrackName == "BattleTransition" && SoundManager.MusicLock)
                SoundManager.PrepareLoopTrack("BattleMainLoop");

            if (_arenaDoor != null && !_arenaDoor.Closed)
                SoundManager.MusicLock = false;

            if (_importantRobotEnemyCore != null && _importantRobotEnemyCore.State == EnemyState.Dying)
            {
                if (_secondImportantRobot != null && _thirdImportantRobot != null)
                {
                    _secondImportantRobot.rigidbody.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
                    _secondImportantRobot.rigidbody.Mass = 400.0f;
                    _secondImportantRobot.rigidbody.Init();
                    _thirdImportantRobot.rigidbody.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
                    _thirdImportantRobot.rigidbody.Mass = 500.0f;
                    _thirdImportantRobot.rigidbody.Init();
                    _fadingTutorials = true;
                }
            }

            if (_fadingTutorials)
            {
                if (_comboTutorialComponents != null && _guardTutorialComponents != null)
                {
                    foreach (DrawableComponent dc in _guardTutorialComponents)
                    {
                        dc.Opacity -= 1.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (dc.Opacity < 0.05f) dc.Opacity = 0.0f;
                    }

                    foreach (DrawableComponent dc in _comboTutorialComponents)
                    {
                        dc.Opacity += 1.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (dc.Opacity > 0.95)
                        {
                            dc.Opacity = 1.0f;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void OnChangeLevel()
        {
            SoundManager.MusicLock = false;
            base.OnChangeLevel();
        }

    }
}
