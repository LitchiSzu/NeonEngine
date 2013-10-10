﻿using FarseerPhysics.Dynamics;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public class Enemy : Component
    {
        private bool _debug;
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }
        private float _startingHealthPoints = 10.0f;
        public float StartingHealthPoints
        {
            get { return _startingHealthPoints; }
            set { _startingHealthPoints = value; }
        }

        private float _currentHealthPoints;

        public Enemy(Entity entity)
            :base(entity, "Enemy")
        {
        }

        public override void Init()
        {
            _currentHealthPoints = _startingHealthPoints;
            base.Init();
        }

        public void ChangeHealthPoints(float value)
        {
            _currentHealthPoints += value;
            if (Debug)
            {
                Console.WriteLine(entity.Name + " have lost " + value + " HP(s) -> Now at " + _currentHealthPoints + " HP(s).");
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
