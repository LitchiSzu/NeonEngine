﻿using Microsoft.Xna.Framework;
using NeonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary
{
    public class ThreatArea : Component
    {
        public Enemy EnemyComponent;
        public Entity EntityFollowed;
        public bool ShouldDetectAgain = true;

        private float _threatRange = 300f;

        public float ThreatRange
        {
            get { return _threatRange; }
            set { _threatRange = value; }
        }

        private string _entityToSearchFor= "";

        public string EntityToSearchFor
        {
            get { return _entityToSearchFor; }
            set { _entityToSearchFor = value; }
        }

        public ThreatArea(Entity entity)
            :base(entity, "ThreatArea")
        {
        }

        public override void Init()
        {
            EnemyComponent = entity.GetComponent<Enemy>();
            if (EnemyComponent != null)
                EnemyComponent._threatArea = this;
            base.Init();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (EntityFollowed != null)
            {
                if (EnemyComponent.State == EnemyState.Patrol && ShouldDetectAgain || EnemyComponent.State == EnemyState.Chase)
                {
                    if (Vector2.DistanceSquared(this.entity.transform.Position, EntityFollowed.transform.Position) < ThreatRange * ThreatRange)
                    {
                        EnemyComponent.State = EnemyState.Chase;
                    }
                    else if (EnemyComponent.State == EnemyState.Chase)
                    {
                        EnemyComponent.State = EnemyState.FinishChase;
                    }
                }
                else
                {
                    Rigidbody rg = EntityFollowed.rigidbody.beacon.CheckGround();
                    Rigidbody rg2 = this.entity.rigidbody.beacon.CheckGround();
                    if (rg != null && rg2 != null && rg == rg2)
                    {
                        ShouldDetectAgain = true;
                    }
                }
            }   
            else
            {
                foreach (Entity ent in entity.containerWorld.entities.Where(e => e.Name == _entityToSearchFor))
                {
                    if (Vector2.DistanceSquared(ent.transform.Position, entity.transform.Position) < ThreatRange * ThreatRange)
                    {
                        EntityFollowed = ent;
                        EnemyComponent.State = EnemyState.Chase;
                    }
                }
            }
           
            base.Update(gameTime);
        }
    }
}
