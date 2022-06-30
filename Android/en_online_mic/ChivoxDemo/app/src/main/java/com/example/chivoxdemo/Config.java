package com.example.chivoxdemo;

public class Config {


    //Chivox authorized account
    public static String appKey = "Authorized appKey";
    public static String secretKey = "Authorized secretKey";
    public static String provision = "Authorized provision code";

    //User Id. It is recommended to set it to easily distinguish the assessment data of different users.
    public static String userId="ChivoxDemo520";

    //Different kernel types and corresponding text

    //Word
    public static  String CoreTypeWord="en.word.pron";
    public static  String DisplayTextWord = "apple";
    public static  String RefTextWord="apple";

    //Sentence
    public static  String CoreTypeSent="en.sent.pron";
    public static  String DisplayTextSent = "Thank you for coming to see me.";
    public static  String RefTextSent="Thank you for coming to see me.";

    //Paragraph
    public static  String CoreTypePred ="en.pred.score";
    public static  String DisplayTextPred = "Happiness is not about being immortal nor having food or rights in one's hand. It’s about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";
    public static  String RefTextPred="Happiness is not about being immortal nor having food or rights in one's hand. It’s about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";

    //Multiple choice
    public static  String coreTypeChoice = "en.choc.score";
    public static  String DisplayTextChoice = "Tell me an animal that can fly.\n A. Tiger \n B. Panda\n C.Bird \n";
    public static  String RefTextChoice = "{\"lm\": [{\"answer\": 0,\"text\": \"Tiger\"}, {\"answer\": 0,\"text\": \"Panda\"},{\"answer\": 1,\"text\": \"Bird\"}]}";

    //Situational Dialogue
    public static  String coreTypeScne = "en.scne.exam";
    public static  String DisplayTextScne = "Please answer the question based on the text below.\n\n Boy: I love summer because I can go to the beakch, Which seasons is your favourite?\n Girl: Spinrg, It's warm but it's not too hot. There are beautiful trees and flowers everywhere. \n\n Question: \nWhy does the boy like summer?\n Your answer: ____\n";
    public static  String RefTextScne = "{\"lm\": [{\"text\": \"Because he can go to the beach\"}, {\"text\": \"Because the boy can go to the beach\"},{\"text\": \"The boy love summer because he can go to the beach\"},{\"text\": \"He can go to the beach\"}]}";
    public static  String DisplayReferenceAnswerScne = " Answer1:\n Because he can go to the beach. \n\n Answer2:\n Because the boy can go to the beach. \n\n Answer3:\n The boy love summer because he can go to the beach \n\n Answer4:\n He can go to the beach\n";

    //Speaking with picture
    public static  String coreTypePrtl = "en.prtl.exam";
    public static  String DisplayTextPrtl = "Please describe the picture in English in 60 seconds according to the picture below. Your description can start like this:\n Look, what are these animals doing now in the zoo? ...\n";
    public static  String RefTextPrtl = "{\"lm\": [{\"text\": \"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. AThere is a monkey over there and it is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.\"}, {\"text\": \"Look, what are these animals doing now in the zoo? There are four elephants in the zoo and they are drinking water. Two cute pandas are sleeping on the tree. A monkey is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.\"},{\"text\": \"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two fat pandas are on the tree and they are sleeping. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.\"}]}";
    public static  String DisplayReferenceAnswerPrtl = "Answer1:\n Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. AThere is a monkey over there and it is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.\n\n Answer2:\n Look, what are these animals doing now in the zoo? There are four elephants in the zoo and they are drinking water. Two cute pandas are sleeping on the tree. A monkey is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.\n\n Answer3:\n Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two fat pandas are on the tree and they are sleeping. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals.";

    //Asr
    public static  String coreTypeAsr = "en.asr.rec";
    public static  String DisplayTextAsr = "Please speak English after clicking Record.";

}
