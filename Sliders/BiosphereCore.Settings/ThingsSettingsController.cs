using UnityEngine;
using Verse;

//GENERIC SLIDER METHODS, nothing important to see
//Causes the updates in BiomeUtil to trigger on closing the options menu
namespace BiosphereCore.Settings
{
	public class ThingsSettingsController : Mod
	{
		public ThingsSettingsController(ModContentPack content)
			: base(content)
		{
			GetSettings<ThingsSettings>();
		}

		public override string SettingsCategory()
		{
			//an over-arching title?
			return "BC.ModSettingsTitle".Translate();
		}

		public override void DoSettingsWindowContents(Rect canvas)
		{
			BiomeUtil.Init();
			ThingsSettings.DoWindowContents(canvas);
		}

		public override void WriteSettings()
		{
			//Called when the Mod Settings Menu closes
			//Writing down the slider settings to remember for when the game starts up again
			base.WriteSettings();
			//updating all of the values based on the slider settings
			BiomeUtil.UpdateBiomeStatsPerUserSettings();
		}
	}
}
