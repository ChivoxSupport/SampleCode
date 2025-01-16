package com.example.chivoxonline;

import android.content.Context;
import android.content.Intent;
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

import com.chivox.aiengine4.AgnException;
import com.chivox.aiengine4.AudioSource;
import com.chivox.aiengine4.Engine;
import com.chivox.aiengine4.Eval;
import com.chivox.aiengine4.media.AudioPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

public class WordSentPredActivity extends AppCompatActivity
{

    private String recFilePath;

    private boolean playing = false;
    private boolean recording = false;

    private MyApplication app;

    private Engine aiengine = null;

    private Eval RecorderInstance;

    private String coreType;

    private Context context;

    private Button recordButton;
    private Button playbackButton;
    private TextView DisplayTextView;
    private TextView jsonResultTextView;
    private ActionBar actionBar;
    private AudioPlayer player;

    private PhoneMap phonemap;

    private ExecutorService workerThread = Executors.newFixedThreadPool(1);


    public void runOnWorkerThread(Runnable runnable) {
        workerThread.execute(runnable);
    }


    protected void onCreate(Bundle savedInstanceState)
    {
        // TODO Auto-generated method stub
        super.onCreate(savedInstanceState);
        setContentView(R.layout.wordsentpred);

        actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        context = this;

        Intent intent = getIntent();
        coreType = intent.getStringExtra("coreType");

        recordButton = (Button) findViewById(R.id.buttonRecord);
        playbackButton = (Button) findViewById(R.id.buttonPlay);
        jsonResultTextView = (TextView) findViewById(R.id.textViewJsonResult);
        DisplayTextView = (TextView) findViewById(R.id.textViewDisplay);

        player = AudioPlayer.sharedInstance();

        if(coreType.equals("en.word.pron"))
        {
            DisplayTextView.setText(Config.DisplayTextWord);
        }
        else if(coreType.equals("en.sent.pron"))
        {
            DisplayTextView.setText(Config.DisplayTextSent);
        }
        else if(coreType.equals("en.pred.score"))
        {
            DisplayTextView.setText(Config.DisplayTextPred);
        }

        //Get global engine instance
        app = (MyApplication) getApplication();
        aiengine = app.getEngine();

        bindEvents();

        phonemap = new PhoneMap();

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
        Log.e(TAG, "WordSentPredActivity destroy");
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

                    runOnWorkerThread(new Runnable()
                    {
                        public void run() {

                            Log.e(TAG, "click record button");

                            JSONObject param = new JSONObject();
                            try
                            {
                                param.put("coreProvideType", "cloud");
                                param.put("soundIntensityEnable", 0);
                                { //Set vad function parameters, optional
                                    JSONObject vad = new JSONObject();

                                    //Because the paragraph kernel takes a long time to read aloud, there will be pauses in the middle, so vad detection is not enabled.
                                    if(coreType.equals("en.pred.score"))
                                    {
                                        vad.put("vadEnable", 0);
                                    }
                                    else
                                    {
                                        vad.put("vadEnable", 1);
                                    }
                                    vad.put("refDuration", 2);
                                    vad.put("speechLowSeek",20);
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
                                { //set kernel parameters, different kernel has different parameters, for more information, please refer to
                                    //the document https://www.chivox.com/opendoc/#/EnglishDoc/coreEn/
                                    JSONObject request = new JSONObject();

                                    if(coreType.equals("en.word.pron"))
                                    {
                                        request.put("coreType", Config.CoreTypeWord);
                                        request.put("refText", Config.RefTextWord);

                                    }else if(coreType.equals("en.sent.pron"))
                                    {
                                        request.put("coreType", Config.CoreTypeSent);
                                        request.put("refText", Config.RefTextSent);

                                    } else if(coreType.equals("en.pred.score"))
                                    {
                                        request.put("coreType", Config.CoreTypePred);
                                        request.put("refText", Config.RefTextPred);
                                    }

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
                                    //Result processing submits to another thread, avoiding blocking or waiting.
                                    runOnWorkerThread(new Runnable()
                                    {
                                        public void run()
                                        {
                                            String overallScore = null;
                                            String fluencyScore = null;
                                            String integrityScore = null;
                                            String accuracyScore = null;
                                            String standardPhone = null;
                                            String yourPhone = null;

                                            StringBuilder result = new StringBuilder();
                                            StringBuilder StandardPron = new StringBuilder();
                                            StringBuilder yourPron = new StringBuilder();


                                            try
                                            {
                                                final JSONObject returnObj = json;

                                                JSONObject resultJSONObject = null;
                                                String audioTips = null;

                                                if (returnObj.has("result"))
                                                {
                                                    resultJSONObject = returnObj.getJSONObject("result");

                                                    JSONObject infoObj = resultJSONObject.getJSONObject("info");
                                                    if (infoObj.has("tips")) {
                                                        result.append(infoObj.getString("tips"));
                                                        result.append("\n Audio quality is poor or duration is too short, please record again!\n");
                                                    }


                                                    if (resultJSONObject.has("overall")) {
                                                        overallScore = resultJSONObject.getString("overall");
                                                        result.append("Overall score: " + overallScore);
                                                    }

                                                    //Different kernels return different dimensions and need to be processed separately
                                                    if (coreType.equals("en.word.pron"))
                                                    {
                                                        standardPhone = resultJSONObject.getJSONObject("details").getJSONArray("word").getJSONObject(0).getString("lab");

                                                        String accentStr = resultJSONObject.getJSONObject("details").getJSONArray("word").getJSONObject(0).getString("accent");
                                                        int accent = Integer.parseInt(accentStr);

                                                        String labArr[] = standardPhone.split(" ");


                                                        for (int i = 0; i < labArr.length; i++) {
                                                            StandardPron.append(phonemap.getMark(labArr[i],accent));
                                                        }


                                                        yourPhone = resultJSONObject.getJSONObject("details").getJSONArray("word").getJSONObject(0).getString("rec");

                                                        String recArr[] = yourPhone.split(" ");

                                                        for (int i = 0; i < recArr.length; i++) {
                                                            yourPron.append(phonemap.getMark(recArr[i],accent));
                                                        }

                                                        result.append("\nstandard pronuciation: " + StandardPron.toString());
                                                        result.append("\nyour     pronuciation: " + yourPron.toString());

                                                    } else if (coreType.equals("en.sent.pron")) {
                                                        fluencyScore = resultJSONObject.getJSONObject("fluency").getString("overall");
                                                        integrityScore = resultJSONObject.getString("integrity");
                                                        accuracyScore = resultJSONObject.getString("accuracy");

                                                        result.append("\n fluency score: " + fluencyScore);
                                                        result.append("\n integrity score: " + integrityScore);
                                                        result.append("\n accuracy score: " + accuracyScore);
                                                        result.append("\n\n");

                                                        JSONArray jsonArray = resultJSONObject.getJSONArray("details");

                                                        String wordLab;
                                                        StringBuilder SuperfluousReadingWords = new StringBuilder();
                                                        StringBuilder MissingReadingWords = new StringBuilder();
                                                        StringBuilder MisreadWords = new StringBuilder();

                                                        String errorType;
                                                        String wordRec;

                                                        for (int i = 0; i < jsonArray.length(); i++) {
                                                            JSONObject jsonObject = jsonArray.getJSONObject(i);
                                                            wordLab = jsonObject.optString("lab", null);
                                                            wordRec = jsonObject.optString("rec",null);
                                                            errorType = jsonObject.optString("is_err", null);

                                                            if( errorType.equals("1"))
                                                            {
                                                                if(!(wordRec.equals("UNK")))
                                                                {
                                                                    SuperfluousReadingWords.append(wordRec + " ");
                                                                }
                                                            }
                                                            else if(errorType.equals("2"))
                                                            {
                                                                MissingReadingWords.append(wordLab + " ");
                                                            }
                                                            else if(errorType.equals("3"))
                                                            {
                                                                MisreadWords.append(wordLab + " ");
                                                            }
                                                        }

                                                        result.append("\n Superfluous Reading:" + SuperfluousReadingWords.toString());
                                                        result.append("\n Missing Reading:" + MissingReadingWords.toString());
                                                        result.append("\n Misread:" + MisreadWords.toString());


                                                    } else if (coreType.equals("en.pred.score")) {
                                                        fluencyScore = resultJSONObject.getJSONObject("fluency").getString("overall");
                                                        integrityScore = resultJSONObject.getString("integrity");
                                                        accuracyScore = resultJSONObject.getString("accuracy");

                                                        result.append("\n fluency score: " + fluencyScore);
                                                        result.append("\n integrity score: " + integrityScore);
                                                        result.append("\n accuracy score: " + accuracyScore);

                                                    }

                                                } else {
                                                    //Process kernel error
                                                    if (returnObj.has("error")) {
                                                        JSONObject errorObj = returnObj.getJSONObject("error");
                                                        String errorStr = errorObj.getString("id");

                                                        if (errorStr.equals("51000")) {
                                                            result.append("51000 error, request parameters or text format is abnormal.\n");
                                                        }
                                                        result.append("errId:" + errorObj.getString("id"));
                                                        result.append("errInfo:" + errorObj.getString("msg"));
                                                    }

                                                    //Process sdk/server error
                                                    if (returnObj.has("errId")) {
                                                        result.append("errId:" + returnObj.getString("errId"));
                                                        result.append("errInfo:" + returnObj.getString("error"));
                                                    }
                                                }

                                            } catch (Exception e)
                                            {
                                                Log.e(TAG, "onEvalResult , json process error!");
                                                e.printStackTrace();
                                            }

                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    jsonResultTextView.setText(result.toString());
                                                }
                                            });

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
                                        jsonResultTextView.setText(vadResult);
                                    }
                                });


                                if ((vadStatus == 2)&&(recording == true))
                                {

                                    recording = false;

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

                            try {
                                eval.start(param);
                                recording = true;
                                // 一段时间后调用stop
                                //
                            } catch (AgnException e) {
                                eval.cancel();
                            }
                        }
                    });
                } else
                    {
                    if (recordButton.getText().equals(getText(R.string.stop)))
                    {
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
        playbackButton.setOnClickListener(new View.OnClickListener()
        {
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
                                            Toast.makeText(context, "Ready to play", Toast.LENGTH_SHORT).show();
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
            playing = true;
        }

        @Override
        public void onStop(AudioPlayer audioPlayer)
        {
            playing = false;
        }

        @Override
        public void onError(AudioPlayer audioPlayer, String s)
        {
            playing = false;
        }
    };

    private String getDiscription( String errorType )
    {
        String discriontion = null;
        if(errorType.equals("0"))
        {
            discriontion = "corrent";
        }else if(errorType.equals("1"))
        {
            discriontion = "superfluous reading";
        }
        else if(errorType.equals("2"))
        {
            discriontion = "missing reading";
        }
        else if(errorType.equals("3"))
        {
            discriontion = "misread";
        }
        return  discriontion;
    }
}
