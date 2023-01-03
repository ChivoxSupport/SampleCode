//
//  CSMainViewController.h
//  ChivoxNewDemo
//
//  Created by 陈松 on 2019/6/24.
//  Copyright © 2019 陈松. All rights reserved.
//

#import "ViewController.h"
#import "CAIEngine.h"

NS_ASSUME_NONNULL_BEGIN

@interface CSMainViewController : UIViewController

@property (nonatomic, retain) IBOutlet UIButton * recordButton;
@property (nonatomic, retain) IBOutlet UIButton * playbackButton;
@property (nonatomic, retain) IBOutlet UITextView *resultTextView;
@property (weak, nonatomic) IBOutlet UITextView *refTextView;
@property (strong, nonatomic) IBOutlet UIImageView *prtlImage;

@property (nonatomic, retain) NSNumber *enginetype;
@property (nonatomic, retain) NSString *coretype;
@property (nonatomic, retain) NSString *AudioSrc;


@property (nonatomic, retain) NSString * audioPath;

@property (nonatomic, retain) ChivoxAIAudioPlayer *player;
@property (nonatomic, retain) ChivoxAIEngine * engineC;

- (IBAction) recordButtonPressed;
- (IBAction) playbackButtonPressed;

- (void) AssessmentInnerRecorderMode;
- (void) AssessmentOutFeedMode;

- (void) showResult: (NSString *) text;
- (void)changeButtonImage:(int)ETnum;
@end

NS_ASSUME_NONNULL_END
