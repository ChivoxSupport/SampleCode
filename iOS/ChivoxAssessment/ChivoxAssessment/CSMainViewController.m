//
//  CSMainViewController.m
//  ChivoxNewDemo
//
//

#import "CSMainViewController.h"
#import "CAIEngine.h"
#import "CoreRequestParam.h"

#import <AVFoundation/AVFoundation.h>

@interface CSMainViewController ()<AVAudioPlayerDelegate>

@end

@implementation CSMainViewController

@synthesize refTextView;
@synthesize resultTextView;
@synthesize recordButton;
@synthesize playbackButton;


- (void)viewDidLoad
{
    [super viewDidLoad];
    NSLog(@"%@",self.engineC);
    
    //Text for display
    if ([self.coretype isEqual: @"en.word.pron"])
    {
        self.refTextView.text = @"apple";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.sent.pron"])
    {
        self.refTextView.text = @"Thank you for coming to see me.";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.pred.score"])
    {
        self.refTextView.text = @"Happiness is not about being immortal nor having food or rights in one's hand. It’s about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.choc.score"])
    {
        self.refTextView.text = @"Tell me an animal that can fly.\n A. Tiger \n B. Panda\n C.Bird \n";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.scne.exam"])
    {
        self.refTextView.text = @"Please answer the question based on the text below.\n\n Boy: I love summer because I can go to the beakch, Which seasons is your favourite?\n Girl: Spinrg, It's warm but it's not too hot. There are beautiful trees and flowers everywhere. \n\n Question: \nWhy does the boy like summer?\n Your answer: ____\n";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.prtl.exam"])
    {
        self.refTextView.text = @"Please describe the picture in English in 60 seconds according to the picture below. Your description can start like this:\n Look, what are these animals doing now in the zoo? ...\n";
        self.refTextView.editable=NO;
        [self.prtlImage setImage:[UIImage imageNamed:@"as.jpg"]];
    }
    else if ([self.coretype isEqual: @"en.asr.rec"])
    {
        self.refTextView.text = @"Please speak English after clicking Record.";
        self.refTextView.editable=NO;
    }
    else
    {
        self.refTextView.text = @"unsupport";
        self.refTextView.editable=NO;
        NSLog(@"暂未开放功能");
    }
    resultTextView.text = @"";
}


- (void) showResult: (NSString *) result
{
    NSLog(@"Result:%s\n",[result UTF8String]);
    //resultTextView.text = result;
    resultTextView.editable=NO;
    NSError *err;
    NSData *jsonData = [result dataUsingEncoding:NSUTF8StringEncoding];
    NSMutableDictionary *resultDic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
    
    NSString *resultForShow = @"";
    
    if(err)
    {
        NSLog(@"json解析失败：%@",err);
    }
    
    NSString *overall = @"";
    NSString *overallStr = @"";
    
    if ([self.coretype isEqual: @"en.word.pron"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        overall = [resultdic2 objectForKey:@"overall"];
        overallStr = [NSString stringWithFormat:@"%@",overall];
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@", str,overallStr];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.sent.pron"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *overall = [resultdic2 objectForKey:@"overall"];
        NSString *overallStr = [NSString stringWithFormat:@"%@",overall];
        
        NSMutableDictionary * resultdic3 = [resultdic2 objectForKey:@"fluency"];
        NSString *fluency = [resultdic3 objectForKey:@"overall"];
        NSString *fluencyStr = [NSString stringWithFormat:@"%@",fluency];
        
        NSString *integrity = [resultdic2 objectForKey:@"integrity"];
        NSString *integrityStr = [NSString stringWithFormat:@"%@",integrity];
        
        NSString *accuracy = [resultdic2 objectForKey:@"accuracy"];
        NSString *accuracyStr = [NSString stringWithFormat:@"%@",accuracy];

        NSLog(@"#%@#%@#%@#%@",overallStr,fluencyStr,integrityStr,accuracyStr);
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@", str,overallStr,@"\nfluency score: ",fluency,@"\nintegrity score: ",integrityStr,@"\naccuracy score: ",accuracyStr];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.pred.score"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *overall = [resultdic2 objectForKey:@"overall"];
        NSString *overallStr = [NSString stringWithFormat:@"%@",overall];
        
        NSMutableDictionary * resultdic3 = [resultdic2 objectForKey:@"fluency"];
        NSString *fluency = [resultdic3 objectForKey:@"overall"];
        NSString *fluencyStr = [NSString stringWithFormat:@"%@",fluency];
        
        NSString *integrity = [resultdic2 objectForKey:@"integrity"];
        NSString *integrityStr = [NSString stringWithFormat:@"%@",integrity];
        
        NSString *accuracy = [resultdic2 objectForKey:@"accuracy"];
        NSString *accuracyStr = [NSString stringWithFormat:@"%@",accuracy];

        NSLog(@"#%@#%@#%@#%@",overallStr,fluencyStr,integrityStr,accuracyStr);
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@", str,overallStr,@"\nfluency score: ",fluency,@"\nintegrity score: ",integrityStr,@"\naccuracy score: ",accuracyStr];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.choc.score"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        overall = [resultdic2 objectForKey:@"overall"];
        overallStr = [NSString stringWithFormat:@"%@",overall];
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@", str,overallStr];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.scne.exam"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *Overall = [resultdic2 objectForKey:@"overall"];
        NSString *OverallStr = [NSString stringWithFormat:@"%@",Overall];
        
        NSMutableDictionary * details = [resultdic2 objectForKey:@"details"];
        
        NSMutableDictionary * multi_dim = [details objectForKey:@"multi_dim"];
        
        NSString *Grammar = [multi_dim objectForKey:@"grammar"];
        NSString *GrammarStr = [NSString stringWithFormat:@"%@",Grammar];
        
        NSString *Content = [multi_dim objectForKey:@"cnt"];
        NSString *ContentStr = [NSString stringWithFormat:@"%@",Content];
        
        NSString *Pronunciation = [multi_dim objectForKey:@"pron"];
        NSString *PronunciationStr = [NSString stringWithFormat:@"%@",Pronunciation];
        
        NSString *Fluency = [multi_dim objectForKey:@"flu"];
        NSString *FluencyStr = [NSString stringWithFormat:@"%@",Fluency];
        

        NSLog(@"#%@#%@#%@#%@#%@",OverallStr,GrammarStr,ContentStr,PronunciationStr,FluencyStr);
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@%@%@%@%@%@%@%@", str,OverallStr,@"/4",@"\nGrammar score: ",GrammarStr,@"/4", @"\nContent score: ",ContentStr,@"/4", @"\nFluency score: ",FluencyStr,@"/4", @"\nPronunciation score: ",PronunciationStr,@"/4"];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.prtl.exam"])
    {
        
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *Overall = [resultdic2 objectForKey:@"overall"];
        NSString *OverallStr = [NSString stringWithFormat:@"%@",Overall];
        
        NSMutableDictionary * details = [resultdic2 objectForKey:@"details"];
        
        NSMutableDictionary * multi_dim = [details objectForKey:@"multi_dim"];
        
        NSString *Grammar = [multi_dim objectForKey:@"grammar"];
        NSString *GrammarStr = [NSString stringWithFormat:@"%@",Grammar];
        
        NSString *Content = [multi_dim objectForKey:@"cnt"];
        NSString *ContentStr = [NSString stringWithFormat:@"%@",Content];
        
        NSString *Pronunciation = [multi_dim objectForKey:@"pron"];
        NSString *PronunciationStr = [NSString stringWithFormat:@"%@",Pronunciation];
        
        NSString *Fluency = [multi_dim objectForKey:@"flu"];
        NSString *FluencyStr = [NSString stringWithFormat:@"%@",Fluency];
        

        NSLog(@"#%@#%@#%@#%@#%@",OverallStr,GrammarStr,ContentStr,PronunciationStr,FluencyStr);
        
        NSString *str =@"Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@%@%@%@%@%@%@%@", str,OverallStr,@"/4",@"\nGrammar score: ",GrammarStr,@"/4", @"\nContent score: ",ContentStr,@"/4", @"\nFluency score: ",FluencyStr,@"/4", @"\nPronunciation score: ",PronunciationStr,@"/4"];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.asr.rec"])
    {
       
    }
    else {
        NSLog(@"unsupport");
    }
    
    resultTextView.text = resultForShow;
    
}

- (IBAction)recordButtonPressed
{
    
    //cs
    const char * button_title = [[recordButton titleForState:UIControlStateNormal] UTF8String];
    NSLog(@"%s",button_title);
    if(strcmp(button_title, "record") == 0) {
        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord error:nil];
//        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker | AVAudioSessionCategoryOptionInterruptSpokenAudioAndMixWithOthers | AVAudioSessionCategoryOptionAllowBluetooth error:nil];
        [[AVAudioSession sharedInstance] setActive:YES error:nil];
        ChivoxAIEvalResultListener *handler = [[ChivoxAIEvalResultListener alloc] init];//创建监听对象
        handler.onEvalResult = ^(NSString * _Nonnull eval, ChivoxAIEvalResult *
        _Nonnull result) {
            NSLog(@"result:%@ path:%@",result.text,result.recFilePath);
            if (result.text == nil) {
                NSLog(@"返回结果空！！！");
                return;
            }
            /*获取本地播放文件路径*/
            self.audioPath = result.recFilePath;
            NSLog(@"%@filename",self.audioPath);
            dispatch_async(dispatch_get_main_queue(), ^{
//                NSLog(@"CSCS%@",value);
//                self.resultTextView.text = result.text;
                [self showResult:result.text];
                //[self.indicatorView stopAnimating];
            });
        };
        handler.onError = ^(NSString * _Nonnull eval, ChivoxAIEvalResult *
        _Nonnull result) {
            NSLog(@"error:%@111111%@----%@",result.text,result.recFilePath,eval);
            NSData *jsonData = [result.text dataUsingEncoding:NSUTF8StringEncoding];
            NSError *err;
            NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
            if(err) {/*JSON解析失败*/
                NSLog(@"JSON解析失败");
            }
            NSString *errId = [[dic objectForKey:@"errId"] stringValue];
            NSString *error = [dic objectForKey:@"error"];
            error = [self errIDString:errId errorInfo:error];

            dispatch_async(dispatch_get_main_queue(), ^{
                self.resultTextView.text = result.text;
                [self errAlert:error errID:errId];
                //[self.indicatorView stopAnimating];
            });
        };
        handler.onVad = ^(NSString * _Nonnull eval, ChivoxAIEvalResult *
        _Nonnull result) {
            NSLog(@"vad:%@111111",result.text);
            NSData *jsonData = [result.text dataUsingEncoding:NSUTF8StringEncoding];
            NSError *err;
            NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
            if(err) {/*JSON解析失败*/
                NSLog(@"JSON解析失败");
            }
            NSNumber *vad_status=[dic objectForKey:@"vad_status"];
            // vad_status==0 : no voice;
            // vad_status==1 : is speaking;
            // vad_status==2 : speaking end;
            // depends on the vad_status, execute the service logic
            if ([vad_status intValue] ==2)
            {
                [self.engineC stop];
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.resultTextView.text = result.text;
                    [self.recordButton setTitle:@"record" forState: UIControlStateNormal];
                    //[self.indicatorView stopAnimating];
                });
            }else
            {
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.resultTextView.text = result.text;
                    //[self.indicatorView stopAnimating];
                });
            }
        };
        handler.onOther = ^(NSString * _Nonnull eval, ChivoxAIEvalResult *
         _Nonnull result) {
            NSLog(@"other:%@111111",result.text);
        };
        handler.onSoundIntensity = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"sound:%@111111",result.text);
            dispatch_async(dispatch_get_main_queue(), ^{
                self.resultTextView.text = result.text;
                //[self.indicatorView stopAnimating];
            });
        };
        ChivoxAIInnerRecorder *innerRecorder = [[ChivoxAIInnerRecorder alloc] init];//创建音频源
        innerRecorder.recordParam.channel = 1;
        innerRecorder.recordParam.sampleRate = 16000;
        innerRecorder.recordParam.sampleBytes = 2;
        innerRecorder.recordParam.duration = 30000;
        /*获取本地播放文件路径*/
        NSString *filename = [[NSString alloc] initWithFormat:@"%@/Documents/record/test.wav",NSHomeDirectory()];
        innerRecorder.recordParam.saveFile = filename;
//        innerRecorder.recordParam.saveFile = [NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES).firstObject stringByAppendingString:[NSString stringWithFormat:@"/%@.wav",[NSString stringWithFormat:@"%@",@(NSDate.date.timeIntervalSince1970)].md5String]];

        NSMutableDictionary *param = [[NSMutableDictionary alloc] init];//创建测评参数
        NSMutableDictionary *audio = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *app = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *vad = [[NSMutableDictionary alloc] init];
        [param setObject:@1 forKey:@"soundIntensityEnable"];
        [param setObject:@"cloud" forKey:@"coreProvideType"];
        /*audio音频数据传参*/
        [audio setObject:@"wav" forKey:@"audioType"];
        [audio setObject:@16000 forKey:@"sampleRate"];
        [audio setObject:@2 forKey:@"sampleBytes"];
        [audio setObject:@1 forKey:@"channel"];
        param[@"audio"] = audio;
//        param[@"soundIntensityEnable"] = @1;
        /*appUser传参*/
        [app setValue:@"iOS-user" forKey:@"userId"];
        param[@"app"] = app;
        
        /*vad*/
        if ([self.coretype isEqual: @"en.word.pron"] || [self.coretype isEqual: @"en.sent.pron"] || [self.coretype isEqual: @"en.choc.score"]  )
        {
            [vad setObject:@1 forKey:@"vadEnable"];
        }else
        {
            [vad setObject:@0 forKey:@"vadEnable"];
        }
        
        [vad setObject:@1 forKey:@"refDuration"];
        [vad setObject:@20 forKey:@"speechLowSeek"];
        param[@"vad"] = vad;
        
        //
        CoreRequestParam *coreRequest = [[CoreRequestParam alloc] init];
        NSMutableDictionary *requestDic = [NSMutableDictionary new];
        
        if ([self.coretype isEqual: @"en.word.pron"])
        {
            requestDic = [coreRequest requestDic_EN_Word_pron];//Word
        }
        else if ([self.coretype isEqual: @"en.sent.pron"])
        {
            requestDic = [coreRequest requestDic_EN_Sent_pron];//Sentence
        }
        else if ([self.coretype isEqual: @"en.pred.score"])
        {
            requestDic = [coreRequest requestDic_EN_Pred_score];//Paragraph
        }
        else if ([self.coretype isEqual: @"en.choc.score"])
        {
            requestDic = [coreRequest requestDic_EN_Choc_score];//Multiple choice
        }
        else if ([self.coretype isEqual: @"en.scne.exam"])
        {
            requestDic = [coreRequest requestDic_EN_Scne_exam];//Situational Dialogue
        }
        else if ([self.coretype isEqual: @"en.prtl.exam"])
        {
            requestDic = [coreRequest requestDic_EN_Prtl_exam];//Describe a picture
        }
        else if ([self.coretype isEqual: @"en.asr.rec"])
        {
            requestDic = [coreRequest requestDic_EN_Asr_rec];//Asr
        }
        else {
            NSLog(@"unsupport");
            //requestDic = [coreRequest requestDic_EN_Word_score];//英文单词评测
        }

        [param setObject:requestDic forKey:@"request"];
        NSLog(@"%@", param);
        ChivoxAIRetValue * e  = nil;
        [ChivoxAIRecorderNotify sharedInstance].onRecordStart = ^{
            NSLog(@"start recorder");
        };
        [ChivoxAIRecorderNotify sharedInstance].onRecordStop = ^{
            NSLog(@"stop recorder123");
            dispatch_async(dispatch_get_main_queue(), ^{
                [self.recordButton setTitle:@"record" forState: UIControlStateNormal];
            });
            
        };
        NSMutableString *tokenid = [[NSMutableString alloc] init];//tokenid传入 获取
        e = [self.engineC start:innerRecorder tokenId:tokenid param:param listener:handler]; //Start assessment
        if (0 != [e errId]){
            NSLog(@"失败原因：%@",e);//打印失败原因
        }
        resultTextView.text = @"";
        //[indicatorView startAnimating];
        [recordButton setTitle:@"stop" forState: UIControlStateNormal];
    } else {
        ChivoxAIRetValue *value = [self.engineC stop];
        NSLog(@"value%@",value);
        [recordButton setTitle:@"record" forState: UIControlStateNormal];
    }
}

- (IBAction)playbackButtonPressed
{
//    if (recorder == NULL) {
//        return;
//    }
//    airecorder_playback(recorder);
//    ET = aiplayer_callback(player);
//    [self changeButtonImage:ET];
//    AVAudioPlayer *playStatus = (AVAudioPlayer *)CFBridgingRelease(player);
//    [self audioPlayerDidFinishPlaying:playStatus successfully:YES];
    //AVAudioSession是一个单例类
//    AVAudioSession *session = [AVAudioSession sharedInstance];
    //AVAudioSessionCategorySoloAmbient是系统默认的category
//    [session setCategory:AVAudioSessionCategorySoloAmbient error:nil];
    //激活AVAudioSession
//    [session setActive:YES error:nil];
    
    
    //cs
    if (self.audioPath == NULL) {
        NSLog(@"audiopath is nil!");
        return;
    }
    self.player = [ChivoxAIAudioPlayer sharedInstance];
    ChivoxAIAudioPlayerListener *event = [[ChivoxAIAudioPlayerListener alloc] init];
    event.onStarted = ^(ChivoxAIAudioPlayer *ap) {
        [self.playbackButton setTitle:@"stop" forState: UIControlStateNormal];
        self.playbackButton.userInteractionEnabled=NO;//交互关闭
        NSLog(@"start");
    };
    event.onError = ^(ChivoxAIAudioPlayer *ap, NSString *err) {
        self.playbackButton.userInteractionEnabled=YES;//交互开启
        [self.playbackButton setTitle:@"playback" forState: UIControlStateNormal];
        NSLog(@"error : %@",err);
    };
    event.onStopped = ^(ChivoxAIAudioPlayer *ap) {
        self.playbackButton.userInteractionEnabled=YES;//交互开启
        [self.playbackButton setTitle:@"playback" forState: UIControlStateNormal];
        NSLog(@"stop");
    };
    [self.player setListener:event];
    [self.player playFile:self.audioPath];
    
}

- (void)changeButtonImage:(int)ETnum{
    if (ETnum == 0) {
        [self changeButtonImage:ETnum];
    } else if (ETnum == 1) {
        self.playbackButton.imageView.image = [UIImage imageNamed:@"REPLAY-ing.png"];
    }else {
        NSLog(@"ERROR:ET is nil");
    }
}

- (IBAction)BackActionNew:(id)sender {
    NSLog(@"asdf");
     //模态返回上一个界面
    [self.presentingViewController dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark - 附带功能
/*显示提示框*/
- (void)errAlert:(NSString *)errMessage errID:(NSString *)errid{
    UIAlertController * alert = [UIAlertController alertControllerWithTitle:errid
                                                                   message:errMessage
                                                            preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction * defaultAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault
                                                          handler:^(UIAlertAction * action) {
                                                              //响应事件
                                                              NSLog(@"action = %@", action);
                                                          }];
    [alert addAction:defaultAction];
    [self presentViewController:alert animated:YES completion:nil];
}

//常见错误码提示
- (NSString *)errIDString:(NSString *)errID errorInfo:(NSString *)errorInfo{
    if([errID isEqualToString:@"51000"]){
        errorInfo = @"请检查文本格式";
    }else if ([errID isEqualToString:@"40092"]){
        errorInfo = @"录音时长超过内核最大时长";
    }else if ([errID isEqualToString:@"41030"]){
        if([errorInfo isEqualToString:@"unauthorized: timestamp expired"]){
            errorInfo = @"请校验设备时间";
        }else if ([errorInfo isEqualToString:@"unauthorized: invalid coreType"]){
            errorInfo = @"请确认填入正确的coreType类型";
        }
    }else if ([errID isEqualToString:@"60010"]){
        errorInfo = @"网络未连接";
    }else if ([errID isEqualToString:@"60014"]){
        errorInfo = @"网络超时，请稍后重试";
    }
    return errorInfo;
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

@end
