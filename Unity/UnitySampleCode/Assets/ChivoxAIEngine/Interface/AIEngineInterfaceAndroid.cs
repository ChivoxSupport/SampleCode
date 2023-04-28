#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
namespace ChivoxAIEngine
{
internal class Chivox0fe79ecc253221d9f4d18629d756bc0a : Chivox9f784d2e80bf5f2f0c4b670ebe7bfaba, Chivox21ae98cf2aceb75fb36a9ce79cc94c1d
{
private readonly string Chivox8a6b49c2807967fc95a79a46670c914a;
public Chivox0fe79ecc253221d9f4d18629d756bc0a()
{
if (Chivox8a6b49c2807967fc95a79a46670c914a == null || Chivox8a6b49c2807967fc95a79a46670c914a.Length == 0)
{
Chivox8a6b49c2807967fc95a79a46670c914a = Chivox276ab70fbf85d8f89f49b477bd615c06();
}
}
public new string Chivoxf30480003d4330d9551c0206a6d22aa4()
{
return Chivox8a6b49c2807967fc95a79a46670c914a;
}
private static string Chivox276ab70fbf85d8f89f49b477bd615c06()
{
AndroidJavaClass Chivox790c45907331252b60c50a8ada2819d1 = new AndroidJavaClass("com.chivox.AIEngine");
AndroidJavaClass Chivox796a7be35cce8547d7d1b0b0f3471517 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
AndroidJavaClass Chivoxdd306593b494826ce3ed00ed75e73b24 = new AndroidJavaClass("java.lang.Byte");
AndroidJavaClass Chivox78a7876e621123a9ffddcbbdd0275bc6 = new AndroidJavaClass("java.lang.reflect.Array");
AndroidJavaObject Chivox75e42d3eece42c96728798a8b50d76a2 = Chivoxdd306593b494826ce3ed00ed75e73b24.GetStatic<AndroidJavaObject>("TYPE");
AndroidJavaObject Chivox75a152b7027f8e634b5c3332e6a097ef = Chivox78a7876e621123a9ffddcbbdd0275bc6.CallStatic<AndroidJavaObject>("newInstance", Chivox75e42d3eece42c96728798a8b50d76a2, 64);
AndroidJavaObject Chivox987954d88162395b4995b151705ab713 = Chivox796a7be35cce8547d7d1b0b0f3471517.GetStatic<AndroidJavaObject>("currentActivity");
Chivox790c45907331252b60c50a8ada2819d1.CallStatic<int>("aiengine_get_device_id", Chivox75a152b7027f8e634b5c3332e6a097ef, Chivox987954d88162395b4995b151705ab713);
int Chivox758bbb72081a31489a0243e30dc051b2 = Chivox78a7876e621123a9ffddcbbdd0275bc6.CallStatic<int>("getLength", Chivox75a152b7027f8e634b5c3332e6a097ef);
byte[] Chivox24f3d9d2800f039705d7dc6af486c1dd = new byte[Chivox758bbb72081a31489a0243e30dc051b2];
for (int Chivox16ee703e30b01eb3614bf500b298575b = 0; Chivox16ee703e30b01eb3614bf500b298575b < Chivox758bbb72081a31489a0243e30dc051b2; ++Chivox16ee703e30b01eb3614bf500b298575b)
{
Chivox24f3d9d2800f039705d7dc6af486c1dd[Chivox16ee703e30b01eb3614bf500b298575b] = Chivox78a7876e621123a9ffddcbbdd0275bc6.CallStatic<byte>("getByte", Chivox75a152b7027f8e634b5c3332e6a097ef, Chivox16ee703e30b01eb3614bf500b298575b);
}
string Chivox22915968357421a04ffa7e1324c5f464 = Encoding.UTF8.GetString(Chivox24f3d9d2800f039705d7dc6af486c1dd);
Chivox790c45907331252b60c50a8ada2819d1.Dispose();
Chivox796a7be35cce8547d7d1b0b0f3471517.Dispose();
Chivoxdd306593b494826ce3ed00ed75e73b24.Dispose();
Chivox78a7876e621123a9ffddcbbdd0275bc6.Dispose();
return Chivox22915968357421a04ffa7e1324c5f464;
}
}
}
#endif
