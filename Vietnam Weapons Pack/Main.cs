using System.Reflection;
using Harmony;
using UnityEngine;

namespace VietnamWeaponsPack
{
    public class Main : VTOLMOD
    {
        public override void ModLoaded()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("danku.canaryweapons");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            VTOLAPI.SceneLoaded += SceneLoaded;
            base.ModLoaded(); 
            
        }
        
        private void SceneLoaded(VTOLScenes scene)
        {
            Debug.Log("the f14 mod is being developed by the elusive 'AirStriker10' and don't let anybody tell you otherwise THEY'RE LYING DONT LISTE");
        }
    }
}