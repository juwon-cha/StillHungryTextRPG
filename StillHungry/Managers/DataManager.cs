using Newtonsoft.Json;
using StillHungry.Controller;
using StillHungry.Data;
using System.Diagnostics;
using System.IO;

namespace StillHungry.Managers
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeData();
    }

    public class DataManager
    {
        const string DATA_PATH = "../../../../Data";

        // json 데이터를 담고있는 딕셔너리는 프로그램 전역에서 접근 가능
        public static Dictionary<EClassType, PlayerStat> PlayerStatDict { get; private set; } = new Dictionary<EClassType, PlayerStat>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
        public static Dictionary<string, DungeonData> DungeonDataDict { get; private set; } = new Dictionary<string, DungeonData>();
        public static Dictionary<int, UserData> SaveDataDict { get; private set; } = new Dictionary<int, UserData>();

        public static void SaveUserData(UserData data, string path)
        {
            UserDataLoader dataToSave = new UserDataLoader();

            // UserData 리스트에 현재 유저 데이터를 추가
            dataToSave.UserData.Add(data);

            string jsonData = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
            File.WriteAllText($"{DATA_PATH}/{path}.json", jsonData);
        }

        public static void LoadUserData()
        {
            SaveDataDict = LoadJson<UserDataLoader, int, UserData>("SaveData/UserData").MakeData();
        }

        public static void LoadGameData()
        {
            PlayerStatDict = LoadJson<PlayerStatLoader, EClassType, PlayerStat>("GameData/PlayerStat").MakeData();
            ItemDict = LoadJson<ItemLoader, int, ItemData>("GameData/ItemData").MakeData();
            DungeonDataDict = LoadJson<DungeonDataLoader, string, DungeonData>("GameData/DungeonData").MakeData();
        }

        // 제약 조건: T 타입으로 들어올 수 있는 클래스는 반드시 ILoader<Key, Value>와 매개변수가 없는 기본 생성자(new T())를 가지고 있어야 함
        public static T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>, new()
        {
            string filePath = $"{DATA_PATH}/{path}.json";

            // 파일 존재 여부 체크
            if (!File.Exists(filePath))
            {
                // 예외 발생 대신 빈 Loader 객체를 반환
                return new T();
            }

            string text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
            {
                // 파일은 있지만 내용이 비어있는 경우
                return new T();
            }

            // json 역직렬화
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
