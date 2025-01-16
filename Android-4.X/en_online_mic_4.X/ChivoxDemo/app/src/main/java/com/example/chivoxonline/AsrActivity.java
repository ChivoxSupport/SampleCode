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

import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

import com.chivox.aiengine4.AgnException;
import com.chivox.aiengine4.AudioSource;
import com.chivox.aiengine4.Engine;
import com.chivox.aiengine4.Eval;
import com.chivox.aiengine4.media.AudioPlayer;

public class AsrActivity extends AppCompatActivity
{
    private TextView QuestionView;

    private boolean playing = false;
    private boolean recording = false;

    private Engine aiengine = null;

    private Eval RecorderInstance;

    private String recFilePath;

    private Context context;

    private Button recordButton;
    private Button playbackButton;
    private TextView RecResultTextView;
    private TextView ScoreResultTextView;
    private ActionBar actionBar;
    private AudioPlayer player;

    private ExecutorService workerThread = Executors.newFixedThreadPool(1);

    private MyApplication app;

    public void runOnWorkerThread(Runnable runnable) {
        workerThread.execute(runnable);
    }

    protected void onCreate(Bundle savedInstanceState) {
        // TODO Auto-generated method stub
        super.onCreate(savedInstanceState);
        setContentView(R.layout.asr);

        actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        context = this;

        QuestionView = (TextView) findViewById(R.id.textViewQuestion);
        QuestionView.setText(Config.DisplayTextAsr);

        recordButton = (Button) findViewById(R.id.buttonRecord);
        playbackButton = (Button) findViewById(R.id.buttonPlay);
        ScoreResultTextView = (TextView) findViewById(R.id.textViewScoreResult);
        RecResultTextView = (TextView) findViewById(R.id.textViewRecResult);


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
            aiengine.close();
        }
        if (playing)
        {
            player.cancel();
        }

        Log.e(TAG, "AsrActivity destroy");
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
                    RecResultTextView.setText("");
                    ScoreResultTextView.setText("");

                    runOnWorkerThread(new Runnable() {
                        public void run() {

                            Log.e(TAG, "click record buttosn");

                            JSONObject param = new JSONObject();
                            try
                            {
                                param.put("coreProvideType", "cloud");
                                {//Set vad function parameters, optional
                                    JSONObject vad = new JSONObject();



                                    vad.put("vadEnable", 0);
                                    vad.put("refDuration", 3);
                                    vad.put("speechLowSeek",40);
                                    param.put("vad", vad);
                                }
                                { //Set user ID
                                    JSONObject app = new JSONObject();

                                    long timestamp = System.currentTimeMillis();
                                    String sig = Config.appKey+timestamp+Config.secretKey;
                                    sig = MD5.getDigest(sig);


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
                                    request.put("coreType", Config.coreTypeAsr);
                                    request.put("res","en.asr.G4");
                                    request.put("refText", "");


                                    JSONObject ext_cur_wrd = new JSONObject();
                                    ext_cur_wrd.put("ext_cur_wrd",1);

                                    JSONObject result = new JSONObject();
                                    result.put("details", ext_cur_wrd);
                                    request.put("result",result);


                                    request.put("rank",100);
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

                            //Assessment result
                            eval.callback.onEvalResult = (eval_, json) ->
                            {
                                    //Assessment result
                                    //Result processing submits to another thread, avoiding blocking or waiting.
                                    runOnWorkerThread(new Runnable() {
                                        public void run() {

                                            StringBuilder recResult = new StringBuilder();
                                            StringBuilder ScoreResult = new StringBuilder();

                                            try {
                                                JSONObject returnObj = json;

                                                int isEnd = returnObj.optInt("eof", 0);

                                                JSONObject resultJSONObject = returnObj.getJSONObject("result");
                                                JSONArray jsonArray = resultJSONObject.getJSONArray("align");

                                                String word = null;
                                                String punctuation = null;

                                                //Extract the recognized text
                                                recResult.append("Recognition result:\n");
                                                for (int i = 0; i < jsonArray.length(); i++) {
                                                    JSONObject jsonObject = jsonArray.getJSONObject(i);
                                                    word = jsonObject.optString("txt", null);
                                                    punctuation = jsonObject.optString("sep", null);

                                                    recResult.append(word);
                                                    recResult.append(punctuation);
                                                    recResult.append(" ");
                                                }

                                                //Extract score results
                                                if (1 == isEnd) {
                                                    ScoreResult.append("Assessment result:\n");
                                                    if (resultJSONObject.has("rec")) {
                                                        ScoreResult.append("\nOverall score: " + resultJSONObject.getString("overall"));
                                                    }
                                                    if (resultJSONObject.has("fluency")) {
                                                        JSONObject fluencyJSONObject = resultJSONObject.getJSONObject("fluency");
                                                        ScoreResult.append("\nFluency score: " + fluencyJSONObject.getString("overall"));
                                                        ScoreResult.append("\nWPM: " + fluencyJSONObject.getString("speed"));
                                                        ScoreResult.append("\nPause times: " + fluencyJSONObject.getString("pause"));
                                                    }
                                                    if (resultJSONObject.has("pron")) {
                                                        ScoreResult.append("\nPronuciation score: " + resultJSONObject.getString("pron"));
                                                    }
                                                }

                                            } catch (Exception e) {
                                                e.printStackTrace();
                                            }

                                            //Update main thread UI
                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    ScoreResultTextView.setText(ScoreResult.toString());
                                                    RecResultTextView.setText(recResult.toString());
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
                                        RecResultTextView.setText(soundIntensityResult);
                                    }
                                });
                            };

                            eval.callback.onVadStatus = (eval_, vadStatus) ->
                            {
                                Log.e(TAG, "onVadStatus: " + vadStatus);

                                String vadResult = "vad Status:" + String.valueOf(vadStatus);

                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        RecResultTextView.setText(vadResult);
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
                            public void run() {
                                try {
                                    RecorderInstance.stop();
                                } catch (AgnException e)
                                {
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
                            if (file.exists()) {
                                AudioPlayer player = AudioPlayer.sharedInstance();
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

    }

    public AudioPlayer.Listener playerListener = new AudioPlayer.Listener()
    {
        @Override
        public void onStart(AudioPlayer audioPlayer)
        {
            Log.e(TAG, "player onStart");
        }

        @Override
        public void onStop(AudioPlayer audioPlayer)
        {
            Log.e(TAG, "player onStop");
        }

        @Override
        public void onError(AudioPlayer audioPlayer, String s)
        {
            playing = false;
        }
    };

}
