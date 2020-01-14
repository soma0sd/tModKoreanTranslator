using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Terraria.ModLoader;
using Terraria.Localization;

namespace tModKoreanTranslator
{
    class MKTMod
    {
        /* 개별 모드 정보 클래스
         * 
         * (Mod) mod: 모드 아이템
         * (bool) active: 번역파일 유무 확인
         * (string) Path: 모드 번역파일 디렉토리 
         * (Version) version: 모드 버전
         * (MKTModMeta) meta: 모드 메타
         * (MKTModLang) lang: 번역 키와 문자열
         */
        public Mod mod;
        public bool active = false;
        public string path;
        public Version version;
        public MKTModMeta meta;
        public Dictionary<string, MKTModLang> langs;

        public MKTMod(Mod mod)
        {
            this.mod = mod;
            this.version = mod.Version;
            this.path = Path.Combine(MKTCore.translatorPath, "tr_" + mod.Name);
            this.langs = new Dictionary<string, MKTModLang>();
            // 0.3.0 번역호환
            if (Directory.Exists(Path.Combine(MKTCore.translatorPath, mod.Name)))
            {
                try
                {
                    Directory.Move(Path.Combine(MKTCore.translatorPath, mod.Name), path);
                }
                catch
                {
                    // 실패시 수동변경
                }
            }
            if (File.Exists(Path.Combine(MKTCore.translatorPath, $"{mod.Name}.mkt")) && !ModContent.GetInstance<Config>().TranslatorMode)
            {
                this.active = true;
                this.meta = new MKTModMeta(MKTCore.LoadJSON(Path.Combine(MKTCore.translatorPath, $"{mod.Name}.mkt"), true).Value<JObject>("meta"));
                Load();
            }
            else if (File.Exists(Path.Combine(path, "_Meta.json")))
            {
                this.active = true;
                this.meta = new MKTModMeta(path);
                // 번역파일 로드
                Compile();
                Load();
            }
            else
            {
                this.meta = new MKTModMeta(mod);
            }
        }
        public void dump()
        {
            // 원시 번역파일 생성
            Directory.CreateDirectory(path);
            meta.Dump(path); // 메타파일 생성
            JObject temp = new JObject();
            JObject npc = new JObject();
            JObject item = new JObject();
            JObject buff = new JObject();
            JObject prefix = new JObject();
            var regex = new System.Text.RegularExpressions.Regex($"Mods[.]{mod.Name}[.].*");
            LocalizedText[] locals = LanguageManager.Instance.FindAll(regex);
            foreach (LocalizedText loc in locals)
            {
                if (loc.Key.Contains("#")) continue;
                string[] keys = loc.Key.Split('.'); // 0: Mods, 1: modName, 2: Kind, 3: internalName
                switch (keys[2])
                {
                    case "ItemName":
                    case "ItemTooltip":
                    case "MapObject":
                    case "ProjectileName":
                        AddLocaleItem(keys[2], keys[3], loc.Value, ref item);
                        break;
                    case "BuffName":
                    case "BuffDescription":
                        AddLocaleItem(keys[2], keys[3], loc.Value, ref buff);
                        break;
                    case "NPCName":
                        AddLocaleItem(keys[2], keys[3], loc.Value, ref npc);
                        break;
                    case "Prefix":
                        AddLocaleItem(keys[2], keys[3], loc.Value, ref prefix);
                        break;
                    default:
                        AddLocaleItem(loc.Key, loc.Value, ref temp);
                        break;

                }
            }
            if (temp.Count > 0) MKTCore.DumpJSON(Path.Combine(path, "Translations.json"), temp);
            if(item.Count > 0) MKTCore.DumpJSON(Path.Combine(path, "Items.json"), item);
            if(prefix.Count > 0) MKTCore.DumpJSON(Path.Combine(path, "Prefixes.json"), prefix);
            if(npc.Count > 0) MKTCore.DumpJSON(Path.Combine(path, "NPCs.json"), npc);
            if(buff.Count > 0) MKTCore.DumpJSON(Path.Combine(path, "Buffs.json"), buff);
        }
        public void Load()
        {
            // 번역파일 로드
            JObject json = MKTCore.LoadJSON(Path.Combine(MKTCore.translatorPath, $"{mod.Name}.mkt"), true);
            foreach (KeyValuePair<string, JToken> pair in json.Value<JObject>("items"))
            {
                ModTranslation translation = mod.CreateTranslation(pair.Key);
                translation.AddTranslation(Language.ActiveCulture, pair.Value.ToObject<string>());
                mod.AddTranslation(translation);
            }
        }
        public void Compile()
        {
            // 번역파일 묶기
            JObject json = new JObject();
            string[] itemFiles = new string[] { "Items.json", "Prefixes.json", "NPCs.json", "Buffs.json" };
            json.Add("meta", MKTCore.LoadJSON(Path.Combine(path, "_Meta.json")));
            if(File.Exists(Path.Combine(path, "Translations.json")))
                json.Add("items", CompJSON(MKTCore.LoadJSON(Path.Combine(path, "Translations.json")), true));
            foreach (string file in itemFiles)
            {
                if (File.Exists(Path.Combine(path, file)))
                json.Value<JObject>("items").Merge(CompJSON(MKTCore.LoadJSON(Path.Combine(path, file))));
            }
            MKTCore.DumpJSON(Path.Combine(MKTCore.translatorPath, $"{mod.Name}.mkt"), json, true);
        }
        public void update()
        {
            // 번역 파일 업데이트 (준비 중)
            // dump();
        }
        private void AddLocaleItem(string key, string value, ref JObject json)
        {
            key = key.Replace($"Mods.{mod.Name}.","");
            if (json[key] == null) json.Add(key, new JObject());
            json.Value<JObject>(key).Add("en", value);
            json.Value<JObject>(key).Add("ko", "");
        }
        private void AddLocaleItem(string kind, string name, string value, ref JObject json)
        {
            if (json[name] == null) json.Add(name, new JObject());
            json.Value<JObject>(name).Add(kind, new JObject());
            json.Value<JObject>(name).Value<JObject>(kind).Add("en", value);
            json.Value<JObject>(name).Value<JObject>(kind).Add("ko", "");
        }
        private JObject CompJSON(JObject json, bool translations=false)
        {
            JObject comp = new JObject();
            foreach (KeyValuePair<string, JToken> p1 in json)
            {
                if (p1.Key.StartsWith("_")) continue;
                if (translations)
                {
                    if (p1.Value.Value<string>("ko").Length > 0)
                    {
                        comp.Add(p1.Key, p1.Value.Value<string>("ko"));
                    }
                    else
                    {
                        comp.Add(p1.Key, p1.Value.Value<string>("en"));
                    }
                    continue;
                }
                foreach (KeyValuePair<string, JToken> p2 in p1.Value.ToObject<JObject>())
                {
                    if (p2.Value.Value<string>("ko").Length > 0)
                    {
                        comp.Add($"{p2.Key}.{p1.Key}", p2.Value.Value<string>("ko"));
                    }
                    else
                    {
                        comp.Add($"{p2.Key}.{p1.Key}", p2.Value.Value<string>("en"));
                    }
                }
            }
            return comp;
        }
    }
    class MKTModMeta
    {
        /* 모드번역도구 메타정보
         */
        public string name;
        public string translator = "";
        public string inspector = "";
        public Version modVersion = null;
        public Version MKTVersion = null; // null is v0.3.0

        public MKTModMeta(Mod mod)
        {
            this.name = mod.DisplayName;
            this.modVersion = mod.Version;
            this.MKTVersion = tModKoreanTranslator.instance.Version;
        }
        public MKTModMeta(string modPath)
        {
            JObject json = MKTCore.LoadJSON(Path.Combine(modPath, "_Meta.json"));
            this.name = json.Value<string>("Mod");
            this.translator = json.Value<string>("Translator");
            this.inspector = json.Value<string>("Inspector");
            this.modVersion = new Version(json.Value<string>("Version"));
            this.MKTVersion = new Version(json.Value<string>("MKT") != null? json.Value<string>("MKT"): "0.3.0");
        }
        public MKTModMeta(JObject json)
        {
            this.name = json.Value<string>("Mod");
            this.translator = json.Value<string>("Translator");
            this.inspector = json.Value<string>("Inspector");
            this.modVersion = new Version(json.Value<string>("Version"));
            this.MKTVersion = new Version(json.Value<string>("MKT") != null ? json.Value<string>("MKT") : "0.3.0");
        }
        public void Dump(string modPath)
        {
            JObject json = new JObject();
            json.Add("Mod", name);
            json.Add("Translator", translator);
            json.Add("Inspector", inspector);
            json.Add("Version", modVersion.ToString());
            json.Add("MKT", MKTVersion.ToString());
            MKTCore.DumpJSON(Path.Combine(modPath, "_Meta.json"), json);
        }
        
    }
}
