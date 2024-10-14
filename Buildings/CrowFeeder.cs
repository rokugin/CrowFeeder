using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using System.Xml.Serialization;

namespace CrowFeeder.Buildings;

[XmlType("Mods_rokugin_CrowFeeder")]
public class CrowFeeder : Building {

    public readonly NetInt crowsFed = new NetInt(0);

    public readonly NetBool crowsFedToday = new NetBool(false);

    public CrowFeeder() : this(Vector2.Zero) { }

    public CrowFeeder(Vector2 tileLocation) : base("Crow Feeder", tileLocation) { }

    protected override void initNetFields() {
        base.initNetFields();
        base.NetFields.AddField(crowsFed, "crowsFed")
            .AddField(crowsFedToday, "crowsFedToday");
    }

    public override void dayUpdate(int dayOfMonth) {
        base.dayUpdate(dayOfMonth);
        if (GetData().ModData != null) {
            GetData().ModData.TryGetValue("rokugin.crowPrize.chance", out string? value);
            float.TryParse(value, out float chance);
            if (crowsFedToday.Value && Game1.random.NextDouble() < chance) {
                ItemRegistry.Create("(O)74");
            }
        }
        crowsFedToday.Value = false;
    }

    public override bool doAction(Vector2 tileLocation, Farmer who) {
        if (daysOfConstructionLeft.Value <= 0 && occupiesTile(tileLocation)) {
            if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true)) {
                Game1.drawObjectDialogue($"This building has fed {crowsFed.Value} crows.");
                return true;
            }
        }
        return base.doAction(tileLocation, who);
    }

}