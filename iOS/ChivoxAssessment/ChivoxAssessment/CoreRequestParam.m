//
//  CoreRequestParam.m
//  ChivoxDemo
//
//  Copyright © 2022 Chivox. All rights reserved.
//

#import "CoreRequestParam.h"

@implementation CoreRequestParam

   //Word
- (NSMutableDictionary *)requestDic_EN_Word_pron{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.word.score" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:100] forKey:@"rank"];
    NSString *refText = @"apple";
    [requestDic setValue:refText forKey:@"refText"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];

    //accent 1: American 2: British; Default value is 0
    //[requestDic setValue:[NSNumber numberWithInt:1] forKey:@"accent"];
    
    //Customize pronunciation
    //NSArray *wordlist = @[@{@"word":@"past",@"pron":@[@[@"p",@"aa",@"s",@"t"]]}];
    //[requestDic setValue:wordlist forKey:@"wordlist"];

    return requestDic;
}

  //Sentence
- (NSMutableDictionary *)requestDic_EN_Sent_pron{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.sent.score" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:100] forKey:@"rank"];
    NSString *refText = @"Thank you for coming to see me.";

    [requestDic setValue:refText forKey:@"refText"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
    
    NSMutableDictionary *resultDic =[[NSMutableDictionary alloc] init];
    NSMutableDictionary *detailDic =[[NSMutableDictionary alloc] init];

    [detailDic setValue:[NSNumber numberWithInt:1] forKey:@"phone"];//评测结果中返回音素维度
    
    [resultDic setValue:detailDic forKey:@"details"];
    [requestDic setValue:resultDic forKey:@"result"];
    return requestDic;
}

  //Paragraph
- (NSMutableDictionary *)requestDic_EN_Pred_score{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.pred.score" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:100] forKey:@"rank"];
    NSString *refText = @"Happiness is not about being immortal nor having food or rights in one's hand. It’s about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";
    [requestDic setValue:refText forKey:@"refText"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];

    NSMutableDictionary *resultDic =[[NSMutableDictionary alloc] init];
    NSMutableDictionary *detailDic =[[NSMutableDictionary alloc] init];
    [detailDic setValue:[NSNumber numberWithInt:1] forKey:@"word"];//显示单词原格式
    [resultDic setValue:detailDic forKey:@"details"];
    [requestDic setValue:resultDic forKey:@"result"];
    return requestDic;
}


   //Multiple choice
- (NSMutableDictionary *)requestDic_EN_Choc_score
{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.choc.score" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:100] forKey:@"rank"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"precision"];
    NSMutableDictionary *refTextDic =[[NSMutableDictionary alloc] init];

    NSMutableDictionary *textDic1 = [[NSMutableDictionary alloc] init];
    [textDic1 setValue:@"Tiger" forKey:@"text"];
    [textDic1 setValue:[NSNumber numberWithInt:0] forKey:@"answer"];
    
    NSMutableDictionary *textDic2 = [[NSMutableDictionary alloc] init];
    [textDic2 setValue:@"Panda" forKey:@"text"];
    [textDic2 setValue:[NSNumber numberWithInt:0] forKey:@"answer"];
    
    NSMutableDictionary *textDic3 = [[NSMutableDictionary alloc] init];
    [textDic3 setValue:@"Bird" forKey:@"text"];
    [textDic3 setValue:[NSNumber numberWithInt:1] forKey:@"answer"];
    
  
    NSMutableArray *lmArray = [NSMutableArray arrayWithObjects:textDic1,textDic2,textDic3,nil];
    [refTextDic setValue:lmArray forKey:@"lm"];
    [requestDic setValue:refTextDic forKey:@"refText"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
    return requestDic;
    
}

   //Situational Dialogue
- (NSMutableDictionary *)requestDic_EN_Scne_exam
{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.scne.exam" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:4] forKey:@"rank"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"precision"];
    NSMutableDictionary *refTextDic =[[NSMutableDictionary alloc] init];

    NSMutableDictionary *textDic1 = [[NSMutableDictionary alloc] init];
    [textDic1 setValue:@"Because he can go to the beach" forKey:@"text"];
    
    NSMutableDictionary *textDic2 = [[NSMutableDictionary alloc] init];
    [textDic2 setValue:@"Because the boy can go to the beach" forKey:@"text"];
    
    NSMutableDictionary *textDic3 = [[NSMutableDictionary alloc] init];
    [textDic3 setValue:@"The boy love summer because he can go to the beach" forKey:@"text"];
    
    NSMutableDictionary *textDic4 = [[NSMutableDictionary alloc] init];
    [textDic4 setValue:@"He can go to the beach" forKey:@"text"];
  
    NSMutableArray *lmArray = [NSMutableArray arrayWithObjects:textDic1,textDic2,textDic3,textDic4,nil];
    [refTextDic setValue:lmArray forKey:@"lm"];
    [requestDic setValue:refTextDic forKey:@"refText"];
    
    NSMutableDictionary *resultDic =[[NSMutableDictionary alloc] init];
    NSMutableDictionary *detailDic =[[NSMutableDictionary alloc] init];
    [detailDic setValue:[NSNumber numberWithInt:1] forKey:@"use_inherit_rank"];
    [resultDic setValue:detailDic forKey:@"details"];
    [requestDic setValue:resultDic forKey:@"result"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
    
    return requestDic;

}

   //Discribe a picture
- (NSMutableDictionary *)requestDic_EN_Prtl_exam
{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.prtl.exam" forKey:@"coreType"];
    [requestDic setValue:[NSNumber numberWithInt:4] forKey:@"rank"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"precision"];
    NSMutableDictionary *refTextDic =[[NSMutableDictionary alloc] init];

    NSMutableDictionary *textDic1 = [[NSMutableDictionary alloc] init];
    [textDic1 setValue:@"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. There is a monkey over there and it is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals." forKey:@"text"];
    
    NSMutableDictionary *textDic2 = [[NSMutableDictionary alloc] init];
    [textDic2 setValue:@"Look, what are these animals doing now in the zoo? There are four elephants in the zoo and they are drinking water. Two cute pandas are sleeping on the tree. A monkey is eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals." forKey:@"text"];
    
    NSMutableDictionary *textDic3 = [[NSMutableDictionary alloc] init];
    [textDic3 setValue:@"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two fat pandas are on the tree and they are sleeping. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running! I like these animals." forKey:@"text"];
    
  
    NSMutableArray *lmArray = [NSMutableArray arrayWithObjects:textDic1,textDic2,textDic3,nil];
    [refTextDic setValue:lmArray forKey:@"lm"];
    [requestDic setValue:refTextDic forKey:@"refText"];
    
    NSMutableDictionary *resultDic =[[NSMutableDictionary alloc] init];
    NSMutableDictionary *detailDic =[[NSMutableDictionary alloc] init];
    [detailDic setValue:[NSNumber numberWithInt:1] forKey:@"use_inherit_rank"];
    [resultDic setValue:detailDic forKey:@"details"];
    [requestDic setValue:resultDic forKey:@"result"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
    
    return requestDic;

}

   //ASR
- (NSMutableDictionary *)requestDic_EN_Asr_rec
{
    NSMutableDictionary *requestDic = [[NSMutableDictionary alloc] init];
    [requestDic setValue:@"en.sent.rec" forKey:@"coreType"];
    [requestDic setValue:@"en.asr.G4" forKey:@"res"];
    [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
    return requestDic;
}


@end
