using System.IO;
using UnityEngine;
namespace ChivoxAIEngine
{
public class Recorder
{
private AudioClip MClip = null;
private int MLastGotPosition = 0;
private int MPosition = 0;
private int MRealPosition = 0;
private bool MRecording = false;
private bool MControling = false;
public int MDuration = 0;
public int MChannel = 1;
public int MSampleBytes = 2;
public int MSampleRate = 16000;
public void Start(ChivoxMedia.RecordParam recordParam)
{
if (Microphone.IsRecording(null))
{
throw new ChivoxException(ChivoxErrorCode.RECORDER_IN_USE, "recorder in use");
}
MDuration = recordParam.Duration;
if (MDuration <= 0)
{
MDuration = 300000;
}
MChannel = recordParam.Channel;
MSampleBytes = recordParam.SampleBytes;
MSampleRate = recordParam.SampleRate;
MClip = Microphone.Start(null, false, (MDuration + 20) / 1000 + 10, MSampleRate);
RecorderNotify.SharedInstance().FireOnStart();
MLastGotPosition = 0;
MPosition = 0;
MRealPosition = 0;
MRecording = true;
MControling = true;
}
public void Stop()
{
if (!Recording())
{
return;
}
if (!Controling())
{
return;
}
if (Microphone.IsRecording(null))
{
Microphone.End(null);
}
RecorderNotify.SharedInstance().FireOnStop();
MRecording = false;
MControling = false;
}
public void Reset()
{
if (Microphone.IsRecording(null))
{
Microphone.End(null);
}
MClip = null;
MLastGotPosition = 0;
MPosition = 0;
MRecording = false;
MControling = false;
}
public int GetLengthAll()
{
return MPosition;
}
public float[] GetFdataAll()
{
if (MClip == null)
{
return null;
}
if (MPosition == 0)
{
return null;
}
float[] fdata = new float[MPosition];
MClip.GetData(fdata, 0);
return fdata;
}
public byte[] GetDataAll()
{
if (MClip == null)
{
return null;
}
if (MPosition == 0)
{
return null;
}
float[] fdata = new float[MPosition];
MClip.GetData(fdata, 0);
return AIEngineInternalUtility.FdataToData(fdata, MSampleBytes);
}
public int GetLengthSinceLast()
{
return MPosition - MLastGotPosition;
}
public byte[] GetDataSinceLast()
{
if (MClip == null)
{
return null;
}
if (MPosition - MLastGotPosition <= 0)
{
return null;
}
float[] fdata = new float[MPosition - MLastGotPosition];
MClip.GetData(fdata, MLastGotPosition);
MLastGotPosition = MPosition;
return AIEngineInternalUtility.FdataToData(fdata, MSampleBytes);
}
public bool Recording()
{
return MRecording;
}
public bool Controling()
{
return MControling;
}
public void UnityUpdate()
{
if (!Recording())
{
return;
}
if (!Controling())
{
return;
}
if (!Microphone.IsRecording(null))
{
ChivoxLogger.Log("Recorder out of control", "Recorder", ChivoxLogger.WARN);
MControling = false;
return;
}
int position = Microphone.GetPosition(null);
if (position < MRealPosition)
{
ChivoxLogger.Log("Recorder out of control", "Recorder", ChivoxLogger.WARN);
MControling = false;
return;
}
MRealPosition = position;
if (MRealPosition >= (int)(MSampleRate * ((MDuration + 20) / 1000f)))
{
MPosition = (int)(MSampleRate * ((MDuration + 20) / 1000f));
Microphone.End(null);
MControling = false;
}
else
{
MPosition = MRealPosition;
}
}
}
}
