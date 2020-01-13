using Newtonsoft.Json.Linq;


namespace tModKoreanTranslator
{
    class MKTModLang
    {
        public JObject json { get { return this._json; } }
        private JObject _json;
        public MKTModLang()
        {
            this._json = new JObject();
        }
        public MKTModLang(string path)
        {
            this._json = MKTCore.LoadJSON(path);
        }
        public string Value(string key)
        {
            // prefix, translations
            JObject desc = _json.Value<JObject>(key);
            if (desc.Value<string>("ko").Length > 0) return desc.Value<string>("ko");
            return desc.Value<string>("en");
        }
        public string Value(string itemName, string key)
        {
            // item, buff, npc
            JObject desc = _json.Value<JObject>(itemName).Value<JObject>(key);
            if (desc.Value<string>("ko").Length > 0) return desc.Value<string>("ko");
            return desc.Value<string>("en");
        }
        public void Add(string key, string text)
        {
            if (this._json[key] == null)
            {
                this._json.Add(key, Desc(text));
            }
            else
            {
                this._json[key] = Desc(text);
            }
        }
        public void Add(string itemName, string key, string text)
        {
            if (this._json[itemName] == null)
            {
                JObject item = new JObject();
                item.Add(key, Desc(text));
                this._json.Add(itemName, item);
            }
            else
            {
                this._json[itemName][key] = Desc(text);
            }
        }
        public void Add2(string itemName, string key, string english, string korean)
        {
            if (this._json[itemName] == null)
            {
                JObject item = new JObject();
                item.Add(key, Desc(english, korean));
                this._json.Add(itemName, item);
            }
            else
            {
                this._json[itemName][key] = Desc(english, korean);
            }
        }
        private JObject Desc(string english, string korean="")
        {
            JObject desc = new JObject();
            desc.Add("en", $@"{english}");
            desc.Add("ko", $@"{korean}");
            return desc;
        }
    }
}
