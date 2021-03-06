﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using NeonEngine.Components.CollisionDetection;

namespace NeonEngine.Components.Private
{
    public class Beacon : Component
    {   
        public Hitbox hitbox;
        public FarseerPhysics.Dynamics.World  PhysicWorld;

        Vector2 GroundRaycast = new Vector2();
        Vector2 GroundRaycastTarget = new Vector2();

        Vector2 LeftRaycast = new Vector2();
        Vector2 LeftRaycastTarget = new Vector2();

        Vector2 RightRaycast = new Vector2();
        Vector2 RightRaycastTarget = new Vector2();

        Vector2 RearGroundRaycast = new Vector2();
        Vector2 RearGroundRaycastTarget = new Vector2();

        Vector2 FrontGroundRaycast = new Vector2();
        Vector2 FrontGroundRaycastTarget = new Vector2();

        Vector2 LeftBottomRaycast = new Vector2();
        Vector2 LeftTopRaycast = new Vector2();
        Vector2 LeftBottomRaycastTarget = new Vector2();
        Vector2 LeftTopRaycastTarget = new Vector2();

        Vector2 RightBottomRaycast = new Vector2();
        Vector2 RightBottomRaycastTarget = new Vector2();
        Vector2 RightTopRaycast = new Vector2();
        Vector2 RightTopRaycastTarget = new Vector2();

        public Vector2 GroundOffset = Vector2.Zero;

        public Beacon(Hitbox hitbox, FarseerPhysics.Dynamics.World PhysicWorld)
            :base(hitbox.entity, "Beacon")
        {
            this.hitbox = hitbox;
            this.PhysicWorld = PhysicWorld;
            RefreshRaycastPosition();
        }

        public void RefreshRaycastPosition()
        {
            GroundRaycast = new Vector2(hitbox.Center.X - hitbox.OffsetX, hitbox.Center.Y - hitbox.OffsetY + hitbox.Height / 2 - 1);
            GroundRaycastTarget = GroundRaycast + new Vector2(0, 3) + GroundOffset;

            RearGroundRaycast = GroundRaycastTarget;
            RearGroundRaycastTarget = RearGroundRaycast + new Vector2(-hitbox.Width / 2 + 1f, 0);

            FrontGroundRaycast = GroundRaycastTarget;
            FrontGroundRaycastTarget = FrontGroundRaycast + new Vector2(hitbox.Width / 2 - 1f, 0);

            LeftRaycast = new Vector2(hitbox.Center.X - hitbox.Width / 2 - hitbox.OffsetX + 5, hitbox.Center.Y - hitbox.OffsetY);
            LeftRaycastTarget = LeftRaycast + new Vector2(-6, 0);
            LeftBottomRaycast = LeftRaycastTarget;
            LeftBottomRaycastTarget = LeftRaycastTarget + new Vector2(0, hitbox.Height / 2);
            LeftTopRaycast = LeftRaycastTarget;
            LeftTopRaycastTarget = LeftRaycastTarget + new Vector2(0, -hitbox.Height / 2);

            RightRaycast = new Vector2(hitbox.Center.X + hitbox.Width / 2 - hitbox.OffsetX - 5, hitbox.Center.Y - hitbox.OffsetY);
            RightRaycastTarget = RightRaycast + new Vector2(6, 0);
            RightBottomRaycast = RightRaycastTarget;
            RightBottomRaycastTarget = RightRaycastTarget + new Vector2(0, hitbox.Height / 2);
            RightTopRaycast = RightRaycastTarget;
            RightTopRaycastTarget = RightRaycastTarget + new Vector2(0, -hitbox.Height / 2);                     
        }

        public override void Update(GameTime gameTime)
        {
            RefreshRaycastPosition();
            base.Update(gameTime);
        }

        public Rigidbody CheckGround(Vector2 offset, Body body = null)
        {
            Rigidbody CurrentGround = null;
            bool hasHit = false;

            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body && fixture.CollisionCategories == Category.Cat1)
                {
                    Entity e = Neon.Utils.GetEntityByBody(fixture.Body);
                    CurrentGround = e != null ? e.rigidbody : null;
                    hasHit = true;
                    return 0;
                }
                return -1;
            },
            CoordinateConversion.screenToWorld(GroundRaycast),
            CoordinateConversion.screenToWorld(GroundRaycastTarget + offset));

            if (!hasHit)
            {
                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && fixture.CollisionCategories == Category.Cat1)
                    {
                        Entity e = Neon.Utils.GetEntityByBody(fixture.Body);
                        CurrentGround = e != null ? e.rigidbody : null;
                        hasHit = true;
                        return 0;
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(FrontGroundRaycast + offset),
                CoordinateConversion.screenToWorld(FrontGroundRaycastTarget + offset));

                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && fixture.CollisionCategories == Category.Cat1)
                    {
                        Entity e = Neon.Utils.GetEntityByBody(fixture.Body);
                        CurrentGround = e != null ? e.rigidbody : null;
                        hasHit = true;
                        return 0;
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(RearGroundRaycast + offset),
                CoordinateConversion.screenToWorld(RearGroundRaycastTarget + offset));
            }

            return CurrentGround;
        }

        public bool CheckLeftGround(Body body = null)
        {
            bool hasHit = false;

            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body && fixture.CollisionCategories == Category.Cat1)
                {
                    hasHit = true;
                    return 0;
                }
                return -1;
            },
            CoordinateConversion.screenToWorld(LeftRaycastTarget + new Vector2(-10, 0)),
            CoordinateConversion.screenToWorld(LeftRaycastTarget + new Vector2(-10,  hitbox.Height / 2 + 5)));

            return hasHit;
        }

        public bool CheckRightGround(Body body = null)
        {
            bool hasHit = false;

            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body && fixture.CollisionCategories == Category.Cat1)
                {
                    hasHit = true;
                    return 0;
                }
                return -1;
            },
            CoordinateConversion.screenToWorld(RightRaycastTarget + new Vector2(10, 0)),
            CoordinateConversion.screenToWorld(RightRaycastTarget + new Vector2(10, hitbox.Height / 2 + 5)));

            return hasHit;
        }

        public Entity CheckLeftSide(float OffsetX, bool searchForAll = false, Body body = null)
        {
            bool hasHit = false;
            Entity hitEntity = null;

            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                {
                    hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                    if (hitEntity != null)
                    {
                        if (hitEntity.rigidbody.OneWayPlatform)
                        {
                            hitEntity = null;
                            return -1;
                        }
                        hasHit = true;
                        return 0;
                    }
                    return -1;                   
                }
                return -1;
            },
            CoordinateConversion.screenToWorld(LeftRaycast),
            CoordinateConversion.screenToWorld(LeftRaycastTarget + new Vector2(-OffsetX, 0)));

            if (!hasHit)
            {
                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                    {
                        hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                        if (hitEntity != null)
                        {
                            if (hitEntity.rigidbody.OneWayPlatform)
                            {
                                hitEntity = null;
                                return -1;
                            }
                            hasHit = true;
                            return 0;
                        }
                        return -1;
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(LeftBottomRaycast + new Vector2(-OffsetX, 0)),
                CoordinateConversion.screenToWorld(LeftBottomRaycastTarget + new Vector2(-OffsetX, -5)));

                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                    {
                        hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                        if (hitEntity != null)
                        {
                            if (hitEntity.rigidbody.OneWayPlatform)
                            {
                                hitEntity = null;
                                return -1;
                            }
                            return 0;
                        }
                        return -1;
                        
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(LeftTopRaycast + new Vector2(-OffsetX, 0)),
                CoordinateConversion.screenToWorld(LeftTopRaycastTarget + new Vector2(-OffsetX, 0)));
            }

            return hitEntity;
        }

        public Entity CheckRightSide(float OffsetX, bool searchForAll = false, Body body = null)
        {
            bool hasHit = false;
            Entity hitEntity = null;

            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                {
                    
                    hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                    if (hitEntity != null)
                    {
                        if (hitEntity.rigidbody.OneWayPlatform)
                        {
                            hitEntity = null;
                            return -1;
                        }
                        return 0;
                    }
                    return -1;
                    
                }
                return -1;
            },
            CoordinateConversion.screenToWorld(RightRaycast),
            CoordinateConversion.screenToWorld(RightRaycastTarget + new Vector2(OffsetX, 0)));

            if (!hasHit)
            {
                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                    {
                        hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                        if (hitEntity != null)
                        {
                            if (hitEntity.rigidbody.OneWayPlatform)
                            {
                                hitEntity = null;
                                return -1;
                            }
                            return 0;
                        }
                        return -1;
                        
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(RightBottomRaycast + new Vector2(OffsetX, 0)),
                CoordinateConversion.screenToWorld(RightBottomRaycastTarget + new Vector2(OffsetX, -5)));

                PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
                {
                    if (fixture.Body != body && (searchForAll || (!searchForAll && fixture.CollisionCategories == Category.Cat1)))
                    {
                        hitEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                        if (hitEntity != null)
                        {
                            if (hitEntity.rigidbody.OneWayPlatform)
                            {
                                hitEntity = null;
                                return -1;
                            }
                            return 0;
                        }
                        return -1;
                        
                    }
                    return -1;
                },
                CoordinateConversion.screenToWorld(RightTopRaycast + new Vector2(OffsetX, 0)),
                CoordinateConversion.screenToWorld(RightTopRaycastTarget + new Vector2(OffsetX, 0)));
            }


            return hitEntity;
        }

        public bool CheckCeiling(Body body = null)
        {
            bool hasHit = false;

            return hasHit;
        }

        public bool CheckTopLeft(Body body = null)
        {
            bool hasHit = false;

            return hasHit;
        }

        public bool CheckTopRight(Body body = null)
        {
            bool hasHit = false;

            return hasHit;
        }

        public Entity Raycast(Vector2 StartPosition, Vector2 EndPosition, Body body = null)
        {
            Entity closestEntity = null;
            float closestDistanceSquared = float.MaxValue;


            PhysicWorld.RayCast((fixture, hitPosition, normal, fraction) =>
            {
                if (fixture.Body != body)
                {
                    Entity currentEntity = Neon.Utils.GetEntityByBody(fixture.Body);
                    if (currentEntity != null)
                    {
                        if (currentEntity.rigidbody.OneWayPlatform || (!currentEntity.rigidbody.IsGround && currentEntity != entity.GameWorld.Avatar))
                            return -1;

                        if (closestEntity == null)
                        {
                            closestEntity = currentEntity;
                            closestDistanceSquared = Vector2.DistanceSquared(CoordinateConversion.worldToScreen(hitPosition), StartPosition);
                        }
                        else
                        {
                            float distanceSquared = Vector2.DistanceSquared(CoordinateConversion.worldToScreen(hitPosition), StartPosition);
                            if (distanceSquared < closestDistanceSquared)
                            {
                                closestEntity = currentEntity;
                                closestDistanceSquared = distanceSquared;
                            }
                        }

                        return -1;
                    }
                    return -1;
                }
                return -1;
            },
                CoordinateConversion.screenToWorld(StartPosition),
                CoordinateConversion.screenToWorld(EndPosition));

            return closestEntity;
        }

    }
}
