using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SlabManBuff{
    class AssetManager{
        private const string assetBundlePath = "src.assets.slabmanbuff";
        private static AssetBundle assetBundle = null;
        private static Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();

        public static void LoadAssetBundle(){
            Assembly ass = Assembly.GetExecutingAssembly();
            using(Stream str = ass.GetManifestResourceStream($"{ass.GetName().Name}.{assetBundlePath}")){
                assetBundle = AssetBundle.LoadFromStream(str);
            }
        }

        public static T LoadAsset<T>(string name) where T:Object{
            if(!assetBundle) return null;

            if(!loadedAssets.ContainsKey(name)){
                T asset = assetBundle.LoadAsset<T>(name);
                loadedAssets.Add(name, asset);
                return asset;
            }else{
                return (T) loadedAssets[name];
            }
        }
    }
}