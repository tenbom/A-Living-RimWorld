using RimWorld.Planet;

//Causing the updates in BiomeUtils to trigger on game-load

namespace BiosphereCore
{
	internal class WorldComp : WorldComponent
	{
		//I guess this method is just tagging along to get at FinalizeInit()
		public WorldComp(World world)
			: base(world)
		{
		}

		public override void FinalizeInit()
		{
			//REPLACE WITH HARMONY
			// insert in map gen, so instead of changing every plant ratio in every biome, simply changes it for the 1 being used.
			base.FinalizeInit();
			BiomeUtil.Init();
			BiomeUtil.UpdateBiomeStatsPerUserSettings();
		}
	}
}
