﻿using System;
using System.IO;
using UnityEngine;

using LitJson;

using ChivoxAIEngine;
using UnityEngine.UI;



public class AIEngineTest : MonoBehaviour
{
    // Use this for initialization


    public Text ScoreText;


    public InputField inputField;

    public static JsonData myresult  ;

    public static bool iscallback = false;

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

    // Update is called once per frame
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
        if (iscallback)
        {
            iscallback = false;
         
          ScoreText.text = myresult["result"]["overall"].ToJson();
           


        }



    }



    private static AIEngineTest MInstance = null;
    private string appKey = "1541408593000042";
    private string secretKey = "8682d02ea4d6d199ae156c3961efbf62";

    private string MProvisionPath = null;
    private string MAudioPath = null;
    private string MLogPath = null;
    private string MRocordPath = null;

    private Engine MEngine = null;
    private readonly IEvalResultListener MListener = new AIEngineTestEvalResultListener();


    void OnCreateSuccess(Engine engine)
    {
        MEngine = engine;
        Debug.Log("create succeed");
    }

    void OnCreateFail(RetValue err)
    {
        Debug.LogWarning("create failed");
        Debug.LogWarning(err.ErrID);
        Debug.LogWarning(err.Error);
    }

    public void OnButtonNew()
    {
        try
        {
            MProvisionPath = AIEngineUtility.CopyStreamingAssets("aiengine.provision");

            MLogPath = Path.Combine(Application.persistentDataPath, "log.txt");
            MRocordPath = Path.Combine(Application.persistentDataPath, "1.wav");

            Engine.SetLogFile(MLogPath);

            Debug.Log("provisionPath: " + MProvisionPath);
            Debug.Log("rocordPath: " + MRocordPath);
            Debug.Log("OnButtonInit succeed");

            JsonData cfg = new JsonData();
            cfg["appKey"] = appKey;
            cfg["secretKey"] = secretKey;
            cfg["provision"] = "yszKzsvMzM3Mz8zPzM/MzsnLycrGmcvJzcbLy8qam8+ZzM/Hy8bGzJqbzJqbmcjKxs+anZvHnMqexpzJzZzGzs3HzMueycnNnJycm83Mz8abms/NycuZmp6ezp7Jns/Jx8vOzMbOxsidx8+ZnMybng==";
            cfg["cloud"] = new JsonData();
            cfg["cloud"]["server"] = "ws://cloud.chivox.com";

            Engine.Create(cfg, OnCreateSuccess, OnCreateFail);

            Debug.Log("cfg: " + cfg.ToJson());
            Debug.Log("OnButtonNew succeed");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonNew caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void OnButtonDelete()
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

    public void OnButtonRecordStart()
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
            param["request"]["coreType"] = "en.sent.score";
            param["request"]["refText"] = inputField.text;   //  "I know the place very well.";
            param["request"]["attachAudioUrl"] = 1;
            string tokenID;
            AudioSrc.InnerRecorder audioSrc = new AudioSrc.InnerRecorder
            {
                recordParam = new ChivoxMedia.RecordParam
                {
                    Duration = 5500,
                    SaveFile = MRocordPath
                }
            };
            MEngine.Start(audioSrc, out tokenID, param, MListener);

            Debug.Log("param: " + param.ToJson());
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

    public void OnButtonRecordStop()
    {
        try
        {
            MEngine.Stop();

            Debug.Log("OnButtonRecordStop succeed ++++++++++++++++++++++++++++++++++++++++++++++"  + "<color=yellow>"+ DateTime.Now +  DateTime.Now.Millisecond+"</color>");
        }
        catch (Exception e)
        {
            Debug.LogWarning("OnButtonRecordStop caught an exception");
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    public void OnButtonRecordCancel()
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



    public class AIEngineTestEvalResultListener : IEvalResultListener
    {
        private static void PrintResult(string tokenID, EvalResult result)
        {
            try
            {
                if (result.RecFilePath != null)
                {
                    AIEngineTest.MInstance.MAudioPath = result.RecFilePath;
                }

                Debug.Log("On Result");
                Debug.Log("tokenID: " + tokenID);
                Debug.Log("tokenID: " + result.TokenID);
                Debug.Log("result: " + result.Text);
                myresult = new JsonData();
                Debug.Log("  收到消息 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + "<color=yellow>"+DateTime.Now + DateTime.Now.Millisecond+ "</color>");
                myresult = LitJson.JsonMapper.ToObject( result.Text);
                iscallback = true;
                Debug.Log("path: " + result.RecFilePath);
            }
            catch (Exception e)
            {
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