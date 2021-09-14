using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;


namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    [BepInDependency(LordAshes.StatMessaging.Guid)]
    public partial class HeightAdjustPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Height Adjust Plug-In";
        public const string Guid = "org.lordashes.plugins.heightadjust";
        public const string Version = "1.0.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut> triggerTransformKey { get; set; }
        private ConfigEntry<KeyboardShortcut> triggerUpKey { get; set; }
        private ConfigEntry<KeyboardShortcut> triggerDownKey { get; set; }

        System.Guid subscription = System.Guid.Empty;

        private bool allowDig = false;


        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            // Not required but good idea to log this state for troubleshooting purpose
            UnityEngine.Debug.Log("Template Plugin: Lord Ashes Template Plugin Is Active.");

            // The Config.Bind() format is category name, setting text, default
            triggerTransformKey = Config.Bind("Hotkeys", "Apply After Board Load", new KeyboardShortcut(KeyCode.T, KeyCode.RightControl));
            triggerUpKey = Config.Bind("Hotkeys", "Adjust Mini Up", new KeyboardShortcut(KeyCode.Plus, KeyCode.LeftControl));
            triggerDownKey = Config.Bind("Hotkeys", "Adjust Mini Down", new KeyboardShortcut(KeyCode.Minus, KeyCode.LeftControl));
            allowDig = Config.Bind("Settings", "Allow digging (height adjustment into the ground)", false).Value;

            Utility.PostOnMainPage(this.GetType());
        }

        void OnGUI()
        {
            if (Utility.isBoardLoaded())
            {
                CreatureBoardAsset asset = null;
                CreaturePresenter.TryGetAsset(LocalClient.SelectedCreatureId, out asset);
                if (asset != null)
                {
                    string floor = StatMessaging.ReadInfo(asset.Creature.CreatureId, HeightAdjustPlugin.Guid + ".Floor");
                    if (floor != "")
                    {
                        float delta = asset.CreatureLoaders[0].transform.position.y - float.Parse(floor);
                        if (delta > 0.1)
                        {
                            GUIStyle guiStyle1 = new GUIStyle();
                            guiStyle1.fontSize = 32;
                            guiStyle1.normal.textColor = Color.black;
                            GUIStyle guiStyle2 = new GUIStyle();
                            guiStyle2.fontSize = 32;
                            guiStyle2.normal.textColor = Color.yellow;
                            GUI.Label(new Rect((1920 / 2) - (100 / 2)-2, 38, 100, 120), Math.Round(delta * 5.0f) + " Feet", guiStyle1);
                            GUI.Label(new Rect((1920 / 2) - (100 / 2), 40, 100, 120), Math.Round(delta * 5.0f) + " Feet", guiStyle2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            if (Utility.isBoardLoaded())
            {
                if (Utility.StrictKeyCheck(triggerDownKey.Value) || Utility.StrictKeyCheck(triggerUpKey.Value))
                {
                    float offset = Utility.StrictKeyCheck(triggerUpKey.Value) ? 0.2f : -0.2f;
                    CreatureBoardAsset asset = null;
                    CreaturePresenter.TryGetAsset(LocalClient.SelectedCreatureId, out asset);
                    string offsets = "";
                    if(asset!=null)
                    {
                        if (StatMessaging.ReadInfo(asset.Creature.CreatureId, HeightAdjustPlugin.Guid + ".Floor") == "")
                        {
                            StatMessaging.SetInfo(asset.Creature.CreatureId, HeightAdjustPlugin.Guid + ".Floor", asset.CreatureLoaders[0].transform.position.y.ToString());
                        }
                        foreach (AssetLoader cl in asset.CreatureLoaders)
                        {
                            Debug.Log("Requesting Creature " + StatMessaging.GetCreatureName(asset.Creature) + " (" + asset.Creature.CreatureId + ") Height From " + cl.transform.position.y + " to " + (cl.transform.position.y + offset));
                            offsets = offsets + (cl.transform.position.y + offset)+",";
                        }
                    }
                    StatMessaging.SetInfo(asset.CreatureId, HeightAdjustPlugin.Guid, offsets);
                }

                if(Utility.StrictKeyCheck(triggerTransformKey.Value))
                {
                    Debug.Log("Applying Height Adjustments");
                    if (subscription!=System.Guid.Empty)
                    {
                        StatMessaging.Unsubscribe(subscription);
                    }
                    StatMessaging.Reset(HeightAdjustPlugin.Guid + ".Floor");
                    StatMessaging.Reset(HeightAdjustPlugin.Guid);
                    subscription = StatMessaging.Subscribe(HeightAdjustPlugin.Guid+".Floor", HandleRequest);
                    subscription = StatMessaging.Subscribe(HeightAdjustPlugin.Guid, HandleRequest);
                }
            }
        }
    }
}
