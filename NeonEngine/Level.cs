﻿using Microsoft.Xna.Framework;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System;
using System.Reflection;

namespace NeonEngine
{
    public class Level
    {
        public Vector2 spawnPoint;
        public string levelFilePath;
        public Rectangle bounds;

        public Level(string levelFilePath, World container, bool collideWorld)
        {
            Console.WriteLine("Level loading...");
            Console.WriteLine("");

            Stream stream = File.OpenRead(levelFilePath + ".xml");
            
            XDocument document = XDocument.Load(stream);

            XElement XnaContent = document.Element("XnaContent");
            XElement Level = XnaContent.Element("Level");

            XElement LevelData = Level.Element("LevelData");
            XElement Geometry = Level.Element("Geometry");
            XElement WaterZones = Level.Element("WaterZones");
            XElement Entities = Level.Element("Entities");

            if(Entities != null)
                EntityImport(Entities, container);
            
            if(WaterZones != null)
                if (WaterZones != null)
                    foreach (XElement w in WaterZones.Elements("Water"))
                    {
                        Rectangle area = new Rectangle();
                        area.X = int.Parse(w.Attribute("X").Value);
                        area.Y = int.Parse(w.Attribute("Y").Value);
                        area.Width = int.Parse(w.Attribute("Width").Value);
                        area.Height = int.Parse(w.Attribute("Height").Value);
                        container.waterzones.Add(new Water(container, area));
                    }
            
            this.levelFilePath = levelFilePath;
            stream.Close();

            Console.WriteLine("");

            Console.WriteLine("Level loaded !");
            Console.WriteLine("");
        }

        public virtual void EntityImport(XElement Entities, World containerWorld)
        {
            foreach (XElement Ent in Entities.Elements("Entity"))
            {
                Entity entity = new Entity(containerWorld);
                entity.Name = Ent.Attribute("Name").Value;

                foreach (XElement Comp in Ent.Element("Components").Elements())
                {
                    if (Comp.Name == "Transform")
                    {
                        entity.transform.Position = Neon.utils.ParseVector2(Comp.Element("Properties").Element("Position").Attribute("Value").Value);
                        entity.transform.Rotation = float.Parse(Comp.Element("Properties").Element("Rotation").Attribute("Value").Value);
                        entity.transform.Scale = float.Parse(Comp.Element("Properties").Element("Scale").Attribute("Value").Value);
                    }
                    else
                    {
                        string AssemblyName = Comp.Attribute("Type").Value.Split('.')[0];
                        string TypeName = Comp.Attribute("Type").Value.Split('.')[1];
                        Type t = Type.GetType(AssemblyName + "." + TypeName + ", " + AssemblyName);
                        if (t == null)
                            foreach (Type type in Neon.Scripts)
                                if (type.Name == TypeName)
                                {
                                    t = type;
                                    break;
                                }

                        Component component = (Component)Activator.CreateInstance(t, entity);
                        component.ID = int.Parse(Comp.Attribute("ID").Value);
                        foreach (XElement Property in Comp.Element("Properties").Elements())
                        {
                            PropertyInfo pi = t.GetProperty(Property.Name.ToString());

                            if (pi.PropertyType.IsSubclassOf(typeof(Component)))
                                continue;
                            else if (pi.PropertyType.Equals(typeof(Vector2)))
                                pi.SetValue(component, Neon.utils.ParseVector2(Property.Attribute("Value").Value), null);
                            else if (pi.PropertyType.IsEnum)
                                pi.SetValue(component, Enum.Parse(pi.PropertyType, Property.Attribute("Value").Value), null);
                            else if (pi.PropertyType.Equals(typeof(Single)))
                                pi.SetValue(component, float.Parse(Property.Attribute("Value").Value), null);
                            else if (pi.PropertyType.Equals(typeof(bool)))
                                pi.SetValue(component, bool.Parse(Property.Attribute("Value").Value), null);
                            else if (pi.PropertyType.Equals(typeof(Int32)))
                                pi.SetValue(component, int.Parse(Property.Attribute("Value").Value), null);
                            else if (pi.PropertyType.Equals(typeof(String)))
                                pi.SetValue(component, Property.Attribute("Value").Value, null);
                        }

                        component.Init();
                        entity.AddComponent(component);
                    }
                }

                foreach (XElement Comp in Ent.Element("Components").Elements())
                {
                    Component comp = entity.Components.First(c => c.ID == int.Parse(Comp.Attribute("ID").Value));

                    foreach (XElement Property in Comp.Element("Properties").Elements())
                    {
                        PropertyInfo pi = comp.GetType().GetProperty(Property.Name.ToString());

                        if (pi.PropertyType.IsSubclassOf(typeof(Component)))
                            if (Property.Attribute("Value").Value != "None")
                            {
                                Component Value = entity.Components.First(c => c.ID == int.Parse(Property.Attribute("Value").Value));
                                pi.SetValue(comp, Value, null);
                            }
                    }
                }

                containerWorld.AddEntity(entity);
            }
        }
    }
}
