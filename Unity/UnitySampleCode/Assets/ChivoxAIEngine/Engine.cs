using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LitJson;
namespace ChivoxAIEngine
{
public class Engine
{
public static SDKInfo SDKInfo = new SDKInfo();
#region Create
public delegate void CreateSuccessCallback(Engine engine);
public delegate void CreateFailCallback(RetValue err);
private class CreateSuccessCallbackObject
{
public CreateSuccessCallback MCreateSuccessCallback;
public Engine MEngine;
}
private class CreateFailCallbackObject
{
public CreateFailCallback MCreateFailCallback;
public RetValue MRetValue;
}
private class CreateParam
{
public JsonData MCfg;
public CreateSuccessCallback MCreateSuccessCallback;
public CreateFailCallback MCreateFailCallback;
}
private static readonly List<CreateSuccessCallbackObject> SCreateSuccessCallbackObjects = new List<CreateSuccessCallbackObject>();
private static readonly List<CreateFailCallbackObject> SCreateFailCallbackObjects = new List<CreateFailCallbackObject>();
public static void Create(JsonData cfg, CreateSuccessCallback successCallback, CreateFailCallback failCallback)
{
Thread createThread = new Thread(new ParameterizedThreadStart(InnerCreate));
CreateParam createParam = new CreateParam
{
MCfg = cfg,
MCreateSuccessCallback = successCallback,
MCreateFailCallback = failCallback
};
createThread.Start(createParam);
}
private static void InnerCreate(object createParamObject)
{
try
{
CreateParam createParam = createParamObject as CreateParam;
JsonData cfg = createParam.MCfg;
try
{
if (cfg == null)
{
throw new ChivoxException(ChivoxErrorCode.ARGUMENT_NULL, "'cfg' is null");
}
Engine engine = new Engine(cfg);
CreateSuccessCallbackObject cObject = new CreateSuccessCallbackObject
{
MCreateSuccessCallback = createParam.MCreateSuccessCallback,
MEngine = engine
};
lock (SCreateSuccessCallbackObjects)
{
SCreateSuccessCallbackObjects.Add(cObject);
}
}
catch (Exception e)
{
ChivoxLogger.Log("new Engine threw an exception", "Engine", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.WARN);
CreateFailCallbackObject cObject;
if (e is ChivoxException)
{
cObject = new CreateFailCallbackObject
{
MCreateFailCallback = createParam.MCreateFailCallback,
MRetValue = new RetValue(e as ChivoxException)
};
}
else
{
cObject = new CreateFailCallbackObject
{
MCreateFailCallback = createParam.MCreateFailCallback,
MRetValue = new RetValue(e)
};
}
lock (SCreateFailCallbackObjects)
{
SCreateFailCallbackObjects.Add(cObject);
}
}
}
catch (Exception e)
{
ChivoxLogger.Log("InnerCreate caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
}
}
private static void StaticUnityUpdateCreateCallbackAll()
{
lock (SCreateSuccessCallbackObjects)
{
foreach (CreateSuccessCallbackObject cObject in SCreateSuccessCallbackObjects)
{
try
{
cObject.MCreateSuccessCallback(cObject.MEngine);
}
catch (Exception e)
{
ChivoxLogger.Log("MCreateSuccessCallback threw an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
}
}
SCreateSuccessCallbackObjects.Clear();
}
lock (SCreateFailCallbackObjects)
{
foreach (CreateFailCallbackObject cObject in SCreateFailCallbackObjects)
{
try
{
cObject.MCreateFailCallback(cObject.MRetValue);
}
catch (Exception e)
{
ChivoxLogger.Log("MCreateFailCallback threw an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
}
}
SCreateFailCallbackObjects.Clear();
}
}
#endregion
#region GetSerialNumber
public static JsonData GetSerialNumber(JsonData inputJson)
{
if (inputJson == null)
{
throw new ChivoxException(ChivoxErrorCode.ARGUMENT_NULL, "'inputJson' is null");
}
GetDeviceIdStatic();
string appKey = GetSerialNumberGetAppKey(inputJson);
JsonData resultJson = GetSerialNumberTryLoadResult(appKey);
if (resultJson != null)
{
return resultJson;
}
string result = AIEngineInternalUtility.StringFix(MInterface.Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr.Zero, Chivoxba38757dfb9abe4a02ec49ba6e4711c7.Chivox7c831efe5f467b2b41eac427c76d1a5e, inputJson.ToJson()));
resultJson = JsonMapper.ToObject(result);
GetSerialNumberTrySaveResult(appKey, resultJson);
return resultJson;
}
private static string GetSerialNumberGetAppKey(JsonData inputJson)
{
try
{
if (inputJson == null)
{
return null;
}
if (!inputJson.IsObject)
{
return null;
}
if (!inputJson.ContainsKey("appKey"))
{
return null;
}
JsonData appKeyJson = inputJson["appKey"];
if (!appKeyJson.IsString)
{
return null;
}
string appKey = (appKeyJson as IJsonWrapper).GetString();
if (appKey == null)
{
return null;
}
return appKey;
}
catch (SystemException e)
{
ChivoxLogger.Log("GetSerialNumberGetAppKey caught an exception ", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return null;
}
}
private static JsonData GetSerialNumberTryLoadResult(string appKey)
{
try
{
string serialNumber = AIEngineInternalUtility.LoadSerialNumber(appKey);
if (serialNumber == null)
{
return null;
}
JsonData resultJson = new JsonData();
resultJson["serialNumber"] = serialNumber;
return resultJson;
}
catch (SystemException e)
{
ChivoxLogger.Log("GetSerialNumberGetAppKeyTryLoadResult caught an exception ", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return null;
}
}
private static void GetSerialNumberTrySaveResult(string appKey, JsonData resultJson)
{
try
{
if (appKey == null)
{
return;
}
if (resultJson == null)
{
return;
}
if (!resultJson.IsObject)
{
return;
}
if (resultJson.ContainsKey("error") || resultJson.ContainsKey("sperror"))
{
return;
}
if (!resultJson.ContainsKey("serialNumber"))
{
return;
}
JsonData serialNumberJson = resultJson["serialNumber"];
if (!serialNumberJson.IsString)
{
return;
}
string serialNumber = (serialNumberJson as IJsonWrapper).GetString();
if (serialNumber == null)
{
return;
}
AIEngineInternalUtility.SaveSerialNumber(appKey, serialNumber);
}
catch (SystemException e)
{
ChivoxLogger.Log("GetSerialNumberTrySaveResult caught an exception ", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return;
}
}
public static void ClearSavedSerialNumber(string appKey)
{
AIEngineInternalUtility.ClearSerialNumber(appKey);
}
public static JsonData GetProvision(JsonData inputJson)
{
if (inputJson == null)
{
throw new ChivoxException(ChivoxErrorCode.ARGUMENT_NULL, "'inputJson' is null");
}
GetDeviceIdStatic();
JsonData resultJson;
string result = AIEngineInternalUtility.StringFix(MInterface.Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr.Zero, Chivoxba38757dfb9abe4a02ec49ba6e4711c7.Chivox7c831efe5f467b2b41eac427c76d1a5e, inputJson.ToJson()));
resultJson = JsonMapper.ToObject(result);
return resultJson;
}
#endregion
#region LogFile
public static void SetLogFile(string file)
{
ChivoxLogger.SLogFile = file;
}
public static string GetLogFile()
{
return ChivoxLogger.SLogFile;
}
#endregion
private Engine(JsonData cfg)
{
EngineNew(cfg);
}
~Engine()
{
try
{
Destroy();
}
catch (Exception e)
{
ChivoxLogger.Log("~Engine caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
}
}
public void Destroy()
{
if (Running())
{
EngineCancel();
}
EngineDelete();
}
#region Unity
public static void StaticUnityStart()
{
}
public static void StaticUnityUpdate()
{
StaticUnityUpdateCreateCallbackAll();
Callback.StaticUnityUpdateResultList();
}
public void UnityStart()
{
}
public void UnityUpdate()
{
if (!Running())
{
return;
}
if (UseRecorder())
{
UnityUpdateRecorder();
RecorderTryFeed();
if (ReorderTimeUp())
{
RecorderTryFeed(true);
Stop();
}
}
}
#endregion
#region Eval
public RetValue Start(AudioSrc audioSrc, out string tokenID, JsonData param, IEvalResultListener linstener)
{
try
{
CheckEngine();
if (Running())
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "don't call 'Start' repeatedly");
}
InnerStart();
EngineStart(param);
Callback.RegisterListener(GetTokenID(), linstener);
Callback.RegisterEngine(GetTokenID(), this);
if (audioSrc is AudioSrc.InnerRecorder)
{
RecorderStart(audioSrc as AudioSrc.InnerRecorder);
}
tokenID = GetTokenID();
return new RetValue();
}
catch (ChivoxException e)
{
ChivoxLogger.Log("Start caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
tokenID = null;
return new RetValue(e);
}
catch (Exception e)
{
ChivoxLogger.Log("Start caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
tokenID = null;
return new RetValue(e);
}
}
public RetValue Feed(byte[] data, int length)
{
try
{
CheckEngine();
if (!Running())
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "don't call 'Feed' before 'Start'");
}
if (UseRecorder())
{
return new RetValue();
}
EngineFeed(data, length);
return new RetValue();
}
catch (ChivoxException e)
{
ChivoxLogger.Log("Feed caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
catch (Exception e)
{
ChivoxLogger.Log("Feed caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
}
private RetValue FeedInner(byte[] data, int length)
{
try
{
CheckEngine();
if (!Running())
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "don't call 'Feed' before 'Start'");
}
EngineFeed(data, length);
return new RetValue();
}
catch (ChivoxException e)
{
ChivoxLogger.Log("FeedInner caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
catch (Exception e)
{
ChivoxLogger.Log("FeedInner caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
}
public RetValue Stop()
{
try
{
CheckEngine();
if (!Running())
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "don't call 'Stop' before 'Start'");
}
if (UseRecorder())
{
RecorderTryFeed(true);
RecorderStop();
}
EngineStop();
InnerStop();
return new RetValue();
}
catch (ChivoxException e)
{
ChivoxLogger.Log("Stop caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
catch (Exception e)
{
ChivoxLogger.Log("Stop caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
}
internal RetValue StopTokenID(string tokenID)
{
if (GetTokenID() != tokenID)
{
return new RetValue();
}
return Stop();
}
public RetValue Cancel()
{
try
{
CheckEngine();
if (!Running())
{
return new RetValue();
}
if (UseRecorder())
{
RecorderStop();
}
EngineCancel();
InnerStop();
return new RetValue();
}
catch (ChivoxException e)
{
ChivoxLogger.Log("Cancel caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
catch (Exception e)
{
ChivoxLogger.Log("Cancel caught an exception", "Engine", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "Engine", ChivoxLogger.ERROR);
return new RetValue(e);
}
}
#endregion
#region Running
private bool MRunning = false;
private void InnerStart()
{
MRunning = true;
}
private void InnerStop()
{
MRunning = false;
}
private bool Running()
{
return MRunning;
}
private void CheckRunning()
{
if (!MRunning)
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "engine is not running, cannot feed, stop or cancel");
}
}
private void CheckNotRunning()
{
if (MRunning)
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_CALL_ORDER_ERR, "engine is running, cannot start");
}
}
#endregion
#region Engine
private static readonly Chivox21ae98cf2aceb75fb36a9ce79cc94c1d MInterface = new Chivoxba38757dfb9abe4a02ec49ba6e4711c7();
private IntPtr MEngine = IntPtr.Zero;
private string MTokenID = null;
private void EngineNew(JsonData cfg)
{
EngineDelete();
MEngine = MInterface.Chivox5be48295a18e467ff9ae78ca451a6470(cfg.ToJson());
ChivoxLogger.Log("call aiengine new", "Engine", ChivoxLogger.INFO);
if (MEngine == IntPtr.Zero)
{
ThrowEngineException();
}
}
private void EngineDelete()
{
if (MEngine != IntPtr.Zero)
{
ChivoxLogger.Log("call aiengine delete", "Engine", ChivoxLogger.INFO);
MInterface.Chivox85754b00137d6cc647305bd6bdc0e3ed(MEngine);
MEngine = IntPtr.Zero;
}
}
private void EngineStart(JsonData param)
{
string tokenID;
ChivoxLogger.Log("call aiengine start", "Engine", ChivoxLogger.INFO);
int rv = MInterface.Chivox38c00b508ba8cdb67f863eb28858af0d(MEngine, param.ToJson(), out tokenID, Callback.AIEngineCallback);
if (rv != 0)
{
ThrowEngineException();
}
MTokenID = AIEngineInternalUtility.StringFix(tokenID);
}
private void EngineFeed(byte[] data, int length)
{
ChivoxLogger.Log("call aiengine feed", "Engine", ChivoxLogger.INFO);
int rv = MInterface.Chivoxb26bd2a9506e7271cd9118ae4a254418(MEngine, data, length);
if (rv != 0)
{
ThrowEngineException();
}
}
private void EngineStop()
{
ChivoxLogger.Log("call aiengine stop", "Engine", ChivoxLogger.INFO);
int rv = MInterface.Chivoxd819074a0b974099598d2d789d53a0cb(MEngine);
if (rv != 0)
{
ThrowEngineException();
}
MTokenID = null;
}
private void EngineCancel()
{
ChivoxLogger.Log("call aiengine cancel", "Engine", ChivoxLogger.INFO);
int rv = MInterface.Chivoxa7a031b129d49d6da0adf1996e3168f5(MEngine);
if (rv != 0)
{
ThrowEngineException();
}
MTokenID = null;
}
private string GetTokenID()
{
return MTokenID;
}
private void CheckEngine()
{
if (MEngine == IntPtr.Zero)
{
throw new ChivoxException(ChivoxErrorCode.ENGINE_DESTROYED, "engine is destroyed");
}
}
private void ThrowEngineException()
{
throw new ChivoxException(MInterface.Chivoxb010022345363c5670222786d6868899(), MInterface.Chivox2f254f00377c6489d2d8ca5b973e9b66());
}
#endregion
#region Recorder
private bool MUseRecorder = false;
private readonly Recorder MRecorder = new Recorder();
private ChivoxMedia.RecordParam MRecorderParam = null;
private void RecorderStart(AudioSrc.InnerRecorder audioSrc)
{
MRecorderParam = audioSrc.recordParam;
if (MRecorderParam.SaveFile != null)
{
MRecorderParam.SaveFile = Path.Combine(Environment.CurrentDirectory, MRecorderParam.SaveFile);
}
MRecorder.Start(MRecorderParam);
MUseRecorder = true;
}
private void RecorderStop()
{
if (!UseRecorder())
{
return;
}
AIEngineInternalUtility.SaveRecord(MRecorderParam.SaveFile, MRecorder.GetFdataAll(), MRecorderParam.SampleRate, MRecorderParam.SampleBytes);
Callback.RegisterPath(GetTokenID(), MRecorderParam.SaveFile);
MRecorder.Stop();
MUseRecorder = false;
}
private void RecorderTryFeed(bool feedLittle = false)
{
if (!UseRecorder())
{
return;
}
if (!(MRecorder.GetLengthSinceLast() >= 1600 || (feedLittle && MRecorder.GetLengthSinceLast() > 0)))
{
return;
}
byte[] data = MRecorder.GetDataSinceLast();
if (data == null)
{
return;
}
FeedInner(data, data.Length);
}
private bool UseRecorder()
{
return MUseRecorder;
}
private bool ReorderTimeUp()
{
return MRecorder.Recording() && !MRecorder.Controling();
}
private void UnityUpdateRecorder()
{
MRecorder.UnityUpdate();
}
#endregion
internal static string GetDeviceIdStatic()
{
return AIEngineInternalUtility.StringFix(MInterface.Chivoxf30480003d4330d9551c0206a6d22aa4());
}
internal static string GetVersionStatic()
{
return AIEngineInternalUtility.StringFix(MInterface.Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr.Zero, Chivoxba38757dfb9abe4a02ec49ba6e4711c7.Chivox9aabcdb3341d058077596ed7654b1634));
}
internal static string GetModulesStatic()
{
return AIEngineInternalUtility.StringFix(MInterface.Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr.Zero, Chivoxba38757dfb9abe4a02ec49ba6e4711c7.Chivoxccb8e05caaf25c97962e8eeb1f5a4521));
}
}
}
