package com.example.chivoxdemo;
import android.app.Application;
import com.chivox.aiengine.Engine;

public class MyApplication extends Application {

    public Engine GlobalAiEngine;

    public void OnCreate()
    {
        GlobalAiEngine = null;
        super.onCreate();
    }

    public Engine getEngine()
    {
        return GlobalAiEngine;
    }

    public void setEngine(Engine aiengine)
    {
        GlobalAiEngine = aiengine;
    }
}
