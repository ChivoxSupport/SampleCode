using System;
using System.IO;
using System.Text;
using UnityEngine;
namespace ChivoxAIEngine
{
internal static class AIEngineInternalUtility
{
internal static string StringFix(string input)
{
int length = 0;
while (length < input.Length && input[length] != 0)
{
++length;
}
return input.Substring(0, length);
}
internal static void SaveSerialNumber(string appKey, string serialNumber)
{
string filePath = GetSerialNumberFilePath(appKey);
StreamWriter stream = new StreamWriter(filePath, false, Encoding.UTF8);
if (stream == null)
{
return;
}
stream.WriteLine(serialNumber);
stream.Close();
}
internal static void ClearSerialNumber(string appKey)
{
string filePath = GetSerialNumberFilePath(appKey);
File.Delete(filePath);
}
internal static string LoadSerialNumber(string appKey)
{
string filePath = GetSerialNumberFilePath(appKey);
StreamReader stream;
try
{
stream = new StreamReader(filePath, Encoding.UTF8);
}
catch
{
return null;
}
string serialNumber = stream.ReadLine();
stream.Close();
return serialNumber;
}
private static string GetSerialNumberFilePath(string appKey)
{
return Path.Combine(Application.persistentDataPath, "chivox_serial_number_" + appKey + ".txt");
}
internal static void SaveRecord(string path, float[] fdata, int sampleRate = 16000, int sampleBytes = 2)
{
try
{
FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
byte[] data = FdataToData(fdata, sampleBytes);
fs.Write(Encoding.UTF8.GetBytes("RIFF"), 0, 4);
fs.Write(BitConverter.GetBytes(36 + data.Length), 0, 4);
fs.Write(Encoding.UTF8.GetBytes("WAVE"), 0, 4);
fs.Write(Encoding.UTF8.GetBytes("fmt "), 0, 4);
fs.Write(BitConverter.GetBytes(16), 0, 4);
fs.Write(BitConverter.GetBytes(1), 0, 2);
fs.Write(BitConverter.GetBytes(1), 0, 2);
fs.Write(BitConverter.GetBytes(sampleRate), 0, 4);
fs.Write(BitConverter.GetBytes(sampleRate * sampleBytes), 0, 4);
fs.Write(BitConverter.GetBytes(sampleBytes), 0, 2);
fs.Write(BitConverter.GetBytes(sampleBytes * 8), 0, 2);
fs.Write(Encoding.UTF8.GetBytes("data"), 0, 4);
fs.Write(BitConverter.GetBytes(data.Length), 0, 4);
fs.Write(data, 0, data.Length);
fs.Close();
}
catch(Exception e)
{
ChivoxLogger.Log("SaveRecord caught an exception", "AIEngineInternalUtility", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "AIEngineInternalUtility", ChivoxLogger.ERROR);
}
}
internal static float[] LoadRecord(string path)
{
FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
byte[] data = new byte[fs.Length];
fs.Read(data, 0, (int)(fs.Length));
float[] fdata = DataToFdata(data);
fs.Close();
return fdata;
}
internal static byte[] FdataToData(float[] fdata, int sampleBytes = 2)
{
switch (sampleBytes)
{
case 1:
return FdataToData1(fdata);
case 2:
return FdataToData2(fdata);
default:
return null;
}
}
private static byte[] FdataToData1(float[] fdata)
{
int length = fdata.Length;
byte[] data = new byte[length];
for (int i = 0; i < length; ++i)
{
int sample = (int)(fdata[i] * 128);
if (sample < 0)
{
sample += 256;
}
data[i] = (byte)(sample);
}
return data;
}
private static byte[] FdataToData2(float[] fdata)
{
int length = fdata.Length;
byte[] data = new byte[length * 2];
for (int i = 0; i < length; ++i)
{
int sample = (int)(fdata[i] * 32768);
if (sample < 0)
{
sample += 65536;
}
data[2 * i + 0] = (byte)(sample % 256);
data[2 * i + 1] = (byte)(sample / 256);
}
return data;
}
internal static float[] DataToFdata(byte[] data)
{
int length = data.Length / 2;
float[] fdata = new float[length];
for (int i = 0; i < length; ++i)
{
int sample = data[2 * i + 0] + (data[2 * i + 1] * 256);
if (sample >= 32768)
{
sample -= 65536;
}
fdata[i] = sample / 32768f;
}
return fdata;
}
internal static byte[] StringToUTF8Bytes(string str)
{
if (null != str)
{
int cnt = Encoding.UTF8.GetByteCount(str);
byte[] b = new byte[cnt + 1];
Encoding.UTF8.GetBytes(str, 0, str.Length, b, 0);
b[cnt] = 0;
return b;
}
return null;
}
internal static string UTF8BytesToString(byte[] data)
{
return UTF8BytesToString(data, (null != data) ? data.Length : 0);
}
internal static string UTF8BytesToString(byte[] data, int len)
{
if (null == data)
{
return null;
}
if (len <= 0)
{
return "";
}
len = (len > data.Length) ? data.Length : len;
for (int i = 0; i < data.Length; ++i)
{
if (data[i] == 0)
{
if (len > i)
{
len = i;
}
break;
}
}
string str = Encoding.UTF8.GetString(data, 0, len);
return str;
}
}
}
