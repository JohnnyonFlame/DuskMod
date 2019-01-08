using System.Reflection;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class WaterMovementScript_Patch
{
    [HarmonyPatch(typeof(WaterMovementScript), "Update")]
    class WaterMovementScript_Update_Patch
    {
        static bool Prefix(WaterMovementScript __instance)
        {
            if (__instance.delaytimer <= (float)0 && __instance.dodelay)
            {
                MyControllerScript characontroller = __instance.p.GetComponent<MyControllerScript>();
                characontroller.inwater = false;
                characontroller.bunnyspeed = (float)0;
                //characontroller.gravityforce = 0.3f;
                //characontroller.CrouchState = false;
                __instance.waterexitsound.GetComponent<AudioSource>().Play();

                Image waterspriteimage = GameObject.Find("WaterSprite").GetComponent<Image>();
                Color newcol = waterspriteimage.color;
                newcol.a = 0.0f;
                waterspriteimage.color = newcol;

                __instance.underwatersound.active = false;
                __instance.bubbles.active = false;
                __instance.dodelay = false;
            }
            __instance.delaytimer -= Time.deltaTime;
            if (__instance.delaytimer < (float)0)
            {
                __instance.delaytimer = (float)0;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(WaterMovementScript), "OnTriggerEnter")]
    [HarmonyPatch(new[] { typeof(Collider) })]
    class WaterMovementScript_OnTriggerEnter_Patch
    {
        static bool Prefix(WaterMovementScript __instance, Collider hit)
        {
            if (hit.transform.gameObject.layer == 20)
            {
                MyControllerScript characontroller = __instance.p.GetComponent<MyControllerScript>();
                if (!characontroller.inwater)
                    __instance.waterentersound.GetComponent<AudioSource>().Play();

                characontroller.inwater = true;
                characontroller.bunnyspeed = (float)0;
                //characontroller.CrouchState = true;
                __instance.waterexitsound.GetComponent<AudioSource>().Play();

                Image waterspriteimage = GameObject.Find("WaterSprite").GetComponent<Image>();
                Color newcol = waterspriteimage.color;
                newcol.a = 0.6f;
                waterspriteimage.color = newcol;

                __instance.underwatersound.active = true;
                __instance.bubbles.active = true;
            }

            return false;
        }
    }
}