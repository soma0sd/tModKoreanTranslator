using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO.Compression;


using Newtonsoft.Json.Linq;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace tModKoreanTranslator
{
    class MKTCore
    {
        /* 공통함수
         * 
         * JObject LoadJSON(string): JSON 파일을 읽어 JObject로 출력
         */
        public static string translatorPath = Path.Combine(Main.SavePath, "Korean Localization");
        //internal static JObject Tr2LangCol2(JObject rawdata)
        //{
        //}
        internal static JObject Tr2LangCol3(JObject rawdata, bool rFalg=true)
        {
            JObject json = new JObject();
            foreach (KeyValuePair<string, JToken> iLevel in rawdata)
            {
                string key1 = iLevel.Key;
                JObject val1 = iLevel.Value.ToObject<JObject>();
                foreach (KeyValuePair<string, JToken> fLevel in val1)
                {
                    string key2 = fLevel.Key;
                    JObject val2 = fLevel.Value.ToObject<JObject>();
                    string txt = val2.Value<string>("ko");
                    if (txt == "") txt = val2.Value<string>("en");
                    if (rFalg)
                    {
                        if (json[key2] == null) json.Add(key2, JObject.Parse($@"{{ {key1}: "" {txt} "" }}"));
                        else json.Value<JObject>(key2).Add(key1, txt);
                    }
                    else
                    {
                        if (json[key1] == null) {
                            JObject _j = new JObject();
                            _j.Add(key2, txt);
                            json.Add(key1, _j);
                        }
                        else { json.Value<JObject>(key1).Add(key2, txt); }
                    }
                }
            }
            return json;
        }
        internal static JObject LoadJSON(string path, bool compress = false)
        {
            if (compress)
            {
                using (StreamReader r = new StreamReader(path))
                {
                    byte[] compressed = Convert.FromBase64String(r.ReadToEnd());
                    using (MemoryStream decomStream = new MemoryStream(compressed))
                    {
                        using (GZipStream gStream = new GZipStream(decomStream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(gStream))
                            {
                                return JObject.Parse(reader.ReadToEnd());
                            }
                        }
                    }
                }

            }
            else
            {
               try
               {
                    using (StreamReader r = new StreamReader(path))
                    {
                        return JObject.Parse(r.ReadToEnd());
                    }
               }
               catch
               {
                    return null;
               }
            }
        }
        internal static void DumpJSON(string path, JObject json, bool compress=false)
        {
            if (compress)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(json.ToString());
                byte[] compressed;
                using (var outStream = new MemoryStream())
                {
                    using (GZipStream gStream = new GZipStream(outStream, CompressionMode.Compress))
                    {
                        gStream.Write(encoded, 0, encoded.Length);
                    }
                    compressed = outStream.ToArray();
                }
                File.WriteAllText(path, System.Convert.ToBase64String(compressed));
            }
            else
            {
                File.WriteAllText(path, json.ToString());
            }
        }
        internal static Dictionary<string, T> FieldItems<T>(string fieldName, Mod mod)
        {
            return (Dictionary<string, T>)typeof(Mod).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mod);
        }
    }
    class Config : ModConfig
    {
        /* 모드 설정
         * (bool) Translator Mode: [false] 번역자 모드
         * (bool) Terraria Patcher: [true] 테라리아 클라이언트 번역
         * (bool) FontChage: [true] 한글 지원 폰트로 변경
         */
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Label("Translator Mode")]
        [Tooltip("Translate File Dump and Update")]
        public bool TranslatorMode { get; set; }

        [DefaultValue(true)]
        [Label("Terraria Patcher")]
        [Tooltip("Terraria & tModLoader Text Translate. Requires a Reload")]
        [ReloadRequired]
        public bool TerrariaPatcher { get; set; }

        [DefaultValue(true)]
        [Label("Font Override")]
        [Tooltip("Overriding Font for Korean [Nanum Pen]")]
        [ReloadRequired]
        public bool FontChage { get; set; }
    }
}
