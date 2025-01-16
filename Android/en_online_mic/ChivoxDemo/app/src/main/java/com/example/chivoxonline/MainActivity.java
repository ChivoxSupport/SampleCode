package com.example.chivoxonline;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import com.chivox.aiengine.Engine;
import com.chivox.aiengine.GlobalCfg;
import com.chivox.aiengine.RetValue;
import com.chivox.aiengine.SdkInfo;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.concurrent.ExecutorService;

import java.util.concurrent.Executors;

import static android.content.ContentValues.TAG;

public class MainActivity extends AppCompatActivity
{

    private ExecutorService workerThread = Executors.newFixedThreadPool(1);

    private static final int GET_RECODE_AUDIO = 1;

    private MyApplication app;

    private Engine aiengine = null;

    private Button btnWord;
    private Button btnSent;
    private Button btnPred;
    private Button btnChoice;
    private Button btnScne;
    private Button btnPrtl;
    private Button btnASR;

    private Button btnExternalRecorder;
    private static String[] PERMISSION_AUDIO = {
            Manifest.permission.RECORD_AUDIO
    };



    private static void verifyAudioPermissions(Activity activity) {
        int permission = ActivityCompat.checkSelfPermission(activity,
                Manifest.permission.RECORD_AUDIO);
        if (permission != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(activity, PERMISSION_AUDIO,
                    GET_RECODE_AUDIO);
        }
    }

    public void runOnWorkerThread(Runnable runnable) {
        workerThread.execute(runnable);
    }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        btnWord = (Button) this.findViewById(R.id.mBtn_word);
        btnSent = (Button) this.findViewById(R.id.mBtn_sentence);
        btnPred = (Button) this.findViewById(R.id.mBtn_paragraph);
        btnChoice = (Button) this.findViewById(R.id.mBtn_choice);
        btnScne = (Button) this.findViewById(R.id.mBtn_Scne);
        btnPrtl = (Button) this.findViewById(R.id.mBtn_Prtl);
        btnASR = (Button) this.findViewById(R.id.mBtn_ASR);
        btnExternalRecorder = (Button) this.findViewById(R.id.mBtn_ExternalRecorder);

        //Get permission
        verifyAudioPermissions(MainActivity.this);

        bindEvents();

        app = (MyApplication) getApplication();
        aiengine = app.getEngine();

        if(null == aiengine)
        {
            //Initialize the engine instance, This is an asynchronous call
            initEngine();
        }else
        {
            Log.e(TAG, "engine instance already exist!");
        }
    }


    /**
     * Delete aiengine instance
     */
    @Override
    protected void onDestroy()
    {
        aiengine.destroy();
        super.onDestroy();
        System.exit(0);
    }



    protected void onResume()
    {
        Log.e(TAG, "MainActivity resume!");
        super.onResume();
    }


    protected void onRestart()
    {
        Log.e(TAG, "MainActivity restart!");
        super.onRestart();
    }


    private void bindEvents()
    {
        btnWord.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, WordSentPredActivity.class);
                intent.putExtra("coreType","en.word.pron");
                startActivity(intent);
            }
        });

        btnSent.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, WordSentPredActivity.class);
                intent.putExtra("coreType","en.sent.pron");
                startActivity(intent);
            }
        });

        btnPred.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, WordSentPredActivity.class);
                intent.putExtra("coreType","en.pred.score");
                startActivity(intent);
            }
        });


        btnChoice.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, ChoiceActivity.class);
                startActivity(intent);
            }
        });

        btnScne.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, SituationalDialogueActivity.class);
                startActivity(intent);
            }
        });

        btnPrtl.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, DescribeThePictureActivity.class);
                startActivity(intent);
            }
        });

        btnASR.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, AsrActivity.class);
                startActivity(intent);
            }
        });

        btnExternalRecorder.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view) {
                Intent intent;
                intent = new Intent(MainActivity.this, OuterFeedActivity.class);
                startActivity(intent);
            }
        });

    }


    private void initEngine() {
        runOnWorkerThread(new Runnable() {
            public void run() {
                JSONObject cfg = new JSONObject();
                try {
                    //vad source path
                    String vadPath = AIEngineHelper.extractResourceOnce(getApplicationContext(), "vad.0.13.bin", false);
                    Log.d(TAG, "vadPath:" + vadPath);

                    String provisionPath = AIEngineHelper.extractResourceOnce(getApplicationContext(), "aiengine.provision", false);
                    Log.d(TAG, "provisionPath:"+provisionPath);

                    //Local log path
                    String LogPath = AIEngineHelper.getFilesDir(getApplicationContext()).getPath() + "/Log.txt";

                    //Configure the appKey, secretKey, and provision authorized by Chivox
                    cfg.put("appKey", Config.appKey);
                    cfg.put("secretKey", Config.secretKey);
                    cfg.put("provision", provisionPath);

                    //Configure the vad function module, optional
                    {
                        JSONObject vad = new JSONObject();
                        vad.put("enable", 1);
                        vad.put("res", vadPath);
                        vad.put("sampleRate", 16000);
                        vad.put("strip", 0);
                        cfg.put("vad", vad);
                    }
                    { //Local log function module
                        JSONObject prof = new JSONObject();
                        prof.put("enable", 1);
                        prof.put("output", LogPath);    //Local log path
                        cfg.put("prof", prof);
                    }
                    //online log
                    GlobalCfg.setLogUpEnable(false);
                    GlobalCfg.setUserId(Config.userId);

                    //Get sdk version
                    Log.e(TAG, "sdk version:" + SdkInfo.singleton().commonSdkVersion + "+" + SdkInfo.singleton().version);


                    //Use online assessments
                    {
                        JSONObject cloud = new JSONObject();
                        cloud.put("enable", 1);
                        cloud.put("server", "wss://cloud.chivox.com:443"); 
                        cfg.put("cloud", cloud);
                    }
                } catch (JSONException e) {
                    // exception
                    return;
                }


                //Create an engine. This call will not block the UI thread. After the creation is successful, it will be called back through Engine.CreateCallback
                Engine.create(MainActivity.this, cfg, new Engine.CreateCallback() {
                    @Override
                    public void onSuccess(Engine engine)
                    {
                        //Created successfully, save the engine instance for subsequent use
                        aiengine = engine;

                        Log.e(TAG, "create aiengine success" + engine);

                        //Set the engine instance as a global variable, facilitate subsequent reuse
                        app = (MyApplication) getApplication();

                        Log.e(TAG, "before set engine");
                        app.setEngine(aiengine);
                        Log.e(TAG, "after set engine");

                        runOnUiThread(new Runnable() {
                            @Override
                            public void run()
                            {
                                Toast.makeText(MainActivity.this, "create aiengine success", Toast.LENGTH_SHORT).show();
                            }
                        });

                    }

                    @Override
                    public void onFail(RetValue err)
                    {
                        //Creation failed, please check e.errId and e.error to analyze the reason.
                        Log.e("TAG", "create aiengine fail" + err.errId + ", " + err.error);

                        runOnUiThread(new Runnable() {
                            @Override
                            public void run()
                            {
                                Toast.makeText(MainActivity.this, "create aiengine fail", Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });


            }
        });
    }
}