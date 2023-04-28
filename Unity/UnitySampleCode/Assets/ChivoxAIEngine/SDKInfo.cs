using System;
using LitJson;
namespace ChivoxAIEngine
{
public class SDKInfo
{
public int VersionMajor
{
get
{
return 2;
}
}
public int VersionMinor
{
get
{
return 0;
}
}
public int VersionPatch
{
get
{
return 4;
}
}
public int VersionTweak
{
get
{
return 0;
}
}
public string VersionBuild
{
get
{
return "20210112";
}
}
public string Version
{
get
{
if (VersionTweak == 0)
{
return VersionMajor.ToString() + "." + VersionMinor.ToString() + "." + VersionPatch.ToString() + "-" + VersionBuild.ToString();
}
else
{
return VersionMajor.ToString() + "." + VersionMinor.ToString() + "." + VersionPatch.ToString() + "." + VersionTweak.ToString() + "-" + VersionBuild.ToString();
}
}
}
public string CommonSDKVersion
{
get
{
string versionAll = Engine.GetVersionStatic();
if (versionAll == null)
{
return null;
}
JsonData versionJsonAll;
try
{
versionJsonAll = JsonMapper.ToObject(versionAll);
}
catch (Exception e)
{
ChivoxLogger.Log("CommonSDKVersion JsonMapper.ToObject an exception", "Recorder", ChivoxLogger.WARN);
ChivoxLogger.Log(e, "Recorder", ChivoxLogger.WARN);
return null;
}
if (versionJsonAll == null)
{
return null;
}
if (!versionJsonAll.ContainsKey("version"))
{
return null;
}
if (!versionJsonAll["version"].IsString)
{
return null;
}
return (versionJsonAll["version"] as IJsonWrapper).GetString();
}
}
public string CommonSDKModules
{
get
{
return Engine.GetModulesStatic();
}
}
}
}
