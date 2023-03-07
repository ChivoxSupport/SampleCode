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
    if([self.AudioSrc isEqual:@"OuterFeed"])
    {
        self.refTextView.text = @"Please click on the microphone icon (no recording required) and the SDK will read the recorded audio file for assessment";
        self.refTextView.editable=NO;
        return;
    }
    
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
        self.refTextView.text = @"Tell me an animal that can fly.\n A. Tiger \n B. Panda\n C. Bird \n";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.scne.exam"])
    {
        self.refTextView.text = @"Please answer the question based on the text below.\n Boy: I love summer because I can go to the beach, Which seasons is your favourite?\n Girl: Spring, It's warm but it's not too hot. There are beautiful trees and flowers everywhere. \nQuestion: \nWhy does the boy like summer?\n Your answer: ____\n";
        self.refTextView.editable=NO;
    }
    else if ([self.coretype isEqual: @"en.prtl.exam"])
    {
        //self.refTextView.text = @"Please describe the picture in English in 60 seconds according to the picture below. Your description can start like this:\n Look, what are these animals doing now in the zoo? ...\n";
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
        NSLog(@"unsupport");
    }
    resultTextView.text = @"";
    [self micPermissionRequest];
}

-(void)micPermissionRequest
{
    if (TARGET_OS_SIMULATOR == 0)
    {
        AVAudioSession* sharedSession = [AVAudioSession sharedInstance];
        [sharedSession requestRecordPermission:^(BOOL granted) {    }];
    }
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
        NSLog(@"Json parsing failure：%@",err);
    }
    
    NSString *overall = @"";

    if ([self.coretype isEqual: @"en.word.pron"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        overall = [resultdic2 objectForKey:@"overall"];
        
        NSString *str =@"\n Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@", str,overall];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.sent.pron"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *overall = [resultdic2 objectForKey:@"overall"];
        
        NSMutableDictionary * resultdic3 = [resultdic2 objectForKey:@"fluency"];
        NSString *fluency = [resultdic3 objectForKey:@"overall"];
        
        NSString *integrity = [resultdic2 objectForKey:@"integrity"];
        
        NSString *accuracy = [resultdic2 objectForKey:@"accuracy"];

        NSLog(@"#%@#%@#%@#%@",overall,fluency,integrity,accuracy);
        
        NSString *str =@"\n\nOverall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@", str,overall,@"\nfluency score: ",fluency,@"\nintegrity score: ",integrity,@"\naccuracy score: ",accuracy];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.pred.score"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *overall = [resultdic2 objectForKey:@"overall"];

        NSMutableDictionary * resultdic3 = [resultdic2 objectForKey:@"fluency"];
        NSString *fluency = [resultdic3 objectForKey:@"overall"];
        
        NSString *integrity = [resultdic2 objectForKey:@"integrity"];
        
        NSString *accuracy = [resultdic2 objectForKey:@"accuracy"];

        NSLog(@"#%@#%@#%@#%@",overall,fluency,integrity,accuracy);
        
        NSString *str =@"\nOverall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@", str,overall,@"\nfluency score: ",fluency,@"\nintegrity score: ",integrity,@"\naccuracy score: ",accuracy];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.choc.score"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        overall = [resultdic2 objectForKey:@"overall"];

        NSString *str =@"\n Overall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@", str,overall];
        
        NSLog(@"############%@",resultForShow);
    }
    else if ([self.coretype isEqual: @"en.scne.exam"])
    {
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *Overall = [resultdic2 objectForKey:@"overall"];

        NSMutableDictionary * details = [resultdic2 objectForKey:@"details"];
        
        NSMutableDictionary * multi_dim = [details objectForKey:@"multi_dim"];
        
        NSString *Grammar = [multi_dim objectForKey:@"grammar"];
    
        NSString *Content = [multi_dim objectForKey:@"cnt"];
        
        NSString *Pronunciation = [multi_dim objectForKey:@"pron"];
        
        NSString *Fluency = [multi_dim objectForKey:@"flu"];

        NSLog(@"#%@#%@#%@#%@#%@",Overall,Grammar,Content,Pronunciation,Fluency);
        
        NSString *str =@"\nOverall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@%@%@%@%@%@%@%@", str,Overall,@"/4",@"\nGrammar score: ",Grammar,@"/4", @"\nContent score: ",Content,@"/4", @"\nFluency score: ",Fluency,@"/4", @"\nPronunciation score: ",Pronunciation,@"/4"];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.prtl.exam"])
    {
        
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
        
        NSString *Overall = [resultdic2 objectForKey:@"overall"];

        NSMutableDictionary * details = [resultdic2 objectForKey:@"details"];
        
        NSMutableDictionary * multi_dim = [details objectForKey:@"multi_dim"];
        
        NSString *Grammar = [multi_dim objectForKey:@"grammar"];
        
        NSString *Content = [multi_dim objectForKey:@"cnt"];
        
        NSString *Pronunciation = [multi_dim objectForKey:@"pron"];
        
        NSString *Fluency = [multi_dim objectForKey:@"flu"];
        

        NSLog(@"#%@#%@#%@#%@#%@",Overall,Grammar,Content,Pronunciation,Fluency);
        
        NSString *str =@"\nOverall score: ";
        resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@%@%@%@%@%@%@%@", str,Overall,@"/4",@"\nGrammar score: ",Grammar,@"/4", @"\nContent score: ",Content,@"/4", @"\nFluency score: ",Fluency,@"/4", @"\nPronunciation score: ",Pronunciation,@"/4"];
        
        NSLog(@"############%@",resultForShow);
        
    }
    else if ([self.coretype isEqual: @"en.asr.rec"])
    {
        NSString *eof = [resultDic objectForKey:@"eof"];
                
        BOOL isEnd = [eof intValue];
                
        NSMutableDictionary * resultdic2 = [resultDic objectForKey:@"result"];
                
        NSMutableArray * Align = [resultdic2 objectForKey:@"align"];
                
        NSString * recText = @"";

        NSString *text = @"";

        NSString *sep = @"";
                
                
        for (NSMutableDictionary *dict in Align)
        {
            text = [dict objectForKey:@"txt"];
            sep = [dict objectForKey:@"sep"];

            //NSLog(@"json txt:%@",text);
            //NSLog(@"json sep:%@",sep);
                    
            recText = [recText stringByAppendingFormat:@"%@%@%@",text, sep, @" "];
                                        
        }

        NSLog(@"\n\nReal-time recognition result %@",recText);
                
        //This is the final result
        if( isEnd )
        {
            NSString *overall = [resultdic2 objectForKey:@"overall"];
                    
            NSMutableDictionary * resultdic3 = [resultdic2 objectForKey:@"fluency"];
                    
            NSString *fluency = [resultdic3 objectForKey:@"overall"];
            NSString *speed = [resultdic3 objectForKey:@"speed"];
            NSString *pause= [resultdic3 objectForKey:@"pause"];
                    
            NSString *pron = [resultdic2 objectForKey:@"pron"];
          
            NSString *str =@"\nOverall: ";
            resultForShow = [NSString stringWithFormat:@"%@%@%@%@%@%@%@%@%@%@%@%@", str,overall,@"\nFluency score: ",fluency,@"\nSpeed(words/min): ",speed,@"\nNumber of pauses: ",pause,@"\nPronunciation score:", pron, @"\nRecognition result:", recText];
        }
        else
        {        
            resultForShow = [@"\n\n" stringByAppendingString: recText];
        }

    }
    else {
        NSLog(@"unsupport");
    }
    
    resultTextView.text = resultForShow;
    
}

- (IBAction)recordButtonPressed
{
    
    if ([self.AudioSrc isEqual: @"OuterFeed"])
    {
        //External Recorder
        [ self AssessmentOutFeedMode ];
        
    }
    else if([self.AudioSrc isEqual: @"InnerRecorder"])
    {
        //SDK Built-in Recorder
        [ self AssessmentInnerRecorderMode ];
        
    }
}


- (void)AssessmentInnerRecorderMode
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
                NSLog(@"Json parsing failure");
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
                NSLog(@"Json parsing failure");
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
                    self.resultTextView.text = [@"\n" stringByAppendingString:result.text];
                    [self.recordButton setTitle:@"record" forState: UIControlStateNormal];
                    //[self.indicatorView stopAnimating];
                });
            }else
            {
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.resultTextView.text = [@"\n" stringByAppendingString:result.text];
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
                self.resultTextView.text = [@"\n" stringByAppendingString:result.text];
                //[self.indicatorView stopAnimating];
            });
        };
        ChivoxAIInnerRecorder *innerRecorder = [[ChivoxAIInnerRecorder alloc] init];//Create audio source
        innerRecorder.recordParam.channel = 1;
        innerRecorder.recordParam.sampleRate = 16000;
        innerRecorder.recordParam.sampleBytes = 2;
        innerRecorder.recordParam.duration = 30000;
        /*获取本地播放文件路径*/
        NSString *filename = [[NSString alloc] initWithFormat:@"%@/Documents/record/test.wav",NSHomeDirectory()];
        innerRecorder.recordParam.saveFile = filename;
//        innerRecorder.recordParam.saveFile = [NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES).firstObject stringByAppendingString:[NSString stringWithFormat:@"/%@.wav",[NSString stringWithFormat:@"%@",@(NSDate.date.timeIntervalSince1970)].md5String]];

        NSMutableDictionary *param = [[NSMutableDictionary alloc] init];//create assessment parameter
        NSMutableDictionary *audio = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *app = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *vad = [[NSMutableDictionary alloc] init];
        
        /*soundIntensityEnable*/
        if ([self.coretype isEqual: @"en.asr.rec"])
        {
            [param setObject:@0 forKey:@"soundIntensityEnable"];
        }else
        {
            [param setObject:@1 forKey:@"soundIntensityEnable"];
        }

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

//External recording mode: Read pre-recorded audio files for evaluation
- (void)AssessmentOutFeedMode
{
    const char * button_title = [[recordButton titleForState:UIControlStateNormal] UTF8String];
    if(strcmp(button_title, "record") == 0) {
        //outter
        ChivoxAIEvalResultListener *handler = [[ChivoxAIEvalResultListener alloc] init];//创建监听对象
        handler.onEvalResult = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"result:%@ path:%@",result.text,result.recFilePath);
            if (result.text == nil) {
                NSLog(@"返回结果空！！！");
                return;
            }
            /*获取本地播放文件路径*/
            self.audioPath = result.recFilePath;
            NSLog(@"%@filename",self.audioPath);
            dispatch_async(dispatch_get_main_queue(), ^{
                //self.resultTextView.text = result.text;
                [self showResult:result.text];
                //[self.indicatorView stopAnimating];
            });
        };
        handler.onError = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"error:%@111111",result.text);
            NSData *jsonData = [result.text dataUsingEncoding:NSUTF8StringEncoding];
            NSError *err;
            NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
            if(err) {/*JSON解析失败*/
                NSLog(@"JSON解析失败");
            }
            NSString *errId = [[dic objectForKey:@"errId"] stringValue];
            NSString *error = [dic objectForKey:@"error"];
            dispatch_async(dispatch_get_main_queue(), ^{
                self.resultTextView.text = result.text;
                [self errAlert:error errID:errId];
                //[self.indicatorView stopAnimating];
            });
        };
        handler.onVad = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"vad:%@111111",result.text);
        };
        handler.onOther = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"other:%@111111",result.text);
        };
        handler.onSoundIntensity = ^(NSString * _Nonnull eval, ChivoxAIEvalResult * _Nonnull result) {
            NSLog(@"sound:%@111111",result.text);
        };
        ChivoxAIOuterFeed * outfeed = [[ChivoxAIOuterFeed alloc] init];//创建音频源

        NSMutableDictionary *param = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *audio = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *app = [[NSMutableDictionary alloc] init];
        
        [param setObject:@1 forKey:@"soundIntensityEnable"];
        [param setObject:@"cloud" forKey:@"coreProvideType"];
        /*audio音频数据传参*/
        [audio setObject:@"wav" forKey:@"audioType"];
        [audio setObject:@16000 forKey:@"sampleRate"];
        [audio setObject:@2 forKey:@"sampleBytes"];
        [audio setObject:@1 forKey:@"channel"];
        param[@"audio"] = audio;
        param[@"soundIntensityEnable"] = @1;
        /*appUser传参*/
        [app setValue:@"iOS-user" forKey:@"userId"];
        param[@"app"] = app;
        
        /*内核request传参*/
        /*英文内核*/
        CoreRequestParam *coreRequest = [[CoreRequestParam alloc] init];

        NSMutableDictionary *requestDic = [coreRequest requestDic_EN_Sent_pron];//英文句子评测

        [requestDic setValue:[NSNumber numberWithInt:100] forKey:@"rank"];//总分分制
        [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"attachAudioUrl"];
        [requestDic setValue:[NSNumber numberWithInt:1] forKey:@"precision"];//段落评分精度设置（0.5/1）
        [param setObject:requestDic forKey:@"request"];
        NSLog(@"%@", param);
        
        ChivoxAIRetValue *ret = nil;
        NSLog(@"%d",[ret errId]);
        NSLog(@"%@",ret);
        [ChivoxAIRecorderNotify sharedInstance].onRecordStart = ^{
        NSLog(@"start recorder");
        };
        [ChivoxAIRecorderNotify sharedInstance].onRecordStop = ^{
        NSLog(@"stop recorder");
        };
        NSMutableString *tokenid = [[NSMutableString alloc] init];
        ret = [self.engineC start:outfeed tokenId:tokenid param:param listener:handler];
        NSLog(@"oooo%@",ret);
        if(0 != [ret errId]){
            NSLog(@"失败原因：%@",ret);//打印失败原因
        }
        resultTextView.text = @"";
        //[indicatorView startAnimating];
        [recordButton setTitle:@"stop" forState: UIControlStateNormal];
        
        int   bytes = 0;
        char  buf[1024]={0};
        FILE *file = NULL;
        NSString *path = [[NSBundle mainBundle] pathForResource:@"Thank-you-for-coming-to-see-me" ofType:@"wav"];
        const char * audiopath = [path UTF8String];
        file = fopen(audiopath, "rb");//待评分音频地址
        if(!file){
            printf("read file error!\n");
            return;
        }
//        fseek(file, 44, SEEK_SET);
        while ((bytes = (int)fread(buf, 1, 1024, file))){
//            ChivoxAIRetValue *value = [self.cloudengine feed:buf length:bytes];
//            NSData *content=[NSData dataWithBytes:buf length:1024];
//            ret = [self.cloudengine feed:content.bytes length:content.length];
            ret = [self.engineC feed:buf length:bytes];
            NSLog(@"csd:%@",ret);
            if (0 != [ret errId]) {
                // feed 调用失败, 一般是因为调用顺序错误. 请查看ret.errId, ret.error分析原因 // 建议在此调用 eval.cancel() 取消评测
                NSLog(@"cs-%@",ret);//打印失败原因
                [self.engineC cancel];
                return;
            }
        }
        // feed 调用成功
        [self.engineC stop];
        [recordButton setTitle:@"record" forState: UIControlStateNormal];
        
    } else {
        [self.engineC stop];
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
