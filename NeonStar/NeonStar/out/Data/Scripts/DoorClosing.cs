﻿using System;
using NeonEngine;
using NeonStarLibrary;
using NeonEngine.Private;
using Microsoft.Xna.Framework;

namespace NeonScripts
{
    public class DoorClosingScript : ScriptComponent
    {
        private string _triggerName = "";

        public string TriggerName
        {
            get { return _triggerName; }
            set { _triggerName = value; }
        }

        private string _closingDoorAnimationName = "";

        public string ClosingDoorAnimationName
        {
            get { return _closingDoorAnimationName; }
            set { _closingDoorAnimationName = value; }
        }

        public DoorClosingScript(Entity entity)
            :base(entity, "DoorClosingScript")
        {
        }
		
		public override void Init()
		{
			if(entity.spritesheets != null && entity.spritesheets.SpritesheetList.ContainsKey(_closingDoorAnimationName) && entity.spritesheets.SpritesheetList[_closingDoorAnimationName] != null)
				entity.spritesheets.ChangeAnimation(_closingDoorAnimationName, 0, false, true, false, entity.spritesheets.SpritesheetList[_closingDoorAnimationName].FrameCount-1);
		}

        public override void Update(GameTime gameTime)
        {
            if (entity.spritesheets.CurrentSpritesheet.currentFrame == 0)
                entity.spritesheets.CurrentSpritesheet.isPlaying = false;
        }
		
		public override void OnTrigger(Entity trigger, Entity triggeringEntity, object[] parameters = null)
		{
            if (trigger.Name == _triggerName)
            {
				if(entity.spritesheets != null && entity.spritesheets.SpritesheetList.ContainsKey(_closingDoorAnimationName) && entity.spritesheets.SpritesheetList[_closingDoorAnimationName] != null)
				{
					entity.spritesheets.ChangeAnimation(_closingDoorAnimationName, 0, true, true, true, entity.spritesheets.SpritesheetList[_closingDoorAnimationName].FrameCount - 1);
					entity.spritesheets.CurrentSpritesheet.ReverseLoop = true;
					entity.rigidbody.IsGround = true;
					entity.rigidbody.Init();
				}
            }
		}
    }
}
