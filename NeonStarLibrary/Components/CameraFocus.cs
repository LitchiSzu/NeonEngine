﻿using Microsoft.Xna.Framework;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.Camera
{
    public class CameraFocus : Component
    {
        #region Properties
        private bool _eightDirectionsMove = false;

        public bool EightDirectionsMove
        {
            get { return _eightDirectionsMove; }
            set { _eightDirectionsMove = value; }
        }

        private float _distanceX = 350.0f;

        public float DistanceX
        {
            get { return _distanceX; }
            set { _distanceX = value; }
        }

        private float _distanceY = 350.0f;

        public float DistanceY
        {
            get { return _distanceY; }
            set { _distanceY = value; }
        }

        private float _smoothingRate = 0.05f;

        public float SmoothingRate
        {
            get { return _smoothingRate; }
            set { _smoothingRate = value; }
        }
        #endregion

        public Vector2 FocusDisplacement;
        public bool IgnoreSoftBounds = false;
        private Vector2 _focusDisplacement = Vector2.Zero;
        private Vector2 _targetFocusDisplacement;

        public CameraFocus(Entity entity)
            :base(entity, "CameraFocus")
        {     
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Neon.Input.Check(NeonStarInput.CameraRight))
            {
                if (_eightDirectionsMove)
                {
                    if (Neon.Input.Check(NeonStarInput.CameraDown))
                    {
                        _targetFocusDisplacement = new Vector2(_distanceX, _distanceY);
                    }
                    else if (Neon.Input.Check(NeonStarInput.CameraUp))
                    {
                        _targetFocusDisplacement = new Vector2(_distanceX, -_distanceX);
                    }
                    else
                    {
                        _targetFocusDisplacement = new Vector2(_distanceX, 0);
                    }
                }
                else
                    _targetFocusDisplacement = new Vector2(_distanceX, 0);

                IgnoreSoftBounds = true;
                
            }
            else if (Neon.Input.Check(NeonStarInput.CameraLeft))
            {
                if (_eightDirectionsMove)
                {
                    if (Neon.Input.Check(NeonStarInput.CameraDown))
                    {
                        _targetFocusDisplacement = new Vector2(-_distanceX, _distanceY);
                    }
                    else if (Neon.Input.Check(NeonStarInput.CameraUp))
                    {
                        _targetFocusDisplacement = new Vector2(-_distanceX, -_distanceY);
                    }
                    else
                    {
                        _targetFocusDisplacement = new Vector2(-_distanceX, 0);
                    }
                }
                else
                    _targetFocusDisplacement = new Vector2(-_distanceX, 0);

                IgnoreSoftBounds = true;
                
            }
            else if (Neon.Input.Check(NeonStarInput.CameraDown))
            {
                _targetFocusDisplacement = new Vector2(0, _distanceY);
                IgnoreSoftBounds = true;
            }
            else if(Neon.Input.Check(NeonStarInput.CameraUp))
            {
                _targetFocusDisplacement = new Vector2(0, -_distanceY);
                IgnoreSoftBounds = true;
            }
            else
            {
                _targetFocusDisplacement = Vector2.Zero;
                IgnoreSoftBounds = false;
            }

            _focusDisplacement = Vector2.Lerp(_focusDisplacement, _targetFocusDisplacement, _smoothingRate);
            if (Vector2.Distance(_focusDisplacement, _targetFocusDisplacement) < 1.0f)
                _focusDisplacement = _targetFocusDisplacement;
            
            FocusDisplacement = _focusDisplacement;

            base.Update(gameTime);
        }
    }
}
