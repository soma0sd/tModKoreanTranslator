using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;


namespace tModKoreanTranslator
{
    public class MKTMOD
    {
        private Mod _mod;

        private Dictionary<string, ModLanguageManager> data;

        public Mod mod { get { return _mod; } }
        public string DisplayName {get{ return _mod.DisplayName; } }
        public string Version
        {
            get
            {
                if (data["meta"].Meta("Version") == null) return null;
                return data["meta"].Meta("Version");
            }
        }
        public string Translator
        {
            get
            {
                if (data["meta"].Meta("Translator") == null) return null;
                return $@"{data["meta"].Meta("Translator")}";
            }
        }
        public string Inspector
        {
            get
            {
                if (data["meta"].Meta("Inspector") == null) return null;
                return $@"{data["meta"].Meta("Inspector")}";
            }
        }

        public MKTMOD(Mod mod)
        {
            this._mod = mod;
            this.data = new Dictionary<string, ModLanguageManager>();
        }
        public void Dump()
        {
            this.data.Add("meta", new ModLanguageManager(mod));
            this.data.Add("translation", new ModLanguageManager(mod));
            this.data.Add("item", new ModLanguageManager(mod));
            this.data.Add("buff", new ModLanguageManager(mod));
            this.data.Add("npc", new ModLanguageManager(mod));
            this.data.Add("prefix", new ModLanguageManager(mod));
            Dictionary<string, ModTranslation> translations = new FieldItems<ModTranslation>("translations", _mod).Get;
            Dictionary<string, ModItem> items = new FieldItems<ModItem>("items", _mod).Get;
            Dictionary<string, ModProjectile> projectiles = new FieldItems<ModProjectile>("projectiles", _mod).Get;
            Dictionary<string, ModBuff> buffs = new FieldItems<ModBuff>("buffs", _mod).Get;
            Dictionary<string, ModNPC> npcs = new FieldItems<ModNPC>("npcs", _mod).Get;
            Dictionary<string, ModPrefix> prefixes = new FieldItems<ModPrefix>("prefixes", _mod).Get;
            data["meta"].Add("Mod", _mod.DisplayName);
            data["meta"].Add("Version", _mod.Version.ToString());
            data["meta"].Add("Translator", "");
            data["meta"].Add("Inspector", "");
            data["translation"].Add("_Desc", "모드가 지원하는 지역화 텍스트입니다");
            data["translation"].Add("_지역화 키워드", "영문 텍스트", "번역 텍스트");
            data["item"].Add("_Desc", "모드 아이템, 모드 발사체");
            data["item"].Add("표시 텍스트 종류", "_아이템 내부명칭", "영문 텍스트", "번역 텍스트");
            data["buff"].Add("_Desc", "모드 버프/디버프");
            data["buff"].Add("표시 텍스트 종류", "_버프 내부명칭", "영문 텍스트", "번역 텍스트");
            data["npc"].Add("_Desc", "모드 NPC");
            data["npc"].Add("표시 텍스트 종류", "_NPC 내부명칭", "영문 텍스트", "번역 텍스트");
            data["prefix"].Add("_Desc", "모드 접두어");
            data["prefix"].Add("접두어", "_내부명칭", "영문 텍스트", "번역 텍스트");
            foreach (KeyValuePair<string, ModTranslation> p in translations)
            {
                if (p.Key.Contains("#")) continue;
                data["translation"].Add(p.Key.Replace(" ",""), p.Value.GetDefault(), "");
            }
            foreach (KeyValuePair<string, ModItem> p in items)
            {
                data["item"].Add("ItemName", p.Key.Replace(" ", ""), p.Value.DisplayName.GetDefault(), "");
                data["item"].Add("ItemTooltip", p.Key.Replace(" ", ""), p.Value.Tooltip.GetDefault(), "");
            }
            foreach (KeyValuePair<string, ModProjectile> p in projectiles)
            {
                data["item"].Add("ProjectileName", p.Key.Replace(" ", ""), p.Value.DisplayName.GetDefault(), "");
            }
            foreach (KeyValuePair<string, ModBuff> p in buffs)
            {
                data["buff"].Add("BuffName", p.Key.Replace(" ", ""), p.Value.DisplayName.GetDefault(), "");
                data["buff"].Add("BuffDescription", p.Key.Replace(" ", ""), p.Value.Description.GetDefault(), "");
            }
            foreach (KeyValuePair<string, ModNPC> p in npcs)
            {
                data["npc"].Add("NPCName", p.Key.Replace(" ", ""), p.Value.DisplayName.GetDefault(), "");
                if (p.Value.CanChat())
                {
                    //data["npc"].Add("Chat1", p.Key, p.Value.GetChat().ToString(), "");
                    //data["npc"].Add("Chat2", p.Key, p.Value.GetChat(), "");
                }
            }
            foreach (KeyValuePair<string, ModPrefix> p in prefixes)
            {
                data["prefix"].Add("Prefix", p.Key.Replace(" ", ""), p.Value.DisplayName.GetDefault(), "");
            }
            data["meta"].DumpTo("_Meta");
            if (data["translation"].Length > 2) data["translation"].DumpTo("Translations");
            if (data["item"].Length > 2) data["item"].DumpTo("Items");
            if (data["buff"].Length > 2) data["buff"].DumpTo("Buffs");
            if (data["npc"].Length > 2) data["npc"].DumpTo("NPCs");
            if (data["prefix"].Length > 2) data["prefix"].DumpTo("Prefixes");


        }
        public MKTMOD Load()
        {
            string dirPath = Path.Combine(ModLanguageManager.exportDir, _mod.Name);
            this.data.Add("meta", new ModLanguageManager(_mod).LoadFrom("_Meta"));
            if (File.Exists(Path.Combine(dirPath, "Translations.json")))
            {
                this.data.Add("translation", new ModLanguageManager(_mod));
                data["translation"].LoadFrom("Translations").LocalizationT();
            }
            if (File.Exists(Path.Combine(dirPath, "Items.json")))
            {
                this.data.Add("item", new ModLanguageManager(_mod));
                data["item"].LoadFrom("Items").LocalizationI();
            }
            if (File.Exists(Path.Combine(dirPath, "Buffs.json")))
            {
                this.data.Add("buff", new ModLanguageManager(_mod));
                data["buff"].LoadFrom("Buffs").LocalizationI();
            }
            if (File.Exists(Path.Combine(dirPath, "NPCs.json")))
            {
                this.data.Add("npc", new ModLanguageManager(_mod));
                data["npc"].LoadFrom("NPCs").LocalizationI();
            }
            if (File.Exists(Path.Combine(dirPath, "Prefixes.json")))
            {
                this.data.Add("prefix", new ModLanguageManager(_mod));
                data["prefix"].LoadFrom("Prefixes").LocalizationI();
            }
            return this;
        }
        private class FieldItems<T>
        {
            public FieldItems(string fieldName, Mod mod)
            {
                this.Get =
                    (Dictionary<string, T>)typeof(Mod).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mod);
            }
            public Dictionary<string, T> Get { get; }
        }
    }
    public class ModLanguageManager
    {
        public readonly static string exportDir = Path.Combine(Main.SavePath, "Korean Localization");

        private JObject _json;
        private Mod _mod;
        public ModLanguageManager(Mod mod)
        { this._json = new JObject(); this._mod = mod; }
        public ModLanguageManager(Mod mod, JObject json)
        { this._json = json; this._mod = mod; }
        public override string ToString()
        { return _json.ToString(); }
        public int Length { get { return _json.Count; } }
        public string Meta(string metaKey)
        {
            return _json.Value<string>(metaKey).ToString();
        }
        public string Value(string key)
        {
            if (_json[key] == null) return null;
            if (_json.Value<JObject>(key).Value<string>("ko").Length > 0) return _json.Value<JObject>(key).Value<string>("ko");
            return _json.Value<JObject>(key).Value<string>("en");
        }
        public string Value(string kind, string name)
        {
            if (_json[name] == null) return null;
            if (_json.Value<JObject>(name).Value<JObject>(kind) == null) return null;
            if (_json.Value<JObject>(name).Value<JObject>(kind).Value<string>("ko").Length > 0) return _json.Value<JObject>(name).Value<JObject>(kind).Value<string>("ko");
            return _json.Value<JObject>(name).Value<JObject>(kind).Value<string>("en");
        }
        public void Add(string key, string english, string korean)
        {
            if (_json[key] == null)
            {
                _json.Add(key, JObject.FromObject(new { en = $@"{english}", ko = $@"{korean}" }));
            }
            else
            {
                _json[key] = JObject.FromObject(new { en = $@"{english}", ko = $@"{korean}" });
            }
        }
        public void Add(string kind, string name, string english, string korean)
        {
            if (_json[name] == null) _json.Add(name, new JObject());
            if (_json[name][kind] == null)
            {
                _json.Value<JObject>(name).Add(kind, JObject.FromObject(new { en = $@"{english}", ko = $@"{korean}" }));
            }
            else
            {
                _json[name][kind] = JObject.FromObject(new { en = $@"{english}", ko = $@"{korean}" });
            }

        }
        public void Add(string kind, string name, string value, bool flag)
        {
            if (_json[name] == null) _json.Add(name, new JObject());
            if (_json[name][kind] == null)
            {
                _json.Value<JObject>(name).Add(kind, $@"{value}");
            }
            else
            {
                _json[name][kind] = $@"{value}";
            }

        }
        public void Add(string meta, string desc)
        {
            if (_json[meta] == null)
            {
                _json.Add(meta, desc);
            }
            else
            {
                _json[meta] = desc;
            }
        }
        public void DumpTo(string fileName)
        {
            Directory.CreateDirectory(Path.Combine(exportDir, _mod.Name));
            File.WriteAllText(Path.Combine(exportDir, _mod.Name, fileName + ".json"), ToString());
        }
        public void LocalizationT()
        {
            var field = (Dictionary<string, ModTranslation>)typeof(Mod).GetField("translations", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_mod);
            foreach (KeyValuePair<string, JToken> pair in _json)
            {
                if (pair.Key.StartsWith("_")) continue;
                ModTranslation translation = _mod.CreateTranslation($"{pair.Key}");
                string text = pair.Value.Value<string>("ko");
                if (text.Length == 0) text = pair.Value.Value<string>("en");
                translation.AddTranslation(Language.ActiveCulture, $@"{text}");
                if (field.ContainsKey(pair.Key)) field[pair.Key].AddTranslation(Language.ActiveCulture, $@"{text}");
                _mod.AddTranslation(translation);
            }
        }
        public void LocalizationI()
        {
            foreach (KeyValuePair<string, JToken> pairItem in _json)
            {
                if (pairItem.Key.StartsWith("_")) continue;
                string name = pairItem.Key;
                foreach (KeyValuePair<string, JToken> pairProp in _json.Value<JObject>(name))
                {
                    string kind = pairProp.Key;
                    string text = pairProp.Value.Value<string>("ko");
                    if (text.Length == 0) text = pairProp.Value.Value<string>("en");
                    ModTranslation translation = _mod.CreateTranslation($"{kind}.{name}");
                    translation.AddTranslation(Language.ActiveCulture, $@"{text}");
                    _mod.AddTranslation(translation);
                }
            }
        }
        public ModLanguageManager LoadFrom(string fileName)
        {
            using (StreamReader r = new StreamReader(Path.Combine(exportDir, _mod.Name, fileName + ".json")))
            {
                return new ModLanguageManager(_mod, JObject.Parse(r.ReadToEnd()));
            }
        }
    }
}
