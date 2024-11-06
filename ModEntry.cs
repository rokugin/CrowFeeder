using CrowFeeder.Integration;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using CrowFeeder.Patches;
using HarmonyLib;
using StardewValley;

namespace CrowFeeder;

internal class ModEntry : Mod {

    ISpaceCoreApi spaceCoreApi = null!;
    static ModConfig Config = new();
    public static IMonitor SMonitor = null!;

    public static LogLevel DesiredLog => Config.Logging ? LogLevel.Info : LogLevel.Trace;

    public override void Entry(IModHelper helper) {
        I18n.Init(helper.Translation);
        Config = helper.ReadConfig<ModConfig>();
        SMonitor = Monitor;
        
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
            prefix: new HarmonyMethod(typeof(FarmPatch), nameof(FarmPatch.AddCrows_Prefix))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        SetupGMCM();
        spaceCoreApi = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore")!;
        if (spaceCoreApi == null) {
            Monitor.Log("\nFatal Error: Cannot get SpaceCore Api, make sure SpaceCore is properly installed.", LogLevel.Error);
            return;
        }
        spaceCoreApi.RegisterSerializerType(typeof(Buildings.CrowFeeder));
        Monitor.Log($"\n{typeof(Buildings.CrowFeeder).AssemblyQualifiedName}", LogLevel.Info);
    }

    private void SetupGMCM() {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

        configMenu.AddSectionTitle(ModManifest, I18n.ConfigLogging);
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.Logging,
            value => Config.Logging = value,
            I18n.ConfigEnabled,
            I18n.ConfigLogging_Tooltip
        );
    }

}