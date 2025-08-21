using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using SkyFrost.Base;

namespace ImNotAsking;

public class ImNotAsking : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.1";
	public override string Name => "ImNotAsking";
	public override string Author => "Delta";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/lill-la/ImNotAsking";

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> HideAsk = new("HideAsk", "Hide the 'Ask to join' button on the contacts list", () => true);

	internal static ModConfiguration Config;

	public override void OnEngineInit() {
		Config = GetConfiguration();
		Config.Save(true);

		Harmony harmony = new("la.lill.ImNotAsking");
		harmony.PatchAll();
	}

	[HarmonyPatch(typeof(ContactItem), "Update", [typeof(Contact), typeof(ContactData)])]
	class ContactItemUpdatePatch {
		public static void Postfix(ContactItem __instance, Contact contact, ContactData data) {
			if (Config.GetValue(HideAsk)) {
				if (contact.IsAccepted && contact.ContactStatus == ContactStatus.Accepted) {
					SessionInfo sessionInfo = data?.CurrentSessionInfo;
					if (sessionInfo == null && data != null) {
						OnlineStatus? onlineStatus = data.CurrentStatus.OnlineStatus;
						if (!((onlineStatus.GetValueOrDefault() == OnlineStatus.Offline) & (onlineStatus != null))) {
							__instance._joinButton.Target.Slot.ActiveSelf = false;
						}
					}
				}
			}
		}
	}
}
