using System;
using System.Collections.Generic;
using Verse;
using Harmony;
using RimWorld;
using System.Reflection;

//adding tweaks to methods in the assemblies, either Prefix (before the actual function runs) or Postfix(after it runs)
namespace BiosphereCore
{
    //starting up Harmony.
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.github.harmony.rimworld.mod.example");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
    }

    [HarmonyPatch(typeof(WildPlantSpawner))]        //the class the functiion to be tweaked is in
    [HarmonyPatch("CalculatePlantsWhichCanGrowAt")] //the name of the function to tweak
    internal class BS_CalculatePlantsWhichCanGrowAt
    {
        private static void Postfix(WildPlantSpawner __instance, IntVec3 c, List<ThingDef> outPlants)
        {
            //WildPlantSpawner __instance = Oprional first argument in a Pre/Postfix.  If you want to be able to access the variables of the class WildPlantSpawner
            //IntVec3 c, List<ThingDef> outPlants = Optional second set of arguments.  Listed, in order, arguments of the original "CalculatePlantsWhichCanGrowAt" function

            //map is a private variable.  To access it I have to do shenanigans
            TerrainDef terrain = Traverse.Create(__instance).Field("map").GetValue<Map>().terrainGrid.TerrainAt(c);
            
            //going through all the tags the terrain can get.
            //outPlants is a list of all the plants that can be planted on the terrain.

            //first, if there should be no plants, clear the outPlants
            if (terrain.HasTag("noPlants"))
            {
                //Log.Message(terrain.defName + "noPlants:" + outPlants.Count);
                outPlants.Clear();
                return;
            }
            // second, if there should only be tagged plants, clear the outPlants, then add in the valid tagged plants.
            if (terrain.HasTag("onlyTaggedPlants"))
            {
                outPlants.Clear();
                foreach(string plant in Globals.allowedPlantsInTerrain.Keys)
                {
                    if (Globals.allowedPlantsInTerrain[plant].Contains(terrain.defName))
                    {
                        outPlants.Add(DefDatabase<ThingDef>.GetNamed(plant));
                    }
                }
            }
            else
            {
            //third, if there should be all plants that fit the fertility along with tagged plants....
            //remove any tagged plant that doesn't have specifically this terrain's tag.
                foreach (string plant in Globals.allowedPlantsInTerrain.Keys)
                {
                    if (!Globals.allowedPlantsInTerrain[plant].Contains(terrain.defName))
                    {
                        for (int i =0; i< outPlants.Count; i++)
                        {
                            if(outPlants[i].defName == plant)
                            {
                                outPlants.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            
        }
    }
}
