using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Text;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;


namespace tModKoreanTranslator
{
    class tModKoreanTranslator : Mod
    {
        public static tModKoreanTranslator instance;
        public List<MKTMOD> MKTmods;
        public override void Load()
        {
            Directory.CreateDirectory(Path.Combine(Main.SavePath, "Korean Localization"));
            instance = this;
            MKTmods = new List<MKTMOD>();
            if (ModContent.GetInstance<Config>().TerrariaPatcherDefault)
            {
                LanguageManager languageManager = LanguageManager.Instance;
                foreach (TmodFile.FileEntry item in
                            typeof(Mod)
                            .GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(this) as TmodFile)
                {
                    if (Path.GetFileNameWithoutExtension(item.Name).StartsWith("ztnc.Terraria.Localization.Content") && item.Name.EndsWith(".json"))
                    {
                        try
                        {
                            languageManager.LoadLanguageFromFileText(Encoding.UTF8.GetString(GetFileBytes(item.Name)));
                        }
                        catch
                        {
                            Logger.InfoFormat("Failed to load language file: " + item);
                        }
                    }
                }
                //ModLanguageManager manager = new ModLanguageManager(this);
                //foreach (LocalizedText localized in LanguageManager.Instance.FindAll(new Regex(".+")))
                //{
                //    string[] key = localized.Key.Split('.');
                //    manager.Add(key[1], key[0], localized.Value, true);
                //}
                //manager.DumpTo("Terraria");
            }
            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod.Name == "ModLoader") continue;
                if (mod.Name == "tModKoreanTranslator") continue;
                if (Directory.Exists(Path.Combine(Main.SavePath, "Korean Localization", mod.Name)))
                {
                    MKTMOD MKTmod = new MKTMOD(mod).Load();
                    MKTmods.Add(MKTmod);
                }
            }
        }
    }
    class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Label("Translator Mode")]
        [Tooltip("Translate File Dump and Update")]
        public bool TranslatorModeDefault { get; set; }

        [DefaultValue(false)]
        [Label("Terraria Patcher")]
        [Tooltip("Terraria & tModLoader Text Translate")]
        public bool TerrariaPatcherDefault { get; set; }
    }
}
