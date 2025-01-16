package com.example.chivoxonline;

import java.util.HashMap;

public class PhoneMap
{
    private HashMap<String, String> markMapUK = new HashMap<String, String>();
    private HashMap<String, String> markMapUS = new HashMap<String, String>();

    public PhoneMap()
    {
        if(null != markMapUK)
        {
            markMapUK.put("ih", "ɪ");
            markMapUK.put("ax", "ə");
            markMapUK.put("oh", "ɒ");
            markMapUK.put("uh", "ʊ");

            markMapUK.put("ah", "ʌ");
            markMapUK.put("eh", "e");
            markMapUK.put("ae", "æ");
            markMapUK.put("iy", "i:");

            markMapUK.put("er", "ɜ:");
            markMapUK.put("ao", "ɔ:");
            markMapUK.put("uw", "u:");
            markMapUK.put("aa", "ɑ:");

            markMapUK.put("ey", "eɪ");
            markMapUK.put("ay", "aɪ");
            markMapUK.put("oy", "ɔɪ");
            markMapUK.put("aw", "aʊ");

            markMapUK.put("ow", "әʊ");
            markMapUK.put("ia", "ɪə");
            markMapUK.put("ea", "ɛә");
            markMapUK.put("ua", "ʊə");

            markMapUK.put("tr", "tr");
            markMapUK.put("dr", "dr");
            markMapUK.put("dz", "dz");
            markMapUK.put("ts", "ts");

            markMapUK.put("p", "p");
            markMapUK.put("k", "k");
            markMapUK.put("m", "m");
            markMapUK.put("s", "s");

            markMapUK.put("f", "f");
            markMapUK.put("sh", "ʃ");
            markMapUK.put("b", "b");
            markMapUK.put("g", "g");

            markMapUK.put("n", "n");
            markMapUK.put("z", "z");
            markMapUK.put("v", "v");
            markMapUK.put("zh", "ʒ");

            markMapUK.put("t", "t");
            markMapUK.put("l", "l");
            markMapUK.put("ng", "ŋ");
            markMapUK.put("th", "θ");

            markMapUK.put("w", "w");
            markMapUK.put("ch", "tʃ");
            markMapUK.put("d", "d");
            markMapUK.put("r", "r");

            markMapUK.put("hh", "h");
            markMapUK.put("dh", "ð");
            markMapUK.put("y", "j");
            markMapUK.put("jh", "dʒ");
        }

        if(null != markMapUS)
        {
            markMapUS.put("ih", "ɪ");
            markMapUS.put("ax", "ə");
            markMapUS.put("oh", "ɔ");
            markMapUS.put("uh", "ʊ");

            markMapUS.put("ah", "ʌ");
            markMapUS.put("eh", "ɛ");
            markMapUS.put("ae", "æ");
            markMapUS.put("iy", "i");

            markMapUS.put("er", "ɜ:");
            markMapUS.put("ao", "ɔ");
            markMapUS.put("uw", "u");
            markMapUS.put("aa", "ɑ");

            markMapUS.put("ey", "e");
            markMapUS.put("ay", "aɪ");
            markMapUS.put("oy", "ɔɪ");
            markMapUS.put("aw", "aʊ");

            markMapUS.put("ow", "oʊ");
            markMapUS.put("ia", "ɪə");
            markMapUS.put("p", "p");
            markMapUS.put("k", "k");

            markMapUS.put("m", "m");
            markMapUS.put("s", "s");
            markMapUS.put("f", "f");
            markMapUS.put("sh", "ʃ");

            markMapUS.put("ts", "ts");
            markMapUS.put("b", "b");
            markMapUS.put("g", "g");
            markMapUS.put("n", "n");

            markMapUS.put("z", "z");
            markMapUS.put("v", "v");
            markMapUS.put("zh", "ʒ");
            markMapUS.put("dz", "dz");

            markMapUS.put("t", "t");
            markMapUS.put("l", "l");
            markMapUS.put("ng", "ŋ");
            markMapUS.put("th", "θ");

            markMapUS.put("w", "w");
            markMapUS.put("ch", "tʃ");
            markMapUS.put("tr", "tr");
            markMapUS.put("d", "d");

            markMapUS.put("r", "r");
            markMapUS.put("hh", "h");
            markMapUS.put("dh", "ð");
            markMapUS.put("y", "j");

            markMapUS.put("jh", "dʒ");
            markMapUS.put("dr", "dr");
        }
    }

    public String getMark(String name, int accent )
    {
        //American pronunciation
        if( 2== accent)
        {
            if(null != markMapUS && null != name)
            {
                if(markMapUS.containsKey(name))
                {
                    return markMapUS.get(name);
                }
            }

        }else {
            if(null != markMapUK && null != name)
            {
                if(markMapUK.containsKey(name))
                {
                    return markMapUK.get(name);
                }
            }

        }
        return null;
    }
}
