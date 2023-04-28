package com.example.chivoxonline;

import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.text.method.ScrollingMovementMethod;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.chivox.aiengine.AudioSrc;
import com.chivox.aiengine.Engine;
import com.chivox.aiengine.EvalResult;
import com.chivox.aiengine.EvalResultListener;
import com.chivox.aiengine.RetValue;
import com.chivox.media.AudioPlayer;
import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

public class DescribeThePictureActivity extends AppCompatActivity
{
    private TextView QuestionView;

    private boolean playing = false;
    private boolean recording = false;

    private Engine aiengine = null;

    private String recFilePath;

    private Context context;

    private int rankPrtl = 4;

    private Boolean isShowAnswer = false;

    private Button recordButton;
    private Button playbackButton;
    private Button referenceAnswerButton;
    private TextView jsonResultTextView;
    private ActionBar actionBar;

    private AudioPlayer player;

    private ExecutorService workerThread = Executors.newFixedThreadPool(1);

    private MyApplication app;

    public void runOnWorkerThread(Runnable runnable)
    {
        workerThread.execute(runnable);
    }

    protected void onCreate(Bundle savedInstanceState)
    {
        // TODO Auto-generated method stub
        super.onCreate(savedInstanceState);
        setContentView(R.layout.speakingwithpicture);

        actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        context = this;

        QuestionView = (TextView) findViewById(R.id.textViewQuestion);
        QuestionView.setText(Config.DisplayTextPrtl);

        recordButton = (Button) findViewById(R.id.buttonRecord);
        playbackButton = (Button) findViewById(R.id.buttonPlay);
        referenceAnswerButton = (Button) findViewById(R.id.buttonReferenceAnswer);

        jsonResultTextView = (TextView) findViewById(R.id.textViewJsonResult);
        jsonResultTextView.setMovementMethod(ScrollingMovementMethod.getInstance());

        player = AudioPlayer.sharedInstance();

        //Get global engine instance
        app = (MyApplication) getApplication();
        aiengine = app.getEngine();

        bindEvents();


    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        switch (item.getItemId()) {
            case android.R.id.home:
                finish();

                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    /**
     * cancel
     */
    @Override
    protected void onDestroy()
    {
        if(recording)
        {
            aiengine.cancel();
        }

        if (playing)
        {
            player.cancel();
        }

        Log.e(TAG, "SpeakingWithPictureActivity destroy");
        super.onDestroy();
    }

    private void bindEvents() {
        recordButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View arg0)
            {
                if ( null == aiengine)
                {
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            Toast.makeText(context, "Engine instance has not been initialized", Toast.LENGTH_SHORT).show();

                        }
                    });
                    return;
                }

                if (recordButton.getText().equals(getText(R.string.record)))
                {
                    recordButton.setText(R.string.stop);
                    jsonResultTextView.setText("");

                    runOnWorkerThread(new Runnable() {
                        public void run() {

                            Log.e(TAG, "click record buttosn");

                            JSONObject param = new JSONObject();
                            try
                            {
                                param.put("coreProvideType", "cloud");
                                param.put("soundIntensityEnable", 1);
                                {//Set vad function parameters, optional
                                    JSONObject vad = new JSONObject();
                                    vad.put("vadEnable", 0);
                                    vad.put("refDuration", 3);
                                    vad.put("speechLowSeek",30);
                                    param.put("vad", vad);
                                }
                                { //Set user ID
                                    JSONObject app = new JSONObject();
                                    app.put("userId",Config.userId);
                                    param.put("app", app);
                                }
                                { //Set audio parameters
                                    JSONObject audio = new JSONObject();
                                    audio.put("audioType", "wav");
                                    audio.put("channel", 1);
                                    audio.put("sampleBytes", 2);
                                    audio.put("sampleRate", 16000);
                                    param.put("audio", audio);
                                }
                                { //set kernel parameters
                                    JSONObject request = new JSONObject();
                                    request.put("coreType", Config.coreTypePrtl);

                                    JSONObject refTextObject = new JSONObject(Config.RefTextPrtl);
                                    request.put("refText", refTextObject);


                                    JSONObject use_inherit_rank = new JSONObject();
                                    use_inherit_rank.put("use_inherit_rank",1);

                                    JSONObject result = new JSONObject();
                                    result.put("details", use_inherit_rank);
                                    request.put("result",result);

                                    request.put("rank",rankPrtl);
                                    request.put("precision",1);
                                    request.put("attachAudioUrl",1);
                                    param.put("request", request);
                                }
                            } catch (JSONException e) {
                                // exception
                                return;
                            }
                            //Configure Recorder
                            AudioSrc.InnerRecorder innerRecorder = new AudioSrc.InnerRecorder();
                            innerRecorder.recordParam.sampleBytes = 2;
                            innerRecorder.recordParam.sampleRate = 16000;

                            File file = new File(AIEngineHelper.getAviFile(context));
                            Log.e(TAG, "file" + file);
                            innerRecorder.recordParam.saveFile = file;     //If you need to save the audio file locally, please set the path
                            innerRecorder.recordParam.duration = 60000;   //Set the recording time(Unit: milliseconds), optional
                            //make a request
                            StringBuilder tokenId = new StringBuilder(); // tokenId - the identification of this assessment task
                            Log.e(TAG, "tokenId" + tokenId);
                            Log.e(TAG, "aiengine" + aiengine);
                            RetValue ret = aiengine.start(context, innerRecorder, tokenId, param, new EvalResultListener()
                            {

                                @Override
                                public void onError(String s, EvalResult evalResult)
                                {
                                    Log.e(TAG, "recordonError" + evalResult.text());
                                    jsonResultTextView.setText(evalResult.text());

                                }

                                @Override
                                public void onEvalResult(String s, final EvalResult evalResult)
                                {
                                    //Assessment result
                                    Log.e(TAG, "recordEvalResult" + evalResult);
                                    Log.e(TAG, "recordEvalResult.recFilePath:" + evalResult.recFilePath());
                                    Log.e(TAG, "recordEvalResult.text:" + evalResult.text());

                                    //Result processing submits to another thread, avoiding blocking or waiting.
                                    runOnWorkerThread(new Runnable() {
                                        public void run() {

                                            recFilePath = evalResult.recFilePath();

                                            String overallScore = null;
                                            String grammarScore = null;
                                            String contentScore = null;
                                            String fluencyScore = null;
                                            String PronunciationScore = null;

                                            StringBuilder recResult = new StringBuilder();

                                            try {
                                                final JSONObject returnObj = new JSONObject(evalResult.text().toString());
                                                final JSONObject resultJSONObject = returnObj.getJSONObject("result");

                                                if (resultJSONObject.has("overall")) {
                                                    overallScore = resultJSONObject.getString("overall");
                                                }

                                                grammarScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("grammar");
                                                contentScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("cnt");
                                                fluencyScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("flu");
                                                PronunciationScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("pron");

                                                recResult.append("Assessment result:");
                                                recResult.append("\nOverall score:" + overallScore + "/" + String.valueOf(rankPrtl));
                                                recResult.append("\nGrammar score:" + grammarScore + "/" + String.valueOf(rankPrtl));
                                                recResult.append("\nContent score:" + contentScore + "/" + String.valueOf(rankPrtl));
                                                recResult.append("\nFluency score:" + fluencyScore + "/" + String.valueOf(rankPrtl));
                                                recResult.append("\nPronunciation score:" + PronunciationScore + "/" + String.valueOf(rankPrtl));

                                            } catch (Exception e) {
                                                e.printStackTrace();
                                            }

                                            //Update main thread UI
                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    jsonResultTextView.setText(recResult.toString());
                                                }
                                            });
                                        }
                                    });
                                }

                                @Override
                                public void onBinResult(String s, final EvalResult evalResult) {
                                    //binary result, reserved for future use
                                    Log.e(TAG, "onBinResult" + evalResult);
                                    Log.e(TAG, "onBinResult.recFilePath:" + evalResult.recFilePath());
                                    Log.e(TAG, "onBinResult.text:" + evalResult.text());
                                    recFilePath = evalResult.recFilePath();
                                    Log.e(TAG, "onBinResult" + evalResult);
                                    //Update main thread UI
                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            jsonResultTextView.setText(evalResult.text());
                                        }
                                    });
                                }

                                @Override
                                public void onVad(String s, final EvalResult evalResult) {
                                    //vad result, returned if vad is enabled
                                    Log.e(TAG, "onVad" + evalResult);
                                    Log.e(TAG, "onVad.recFilePath:" + evalResult.recFilePath());
                                    Log.e(TAG, "onVad.text:" + evalResult.text());
                                    recFilePath = evalResult.recFilePath();
                                    Log.e(TAG, "onVad" + evalResult);

                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            jsonResultTextView.setText(evalResult.text());
                                        }
                                    });

                                    try
                                    {
                                        JSONObject json = new JSONObject(evalResult.text());
                                        if (json.has("vad_status") || json.has("sound_intensity"))
                                        {
                                            /* received vad_status report in json formatting */
                                            int status = json.optInt("vad_status");
                                            final int sound_intensity = json.optInt("sound_intensity");
                                            if (status == 2) {
                                                runOnWorkerThread(new Runnable() {
                                                    public void run() {
                                                        RetValue retstop = aiengine.stop();
                                                        runOnUiThread(new Runnable() {
                                                            public void run() {
                                                                if (recordButton.getText().equals(getText(R.string.stop))) {
                                                                    recordButton.setText(R.string.record);
                                                                }
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        }
                                    } catch (JSONException e) {
                                        /* parse result error */
                                        Log.d(TAG, "pare result error!");
                                    }

                                }

                                @Override
                                public void onSoundIntensity(String s, final EvalResult evalResult) {
                                    //Real-time sound intensity results, the soundIntensity field needs to be set to 1
                                    Log.e(TAG, "onSoundIntensity" + evalResult);
                                    Log.e(TAG, "onSoundIntensity.recFilePath:" + evalResult.recFilePath());
                                    Log.e(TAG, "onSoundIntensity.text:" + evalResult.text());
                                    recFilePath = evalResult.recFilePath();
                                    Log.e(TAG, "onSoundIntensity" + evalResult);
                                    //Update main thread UI
                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            jsonResultTextView.setText(evalResult.text());
                                        }
                                    });

                                }

                                @Override
                                public void onOther(String s, final EvalResult evalResult) {
                                    //reserve
                                    Log.e(TAG, "onOther" + evalResult);
                                    Log.e(TAG, "onOther.recFilePath:" + evalResult.recFilePath());
                                    Log.e(TAG, "onOther.text:" + evalResult.text());
                                    recFilePath = evalResult.recFilePath();
                                    Log.e(TAG, "onOther" + evalResult);

                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            jsonResultTextView.setText(evalResult.text());
                                        }
                                    });
                                }
                            });

                            if (0 != ret.errId)
                            {
                                //Failed to call start interface, please check ret.errId, ret.error to analyze the reason
                                Log.e(TAG, "engine start fail, errId:"+ ret.errId + "errInfo:" + ret.error);
                                return;
                            }
                            else
                            {
                                Log.e(TAG, "start recording");
                                recording = true;
                            }

                        }
                    });
                }
                else
                    {
                    if (recordButton.getText().equals(getText(R.string.stop)))
                    {
                        recordButton.setText(R.string.record);

                        runOnWorkerThread(new Runnable() {
                            public void run() {
                                RetValue ret = aiengine.stop();
                                recording = false;
                                if (0 != ret.errId) {
                                    //Failed to call stop interface, please check ret.errId, ret.error to analyze the reason
                                    Log.i(TAG, "stop_errorid" + ret.errId);
                                    return;
                                }
                            }
                        });
                    }
                }
            }
        });
        playbackButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                runOnWorkerThread(new Runnable() {
                    public void run() {
                        if (!TextUtils.isEmpty(recFilePath)) {
                            File file = new File(recFilePath);
                            if (file.exists())
                            {
                                if (playing)
                                {
                                    playing = false;
                                    player.cancel();
                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            Toast.makeText(context, "Stop play", Toast.LENGTH_SHORT).show();
                                        }
                                    });
                                }
                                else {
                                    runOnUiThread(new Runnable() {
                                        @Override
                                        public void run() {
                                            Toast.makeText(context, "Ready to play", Toast.LENGTH_SHORT).show();//不读句子 播放噪音
                                        }
                                    });
                                    player.play(recFilePath, playerListener);
                                }
                            } else {
                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        Toast.makeText(context, "File does not exist, record again", Toast.LENGTH_SHORT).show();
                                    }
                                });
                            }

                        } else {
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    Toast.makeText(context, "Please re-record", Toast.LENGTH_SHORT).show();

                                }
                            });

                        }

                    }
                });
            }
        });

        referenceAnswerButton.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View view)
            {
                if (!isShowAnswer)
                {
                    isShowAnswer = true;
                    referenceAnswerButton.setText(R.string.txt_hide_answer);
                    jsonResultTextView.setText(Config.DisplayReferenceAnswerPrtl);
                }
                else
                {
                    isShowAnswer = false;
                    referenceAnswerButton.setText(R.string.txt_show_answer);
                    jsonResultTextView.setText("");
                }
            }
        });

    }

    public AudioPlayer.Listener playerListener = new AudioPlayer.Listener() {

        @Override
        public void onStarted(AudioPlayer audioPlayer) {
            playing = true;

        }

        @Override
        public void onStopped(AudioPlayer audioPlayer) {
            playing = false;
        }

        @Override
        public void onError(AudioPlayer audioPlayer, String s) {
            playing = false;

        }
    };

}
