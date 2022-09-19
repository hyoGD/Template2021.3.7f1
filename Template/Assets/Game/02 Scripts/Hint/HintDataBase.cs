using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using System.Globalization;
using System.Threading;

public class HintDataBase : MonoBehaviour
{
    [Header("       CONFIRM DATA FORM GOOGLE_SHEET          ")]
    public string docID = "";
    public int sheetID;
    public TextAsset data;

    [Header("-------------------->USE DATA<-----------------------")]
    private static List<Hint> hints = new List<Hint>();
    public static List<Hint> listHintData => hints;
    [SerializeField]private List<Hint> listHint;
    public Hint GetHint(int _index)
    {
        return listHintData[_index - 1];
    }

    private void Awake()
    {
        CultureInfo ci = new CultureInfo("en-us");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        if (data == null || string.IsNullOrEmpty(data.text))
        {
            LoadData();
        }
        else
        {
            ReadLocalData(data.text);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadData()
    {
        var data = CSVOnlineReader.ReadGSheet(docID, sheetID);
        if (data != null && data.Count > 0)
        {
            string sData = JsonConvert.SerializeObject(data);
            File.WriteAllText("Assets/Game/05 Resources/HintData.txt", sData);
         
          //  Debug.Log($"Level load succes {hints.Count}");
        }
        else { Debug.LogError("Level load failed"); }
    }

    //---------------------------------------------------------------------------
    public void ReadLocalData(string str)
    {
        hints.Clear();
        List<Dictionary<string, string>> lst = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(str);
        if (lst != null && lst.Count > 0)
        {

            foreach (var i in lst)
            {
                if (!string.IsNullOrEmpty(i["Level"]))
                {
                    Hint hint = new Hint(i);
                    hints.Add(hint);
                }

                hints[hints.Count-1].hintData.Add(i["HintData"]);
                hints[hints.Count - 1].hintDataEnglish.Add(i["Hint"]);
                hints[hints.Count - 1].hintDataJapan.Add(i["ヒント"]);
            }
            listHint = new List<Hint>(hints);
            //   Debug.Log($"Read LevelData success");
        }
    }
}

//---------------------------------------------------------------------------
#region Custom Inspector   
#if UNITY_EDITOR
[CustomEditor(typeof(HintDataBase))]
public class LoadDataFromGSheet : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HintDataBase control = (HintDataBase)target;
        if (GUILayout.Button("Load All Data from GSheet"))
        {
            control.LoadData();
            control.ReadLocalData(control.data.text);
        }
    }
}
#endif
#endregion

//---------------------------------------------------------------------------
[System.Serializable]
public class Hint
{
    #region Variables
    public int level;
   // public int amount;
    public List<string> hintData = new List<string>();
    public List<string> hintDataEnglish = new List<string>();
    public List<string> hintDataJapan = new List<string>();
    #endregion
    public Hint()
    {

    }

    public Hint(Dictionary<string, string> dict)
    {
      //  Debug.LogError(dict["Level"]);
        if (!string.IsNullOrEmpty(dict["Level"]))
        {
            level = int.Parse(dict["Level"]);
        }
    }

 
}


