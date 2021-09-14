using BepInEx;
using System;
using UnityEngine;

namespace LordAshes
{
    public partial class HeightAdjustPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// Handler for Stat Messaging subscribed messages.
        /// </summary>
        /// <param name="changes"></param>
        public void HandleRequest(StatMessaging.Change[] changes)
        {
            foreach (StatMessaging.Change change in changes)
            {
                if (change.action != StatMessaging.ChangeType.removed) { SetRequest(change.cid, change.value); }
            }
        }

        /// <summary>
        /// Handler for Radial Menu selections
        /// </summary>
        /// <param name="cid"></param>
        private void SetRequest(CreatureGuid cid, string offsets)
        {
            CreatureBoardAsset asset = null;
            CreaturePresenter.TryGetAsset(cid, out asset);
            if(asset!=null)
            {
                string floor = StatMessaging.ReadInfo(asset.Creature.CreatureId, HeightAdjustPlugin.Guid+".Floor");
                string[] offsetsArray = offsets.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < offsetsArray.Length; i++)
                {
                    Debug.Log("Applying Creature " + StatMessaging.GetCreatureName(asset.Creature) + " (" + asset.Creature.CreatureId + ") Height of " + offsetsArray[i]+" with floor at "+floor);
                    if (!allowDig) { if (float.Parse(offsetsArray[i]) < float.Parse(floor)) { offsetsArray[i] = floor; } }
                    asset.CreatureLoaders[i].transform.position = new Vector3(asset.CreatureLoaders[i].transform.position.x, float.Parse(offsetsArray[i]), asset.CreatureLoaders[i].transform.position.z);
                }
                float delta = Math.Abs(float.Parse(offsetsArray[0]) - float.Parse(floor));
                Renderer renderer = asset.BaseLoader.LoadedAsset.GetComponent<Renderer>();
                Debug.Log("Creature "+StatMessaging.GetCreatureName(asset.Creature) + "(" + asset.Creature.CreatureId + ") "+((delta <= 0.1) ? "Showing " : "Hiding") + " Base");
                if (renderer != null) { renderer.enabled = (delta <= 0.1); }
                int seek = asset.transform.GetInstanceID();
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                {
                    if(go.name.Contains("Indicator"))
                    {
                        Transform parent = go.transform;
                        for (int i = 0; i < 10; i++)
                        {
                            try
                            {
                                parent = parent.parent;
                                if (parent.GetInstanceID() == seek)
                                {
                                    Debug.Log("Creature " + StatMessaging.GetCreatureName(asset.Creature) + "(" + asset.Creature.CreatureId + ") " + ((delta <= 0.1) ? "Showing " : "Hiding") + " Ring");
                                    renderer = go.GetComponentInChildren<Renderer>();
                                    if (renderer != null) { renderer.enabled = (delta <= 0.1); }
                                    break;
                                }
                            }
                            catch (Exception) { break; }
                        }
                    }
                }
            }
        }
    }
}
