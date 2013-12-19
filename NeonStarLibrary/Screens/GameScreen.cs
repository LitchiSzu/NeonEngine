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

namespace NeonStarLibrary
{
    public class GameScreen : World
    {
        public List<Enemy> enemies;
        public List<Attack> FreeAttacks = new List<Attack>();
        public List<Entity> Bullets = new List<Entity>();

        public NeonPool<Entity> BulletsPool;
        public bool MustFollowAvatar = true;

        public Entity entityToChase;

        public GameScreen(string levelFile, Game game)
            : base(game)
        {
            enemies = new List<Enemy>();
            
            BulletsPool = new NeonPool<Entity>(() => new Entity(this));

            BulletsManager.LoadBullets();
            AttacksManager.LoadAttacks();          

            if(levelFile != "")
                LoadLevel(new Level(levelFile, this, true));

            entityToChase = Neon.world.GetEntityByName("LiOn");

            camera.Bounded = true;
        } 

        public override void PreUpdate(GameTime gameTime)
        {
            base.PreUpdate(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Pause)
            {
                if (MustFollowAvatar && entityToChase != null)
                    camera.Chase(entityToChase.transform.Position, gameTime);
                else if (entityToChase == null)
                {
                    entityToChase = Neon.world.GetEntityByName("LiOn");
                }
                for (int i = FreeAttacks.Count - 1; i >= 0; i--)
                {
                    Attack attack = FreeAttacks[i];
                    attack.Update(gameTime);
                    if (attack.CooldownFinished)
                    {
                        FreeAttacks.Remove(attack);
                        attack._entity.Destroy();
                        attack.CancelAttack();
                    }
                }

                for (int i = Bullets.Count - 1; i >= 0; i--)
                    Bullets[i].Update(gameTime);             
            }

            if (Neon.Input.Pressed(Buttons.Start))
                Pause = !Pause;

            base.Update(gameTime);
        }

        public virtual void ReloadLevel()
        {
            ChangeScreen(new GameScreen( this.levelFilePath, game));
        }

        public override void ManualDrawBackHUD(SpriteBatch sb)
        {
            base.ManualDrawBackHUD(sb);
        }
    }
}
