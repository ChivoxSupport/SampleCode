using System.IO;
using UnityEngine;
namespace ChivoxAIEngine
{
public static class AIEngineUtility
{
public static string CopyStreamingAssets(string relative_path)
{
byte[] data;
string old_path = Path.Combine(Application.streamingAssetsPath, relative_path);
if (old_path.IndexOf("://") != -1)
{
WWW old_file = new WWW(old_path);
while (!old_file.isDone) { }
data = old_file.bytes;
old_file.Dispose();
}
else
{
FileStream old_file = new FileStream(old_path, FileMode.Open, FileAccess.Read);
data = new byte[old_file.Length];
old_file.Read(data, 0, (int)old_file.Length);
old_file.Dispose();
}
string new_path = Path.Combine(Application.persistentDataPath + "/", relative_path);
FileStream new_file;
new_file = new FileStream(new_path, FileMode.Create, FileAccess.Write);
new_file.Write(data, 0, data.Length);
new_file.Close();
return new_path;
}
}
}
