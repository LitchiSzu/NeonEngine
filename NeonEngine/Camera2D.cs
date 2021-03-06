﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;
using NeonEngine.Components.Camera;

namespace NeonEngine
{
    public class Camera2D
    {
        protected float _zoom; // Camera Zoom
        Matrix _transform; // Matrix Transform
        Vector2 _pos; // Camera Position
        Vector2 _ppos; // camera Position since last frame
        protected float _rotation; // Camera Rotation
        private bool _bounded = false;
        public Vector2 BasePosition;
        public float ChaseStrengtheningRate = 0.05f;
        public bool MustStrengthen = true;

        public bool MovedLastFrame = false;
        public bool HasMetSoftBounds = false;

        public List<CameraBound> CameraBounds = new List<CameraBound>();

        public float ChaseStrength = 0.9f;

        public bool Bounded
        {
            get { return _bounded; }
            set { _bounded = value; }
        }
       
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
        }

        public Vector2 Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
            }
        }

        public Vector2 PreviousPosition
        {
            get { return _ppos; }
            set { _ppos = value; }
        }

        public Vector2 Delta
        {
            get { return _pos - _ppos; }
        }

        public Camera2D()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        public void SmoothFollow(Vector2 targetPosition)
        {
            Position -= (Position - targetPosition) / Math.Max((1250 / Vector2.Distance(Position, targetPosition)), 1);
        }

        public void SmoothFollow(Entity targetObject)
        {         
            if (Bounded && CameraBounds.Count > 0)
            {
                float NewPositionX = Position.X + (float)Math.Sqrt(Math.Pow(Position.X - targetObject.transform.Position.X, 2f));
                float NewPositionY = Position.Y + (float)Math.Sqrt(Math.Pow(Position.Y - targetObject.transform.Position.Y, 2f));

                foreach (CameraBound cb in CameraBounds)
                {
                    if (!cb.SoftBound && cb.entity.ViewedByCamera(new Vector2(NewPositionX, NewPositionY)) || cb.SoftBound && cb.BoundStrength > 0.0f)
                    {
                        switch (cb.BoundSide)
                        {
                            case Side.Up:
                                NewPositionY = MathHelper.Clamp(NewPositionY, cb.entity.transform.Position.Y + Neon.HalfScreen.Y / Zoom, float.MaxValue);
                                break;

                            case Side.Right:
                                NewPositionX =MathHelper.Clamp(NewPositionX, float.MinValue, cb.entity.transform.Position.X - Neon.HalfScreen.X / Zoom);
                                break;

                            case Side.Down:
                                NewPositionY = MathHelper.Clamp(NewPositionY, float.MinValue, cb.entity.transform.Position.Y - Neon.HalfScreen.Y / Zoom);
                                break;

                            case Side.Left:
                                NewPositionX = MathHelper.Clamp(NewPositionX, cb.entity.transform.Position.X + Neon.HalfScreen.X / Zoom, float.MaxValue);
                                break;
                        }
                    }                   
                }

                Position = new Vector2(NewPositionX, NewPositionY);
            }
            else
                Position -= (Position - targetObject.transform.Position) / Math.Max((1250 / Vector2.Distance(Position, targetObject.transform.Position)), 1);
        }


        public void Chase(Vector2 desiredPosition, Vector2 focusDisplacement, bool ignoreSoftBounds, GameTime gameTime)
        {
            _ppos = _pos;
            Vector2 OldPosition = _pos;
            if (MustStrengthen)
            {
                if (ChaseStrength < 0.9f)
                    ChaseStrength = MathHelper.Lerp(ChaseStrength, 1.0f, ChaseStrengtheningRate);
                if (ChaseStrength > 0.9f)
                    ChaseStrength = 0.9f;
            }
            Vector2 NewPosition = new Vector2(_pos.X, _pos.Y);

            _pos = new Vector2(MathHelper.Lerp(NewPosition.X, desiredPosition.X, ChaseStrength), MathHelper.Lerp(NewPosition.Y, desiredPosition.Y, ChaseStrength));
            //_pos = desiredPosition;
            
            NewPosition = _pos;

            bool boundedUp = false;
            bool boundedRight = false;
            bool boundedDown = false;
            bool boundedLeft = false;

            bool hasMetSoftBounds = false;

            if (Bounded && CameraBounds.Count > 0)
            {
                foreach (CameraBound cb in CameraBounds)
                {
                    if ((cb.entity.ViewedByCamera(NewPosition)))
                    {
                        if (cb.SoftBound)
                            hasMetSoftBounds = true;
                        switch (cb.BoundSide)
                        {
                            case Side.Up:
                                NewPosition.Y = (float)MathHelper.Lerp(NewPosition.Y, cb.entity.transform.Position.Y + Neon.HalfScreen.Y / Neon.World.Camera.Zoom, cb.BoundStrength);
                                boundedUp = true;
                                break;

                            case Side.Right:
                                NewPosition.X = (float)MathHelper.Lerp(NewPosition.X, cb.entity.transform.Position.X - Neon.HalfScreen.X / Neon.World.Camera.Zoom, cb.BoundStrength);
                                boundedRight = true;
                                break;

                            case Side.Down:
                                NewPosition.Y = (float)MathHelper.Lerp(NewPosition.Y, cb.entity.transform.Position.Y - Neon.HalfScreen.Y / Neon.World.Camera.Zoom, cb.BoundStrength);
                                boundedDown = true;
                                break;

                            case Side.Left:
                                NewPosition.X = (float)MathHelper.Lerp(NewPosition.X, cb.entity.transform.Position.X + Neon.HalfScreen.X / Neon.World.Camera.Zoom, cb.BoundStrength);
                                boundedLeft = true;
                                break;
                        }
                    }
                }          
            }

            if (hasMetSoftBounds)
                HasMetSoftBounds = true;
            else
                HasMetSoftBounds = false;

            if (Math.Abs(NewPosition.X - _pos.X) >= 1)
            {
                _pos.X = NewPosition.X;
            }
            if (Math.Abs(NewPosition.Y - _pos.Y) >= 1)
            {
                _pos.Y = NewPosition.Y;
            }

            BasePosition = _pos;

            if (focusDisplacement != Vector2.Zero)
            {
                if(!(boundedLeft && focusDisplacement.X < 0) && !(boundedRight && focusDisplacement.X > 0))
                    NewPosition.X += focusDisplacement.X;
                if(!(boundedUp && focusDisplacement.Y < 0) && !(boundedDown && focusDisplacement.Y > 0))
                    NewPosition.Y += focusDisplacement.Y;

                if (Bounded && CameraBounds.Count > 0)
                {
                    foreach (CameraBound cb in CameraBounds)
                    {
                        if ((cb.entity.ViewedByCamera(NewPosition)))
                        {
                            switch (cb.BoundSide)
                            {
                                case Side.Up:
                                    if (focusDisplacement.Y > 0)
                                        break;
                                    NewPosition.Y = (float)MathHelper.Lerp(NewPosition.Y, cb.entity.transform.Position.Y + Neon.HalfScreen.Y / Neon.World.Camera.Zoom, cb.BoundStrength);
                                    boundedUp = true;
                                    break;

                                case Side.Right:
                                    if (focusDisplacement.X < 0)
                                        break;
                                    NewPosition.X = (float)MathHelper.Lerp(NewPosition.X, cb.entity.transform.Position.X - Neon.HalfScreen.X / Neon.World.Camera.Zoom, cb.BoundStrength);
                                    boundedRight = true;
                                    break;

                                case Side.Down:
                                    if (focusDisplacement.Y < 0)
                                        break;
                                    NewPosition.Y = (float)MathHelper.Lerp(NewPosition.Y, cb.entity.transform.Position.Y - Neon.HalfScreen.Y / Neon.World.Camera.Zoom, cb.BoundStrength);
                                    boundedDown = true;
                                    break;

                                case Side.Left:
                                    if (focusDisplacement.X > 0)
                                        break;
                                    NewPosition.X = (float)MathHelper.Lerp(NewPosition.X, cb.entity.transform.Position.X + Neon.HalfScreen.X / Neon.World.Camera.Zoom, cb.BoundStrength);
                                    boundedLeft = true;
                                    break;
                            }
                        }
                    }
                }

                if (Math.Abs(NewPosition.X - _pos.X) >= 1)
                {
                    _pos.X = NewPosition.X;
                }
                if (Math.Abs(NewPosition.Y - _pos.Y) >= 1)
                {
                    _pos.Y = NewPosition.Y;
                }
            }

            

            if (_pos != OldPosition)
                MovedLastFrame = true;
            else
                MovedLastFrame = false;
            
        }

        public void Move(int x, int y)
        {
            Position += new Vector2(x, y);
        }

        public void Rotate(float addToAngle)
        {
            _rotation += addToAngle;
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }

        public Entity[] GetEntitiesInView(Vector2 cameraPosition)
        {
            return Neon.World.Entities.Where(en => en.ViewedByCamera(cameraPosition)).ToArray();
        }
    }
}
