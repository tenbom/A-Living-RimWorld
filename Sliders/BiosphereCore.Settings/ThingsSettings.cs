using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;

// CORE OF THE SLIDER FUNCTIONALITY
namespace BiosphereCore.Settings
{
	public class ThingsSettings : ModSettings
	{
		private const int animalMaxPercent = 400;
		private const int animalMinPercent = 0;
		private const int plantMaxPercent = 400;
		private const int plantMinPercent = 0;
		private const int BiospherePlantMaxPercent = 400;
		private const int BiospherePlantMinPercent = 0;
		private const int VGPPlantMaxPercent = 400;
		private const int VGPPlantMinPercent = 0;

		public static int animalSlider, plantSlider, BioSlider, VGPSlider;

		public static int sliderStep = 10; // be carefull about changing, when you restart, the last slider values in Options will be wildly different

		public static void DoWindowContents(Rect canvas)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap();

			listing_Standard.Label("BC.animalDensityLevel".Translate() + "  " + string.Format("{0:F0}", animalSlider) + " %");
			animalSlider = (int)listing_Standard.Slider((animalSlider / sliderStep) * sliderStep, animalMinPercent, animalMaxPercent);

			listing_Standard.Gap();

			listing_Standard.Label("BC.plantDensityLevel".Translate() + "  " + string.Format("{0:F0}", plantSlider) + " %");
			plantSlider = (int)listing_Standard.Slider((plantSlider/sliderStep)*sliderStep, plantMinPercent, plantMaxPercent);

			listing_Standard.Gap();

			//listing_Standard.Label("Specific Mod Settings");

			listing_Standard.Label( "BC.ALRDensityLevel".Translate() + "  " + string.Format("{0:F0}", BioSlider) + " %");
			BioSlider = (int)listing_Standard.Slider((BioSlider / sliderStep) * sliderStep, BiospherePlantMinPercent, BiospherePlantMaxPercent);

			if (ModLister.HasActiveModWithName("VGP Vegetable Garden"))
			{
				//listing_Standard.Gap();

				listing_Standard.Label("BC.VGPDensityLevel".Translate() + "  " + string.Format("{0:F0}", VGPSlider) + " %");
				VGPSlider = (int)listing_Standard.Slider((VGPSlider / sliderStep) * sliderStep, VGPPlantMinPercent, VGPPlantMaxPercent);
			}

			listing_Standard.End();
		}

		public override void ExposeData()
		{
			// This lists the variables that should be saved even after the game is turned off. 
			// The mod settings should persist even after you restart the game. The last value (typically 100) is the default value if there is no saved value yet.
			base.ExposeData();
			Scribe_Values.Look(ref animalSlider, "animal_global_Density", 100);
			Scribe_Values.Look(ref plantSlider, "plant_global_Density", 100);
			Scribe_Values.Look(ref BioSlider, "plant_ALR_Density", 100);
			Scribe_Values.Look(ref VGPSlider, "plant_VGP_Density", 100);
		}
	}
}
