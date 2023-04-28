using System;
using LitJson;
namespace ChivoxAIEngine
{
public class EvalResult
{
public string TokenID;
public bool IsLast;
public string Text;
public byte[] Data;
public string RecFilePath;
public EvalResult(IntPtr usrdata, string id, int type, byte[] data, int size)
{
TokenID = AIEngineInternalUtility.StringFix(id);
if (type == Chivoxba38757dfb9abe4a02ec49ba6e4711c7.Chivoxf2598ad29f5796ae17edafd06648a552)
{
Data = data;
Text = AIEngineInternalUtility.StringFix(AIEngineInternalUtility.UTF8BytesToString(data, size));
JsonData jsonResult = JsonMapper.ToObject(Text);
if (jsonResult.ContainsKey("eof") && jsonResult["eof"].IsInt)
{
if ((jsonResult["eof"] as IJsonWrapper).GetInt() == 0)
{
IsLast = false;
}
else
{
IsLast = true;
}
}
else if (jsonResult.ContainsKey("vad_status") || jsonResult.ContainsKey("sound_intensity"))
{
IsLast = false;
}
else
{
IsLast = true;
}
}
else
{
Data = data;
Text = null;
IsLast = true;
}
}
}
}
