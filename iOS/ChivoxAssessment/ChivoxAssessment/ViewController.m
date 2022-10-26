//
//  ViewController.m
//  ChivoxDemo-E
//


#import "ViewController.h"
#import "CSMainViewController.h"

@interface ViewController ()

@end

@implementation ViewController

- (void)initAll
{
    NSMutableDictionary *cfg = [[NSMutableDictionary alloc] init]; //
    
    NSString *appKey = @"your appKey";
    NSString *secretKey = @"your secretKey";
    
    [cfg setObject:appKey forKey:@"appKey"];
    [cfg setObject:secretKey forKey:@"secretKey"];
    
    
    NSString * provision = [[NSBundle mainBundle] pathForResource:@"aiengine" ofType:@"provision"];
    [cfg setObject:provision forKey:@"provision"];
    
    //Get sdk version
    NSLog(@"commonSDK:%@,SecSDK:%@",[[ChivoxAIEngine sdkInfo] commonSdkVersion],[[ChivoxAIEngine sdkInfo] version]);
    
    //set cloud field parameter
    NSMutableDictionary *cloud = [[NSMutableDictionary alloc] init];
    [cloud setObject:@1 forKey:@"enable"];
    [cloud setObject:@1 forKey:@"protocol"];
    [cloud setObject:@20 forKey:@"connectTimeout"];
    [cloud setObject:@10 forKey:@"serverTimeout"];
    cfg[@"cloud"] = cloud;

    /*Log*/
    NSMutableDictionary *logDic = [self profLog];
    [cfg setValue:logDic forKey:@"prof"];
    
    /*vad */
    NSMutableDictionary *vad = [self vadInit];
    cfg[@"vad"] = vad;
    
    [self.cloudengine setLogFile:@"/Users/chensong/Desktop/log1.txt"];

    //
    ChivoxAIEngineCreateCallback *cb = [ChivoxAIEngineCreateCallback
          onSuccess:^(ChivoxAIEngine * _Nonnull engine) {
        self.cloudengine = engine;
        NSLog(@"success: %@",engine);
        //Get sdk version
        NSLog(@"commonSDK:%@,SecSDK:%@",[[ChivoxAIEngine sdkInfo] commonSdkVersion],[[ChivoxAIEngine sdkInfo] version]);
        NSLog(@"%@\n%@",[[ChivoxAIEngine sdkInfo] commonSdkModules],[[ChivoxAIEngine sdkInfo] versionBuild]);
        NSLog(@"%d\n%d\n%d\n%d",[[ChivoxAIEngine sdkInfo] versionMajor],[[ChivoxAIEngine sdkInfo] versionMinor],[[ChivoxAIEngine sdkInfo] versionPatch],[[ChivoxAIEngine sdkInfo] versionTweak]);
    } onFail:^(ChivoxAIRetValue * _Nonnull err) {
        NSLog(@"fail: %@",err);
        dispatch_async(dispatch_get_main_queue(), ^{
            [self errAlert:[NSString stringWithFormat:@"%@", err] errID:@"ERROR"];
        });
    }];
    //create engine instance
    [ChivoxAIEngine create:cfg cb:cb];
}

//setup for real machine
-(void)micPermissionRequest
{
    if (TARGET_OS_SIMULATOR == 0)
    {
        AVAudioSession* sharedSession = [AVAudioSession sharedInstance];
        [sharedSession requestRecordPermission:^(BOOL granted) {    }];
    }
}


- (void)viewDidLoad {
    [super viewDidLoad];
    if (self.cloudengine == NULL){
        NSLog(@"%@###init", self.cloudengine);
        [[NSFileManager defaultManager] createDirectoryAtPath:[NSHomeDirectory() stringByAppendingFormat:@"/Documents/record"] withIntermediateDirectories:YES attributes:nil error:nil];
        [self performSelectorInBackground:@selector(initAll) withObject:nil];
    }else{
        NSLog(@"engine is inactive");
    }
    // Do any additional setup after loading the view.
}

//Word kernel
- (IBAction)AButtonAction:(id)sender {
    NSLog(@"A");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.word.pron"];
    mainVC.engineC = self.cloudengine;

    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}

//Sentence Kernel
- (IBAction)BButtonAction:(id)sender {
    NSLog(@"B");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.sent.pron"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}

- (IBAction)CButtonAction:(id)sender {
    NSLog(@"C");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.pred.score"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}

- (IBAction)DButtonAction:(id)sender {
    NSLog(@"D");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.choc.score"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}


- (IBAction)EButtonAction:(id)sender {
    NSLog(@"E");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.scne.exam"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}

- (IBAction)FButtonAction:(id)sender {
    NSLog(@"F");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.prtl.exam"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}


- (IBAction)GButtonAction:(id)sender {
    NSLog(@"G");
    CSMainViewController *mainVC = [CSMainViewController new];
    mainVC.coretype = [NSString stringWithFormat:@"en.asr.rec"];
    mainVC.engineC = self.cloudengine;
    //模态画面风格
    mainVC.modalTransitionStyle=UIModalTransitionStyleCrossDissolve;
    mainVC.modalPresentationStyle=UIModalPresentationFullScreen;
    [self presentViewController:mainVC animated:YES completion:nil];
}

- (IBAction)HButtonAction:(id)sender {
    NSLog(@"H");
}


#pragma mark - 附带功能
/*Display Prompt box*/
- (void)errAlert:(NSString *)errMessage errID:(NSString *)errid{
    UIAlertController * alert = [UIAlertController alertControllerWithTitle:errid
                                                                   message:errMessage
                                                            preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction * defaultAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault
                                                          handler:^(UIAlertAction * action) {
                                                              
                                                              NSLog(@"action = %@", action);
                                                          }];
    [alert addAction:defaultAction];
    [self presentViewController:alert animated:YES completion:nil];
}

/*Enabling Debug Logs*/
- (NSMutableDictionary *)profLog{
    NSMutableDictionary *logDic = [[NSMutableDictionary alloc] init];
    NSString *logFileName = @"/Users/chensong/Desktop/log.txt";
    [logDic setValue:[NSNumber numberWithInt:0] forKey:@"enable"];
    [logDic setValue:logFileName forKey:@"output"];
    return logDic;
}

/*Enable the vad function*/
- (NSMutableDictionary *)vadInit{
    NSMutableDictionary *vadDic = [[NSMutableDictionary alloc] init];
    
    NSString * vadPath = [[NSBundle mainBundle]pathForResource:@"vad.0.13" ofType:@"bin"];
    [vadDic setObject:@1 forKey:@"enable"];
    [vadDic setObject:vadPath forKey:@"res"];

    [vadDic setObject:@16000 forKey:@"sampleRate"];
    [vadDic setObject:@0 forKey:@"strip"];
    
    return vadDic;
}

@end
