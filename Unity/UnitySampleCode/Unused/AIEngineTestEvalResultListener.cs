
using ChivoxAIEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEngineTestEvalResultListener : IEvalResultListener
{





    private void PrintResult(string tokenID, EvalResult result)
    {
        try
        {
            if (result.RecFilePath != null)
            {
                ChivoxTest.MInstance.MAudioPath = result.RecFilePath;
            }

            Debug.Log("On Result");
            Debug.Log("tokenID: " + tokenID);
            Debug.Log("tokenID: " + result.TokenID);
            Debug.Log("result: " + result.Text);

            Debug.Log("  收到消息 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + "<color=yellow>" + DateTime.Now + DateTime.Now.Millisecond + "</color>");


            Debug.Log("path: " + result.RecFilePath);


            var jo = LitJson.JsonMapper.ToObject(result.Text); ;
            if (jo["result"] != null)
            {
                 //if (AppBaseDefine.soundScore != "")
                 //   {
                 //       AOutput.Log("已存在假分 = " + AppBaseDefine.soundScore);
                 //       WebSocketConnector.Instance.bConnecting = false;
                 //       WebSocketConnector.Instance.bConnected = false;
                 //       return;
                 //   }

                   jo = jo["result"];

                    var tempScore = 0;
                    if (jo.ContainsKey("integrity"))
                    {

                        Debug.Log($"完整度: {jo["integrity"].ToString()} " +
                            $"准确度:{jo["accuracy"].ToString()} " +
                            $"信噪比:{jo["info"]["snr"].ToString()} " +
                            $"音量:{jo["info"]["volume"].ToString()} " + $"总分:{ jo["overall"].ToString()   }");

                        var integrity =int.Parse( jo["integrity"].ToString());
                        var accuracy = int.Parse(jo["accuracy"].ToString());
                        //临时分数  = 0.3 完整度  +  准确度  0.7  
                        tempScore = (int)(integrity * 0.3 + accuracy * 0.7);

                        var volume = int.Parse(jo["info"]["volume"].ToString());
                        var overall = int.Parse(jo["overall"].ToString());

                        if (volume <= 300)
                        {
                            tempScore = 0;
                        }
                        else if (overall < 30)
                        {
                            //额外分数 =  随机加上最高15   再加 10  
                            var irdm = UnityEngine.Random.Range(10 , 15);

                            // 如果额外分数大于 overall  使用 临时分数
                            if (irdm > overall)
                            {
                                overall = irdm;
                            }
                        }

                        tempScore = overall;



                    }
                    else
                    {
                        Debug.Log($"****************************************总分:{ jo["overall"].ToString()   }");
                        tempScore = int.Parse( jo["overall"].ToString());
                    }

                    if (tempScore > 0)
                    {
                        //AudioFileManager.Instance.AudioFilePath = DownloadManager.Instance.LocalFilePath + "AIRecord/" + AppBaseDefine.tempRecordList + ".wav";
                        //AudioFileManager.Instance.AudioName = AppBaseDefine.tempRecordList + ".wav";
                        //AudioFileManager.Instance.m_ziyuancategory = 6;
                        //AudioFileManager.Instance.m_Dayid = AppBaseDefine.FromReadToThis ? AppBaseDefine.TempCurrentDay : AppBaseDefine.CurrentDay;
                        //AudioFileManager.Instance.m_Score = tempScore;
                        //AudioFileManager.Instance.IsSend = true;

                    }


                    //AppBaseDefine.soundScore = tempScore.ToString();
                    //AppBaseDefine.continueScore = tempScore.ToString();
                    ChivoxTest.MInstance.Callback?.Invoke(); 
                 

               


            }

           // ChivoxTest.MInstance.RecordCancel();
           //ChivoxTest.MInstance.OnDelete();




        }
        catch (Exception e)
        {
            ChivoxTest.MInstance.Callback= null;
            Debug.LogWarning("PrintResult caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }



    public void OnError(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

    public void OnEvalResult(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

    public void OnBinResult(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

    public void OnVad(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

    public void OnSoundIntensity(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

    public void OnOther(string tokenID, EvalResult result)
    {
        PrintResult(tokenID, result);
    }

}

