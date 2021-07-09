﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnhollowerBaseLib;
using System.Runtime.InteropServices;

namespace GunfireRebornMods
{
    public class ModManager : MonoBehaviour
    {
        public ModManager(IntPtr intPtr) : base(intPtr) { }
        public List<ModBase> Mods = new List<ModBase>();
        public GUILayoutOption[] GUILayoutOption = new GUILayoutOption[0];
        unsafe void OnGUI()
        {
            foreach (var mod in Mods) if (mod.Enabled) mod.OnGUI();
            if (Cursor.lockState == CursorLockMode.Locked) return;
            var area = new Rect(25, 25, 150, 250);
            GUI.Box(area, "shalzuth's mods");
            GUILayout.BeginArea(area);
            GUILayout.Space(12);
            foreach (var mod in Mods)
            {
                var val = GUILayout.Toggle(mod.Enabled, mod.GetType().Name, GUILayoutOption);
                if (val != mod.Enabled)
                {
                    if (val) mod.OnEnable();
                    else mod.OnDisable();
                    mod.Enabled = val;
                }
                if (mod.Enabled && mod.HasConfig) mod.SliderVal = GUILayout.DoHorizontalSlider(mod.SliderVal, mod.SliderMin, mod.SliderMax, new GUIStyle(GUI.skin.horizontalSlider), new GUIStyle(GUI.skin.horizontalSliderThumb), GUILayoutOption);
                if (mod.Enabled) mod.OnGUI();
            }
            GUILayout.EndArea();
        }
        void Update()
        {
            foreach (var mod in Mods) if (mod.Enabled) mod.Update();
        }
        void OnDisable()
        {
            foreach (var mod in Mods) if (mod.Enabled) mod.OnDisable();
        }
    }
}