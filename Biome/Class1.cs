/* first orders of business...
 *  Core Syntax for Harmony
 *  Injecting Harmony
 *  Using a setting from Mod Settings
 *  Pointing to a different XML
 * 
 * 
 * 
 */

using System;
using Verse;
using Harmony;
using System.Reflection;

namespace Biome
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
    //Harmony patches
    /*
     * Use a different Biome type (defined in XML) depending on the biome and an options setting
     *  1. find where rimworld chooses the biome to use
     *      MapGenerator -> GenerateMap((IntVec3 mapSize, MapParent parent, MapGeneratorDef mapGenerator, IEnumerable<GenStepWithParams> extraGenStepDefs = null, Action<Map> extraInitBeforeContentGen = null)
     *          MapParent looks promising.  If i find where this is called
     *          BiomeDef-> BiomeDef Named(string defName)
     *  2. Rebalancing plant amounts
     *      find a place to run through all the plants in a biome
     *      plugging them into a Get() to find if they are vanilla or VGP or Biomes
            then multiplying by a slider amount
            EXCLUDING GRASS
        ANSWER BiosphereCore
            Has sliders setup and cycles through all biomes editing values
     */
    [HarmonyPatch(typeof(WindowStack))]
    [HarmonyPatch("Add")]
    [HarmonyPatch(new Type[] { typeof(Window) })]
    class Patch
    {
        static void Prefix(Window window)
        {
            Log.Warning("Window: " + window);
        }
    }
}
