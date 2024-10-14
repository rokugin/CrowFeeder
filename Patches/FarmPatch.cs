using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;
using CrowFeeder.Buildings;
using StardewValley.Extensions;
using StardewValley.BellsAndWhistles;

namespace CrowFeeder.Patches;

static class FarmPatch {

    public static bool AddCrows_Prefix(Farm __instance) {
        List<Buildings.CrowFeeder> feeders = new();
        foreach (var v in __instance.buildings) {
            if (v is Buildings.CrowFeeder) {
                feeders.Add((Buildings.CrowFeeder)v);
            }
        }
        if (feeders.Count < 1) return true;

        int numCrops = 0;
        foreach (KeyValuePair<Vector2, TerrainFeature> pair in __instance.terrainFeatures.Pairs) {
            if (pair.Value is HoeDirt { crop: not null }) {
                numCrops++;
            }
        }

        int crows = 0;
        int potentialCrows = Math.Min(4, numCrops / 16);
        for (int i = 0; i < potentialCrows; i++) {
            if (!(Game1.random.NextDouble() < 0.3)) {
                continue;
            }

            for (int attempts = 0; attempts < 10; attempts++) {
                if (!Utility.TryGetRandom(__instance.terrainFeatures, out var tile, out var feature) ||
                    !(feature is HoeDirt dirt) || dirt.crop?.currentPhase.Value <= 1) {
                    continue;
                }
                Buildings.CrowFeeder feeder = Game1.random.ChooseFrom(feeders);
                feeder.crowsFed.Value++;
                SpawnCrow(__instance, new Vector2(feeder.tileX.Value, feeder.tileY.Value));
                crows++;
                break;
            }
        }
        ModEntry.SMonitor.Log($"Crows spawned: {crows}", ModEntry.DesiredLog);
        return false;
    }

    static void SpawnCrow(GameLocation location, Vector2 pos) {
        if (location.critters == null && location.IsOutdoors) {
            location.critters = new List<Critter>();
        }

        Critter crow = new Crow((int)pos.X, (int)pos.Y);
        crow.position += new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-16, 16));
        location.critters!.Add(crow);
    }

}