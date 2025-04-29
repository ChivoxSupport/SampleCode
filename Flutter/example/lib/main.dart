import 'dart:async';
import 'dart:convert';
import 'package:permission_handler/permission_handler.dart';
import 'package:chivox_aiengine/chivox_aiengine.dart';
import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';


const appKey = "your appKey";
const secretKey = "your secretKey";
const provisionB64 ="your provision";
const userId = "Flutter-Test";

class GlobalEngine{
  static ChivoxAiengine? _engine = null;
}

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context)
  {

    aiengineNew();

    return MaterialApp(
      home: HomePage(),
    );
  }

  Future<void> aiengineNew() async {

    Map cfg = {
      "appKey": appKey,
      "secretKey": secretKey,
      "provision": provisionB64,
      "cloud": {"server": "wss://cloud.chivox.com:443"}
    };

    GlobalEngine._engine = await ChivoxAiengine.create(json.encode(cfg));

    print("after new");


    //setState(() {
    //aiengineStart();
    //});
  }

}

class HomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Chivox Speech Assessment'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // 第一行按钮
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                HomeButton('    Word    ', "en.word.score"),
                SizedBox(width: 16.0),
                HomeButton('Sentence', "en.sent.score"),
              ],
            ),
            // 第二行按钮
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                HomeButton('Paragraph', "en.pred.score"),
                SizedBox(width: 16.0),
                HomeButton('  Choice  ', "en.choc.score"),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class HomeButton extends StatelessWidget {
  final String buttonText;
  final String coreType;

  HomeButton(this.buttonText,this.coreType);

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: () {
        // 点击按钮时，打开新页面，并传递参数
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => NewPage(buttonText: buttonText, coreType: coreType),
          ),
        );
      },
      child: Text(buttonText),
    );
  }
}

class NewPage extends StatefulWidget {

  final String buttonText;
  final String coreType;

  NewPage({ required this.buttonText, required this.coreType});

  @override
  _NewPageState createState() => _NewPageState();
}

class _NewPageState extends State<NewPage>
{

  String textValue = ''; // 初始化文本框的内容

  ChivoxAiengine? _engine = GlobalEngine._engine ;

  String displayText = '';
  var refText;
  String coreType = '';
  String questionType = '';

  TextEditingController _textEditingController = TextEditingController();

  Future<void> aiengineStart() async
  {

    textValue = '';
    setState(() {});

    var audioSrc = {
      "srcType": "innerRecorder", // innerRecorder - 内置录音机
      "innerRecorderParam": {
        //"duration": 20000,
        "channel": 1,
        "sampleBytes": 2,
        "sampleRate": 16000,
      },
    };

    var param = {
      "soundIntensityEnable": 1,
      "coreProvideType": "cloud",
      "app": {
        "userId": userId
      },
      "vad": {
        "vadEnable": 0,
        "refDuration": 2,
        "speechLowSeek": 50
      },
      "audio": {
        "audioType": "wav",
        "sampleRate": 16000,
        "sampleBytes": 2,
        "channel": 1
      },
      "request": {
        "rank": 100,
        "refText": refText,
        "coreType":coreType,
        "attachAudioUrl": 1
      }
    };

    print(param);


    print(_engine);
    await _engine!.start(
        audioSrc,
        json.encode(param),
        ChivoxAiengineResultListener(
            onEvalResult: (ChivoxAiengineResult result) {
              print("==========onEvalResult================");

              var jsonString = result.text;

              Map<String, dynamic> jsonData = jsonDecode(jsonString.toString());

              int overall = 0;
              int fluency = 0;
              int integrity = 0;
              int accuracy = 0;

              String OverallScore =  ' ';
              String FluencyScore =  ' ';
              String IntegrityScore =  ' ';
              String AccuracyScore =  ' ';

              print('callback coreType: $coreType');

              if("en.word.score" == coreType)
              {
                overall = jsonData['result']['overall'];

                OverallScore =  'Overall Score : ${overall}';

                print('en.word.score Overall: $overall');

                textValue =  'Overall Score : ${overall}' + '\n';

              }
              else if(("en.sent.score" == coreType))
              {
                overall = jsonData['result']['overall'];
                fluency = jsonData['result']['fluency']['overall'];
                integrity = jsonData['result']['integrity'];
                accuracy = jsonData['result']['accuracy'];

                OverallScore =  'Overall Score : ${overall}';
                FluencyScore =  'Fluency Score : ${fluency}';
                IntegrityScore =  'Integrity Score : ${integrity}';
                AccuracyScore =  'Accuracy Score : ${accuracy}';

                textValue =  OverallScore + '\n' + FluencyScore + '\n' + IntegrityScore + '\n' + AccuracyScore;

              }
              else if("en.pred.score" == coreType)
              {
                overall = jsonData['result']['overall'];
                fluency = jsonData['result']['fluency']['overall'];
                integrity = jsonData['result']['integrity'];
                accuracy = jsonData['result']['accuracy'];

                OverallScore =  'Overall Score : ${overall}';
                FluencyScore =  'Fluency Score : ${fluency}';
                IntegrityScore =  'Integrity Score : ${integrity}';
                AccuracyScore =  'Accuracy Score : ${accuracy}';

                textValue =  OverallScore + '\n' + FluencyScore + '\n' + IntegrityScore + '\n' + AccuracyScore;

              }
              else if("en.choc.score" == coreType)
              {
                overall = jsonData['result']['overall'];

                OverallScore =  'Overall Score : ${overall}';

                textValue =  OverallScore + '\n';
              }

              //jsonData['result']['overall'];

              print('textValue: $textValue');

              print('Overall: $overall');


              print("==========before set text value================");


              setState(() {});

              print("==========after set text value=================");

              //plog.log(result.text!);
            },
            onBinaryResult: (result) {
              print("==========onBinaryResult================");

            },
            onError: (result) {
              print("==========onError================");
              print(result.text);

            },
            onVad: (result) {
              print("==========onVad================");
            },
            onSoundIntensity: (result) {
              print("==========onSoundIntensity================");

              textValue =  result.text.toString();

              setState(() {});

            },
            onOther: (result) {
              print("==========onOther================");

            }));

    setState(() {
      Timer.periodic(const Duration(seconds: 5), (timer) {
        timer.cancel();
        //aiengineStop();
      });
    });
  }

  Future<void> _startRecording() async
  {

    if(_engine == null)
      {
        Fluttertoast.showToast(msg: 'The engine has not been initialized yet, please wait and try again!');
        return;
      }
      print("start recording");

      PermissionStatus status = await Permission.microphone.request();
      print("=======start recording=====");
      if (status.isGranted) {
        aiengineStart();
        // 录音权限已授权
        // 进行相关操作
      } else if (status.isDenied) {
        // 录音权限被拒绝
        // 显示提示信息

        print("audio not granted ");
      } else if (status.isPermanentlyDenied) {
        // 录音权限被永久拒绝
        // 可以引导用户进入系统设置页面手动授权
        print("audio not granted forever");

      }

  }

  Future<void> _stopRecording() async
  {
    print("stop recording");
    //await _recorder.stopRecorder();
    aiengineStop();
  }

  Future<void> aiengineStop() async {
    print("======stop 1=====");
    await _engine!.stop();
    print("======stop 2=====");
    setState(() {});
  }

  Future<void> _SetRefText(String coreTypeTemp) async {

    print('Overall: $coreTypeTemp');

    coreType = coreTypeTemp;

    if("en.word.score" == coreType)
    {
      displayText = "apple";
      refText = "apple";
    }
    else if("en.sent.score" == coreType)
    {
      displayText = "Thanks for coming to see me";
      refText = "Thanks for coming to see me";
    }
    else if("en.pred.score" == coreType)
    {
      displayText = "Happiness is about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";
      refText = "Happiness is about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";
    }
    else if("en.choc.score" == coreType)
    {
      displayText = "Tell me an animal that can fly.\n A. Tiger \n B. Panda\n C.Bird \n";

      var lm  = {
        "lm": [
          {"answer": 0,"text": "Tiger"},
          {"answer": 0,"text": "Panda"},
          {"answer": 1,"text": "Bird"},
        ]
      };

      refText = lm;

    }

    print('after set refText');

    setState(() {});

    print("updata UI");
  }

  @override
  Widget build(BuildContext context)
  {
    print('callback coreType: ${widget.coreType}');

    coreType = widget.coreType;

    _SetRefText(coreType);

    return Scaffold(
      appBar: AppBar(
        title: Text(widget.buttonText),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              //"Thanks for coming to see me\n", //显示要朗读的内容
              displayText,
              style: TextStyle(fontSize: 25, height:1),
            ),
            SizedBox(height: 20.0),
            Text(
              textValue, //
              style: TextStyle(fontSize: 25),
            ),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                ElevatedButton (
                  child: Text('Start Recording'),
                  onPressed: _startRecording,
                ),
                SizedBox(width: 16.0),
                ElevatedButton (
                  child: Text('Stop Recording'),
                  onPressed: _stopRecording,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
