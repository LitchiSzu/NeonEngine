﻿using NeonEngine;
using NeonStarLibrary.Components.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonStarLibrary.Components.EnergyObjects
{
    public abstract class EnergyDevice : Component
    {
        #region Properties

        protected DeviceState _state = DeviceState.Deactivated;

        public DeviceState State
        {
            get { return _state; }
            set 
            { 
                _state = value;
            }
        }

        #endregion      

        public EnergyDevice(Entity entity)
            :base(entity, "EnergyDevice")
        {
        }

        public virtual void ActivateDevice()
        {
            _state = DeviceState.Activated;
            DeviceManager.SetDeviceState(entity.GameWorld.LevelGroupName, entity.GameWorld.LevelName, entity.Name, State);
        }

        public virtual void DeactivateDevice()
        {
            _state = DeviceState.Deactivated;
            DeviceManager.SetDeviceState(entity.GameWorld.LevelGroupName, entity.GameWorld.LevelName, entity.Name, State);
        }
    }
}
