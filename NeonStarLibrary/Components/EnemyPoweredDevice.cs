﻿using NeonEngine;
using NeonEngine.Components.Graphics2D;
using NeonStarLibrary.Components.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.EnergyObjects
{
    public class EnemyPoweredDevice : EnergyDevice
    {
        #region Properties
        private bool _displayGauge = true;

        public bool DisplayGauge
        {
            get { return _displayGauge; }
            set { _displayGauge = value; }
        }

        private string _firstEnemyToKill = "";

        public string FirstEnemyToKill
        {
            get { return _firstEnemyToKill; }
            set { _firstEnemyToKill = value; }
        }

        private string _secondEnemyToKill = "";

        public string SecondEnemyToKill
        {
            get { return _secondEnemyToKill; }
            set { _secondEnemyToKill = value; }
        }

        private string _thirdEnemyToKill = "";

        public string ThirdEnemyToKill
        {
            get { return _thirdEnemyToKill; }
            set { _thirdEnemyToKill = value; }
        }

        private string _fourthEnemyToKill = "";

        public string FourthEnemyToKill
        {
            get { return _fourthEnemyToKill; }
            set { _fourthEnemyToKill = value; }
        }

        private string _fifthEnemyToKill = "";

        public string FifthEnemyToKill
        {
            get { return _fifthEnemyToKill; }
            set { _fifthEnemyToKill = value; }
        }

        private bool _addSignOnTargets = true;

        public bool AddSignOnTargets
        {
            get { return _addSignOnTargets; }
            set { _addSignOnTargets = value; }
        }

        private string _lockName = "DoorLock";

        public string LockName
        {
            get { return _lockName; }
            set { _lockName = value; }
        }

        private string _gaugeName = "Gauge";

        public string GaugeName
        {
            get { return _gaugeName; }
            set { _gaugeName = value; }
        }

        private float _gaugeMaxWidth = 100.0f;

        public float GaugeMaxWidth
        {
            get { return _gaugeMaxWidth; }
            set { _gaugeMaxWidth = value; }
        }
        #endregion

        public List<EnemyCore> _enemiesToKill;
        private TilableGraphic _gauge;
        private SpriteSheet _doorLock;


        private float _totalEnemiesToKill = 0.0f;
        private float _currentEnemiesKilled = 0.0f;
        private float _gaugeTargetWidth = 0.0f;

        public EnemyPoweredDevice(Entity entity)
            :base(entity)
        {
            Name = "EnemyPoweredDevice";
        }

        public override void Init()
        {
            if (_displayGauge)
            {
                _totalEnemiesToKill = 0.0f;
                _currentEnemiesKilled = 0.0f;
                List<TilableGraphic> tg = entity.GetComponentsByInheritance<TilableGraphic>();
                if (tg != null && tg.Count > 0)
                    tg = tg.Where(t => t.NickName == _gaugeName).ToList();
                if(tg.Count > 0)
                    _gauge = tg.First();

                if (_gauge != null)
                    _gauge.TilingWidth = 0.0f;
                
                List<SpriteSheet> ss = entity.GetComponentsByInheritance<SpriteSheet>();
                if (ss != null && ss.Count > 0)
                    ss = ss.Where(t => t.NickName == _lockName).ToList();
                if (ss.Count > 0)
                    _doorLock = ss.First();


                if (_doorLock != null)
                {
                    _doorLock.isPlaying = false;
                }

            }

            State = DeviceManager.GetDeviceState(entity.GameWorld.LevelGroupName, entity.GameWorld.LevelName, entity.Name);

            _enemiesToKill = new List<EnemyCore>();

            AddEnemyCore(_firstEnemyToKill);           
            AddEnemyCore(_secondEnemyToKill);
            AddEnemyCore(_thirdEnemyToKill);
            AddEnemyCore(_fourthEnemyToKill);
            AddEnemyCore(_fifthEnemyToKill);

            if (State == DeviceState.Activated)
            {
                if (_doorLock != null)
                    _doorLock.Active = false;
                if (_gauge != null)
                    _gauge.Active = false;
            }

            if (_addSignOnTargets)
            {
                if (this.State == DeviceState.Deactivated)
                {
                    foreach (EnemyCore ec in _enemiesToKill)
                    {
                        Graphic graphic = new Graphic(ec.entity);
                        graphic.GraphicTag = "EnemyToKillSign";
                        graphic.Layer = 0.499f;
                        graphic.HasToBeSaved = false;
                        ec.entity.AddComponent(graphic);
                        graphic.Init();
                    }
                }
            }

            if (_displayGauge)
            {
                _totalEnemiesToKill = _enemiesToKill.Count;
            }

            base.Init();
        }

        private void AddEnemyCore(string s)
        {
            Entity e = entity.GameWorld.GetEntityByName(s);
            if (e != null)
            {
                EnemyCore ec = e.GetComponent<EnemyCore>();
                if (ec != null) _enemiesToKill.Add(ec);   
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for(int i = _enemiesToKill.Count - 1; i >= 0; i--)
            {
                EnemyCore ec = _enemiesToKill[i];
                if (ec.State == EnemyState.Dead)
                {
                    _enemiesToKill.Remove(ec);
                    _currentEnemiesKilled++;
                    if (_doorLock != null)
                    {
                        _doorLock.currentFrame = 0;
                        _doorLock.isPlaying = true;
                    }

                }
            }

            if (_displayGauge)
            {
                _gaugeTargetWidth = _currentEnemiesKilled / _totalEnemiesToKill * GaugeMaxWidth;
                if (_gauge != null)
                    _gauge.TilingWidth = _gaugeTargetWidth;

                _gauge.Offset = new Microsoft.Xna.Framework.Vector2(-GaugeMaxWidth / 2 + _gaugeTargetWidth / 2, _gauge.Offset.Y);
            }
            if (_enemiesToKill.Count <= 0)
            {
                if (State == DeviceState.Deactivated)
                {
                    this.ActivateDevice();
                    if (_doorLock != null)
                    {
                        _doorLock.Active = false;
                    }
                }
            }
            base.Update(gameTime);
        }
    }
}
