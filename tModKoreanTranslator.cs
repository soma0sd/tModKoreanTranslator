using System;
using System.IO;
using System.Collections.Generic;

using ReLogic.Graphics;

using Terraria;
using Terraria.ModLoader;


namespace tModKoreanTranslator
{
    class tModKoreanTranslator : Mod
    {
        /* 모드 객체
         * 
         * (tModKoreanTranslator) instance: Load() 이후의 클래스 참조
         * (string) translatorPath: 모드 번역파일 경로
         */
        public static tModKoreanTranslator instance;
        public List<MKTMod> MKTmods;

        public tModKoreanTranslator() { }
        public override bool LoadResource(string path, int length, Func<Stream> getStream)
        {
            // 설정에 따라 폰트를 로드
            string extension = Path.GetExtension(path).ToLower();
            if (extension == ".xnb" && path.StartsWith("Fonts/"))
            {
                if (ModContent.GetInstance<Config>().FontChage)
                {
                    return base.LoadResource(path, length, getStream);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.LoadResource(path, length, getStream);
            }
        }
        public override void Load()
        {
            // 모드 초기화
            instance = this;
            Directory.CreateDirectory(MKTCore.translatorPath);
            if (ModContent.GetInstance<Config>().FontChage) {
                DynamicSpriteFont MKTPFontMouse = GetFont("Fonts/MKT_MouseText");
                DynamicSpriteFont MKTPItemStack = GetFont("Fonts/MKT_ItemStack");
                DynamicSpriteFont MKTFontDeath = GetFont("Fonts/MKT_DeathText");
                Main.fontMouseText = MKTPFontMouse; // 툴팁, 채팅
                Main.fontItemStack = MKTPItemStack; // 업적, UI 제목
                Main.fontDeathText = MKTFontDeath;  // 타이틀 메뉴
                //Main.fontCombatText[0] = MKTPFontMouse;
                //Main.fontCombatText[1] = MKTPFontMouse;
            }

            // 설치한 모드 탐색
            this.MKTmods = new List<MKTMod>();
            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod.Name == "ModLoader") continue;
                if (mod.Name == this.Name) continue;
                this.MKTmods.Add(new MKTMod(mod));
            }

            // 클라이언트 번역
            if (ModContent.GetInstance<Config>().TerrariaPatcher) new MKTTerraria();
        }
        public override void Unload()
        {
            instance = null;
            MKTmods = null;
        }
    }
}
