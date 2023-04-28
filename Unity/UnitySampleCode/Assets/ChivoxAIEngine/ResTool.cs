using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Xml;
using LitJson;
namespace ChivoxAIEngine
{
public static class ResTool
{
#if UNITY_2018_1_OR_NEWER && !NET_2_0 && !NET_2_0_SUBSET
public interface IExtractHint
{
void OnProgress(float progress);
}
public static string Extract(string[] assetNames, string resRoot, IExtractHint hint)
{
try
{
if (resRoot == null)
{
throw new ApplicationException("'resRoot' is null");
}
if (assetNames == null || assetNames.Length == 0)
{
if (hint != null)
{
hint.OnProgress(1f);
}
return null;
}
int assetCount = assetNames.Length;
for (int i = 0; i < assetCount; ++i)
{
string name = assetNames[i];
if (name != null)
{
name = name.Trim();
}
if (name == null || name.Length == 0)
{
if (hint != null)
{
hint.OnProgress((i + 1f) / assetCount);
}
continue;
}
string error = Extract(name, resRoot, null);
if (error != null)
{
return error;
}
if (hint != null)
{
hint.OnProgress((i + 1f) / assetCount);
}
}
return null;
}
catch (Exception e)
{
ChivoxLogger.Log("Extract caught an exception", "ResTool", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.ERROR);
return "Extract failed: " + e.Message;
}
}
public static string Extract(string assetName, string resRoot, IExtractHint hint)
{
try
{
if (assetName == null)
{
throw new ApplicationException("'assetName' is null");
}
if (resRoot == null)
{
throw new ApplicationException("'resRoot' is null");
}
string copyName = AIEngineUtility.CopyStreamingAssets(assetName);
if (copyName == null || copyName.Length == 0)
{
throw new ApplicationException("invalid asset: " + assetName);
}
string baseName = Path.GetFileNameWithoutExtension(copyName);
if (baseName.Length == 0)
{
throw new ApplicationException("invalid asset: " + assetName);
}
string targetDir = Path.Combine(resRoot, baseName);
string targetMD5FilePath = Path.Combine(targetDir, ".md5sum");
string targetMD5 = GetFileMD5(copyName);
string storeMD5 = null;
if (File.Exists(targetMD5FilePath))
{
storeMD5 = ReadFileText(targetMD5FilePath);
}
if (targetMD5 != storeMD5)
{
try
{
if (Directory.Exists(targetDir))
{
Directory.Delete(targetDir, true);
}
}
catch (Exception e)
{
ChivoxLogger.Log("Directory threw an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
}
//ZipFile.ExtractToDirectory(copyName, targetDir);  //在线评测不需要
WriterFileText(targetMD5FilePath, targetMD5);
}
else
{
ChivoxLogger.Log("Do not need Extract: " + assetName, "ResTool", ChivoxLogger.INFO);
}
if (hint != null)
{
hint.OnProgress(1f);
}
return null;
}
catch (Exception e)
{
ChivoxLogger.Log("Extract caught an exception", "ResTool", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.ERROR);
return "Extract fail: " + e.Message;
}
}
#endif
private static void WriterFileData(string filePath, byte[] fileData)
{
FileStream fileStream = null;
try
{
fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
fileStream.Write(fileData, 0, fileData.Length);
fileStream.Close();
}
catch (Exception e)
{
ChivoxLogger.Log("WriterFileData caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
}
finally
{
if (fileStream != null)
{
fileStream.Dispose();
}
}
}
private static void WriterFileText(string filePath, string fileText)
{
try
{
WriterFileData(filePath, Encoding.UTF8.GetBytes(fileText));
}
catch (Exception e)
{
ChivoxLogger.Log("WriterFileText caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
}
}
private static byte[] ReadFileData(string filePath)
{
FileStream fileStream = null;
try
{
fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
long fileLongLength = fileStream.Length;
int fileLength = (int)(fileLongLength);
if (fileLength != fileLongLength)
{
return null;
}
byte[] fileData = new byte[fileLength];
fileStream.Read(fileData, 0, fileLength);
fileStream.Close();
fileStream.Dispose();
return fileData;
}
catch (Exception e)
{
ChivoxLogger.Log("ReadFileData caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
return null;
}
finally
{
if (fileStream != null)
{
fileStream.Dispose();
}
}
}
private static string ReadFileText(string filePath)
{
try
{
return Encoding.UTF8.GetString(ReadFileData(filePath));
}
catch (Exception e)
{
ChivoxLogger.Log("ReadFileText caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
return null;
}
}
private static string GetFileMD5(string filePath)
{
FileStream fileStream = null;
MD5 md5 = null;
try
{
fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
md5 = MD5.Create();
byte[] fileMD5Data = md5.ComputeHash(fileStream);
string fileMD5 = BytesToHex(fileMD5Data);
fileStream.Close();
return fileMD5;
}
catch (Exception e)
{
ChivoxLogger.Log("GetFileMD5 caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
return null;
}
finally
{
if (fileStream != null)
{
fileStream.Dispose();
}
if (md5 != null)
{
#if !NET_2_0 && !NET_2_0_SUBSET
md5.Dispose();
#endif
}
}
}
private static string BytesToHex(byte[] data)
{
try
{
return BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
}
catch (Exception e)
{
ChivoxLogger.Log("BytesToHex caught an exception", "ResTool", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.WARN);
return null;
}
}
public static JsonData LoadNativeCfgJson(string resRoot, string[] resNames)
{
try
{
JsonData natCfg = new JsonData();
foreach (string resName in resNames)
{
string resDir = Path.Combine(resRoot, resName);
string confFile = Path.Combine(resDir, "conf.xml");
XmlDocument dom = new XmlDocument();
dom.Load(confFile);
XmlNode first = dom.ChildNodes[1];
string coreName = first.Name;
XmlNodeList list = first.ChildNodes;
string binValue = null;
string lmValue = null;
foreach (XmlNode node in list)
{
if (node.Name == "bin")
{
binValue = node.InnerText;
if (binValue[0] == '/' || binValue[0] == '\\')
{
binValue = binValue.Substring(1);
}
}
else if (node.Name == "lm")
{
lmValue = node.InnerText;
if (lmValue[0] == '/' || lmValue[0] == '\\')
{
lmValue = lmValue.Substring(1);
}
}
}
if (coreName != null)
{
natCfg[coreName] = new JsonData();
if (binValue != null)
{
natCfg[coreName]["resDirPath"] = Path.Combine(resDir, binValue);
}
if (lmValue != null)
{
natCfg[coreName]["reslm"] = Path.Combine(resDir, lmValue);
}
}
}
return natCfg;
}
catch (Exception e)
{
ChivoxLogger.Log("LoadNativeCfgJson caught an exception", "ResTool", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "ResTool", ChivoxLogger.ERROR);
return null;
}
}
}
}
