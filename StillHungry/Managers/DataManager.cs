using Newtonsoft.Json;
using StillHungry.Controller;
using StillHungry.Data;

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
        public static Dictionary<int, MonsterStat> MonsterStatDict { get; private set; } = new Dictionary<int, MonsterStat>();
        public static Dictionary<int, QuestData> QuestDataDict { get; private set; } = new Dictionary<int, QuestData>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
        public static Dictionary<int, DungeonData> DungeonDataDict { get; private set; } = new Dictionary<int, DungeonData>();
        public static Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();

        // 현재 플레이 중인 유저의 데이터
        public static UserData CurrentUser { get; set; }
        // 모든 슬롯의 데이터를 담는 딕셔너리
        public static Dictionary<int, UserData> UserSlots { get; private set; } = new Dictionary<int, UserData>();

        // 플레이어가 선택한 슬롯에 현재 유저 데이터 저장
        public static void SaveCurrentUserData(int slotIndex)
        {
            if (CurrentUser == null) return;

            CurrentUser.SlotID = slotIndex;

            // 전체 슬롯 데이터 불러오기
            // 특정 슬롯만 저장하더라도, 다른 슬롯의 데이터를 날리지 않기 위해 UserData.json 전체를 먼저 읽어온다.
            string path = $"{DATA_PATH}/SaveData/UserData.json";
            UserDataLoader loader;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                loader = JsonConvert.DeserializeObject<UserDataLoader>(json);
            }
            else
            {
                loader = new UserDataLoader();
            }

            // 기존 슬롯 데이터가 있으면 제거하고, 현재 플레이어 데이터(CurrentUser)를 추가
            // 현재 슬롯 데이터 찾아서 교체 또는 추가
            var existingData = loader.UserData.FirstOrDefault(u => u.SlotID == slotIndex);
            if (existingData != null)
            {
                loader.UserData.Remove(existingData);
            }
            loader.UserData.Add(CurrentUser);

            // 파일에 저장
            // 수정된 C# 객체를 다시 JSON 문자열로 변환 (직렬화)
            string jsonData = JsonConvert.SerializeObject(loader, Formatting.Indented);

            // 파일에 덮어쓰기
            File.WriteAllText(path, jsonData);

            // 메모리에 있는 슬롯 데이터도 갱신
            LoadAllSlotsData();
        }

        public static bool LoadUserDataFromSlot(int slotIndex)
        {
            LoadAllSlotsData();
            if (UserSlots.TryGetValue(slotIndex, out UserData userData))
            {
                CurrentUser = userData;
                return true;
            }
            return false;
        }

        public static void LoadAllSlotsData()
        {
            UserSlots = LoadJson<UserDataLoader, int, UserData>("SaveData/UserData").MakeData();
        }

        public static void LoadGameData()
        {
            // Newtonsoft.Json 라이브러리가 JSON의 키(key)와 Loader 클래스의 프로퍼티(변수) 이름을 비교하여,
            // 일치하는 곳에 JSON 데이터를 자동으로 채워 넣는다.
            // LoadJson<...>(...)을 호출해서 Loader 객체를 생성하고,
            // 그 후 MakeData() 메서드가 List에 담긴 데이터를 게임에서 사용하기 편한 Dictionary 형태로 변환하여 최종 저장
            PlayerStatDict = LoadJson<PlayerStatLoader, EClassType, PlayerStat>("GameData/PlayerStat").MakeData();
            MonsterStatDict = LoadJson<MonsterStatLoader, int, MonsterStat>("GameData/MonsterStat").MakeData();
            QuestDataDict = LoadJson<QuestDataLoader, int, QuestData>("GameData/QuestData").MakeData();
            ItemDict = LoadJson<ItemLoader, int, ItemData>("GameData/ItemData").MakeData();
            DungeonDataDict = LoadJson<DungeonDataLoader, int, DungeonData>("GameData/DungeonData").MakeData();
            SkillDict = LoadJson<SkillDataLoader, int, SkillData>("GameData/SkillData").MakeData();
        }

        // 제약 조건: T 타입으로 들어올 수 있는 클래스는 반드시 ILoader<Key, Value>와 매개변수가 없는 기본 생성자(new T())를 가지고 있어야 함
        // 예를 들어, T는 ItemLoader, Key는 int, Value는 ItemData가 된다
        public static T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>, new()
        {
            // 파일 경로 조합
            string filePath = $"{DATA_PATH}/{path}.json";

            // 파일 존재 여부 체크
            if (!File.Exists(filePath))
            {
                // 예외 발생 대신 빈 Loader 객체를 반환
                return new T();
            }

            // 파일의 모든 텍스트 읽어옴
            string text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
            {
                // 파일은 있지만 내용이 비어있는 경우
                return new T();
            }

            // json 역직렬화
            // json 텍스트를 T 타입(예를 들어, ItemLoader) 객체로 역직렬화
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
