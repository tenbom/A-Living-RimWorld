using BiosphereCore.Settings;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Harmony;
using System.Reflection;
using System.Reflection.Emit;

//Using the slider information and other necessary processes
//runs after option menu closes and before a game starts

namespace BiosphereCore
{
	public static class Globals
	{
		//variables I want to be able to access in the Harmony file
		public static Dictionary<string, List<string>> allowedPlantsInTerrain;
	}

	internal static class BiomeUtil
	{
		private struct BiomeOriginalValues
		{
			// a place to store the original Biome values, so if someone goes into settings again and changes a ratio, I have the original values to multiply by the new ratio.
			public readonly float AnimalDensity;
			public readonly float PlantDensity;
			public readonly Dictionary<ThingDef, float> CachedPlantCommonalities;

			public BiomeOriginalValues(float animalDensity, float plantDensity, Dictionary<ThingDef, float> cachedPlantCommonalities)
			{
				AnimalDensity = animalDensity;
				PlantDensity = plantDensity;
				CachedPlantCommonalities = cachedPlantCommonalities;
			}
		}

		//creating a Dictionary of Biomes->Biome original values
		private static readonly Dictionary<BiomeDef, BiomeOriginalValues> BiomeStats = new Dictionary<BiomeDef, BiomeOriginalValues>();

		public static void Init()
		{
			if (BiomeStats.Count == 0)
			{
				//DefDatabase is stores every definition loaded into RimWorld.  You can sift through it by type, DefDatabase<BiomeDef> has only the BiomeDefs.
				//to get a list from DefDatabase, you tack on .AllDefs
				foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
				{
					//this causes the cache of plant commonalities to build
					biome.CommonalityOfPlant(new ThingDef());

					//once it is built, store it in the original value vault, along with a couple other values
					BiomeStats.Add(biome, new BiomeOriginalValues(biome.animalDensity, biome.plantDensity, Traverse.Create(biome).Field("cachedPlantCommonalities").GetValue<Dictionary<ThingDef, float>>()));
					

					//debug
					/*foreach (KeyValuePair<ThingDef, float> kvp in BiomeStats[allDef].CachedPlantCommonalities)
					{
						Log.Warning("Key = " + kvp.Key + " || Value = " + kvp.Value);
					}*/
				}
			}
		}

		//this is the function called when the options menu closes and at game load
		public static void UpdateBiomeStatsPerUserSettings()
		{
			Log.Warning("Begin UpdateBiomeStatsPerUserSettings");
			Init();
			doWork();
			Log.Warning("End UpdateBiomeStatsPerUserSettings");
		}
		public static void doWork()
		{
			//This function does all the heavy lifting.
			//at the moment that is.. 
			//	setting animal/plant densities based on sliders
			//	compiling the plant->terrains dictionary for the Harmony functions

			Globals.allowedPlantsInTerrain = new Dictionary<string, List<string>>();

			foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
			{
				//Log.Message("    " + allDef.defName);
				Dictionary<ThingDef, float> _cachedPlantCommonalities = new Dictionary<ThingDef, float>();	//a container of the same type that RimWorld uses to hold all of its plant->abundance information in each biome
				if (BiomeStats.TryGetValue(biome, out BiomeOriginalValues value))
				{
					// overall animal and plant densities are easy, the biome has an easy to access variable holding them
					biome.animalDensity = value.AnimalDensity * ThingsSettings.animalSlider / 100f;
					biome.plantDensity = value.PlantDensity * ThingsSettings.plantSlider / 100f;

					// cycle through each plant in the biome, do mod based functions and plant/terrain based functions
					foreach (ThingDef _thingDef in biome.AllWildPlants)
					{
						//building up the plant->terrains dictionary
						if(_thingDef.thingSetMakerTags.Count > 0)
						{
							//Log.Message("found plant: " + _thingDef.defName);
							foreach (string _tag in _thingDef.thingSetMakerTags)
							{
								//Log.Message("  adding tag: " + _tag);
								if (!Globals.allowedPlantsInTerrain.ContainsKey(_thingDef.defName))
								{
									Globals.allowedPlantsInTerrain.Add(_thingDef.defName, new List<string>());
								}
								Globals.allowedPlantsInTerrain[_thingDef.defName].Add(_tag);
							}
						}
						//applying mod specific sliders
						if (_thingDef.modContentPack.Name.Contains("VGP"))
						{
							//if the plant's tagged origin is a mod containing "VGP" in its name... give it the VGP treatment
							_cachedPlantCommonalities.Add(_thingDef, value.CachedPlantCommonalities[_thingDef] * ThingsSettings.VGPSlider / 100f);// * ThingsSettings.wildCropDensityLevel);
						}
						else if (_thingDef.modContentPack.Name.Contains("Biosphere"))
						{
							//if the plant's tagged origin is a mod containing "Biosphere" in its name... give it the Biosphere treatment
							_cachedPlantCommonalities.Add(_thingDef, value.CachedPlantCommonalities[_thingDef] * ThingsSettings.BioSlider / 100f);
						}
						else
						{
							//otherwise add an unedited copy of it to our container.  We are replacing the entire original list with a new edited list so we need every entry.  This is the only way....
							_cachedPlantCommonalities.Add(_thingDef, value.CachedPlantCommonalities[_thingDef]);
						}
					}
					//Klingon for "replacing the Biome's list of plants->abundance with our new edited one"
					Traverse.Create(biome).Field("cachedPlantCommonalities").SetValue(_cachedPlantCommonalities);

					//DEBUG, outputs the entire list of plants and their abundance to the debug log each time you close the mod settings
					/*
					foreach (KeyValuePair<ThingDef, float> kvp in Traverse.Create(allDef).Field("cachedPlantCommonalities").GetValue<Dictionary<ThingDef, float>>())
					{
						Log.Warning("Key = " + kvp.Key + " || Value = " + kvp.Value);
					}
					*/
				}
			}
		}
	}
}
