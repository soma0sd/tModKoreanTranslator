using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Terraria.ModLoader;
using Terraria.Localization;


namespace tModKoreanTranslator
{
    class MKTTerraria
    {
        // 테라리아 번역 클래스
        private string translatorPath = Path.Combine(MKTCore.translatorPath, "tr_Terraria");
        public MKTTerraria()
        {

            if (Directory.Exists(translatorPath))
            {
                LoadUserLocalization();
            }
            else
            {
                DumpUserLocalization();
                LoadUserLocalization();
            }

        }
        private void LoadUserLocalization()
        {
            LanguageManager manager = LanguageManager.Instance;
            bool trMode = ModContent.GetInstance<Config>().TranslatorMode;
            if (File.Exists(Path.Combine(MKTCore.translatorPath, "Terraria.mkt")) && trMode == false)
            {
                manager.LoadLanguageFromFileText(MKTCore.LoadJSON(Path.Combine(MKTCore.translatorPath, "Terraria.mkt"), true).ToString());
            }
            else if (File.Exists(Path.Combine(translatorPath, "Translations.json")))
            {
                JObject json = MKTCore.Tr2LangCol3(MKTCore.LoadJSON(Path.Combine(translatorPath, "Translations.json")), false);
                MKTCore.DumpJSON(Path.Combine(MKTCore.translatorPath, "Terraria.mkt"), json, true);
                manager.LoadLanguageFromFileText(json.ToString());
            }
        }
        private void DumpUserLocalization()
        {
            MKTModLang lang = new MKTModLang();
            JObject modLocal = JObject.Parse(Encoding.UTF8.GetString(tModKoreanTranslator.instance.GetFileBytes("Localization/ztnc.Terraria.Localization.json")));
            Directory.CreateDirectory(translatorPath);
            LocalizedText[] rawdata = LanguageManager.Instance.FindAll(new Regex(".+"));
            foreach (LocalizedText t in rawdata)
            {
                string[] keys = t.Key.Split('.');
                string key1 = keys[0];
                string key2 = keys[1];
                if (modLocal[key1] != null)
                {
                    if (modLocal[key1][key2] != null) { lang.Add2(key1, key2, t.Value, modLocal[key1].Value<string>(key2)); }
                    else { lang.Add(key1, key2, t.Value); }
                }
                else { lang.Add(key1, key2, t.Value); }

            }
            MKTCore.DumpJSON(Path.Combine(translatorPath, "Translations.json"), lang.json);
        }
    }
}
