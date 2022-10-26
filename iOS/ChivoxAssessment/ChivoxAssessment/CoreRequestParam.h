//
//  CoreRequestParam.h
//  ChivoxDemo
//
//  Copyright © 2022 Chivox. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface CoreRequestParam : NSObject

  //Word kernel
- (NSMutableDictionary *)requestDic_EN_Word_pron;

  //Sentence kernel
- (NSMutableDictionary *)requestDic_EN_Sent_pron;

  //Paragraph kernel
- (NSMutableDictionary *)requestDic_EN_Pred_score;

  //Multiple choice kernel
- (NSMutableDictionary *)requestDic_EN_Choc_score;

  //Situational Dialogue
- (NSMutableDictionary *)requestDic_EN_Scne_exam;

  //Describe a picture
- (NSMutableDictionary *)requestDic_EN_Prtl_exam;

  //ASR
- (NSMutableDictionary *)requestDic_EN_Asr_rec;

@end

NS_ASSUME_NONNULL_END
