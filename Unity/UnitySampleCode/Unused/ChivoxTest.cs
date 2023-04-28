
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;
using ChivoxAIEngine;

public class ChivoxTest : MonoBehaviour
{



    public static ChivoxTest MInstance = null;
    private string appKey = "1541408593000042";
    private string secretKey = "8682d02ea4d6d199ae156c3961efbf62";

    public string MProvisionPath = null;
    public string MAudioPath = null;
    public string MLogPath = null;
    public string MRocordPath = null;

    private Engine MEngine = null;
    private readonly IEvalResultListener MListener = new EvalResultListener();












    private void Start()
    {
        try
        {
            MInstance = this;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Start caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }
    private void Update()
    {
        try
        {
            Engine.StaticUnityUpdate();

            if (MEngine != null)
            {
                MEngine.UnityUpdate();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Update caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }




    }
    void OnCreateSuccess(Engine engine)
    {
        MEngine = engine;
        Debug.Log("create succeed");
        RecordStart();
    }

    void OnCreateFail(RetValue err)
    {
        Debug.LogWarning("create failed      sdk  启动失败之后添加逻辑 ++++++++++++++++++++ ");
        Debug.LogWarning(err.ErrID);
        Debug.LogWarning(err.Error);
    }
    public Action Callback;
    public void StartToConnect(Action actionCallback)
    {
        this.Callback = actionCallback;
        try
        {
            MProvisionPath = AIEngineUtility.CopyStreamingAssets("aiengine.provision");

            MLogPath = Path.Combine(Application.persistentDataPath, "log.txt");
            string forderpath = Application.persistentDataPath + "/AIRecord";
            if (!Directory.Exists(forderpath))
            {
                Directory.CreateDirectory(forderpath);

            }


            MRocordPath = Path.Combine(forderpath,  "xxx.wav");

            Engine.SetLogFile(MLogPath);

            Debug.Log("provisionPath: " + MProvisionPath);
            Debug.Log("rocordPath: " + MRocordPath);
            Debug.Log("OnButtonInit succeed");

            JsonData cfg = new JsonData();
            cfg["appKey"] = appKey;
            cfg["secretKey"] = secretKey;
            cfg["provision"] = "yszKzsvMzM3Mz8zPzM/MzsnLycrGmcvJzcbLy8qam8+ZzM/Hy8bGzJqbzJqbmcjKxs+anZvHnMqexpzJzZzGzs3HzMueycnNnJycm83Mz8abms/NycuZmp6ezp7Jns/Jx8vOzMbOxsidx8+ZnMybng==";   //MProvisionPath;
            cfg["cloud"] = new JsonData();
            cfg["cloud"]["server"] = "ws://cloud.chivox.com";
            Debug.Log("cfg: " + cfg.ToJson());
            Engine.Create(cfg, OnCreateSuccess, OnCreateFail);


            Debug.Log("OnButtonNew succeed");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonNew caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void OnDelete()
    {
        try
        {
            MEngine = null;

            Debug.Log("OnButtonDelete succeed");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonDelete caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void RecordStart()
    {
        try
        {
            JsonData param = new JsonData();
            param["coreProvideType"] = "cloud";
            param["app"] = new JsonData();
            param["app"]["userId"] = "this-is-user-id";
            param["audio"] = new JsonData();
            param["audio"]["audioType"] = "wav";
            param["audio"]["channel"] = 1;
            param["audio"]["sampleBytes"] = 2;
            param["audio"]["sampleRate"] = 16000;
            param["request"] = new JsonData();
            param["request"]["coreType"] = "";//UIViewUtil.CurrentLinkName == "句子" ? "en.sent.score" : "en.word.score";
            param["request"]["refText"] = ""; //AppBaseDefine.tempRecordList;  //"I know the place very well.";

            string tokenID;
            AudioSrc.InnerRecorder audioSrc = new AudioSrc.InnerRecorder
            {
                recordParam = new ChivoxMedia.RecordParam
                {
                    Duration = 5500,



                    SaveFile = MRocordPath



                }
            };

            Debug.Log("param: " + param.ToJson());


            MEngine.Start(audioSrc, out tokenID, param, MListener);
            Debug.Log("tokenID: " + tokenID);
            Debug.Log("OnButtonRecordStart succeed");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonRecordStart caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void RecordStop()
    {
        try
        {
            MEngine.Stop();

            Debug.Log("OnButtonRecordStop succeed ++++++++++++++++++++++++++++++++++++++++++++++" + "<color=yellow>" + DateTime.Now + DateTime.Now.Millisecond + "</color>");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonRecordStop caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void RecordCancel()
    {
        try
        {
            MEngine.Cancel();

            Debug.Log("OnButtonRecordCancel succeed");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonRecordCancel caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void OnButtonPlayAudio()
    {
        try
        {
            ChivoxMedia.AudioPlayer.SharedInstance().MAudioSource = GetComponent<AudioSource>();
            ChivoxMedia.AudioPlayer.SharedInstance().PlayOneShot(MAudioPath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonPlayAudio caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }






    public class EvalResultListener : IEvalResultListener
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
                        //{
                        //    AOutput.Log("已存在假分 = " + AppBaseDefine.soundScore);
                        //    WebSocketConnector.Instance.bConnecting = false;
                        //    WebSocketConnector.Instance.bConnected = false;
                        //    return;
                        //}

                        jo = jo["result"];

                        var tempScore = 0;
                        if (jo.ContainsKey("integrity"))
                        {

                            Debug.Log($"完整度: {jo["integrity"].ToString()} " +
                                $"准确度:{jo["accuracy"].ToString()} " +
                                $"信噪比:{jo["info"]["snr"].ToString()} " +
                                $"音量:{jo["info"]["volume"].ToString()} " + $"总分:{ jo["overall"].ToString()   }");

                            var integrity = jo["integrity"].ToString().ToInt();
                            var accuracy = jo["accuracy"].ToString().ToInt();
                            //临时分数  = 0.3 完整度  +  准确度  0.7  
                            tempScore = (int)(integrity * 0.3 + accuracy * 0.7);

                            var volume = jo["info"]["volume"].ToString().ToInt();
                            var overall = jo["overall"].ToString().ToInt();

                            if (volume <= 300)
                            {
                                tempScore = 0;
                            }
                            else if (overall < 30)
                            {
                                //额外分数 =  随机加上最高15   再加 10  
                                var irdm = UnityEngine.Random.Range(15 ,10);

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
                            tempScore = jo["overall"].ToString().ToInt();
                        }
                        //AppBaseDefine.soundScore = tempScore.ToString();
                        //AppBaseDefine.continueScore = tempScore.ToString();
                        Debug.Log("  Callback   " + MInstance.Callback == null);
                        ChivoxTest.MInstance.Callback?.Invoke();
                        if (tempScore > 0)
                        {
                            //AudioFileManager.Instance.AudioFilePath = DownloadManager.Instance.LocalFilePath + "AIRecord/" + AppBaseDefine.tempRecordList + ".wav";
                            //AudioFileManager.Instance.AudioName = AppBaseDefine.tempRecordList + ".wav";
                            //AudioFileManager.Instance.m_ziyuancategory = 6;
                            //AudioFileManager.Instance.m_Dayid = AppBaseDefine.FromReadToThis ? AppBaseDefine.TempCurrentDay : AppBaseDefine.CurrentDay;
                            //AudioFileManager.Instance.m_Score = tempScore;
                            //AudioFileManager.Instance.IsSend = true;

                        }

                    

                }

                // ChivoxTest.MInstance.RecordCancel();
                //ChivoxTest.MInstance.OnDelete();

            }
            catch (Exception e)
            {
                ChivoxTest.MInstance.Callback = null;
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



















}
