using System;
using System.IO;
using System.Text;
namespace ChivoxAIEngine
{
public static class ChivoxLogger
{
public static string SLogFile = null;
public const int DEBUG = 0;
public const int INFO = 1;
public const int NOTICE = 2;
public const int WARN = 3;
public const int ERROR = 4;
public static void Log()
{
InnerLog("", "log", INFO);
}
public static void Log(string text, string tag = "log", int levelNumber = INFO)
{
InnerLog(text, tag, levelNumber);
}
public static void Log(object cObject, string tag = "log", int levelNumber = INFO)
{
if (cObject == null)
{
Log("null");
return;
}
InnerLog(cObject.ToString(), tag, levelNumber);
}
public static void Log(Exception e, string tag = "log", int levelNumber = INFO)
{
if (e == null)
{
InnerLog("null", tag, levelNumber);
return;
}
InnerLog(e.Message, tag, levelNumber);
InnerLog(e.StackTrace, tag, levelNumber);
}
private static string GetLevelText(int levelNumber)
{
switch (levelNumber)
{
case DEBUG:
return "DEBUG";
case INFO:
return "INFO ";
case NOTICE:
return "NOTIC";
case WARN:
return "WARN ";
case ERROR:
return "ERROR";
default:
return "UKNOW";
}
}
private static void InnerLog(string text, string tag, int levelNumber)
{
try
{
switch (levelNumber)
{
case INFO:
case NOTICE:
UnityEngine.Debug.Log(text);
break;
case WARN:
case ERROR:
UnityEngine.Debug.LogWarning(text);
break;
}
if (SLogFile == null)
{
return;
}
StreamWriter logWriter = new StreamWriter(SLogFile, true, Encoding.UTF8);
if (logWriter == null)
{
return;
}
string fullText = "[" + DateTime.Now.ToString() + "]" + "[" + GetLevelText(levelNumber) + "]" + "<" + tag + ">" + text;
logWriter.WriteLine(fullText);
logWriter.Close();
}
catch (Exception e)
{
UnityEngine.Debug.LogWarning("InnerLog threw an exception");
UnityEngine.Debug.LogWarning(e.Message);
UnityEngine.Debug.LogWarning(e.StackTrace);
}
}
}
}
