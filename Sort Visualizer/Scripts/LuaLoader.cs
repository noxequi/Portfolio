using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter; 

public class LuaLoader : MonoBehaviour
{
    private Script luaScript;

    void Start()
    {
        string luaFilePath = Application.dataPath + "/Scripts/script.lua";
        string luaScriptContent = System.IO.File.ReadAllText(luaFilePath);

        luaScript = new Script();
        luaScript.DoString(luaScriptContent);

        DynValue function = luaScript.Globals.Get("LuaFunctionName");
        luaScript.Call(function);
    }
}
