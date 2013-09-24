﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeonEngine;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework;

namespace NeonStarEditor
{
    public partial class PrefabListControl : UserControl
    {
        public EditorScreen GameWorld;

        public PrefabListControl()
            :base()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            RefreshList();
            base.OnLoad(e);
        }

        public void RefreshList()
        {
            if (GameWorld != null)
            {
                List<string> PrefabsList;
                PrefabsList = new List<string>(Directory.GetFiles(@"../Data/Prefabs"));
                for (int i = PrefabsList.Count - 1; i >= 0; i--)
                    PrefabsList[i] = Path.GetFileNameWithoutExtension(PrefabsList[i]);
                PrefabListBox.DataSource = PrefabsList;
            }
        }

        private void AddPrefabButton_Click(object sender, EventArgs e)
        {
            string path = @"../Data/Prefabs/" + PrefabListBox.Text + ".prefab";
            DataManager.LoadPrefab(path, GameWorld);
            ActionManager.SaveAction(ActionType.AddEntity, GameWorld.entityList.Last());
        }
    }
}
