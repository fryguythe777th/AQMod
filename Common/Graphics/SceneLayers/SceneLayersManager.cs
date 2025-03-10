﻿using System.Collections.Generic;
using Terraria;

namespace AQMod.Common.Graphics.SceneLayers
{
    public sealed class SceneLayersManager
    {
        private static Dictionary<string, SceneLayer>[] layers;

        internal static LayerKey Register(string name, SceneLayer layer, SceneLayering layering)
        {
            if (AQMod.Loading)
            {
                var key = new LayerKey(layering, name);
                layers[key.Index].Add(key.Name, layer);
                return key;
            }
            return LayerKey.Null;
        }

        public static SceneLayer GetLayer(SceneLayering layering, string name)
        {
            return layers[(byte)layering][name];
        }

        public static T GetLayer<T>(SceneLayering layering, string name) where T : SceneLayer
        {
            return (T)layers[(byte)layering][name];
        }

        public static SceneLayer GetLayer(LayerKey key)
        {
            return layers[key.Index][key.Name];
        }

        public static T GetLayer<T>(LayerKey key) where T : SceneLayer
        {
            return (T)GetLayer(key);
        }

        private static SceneLayer[] GetEntireLayer(SceneLayering layering)
        {
            var l = SceneLayersManager.layers[(byte)layering];
            var layers = new SceneLayer[l.Count];
            int i = 0;
            foreach (var layer in l)
            {
                layers[i] = layer.Value;
                i++;
            }
            return layers;
        }

        /// <summary>
        /// Draws an entire layer
        /// </summary>
        /// <param name="layering"></param>
        internal static void DrawLayer(SceneLayering layering)
        {
            if (Main.gameMenu)
                return;
            foreach (var layer in layers[(byte)layering])
            {
                layer.Value.DrawLayer();
            }
        }

        public static void UpdateLayers()
        {
            foreach (var d in layers)
            {
                foreach (var l in d)
                {
                    l.Value.Update();
                }
            }
        }

        public static void Initialize()
        {
            foreach (var d in layers)
            {
                foreach (var l in d)
                {
                    l.Value.Initialize();
                }
            }
        }

        internal static void Setup()
        {
            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
            layers = new Dictionary<string, SceneLayer>[(byte)SceneLayering.Count];
            for (byte i = 0; i < (byte)SceneLayering.Count; i++)
            {
                layers[i] = new Dictionary<string, SceneLayer>();
            }
        }

        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (behindTiles)
                DrawLayer(SceneLayering.BehindTiles_BehindNPCs);
            else
                DrawLayer(SceneLayering.BehindNPCs);
            orig(self, behindTiles);
            if (behindTiles)
                DrawLayer(SceneLayering.BehindTiles_InfrontNPCs);
            else
                DrawLayer(SceneLayering.InfrontNPCs);
        }

        internal static void Unload()
        {
            if (layers != null)
            {
                string failedLayer = "";
                byte failedStep = 0;
                try
                {
                    failedStep = 0;
                    foreach (var dictionary in layers)
                    {
                        failedStep = 1;
                        foreach (var layer in dictionary)
                        {
                            failedStep = 2;
                            failedLayer = layer.Key;
                            failedStep = 3;
                            layer.Value.Unload();
                        }
                        failedStep = 4;
                    }
                    failedStep = 5;
                }
                catch
                {
                    if (AQMod.Instance != null && AQMod.Instance.Logger != null)
                    {
                        var l = AQMod.Instance.Logger;
                        l.Error("Couldn't fully unload scene layers");
                        l.Error("Failed Step: " + failedStep);
                        l.Error("Failed Layer: " + failedLayer);
                    }
                }
                layers = null;
            }
        }
    }
}