using System;
using System.Collections.Generic;
using LitJson;
namespace ChivoxAIEngine
{
public static class Callback
{
private class Message
{
public IEvalResultListener MListener;
public WeakReference MWeakEngine;
public string MPath;
}
private static readonly Dictionary<string, Message> STokenIDMap = new Dictionary<string, Message>();
public static void RegisterListener(string tokenID, IEvalResultListener listener)
{
if (STokenIDMap.ContainsKey(tokenID))
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return;
}
message.MListener = listener;
}
else
{
Message message = new Message
{
MListener = listener
};
STokenIDMap.Add(tokenID, message);
}
}
public static void RegisterEngine(string tokenID, Engine engine)
{
if (STokenIDMap.ContainsKey(tokenID))
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return;
}
message.MWeakEngine = new WeakReference(engine);
}
else
{
Message message = new Message
{
MWeakEngine = new WeakReference(engine)
};
STokenIDMap.Add(tokenID, message);
}
}
public static void RegisterPath(string tokenID, string path)
{
if (STokenIDMap.ContainsKey(tokenID))
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return;
}
message.MPath = path;
}
else
{
Message message = new Message
{
MPath = path
};
STokenIDMap.Add(tokenID, message);
}
}
public static void Unregister(string tokenID)
{
STokenIDMap.Remove(tokenID);
}
private static Message TryGetMessage(string tokenID)
{
Message message;
STokenIDMap.TryGetValue(tokenID, out message);
if (message == null)
{
return null;
}
return message;
}
public static IEvalResultListener TryGetListener(string tokenID)
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return null;
}
return message.MListener;
}
public static Engine TryGetEngine(string tokenID)
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return null;
}
if (message.MWeakEngine == null)
{
return null;
}
if (!message.MWeakEngine.IsAlive)
{
return null;
}
Engine engine = message.MWeakEngine.Target as Engine;
if (engine == null)
{
return null;
}
return engine;
}
public static string TryGetPath(string tokenID)
{
Message message = TryGetMessage(tokenID);
if (message == null)
{
return null;
}
return message.MPath;
}
private static readonly List<EvalResult> SResultList = new List<EvalResult>();
[AOT.MonoPInvokeCallback(typeof(Chivoxd161509b47344be06dc395657bb1ec1b.Chivox5192fcf9c412a141a1f50a57e24c8510))]
public static int AIEngineCallback(IntPtr usrdata, string id, int type, byte[] data, int size)
{
EvalResult result = new EvalResult(usrdata, id, type, data, size);
result.RecFilePath = TryGetPath(result.TokenID);
lock (SResultList)
{
SResultList.Add(result);
}
return 0;
}
public static void StaticUnityUpdateResultList()
{
lock (SResultList)
{
foreach (EvalResult result in SResultList)
{
string tokenID = result.TokenID;
IEvalResultListener listener = TryGetListener(tokenID);
if (listener != null)
{
DoCallback(result, listener);
}
if (result.IsLast)
{
Engine engine = TryGetEngine(tokenID);
if (engine != null)
{
engine.StopTokenID(tokenID);
}
Unregister(tokenID);
}
}
SResultList.Clear();
}
}
private static void DoCallback(EvalResult result, IEvalResultListener listener)
{
if (result.Text == null)
{
listener.OnBinResult(result.TokenID, result);
}
else
{
JsonData jsonResult = JsonMapper.ToObject(result.Text);
if (jsonResult.ContainsKey("error") || jsonResult.ContainsKey("errId") || jsonResult.ContainsKey("errID"))
{
listener.OnError(result.TokenID, result);
}
else if (jsonResult.ContainsKey("vad_status"))
{
listener.OnVad(result.TokenID, result);
}
else if (jsonResult.ContainsKey("sound_intensity"))
{
listener.OnSoundIntensity(result.TokenID, result);
}
else
{
listener.OnEvalResult(result.TokenID, result);
}
}
}
}
}
