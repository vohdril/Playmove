using Newtonsoft.Json;

namespace Playmove.Util
{
	public static class JSONExtensions
    {
        public static T Read<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }
        
        public static T ParseFromObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        } 
        
        public static string StringfyFromObject<T>(T _object)
        {
            return JsonConvert.SerializeObject(_object);
        }

        public static void Write<T>(T model, string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(model));
        }

    }
}
