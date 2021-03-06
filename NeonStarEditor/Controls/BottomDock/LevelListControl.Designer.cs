﻿namespace NeonStarEditor.Controls.BottomDock
{
    partial class LevelListControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelListControl));
            this.EntityList = new System.Windows.Forms.Label();
            this.levelListTreeView = new System.Windows.Forms.TreeView();
            this.LoadLevel = new System.Windows.Forms.Button();
            this.AddEntityButton = new System.Windows.Forms.Button();
            this.newFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.levelNameText = new System.Windows.Forms.TextBox();
            this.groupNameText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.loadSpawnPoint = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DefaultLayerBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadSpawnPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // EntityList
            // 
            this.EntityList.AutoSize = true;
            this.EntityList.Font = new System.Drawing.Font("Agency FB", 15.5F);
            this.EntityList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.EntityList.Location = new System.Drawing.Point(92, 5);
            this.EntityList.Name = "EntityList";
            this.EntityList.Size = new System.Drawing.Size(64, 25);
            this.EntityList.TabIndex = 3;
            this.EntityList.Text = "Level List";
            // 
            // levelListTreeView
            // 
            this.levelListTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.levelListTreeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.levelListTreeView.Location = new System.Drawing.Point(12, 37);
            this.levelListTreeView.Name = "levelListTreeView";
            this.levelListTreeView.Size = new System.Drawing.Size(216, 116);
            this.levelListTreeView.TabIndex = 4;
            this.levelListTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.levelListTreeView_AfterSelect);
            // 
            // LoadLevel
            // 
            this.LoadLevel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LoadLevel.BackgroundImage")));
            this.LoadLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LoadLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.LoadLevel.Location = new System.Drawing.Point(393, 47);
            this.LoadLevel.Name = "LoadLevel";
            this.LoadLevel.Size = new System.Drawing.Size(30, 30);
            this.LoadLevel.TabIndex = 5;
            this.LoadLevel.UseVisualStyleBackColor = true;
            this.LoadLevel.Click += new System.EventHandler(this.LoadLevel_Click);
            // 
            // AddEntityButton
            // 
            this.AddEntityButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.AddEntityButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddEntityButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.AddEntityButton.Location = new System.Drawing.Point(393, 117);
            this.AddEntityButton.Name = "AddEntityButton";
            this.AddEntityButton.Size = new System.Drawing.Size(30, 30);
            this.AddEntityButton.TabIndex = 6;
            this.AddEntityButton.Text = "N";
            this.AddEntityButton.UseVisualStyleBackColor = true;
            this.AddEntityButton.Click += new System.EventHandler(this.AddEntityButton_Click);
            // 
            // newFolder
            // 
            this.newFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("newFolder.BackgroundImage")));
            this.newFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.newFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.newFolder.Location = new System.Drawing.Point(393, 81);
            this.newFolder.Name = "newFolder";
            this.newFolder.Size = new System.Drawing.Size(30, 30);
            this.newFolder.TabIndex = 6;
            this.newFolder.UseVisualStyleBackColor = true;
            this.newFolder.Click += new System.EventHandler(this.newFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.label2.Location = new System.Drawing.Point(6, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Group Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.label1.Location = new System.Drawing.Point(6, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Level Name";
            // 
            // levelNameText
            // 
            this.levelNameText.Location = new System.Drawing.Point(9, 80);
            this.levelNameText.Name = "levelNameText";
            this.levelNameText.Size = new System.Drawing.Size(133, 20);
            this.levelNameText.TabIndex = 7;
            this.levelNameText.Enter += new System.EventHandler(this.levelNameText_Enter);
            this.levelNameText.Leave += new System.EventHandler(this.levelNameText_Leave);
            // 
            // groupNameText
            // 
            this.groupNameText.Location = new System.Drawing.Point(9, 33);
            this.groupNameText.Name = "groupNameText";
            this.groupNameText.Size = new System.Drawing.Size(133, 20);
            this.groupNameText.TabIndex = 8;
            this.groupNameText.Enter += new System.EventHandler(this.levelNameText_Enter);
            this.groupNameText.Leave += new System.EventHandler(this.levelNameText_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.groupNameText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.levelNameText);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.groupBox1.Location = new System.Drawing.Point(234, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(153, 116);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Level Info";
            // 
            // loadSpawnPoint
            // 
            this.loadSpawnPoint.Location = new System.Drawing.Point(432, 56);
            this.loadSpawnPoint.Name = "loadSpawnPoint";
            this.loadSpawnPoint.Size = new System.Drawing.Size(39, 20);
            this.loadSpawnPoint.TabIndex = 12;
            this.loadSpawnPoint.Enter += new System.EventHandler(this.loadSpawnPoint_Enter);
            this.loadSpawnPoint.Leave += new System.EventHandler(this.loadSpawnPoint_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.label3.Location = new System.Drawing.Point(429, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "SpawnPoint";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.label4.Location = new System.Drawing.Point(239, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Default Layer";
            // 
            // DefaultLayerBox
            // 
            this.DefaultLayerBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DefaultLayerBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DefaultLayerBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.DefaultLayerBox.Location = new System.Drawing.Point(325, 10);
            this.DefaultLayerBox.Name = "DefaultLayerBox";
            this.DefaultLayerBox.Size = new System.Drawing.Size(117, 20);
            this.DefaultLayerBox.TabIndex = 8;
            this.DefaultLayerBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DefaultLayerBox.TextChanged += new System.EventHandler(this.DefaultLayerBox_TextChanged);
            this.DefaultLayerBox.Enter += new System.EventHandler(this.DefaultLayerBox_Enter);
            // 
            // LevelListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DefaultLayerBox);
            this.Controls.Add(this.loadSpawnPoint);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.newFolder);
            this.Controls.Add(this.AddEntityButton);
            this.Controls.Add(this.LoadLevel);
            this.Controls.Add(this.levelListTreeView);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.EntityList);
            this.Name = "LevelListControl";
            this.Size = new System.Drawing.Size(505, 165);
            this.Load += new System.EventHandler(this.LevelListControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadSpawnPoint)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label EntityList;
        private System.Windows.Forms.TreeView levelListTreeView;
        private System.Windows.Forms.Button LoadLevel;
        private System.Windows.Forms.Button AddEntityButton;
        private System.Windows.Forms.Button newFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox levelNameText;
        private System.Windows.Forms.TextBox groupNameText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown loadSpawnPoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox DefaultLayerBox;
    }
}
