using System.IO;

using Newtonsoft.Json.Linq;

using Terraria;
using Terraria.ModLoader;


namespace tModKoreanTranslator
{
    class MKTPlayer: ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            tModKoreanTranslator core = tModKoreanTranslator.instance;
            Main.NewText("통합 모드 번역 모드", 121, 134, 203, false);
            bool translateMode = ModContent.GetInstance<Config>().TranslatorModeDefault;
            if (ModContent.GetInstance<Config>().TerrariaPatcherDefault)
            {
                Main.NewText(@"클라이언트 번역: 테라리안 in Naver (https://cafe.naver.com/ztnc)", 121, 134, 203, false);
            }

            if (translateMode) Main.NewText("번역자 모드 활성화", 121, 134, 203, false);
            Main.NewText("", 121, 134, 203, false);
            foreach (MKTMOD MKTmod in core.MKTmods)
            {
                if (MKTmod.Translator.Length == 0 && MKTmod.Inspector.Length == 0) Main.NewText($"[{MKTmod.DisplayName} v{MKTmod.Version}]", 159, 168, 218, false);
                if (MKTmod.Translator.Length > 0 && MKTmod.Inspector.Length == 0) Main.NewText($"[{MKTmod.DisplayName} v{MKTmod.Version}] 번역: {MKTmod.Translator}", 159, 168, 218, false);
                if (MKTmod.Translator.Length > 0 && MKTmod.Inspector.Length > 0) Main.NewText($"[{MKTmod.DisplayName} v{MKTmod.Version}] 번역: {MKTmod.Translator}, 검수: {MKTmod.Inspector}", 159, 168, 218, false);
                if (MKTmod.mod.Version.ToString() != MKTmod.Version)
                    Main.NewText($"모드 {MKTmod.mod.DisplayName} v{MKTmod.mod.Version.ToString()}의 번역파일 버전이 v{MKTmod.Version}입니다 ", 159, 168, 218, false);
            }
            if (translateMode)
            {
                foreach (Mod mod in ModLoader.Mods)
                {
                    if (mod.Name == "ModLoader") continue;
                    if (mod.Name == "tModKoreanTranslator") continue;
                    if (!Directory.Exists(Path.Combine(Main.SavePath, "Korean Localization", mod.Name)))
                    {
                        new MKTMOD(mod).Dump();
                        Main.NewText($"(번역자 모드) [{mod.DisplayName} v{mod.Version.ToString()}] 새로운 번역 파일 생성", 159, 168, 218, false);
                    }
                }
            }
        }
    }
}
