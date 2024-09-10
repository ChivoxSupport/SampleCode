package com.example.chivoxonline;

import android.content.Context;
import android.os.Environment;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.security.MessageDigest;
import java.util.Arrays;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;


public class AIEngineHelper
{

    private static String TAG = "AIEngineHelper";
    private static int BUFFER_SIZE = 4096;

    private static String readFileAsString(File file) throws IOException {
        String line;
        StringBuffer sb = new StringBuffer();
        BufferedReader br = new BufferedReader(new FileReader(file));
        while ((line = br.readLine()) != null) {
            sb.append(line);
        }

        br.close();
        return sb.toString();
    }

    private static void writeFileAsString(File file, String str) throws IOException {
        FileWriter fw = new FileWriter(file);
        fw.write(str);
        fw.close();
    }

    private static void writeFileAsBytes(File file, byte [] bytes) throws IOException {
        FileOutputStream fstream = new FileOutputStream(file);
        BufferedOutputStream stream = new BufferedOutputStream(fstream);
        stream.write(bytes);
        stream.close();
        fstream.close();
    }

    private static void removeDirectory(File directory) {
        if (directory.isDirectory()) {
            File[] files = directory.listFiles();
            for (int i = 0; i < files.length; i++) {
                if (files[i].isDirectory()) {
                    removeDirectory(files[i]);
                }
                files[i].delete();
            }
            directory.delete();
        }
    }

    /**
     * extract resource once, the resource should in zip formatting
     *
     * @param context
     * @param name
     * @return return resource directory contains resources for native aiengine cores on success, otherwise return null
     */
    public static String extractResourceOnce(Context context, String name, boolean unzip) {

        try {
            if (unzip) {
                String pureName = name.replaceAll("\\.[^.]*$", "");

                File filesDir = getFilesDir(context);
                File targetDir = new File(filesDir, pureName);

                String md5sum = md5sum(context.getAssets().open(name));

                File md5sumFile = new File(targetDir, ".md5sum");
                if (targetDir.isDirectory()) {
                    if (md5sumFile.isFile()) {
                        String md5sum2 = readFileAsString(md5sumFile);
                        if (md5sum2.equals(md5sum)) {
                            return targetDir.getAbsolutePath(); /* already extracted */
                        }
                    }

                    removeDirectory(targetDir); /* remove old dirty resource */
                }

                unzip(context.getAssets().open(name), targetDir);
                writeFileAsString(md5sumFile, md5sum);

                return targetDir.getAbsolutePath();
            } else {
                File targetFile = new File(getFilesDir(context), name);
                copyInputStreamToFile(context.getAssets().open(name), targetFile);
                return targetFile.getAbsolutePath();
            }
        } catch (Exception e) {
            Log.e(TAG, "failed to extract resource", e);
        }

        return null;
    }

    public static File getFilesDir(Context context) {

        File targetDir = null;

        if(Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED)) {
            // targetDir = context.getExternalFilesDir(null); // not support android 2.1
            targetDir = new File(Environment.getExternalStorageDirectory(), "Android/data/" + context.getApplicationInfo().packageName + "/files");
            if (!targetDir.exists()) {
                targetDir.mkdirs();
            }
        }

        if (targetDir == null || !targetDir.exists()) {
            targetDir = context.getFilesDir();
        }

        return targetDir;
    }

    private static void unzip(InputStream is, File targetDir)
            throws IOException {

        ZipInputStream zis = new ZipInputStream(new BufferedInputStream(is,
                BUFFER_SIZE));

        ZipEntry ze;
        while ((ze = zis.getNextEntry()) != null) {
            if (ze.isDirectory()) {
                new File(targetDir, ze.getName()).mkdirs();
            } else {

                File file = new File(targetDir, ze.getName());
                File parentdir = file.getParentFile();
                if (parentdir != null && (!parentdir.exists())) {
                    parentdir.mkdirs();
                }

                int pos;
                byte[] buf = new byte[BUFFER_SIZE];
                OutputStream bos = new FileOutputStream(file);
                while ((pos = zis.read(buf, 0, BUFFER_SIZE)) > 0) {
                    bos.write(buf, 0, pos);
                }
                bos.flush();
                bos.close();

                Log.d(TAG, file.getAbsolutePath());
            }
        }

        zis.close();
        is.close();
    }

    private static void copyInputStreamToFile(InputStream is, File file)
            throws Exception {
        int bytes;
        byte[] buf = new byte[BUFFER_SIZE];

        FileOutputStream fos = new FileOutputStream(file);
        while ((bytes = is.read(buf, 0, BUFFER_SIZE)) > 0) {
            fos.write(buf, 0, bytes);
        }

        is.close();
        fos.close();
    };

    private static String sha1(String message) {
        try {
            MessageDigest md = MessageDigest.getInstance("SHA-1");
            md.update(message.getBytes(), 0, message.length());
            return bytes2hex(md.digest());
        } catch (Exception e) {
            /* ignore */
        }
        return null;
    }

    private static String bytes2hex(byte[] bytes) {
        StringBuffer sb = new StringBuffer(bytes.length * 2);
        for (int i = 0; i < bytes.length; i++) {
            int v = bytes[i] & 0xff;
            if (v < 16) {
                sb.append('0');
            }
            sb.append(Integer.toHexString(v));
        }
        return sb.toString();
    }

    private static String md5sum(InputStream is) {
        int bytes;
        byte buf[] = new byte[BUFFER_SIZE];
        try {
            MessageDigest md = MessageDigest.getInstance("MD5");
            while ((bytes = is.read(buf, 0, BUFFER_SIZE)) > 0) {
                md.update(buf, 0, bytes);
            }
            is.close();
            return bytes2hex(md.digest());
        } catch (Exception e) {
            /* ignore */
        }
        return null;
    }

    public static String getProvisionTmpFile(Context context)
    {
        File filesDir = getFilesDir(context);
        String str = filesDir.getAbsolutePath() + "/aiengine.provision.temp";

        Log.d(TAG, "provison tmp file path: " + str);

        return str;
    }

    public static String getAviFile(Context context)
    {
        File filesDir = getFilesDir(context);
        String str = filesDir.getAbsolutePath() + "/test.wav";
        Log.d("Test", "wav file path: " + str);
        return str;
    }

}
