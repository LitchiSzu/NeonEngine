﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeonEngine;
using NeonStarLibrary;
using System.Xml.Linq;
using System.Globalization;
using Microsoft.Xna.Framework;
using NeonStarLibrary.Components.Avatar;

namespace NeonStarEditor
{
    public partial class AttacksSettingsManager : UserControl
    {
        AttackInfo CurrentAttackSelected;

        AttackEffect CurrentAttackEffectSelected = null;
        List<AttackInfo> _completeAttackList;

        public AttacksSettingsManager()
        {
            InitializeComponent();
            this.Location = new System.Drawing.Point(Neon.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - this.Width / 2, Neon.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - this.Height / 2);
            foreach (Enum e in Enum.GetValues(typeof(AttackType)))
                TypeComboBox.Items.Add(e);

            foreach (Enum e in Enum.GetValues(typeof(Element)))
                ElementCombobox.Items.Add(e);
        }

        public void InitializeData(bool fromGame = true)
        {
            _completeAttackList = new List<AttackInfo>();
            AttacksList.Nodes.Clear();
            AttacksManager._attacksInformation = AttacksManager._attacksInformation.OrderBy(a => a.Name).ToList();
            foreach (AttackInfo ai in AttacksManager._attacksInformation)
            {
                if (!AttacksList.Nodes.ContainsKey(ai.GroupName))
                {
                    TreeNode groupTn = new TreeNode(ai.GroupName);
                    groupTn.Name = ai.GroupName;
                    AttacksList.Nodes.Add(groupTn);
                }
                    
    
                TreeNode tn = new TreeNode(ai.Name);
                tn.Tag = ai;
                
                AttacksList.Nodes[ai.GroupName].Nodes.Add(tn);
                _completeAttackList.Add(ai);
                if (CurrentAttackSelected != null && CurrentAttackSelected == ai)
                    AttacksList.SelectedNode = tn;
            }   
       
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (CurrentAttackSelected != null)
            {
                AttacksManager._attacksInformation.Remove(CurrentAttackSelected);
                InitializeData(false);
            }
        }

        private void AddNew_Click(object sender, EventArgs e)
        {
            int i = 1;
            while(_completeAttackList.Where(a => a.Name == "Attack"+i).Count() > 0)
                i++;

            AttackInfo ai = new AttackInfo();
            ai.Name = "Attack"+i;
            ai.GroupName = "NoGroup";

            AttacksManager._attacksInformation.Add(ai);
            CurrentAttackSelected = ai;
            InitializeData(false);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement xnaContent = new XElement("XnaContent");
            XElement attacks = new XElement("Attacks");

            foreach (AttackInfo ai in _completeAttackList)
            {
                XElement attack = new XElement("Attack", new XAttribute("ID", i), 
                                                new XAttribute("Name", ai.Name),
                                                new XAttribute("Group", ai.GroupName),
                                                new XAttribute("Type", ai.Type.ToString()));

                XElement hitboxes = new XElement("Hitboxes");
                foreach(Microsoft.Xna.Framework.Rectangle rectangle in ai.Hitboxes)
                {
                    XElement hitbox = new XElement("Hitbox", new XAttribute("OffsetX", rectangle.X),
                                                    new XAttribute("OffsetY", rectangle.Y),
                                                    new XAttribute("Width", rectangle.Width),
                                                    new XAttribute("Height", rectangle.Height));
                    hitboxes.Add(hitbox);
                }

                attack.Add(hitboxes);

                XElement element = new XElement("AttackElement", ai.AttackElement.ToString());
                attack.Add(element);

                XElement damageOnHit = new XElement("DamageOnHit", ai.DamageOnHit.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(damageOnHit);

                XElement airOnly = new XElement("AirOnly", ai.AirOnly.ToString());
                attack.Add(airOnly);

                XElement cancelOnGround = new XElement("GroundCancel", ai.CancelOnGround.ToString());
                attack.Add(cancelOnGround);

                XElement onlyOnceInAir = new XElement("OnlyOnceInAir", ai.OnlyOnceInAir.ToString());
                attack.Add(onlyOnceInAir);

                XElement delay = new XElement("Delay", ai.Delay.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(delay);

                XElement cooldown = new XElement("Cooldown", ai.Cooldown.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(cooldown);

                XElement localCooldown = new XElement("LocalCooldown", ai.LocalCooldown.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(localCooldown);

                XElement duration = new XElement("Duration", ai.Duration.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(duration);

                XElement airLock = new XElement("AirLock", ai.AirLock.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(airLock);

                XElement targetAirLock = new XElement("TargetAirLock", ai.TargetAirLock.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(targetAirLock);

                XElement airFactor = new XElement("AirFactor", ai.AirFactor.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(airFactor);

                XElement stunLock = new XElement("StunLock", ai.StunLock.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(stunLock);

                XElement multiHitDelay = new XElement("MultiHitDelay", ai.MultiHitDelay.ToString("G", CultureInfo.InvariantCulture));
                attack.Add(multiHitDelay);

                XElement airImpulse = new XElement("AirImpulse", Neon.Utils.Vector2ToString(ai.AirImpulse));
                attack.Add(airImpulse);

                XElement alwaysStunlock = new XElement("AlwaysStunlock", ai.AlwaysStunlock.ToString());
                attack.Add(alwaysStunlock);

                XElement onDelaySpecialEffects = new XElement("OnDelaySpecialEffects");
                foreach (AttackEffect effect in ai.OnDelaySpecialEffects)
                {
                    onDelaySpecialEffects.Add(CreateEffectText(effect));
                }
                attack.Add(onDelaySpecialEffects);

                XElement onDurationSpecialEffects = new XElement("OnDurationSpecialEffects");
                foreach (AttackEffect effect in ai.OnDurationSpecialEffects)
                {
                    onDurationSpecialEffects.Add(CreateEffectText(effect));
                }
                attack.Add(onDurationSpecialEffects);

                XElement onHitSpecialEffects = new XElement("OnHitSpecialEffects");
                foreach (AttackEffect effect in ai.OnHitSpecialEffects)
                {
                    onHitSpecialEffects.Add(CreateEffectText(effect));
                }
                attack.Add(onHitSpecialEffects);

                XElement onGroundCancelSpecialEffects = new XElement("OnGroundCancelSpecialEffects");
                foreach (AttackEffect effect in ai.OnGroundCancelSpecialEffects)
                {
                    onGroundCancelSpecialEffects.Add(CreateEffectText(effect));
                }
                attack.Add(onGroundCancelSpecialEffects);

                XElement onFinishSpecialEffects = new XElement("OnFinishSpecialEffects");
                foreach (AttackEffect effect in ai.OnFinishSpecialEffects)
                {
                    onFinishSpecialEffects.Add(CreateEffectText(effect));
                }
                attack.Add(onFinishSpecialEffects);

                attacks.Add(attack);
            }
            xnaContent.Add(attacks);
            document.Add(xnaContent);

            document.Save(@"../Data/Config/Attacks.xml");

            AttacksManager.LoadAttacks();

        }

        private XElement CreateEffectText(AttackEffect effectKvp)
        {
            XElement effect = new XElement("Effect", new XAttribute("Type", effectKvp.specialEffect.ToString()));

            switch(effectKvp.specialEffect)
            {
                case SpecialEffect.Impulse:
                    Vector2 impulseValue = (Vector2)effectKvp.Parameters[0];

                    XElement parameter = new XElement("Parameter", new XAttribute("Value", "{X:" + impulseValue.X.ToString("G", CultureInfo.InvariantCulture) + " Y:" + impulseValue.Y.ToString("G", CultureInfo.InvariantCulture) + "}"));
                    XElement boolPulse = new XElement("SecondParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[1]).ToString()));
                    XElement inAirBool = new XElement("ThirdParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[2]).ToString()));
                    effect.Add(parameter);
                    effect.Add(boolPulse);
                    effect.Add(inAirBool);
                    break;

                case SpecialEffect.PercentageDamageBoost:
                    XElement percentage = new XElement("Parameter", new XAttribute("Value", ((float)effectKvp.Parameters[0]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement duration = new XElement("SecondParameter", new XAttribute("Value", ((float)effectKvp.Parameters[1]).ToString("G", CultureInfo.InvariantCulture)));
                    effect.Add(percentage);
                    effect.Add(duration);
                    break;

                case SpecialEffect.PositionalPulse:
                    Vector2 pulseValue = (Vector2)effectKvp.Parameters[0];
                    XElement parameterPulse = new XElement("Parameter", new XAttribute("Value", "{X:" + pulseValue.X.ToString("G", CultureInfo.InvariantCulture) + " Y:" + pulseValue.Y.ToString("G", CultureInfo.InvariantCulture) + "}"));

                    effect.Add(parameterPulse);
                    break;

                case SpecialEffect.StartAttack:
                    string attackValue = (string)effectKvp.Parameters[0];
                    XElement parameterString = new XElement("Parameter", new XAttribute("Value", attackValue));
                    XElement linkedToLauncher = new XElement("SecondParameter", new XAttribute("Value", (bool)effectKvp.Parameters[1]));
                    XElement attackDelay = new XElement("ThirdParameter", new XAttribute("Value", (float)effectKvp.Parameters[2]));
                    effect.Add(parameterString);
                    effect.Add(linkedToLauncher);
                    effect.Add(attackDelay);
                    break;

                case SpecialEffect.ShootBullet:
                    string bulletName = (string)(effectKvp.Parameters[0] as BulletInfo).Name;
                    XElement parameterBullet = new XElement("Parameter", new XAttribute("Value", bulletName));
                    XElement offsetBullet = new XElement("SecondParameter", new XAttribute("Value", Neon.Utils.Vector2ToString((Vector2)(effectKvp.Parameters[1]))));
                    effect.Add(parameterBullet);
                    effect.Add(offsetBullet);
                    break;

                case SpecialEffect.ShootBulletAtTarget:
                    string bulletName2 = (string)(effectKvp.Parameters[0] as BulletInfo).Name;
                    XElement parameterBullet2 = new XElement("Parameter", new XAttribute("Value", bulletName2));
                    XElement offsetBullet2 = new XElement("SecondParameter", new XAttribute("Value", Neon.Utils.Vector2ToString((Vector2)(effectKvp.Parameters[1]))));
                    effect.Add(parameterBullet2);
                    effect.Add(offsetBullet2);
                    break;

                case SpecialEffect.Invincible:
                    XElement parameterInvincibility = new XElement("Parameter", new XAttribute("Value", ((float)effectKvp.Parameters[0]).ToString("G", CultureInfo.InvariantCulture)));
                    effect.Add(parameterInvincibility);
                    break;

                case SpecialEffect.EffectAnimation:
                    if (effectKvp.Parameters[0] == null)
                        return null;
                    XElement parameterAnimation = new XElement("Parameter", new XAttribute("Value", AssetManager.GetSpritesheetTag((effectKvp.Parameters[0] as SpriteSheetInfo))));
                    XElement parameterRotation = new XElement("SecondParameter", new XAttribute("Value", ((float)effectKvp.Parameters[1]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterOffset = new XElement("ThirdParameter", new XAttribute("Value", Neon.Utils.Vector2ToString(((Vector2)effectKvp.Parameters[2]))));
                    XElement parameterFollow = new XElement("FourthParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[3]).ToString()));
                    XElement parameterScale = new XElement("FifthParameter", new XAttribute("Value", ((float)effectKvp.Parameters[4]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterDelay = new XElement("SixthParameter", new XAttribute("Value", ((float)effectKvp.Parameters[5]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterLayer = new XElement("SeventhParameter", new XAttribute("Value", ((float)effectKvp.Parameters[6]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterLoopAnimation = new XElement("EighthParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[7]).ToString()));
                    XElement parameterStopWithAttack = new XElement("NinthParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[8]).ToString()));
                    effect.Add(parameterAnimation);
                    effect.Add(parameterRotation);
                    effect.Add(parameterOffset);
                    effect.Add(parameterFollow);
                    effect.Add(parameterScale);
                    effect.Add(parameterDelay);
                    effect.Add(parameterLayer);
                    effect.Add(parameterLoopAnimation);
                    effect.Add(parameterStopWithAttack);
                    break;

                case SpecialEffect.MoveWhileAttacking:
                    XElement parameterChase = new XElement("Parameter", new XAttribute("Value", ((float)(effectKvp.Parameters[0])).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterStop = new XElement("SecondParameter", new XAttribute("Value", ((bool)(effectKvp.Parameters[1])).ToString()));
                    effect.Add(parameterChase);
                    effect.Add(parameterStop);
                    
                    break;

                case SpecialEffect.DamageOverTime:
                    XElement parameterDuration = new XElement("Parameter", new XAttribute("Value", ((float)effectKvp.Parameters[0]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterDamage = new XElement("SecondParameter", new XAttribute("Value", ((float)effectKvp.Parameters[1]).ToString("G", CultureInfo.InvariantCulture)));
                    XElement parameterTick = new XElement("ThirdParameter", new XAttribute("Value", ((float)effectKvp.Parameters[2]).ToString("G", CultureInfo.InvariantCulture)));
                    effect.Add(parameterDuration);
                    effect.Add(parameterDamage);
                    effect.Add(parameterTick);
                    break;                
                    
                case SpecialEffect.InstantiatePrefab:
                    XElement parameterPrefabName = new XElement("Parameter", new XAttribute("Value", effectKvp.Parameters[0]));
                    XElement parameterImpulsePrefab = new XElement("SecondParameter", new XAttribute("Value", Neon.Utils.Vector2ToString((Vector2)effectKvp.Parameters[1])));
                    XElement parameterOffsetPrefab = new XElement("ThirdParameter", new XAttribute("Value", Neon.Utils.Vector2ToString((Vector2)effectKvp.Parameters[2])));
                    effect.Add(parameterPrefabName);
                    effect.Add(parameterImpulsePrefab);
                    effect.Add(parameterOffsetPrefab);
                    break;

                case SpecialEffect.PlaySound:
                    XElement parameterSound = new XElement("Parameter", new XAttribute("Value", effectKvp.Parameters[0]));
                    XElement parameterVolume = new XElement("SecondParameter", new XAttribute("Value", ((float)effectKvp.Parameters[1]).ToString()));
                    XElement parameterStopSound = new XElement("ThirdParameter", new XAttribute("Value", ((bool)effectKvp.Parameters[2]).ToString()));
                    effect.Add(parameterSound);
                    effect.Add(parameterVolume);
                    effect.Add(parameterStopSound);
                    break;
            }

            return effect;
        }

        private void ClosePanel_Click(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).ToggleAttackManager();
        }

        private void InitEffectData()
        {
            for (int i = EffectsInfoPanel.Controls.Count - 1; i >= 0; i--)
                EffectsInfoPanel.Controls[i].Dispose();

            if (CurrentAttackEffectSelected != null)
            {
                Label label = new Label();
                label.Text = "Effect Type";
                label.Height = 15;
                label.Location = new System.Drawing.Point(5, 10);
                this.EffectsInfoPanel.Controls.Add(label);

                ComboBox comboBox = new ComboBox();
                foreach (Enum e in Enum.GetValues(typeof(SpecialEffect)))
                    comboBox.Items.Add(e);
                comboBox.SelectedItem = CurrentAttackEffectSelected.specialEffect;
                comboBox.Location = new System.Drawing.Point(5, 10 + label.Height + 5);
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.SelectedValueChanged += comboBox_SelectedValueChanged;
                this.EffectsInfoPanel.Controls.Add(comboBox);

                switch(CurrentAttackEffectSelected.specialEffect)
                {
                    case SpecialEffect.Impulse:

                        label = new Label();
                        label.Text = "Impulse Power";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown impulsePower = new NumericUpDown();
                        impulsePower.Name = "ImpulsePowerX";
                        impulsePower.Maximum = 50000;
                        impulsePower.Minimum = -50000;
                        impulsePower.Width = 80;
                        impulsePower.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[0]).X;
                        impulsePower.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        impulsePower.Enter += Numeric_Enter;
                        impulsePower.Leave += Numeric_Leave;
                        impulsePower.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(impulsePower);

                        impulsePower = new NumericUpDown();
                        impulsePower.Name = "ImpulsePowerY";
                        impulsePower.Maximum = 50000;
                        impulsePower.Minimum = -50000;
                        impulsePower.Width = 80;
                        impulsePower.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[0]).Y;
                        impulsePower.Location = new System.Drawing.Point(impulsePower.Width + 10, label.Location.Y + label.Height + 5);
                        impulsePower.Enter += Numeric_Enter;
                        impulsePower.Leave += Numeric_Leave;
                        impulsePower.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(impulsePower);

                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = "Must stop on target";
                        checkBox.Name = "TargetStop";
                        checkBox.Checked = (bool)CurrentAttackEffectSelected.Parameters[1];
                        checkBox.Location = new System.Drawing.Point(label.Location.X, impulsePower.Location.Y + impulsePower.Height + 5);
                        checkBox.CheckedChanged += checkBox_CheckedChanged;
                        EffectsInfoPanel.Controls.Add(checkBox);

                        CheckBox checkBox2 = new CheckBox();
                        checkBox2.Text = "Not in air";
                        checkBox2.Name = "NotInAirCheckbox";
                        checkBox2.Checked = (bool)CurrentAttackEffectSelected.Parameters[2];
                        checkBox2.Location = new System.Drawing.Point(label.Location.X, checkBox.Location.Y + checkBox.Height + 5);
                        checkBox2.CheckedChanged += checkBox_CheckedChanged;
                        EffectsInfoPanel.Controls.Add(checkBox2);

                        break;

                    case SpecialEffect.PositionalPulse:
                        label = new Label();
                        label.Text = "Pulse Power";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown pulsePower = new NumericUpDown();
                        pulsePower.Name = "ImpulsePowerX";
                        pulsePower.Maximum = 50000;
                        pulsePower.Minimum = -50000;
                        pulsePower.Width = 80;
                        pulsePower.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[0]).X;
                        pulsePower.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        pulsePower.Enter += Numeric_Enter;
                        pulsePower.Leave += Numeric_Leave;
                        pulsePower.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(pulsePower);

                        pulsePower = new NumericUpDown();
                        pulsePower.Name = "ImpulsePowerY";
                        pulsePower.Maximum = 50000;
                        pulsePower.Minimum = -50000;
                        pulsePower.Width = 80;
                        pulsePower.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[0]).Y;
                        pulsePower.Location = new System.Drawing.Point(pulsePower.Width + 10, label.Location.Y + label.Height + 5);
                        pulsePower.Enter += Numeric_Enter;
                        pulsePower.Leave += Numeric_Leave;
                        pulsePower.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(pulsePower);
                        break;

                    case SpecialEffect.StartAttack:
                        label = new Label();
                        label.Text = "Attack to launch";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        TextBox textBox = new TextBox();
                        textBox.Name = "AttackName";
                        textBox.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        textBox.Width = 150;
                        textBox.Enter += textBox_Enter;
                        textBox.Leave += textBox_Leave;
                        textBox.Text = (string)CurrentAttackEffectSelected.Parameters[0];
                        this.EffectsInfoPanel.Controls.Add(textBox);

                        label = new Label();
                        label.Text = "Attached to Launcher";
                        label.Height = 15;
                        label.Width = 200;
                        label.Location = new System.Drawing.Point(5, textBox.Location.Y + textBox.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        CheckBox checkbox3 = new CheckBox();
                        checkbox3.Name = "AttachedToLauncher";
                        checkbox3.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        checkbox3.CheckedChanged += checkbox3_CheckedChanged;
                        checkbox3.Checked = (bool)CurrentAttackEffectSelected.Parameters[1];
                        this.EffectsInfoPanel.Controls.Add(checkbox3);

                        label = new Label();
                        label.Text = "Delay";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(checkbox3.Location.X, checkbox3.Location.Y + checkbox3.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown duration = new NumericUpDown();
                        duration.Name = "AttackDelay";
                        duration.Maximum = 50000;
                        duration.Minimum = -50000;
                        duration.Width = 80;
                        duration.DecimalPlaces = 2;
                        duration.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[2]);
                        duration.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        duration.Enter += Numeric_Enter;
                        duration.Leave += Numeric_Leave;
                        duration.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(duration);
                        break;

                    case SpecialEffect.ShootBullet:
                    case SpecialEffect.ShootBulletAtTarget:
                        label = new Label();
                        label.Text = "Bullet to launch";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        TextBox textBox2 = new TextBox();
                        textBox2.Name = "BulletName";
                        textBox2.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        textBox2.Width = 150;
                        textBox2.Enter += textBox_Enter;
                        textBox2.Leave += textBox_Leave;
                        textBox2.Text = (CurrentAttackEffectSelected.Parameters[0] as BulletInfo) != null ? (string)(CurrentAttackEffectSelected.Parameters[0] as BulletInfo).Name : "";
                        this.EffectsInfoPanel.Controls.Add(textBox2);

                        label = new Label();
                        label.Text = "Offset";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, textBox2.Location.Y + textBox2.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown offsetShot = new NumericUpDown();
                        offsetShot.Name = "OffsetBulletX";
                        offsetShot.Maximum = 50000;
                        offsetShot.Minimum = -50000;
                        offsetShot.Width = 80;
                        offsetShot.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[1]).X;
                        offsetShot.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        offsetShot.Enter += Numeric_Enter;
                        offsetShot.Leave += Numeric_Leave;
                        offsetShot.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offsetShot);

                        offsetShot = new NumericUpDown();
                        offsetShot.Name = "OffsetBulletY";
                        offsetShot.Maximum = 50000;
                        offsetShot.Minimum = -50000;
                        offsetShot.Width = 80;
                        offsetShot.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[1]).Y;
                        offsetShot.Location = new System.Drawing.Point(offsetShot.Width + 10, label.Location.Y + label.Height + 5);
                        offsetShot.Enter += Numeric_Enter;
                        offsetShot.Leave += Numeric_Leave;
                        offsetShot.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offsetShot);
                        break;

                    case SpecialEffect.Invincible:
                        label = new Label();
                        label.Text = "Duration";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown attackDelay = new NumericUpDown();
                        attackDelay.Name = "InvincibleDuration";
                        attackDelay.Maximum = 50000;
                        attackDelay.Minimum = -50000;
                        attackDelay.Width = 80;
                        attackDelay.DecimalPlaces = 2;
                        attackDelay.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[0]);
                        attackDelay.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        attackDelay.Enter += Numeric_Enter;
                        attackDelay.Leave += Numeric_Leave;
                        attackDelay.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(attackDelay);
                        break;

                    case SpecialEffect.EffectAnimation:
                        label = new Label();
                        label.Text = "Spritesheet";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        ComboBox comboBox2 = new ComboBox();
                        comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboBox2.BindingContext = new BindingContext();
                        BindingSource bs = new BindingSource();
                        List<string> spritesheets = new List<string>();
                        spritesheets.AddRange(AssetManager.CommonSpritesheets.Keys.OrderBy(k => k.ToString()));
                        spritesheets.AddRange(AssetManager.GroupSpritesheets.Keys.OrderBy(k => k.ToString()));
                        spritesheets.AddRange(AssetManager.LevelSpritesheets.Keys.OrderBy(k => k.ToString()));

                        bs.DataSource = spritesheets;
                        comboBox2.DataSource = bs;
                        comboBox2.Name = "SpritesheetName";
                        comboBox2.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        comboBox2.Width = 250;
                        comboBox2.SelectedItem = (string)AssetManager.GetSpritesheetTag((SpriteSheetInfo)CurrentAttackEffectSelected.Parameters[0]);
                        comboBox2.SelectedValueChanged += comboBox_SelectedValueChanged;                       
                        this.EffectsInfoPanel.Controls.Add(comboBox2);

                        label = new Label();
                        label.Text = "Offset";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, comboBox2.Location.Y + comboBox2.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown offset = new NumericUpDown();
                        offset.Name = "OffsetX";
                        offset.Maximum = 50000;
                        offset.Minimum = -50000;
                        offset.Width = 80;
                        offset.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[2]).X;
                        offset.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        offset.Enter += Numeric_Enter;
                        offset.Leave += Numeric_Leave;
                        offset.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offset);

                        offset = new NumericUpDown();
                        offset.Name = "OffsetY";
                        offset.Maximum = 50000;
                        offset.Minimum = -50000;
                        offset.Width = 80;
                        offset.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[2]).Y;
                        offset.Location = new System.Drawing.Point(offset.Width + 10, label.Location.Y + label.Height + 5);
                        offset.Enter += Numeric_Enter;
                        offset.Leave += Numeric_Leave;
                        offset.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offset);

                        label = new Label();
                        label.Text = "Rotation";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, offset.Location.Y + offset.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        offset = new NumericUpDown();
                        offset.Name = "AnimationRotation";
                        offset.Maximum = 50000;
                        offset.Minimum = -50000;
                        offset.DecimalPlaces = 3;
                        offset.Width = 80;
                        offset.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[1]);
                        offset.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        offset.Enter += Numeric_Enter;
                        offset.Leave += Numeric_Leave;
                        offset.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offset);

                        CheckBox checkBoxFollow = new CheckBox();
                        checkBoxFollow.Text = "Must Follow";
                        checkBoxFollow.Name = "MustFollowCheckbox";
                        checkBoxFollow.Checked = (bool)CurrentAttackEffectSelected.Parameters[3];
                        checkBoxFollow.Location = new System.Drawing.Point(label.Location.X, offset.Location.Y + offset.Height + 5);
                        checkBoxFollow.CheckedChanged += checkBox_CheckedChanged;
                        EffectsInfoPanel.Controls.Add(checkBoxFollow);

                        label = new Label();
                        label.Text = "Scale";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(checkBoxFollow.Location.X, checkBoxFollow.Location.Y + checkBoxFollow.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown scale = new NumericUpDown();
                        scale.Name = "EffectScale";
                        scale.Maximum = 50000;
                        scale.Minimum = -50000;
                        scale.Width = 80;
                        scale.DecimalPlaces = 2;
                        scale.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[4]);
                        scale.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        scale.Enter += Numeric_Enter;
                        scale.Leave += Numeric_Leave;
                        scale.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(scale);

                        label = new Label();
                        label.Text = "Delay";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(scale.Location.X, scale.Location.Y + scale.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown delay = new NumericUpDown();
                        delay.Name = "DelayAnimation";
                        delay.Maximum = 50000;
                        delay.Minimum = -50000;
                        delay.Width = 80;
                        delay.DecimalPlaces = 2;
                        delay.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[5]);
                        delay.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        delay.Enter += Numeric_Enter;
                        delay.Leave += Numeric_Leave;
                        delay.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(delay);

                        label = new Label();
                        label.Text = "Layer";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(delay.Location.X, delay.Location.Y + delay.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown layer = new NumericUpDown();
                        layer.Name = "LayerAnimation";
                        layer.Maximum = 50000;
                        layer.Minimum = -50000;
                        layer.Width = 80;
                        layer.DecimalPlaces = 2;
                        layer.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[6]);
                        layer.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        layer.Enter += Numeric_Enter;
                        layer.Leave += Numeric_Leave;
                        layer.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(layer);

                        CheckBox checkbox6 = new CheckBox();
                        checkbox6.Text = "Loop";
                        checkbox6.Location = new System.Drawing.Point(5, layer.Location.Y + layer.Height + 5);
                        checkbox6.CheckedChanged += checkbox6_CheckedChanged;
                        checkbox6.Checked = (bool)CurrentAttackEffectSelected.Parameters[7];
                        this.EffectsInfoPanel.Controls.Add(checkbox6);

                        CheckBox checkbox7 = new CheckBox();
                        checkbox7.Text = "StopWithAttack";
                        checkbox7.Location = new System.Drawing.Point(5, checkbox6.Location.Y + checkbox6.Height + 5);
                        checkbox7.CheckedChanged += checkbox7_CheckedChanged;
                        checkbox7.Checked = (bool)CurrentAttackEffectSelected.Parameters[8];
                        this.EffectsInfoPanel.Controls.Add(checkbox7);
                        break;

                    case SpecialEffect.MoveWhileAttacking:
                        label = new Label();
                        label.Text = "Move speed";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown speed = new NumericUpDown();
                        speed.Name = "MoveSpeed";
                        speed.Maximum = 50000;
                        speed.Minimum = -50000;
                        speed.Width = 80;
                        speed.DecimalPlaces = 2;
                        speed.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[0]);
                        speed.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        speed.Enter += Numeric_Enter;
                        speed.Leave += Numeric_Leave;
                        speed.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(speed);

                        CheckBox checkBox5 = new CheckBox();
                        checkBox5.Text = "Must stop on target";
                        checkBox5.Name = "TargetStop";
                        checkBox5.Checked = (bool)CurrentAttackEffectSelected.Parameters[1];
                        checkBox5.Location = new System.Drawing.Point(label.Location.X, speed.Location.Y + speed.Height + 5);
                        checkBox5.CheckedChanged += checkBox_CheckedChanged;
                        EffectsInfoPanel.Controls.Add(checkBox5);
                        break;

                    case SpecialEffect.PercentageDamageBoost:
                        label = new Label();
                        label.Text = "Duration";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown boostDuration = new NumericUpDown();
                        boostDuration.Name = "BoostDuration";
                        boostDuration.Maximum = 50000;
                        boostDuration.Minimum = -50000;
                        boostDuration.DecimalPlaces = 2;
                        boostDuration.Width = 80;
                        boostDuration.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[0]);
                        boostDuration.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        boostDuration.Enter += Numeric_Enter;
                        boostDuration.Leave += Numeric_Leave;
                        boostDuration.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(boostDuration);

                        label = new Label();
                        label.Text = "Damage Boost";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, boostDuration.Height + boostDuration.Location.Y + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown boostPercentage = new NumericUpDown();
                        boostPercentage.Name = "BoostPercentage";
                        boostPercentage.Maximum = 50000;
                        boostPercentage.Minimum = -50000;
                        boostPercentage.DecimalPlaces = 2;
                        boostPercentage.Width = 80;
                        boostPercentage.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[1]);
                        boostPercentage.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        boostPercentage.Enter += Numeric_Enter;
                        boostPercentage.Leave += Numeric_Leave;
                        boostPercentage.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(boostPercentage);
                        break;

                    case SpecialEffect.DamageOverTime:
                        label = new Label();
                        label.Text = "Duration";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown dotDuration = new NumericUpDown();
                        dotDuration.Name = "DamageDuration";
                        dotDuration.Maximum = 50000;
                        dotDuration.Minimum = -50000;
                        dotDuration.DecimalPlaces = 2;
                        dotDuration.Width = 80;
                        dotDuration.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[0]);
                        dotDuration.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        dotDuration.Enter += Numeric_Enter;
                        dotDuration.Leave += Numeric_Leave;
                        dotDuration.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(dotDuration);

                        label = new Label();
                        label.Text = "Damage";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, dotDuration.Height + dotDuration.Location.Y + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown dotDamage = new NumericUpDown();
                        dotDamage.Name = "DamageValue";
                        dotDamage.Maximum = 50000;
                        dotDamage.Minimum = -50000;
                        dotDamage.DecimalPlaces = 2;
                        dotDamage.Width = 80;
                        dotDamage.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[1]);
                        dotDamage.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        dotDamage.Enter += Numeric_Enter;
                        dotDamage.Leave += Numeric_Leave;
                        dotDamage.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(dotDamage);

                        label = new Label();
                        label.Text = "Tick Timer";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, dotDamage.Height + dotDamage.Location.Y + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown dotSpeed = new NumericUpDown();
                        dotSpeed.Name = "TickTimer";
                        dotSpeed.Maximum = 50000;
                        dotSpeed.Minimum = -50000;
                        dotSpeed.DecimalPlaces = 2;
                        dotSpeed.Width = 80;
                        dotSpeed.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[2]);
                        dotSpeed.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        dotSpeed.Enter += Numeric_Enter;
                        dotSpeed.Leave += Numeric_Leave;
                        dotSpeed.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(dotSpeed);
                        break;

                    case SpecialEffect.InstantiatePrefab:
                        label = new Label();
                        label.Text = "Prefab to create";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        TextBox textBox3 = new TextBox();
                        textBox3.Name = "PrefabName";
                        textBox3.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        textBox3.Width = 150;
                        textBox3.Enter += textBox_Enter;
                        textBox3.Leave += textBox_Leave;
                        textBox3.Text = CurrentAttackEffectSelected.Parameters[0] as string;
                        this.EffectsInfoPanel.Controls.Add(textBox3);

                        label = new Label();
                        label.Text = "Impulse At Spawn";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, textBox3.Location.Y + textBox3.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown impulsePrefab = new NumericUpDown();
                        impulsePrefab.Name = "ImpulsePrefabX";
                        impulsePrefab.Maximum = 50000;
                        impulsePrefab.Minimum = -50000;
                        impulsePrefab.Width = 80;
                        impulsePrefab.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[1]).X;
                        impulsePrefab.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        impulsePrefab.Enter += Numeric_Enter;
                        impulsePrefab.Leave += Numeric_Leave;
                        impulsePrefab.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(impulsePrefab);

                        impulsePrefab = new NumericUpDown();
                        impulsePrefab.Name = "ImpulsePrefabY";
                        impulsePrefab.Maximum = 50000;
                        impulsePrefab.Minimum = -50000;
                        impulsePrefab.Width = 80;
                        impulsePrefab.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[1]).Y;
                        impulsePrefab.Location = new System.Drawing.Point(impulsePrefab.Width + 10, label.Location.Y + label.Height + 5);
                        impulsePrefab.Enter += Numeric_Enter;
                        impulsePrefab.Leave += Numeric_Leave;
                        impulsePrefab.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(impulsePrefab);

                        label = new Label();
                        label.Text = "Spawn Offset";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, impulsePrefab.Location.Y + impulsePrefab.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown offsetPrefab = new NumericUpDown();
                        offsetPrefab.Name = "OffsetPrefabX";
                        offsetPrefab.Maximum = 50000;
                        offsetPrefab.Minimum = -50000;
                        offsetPrefab.Width = 80;
                        offsetPrefab.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[2]).X;
                        offsetPrefab.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        offsetPrefab.Enter += Numeric_Enter;
                        offsetPrefab.Leave += Numeric_Leave;
                        offsetPrefab.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offsetPrefab);

                        offsetPrefab = new NumericUpDown();
                        offsetPrefab.Name = "OffsetPrefabY";
                        offsetPrefab.Maximum = 50000;
                        offsetPrefab.Minimum = -50000;
                        offsetPrefab.Width = 80;
                        offsetPrefab.Value = (decimal)((Vector2)CurrentAttackEffectSelected.Parameters[2]).Y;
                        offsetPrefab.Location = new System.Drawing.Point(offsetPrefab.Width + 10, label.Location.Y + label.Height + 5);
                        offsetPrefab.Enter += Numeric_Enter;
                        offsetPrefab.Leave += Numeric_Leave;
                        offsetPrefab.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(offsetPrefab);
                        break;

                    case SpecialEffect.PlaySound:
                        label = new Label();
                        label.Text = "Sound To Play";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, 60);
                        this.EffectsInfoPanel.Controls.Add(label);

                        ComboBox comboBox3 = new ComboBox();
                        comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboBox3.BindingContext = new BindingContext();
                        BindingSource bs2 = new BindingSource();
                        List<string> sounds = new List<string>();
                        sounds.AddRange(SoundManager.Sounds.Keys.OrderBy(k => k.ToString()));

                        bs2.DataSource = sounds;
                        comboBox3.DataSource = bs2;
                        comboBox3.Name = "SoundName";
                        comboBox3.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        comboBox3.Width = 150;
                        comboBox3.SelectedItem = (string)CurrentAttackEffectSelected.Parameters[0];
                        comboBox3.SelectedValueChanged += comboBox_SelectedValueChanged;
                        this.EffectsInfoPanel.Controls.Add(comboBox3);

                        label = new Label();
                        label.Text = "Volume";
                        label.Height = 15;
                        label.Location = new System.Drawing.Point(5, comboBox3.Location.Y + comboBox3.Height + 5);
                        this.EffectsInfoPanel.Controls.Add(label);

                        NumericUpDown volume = new NumericUpDown();
                        volume.Name = "SoundVolume";
                        volume.Maximum = 50000;
                        volume.Minimum = -50000;
                        volume.Width = 80;
                        volume.DecimalPlaces = 2;
                        volume.Increment = (decimal)0.1f;
                        volume.Value = (decimal)((float)CurrentAttackEffectSelected.Parameters[1]);
                        volume.Location = new System.Drawing.Point(5, label.Location.Y + label.Height + 5);
                        volume.Enter += Numeric_Enter;
                        volume.Leave += Numeric_Leave;
                        volume.ValueChanged += Numeric_ValueChanged;
                        this.EffectsInfoPanel.Controls.Add(volume);
                        
                        CheckBox checkBox6 = new CheckBox();
                        checkBox6.Text = "StopSoundWithAttack";
                        checkBox6.Name = "StopSound";
                        checkBox6.Checked = (bool)CurrentAttackEffectSelected.Parameters[2];
                        checkBox6.Location = new System.Drawing.Point(label.Location.X, volume.Location.Y + volume.Height + 5);
                        checkBox6.CheckedChanged += checkBox_CheckedChanged;
                        EffectsInfoPanel.Controls.Add(checkBox6);
                        break;
                }
            }
        }

        private void checkbox7_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAttackEffectSelected.Parameters[8] = (sender as CheckBox).Checked;
        }

        void checkbox6_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAttackEffectSelected.Parameters[7] = (sender as CheckBox).Checked;
        }

        void checkbox3_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAttackEffectSelected.Parameters[1] = (sender as CheckBox).Checked;
        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Name == "NotInAirCheckbox")
            {
                CurrentAttackEffectSelected.Parameters[2] = (bool)(sender as CheckBox).Checked;
            }
            else if ((sender as CheckBox).Name == "MustFollowCheckbox")
            {
                CurrentAttackEffectSelected.Parameters[3] = (bool)(sender as CheckBox).Checked;
            }
            else if ((sender as CheckBox).Name == "StopSound")
            {
                CurrentAttackEffectSelected.Parameters[2] = (bool)(sender as CheckBox).Checked;
            }
            else
                CurrentAttackEffectSelected.Parameters[1] = (bool)(sender as CheckBox).Checked;
        }

        void textBox_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Name == "BulletName")
                CurrentAttackEffectSelected.Parameters[0] = BulletsManager.GetBulletInfo((sender as TextBox).Text);
            else
                CurrentAttackEffectSelected.Parameters[0] = (sender as TextBox).Text;
            (Neon.World as EditorScreen).FocusedTextBox = null;

        }

        void textBox_Enter(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).FocusedTextBox = sender as TextBox;
        }

        private void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).Name == "SpritesheetName")
            {
                CurrentAttackEffectSelected.Parameters[0] = AssetManager.GetSpriteSheet((string)((sender as ComboBox).SelectedItem));
                return;
            }
            else if ((sender as ComboBox).Name == "SoundName")
            {
                CurrentAttackEffectSelected.Parameters[0] = (string)((sender as ComboBox).SelectedItem);
                return;
            }


            if (CurrentAttackEffectSelected.specialEffect != (SpecialEffect)(sender as ComboBox).SelectedItem)
            {
                CurrentAttackEffectSelected.specialEffect = (SpecialEffect)(sender as ComboBox).SelectedItem;
                CurrentAttackEffectSelected.Parameters = null;
                switch (CurrentAttackEffectSelected.specialEffect)
                {
                    case SpecialEffect.PositionalPulse:
                        CurrentAttackEffectSelected.Parameters = new object[] { new Vector2() };
                        break;

                    case SpecialEffect.Impulse:
                        CurrentAttackEffectSelected.Parameters = new object[] { new Vector2(), false, false };
                        break;

                    case SpecialEffect.Invincible:
                        CurrentAttackEffectSelected.Parameters = new object[] { 0.0f };
                        break;

                    case SpecialEffect.ShootBullet:
                        CurrentAttackEffectSelected.Parameters = new object[] { BulletsManager._bulletsInformation[0], new Vector2() };
                        break;

                    case SpecialEffect.ShootBulletAtTarget:
                        CurrentAttackEffectSelected.Parameters = new object[] { BulletsManager._bulletsInformation[0], new Vector2() };
                        break;

                    case SpecialEffect.StartAttack:
                        CurrentAttackEffectSelected.Parameters = new object[] { "", false, 0.0f };
                        break;

                    case SpecialEffect.EffectAnimation:
                        CurrentAttackEffectSelected.Parameters = new object[] { new SpriteSheetInfo(), 0.0f, new Vector2(), false, 1.0f, 0.0f, 1.0f, false, false };
                        break;

                    case SpecialEffect.MoveWhileAttacking:
                        CurrentAttackEffectSelected.Parameters = new object[] { 0.0f, false };
                        break;

                    case SpecialEffect.PercentageDamageBoost:
                        CurrentAttackEffectSelected.Parameters = new object[] { 0.0f, 0.0f };
                        break;

                    case SpecialEffect.DamageOverTime:
                        CurrentAttackEffectSelected.Parameters = new object[] { 0.0f, 0.0f, 0.0f };
                        break;

                    case SpecialEffect.InstantiatePrefab:
                        CurrentAttackEffectSelected.Parameters = new object[] { "", new Vector2(), new Vector2() };
                        break;

                    case SpecialEffect.PlaySound:
                        CurrentAttackEffectSelected.Parameters = new object[] { "", 1.0f, false };
                        break;
                }
                this.InitEffectData();
            }

        }


        private void InitInformations()
        {
            this.AttackName.Text = this.CurrentAttackSelected.Name;
            this.GroupName.Text = this.CurrentAttackSelected.GroupName;
            this.TypeComboBox.SelectedIndex = (int)this.CurrentAttackSelected.Type;
            this.ElementCombobox.SelectedIndex = (int)this.CurrentAttackSelected.AttackElement;
            this.DamageNumeric.Value = (decimal)this.CurrentAttackSelected.DamageOnHit * -1;
            this.DelayNumeric.Value = (decimal)this.CurrentAttackSelected.Delay;
            this.DurationNumeric.Value = (decimal)this.CurrentAttackSelected.Duration;
            this.CooldownNumeric.Value = (decimal)this.CurrentAttackSelected.Cooldown;
            this.AirLockNumeric.Value = (decimal)this.CurrentAttackSelected.AirLock;
            this.TargetAirLockNumeric.Value = (decimal)this.CurrentAttackSelected.TargetAirLock;
            this.AirOnlyCheckbox.Checked = this.CurrentAttackSelected.AirOnly;
            this.GroundCancelCheckbox.Checked = this.CurrentAttackSelected.CancelOnGround;
            this.OnlyOnceInAir.Checked = this.CurrentAttackSelected.OnlyOnceInAir;
            this.AirFactorNU.Value = (decimal)this.CurrentAttackSelected.AirFactor;
            this.StunLockNumeric.Value = (decimal)this.CurrentAttackSelected.StunLock;
            this.LocalCooldownNumeric.Value = (decimal)this.CurrentAttackSelected.LocalCooldown;
            this.MultiHitDelayNU.Value = (decimal)this.CurrentAttackSelected.MultiHitDelay;
            this.AirImpulseX.Value = (decimal)this.CurrentAttackSelected.AirImpulse.X;
            this.AirImpulseY.Value = (decimal)this.CurrentAttackSelected.AirImpulse.Y;
            this.AlwaysStunlock.Checked = this.CurrentAttackSelected.AlwaysStunlock;

            int yPosition = 5;
            int rectangleIndex = 0;
            
            for (int i = HitboxesPanel.Controls.Count - 1; i >= 0; i--)
                HitboxesPanel.Controls[i].Dispose();

            foreach (Microsoft.Xna.Framework.Rectangle rectangle in CurrentAttackSelected.Hitboxes)
                {
                    Label label = new Label();
                    label.Text = "X";
                    label.Location = new System.Drawing.Point(5, yPosition + 4);
                    label.Width = 10;
                    this.HitboxesPanel.Controls.Add(label);

                    NumericUpDown numericUpDown = new NumericUpDown();
                    numericUpDown.Name = "X_" + rectangleIndex;
                    numericUpDown.Width = 50;
                    numericUpDown.Minimum = -1000;
                    numericUpDown.Maximum = 1000;
                    numericUpDown.Location = new System.Drawing.Point(label.Location.X + label.Width + 5, yPosition);
                    numericUpDown.Value = rectangle.X;
                    numericUpDown.Enter += Numeric_Enter;
                    numericUpDown.Leave += Numeric_Leave;
                    numericUpDown.ValueChanged += hitboxesNumericUpDown_ValueChanged;
                    this.HitboxesPanel.Controls.Add(numericUpDown);

                    label = new Label();
                    label.Text = "Y";
                    label.Location = new System.Drawing.Point(numericUpDown.Location.X + numericUpDown.Width + 5, yPosition + 4);
                    label.Width = 10;
                    this.HitboxesPanel.Controls.Add(label);

                    numericUpDown = new NumericUpDown();
                    numericUpDown.Name = "Y_" + rectangleIndex;
                    numericUpDown.Width = 50;
                    numericUpDown.Minimum = -1000;
                    numericUpDown.Maximum = 1000;
                    numericUpDown.Location = new System.Drawing.Point(label.Location.X + label.Width + 5, yPosition);
                    numericUpDown.Value = rectangle.Y;
                    numericUpDown.Enter += Numeric_Enter;
                    numericUpDown.Leave += Numeric_Leave;
                    numericUpDown.ValueChanged += hitboxesNumericUpDown_ValueChanged;
                    this.HitboxesPanel.Controls.Add(numericUpDown);

                    label = new Label();
                    label.Text = "Width";
                    label.Location = new System.Drawing.Point(numericUpDown.Location.X + numericUpDown.Width + 5, yPosition + 4);
                    label.Width = 35;
                    this.HitboxesPanel.Controls.Add(label);

                    numericUpDown = new NumericUpDown();
                    numericUpDown.Name = "W_" + rectangleIndex;
                    numericUpDown.Width = 50;
                    numericUpDown.Minimum = -1000;
                    numericUpDown.Maximum = 1000;
                    numericUpDown.Location = new System.Drawing.Point(label.Location.X + label.Width + 5, yPosition);
                    numericUpDown.Value = rectangle.Width;
                    numericUpDown.Enter += Numeric_Enter;
                    numericUpDown.Leave += Numeric_Leave;
                    numericUpDown.ValueChanged += hitboxesNumericUpDown_ValueChanged;
                    this.HitboxesPanel.Controls.Add(numericUpDown);

                    label = new Label();
                    label.Text = "Height";
                    label.Location = new System.Drawing.Point(numericUpDown.Location.X + numericUpDown.Width + 5, yPosition + 4);
                    label.Width = 37;
                    this.HitboxesPanel.Controls.Add(label);

                    numericUpDown = new NumericUpDown();
                    numericUpDown.Name = "H_" + rectangleIndex;
                    numericUpDown.Width = 50;
                    numericUpDown.Minimum = -1000;
                    numericUpDown.Maximum = 1000;
                    numericUpDown.Location = new System.Drawing.Point(label.Location.X + label.Width + 5, yPosition);
                    numericUpDown.Value = rectangle.Height;
                    numericUpDown.Enter += Numeric_Enter;
                    numericUpDown.Leave += Numeric_Leave;
                    numericUpDown.ValueChanged += hitboxesNumericUpDown_ValueChanged;
                    this.HitboxesPanel.Controls.Add(numericUpDown);

                    yPosition += numericUpDown.Height + 5;
                    rectangleIndex++;
                }

            if (rectangleIndex > 0)
            {
                Button buttonRemove = new Button();
                buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                buttonRemove.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
                buttonRemove.Location = new System.Drawing.Point(5, yPosition + 5);
                buttonRemove.Size = new System.Drawing.Size(100, 23);
                buttonRemove.Text = "Remove Last";
                buttonRemove.Click += buttonRemove_Click;
                buttonRemove.UseVisualStyleBackColor = true;

                this.HitboxesPanel.Controls.Add(buttonRemove);
            }

            Button buttonAdd = new Button();
            buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            buttonAdd.Location = new System.Drawing.Point(115, yPosition + 5);
            buttonAdd.Size = new System.Drawing.Size(100, 23);
            buttonAdd.Text = "Add New";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += buttonAdd_Click;

            this.HitboxesPanel.Controls.Add(buttonAdd);
            this.OnDurationSpecialEffectsList.DataSource = null;
            this.OnDurationSpecialEffectsList.DataSource = this.CurrentAttackSelected.OnDurationSpecialEffects;
            this.OnDurationSpecialEffectsList.DisplayMember = "NameType";
            this.OnHitSpecialEffectsList.DataSource = null;
            this.OnHitSpecialEffectsList.DataSource = this.CurrentAttackSelected.OnHitSpecialEffects;
            this.OnHitSpecialEffectsList.DisplayMember = "NameType";
            this.OnGroundCancelSpecialEffectList.DataSource = null;
            this.OnGroundCancelSpecialEffectList.DataSource = this.CurrentAttackSelected.OnGroundCancelSpecialEffects;
            this.OnGroundCancelSpecialEffectList.DisplayMember = "NameType";
            this.OnDelaySpecialEffectsList.DataSource = null;
            this.OnDelaySpecialEffectsList.DataSource = this.CurrentAttackSelected.OnDelaySpecialEffects;
            this.OnDelaySpecialEffectsList.DisplayMember = "NameType";
            this.OnFinishSpecialEffectsList.DataSource = null;
            this.OnFinishSpecialEffectsList.DataSource = this.CurrentAttackSelected.OnFinishSpecialEffects;
            this.OnFinishSpecialEffectsList.DisplayMember = "NameType";
        }

        void buttonAdd_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.Hitboxes.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
            InitInformations();
        }


        void buttonRemove_Click(object sender, EventArgs e)
        {
            if (CurrentAttackSelected != null)
            {
                CurrentAttackSelected.Hitboxes.RemoveAt(CurrentAttackSelected.Hitboxes.Count - 1);
                InitInformations();
            }
            

        }

        private void hitboxesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            string[] nameInformation = numericUpDown.Name.Split('_');

            string name = nameInformation[0];
            Microsoft.Xna.Framework.Rectangle rectangle = CurrentAttackSelected.Hitboxes[int.Parse(nameInformation[1])];

            switch (name)
            {
                case "X":
                    CurrentAttackSelected.Hitboxes[int.Parse(nameInformation[1])] = new Microsoft.Xna.Framework.Rectangle((int)numericUpDown.Value, rectangle.Y, rectangle.Width, rectangle.Height);
                    break;

                case "Y":
                    CurrentAttackSelected.Hitboxes[int.Parse(nameInformation[1])] = new Microsoft.Xna.Framework.Rectangle(rectangle.X, (int)numericUpDown.Value, rectangle.Width, rectangle.Height);
                    break;

                case "W":
                    CurrentAttackSelected.Hitboxes[int.Parse(nameInformation[1])] = new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, (int)numericUpDown.Value, rectangle.Height);
                    break;

                case "H":
                    CurrentAttackSelected.Hitboxes[int.Parse(nameInformation[1])] = new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)numericUpDown.Value);
                    break;
            }

        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.Type = (AttackType)this.TypeComboBox.SelectedIndex;
        }

        private void ElementComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.AttackElement = (Element)this.ElementCombobox.SelectedIndex;
        }

        private void Numeric_ValueChanged(object sender, EventArgs e)
        {
            switch((sender as NumericUpDown).Name)
            {
                case "AttackDelay":
                    CurrentAttackEffectSelected.Parameters[2] = (float)(sender as NumericUpDown).Value;
                    break;

                case "SoundVolume":
                    CurrentAttackEffectSelected.Parameters[1] = (float)(sender as NumericUpDown).Value;
                    break;

                case "DamageNumeric":
                    this.CurrentAttackSelected.DamageOnHit = (float)(sender as NumericUpDown).Value * -1;
                    break;

                case "DelayNumeric":
                    this.CurrentAttackSelected.Delay = (float)(sender as NumericUpDown).Value;
                    break;

                case "DurationNumeric":
                    this.CurrentAttackSelected.Duration = (float)(sender as NumericUpDown).Value;
                    break;

                case "CooldownNumeric":
                    this.CurrentAttackSelected.Cooldown = (float)(sender as NumericUpDown).Value;
                    break;

                case "AirLockNumeric":
                    this.CurrentAttackSelected.AirLock = (float)(sender as NumericUpDown).Value;
                    break;

                case "TargetAirLockNumeric":
                    this.CurrentAttackSelected.TargetAirLock = (float)(sender as NumericUpDown).Value;
                    break;

                case "ImpulsePowerX":
                    Vector2 impulsePowerY = (Vector2)CurrentAttackEffectSelected.Parameters[0];
                    CurrentAttackEffectSelected.Parameters[0] = new Vector2((float)(sender as NumericUpDown).Value, impulsePowerY.Y);
                    break;

                case "ImpulsePowerY":
                    Vector2 impulsePowerX = (Vector2)CurrentAttackEffectSelected.Parameters[0];
                    CurrentAttackEffectSelected.Parameters[0] = new Vector2(impulsePowerX.X, (float)(sender as NumericUpDown).Value);
                    break;

                case "OffsetX":
                    Vector2 offset = (Vector2)CurrentAttackEffectSelected.Parameters[2];
                    CurrentAttackEffectSelected.Parameters[2] = new Vector2((float)(sender as NumericUpDown).Value, offset.Y);
                    break;

                case "OffsetY":
                    Vector2 offset2 = (Vector2)CurrentAttackEffectSelected.Parameters[2];
                    CurrentAttackEffectSelected.Parameters[2] = new Vector2(offset2.X, (float)(sender as NumericUpDown).Value);
                    break;

                case "AnimationRotation":
                    CurrentAttackEffectSelected.Parameters[1] = (float)(sender as NumericUpDown).Value;
                    break;

                case "AirFactorNU":
                    this.CurrentAttackSelected.AirFactor = (float)(sender as NumericUpDown).Value;
                    break;

                case "StunLockNumeric":
                    this.CurrentAttackSelected.StunLock = (float)(sender as NumericUpDown).Value;
                    break;

                case "InvincibleDuration":
                    CurrentAttackEffectSelected.Parameters[0] = (float)(sender as NumericUpDown).Value;
                    break;

                case "MoveSpeed":
                    CurrentAttackEffectSelected.Parameters[0] = (float)(sender as NumericUpDown).Value;
                    break;

                case "LocalCooldownNumeric":
                    this.CurrentAttackSelected.LocalCooldown = (float)(sender as NumericUpDown).Value;
                    break;

                case "BoostDuration":
                    CurrentAttackEffectSelected.Parameters[0] = (float)(sender as NumericUpDown).Value;
                    break;

                case "BoostPercentage":
                    CurrentAttackEffectSelected.Parameters[1] = (float)(sender as NumericUpDown).Value;
                    break;

                case "DamageDuration":
                    CurrentAttackEffectSelected.Parameters[0] = (float)(sender as NumericUpDown).Value;
                    break;

                case "DamageValue":
                    CurrentAttackEffectSelected.Parameters[1] = (float)(sender as NumericUpDown).Value;
                    break;

                case "TickTimer":
                    CurrentAttackEffectSelected.Parameters[2] = (float)(sender as NumericUpDown).Value;
                    break;

                case "OffsetBulletX":
                    Vector2 offset3 = (Vector2)CurrentAttackEffectSelected.Parameters[1];
                    CurrentAttackEffectSelected.Parameters[1] = new Vector2((float)(sender as NumericUpDown).Value, offset3.Y);
                    break;

                case "OffsetBulletY":
                    Vector2 offset4 = (Vector2)CurrentAttackEffectSelected.Parameters[1];
                    CurrentAttackEffectSelected.Parameters[1] = new Vector2(offset4.X, (float)(sender as NumericUpDown).Value);
                    break;

                case "MultiHitDelayNU":
                    this.CurrentAttackSelected.MultiHitDelay = (float)(sender as NumericUpDown).Value;
                    break;

                case "EffectScale":
                    CurrentAttackEffectSelected.Parameters[4] = (float)(sender as NumericUpDown).Value;
                    break;

                case "LayerAnimation":
                    CurrentAttackEffectSelected.Parameters[6] = (float)(sender as NumericUpDown).Value;
                    break;

                case "ImpulsePrefabX":
                    Vector2 offset5 = (Vector2)CurrentAttackEffectSelected.Parameters[1];
                    CurrentAttackEffectSelected.Parameters[1] = new Vector2((float)(sender as NumericUpDown).Value, offset5.Y);
                    break;

                case "ImpulsePrefabY":
                    Vector2 offset6 = (Vector2)CurrentAttackEffectSelected.Parameters[1];
                    CurrentAttackEffectSelected.Parameters[1] = new Vector2(offset6.X,(float)(sender as NumericUpDown).Value);
                    break;

                case "OffsetPrefabX":
                    Vector2 offset7 = (Vector2)CurrentAttackEffectSelected.Parameters[2];
                    CurrentAttackEffectSelected.Parameters[2] = new Vector2((float)(sender as NumericUpDown).Value, offset7.Y);
                    break;

                case "OffsetPrefabY":
                    Vector2 offset8 = (Vector2)CurrentAttackEffectSelected.Parameters[2];
                    CurrentAttackEffectSelected.Parameters[2] = new Vector2(offset8.X, (float)(sender as NumericUpDown).Value);
                    break;

                case "DelayAnimation":
                    CurrentAttackEffectSelected.Parameters[5] = (float)(sender as NumericUpDown).Value;
                    break;

                case "AirImpulseX":
                    Vector2 impulse = this.CurrentAttackSelected.AirImpulse;
                    this.CurrentAttackSelected.AirImpulse = new Vector2((float)(sender as NumericUpDown).Value, impulse.Y);
                    break;

                case "AirImpulseY":
                    Vector2 impulse2 = this.CurrentAttackSelected.AirImpulse;
                    this.CurrentAttackSelected.AirImpulse = new Vector2(impulse2.X, (float)(sender as NumericUpDown).Value);
                    break;
            }
        }

        private void Numeric_Enter(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).FocusedNumericUpDown = sender as NumericUpDown;
        }

        private void Numeric_Leave(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).FocusedNumericUpDown = null;
        }

        private void AttackName_Enter(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).FocusedTextBox = sender as TextBox;
        }

        private void AttackName_Leave(object sender, EventArgs e)
        {
            CurrentAttackSelected.Name = (sender as TextBox).Text;
            (Neon.World as EditorScreen).FocusedTextBox = null;
            InitializeData(false);
        }

        private void AddSpecial_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnDurationSpecialEffects.Add(new AttackEffect(SpecialEffect.Impulse, new object[] { Vector2.Zero, false, false }));
            InitInformations();
        }

        private void AddOnHit_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnHitSpecialEffects.Add(new AttackEffect(SpecialEffect.Impulse, new object[] { Vector2.Zero, false, false }));
            InitInformations();
        }

        private void AddOnGround_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnGroundCancelSpecialEffects.Add(new AttackEffect(SpecialEffect.Impulse, new object[] { Vector2.Zero, false, false }));
            InitInformations();
        }

        private void RemoveSpecial_Click(object sender, EventArgs e)
        {
            if (OnDurationSpecialEffectsList.SelectedValue != null)
                this.CurrentAttackSelected.OnDurationSpecialEffects.RemoveAt(OnDurationSpecialEffectsList.SelectedIndex);
            InitInformations();
        }

        private void RemoveOnHit_Click(object sender, EventArgs e)
        {
            if (OnHitSpecialEffectsList.SelectedValue != null)
                this.CurrentAttackSelected.OnHitSpecialEffects.RemoveAt(OnHitSpecialEffectsList.SelectedIndex);
            InitInformations();
        }

        private void RemoveOnGround_Click(object sender, EventArgs e)
        {
            if (OnGroundCancelSpecialEffectList.SelectedValue != null)
                this.CurrentAttackSelected.OnGroundCancelSpecialEffects.RemoveAt(OnGroundCancelSpecialEffectList.SelectedIndex);
            InitInformations();
        }

        private void SpecialEffectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                CurrentAttackEffectSelected = null;
            }
            else
            {
                CurrentAttackEffectSelected = this.CurrentAttackSelected.OnDurationSpecialEffects[(sender as ListBox).SelectedIndex];
            }           
            InitEffectData();
        }

        private void OnHitSpecialEffectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                CurrentAttackEffectSelected = null;
            }
            else
            {
                CurrentAttackEffectSelected = this.CurrentAttackSelected.OnHitSpecialEffects[(sender as ListBox).SelectedIndex];
            }          
            InitEffectData();
        }

        private void OnGroundCancelSpecialEffectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                CurrentAttackEffectSelected = null;
            }
            else
            {
                CurrentAttackEffectSelected = this.CurrentAttackSelected.OnGroundCancelSpecialEffects[(sender as ListBox).SelectedIndex];
            }
            
            InitEffectData();
        }

        private void GroundCancelCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.CancelOnGround = (sender as CheckBox).Checked;
        }

        private void AirOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.AirOnly = (sender as CheckBox).Checked;
        }

        private void OnlyOnceInAir_CheckedChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnlyOnceInAir = (sender as CheckBox).Checked;
        }

        private void AddDelayEffect_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnDelaySpecialEffects.Add(new AttackEffect(SpecialEffect.Impulse, new object[] { Vector2.Zero, false, false }));
            InitInformations();
        }

        private void RemoveDelayEffect_Click(object sender, EventArgs e)
        {
            if (OnDelaySpecialEffectsList.SelectedValue != null)
                this.CurrentAttackSelected.OnDelaySpecialEffects.RemoveAt(OnDelaySpecialEffectsList.SelectedIndex);
            InitInformations();
        }

        private void OnDelaySpecialEffectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                CurrentAttackEffectSelected = null;
            }
            else
            {
                CurrentAttackEffectSelected = this.CurrentAttackSelected.OnDelaySpecialEffects[(sender as ListBox).SelectedIndex];
            }
            InitEffectData();
        }

        private void AlwaysStunlock_CheckedChanged(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.AlwaysStunlock = (sender as CheckBox).Checked;
        }

        private void AddFinishEffect_Click(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.OnFinishSpecialEffects.Add(new AttackEffect(SpecialEffect.Impulse, new object[] { Vector2.Zero, false, false }));
            InitInformations();
        }

        private void RemoveFinishEffect_Click(object sender, EventArgs e)
        {
            if (OnFinishSpecialEffectsList.SelectedValue != null)
                this.CurrentAttackSelected.OnFinishSpecialEffects.RemoveAt(OnFinishSpecialEffectsList.SelectedIndex);
            InitInformations();
        }

        private void OnFinishSpecialEffectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                CurrentAttackEffectSelected = null;
            }
            else
            {
                CurrentAttackEffectSelected = this.CurrentAttackSelected.OnFinishSpecialEffects[(sender as ListBox).SelectedIndex];
            }
            InitEffectData();
        }

        private void GroupName_Leave(object sender, EventArgs e)
        {
            this.CurrentAttackSelected.GroupName = (sender as TextBox).Text;
            this.InitializeData();
            (Neon.World as EditorScreen).FocusedTextBox = null;
        }

        private void GroupName_Enter(object sender, EventArgs e)
        {
            (Neon.World as EditorScreen).FocusedTextBox = (sender as TextBox);
        }

        private void AttacksList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tv = (sender as TreeView);
            if (tv != null)
            {
                if (tv.SelectedNode != null && tv.SelectedNode.Tag != null)
                {
                    CurrentAttackSelected = tv.SelectedNode.Tag as AttackInfo;
                    if (CurrentAttackSelected != null)
                        InitInformations();
                }
            }
            
        }
    }
}
