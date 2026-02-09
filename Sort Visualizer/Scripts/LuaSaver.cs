using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LuaSaver : MonoBehaviour
{
    public TMP_InputField inputField;

    public void SaveLuaCode()
    {
        string luaCode = inputField.text;

        string directoryPath = Path.Combine(Application.streamingAssetsPath, "LuaScripts");
        string filePath = Path.Combine(directoryPath, "myScript.lua");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(filePath, luaCode);
        //Debug.Log("Lua code saved to: " + filePath);
    }

}
