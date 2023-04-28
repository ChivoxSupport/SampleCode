package com.example.chivoxonline;

import android.content.Context;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

import com.chivox.aiengine.AudioSrc;
import com.chivox.aiengine.Engine;
import com.chivox.aiengine.EvalResult;
import com.chivox.aiengine.EvalResultListener;
import com.chivox.aiengine.RetValue;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

public class OuterFeedActivity extends AppCompatActivity
{
    private TextView QuestionView;

    private boolean recording = false;

    private Engine aiengine = null;
    private String AudioName = "I-know-the-place-very-well.wav";

    private Context context;

    private Button AssessmentButton;

    private TextView jsonResultTextView;
    private ActionBar actionBar;


    private ExecutorService workerThread = Executors.newFixedThreadPool(1);

    private MyApplication app;

    public void runOnWorkerThread(Runnable runnable) {
        workerThread.execute(runnable);
    }

    protected void onCreate(Bundle savedInstanceState) {
        // TODO Auto-generated method stub
        super.onCreate(savedInstanceState);
        setContentView(R.layout.outerfeed);

        actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        context = this;

        QuestionView = (TextView) findViewById(R.id.textViewQuestion);
        QuestionView.setText(Config.DisplayTextOuterFeed);

        AssessmentButton = (Button) findViewById(R.id.buttonAssessment);
        jsonResultTextView = (TextView) findViewById(R.id.textViewJsonResult);

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
        Log.e(TAG, "AsrActivity destroy");
        super.onDestroy();
    }

    private void bindEvents()
    {
        AssessmentButton.setOnClickListener(new View.OnClickListener() {
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


                //Update main thread UI
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        jsonResultTextView.setText(" ");
                    }
                });


                AssessmentButton.setClickable(false);
                runOnWorkerThread(new Runnable() {
                    public void run()
                    {

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
                                request.put("coreType", Config.CoreTypeSent);
                                request.put("refText", "I know the place well.");

                                request.put("rank",100);
                                request.put("attachAudioUrl",1);
                                param.put("request", request);
                            }
                        } catch (JSONException e) {
                            // exception
                            return;
                        }

                        StringBuilder tokenId = new StringBuilder(); // tokenId - the identification of current assessment task
                        Log.e(TAG, "tokenId" + tokenId);
                        Log.e(TAG, "aiengine" + aiengine);


                        RetValue ret = aiengine.start(context, new AudioSrc.OuterFeed(), tokenId, param, new EvalResultListener()
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
                                        String overallScore = null;
                                        String fluencyScore = null;
                                        String integrityScore = null;
                                        String accuracyScore = null;

                                        StringBuilder ScoreResult = new StringBuilder();

                                        try {
                                            final JSONObject returnObj = new JSONObject(evalResult.text().toString());

                                            JSONObject resultJSONObject = null;
                                            String audioTips = null;

                                            if (returnObj.has("result")) {
                                                ScoreResult.append("Assessment result:\n");

                                                resultJSONObject = returnObj.getJSONObject("result");

                                                overallScore = resultJSONObject.getString("overall");
                                                ScoreResult.append(" Overall score: " + overallScore);

                                                fluencyScore = resultJSONObject.getJSONObject("fluency").getString("overall");
                                                integrityScore = resultJSONObject.getString("integrity");
                                                accuracyScore = resultJSONObject.getString("accuracy");

                                                ScoreResult.append("\n Fluency score: " + fluencyScore);
                                                ScoreResult.append("\n Integrity score: " + integrityScore);
                                                ScoreResult.append("\n Accuracy score: " + accuracyScore);
                                                ScoreResult.append("\n\n");

                                            }

                                        } catch (Exception e) {
                                            e.printStackTrace();
                                        }

                                        //Update main thread UI
                                        runOnUiThread(new Runnable() {
                                            @Override
                                            public void run() {
                                                jsonResultTextView.setText(ScoreResult.toString());
                                                AssessmentButton.setClickable(true);
                                            }
                                        });

                                    }
                                });
                            }

                            @Override
                            public void onBinResult(String s, final EvalResult evalResult)
                            {
                                //binary result, reserved for future use
                                Log.e(TAG, "onBinResult" + evalResult);
                                Log.e(TAG, "onBinResult.recFilePath:" + evalResult.recFilePath());
                                Log.e(TAG, "onBinResult.text:" + evalResult.text());
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
                            public void onVad(String s, final EvalResult evalResult)
                            {
                                //vad result, returned if vad is enabled
                                Log.e(TAG, "onVad" + evalResult);
                                Log.e(TAG, "onVad.recFilePath:" + evalResult.recFilePath());
                                Log.e(TAG, "onVad.text:" + evalResult.text());
                                Log.e(TAG, "onVad" + evalResult);

                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        jsonResultTextView.setText(evalResult.text());
                                    }
                                });

                                try {
                                    JSONObject json = new JSONObject(evalResult.text());
                                    if (json.has("vad_status") || json.has("sound_intensity")) {
                                        /* received vad_status report in json formatting */
                                        int status = json.optInt("vad_status");
                                        final int sound_intensity = json.optInt("sound_intensity");
                                        if (status == 2) {
                                            runOnWorkerThread(new Runnable() {
                                                public void run() {
                                                    RetValue retstop = aiengine.stop();
                                                    runOnUiThread(new Runnable() {
                                                        public void run() {
                                                            if (AssessmentButton.getText().equals(getText(R.string.stop))) {
                                                                AssessmentButton.setText(R.string.record);
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
                            public void onSoundIntensity(String s, final EvalResult evalResult)
                            {
                                //Real-time sound intensity results, the soundIntensity field needs to be set to 1
                                Log.e(TAG, "onSoundIntensity" + evalResult);
                                Log.e(TAG, "onSoundIntensity.recFilePath:" + evalResult.recFilePath());
                                Log.e(TAG, "onSoundIntensity.text:" + evalResult.text());
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
                            public void onOther(String s, final EvalResult evalResult)
                            {
                                //reserve
                                Log.e(TAG, "onOther" + evalResult);
                                Log.e(TAG, "onOther.recFilePath:" + evalResult.recFilePath());
                                Log.e(TAG, "onOther.text:" + evalResult.text());
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

                        //Read the audio file (receive the real-time audio stream of the external recorder), 
						//call the feed interface to send the audio, it may be called many times
                        InputStream fis = null;
                        try {
                            fis = getApplicationContext().getAssets().open(AudioName);
                        } catch (IOException e1) {
                            e1.printStackTrace();
                        }
                        byte[] buf = new byte[1024];
                        int bytes = 0;
                        try {
                            fis.skip(44);
                        } catch (IOException e1) {
                            e1.printStackTrace();
                        }
                        try {
                            while (-1 != (bytes = fis.read(buf, 0, 1024))) {
                                RetValue rett = aiengine.feed(buf, bytes);
                                if (rett.errId != 0)
                                    break;
                            }
                            System.out.println("end read file(feed)");
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                        RetValue ret3 = aiengine.stop();
                        try {
                            fis.close();
                        } catch (IOException e) {
                            e.printStackTrace();
                        }


                    }
                });
            }
        });

    }

}
