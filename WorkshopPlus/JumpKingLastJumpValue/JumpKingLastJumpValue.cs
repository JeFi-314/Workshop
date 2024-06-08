﻿using HarmonyLib;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKingLastJumpValue.Menu;
using JumpKingLastJumpValue.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace JumpKingLastJumpValue
{
    [JumpKingMod(IDENTIFIER)]
    public static class JumpKingLastJumpValue
    {
        const string IDENTIFIER = "Phoenixx19.LastJumpValue";
        const string HARMONY_IDENTIFIER = "Phoenixx19.LastJumpValue.Harmony";
        const string SETTINGS_FILE = "Phoenixx19.LastJumpValue.Settings.xml";

        public static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            // set path for dll
            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // try reading config file
            try
            {
                Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
                Preferences = new Preferences();
            }

            // add save on property changed
            Preferences.PropertyChanged += SaveSettingsOnFile;

            // setup harmony
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            // patching on each class (is better than attributes)
            new JumpChargeCalc(harmony);
            new GameLoopDraw(harmony);
        }

        #region Menu Items
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleLastJumpValue ToggleLJV(object factory, GuiFormat format)
        {
            return new ToggleLastJumpValue();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static LastJumpValueOption Option(object factory, GuiFormat format)
        {
            return new LastJumpValueOption();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleGaugeDisplay ToggleGD(object factory, GuiFormat format)
        {
            return new ToggleGaugeDisplay();
        }
        #endregion

        private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            try
            {
                XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            }
        }
    }
}
