using System;
namespace ChivoxAIEngine
{
public class RecorderNotify
{
private static readonly RecorderNotify SInstance = new RecorderNotify();
private IListener MListener = null;
private RecorderNotify()
{
}
public static RecorderNotify SharedInstance()
{
return SInstance;
}
public void SetListener(IListener lis)
{
MListener = lis;
}
internal void FireOnStart()
{
try
{
if (MListener == null)
{
return;
}
MListener.OnRecordStart();
}
catch (Exception e)
{
ChivoxLogger.Log("OnRecordStart threw an exception", "RecorderNotify", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "RecorderNotify", ChivoxLogger.ERROR);
}
}
internal void FireOnStop()
{
try
{
if (MListener == null)
{
return;
}
MListener.OnRecordStop();
}
catch (Exception e)
{
ChivoxLogger.Log("OnRecordStop threw an exception", "RecorderNotify", ChivoxLogger.ERROR);
ChivoxLogger.Log(e, "RecorderNotify", ChivoxLogger.ERROR);
}
}
public interface IListener
{
void OnRecordStart();
void OnRecordStop();
}
}
}
