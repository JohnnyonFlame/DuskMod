using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using System.Reflection;
using UnityEngine;

[ModEntryPoint]
public static class Stickies
{
    public static void Main()
    {
        var harmony = HarmonyInstance.Create("com.github.johnnyonflame.stickies");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(DestructibleObjectScript))]
    [HarmonyPatch("OnDestroy")]
    class BasicAIScript_OnDestroy_Patch
    {
        static bool Prefix(DestructibleObjectScript __instance)
        {
            if (__instance.tag == "EnemyTag")
            {
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    var tr = __instance.transform.GetChild(i);
                    var buzz = tr.gameObject.GetComponent<BuzzSawHitScript>();
                    if (buzz != null)
                    {
                        buzz.die();
                    }
                }
            }

            return true;
        }
    }
   
    [HarmonyPatch(typeof(BuzzSawHitScript))]
    [HarmonyPatch("OnCollisionEnter")]
    [HarmonyPatch(new Type[] { typeof(Collision) })]
    class BuzzSawHitScript_OnCollisionEnter_Patch
    {
        static bool Prefix(BuzzSawHitScript __instance, Collision hit)
        {
            if (hit.transform.gameObject.tag == "EnemyTag")
            {
                __instance.transform.parent = hit.transform.gameObject.transform;
                __instance.GetComponent<Rigidbody>().velocity = new Vector3();
                __instance.GetComponent<Rigidbody>().useGravity = false;
                __instance.GetComponent<Rigidbody>().detectCollisions = false;
                return false;
            }
            else if ((DestructibleObjectScript)hit.transform.gameObject.GetComponent(typeof(DestructibleObjectScript)) == null)
            {
                __instance.GetComponent<Rigidbody>().velocity = new Vector3();
                __instance.GetComponent<Rigidbody>().useGravity = false;
                __instance.GetComponent<Rigidbody>().detectCollisions = false;
                return false;
            }
            return true;
        }
    }
}