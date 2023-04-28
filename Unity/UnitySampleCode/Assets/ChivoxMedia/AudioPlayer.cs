using UnityEngine;
using ChivoxAIEngine;
namespace ChivoxMedia
{
public class AudioPlayer
{
private static readonly AudioPlayer SSharedInstance = new AudioPlayer();
public static AudioPlayer SharedInstance()
{
return SSharedInstance;
}
public AudioSource MAudioSource;
public void SetAudioSource(AudioSource audioSource)
{
MAudioSource = audioSource;
}
public void PlayOneShot(string path)
{
#if UNITY_STANDALONE_WIN
WWW www = new WWW("file:///" + path);
#else
WWW www = new WWW("file://" + path);
#endif
ChivoxLogger.Log("PlayOneShot " + path, "AudioPlayer", ChivoxLogger.INFO);
AudioClip audioClip = www.GetAudioClip();
MAudioSource.PlayOneShot(audioClip);
www.Dispose();
}
public void Cancel()
{
if (MAudioSource.isPlaying)
{
MAudioSource.Stop();
}
}
}
}
