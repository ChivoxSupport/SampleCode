package com.example.chivoxonline;


import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.chivox.aiengine4.AgnException;
import com.chivox.aiengine4.AudioSource;
import com.chivox.aiengine4.Engine;
import com.chivox.aiengine4.Eval;
import com.chivox.aiengine4.media.AudioPlayer;

import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

public class SituationalDialogueActivity extends AppCompatActivity
{
    private TextView QuestionView;

    private boolean playing = false;

    private Engine aiengine = null;

    private Eval RecorderInstance;

    private boolean recording = false;

    private String recFilePath;

    private int rankScne = 4;

    private Boolean isShowAnswer = false;

    private Context context;

    private Button recordButton;
    private Button playbackButton;
    private Button referenceAnswerButton;
    private ActionBar actionBar;

    private TextView jsonResultTextView;

    private ExecutorService workerThread = Executors.newFixedThreadPool(1);

    private MyApplication app;

    private AudioPlayer player;

    public void runOnWorkerThread(Runnable runnable)
    {
        workerThread.execute(runnable);
    }

    protected void onCreate(Bundle savedInstanceState)
    {
        // TODO Auto-generated method stub
        super.onCreate(savedInstanceState);
        setContentView(R.layout.situationaldialogue);

        actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        context = this;

        QuestionView = (TextView) findViewById(R.id.textViewQuestion);
        QuestionView.setText(Config.DisplayTextScne);

        recordButton = (Button) findViewById(R.id.buttonRecord);
        playbackButton = (Button) findViewById(R.id.buttonPlay);
        referenceAnswerButton = (Button) findViewById(R.id.buttonReferenceAnswer);

        jsonResultTextView = (TextView) findViewById(R.id.textViewJsonResult);

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
            RecorderInstance.cancel();
        }

        if (playing)
        {
            player.cancel();
        }
        Log.e(TAG, "SituationalDialogueActivity destroy");
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
                                    vad.put("speechLowSeek",20);
                                    param.put("vad", vad);
                                }
                                {
                                    //Set user ID and signature information
                                    long timestamp = System.currentTimeMillis();
                                    String sig = Config.appKey+timestamp+Config.secretKey;
                                    sig = MD5.getDigest(sig);

                                    JSONObject app = new JSONObject();

                                    app.put("applicationId", Config.appKey);
                                    app.put("sig", sig);
                                    app.put("alg", Config.alg);
                                    app.put("timestamp", String.valueOf(timestamp));
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
                                    request.put("coreType", Config.coreTypeScne);

                                    JSONObject refTextObject = new JSONObject(Config.RefTextScne);
                                    request.put("refText", refTextObject);


                                    JSONObject use_inherit_rank = new JSONObject();
                                    use_inherit_rank.put("use_inherit_rank",1);

                                    JSONObject result = new JSONObject();
                                    result.put("details", use_inherit_rank);
                                    request.put("result",result);

                                    request.put("rank",rankScne);
                                    request.put("precision",1);
                                    request.put("attachAudioUrl",1);
                                    param.put("request", request);
                                }
                            } catch (JSONException e) {
                                // exception
                                return;
                            }

                            File file = new File(AIEngineHelper.getAviFile(context));
                            Log.e(TAG, "file path11: " + file);

                            //Configure Recorder
                            Eval eval = new Eval.Builder(aiengine)
                                    .setAudioSource(AudioSource.InnerRecorder)
                                    .setRecordDuration(25000)  //Set recording duration (Unit: milliseconds)
                                    .setRecordSave(file)
                                    .build();

                            RecorderInstance = eval;

                            eval.callback.onRecorderStart = (eval_) -> {
                                Log.e(TAG, "start recording");
                            };
                            eval.callback.onRecorderData = (eval_,data) -> {
                                Log.e(TAG, "audio data: "+ data );
                            };
                            eval.callback.onRecorderStop = (eval_, saveFile, duration)-> {
                                if(null != saveFile)
                                {
                                    Log.e(TAG, "Save audio successfully! path: "+ saveFile);
                                    recFilePath = saveFile.toString();
                                }
                            };
                            eval.callback.onRecorderError = (eval_, info) -> {
                                Log.e(TAG, "recorder error: " +info);
                            };

                            eval.callback.onError = (eval_, json) ->
                            {
                                Log.e(TAG, "onError: "+json);
                            };


                            eval.callback.onEvalResult = (eval_, json) ->
                            {
                                    //Assessment result

                                    //Result processing submits to another thread, avoiding blocking or waiting.
                                    runOnWorkerThread(new Runnable() {
                                        public void run() {

                                            String overallScore = null;
                                            String grammarScore = null;
                                            String contentScore = null;
                                            String fluencyScore = null;
                                            String PronunciationScore = null;

                                            StringBuilder result = new StringBuilder();

                                            try {
                                                final JSONObject returnObj = json;
                                                final JSONObject resultJSONObject = returnObj.getJSONObject("result");

                                                if (resultJSONObject.has("overall")) {
                                                    overallScore = resultJSONObject.getString("overall");
                                                }

                                                grammarScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("grammar");
                                                contentScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("cnt");
                                                fluencyScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("flu");
                                                PronunciationScore = resultJSONObject.getJSONObject("details").getJSONObject("multi_dim").getString("pron");

                                                result.append("Assessment result:");
                                                result.append("\nOverall score:" + overallScore + "/" + String.valueOf(rankScne));
                                                result.append("\nGrammar score:" + grammarScore + "/" + String.valueOf(rankScne));
                                                result.append("\nContent score:" + contentScore + "/" + String.valueOf(rankScne));
                                                result.append("\nFluency score:" + fluencyScore + "/" + String.valueOf(rankScne));
                                                result.append("\nPronunciation score:" + PronunciationScore + "/" + String.valueOf(rankScne));

                                            } catch (Exception e) {
                                                e.printStackTrace();
                                            }

                                            //Update main thread UI
                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    jsonResultTextView.setText(result.toString());
                                                }
                                            });

                                        }
                                    });
                                };

                            eval.callback.onSoundIntensity = (eval_, soundIntensity) -> {
                                Log.e(TAG, "Sound Intensity: " + soundIntensity);

                                String soundIntensityResult = "onSoundIntensity:" + String.valueOf(soundIntensity);

                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        jsonResultTextView.setText(soundIntensityResult);
                                    }
                                });
                            };

                            eval.callback.onVadStatus = (eval_, vadStatus) ->
                            {
                                Log.e(TAG, "onVadStatus: " + vadStatus);

                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        jsonResultTextView.setText(vadStatus);
                                    }
                                });

                                if (vadStatus == 2) {
                                    runOnWorkerThread(new Runnable() {
                                        public void run() {
                                            try {
                                                RecorderInstance.stop();
                                            } catch (AgnException e) {
                                                e.printStackTrace();
                                            }
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
                            };

                            try {
                                eval.start(param);
                                // 一段时间后调用stop
                                //
                            } catch (AgnException e) {
                                eval.cancel();
                            }

                        }
                    });
                } else {
                    if (recordButton.getText().equals(getText(R.string.stop))) {
                        recordButton.setText(R.string.record);
                        runOnWorkerThread(new Runnable() {
                            public void run()
                            {
                                try {
                                    RecorderInstance.stop();
                                } catch (AgnException e) {
                                    e.printStackTrace();
                                }
                                recording = false;
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

        referenceAnswerButton.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if (!isShowAnswer)
                {
                    isShowAnswer = true;
                    referenceAnswerButton.setText(R.string.txt_hide_answer);
                    jsonResultTextView.setText(Config.DisplayReferenceAnswerScne);
                } else {
                    isShowAnswer = false;
                    referenceAnswerButton.setText(R.string.txt_show_answer);
                    jsonResultTextView.setText("");
                }
            }
        });

    }


    public AudioPlayer.Listener playerListener = new AudioPlayer.Listener() {

        @Override
        public void onStart(AudioPlayer audioPlayer) {
            playing = true;
        }

        @Override
        public void onStop(AudioPlayer audioPlayer) {
            playing = false;
        }

        @Override
        public void onError(AudioPlayer audioPlayer, String s) {
            playing = false;
        }
    };

}
