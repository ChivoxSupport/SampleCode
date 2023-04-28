
using ChivoxAIEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public enum EnumKernelType
{
    none,

    /// <summary>
    /// Word
    /// </summary>
    enwordscore,
	
    /// <summary>
    /// Word Pronunciation Correction
    /// </summary>
    enwordpron,
	
    /// <summary>
    /// Sentence
    /// </summary>
    ensentscore,
	
    /// <summary>
    /// Sentence Pronunciation Correction
    /// </summary>
    ensentpron,
	
    /// <summary>
    /// Paragraph
    /// </summary>
    enpredscore,
	
    /// <summary>
    /// Choice
    /// </summary>
    enchocscore,
	
    /// <summary>
    /// AITalk
    /// </summary>
    ensentrecscore,
	
    /// <summary>
    /// Semi-open question
    /// </summary>
    enscneexam,
	
    /// <summary>
    /// Open question
    /// </summary>
    enprtlexam,
	
    /// <summary>
    /// ASR
    /// </summary>
    enasrrec,
	
    //==========================================================Chinese Assessment Type========================================================
    /// <summary>
    /// Word(Chinese character)
    /// </summary>
    cnwordraw,

    /// <summary>
    /// Sentence(Chinese character)
    /// </summary>
    cnsentraw,

    /// <summary>
    /// Paragraph(Chinese character)
    /// </summary>
    cnpredraw,


    /// <summary>
    /// Limited Branching(Chinese character)
    /// </summary>
    cnrecraw,

    /// <summary>
    /// AITalk(Chinese character)
    /// </summary>
    cnrecscoreraw,

    /// <summary>
    /// Word(Chinese Pinyin)
    /// </summary>
    cnwordscore
}

public class ChivoxManger : SingletonMono<ChivoxManger>
{
    private string appKey = "your appKey";
    private string secretKey = "your secretKey";

    [SerializeField]
    private string MProvisionPath = null;
    [SerializeField]
    private string MVadPath = null;
    [SerializeField]
    private string MAudioPath = null;
    [SerializeField]
    private string MLogPath = null;
    [SerializeField]
    private string MRocordPath = null;
    [SerializeField]
    private string MOutsideAudioFileName = string.Empty;

    [SerializeField]
    private Text ResultText;

    [SerializeField]
    private Text ErrorMessageTip;

    [SerializeField]
    private Button OutsideRocordButton;

    [SerializeField]
    private Button InsideRocordButton;

    [SerializeField]
    private GameObject InsideBottomPanel;

    [SerializeField]
    private GameObject OutsideBottomPanel;

    [SerializeField]
    private Button WordButton;

    [SerializeField]
    private Button SentenceButton;

    [SerializeField]
    private Button ParagraphButton;

    [SerializeField]
    private Button AITalkButton;
    [SerializeField]
    private Button ChoiceButton;
    [SerializeField]
    private Button ScneButton;
    [SerializeField]
    private Button PrtlButton;
    [SerializeField]
    private Button AsrButton;
    [SerializeField]
    private Button WordPronButton;
    [SerializeField]
    private Button SentPronButton;


    [SerializeField]
    private Button dzhzButton;

    [SerializeField]
    private Button cjhzButton;

    [SerializeField]
    private Button dlhzButton;

    [SerializeField]
    private Button yxfzhzButton;

    [SerializeField]
    private Button aihzButton;

    [SerializeField]
    private Button dzpyButton;

    [SerializeField]
    private Button cjpyButton;

    [SerializeField]
    private Button dlpyzButton;


    [SerializeField]
    private Button yxfzpyButton;

    [SerializeField]
    private Button aipyButton;


    [SerializeField]
    private Button StartRocordButton;

    [SerializeField]
    private Button StopRocordButton;

    [SerializeField]
    private Button ReplayAudioButton;

    [SerializeField]
    private Button StartSentenceEvaluatingButoon;

    [SerializeField]
    private Button StopSentenceEvaluatingButoon;

    [SerializeField]
    private GameObject WaittingPanel;

    [SerializeField]
    private InputField RecordrefText;

    [SerializeField]
    private Scrollbar LogScrollbar;

    [SerializeField]
    private AudioSource AudioSound;

    [SerializeField]
    private RectTransform Content;

    [SerializeField]
    private Button InsideCloseButton;

    [SerializeField]
    private Button OutsideCloseButton;

    [SerializeField]
    private Button switchbutton;


    [SerializeField]
    private GameObject OtherPanel;

    [SerializeField]
    private Text TitleText;

    [SerializeField]
    private Text DetailText;

    [SerializeField]
    private GameObject DZPYGroup;

    [SerializeField]
    private GameObject enpanel;


    [SerializeField]
    private GameObject cnpanel;



    [SerializeField]
    private Toggle IsNeedVad;


    [SerializeField]
    private Toggle ToggAccent;

    private string cnReftxt = string.Empty;

    public string Answer_A = string.Empty;
    public string Answer_B = string.Empty;
    public string Answer_C = string.Empty;
    public string Answer_D = string.Empty;
    private Timer CurrentTimer = null;


    private EnumKernelType enumKernelType = EnumKernelType.none;


    private Engine mEngine = null;
    private readonly IEvalResultListener MListener = new EvalResultListener();

    /// <summary>
    /// Record Type
    /// </summary>
    private string RecordType = string.Empty;


    /// <summary>
    /// Assessment callback function
    /// </summary>
    public Action<string> OnCallback;

    /// <summary>
    /// create engine success
    /// </summary>
    public Action OnCreateSuccessCallback;
	
    /// <summary>
    /// create engine fail
    /// </summary>
    public Action<int, string> OnOnCreateFailCallback;
	
    /// <summary>
    /// Assessment result
    /// </summary>
    public Action<EvalResult> OnResultCallback;
	
    /// <summary>
	/// record start callback
    /// </summary>
    public Action OnRecordStartCallback;
	
    /// <summary>
    /// record stop callback
    /// </summary>
    public Action OnRecordStopCallback;

    public bool IsOutsideAudio = false;

    protected override void Start()
    {
        base.Start();
        AudioSound = GetComponent<AudioSource>();
        ResultText = transform.Find("LeftUp/ScoreText").GetComponent<Text>();
        ErrorMessageTip = transform.Find("RightUp/Scroll View/Viewport/Content/TipText").GetComponent<Text>();
        Content = transform.Find("RightUp/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        LogScrollbar = transform.Find("RightUp/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();
        OutsideRocordButton = transform.Find("Middle/OutsideRocordButton").GetComponent<Button>();
        InsideRocordButton = transform.Find("Middle/InsideRocordButton").GetComponent<Button>();
        InsideBottomPanel = transform.Find("InsideBottom").gameObject;
        enpanel = InsideBottomPanel.transform.Find("Group").gameObject;
        cnpanel = InsideBottomPanel.transform.Find("cnGroup").gameObject;
        OutsideBottomPanel = transform.Find("OutsideBottom").gameObject;
        WordButton = InsideBottomPanel.transform.Find("Group/WordButton").GetComponent<Button>();
        SentenceButton = InsideBottomPanel.transform.Find("Group/SentenceButton").GetComponent<Button>();
        ParagraphButton = InsideBottomPanel.transform.Find("Group/ParagraphButton").GetComponent<Button>();
        AITalkButton = InsideBottomPanel.transform.Find("Group/AITalkButton").GetComponent<Button>();
        ChoiceButton = InsideBottomPanel.transform.Find("Group/ChoiceButton").GetComponent<Button>();
        ScneButton = InsideBottomPanel.transform.Find("Group/ScneButton").GetComponent<Button>();
        PrtlButton = InsideBottomPanel.transform.Find("Group/PrtlButton").GetComponent<Button>();
        AsrButton = InsideBottomPanel.transform.Find("Group/AsrButton").GetComponent<Button>();
        WordPronButton = InsideBottomPanel.transform.Find("Group/WordPronButton").GetComponent<Button>();
        dzhzButton = InsideBottomPanel.transform.Find("cnGroup/dzhz").GetComponent<Button>();
        cjhzButton = InsideBottomPanel.transform.Find("cnGroup/cjhz").GetComponent<Button>();
        dlhzButton = InsideBottomPanel.transform.Find("cnGroup/dlhz").GetComponent<Button>();
        yxfzhzButton = InsideBottomPanel.transform.Find("cnGroup/yxfzhz").GetComponent<Button>();
        aihzButton = InsideBottomPanel.transform.Find("cnGroup/aihz").GetComponent<Button>();
        dzpyButton = InsideBottomPanel.transform.Find("cnGroup/dzpy").GetComponent<Button>();
        SentPronButton = InsideBottomPanel.transform.Find("Group/SentPronButton").GetComponent<Button>();
        StartRocordButton = InsideBottomPanel.transform.Find("Button/StartRocord").GetComponent<Button>();
        StopRocordButton = InsideBottomPanel.transform.Find("Button/StopRocord").GetComponent<Button>();
        ReplayAudioButton = InsideBottomPanel.transform.Find("Button/ReplayAudio").GetComponent<Button>();
        DZPYGroup = InsideBottomPanel.transform.Find("Button/group").gameObject;
        RecordrefText = InsideBottomPanel.transform.Find("InputField").GetComponent<InputField>();
        InsideCloseButton = InsideBottomPanel.transform.Find("CloseButton").GetComponent<Button>();
        OtherPanel = InsideBottomPanel.transform.Find("OtherPanel").gameObject;
        switchbutton = InsideBottomPanel.transform.Find("switchButton").GetComponent<Button>();
        IsNeedVad = InsideBottomPanel.transform.Find("VadToggle").GetComponent<Toggle>();
        ToggAccent = InsideBottomPanel.transform.Find("AccentToggle").GetComponent<Toggle>();
        TitleText = OtherPanel.transform.Find("Title").GetComponent<Text>();
        DetailText = OtherPanel.transform.Find("Detail").GetComponent<Text>();
        StartSentenceEvaluatingButoon = OutsideBottomPanel.transform.Find("StartEvaluating").GetComponent<Button>();
        StopSentenceEvaluatingButoon = OutsideBottomPanel.transform.Find("StopEvaluating").GetComponent<Button>();
        OutsideCloseButton = OutsideBottomPanel.transform.Find("CloseButton").GetComponent<Button>();
        WaittingPanel = transform.Find("WaittingPanel").gameObject;
        MOutsideAudioFileName = "I want to know the past and present of hongkong";
        OutsideRocordButton.onClick.AddListener(() =>
        {
            if (InsideBottomPanel != null)
            {
                if (InsideBottomPanel.activeSelf)
                {
                    InsideBottomPanel.gameObject.SetActive(false);
                }
            }
            else
            {

                Debug.Log($"{" InsideBottomPanel is  null!"}");
            }
            if (OutsideBottomPanel != null)
            {
                if (!OutsideBottomPanel.activeSelf)
                {
                    RecordrefText.text = string.Empty;
                    OutsideBottomPanel.gameObject.SetActive(true);
                    IsOutsideAudio = true;
                    RecordrefText.text = MOutsideAudioFileName;
                }
            }
            else
            {

                Debug.Log($"{ "OutsideBottomPanel is  null!"}");
            }
        });

        InsideRocordButton.onClick.AddListener(() =>
        {
            if (OutsideBottomPanel != null)
            {
                if (OutsideBottomPanel.activeSelf)
                {
                    OutsideBottomPanel.gameObject.SetActive(false);
                }
            }
            else
            {

                Debug.Log($"{ "OutsideBottomPanel is  null!"}");
            }
            if (InsideBottomPanel != null)
            {
                if (!InsideBottomPanel.activeSelf)
                {
                    RecordrefText.text = string.Empty;
                    InsideBottomPanel.gameObject.SetActive(true);
                    IsOutsideAudio = false;
                }
            }
            else
            {

                Debug.Log($"{ "InsideBottomPanel is  null!"}");
            }

        });
        var groupcontent = DZPYGroup.transform.Find("1").transform;
        for (int i = 0; i < groupcontent.childCount; i++)
        {
            var button = groupcontent.GetChild(i).GetComponent<Button>();
            button.onClick.AddListener(() =>
            {

                RecordrefText.text = button.transform.GetChild(0).GetComponent<Text>().text;

            });

        }
        switchbutton.onClick.AddListener(() =>
        {

            if (enpanel.activeSelf)
            {
                enpanel.SetActive(false);
                cnpanel.SetActive(true);
            }
            else if (cnpanel.activeSelf)
            {
                cnpanel.SetActive(false);
                enpanel.SetActive(true);

            }
            RecordrefText.text = string.Empty;
            RecordrefText.gameObject.SetActive(true);
            RecordrefText.interactable = true;
            ErrorMessageTip.text = string.Empty;
            ResultText.text = string.Empty;
            OtherPanel.SetActive(false);
            DZPYGroup.SetActive(false);

        });
        dzhzButton.onClick.AddListener(() =>
        {
            ChangeRecordType(dzhzButton);
        });
        cjhzButton.onClick.AddListener(() =>
        {
            ChangeRecordType(cjhzButton);
        });
        dlhzButton.onClick.AddListener(() =>
        {
            ChangeRecordType(dlhzButton);
        });
        yxfzhzButton.onClick.AddListener(() =>
        {
            ChangeRecordType(yxfzhzButton);
        });
        aihzButton.onClick.AddListener(() =>
        {
            ChangeRecordType(aihzButton);
        });
        dzpyButton.onClick.AddListener(() =>
        {
            ChangeRecordType(dzpyButton);
        });
        WordButton.onClick.AddListener(() =>
        {
            ChangeRecordType(WordButton);
        });
        SentenceButton.onClick.AddListener(() =>
        {
            ChangeRecordType(SentenceButton);
        });
        ParagraphButton.onClick.AddListener(() =>
        {
            ChangeRecordType(ParagraphButton);
        });
        AITalkButton.onClick.AddListener(() =>
        {
            ChangeRecordType(AITalkButton);
        });
        ChoiceButton.onClick.AddListener(() =>
        {
            ChangeRecordType(ChoiceButton);
        });
        ScneButton.onClick.AddListener(() =>
        {
            ChangeRecordType(ScneButton);
        });
        PrtlButton.onClick.AddListener(() =>
        {
            ChangeRecordType(PrtlButton);
        });
        AsrButton.onClick.AddListener(() =>
        {
            ChangeRecordType(AsrButton);
        });
        WordPronButton.onClick.AddListener(() =>
        {
            ChangeRecordType(WordPronButton);
        });
        SentPronButton.onClick.AddListener(() =>
        {
            ChangeRecordType(SentPronButton);
        });
        StartRocordButton.onClick.AddListener(RecordStart);
        StopRocordButton.onClick.AddListener(RecordStop);
        ReplayAudioButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnButtonPlayAudio());
        });
        StartSentenceEvaluatingButoon.onClick.AddListener(() =>
        {
            OnOutsideAudioEvaluating(() =>
            {

                RecordStop();
            });
        });
        InsideCloseButton.onClick.AddListener(() =>
        {

            if (InsideBottomPanel != null)
            {
                if (InsideBottomPanel.activeSelf)
                {
                    ResultText.text = string.Empty;
                    InsideBottomPanel.gameObject.SetActive(false);
                    ErrorMessageTip.text = string.Empty;
                }
            }

        });
        OutsideCloseButton.onClick.AddListener(() =>
        {

            if (OutsideBottomPanel != null)
            {
                if (OutsideBottomPanel.activeSelf)
                {
                    ResultText.text = string.Empty;
                    OutsideBottomPanel.gameObject.SetActive(false);
                    ErrorMessageTip.text = string.Empty;
                }
            }

        });

        InitChivoxSDK();
    }

    void ChangeRecordType(Button btnName)
    {
        // IsNeedVad.isOn = false;
        RecordrefText.gameObject.SetActive(true);
        RecordrefText.interactable = true;
        ErrorMessageTip.text = string.Empty;
        ResultText.text = string.Empty;
        OtherPanel.SetActive(false);
        DZPYGroup.SetActive(false);
        switch (btnName.name)
        {
            case "WordButton":
                RecordType = "word";
                enumKernelType = EnumKernelType.enwordscore;
                RecordrefText.text = "Apple";
                break;

            case "SentenceButton":
                RecordType = "sent";
                enumKernelType = EnumKernelType.ensentscore;
                RecordrefText.text = "I want to know the past and present of Hong Kong.";
                break;

            case "ParagraphButton":
                RecordType = "pred";
                enumKernelType = EnumKernelType.enpredscore;
                RecordrefText.text = "It was Sunday. I never get up early on Sundays. I sometimes stay in bed until lunchtime. Last Sunday I got up very late. I looked out of the window. It was dark outside.";
                break;
            case "AITalkButton":
                RecordType = "aitalk";
                enumKernelType = EnumKernelType.ensentrecscore;
                RecordrefText.text = " I go to school on foot. | I walk to school. | I go to school by bus. |I go to school.";
                break;
            case "ChoiceButton":
                RecordType = "rec";
                enumKernelType = EnumKernelType.enchocscore;
                RecordrefText.text = "Today is Friday.|Beijing.|London.|Paris.|New York.";
                OtherPanel.SetActive(true);
                TitleText.text = "Tell me an animal that can fly.";
                Answer_A = "A: Tiger.";
                Answer_B = "B: Panda.";
                Answer_C = "C: Bird.";
                Answer_D = string.Empty;
                SetText();
                break;
            case "ScneButton":
                RecordType = "scne";
                enumKernelType = EnumKernelType.enscneexam;
                RecordrefText.text = "null";
                OtherPanel.SetActive(true);
                TitleText.text = "Please answer the question based on the text below.";
                Answer_A = "Boy: I love summer because i can go to the beach. Which season is your favourite";
                Answer_B = "Girl: Spring, it's warm but it's not too hot. There are beautiful tress and flowers everywhere";
                Answer_C = "Question: Why does the boy like summer?";
                Answer_D = "Anwser:______________________________";
                SetText();
                break;
            case "PrtlButton":
                RecordType = "prtl";
                enumKernelType = EnumKernelType.enprtlexam;
                RecordrefText.text = "null";
                OtherPanel.SetActive(true);
                TitleText.text = "Oral Composition :";
                Answer_A = "1.Carmen的哥哥是一位音乐家,他在一个乐队中演奏.他住在城市的一间公寓里,和他的狗 Rascal 住在一起.";
                Answer_B = "2.通常每个周末他都会和乐队一起演奏,但是这个周末他们要去乡村攀岩.";
                Answer_C = "3.Carmen下周六将会见到哥哥,因为那天是他的生日,他哥哥会来吃晚饭.";
                Answer_D = string.Empty;
                SetText();
                break;
            case "AsrButton":
                RecordType = "asr";
                enumKernelType = EnumKernelType.enasrrec;
                RecordrefText.text = "Please speak English after clicking Record button.";
                RecordrefText.interactable = false;
                break;
            case "WordPronButton":
                RecordType = "wordpron";
                enumKernelType = EnumKernelType.enwordpron;
                RecordrefText.text = "Ball";
                break;
            case "SentPronButton":
                RecordType = "sentpron";
                enumKernelType = EnumKernelType.ensentpron;
                RecordrefText.text = "You cannot count on anyone except yourself.";
                break;

            case "dzhz":
                enumKernelType = EnumKernelType.cnwordraw;
                RecordrefText.text = "hǎi\n海";
                cnReftxt = "海";
                //  Debug.Log(RecordrefText.text.Split('\n')[1]);
                break;

            case "cjhz":
                enumKernelType = EnumKernelType.cnsentraw;
                RecordrefText.text = "zhǔ  chí  rén  hé  lái  bīn  yī  wèn  yī  dá\n" +
                                    "  主  持   人   和  来   宾   一   问  一  答";
                cnReftxt = "主持人和来宾一问一答";
                break;
            case "dlhz":
                enumKernelType = EnumKernelType.cnpredraw;
                RecordrefText.text = "qiū  tiān  shì  jīn  huáng  de  jì  jié  xiāng  tián  kě  kǒu  de  shi  zi  guà  zai  zhi  tóu  \n " +
                                     "秋   天    是    金     黄    的   季   节,    香      甜    可   口    的    柿   子   挂    在    枝     头,  " +
                                     "\nxiàng  yí  gè  gè  xiǎo  deng  lóng  yi  yàng  tián  ye  li  dào  zi  chéng  shú  le " +
                                     "\n 像     一   个    个    小     灯    笼    一    样.   田     野    里,  稻   子    成    熟    了   " +
                                     "\nyuăn  yuăn  kan  qù  xiàng  jīn  sè  de  hǎi  yáng  wéi  fēng  chuī lái  dào  làng  fān  gún" +
                                     "\n远       远      看    去,    像      金    色   的    海   洋,   微    风     吹   来,    稻     浪    翻    滚. ";
                cnReftxt = "秋天是金黄的季节,香甜可口的柿子挂在枝头,像一个个小灯笼一样.田野里,稻子成熟了,远远看去,像金色的海洋,微风吹来,稻浪翻滚.";
                break;

            case "yxfzhz":
                enumKernelType = EnumKernelType.cnrecraw;
                RecordrefText.text = "一件衬衣|一条裙子|一条裤子";
                OtherPanel.SetActive(true);
                TitleText.text = "机器人:欢迎光临!请问,您要买什么? ";
                Answer_B = "A: 一件衬衣";
                Answer_C = "B: 一条裙子";
                Answer_D = "C: 一条裤子";
                Answer_A = "你说: ...";
                SetText();

                break;
            case "aihz":
                enumKernelType = EnumKernelType.cnrecscoreraw;
                RecordrefText.text = "今天天气晴朗|今天天气阴沉沉的|我喜欢今天的天气";

                break;
            case "dzpy":
                enumKernelType = EnumKernelType.cnwordscore;
                RecordrefText.gameObject.SetActive(false);
                DZPYGroup.SetActive(true);
                break;

            default:
                break;
        }


    }

    public void SetText()
    {

        DetailText.text = Answer_A + "\n" + Answer_B + "\n" + Answer_C + "\n" + Answer_D;

    }

    /// <summary>
    /// Get kernel type
    /// </summary>
    /// <returns></returns>
    string GetRecordType()
    {
        string type = string.Empty;
        switch (enumKernelType)
        {
            case EnumKernelType.none:
                break;
            case EnumKernelType.enwordscore:
                type = "en.word.score";
                break;
            case EnumKernelType.enwordpron:
                type = "en.word.pron";
                break;
            case EnumKernelType.ensentscore:
                type = "en.sent.score";
                break;
            case EnumKernelType.ensentpron:
                type = "en.sent.pron";
                break;
            case EnumKernelType.enpredscore:
                type = "en.pred.score";
                break;
            case EnumKernelType.enchocscore:
                type = "en.choc.score";
                break;
            case EnumKernelType.ensentrecscore:
                type = "en.sent.recscore";
                break;
            case EnumKernelType.enscneexam:
                type = "en.scne.exam";
                break;
            case EnumKernelType.enprtlexam:
                type = "en.prtl.exam";
                break;
            case EnumKernelType.enasrrec:
                type = "en.asr.rec";
                break;
            case EnumKernelType.cnwordraw:
                type = "cn.word.raw";
                break;
            case EnumKernelType.cnsentraw:
                type = "cn.sent.raw";
                break;
            case EnumKernelType.cnpredraw:
                type = "cn.pred.raw";
                break;
            case EnumKernelType.cnrecraw:
                type = "cn.rec.raw";
                break;
            case EnumKernelType.cnrecscoreraw:
                type = "cn.recscore.raw";
                break;
            case EnumKernelType.cnwordscore:
                type = "cn.word.score";
                break;
            default:
                break;
        }

        return type;

    }
    /// <summary>
    /// Init Chivox SDK
    /// </summary>
    public void InitChivoxSDK()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //Request web recording permission
        ChivoxFrameWork.WebGL.Microphone.GetUserMedia();

#endif
        // try
        // {
        MProvisionPath = AIEngineUtility.CopyStreamingAssets("aiengine.provision");
        MVadPath = AIEngineUtility.CopyStreamingAssets("vad.0.13.bin");
        MLogPath = Path.Combine(Application.persistentDataPath, "log.txt");
        MAudioPath = Application.streamingAssetsPath +  "/outside.wav";
        string forderpath = Application.persistentDataPath + "/AIRecord";
        if (!Directory.Exists(forderpath))
        {
            Directory.CreateDirectory(forderpath);
        }

        MRocordPath = Path.Combine(forderpath, string.Format("{0}.wav", "inside"));
        // Debug.Log("provisionPath: " + MProvisionPath);
        //  Debug.Log("rocordPath: " + MRocordPath);
        JsonData cfg = new JsonData
        {
            ["appKey"] = appKey,
            ["secretKey"] = secretKey,
            ["provision"] = MProvisionPath,
            //["serverTimeout"] =180,
            ["cloud"] = new JsonData
            {
                ["server"] = "wss://cloud.chivox.com:443"
            },
            ["vad"] = new JsonData()

        };
        cfg["vad"]["enable"] = IsNeedVad.isOn ? 1 : 0; // whether load the vad module
        cfg["vad"]["res"] = MVadPath;

        // Debug.Log("cfg: " + cfg.ToJson());
		
		//Create an engine. After creation, success is returned through function OnCreateSuccess, and failure is returned through function OnCreateFail
        Engine.Create(cfg, OnCreateSuccess, OnCreateFail);
        // Debug.Log("SDKInit succeed");

        Debug.Log($"Cfg:{  cfg.ToJson()}");

        // }
        //catch (Exception e)
        //{
        //    Debug.Log("SDK caught an exception");
        //    Debug.Log(e.Message);
        //    Debug.Log(e.StackTrace);
        //}
    }

    void OnCreateSuccess(Engine engine)
    {
		//Created successfully, save the engine instance for subsequent use
        mEngine = engine;
        OnCreateSuccessCallback?.Invoke();
        Debug.Log($"{"SDK  create succeed" }");

    }

    void OnCreateFail(RetValue err)
    {
		//Creation failed, please check e.errId and e.error to analyze the reason.
        OnOnCreateFailCallback?.Invoke(err.ErrID, err.Error);
        Debug.Log("create failed");
        Debug.Log(err.ErrID);
        Debug.Log(err.Error);

    }
    public void ShowLog(string mes)
    {

        ErrorMessageTip.text += mes + "\n\n\n\n";
        CurrentTimer = null;
        CurrentTimer = new Timer(0.5f);
        CurrentTimer.tick += () =>
        {
            Content.GetComponent<VerticalLayoutGroup>().enabled = false;
            Content.GetComponent<VerticalLayoutGroup>().enabled = true;
            Content.localPosition = new Vector2(0, 0);
            LogScrollbar.value = 0;  
        };
        CurrentTimer.Start();

    }
    private void Update()
    {

        if (CurrentTimer != null)
        {
            CurrentTimer.Update(Time.deltaTime);
        }

        Engine.StaticUnityUpdate();
        if (mEngine != null)
        {
            mEngine.UnityUpdate();
        }

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        RecordDelete();

    }
    /// <summary>
    /// Destroy the engine
    /// </summary>
    public void RecordDelete()
    {
        try
        {
            if (mEngine == null)
                return;
            mEngine.Destroy();
            mEngine = null;
            Debug.Log("OnButtonDelete succeed");

        }
        catch (Exception e)
        {
            Debug.Log("OnButtonDelete caught an exception");
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }


    /// <summary>
    /// Get record duration
    /// </summary>
    /// <returns></returns>
    int GetRecordDuration()
    {
        int dur = 0;
        switch (enumKernelType)
        {

            case EnumKernelType.enwordscore:
                dur = 15;
                break;
            case EnumKernelType.enwordpron:
                dur = 15;
                break;
            case EnumKernelType.ensentscore:
                dur = 35;
                break;
            case EnumKernelType.ensentpron:
                dur = 35;
                break;
            case EnumKernelType.enpredscore:
                dur = 175;
                break;
            case EnumKernelType.enchocscore:
                dur = 35;
                break;
            case EnumKernelType.ensentrecscore:
                dur = 45;
                break;
            case EnumKernelType.enscneexam:
                dur = 115;
                break;
            case EnumKernelType.enprtlexam:
                dur = 115;
                break;
            case EnumKernelType.enasrrec:
                dur = 90;
                break;
            case EnumKernelType.cnwordraw:

                dur = 20;

                break;
            case EnumKernelType.cnsentraw:

                dur = 40;
                break;
            case EnumKernelType.cnpredraw:

                dur = 300;
                break;
            case EnumKernelType.cnrecraw:

                dur = 40;
                break;
            case EnumKernelType.cnrecscoreraw:

                dur = 40;
                break;
            case EnumKernelType.cnwordscore:

                dur = 20;
                break;
            default:
                break;
        }


        return dur * 1000;


    }

    /// <summary>
    /// start record
    /// </summary>
    public void RecordStart()
    {
        Debug.Log("Record duration：" + GetRecordDuration());
        if (string.IsNullOrEmpty(GetRecordType()))
        {
            Debug.Log("Plaase choose record type!");
            return;
        }
        if (string.IsNullOrEmpty(RecordrefText.text))
        {
            Debug.Log("Please input assessment text");
            return;
        }
        Debug.Log("coreType " + GetRecordType());

        //try
        //{
        if (File.Exists(MRocordPath))
        {
            File.Delete(MRocordPath);
        }
        ResultText.text = string.Empty;
        ErrorMessageTip.text = string.Empty;
        if (mEngine == null)
            return;
        JsonData param = new JsonData
        {
            ["coreProvideType"] = "cloud",
            ["vad"] = new JsonData
            {
                ["vadEnable"] = IsNeedVad.isOn ? 1 : 0,
                ["refDuration"] = 2,
                ["speechLowSeek"] = 50

            },
            ["app"] = new JsonData()

        };
        //param["vad"]["vadEnable"] = IsNeedVad.isOn ? 1 : 0;  //Whether enable vad :  1. enbale 0. disable
        //param["vad"]["refDuration"] = 1;                     //Set the audio vad delay to take effect (unit: second), block VAD within a few seconds of the first recording.
        //param["vad"]["speechLowSeek"] = 30;                  //Sensitivity, the unit is 10ms, set to N, which means that 10*N milliseconds after the speech stops are judged to be over.
        param["app"]["userId"] = "DK_Test";
        param["audio"] = new JsonData
        {
            ["audioType"] = "wav",
            ["channel"] = 1,
            ["sampleBytes"] = 2,
            ["sampleRate"] = 16000
        };
        JsonData requestparm = new JsonData();
        requestparm["coreType"] = GetRecordType();
        requestparm["accent"] = ToggAccent.isOn ? 1 : 2; // 1 British pronunciation 2 American pronunciation
        requestparm["rank"] = 100;
        requestparm["precision"] = 1;
        requestparm["attachAudioUrl"] = 1;

        //set kernel parameters
        switch (enumKernelType)
        {

            case EnumKernelType.enwordscore:
                requestparm["refText"] = RecordrefText.text;
                break;
				
            case EnumKernelType.enwordpron:
                requestparm["refText"] = RecordrefText.text;
                MyRoot enwordpronmyRoot = new MyRoot();
                enwordpronmyRoot.details.Add("overall_adjust", 0); //Increase or reduce the evaluation scores. The value range is between[-1,1] and the precision is 0.1 .
                var JsonenwordpronmyRoot = JsonMapper.ToJson(enwordpronmyRoot);
                requestparm["result"] = JsonMapper.ToObject(JsonenwordpronmyRoot);

                break;
            case EnumKernelType.ensentscore:
                requestparm["refText"] = RecordrefText.text;

                MyRoot ensentscore = new MyRoot();
                ensentscore.details.Add("phone", 1);  //Enable to return the phoneme dimension in the evaluation result. 

                var Jsonensentscore = JsonMapper.ToJson(ensentscore);
                requestparm["result"] = JsonMapper.ToObject(Jsonensentscore);
                break;
				
            case EnumKernelType.ensentpron:
                requestparm["refText"] = RecordrefText.text; // RecordrefText.text;
                MyRoot ensentpron = new MyRoot();
                ensentpron.details.Add("phone", 0);  //Enable to return the phoneme dimension in the evaluation result. 
                ensentpron.details.Add("ext_cur_wrd", 0);//Return the read content in real time. 
                var Jsonensentpron = JsonMapper.ToJson(ensentpron);
                requestparm["result"] = JsonMapper.ToObject(Jsonensentpron);
                break;
				
            case EnumKernelType.enpredscore:
                requestparm["refText"] = RecordrefText.text;
                break;
				
            case EnumKernelType.enchocscore:
			
				requestparm["refText"] = JsonMapper.ToObject("{\"lm\": [{\"answer\": 0,\"text\": \"Tiger\"}, {\"answer\": 0,\"text\": \"Panda\"},{\"answer\": 1,\"text\": \"Bird\"}]}"); 
				
                break;
            case EnumKernelType.ensentrecscore:
                requestparm["keyWords"] = "go to |school | on foot # walk to | school";    //aitalk keywords
                requestparm["refText"] = RecordrefText.text;
                break;
            case EnumKernelType.enscneexam:
                requestparm["precision"] = 1;
                Root answerlist = new Root();

                answerlist.lm.Add(new Dictionary<string, string> { { "text", "He likes summer because he can go to the beach." } });
                answerlist.lm.Add(new Dictionary<string, string> { { "text", "He likes summer because he can go to school." } });
                answerlist.lm.Add(new Dictionary<string, string> { { "text", "He likes summer because he can go shopping." } });

                var temp = JsonMapper.ToJson(answerlist);

                requestparm["refText"] = JsonMapper.ToObject(temp);

                break;

            case EnumKernelType.enprtlexam:

                requestparm["precision"] = 1;
                Root answerlist1 = new Root();
				
                answerlist1.lm.Add(new Dictionary<string, string> { { "text", "Carmen'sbrother is a musician . He plays in a band . He lives in an apartment in the city . He lives with his dog Rascal ." } });
                answerlist1.lm.Add(new Dictionary<string, string> { { "text", "He lives with his dog Rascal . He usually plays with the band every weekend , but this weekendthey'regoing to the country to go rock climbing . " } });
                answerlist1.lm.Add(new Dictionary<string, string> { { "text", "Carmen will see his brother next Saturday , because it will be his birthday ." } });

                var temp1 = JsonMapper.ToJson(answerlist1);

                requestparm["refText"] = JsonMapper.ToObject(temp1);

                break;
            case EnumKernelType.enasrrec:

                requestparm["res"] = "en.asr.G4";

                break;
            case EnumKernelType.cnwordraw:

                requestparm["res"] = "chn.wrd.G4.A2";      //For children groups, please configure "chn.wrd.G4.A2"  For adult groups, please configure "chn.wrd.G4.A7"
                requestparm["refText"] = cnReftxt;

                break;
				
            case EnumKernelType.cnsentraw:
                requestparm["res"] = "chn.snt.G4.A2";      //For children groups, please configure "chn.snt.G4.A2" For adult groups, please configure "chn.snt.G4.A7"
                requestparm["refText"] = cnReftxt;
                break;
				
            case EnumKernelType.cnpredraw:
                requestparm["refText"] = cnReftxt;
                MyRoot cnpredraw = new MyRoot();
                cnpredraw.details.Add("word", 0);

                var Jsoncnpredraw = JsonMapper.ToJson(cnpredraw);
                requestparm["result"] = JsonMapper.ToObject(Jsoncnpredraw);


                break;
            case EnumKernelType.cnrecraw:

                requestparm["refText"] = RecordrefText.text;
                MyRoot cnrecraw = new MyRoot();
                cnrecraw.details.Add("use_details", 1); //Enable to output the score of each Chinese character
                var Jsoncnrecraw = JsonMapper.ToJson(cnrecraw);
                requestparm["result"] = JsonMapper.ToObject(Jsoncnrecraw);


                break;
            case EnumKernelType.cnrecscoreraw:
                requestparm["refText"] = RecordrefText.text;
                requestparm["keyWords"] = "晴朗";
                MyRoot cnrecscoreraw = new MyRoot();
                cnrecscoreraw.details.Add("use_details", 1); //Enable to output the score of each Chinese character
                var Jsoncnrecscoreraw = JsonMapper.ToJson(cnrecscoreraw);
                requestparm["result"] = JsonMapper.ToObject(Jsoncnrecscoreraw);



                break;

            case EnumKernelType.cnwordscore:

                requestparm["refText"] = cnReftxt;

                MyRoot cnwordscore = new MyRoot();
                cnwordscore.details.Add("gop_adjust", 0); //Increase or reduce the evaluation scores. The value range is between[-1,1] and the precision is 0.1 . 
                var Jsoncnwordscore = JsonMapper.ToJson(cnwordscore);
                requestparm["result"] = JsonMapper.ToObject(Jsoncnwordscore);

                break;

            default:
                break;
        }






        param["request"] = requestparm;




        string tokenID;


        AudioSrc.InnerRecorder audioSrc = new AudioSrc.InnerRecorder
        {
            recordParam = new ChivoxMedia.RecordParam
            {
                Duration = GetRecordDuration(),

                SaveFile = MRocordPath
            }
        };

        Debug.Log(param.ToJson());
        mEngine.Start(audioSrc, out tokenID, param, MListener);
        //OnRecordStartCallback?.Invoke();
        //Debug.Log("tokenID: " + tokenID);
        //Debug.Log("OnButtonRecordStart succeed");
        //CurrentTimer = null;
        //CurrentTimer = new Timer(GetRecordDuration() / 1000);
        //CurrentTimer.tick += () =>
        //{
        //    RecordStop();

        //};
        //CurrentTimer.Start();
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("OnButtonRecordStart caught an exception");
        //    Debug.Log(e.Message);
        //    Debug.Log(e.StackTrace);
        //}
    }



    public void RecordStop()
    {
        try
        {
            if (mEngine == null)
                return;
            if (CurrentTimer != null)
            {

                CurrentTimer = null;
            }

            mEngine.Stop();
            OnRecordStopCallback?.Invoke();
            Debug.Log("OnButtonRecordStop succeed +++++" + "<color=yellow>" + DateTime.Now + DateTime.Now.Millisecond + "</color>");
        }
        catch (Exception e)
        {
            mEngine.Stop();
            Debug.Log("OnButtonRecordStop caught an exception");
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    public void RecordCancel()
    {
        try
        {
            if (mEngine == null)
                return;
            if (CurrentTimer != null)
            {

                CurrentTimer = null;
            }
            mEngine.Cancel();

            Debug.Log("OnButtonRecordCancel succeed");
        }
        catch (Exception e)
        {
            Debug.Log("OnButtonRecordCancel caught an exception");
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    public void ChangeWaittingPanel(bool isshow)
    {
        WaittingPanel.SetActive(isshow);
    }

    //External recording mode
    private void OnOutsideAudioEvaluating(System.Action OnComplet)
    {
        Debug.Log("External Recording!");
        ChangeWaittingPanel(true);
        ResultText.text = string.Empty;
        JsonData param = new JsonData
        {
            //use online assessment
            ["coreProvideType"] = "cloud",
            ["app"] = new JsonData()
        };
        param["app"]["userId"] = "this-is-user-id"; //Set user Id
        param["audio"] = new JsonData
        {
            ["audioType"] = "wav",
            ["channel"] = 1,
            ["sampleBytes"] = 2,
            ["sampleRate"] = 16000
        };
        MyRoot ensentscore = new MyRoot();
        ensentscore.details.Add("phone", 1);  //Enable to return the phoneme dimension in the evaluation result. 
        ensentscore.details.Add("gop_adjust", 1);
        var Jsonensentscore = JsonMapper.ToJson(ensentscore);


        param["request"] = new JsonData
        {
           
            ["voiced"] = 1,
            ["coreType"] = "en.sent.score",
            ["refText"] =  MOutsideAudioFileName,
            ["result"] = JsonMapper.ToObject(Jsonensentscore)

        };




        Debug.Log("params  : " + param.ToJson());

        mEngine.Start(new AudioSrc.OuterFeed(), out string tokenID, param, MListener);
        FileStream file = new FileStream(MAudioPath, FileMode.Open, FileAccess.Read);
        Debug.Log("MAudioPath    " + MAudioPath);
        file.Seek(44, SeekOrigin.Begin);
        byte[] buf = new byte[3200];
        int bytes;
        while ((bytes = file.Read(buf, 0, 3200)) > 0)
        {
            mEngine.Feed(buf, bytes);
        }
        file.Close();
        OnComplet?.Invoke();



    }


    private IEnumerator OnButtonPlayAudio()
    {
        if (File.Exists(MRocordPath))
        {

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(MRocordPath, UnityEngine.AudioType.UNKNOWN);

            yield return request.SendWebRequest();
            if (request.isDone)
            {
                CloseSound();
                AudioClip tempClip = DownloadHandlerAudioClip.GetContent(request);
                AudioSound.clip = tempClip;
                AudioSound.loop = false;
                AudioSound.Play();
            }

        }
        else
        {
            Debug.Log("Audiofile is not exists!  :" + MRocordPath);

        }




        //try
        //{
        //    ChivoxMedia.AudioPlayer.SharedInstance().MAudioSource = GetComponent<AudioSource>();
        //  ChivoxMedia.AudioPlayer.SharedInstance().PlayOneShot(MAudioPath);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogWarning("OnButtonPlayAudio caught an exception");
        //    Debug.LogWarning(e.Message);
        //    Debug.LogWarning(e.StackTrace);
        //}
    }

    private void CloseSound()
    {
        AudioSound.loop = false;
        if (AudioSound != null)
            AudioSound.Stop();
    }



    public void RecordFeed(byte[] data, int length)
    {
        try
        {
            if (mEngine == null)
                return;
            if (data == null)
                return;

            mEngine.Feed(data, length);
        }
        catch (Exception e)
        {
            Debug.Log("OnButtonRecordCancel caught an exception");
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    public class EvalResultListener : IEvalResultListener
    {
        private void PrintResult(string tokenID, EvalResult result)
        {
            try
            {


                //  Debug.Log("tokenID: " + tokenID);
                Debug.Log("tokenID: \n" + result.TokenID);
                Debug.Log("On Result \n" + result.Text);
                Singleton.ShowLog(result.Text);
                //Debug.Log("  Recive message+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + "<color=yellow>" + DateTime.Now + DateTime.Now.Millisecond + "</color>");
                Debug.Log("path: " + result.RecFilePath);
                Singleton.ChangeWaittingPanel(false);

                var jo1 = LitJson.JsonMapper.ToObject(result.Text);
                if (jo1.ContainsKey("vad_status"))   
                {
                    var vadstates = jo1["vad_status"].ToString();
                    if (vadstates == "2")  //Detect that the user has stopped speaking, call the aiengine.stop interface to stop sending audio data, 
                                            //and wait for the engine to feedback scoring result
                    {
                        Debug.Log($"****************************************stop speaking");
                        Singleton.RecordStop();
                    }
                }
                else
                {

                    if (jo1["result"] != null)
                    {
                        var jo = jo1["result"];


                        if (jo.ContainsKey("integrity"))
                        {
                            string overall = string.Empty; 
                            string fluency = string.Empty; 
                            string integrity = string.Empty; 
                            string accuracy = string.Empty;  
                            string othervalue = string.Empty; 
                            if (jo.ContainsKey("overall"))
                            {
                                overall = jo["overall"].ToString();
                            }
                            if (jo.ContainsKey("fluency"))
                            {
                                if (jo["fluency"].IsInt)
                                {
                                    fluency = jo["fluency"].ToString();
                                }
                                else
                                {
                                    if (jo["fluency"].ContainsKey("overall"))
                                    {
                                        fluency = jo["fluency"]["overall"].ToString();

                                    }
                                }

                            }
                            if (jo.ContainsKey("integrity"))
                            {
                                integrity = jo["integrity"].ToString();
                            }
                            if (jo.ContainsKey("accuracy"))
                            {
                                accuracy = jo["accuracy"].ToString();
                            }
                            else if (jo.ContainsKey("pron"))
                            {
                                accuracy = jo["pron"].ToString();
                            }


                            if (Singleton.enumKernelType == EnumKernelType.ensentrecscore)
                            {
                                if (jo.ContainsKey("recscore"))
                                {
                                    if (jo["recscore"].ContainsKey("matched"))
                                    {
                                        othervalue = "keywords: " + jo["recscore"]["matched"].ToJson();

                                    }

                                }


                            }


                            Singleton.ResultText.text =
                                "Overall score：" + overall + "\n" +
                               "fluency score: " + fluency + "\n" +
                               "integrity score: " + integrity + "\n" +
                               "accuracy score: " + accuracy + "\n" +
                               othervalue;

                        }
                        else
                        {
                            string recf = string.Empty; 
                            Debug.Log($"****************************************Overall score:{ jo["overall"]}");
							
                            if (Singleton.enumKernelType == EnumKernelType.enasrrec)
                            {
                                recf = jo["rec"].ToJson() == "\"\"" ? string.Empty : jo["rec"].ToJson();
                                string fluency = string.Empty;
                                string pause = string.Empty;
                                string speed = string.Empty;
                                if (jo.ContainsKey("fluency"))
                                {
                                    if (jo["fluency"].IsInt)
                                    {
                                        fluency = jo["fluency"].ToString();
                                    }
                                    else
                                    {
                                        if (jo["fluency"].ContainsKey("overall"))
                                        {
                                            fluency = jo["fluency"]["overall"].ToString();

                                        }
                                        if (jo["fluency"].ContainsKey("pause"))
                                        {
                                            pause = jo["fluency"]["pause"].ToString();

                                        }
                                        if (jo["fluency"].ContainsKey("speed"))
                                        {
                                            speed = jo["fluency"]["speed"].ToString();

                                        }

                                    }

                                }
                                Singleton.ResultText.text = $"Overall score:{ jo["overall"].ToString() + "\n" + "fluency score:" + fluency + "\n" + "pause times:" + pause + "\n" + "Speed:" + speed + "\n" + "Recognition result:  " + recf }";
                            }
                            else
                            {
                                Singleton.ResultText.text = $"Overall score:{ jo["overall"].ToJson()}";
                            }

                            //tempScore = jo["overall"].ToString().ToInt();
                        }




                    }
                }
            }
            catch (Exception e)
            {
                ChivoxManger.Singleton.OnCallback = null;
                Debug.Log("PrintResult caught an exception");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
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
            // Debug.Log("OnVad ----------------------------------------------------");
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


public class Root
{
    /// <summary>
    /// answer list
    /// </summary>
    public List<Dictionary<string, string>> lm = new List<Dictionary<string, string>>();
}
public class MyRoot
{
    /// <summary>
    /// </summary>
    public Dictionary<string, double> details = new Dictionary<string, double>();
}