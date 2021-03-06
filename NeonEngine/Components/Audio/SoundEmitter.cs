﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NeonEngine.Components.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonEngine.Components.Private
{
    public class SoundInstanceInfo
    {
        public string Name = "";
        public SoundEffect Sound;
        public float Volume = 1.0f;
        public float Pitch;
        public bool Is3DSound = true;
        public Vector2 Offset;
    }

    public class SoundEmitter : Component
    {
        #region Properties
        private bool _debug = true;

        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        protected float _zDistance = 0.0f;

        public float ZDistance
        {
            get { return _zDistance; }
            set { _zDistance = value; }
        }

        protected bool _is3DSound = true;

        public virtual bool Is3DSound
        {
            get { return _is3DSound; }
            set 
            { 
                _is3DSound = value;
            }
        }

        protected bool _isLooped = true;

        public virtual bool IsLooped
        {
            get { return _isLooped; }
            set
            { 
                _isLooped = value;
            }
        }

        protected float _volume = 1.0f;

        public virtual float Volume
        {
            get { return _volume; }
            set 
            { 
                _volume = value;
            }
        }

        protected float _pitch = 0.0f;

        public virtual float Pitch
        {
            get { return _pitch; }
            set 
            { 
                _pitch = value;
            }
        }

        protected float _maxDistance = 0.0f;

        public float MaxDistance
        {
            get { return _maxDistance; }
            set { _maxDistance = value; }
        }

        protected float _distanceVolumeTreshold = 0.3f;

        public virtual float DistanceVolumeTreshold
        {
            get { return _distanceVolumeTreshold; }
            set { _distanceVolumeTreshold = value; }
        }
        #endregion

        public AudioEmitter AudioEmitter;

        public Dictionary<SoundEffectInstance, SoundInstanceInfo> SoundInstances;
        
        protected float _currentVolume = 0.0f;

        public SoundEmitter(Entity entity)
            :base(entity, "SoundEmitter")
        {   
        }

        public override void Init()
        {
            SoundInstances = new Dictionary<SoundEffectInstance, SoundInstanceInfo>();
            
            if (AudioEmitter == null)
                AudioEmitter = new AudioEmitter();

            if (AudioEmitter != null)
            {
                AudioEmitter.Position = new Microsoft.Xna.Framework.Vector3(entity.transform.Position, _zDistance);
            }
            entity.GameWorld.AudioEmitters.Add(this);
            base.Init();
        }

        public override void Remove()
        {
            if (AudioEmitter != null)
                entity.GameWorld.AudioEmitters.Remove(this);
            base.Remove();
        }

        public override void PreUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (AudioEmitter != null && _is3DSound)
            {
                AudioEmitter.Position = new Microsoft.Xna.Framework.Vector3(entity.transform.Position, _zDistance);
            }
            if (MaxDistance != 0.0f)
            {
                foreach (AudioListener al in entity.GameWorld.AudioListeners)
                {
                    float distance = Vector2.Distance(new Vector2(al.Position.X, al.Position.Y), entity.transform.Position);
                    if (distance <= MaxDistance)
                    {
                        _currentVolume = _volume *( 1 - (distance / MaxDistance));
                        if(_currentVolume <= _distanceVolumeTreshold)
                        {
                            _currentVolume = _volume * (1 - (distance / MaxDistance * 1.1f));
                        }
                    }
                    else
                    {
                        _currentVolume = 0.0f;
                    }
                }
            }
            else
                _currentVolume = _volume;
            
            base.Update(gameTime);
        }

        public override void OnChangeLevel()
        {
            base.OnChangeLevel();
        }
    }
}
