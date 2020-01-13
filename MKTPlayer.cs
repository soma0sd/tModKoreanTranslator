using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;

namespace tModKoreanTranslator
{
    class MKTPlayer: ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            if (ModContent.GetInstance<Config>().TerrariaPatcher)
            {
                Main.NewText($"클라이언트 번역 제공: 테라리안 in Naver", 243, 229, 245);
                Main.NewText($"           https://cafe.naver.com/ztnc", 243, 229, 245);
            }
            List<MKTMod> MKTmods = tModKoreanTranslator.instance.MKTmods;
            foreach (MKTMod mktmod in MKTmods)
            {
                MKTModMeta meta = mktmod.meta;
                if (mktmod.active)
                {
                    Main.NewText($"{meta.name} {meta.modVersion.ToString()}", 243, 229, 245);
                    if (ModContent.GetInstance<Config>().TranslatorMode)
                    {
                        // 번역자 모드가 켜져있는 경우
                        mktmod.update(); // debug
                        // 번역파일을 업데이트
                        if (mktmod.version.CompareTo(meta.modVersion) > 0)
                        {
                            mktmod.update();
                            Main.NewText($"   {meta.modVersion.ToString()} => {mktmod.version.ToString()} 업데이트 했습니다.", 243, 229, 245);
                        }
                        else if (mktmod.version.CompareTo(meta.modVersion) < 0)
                        {
                            Main.NewText($"   모드가 번역파일보다 구버전입니다.", 243, 229, 245);
                        }
                        else
                        {
                            Main.NewText($"    번역파일과 동일한 버전입니다.", 243, 229, 245);
                        }
                    }
                }
                else
                {
                    if (ModContent.GetInstance<Config>().TranslatorMode)
                    {
                        // 번역자 모드가 켜져있는 경우
                        // 번역파일이 없는 모드의 원시 번역파일 생성
                        mktmod.dump();
                        Main.NewText($"{meta.name}의 번역파일을 생성", 243, 229, 245);
                    }
                }
            }
        }
    }
}
