using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using MelonLoader;
using VRC;
using VRC.Core;
using VRC.Management;

namespace CryoMod
{
	public class CryoMod : MelonMod
	{
		static readonly string ConfigFile = "UserData\\CryoModConfig.json";
		static readonly string BlacklistFile = "UserData\\blacklist.txt";
		static List<string> Blacklist = new List<string>();
		static ConfigClass Config;
		public override void OnApplicationStart()
		{
			if (!File.Exists(ConfigFile))
			{
				File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(new ConfigClass
				{
					BlockUser = false,
					HideUser = true
				}));
			}
			Config = JsonConvert.DeserializeObject<ConfigClass>(File.ReadAllText(ConfigFile));
			if (!File.Exists(BlacklistFile))
			{
				File.Create(BlacklistFile);
			}
			Blacklist = File.ReadAllLines(BlacklistFile).ToList();
			MelonLogger.Msg("VRC Started. \nCryoMod Initialized");
		}

		public void SetupHooks()
		{
			try
			{
				NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_0.Method_Public_Void_UnityAction_1_T_1((Action<Player>)delegate (Player player)
				{
					OnPlayerJoin(player);
				});
				NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_1.Method_Public_Void_UnityAction_1_T_1((Action<Player>)delegate (Player player)
				{
					OnPlayerLeave(player);
				});
			}
			catch (Exception ex) { MelonLogger.Error($"{ex.Message}\n{ex.StackTrace}"); }
		}

		public static void OnPlayerJoin(Player player)
		{
			Config = JsonConvert.DeserializeObject<ConfigClass>(File.ReadAllText(ConfigFile));
			Blacklist = File.ReadAllLines("UserData\\blacklist.txt").ToList();
			if (Blacklist.Contains(player.field_Private_APIUser_0.id))
			{
				if (Config.BlockUser)
				{
					ModerationManager.prop_ModerationManager_0.Method_Private_Void_String_ModerationType_Action_1_ApiPlayerModeration_Action_1_String_0(player.field_Private_APIUser_0.id, ApiPlayerModeration.ModerationType.Block);
				}
				if (Config.HideUser)
				{
					ModerationManager.prop_ModerationManager_0.Method_Private_Void_String_ModerationType_Action_1_ApiPlayerModeration_Action_1_String_0(player.field_Private_APIUser_0.id, ApiPlayerModeration.ModerationType.HideAvatar);
				}
			}
		}

		public static void OnPlayerLeave(Player player)
		{

		}

        public override void VRChat_OnUiManagerInit()
		{
			SetupHooks();
		}
    }
}
