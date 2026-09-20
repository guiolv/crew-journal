// CrewJournal — save JSON via DataContractJsonSerializer (sem UnityEngine).
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CrewJournal.Logic
{
    public static class SaveSystem
    {
        public static string Serialize(GameData d)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GameData));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, d);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static GameData Deserialize(string json)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GameData));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (GameData)ser.ReadObject(ms);
            }
        }

        public static void SaveToFile(GameData d, string path)
        {
            File.WriteAllText(path, Serialize(d), Encoding.UTF8);
        }

        public static GameData LoadFromFile(string path)
        {
            return Deserialize(File.ReadAllText(path, Encoding.UTF8));
        }
    }
}
