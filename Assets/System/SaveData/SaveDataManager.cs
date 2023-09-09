using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Taki.TakiAESJsonSave;


public class SaveDataManager : SimgletonMonoBehaviour<SaveDataManager>
{
    public SaveDataManager() { }

    //[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        GameObject _ga = new GameObject("SaveDataManager", typeof(SaveDataManager));

        DontDestroyOnLoad(_ga);
    }

    public SaveData _saveData { get; private set; }
    private string _fileName = "SAVEDATA";

    protected override void Awake()
    {
        base.Awake();

        _saveData = new SaveData();
    }

    public async Task LoadSaveData()
    {
        _saveData = await SaveDataIO.LoadPlayerDataAsync<SaveData>(_fileName);
        if (_saveData == null)
        {
            _saveData = new SaveData();
        }
    }

    public async Task SaveSaveData()
    {
        await SaveDataIO.SavePlayerDataAsync<SaveData>(_saveData, _fileName);
    }
}

