//
//  ChivoxAIGlobalCfg.h
//  CAIEngine
//
//  Created by sq-ios92 on 2020/12/3.
//  Copyright © 2020年 chivox. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ChivoxAIGlobalCfg : NSObject

/// 用于LogUp模块的userId
+ (NSString *)getUserId;
+ (void)setUserId:(NSString *)userId;

/// LogUp模块是否开启
+ (BOOL)isLogUpEnable;
+ (void)setLogUpEnable:(BOOL)enable;

/// 是否使用通用SDK中的在线评测
+ (BOOL)useCommonSDKCloudEval;
+ (void)setUseCommonSDKCloudEval:(BOOL)enable;

@end
