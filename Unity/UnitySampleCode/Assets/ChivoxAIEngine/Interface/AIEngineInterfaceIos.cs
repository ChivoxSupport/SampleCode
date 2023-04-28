#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
namespace ChivoxAIEngine
{
internal class Chivox7f288e5dd0c5d3fdc2f761c69c60ecb4 : Chivox21ae98cf2aceb75fb36a9ce79cc94c1d
{
public Chivox7f288e5dd0c5d3fdc2f761c69c60ecb4()
{
Chivoxf30480003d4330d9551c0206a6d22aa4();
}
public IntPtr Chivox5be48295a18e467ff9ae78ca451a6470(string Chivox4e9345d32e9eb3081414c942f8242d0e)
{
byte[] b = AIEngineInternalUtility.StringToUTF8Bytes(Chivox4e9345d32e9eb3081414c942f8242d0e);
return aiengine_new(b);
}
[DllImport("__Internal")]
private static extern IntPtr aiengine_new([In] byte[] Chivox4e9345d32e9eb3081414c942f8242d0e);
public void Chivox85754b00137d6cc647305bd6bdc0e3ed(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3)
{
aiengine_delete(Chivox91b5e080d4fcc88459040ee5120abbb3);
}
[DllImport("__Internal")]
private static extern int aiengine_delete(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3);
public int Chivox38c00b508ba8cdb67f863eb28858af0d(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, string Chivoxee0d828139a28617efd9505068b27daf, out string Chivoxa57041fbe1c915012eb15b3e882fd38b, Chivoxd161509b47344be06dc395657bb1ec1b.Chivox5192fcf9c412a141a1f50a57e24c8510 Chivoxafeac0457b7123b4e517408b1115dc2e)
{
byte[] bParam = AIEngineInternalUtility.StringToUTF8Bytes(Chivoxee0d828139a28617efd9505068b27daf);
byte[] Chivox340c71ceec4de8bdcffe675ccd0d6e2e = new byte[64];
int Chivox4c124974c05478053529b8bf8ac7ef4f = aiengine_start(Chivox91b5e080d4fcc88459040ee5120abbb3, bParam, Chivox340c71ceec4de8bdcffe675ccd0d6e2e, Chivoxafeac0457b7123b4e517408b1115dc2e, IntPtr.Zero);
Chivoxa57041fbe1c915012eb15b3e882fd38b = AIEngineInternalUtility.UTF8BytesToString(Chivox340c71ceec4de8bdcffe675ccd0d6e2e);
return Chivox4c124974c05478053529b8bf8ac7ef4f;
}
[DllImport("__Internal")]
private static extern int aiengine_start(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, [In] byte[] Chivoxee0d828139a28617efd9505068b27daf, [In, Out] byte[] Chivox340c71ceec4de8bdcffe675ccd0d6e2e, [MarshalAs(UnmanagedType.FunctionPtr)] Chivoxd161509b47344be06dc395657bb1ec1b.Chivox5192fcf9c412a141a1f50a57e24c8510 Chivoxafeac0457b7123b4e517408b1115dc2e, IntPtr Chivoxbb5cade7983ace031cf19b032c713523);
public int Chivoxb26bd2a9506e7271cd9118ae4a254418(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, byte[] Chivox14be6b37673cd40e800f16d819c6423c, int Chivox758bbb72081a31489a0243e30dc051b2)
{
if (Chivox758bbb72081a31489a0243e30dc051b2 > Chivox14be6b37673cd40e800f16d819c6423c.Length)
{
Chivox758bbb72081a31489a0243e30dc051b2 = Chivox14be6b37673cd40e800f16d819c6423c.Length;
}
return aiengine_feed(Chivox91b5e080d4fcc88459040ee5120abbb3, Chivox14be6b37673cd40e800f16d819c6423c, Chivox758bbb72081a31489a0243e30dc051b2);
}
[DllImport("__Internal")]
private static extern int aiengine_feed(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, byte[] Chivox14be6b37673cd40e800f16d819c6423c, int Chivoxcfd8efca7e0b1d13ee6b42707840e0a9);
public int Chivoxd819074a0b974099598d2d789d53a0cb(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3)
{
return aiengine_stop(Chivox91b5e080d4fcc88459040ee5120abbb3);
}
[DllImport("__Internal")]
private static extern int aiengine_stop(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3);
public int Chivoxa7a031b129d49d6da0adf1996e3168f5(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3)
{
return aiengine_cancel(Chivox91b5e080d4fcc88459040ee5120abbb3);
}
[DllImport("__Internal")]
private static extern int aiengine_cancel(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3);
public string Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, int Chivoxd04b747eb68c12eae558b25271f983ed)
{
byte[] Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c = new byte[1024];
int rv = aiengine_opt(Chivox91b5e080d4fcc88459040ee5120abbb3, Chivoxd04b747eb68c12eae558b25271f983ed, Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c, Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c.Length);
if (rv < 0)
            {
return "";
            }
return AIEngineInternalUtility.UTF8BytesToString(Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c, rv);
}
public string Chivox086e3fa0bbbfa1f84c5323b47f481f78(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, int Chivoxd04b747eb68c12eae558b25271f983ed, string Chivox22915968357421a04ffa7e1324c5f464)
{
byte[] Chivox24f3d9d2800f039705d7dc6af486c1dd = AIEngineInternalUtility.StringToUTF8Bytes(Chivox22915968357421a04ffa7e1324c5f464);
byte[] Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c = new byte[1024];
for (int Chivox16ee703e30b01eb3614bf500b298575b = 0; Chivox16ee703e30b01eb3614bf500b298575b < Chivox24f3d9d2800f039705d7dc6af486c1dd.Length && Chivox16ee703e30b01eb3614bf500b298575b < Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c.Length; ++Chivox16ee703e30b01eb3614bf500b298575b)
{
Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c[Chivox16ee703e30b01eb3614bf500b298575b] = Chivox24f3d9d2800f039705d7dc6af486c1dd[Chivox16ee703e30b01eb3614bf500b298575b];
}
int rv = aiengine_opt(Chivox91b5e080d4fcc88459040ee5120abbb3, Chivoxd04b747eb68c12eae558b25271f983ed, Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c, Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c.Length);
if (rv < 0)
{
return "";
}
return AIEngineInternalUtility.UTF8BytesToString(Chivoxc68bac0bf24f0c66f84aa7dbb2fd160c, rv);
}
[DllImport("__Internal")]
private static extern int aiengine_opt(IntPtr Chivox91b5e080d4fcc88459040ee5120abbb3, int Chivoxd04b747eb68c12eae558b25271f983ed, [In, Out] byte[] Chivox14be6b37673cd40e800f16d819c6423c, int Chivoxcfd8efca7e0b1d13ee6b42707840e0a9);
public string Chivoxf30480003d4330d9551c0206a6d22aa4()
{
byte[] Chivoxcff9a515c46a7080530b16ec0fdaa0de = new byte[64];
int rv = aiengine_get_device_id(Chivoxcff9a515c46a7080530b16ec0fdaa0de);
if (rv != 0)
            {
return "";
            }
return AIEngineInternalUtility.UTF8BytesToString(Chivoxcff9a515c46a7080530b16ec0fdaa0de);
}
[DllImport("__Internal")]
private static extern int aiengine_get_device_id([In, Out] byte[] Chivoxcff9a515c46a7080530b16ec0fdaa0de);
public int Chivoxb010022345363c5670222786d6868899()
{
return aiengine_get_last_error_code();
}
[DllImport("__Internal")]
private static extern int aiengine_get_last_error_code();
public string Chivox2f254f00377c6489d2d8ca5b973e9b66()
{
return Marshal.PtrToStringAnsi(aiengine_get_last_error_text());
}
[DllImport("__Internal")]
private static extern IntPtr aiengine_get_last_error_text();
public void Chivox2e7571d909703e5b6b526cf44df6a167()
{
}
public void Chivoxc67fe4304e2bc58bd190df0b815fd32e()
{
}
}
}
#endif
