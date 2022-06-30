<template>
    <view class="tabs">
		<!-- menu -->
        <scroll-view id="tab-bar" class="scroll-h" :scroll-x="true" :show-scrollbar="false" :scroll-into-view="scrollInto">
            <view v-for="(tab,index) in tabBars" :key="tab.id" class="uni-tab-item" :id="tab.id" :data-current="index" @click="ontabtap">
                <text class="uni-tab-item-title" :class="tabIndex==index ? 'uni-tab-item-title-active' : ''">{{tab.name}}</text>
            </view>
        </scroll-view>
        <view class="line-h"></view>
		
		<!-- title -->
        <swiper :current="tabIndex" class="swiper-box" :duration="300" @change="ontabchange" >
            <swiper-item v-for="(tab,index1) in newsList" :key="index1">
				<view class="buju">
					<view class="pron"><text>{{tab.trans}}</text></view>
					<view class="text">
						<text v-if="coreType != 'en.asr.rec'">{{tab.text}}</text>
						<text v-else >{{recStr}}</text>
					</view>
					<view class="pron">{{tab.pron}}</view>
					<view v-if="index1 == 5">
						<image class="img"  src="../../static/animals.png"></image>
						<button class="btnAns" @click="getAnswer2()">{{btnText}}</button>
					</view>
					<view v-if="index1 == 4">
						<button class="btnAns" @click="getAnswer1()">{{btnText}}</button>
					</view>
				</view>
            </swiper-item>
        </swiper>
		<view style="margin-top: 10px;margin-left: 5%;">
			<p v-if="answer1" style="width:60%;float: right;">
				<text style="color: #007AFF;">
					Answer1: \n He likes summer because he can go to the beach.
					Answer2: \n Because he can go to the beach.
					Answer3: \n So he can go to the beach.
					Answer4: \n He can go to the beach.
				</text>
			</p>
			<p v-if="answer2">
				<text style="color: #007AFF;">
					Answer1: \n Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running!
					\n Answer2: \n Look, what are these animals doing now in the zoo? There are four elephants in the zoo and they are drinking water. Two cute pandas are sleeping on the tree. A monkey is eating a banana. And a tiger is swimming in the water. Two giraffes are running!
					\n Answer3: \n Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two fat pandas are on the tree and they are sleeping. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running!
				</text>
			</p>
		</view>
		<!-- result -->
		<view style="width:55%;text-align: left;float:right;margin-top: 25px;">
			<view>Overall score:　{{totalScore}} <block v-if="coreType=='en.prtl.exam' || coreType=='en.scne.exam'">/4</block></view>
			<!-- word -->
			<view v-if="result && coreType=='en.word.pron'">Standard pronunciation:　{{lab}}</view>
			<view v-if="result && coreType=='en.word.pron'">Your pronunciation:　{{rec}}</view>
			<!-- sent -->
			<view v-if="result && coreType=='en.sent.pron'">Fluency:　{{fluency}}</view>
			<view v-if="result && coreType=='en.sent.pron'">Accuracy:　{{accuracy}}</view>
			<view v-if="result && coreType=='en.sent.pron'">Integrity:　{{integrity}}</view>
			<view v-if="result && coreType=='en.sent.pron'">Misread:　{{Error}}</view>
			<view v-if="result && coreType=='en.sent.pron'">Missing reading:　{{Missed}}</view>
			<view v-if="result && coreType=='en.sent.pron'">Superfluous reading:　{{Multiple}}</view>
			<!-- pred -->
			<view v-if="result && coreType=='en.pred.score'">Fluency:　{{fluency}}</view>
			<view v-if="result && coreType=='en.pred.score'">Accuracy:　{{accuracy}}</view>
			<view v-if="result && coreType=='en.pred.score'">Integrity:　{{integrity}}</view>
			<!-- en.scne.exam -->
			<view v-if="result && coreType=='en.scne.exam'">Fluency:　{{fluency}}/4</view>
			<view v-if="result && coreType=='en.scne.exam'">Pronunciation:　{{pron}}/4</view>
			<view v-if="result && coreType=='en.scne.exam'">Grammar:　{{grammar}}/4</view>
			<view v-if="result && coreType=='en.scne.exam'">Content:　{{content}}/4</view>
			<!-- en.prtl.exam -->
			<view v-if="result && coreType=='en.prtl.exam'">Fluency:　{{fluency}}/4</view>
			<view v-if="result && coreType=='en.prtl.exam'">Pronunciation:　{{pron}}/4</view>
			<view v-if="result && coreType=='en.prtl.exam'">Grammar:　{{grammar}}/4</view>
			<view v-if="result && coreType=='en.prtl.exam'">Content:　{{content}}/4</view>
			<!-- en.asr.rec -->
			<view v-if="result && coreType=='en.asr.rec'">Fluency:　{{fluency}}</view>
			<view v-if="result && coreType=='en.asr.rec'">Pronunciation:　{{pron}}</view>
			<view v-if="result && coreType=='en.asr.rec'">Speed:　{{speed}} word/minute</view>
		</view>
		
		
		
		
		<!-- record Button -->
		<view>
			<block v-if="recording">
				<view class="btn">
					<view class="txt"><text>tap to end</text></view>
					<view @tap="stopRecord()">
						<image src="https://api.chivoxapp.com/tstest/chivoxapp/uniapp/wave.gif" class="bowen" mode="aspectFit"></image>
					</view>
				</view>
			</block>
			<block v-else-if="!recording">
				<view class="btn">
					<view class="txt"><text>tap to start</text></view>
					<view @tap="record()" class="roundBtn">
						<p><image src="https://api.chivoxapp.com/tstest/chivoxapp/uniapp/luyin@2x.png" mode="aspectFit"></image></p>
					</view>
					
					<view @tap="playBack()" class="roundBack">
						<p><image src="../../static/playBack.png" mode="aspectFit"></image></p>
					</view>
				</view>
			</block>
		</view>

    </view>
</template>
<script>
	/* 1.Import SDK */
	import Html5Recorder from '../node_modules/chivox_h5sdk/src/html5/html5recorder';
	import phoneticConvert from "../../static/chivox/phoneticConvert.js"
	/* 2.initialization */

	let sdkStatus = false;
	let player = new Html5Player();
	let sdk = new Html5Recorder({
	    appKey: "your appkey",  /* Appkey authorized by Chivox. */
	    sigurl: "https://your server address/JSSDK/php/sig.php",  /* Interface address for obtaining identity information. */
	    server: "wss://cloud.chivox.com",
	    onInit: function (mess) {
	        console.log("Init success!")
			sdkStatus = true;
	    }, 
	    onError: function (err) {
	        console.log("Init onError:" + JSON.stringify(err))
			if(err.hasOwnProperty("id")){
				uni.showModal({
					title:"Init ERROR",
					content: "errId:" + err.id + "\nerrInfo:" + err.message,
					success:function(res){
						if(res.confirm){
							console.log("confirm")
						}
						if(res.cancel){
							console.log("cancel")
						}
					}
				})
			}else{
				uni.showModal({
					title:"Error",
					content: JSON.stringify(err),
					success:function(res){
						if(res.confirm){
							console.log("confirm")
						}
						if(res.cancel){
							console.log("cancel")
						}
					}
				})
			}
	    }
	});
	
    export default {
        data() {
            return {
				btnText:"Show reference answer",
				answer1: false,
				answer2: false,
				recording: false,
				result: false,
                newsList: [{
					trans: 'n. 苹果',
                    text: "apple",
					pron:"UK ['æp(ə)l]　US [ˈæpl]",
					duration: 4000,
					serverParams: {
						coreType:'en.word.pron',
						refText:"apple",
						rank: 100,
						attachAudioUrl: 1,
						userId: "chivox tester"
					}
                }, {
					trans: '',
                    text: 'Thank you for coming to see me.',
					pron:'',
					duration: 10000,
					serverParams: {
						coreType:'en.sent.pron',
						refText:"Thank you for coming to see me.",
						rank: 100,
						attachAudioUrl: 1,
						userId: "chivox tester"
					}
                }, {
					trans: '',
                    text: "Happiness is not about being immortal nor having food or rights in one's hand. \n It's about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.",
					pron:'',
					duration: 60000,
					serverParams: {
						coreType:'en.pred.score',
						rank: 100,
						precision: 1,
						attachAudioUrl: 1,
						reftext:"Happiness is not about being immortal nor having food or rights in one's hand. It's about having each tiny wish come true, or having something to eat when you are hungry or having someone's love when you need love.",
						userId: "chivox tester"
					}
				}, {
					trans: 'Tell me an animal that can fly.',
                    text: 'A. Tiger.　　B. Panda.　　C. Bird.',
					pron:'',
					duration: 10000,
					serverParams: {
						coreType:'en.choc.score',
						rank: 100,
						attachAudioUrl: 1,
						refText:{"lm": [
							{"answer": 0,"text": "Tiger."},
							{"answer": 0,"text": "Panda."},
							{"answer": 1,"text": "Bird."}]},
						userId: "chivox tester",
					}
				}, {
					trans: "Please answer the question based on the text below. \n\n Boy: I love summer because I can go to the beach. Which seasons is your favourite? \n Girl: Spring. It's not too hot. There are beautiful trees and flowers everywhere. \n",
                    text: '\n Question: Why does the boy like summer? \n Your answer: ...',
					pron:'',
					duration: 40000,
					serverParams: {
						coreType:'en.scne.exam',
						rank: 4,
						precision: 1,
						attachAudioUrl: 1,
						keywords: ["go to","likes","beach"],
						refText:{"lm":[
							{"text": "He likes summer because he can go to the beach."},
							{"text": "Because he can go to the beach."},
							{"text": "So he can go to the beach."},
							{"text": "He can go to the beach."},
							{"text": "Because he likes the beach."},
							{"text": "He likes the beach."}]},
						result: {
							details:{
								use_inherit_rank: 1
							}
						},
						userId: "chivox tester",
					}	
                }, {
					trans:"Please describe the picture in English in 60 seconds according to the picture below. Your description can start like this:",
                    text: 'Look, what are these animals doing now in the zoo? ...',
					pron:'',
					duration: 60000,
					serverParams: {
						coreType:'en.prtl.exam',
						rank: 4,
						precision: 1,
						attachAudioUrl: 1,
						keywords: ["drinking","eating","giraffes"],
						refText: {"lm":[
						{text:"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running!"},
						{text:"Look, what are these animals doing now in the zoo? There are four elephants in the zoo and they are drinking water. Two cute pandas are sleeping on the tree. A monkey is eating a banana. And a tiger is swimming in the water. Two giraffes are running!"},
						{text:"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two fat pandas are on the tree and they are sleeping. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. Two giraffes are running!"},
						{text:"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. There is a monkey over there and it is eating a banana. And a tiger is swimming in the water. Two giraffes are running!"},
						{text:"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. A monkey is sitting over there and eating a banana. There is a tiger in the water and it is swimming. Two giraffes are running!"},
						{text:"Look, what are these animals doing now in the zoo? Four elephants are drinking water. Two cute pandas are sleeping on the tree. A monkey is sitting over there and eating a banana. And a tiger is swimming in the water. There are two giraffes and they are running!"}]},
						result: {
							details:{
								use_inherit_rank: 1
							}
						},
						userId: "chivox tester",
					}
				}, {
					trans:"Please say a paragraph in English ...",
                    text: '',
					pron:'',
					coreType:'en.asr.rec',
					duration: 120000,
					serverParams: {
						coreType:  "en.asr.rec", 
						res:  "en.asr.G4", 
						attachAudioUrl:  1,
						result: {
							details:{
								ext_cur_wrd: 1,
								gop_adjust: 0
							}
						},
						userId: "chivox tester"
					}
                }],
                tabIndex: 0,
                tabBars: [{
                    name: 'Word',
                    id: 'word'
                }, {
                    name: 'Sentence',
                    id: 'sent'
                }, {
                    name: 'Paragraph',
                    id: 'pred'
                }, {
                    name: 'Multiple Choice',
                    id: 'choc'
                }, {
                    name: 'Situational Dialogue',
                    id: 'scne'
                }, {
                    name: 'Speaking with pictures',
                    id: 'prtl'
                }, {
                    name: 'Asr',
                    id: 'asrRec'
                }],
                scrollInto: "",
				coreType:'en.word.pron',
				totalScore: 0,
				url:'',
				/* word */
				rec:'',
				lab:'',
				/* sent */
				fluency: 0,
				accuracy: 0,
				integrity: 0,
				Error: "",
				Missed: "",
				Multiple: "",
				grammar: 0,
				content: 0,
				pron: 0,
				/* asr */
				asrRec: '',
				speed: 0,
				recStr: '',
				internalScore: false
				
            }
        },
        onLoad() {},
        methods: {
			getAnswer1(){
				if (this.btnText == "Show reference answer"){
					this.answer1 = true;
					this.btnText = "Hide reference answer";
				}else if(this.btnText == "Hide reference answer"){
					this.answer1 = false;
					this.btnText = "Show reference answer";
				}
			},
			getAnswer2(){
				if (this.btnText == "Show reference answer"){
					this.answer2 = true;
					this.btnText = "Hide reference answer";
				}else if(this.btnText == "Hide reference answer"){
					this.answer2 = false;
					this.btnText = "Show reference answer";
				}
			},
            ontabtap(e) {
                let index = e.target.dataset.current || e.currentTarget.dataset.current;
                this.switchTab(index);
            },
            ontabchange(e) {
                let index = e.target.current || e.detail.current;
				this.internalScore = false;
                this.switchTab(index);
				this.answer1 = false;
				this.answer2 = false;
				this.btnText = "Show reference answer";
				this.coreType = this.newsList[this.tabIndex].serverParams.coreType;
				this.result = false;
				this.totalScore = 0;
				this.url = '';
				console.log("coreType:"+this.coreType)
				if(this.recording){
					this.stopRecord();
				}
            },
            switchTab(index) {
                this.tabIndex = index;
                this.scrollInto = this.tabBars[index].id;
            },
			
			/* 3.Start recording */
			record(){
				let that = this;
				this.recording = true;
				this.result = false;
				this.internalScore = false;
				this.recStr = '';
				if(sdkStatus){
					sdk.record({
						playDing: true,
					    audioType: "wav", /* mp3 or wav */
					    duration: this.newsList[this.tabIndex].duration,
					    serverParams: this.newsList[this.tabIndex].serverParams,
					    onRecordIdGenerated: function(tokenId) {
					        console.log("tokenId:"+JSON.stringify(tokenId));
					    },
					    onStart: function () {
							console.log("onStart");
					    },        
					    onStop: function () {
							console.log("onStop");
					    },
					    onScore:(score) =>{
							this.recording = false;
							if(score.hasOwnProperty("result")){
								this.result = true;
								this.totalScore = score.result.overall;
								let subUrl = score.audioUrl.split(":");
								let backUrl = subUrl[1].split("/");
								this.url ='https://' + subUrl[0] + '/' + backUrl[1] + '.mp3'; /* Address of playback audio */
								console.log(this.url);
								 /* word */
								if(this.coreType == 'en.word.pron'){
									let rec = score.result.details.word[0].rec; 
									let recAry = rec.split(" "); 
									let stressAry = score.result.details.word[0].stress;
									let accent = score.result.details.word[0].accent;
									let labStr = '';
									let recStr = '';
									/* stress */
									for(let s=0; s<stressAry.length; s++){
										if(stressAry[s].ref == 1){
											labStr += "'";
										}
										let strAry = stressAry[s].char.split("_"); 
										for(let l=0; l<strAry.length; l++){
											if(accent == 2){
												labStr += phoneticConvert.phoneticConvertUS(strAry[l]); //K.K.(US)
											}else{
												labStr += phoneticConvert.phoneticConvert(strAry[l]); //IPA88(UK)
											}
										}
									}
									this.lab = labStr;
									
									for(let i=0; i<recAry.length; i++){
										if(accent == 2){
											recStr += phoneticConvert.phoneticConvertUS(recAry[i]); //K.K.(US)
										}else{
											recStr += phoneticConvert.phoneticConvert(recAry[i]); //IPA88(UK)
										}
									}
									this.rec = recStr;
									
								}/* sent & pred*/
								else if(this.coreType == 'en.sent.pron' || this.coreType == 'en.pred.score'){
									this.fluency = score.result.fluency.overall;
									this.accuracy = score.result.accuracy;
									this.integrity = score.result.integrity;
									if(this.coreType == 'en.sent.pron'){
										let sentDetails = score.result.details;
										let error = '';
										let miss = '';
										let more = '';
										for(let i=0; i<sentDetails.length; i++){
											let wordObj = sentDetails[i];
											switch(wordObj.is_err){
												case 0:
													break;
												case 1:
													if(wordObj.rec != "UNK"){
														more += wordObj.rec+" ";
													}
													break;
												case 2:
													miss += wordObj.lab+" ";
													break;
												case 3:
													error += wordObj.lab+" ";
													
													break;
											}
										}
										this.Error = error;
										this.Missed = miss;
										this.Multiple = more;
									}
								}/* scne &  prtl*/
								else if(this.coreType == 'en.scne.exam' || this.coreType == 'en.prtl.exam'){
									this.fluency = score.result.details.multi_dim.flu;
									this.grammar = score.result.details.multi_dim.grammar;
									this.content = score.result.details.multi_dim.cnt;
									this.pron = score.result.details.multi_dim.pron;
								}/* asr */
								else if(this.coreType == 'en.asr.rec'){
									this.pron = score.result.pron;
									this.fluency = score.result.fluency.overall;
									this.speed = score.result.fluency.speed;
								}
							}else{
								console.log("No Result:"+ JSON.stringify(score))
								uni.showToast({
									title:"No Result:" + JSON.stringify(score),
									icon:"none",
									duration:2000
								})
							}
						},
						onInternalScore: function(score){
							let recStr = '';
							if(score.hasOwnProperty("result")){
								let align = score.result.align;
								align.forEach(function(sub){
									recStr += sub.txt + sub.sep +' ';
								})
							}
							that.internalScore = true;
							that.recStr = recStr;
						},
					    onScoreError: function (err) {
							this.recording = false;
							console.log("onScoreError:"+JSON.stringify(err));
							if(err.hasOwnProperty("error")){
								uni.showModal({
									title:"Error",
									content: "errId:" + err.error.id + "\nerrInfo:" + err.error.msg,
									success:function(res){
										if(res.confirm){
											console.log("confirm")
										}
										if(res.cancel){
											console.log("cancel")
										}
									}
								})
							}else if(err.hasOwnProperty("id")){
								uni.showModal({
									title:"ERROR",
									content: "errId:" + err.id + "\nerrInfo:" + err.message,
									success:function(res){
										if(res.confirm){
											console.log("confirm")
										}
										if(res.cancel){
											console.log("cancel")
										}
									}
								})
							}else{
								uni.showModal({
									title:"Error",
									content: JSON.stringify(err),
									success:function(res){
										if(res.confirm){
											console.log("confirm")
										}
										if(res.cancel){
											console.log("cancel")
										}
									}
								})
							}
							
							
					    }
					})
				}else{
					uni.showToast({
						title:"Record after initialization.",
						icon:"none",
						duration:2000
					})
				}
			},
		
			/* 4.Stop recording */
			stopRecord(){
				this.recording = false;
				sdk.stopRecord();
			},
		
			/* 5.playBack */
			playBack(){
				if(this.url.length != 0 && this.url !=""){
					this.play(this.url);
				}else{
					uni.showToast({
						title:"Please complete the recording before playback.",
						icon:"none",
						duration:2000
					})
				}
			},
			play(url){
				uni.showLoading({
					title:"playing..."
				});
				player.load({
				    url: url,
				    success: function (code,message) {
				        player.play({
				            position: 0,
				            onStop: function() {
				                console.log("player onStop");
								uni.hideLoading();
				            },
				            onStart: function () {
				                console.log("player onStart");
				            }
				        })        
				    },
				    error: function(err) {
				        console.log("player error:" + JSON.stringify(err));
						uni.showModal({
							title:"player error",
							content: JSON.stringify(err),
							success:function(res){
								if(res.confirm){
									console.log("confirm")
								}
								if(res.cancel){
									console.log("cancel")
								}
							}
						})
				    }
				})
			}
		}
    }
</script>

<style>
    page {
        width: 100%;
        min-height: 100%;
        display: flex;
    }
	.btnAns{
		width:165px;
		font-size: 25rpx;
		margin-top: 5px; 
		box-shadow: 0 0 5px #007AFF;
		color: #007AFF;
		margin-bottom: 10px;
	}
	.buju{
		display: flex;
		flex-direction:column;
		justify-content:center;
		height:39vh;
	}
	.btn{
		position:fixed;
		width:100%;
		bottom:10px;
		z-index:1024;
		height:110px;
		box-shadow:inset 0px 10px 10px #e9e9e9;
	}
	.txt{
		line-height: 35px; 
		text-align: center;
	}
	.roundBtn{
		margin:auto; 
		width:60px; 
		height:60px;
		background-color:#167bff;
		border-radius: 50%;
		box-shadow: 0px 0px 10px #0081FF;
	}
	.roundBtn p{
		text-align:center;
	}
	.roundBtn image{
		width:24px;
		height:58px;  
	}
	.roundBack{
		margin-left:58%;
		margin-top:-60px;
		width:60px; 
		height:60px;
		border-radius: 50%;
		box-shadow: 0px 0px 10px #167bff;
	}
	.roundBack p{
		text-align: center;
	}
	.roundBack image{
		width:28px;
		height:58px;
	}
	.bowen{
		width:100%;
		height:120px;
		bottom: 20px;
	}
    .tabs {
        flex: 1;
        flex-direction: column;
        overflow: hidden;
        background-color: #ffffff;
        height: 100vh;
    }

    .line-h {
        height: 1rpx;
        background-color: #e1e1e1;
    }
	
    .scroll-h {
		width:90%;
		margin-left: 5%;
		height: 80rpx;
        flex-direction: row;
        white-space: nowrap;
    }

    .uni-tab-item {
        display: inline-block;
        flex-wrap: nowrap;
        padding-left: 34rpx;
        padding-right: 34rpx;
    }

    .uni-tab-item-title {
        color: #555;
        font-size: 30rpx;
        height: 80rpx;
        line-height: 80rpx;
        flex-wrap: nowrap;
        white-space: nowrap;
    }

    .uni-tab-item-title-active {
        color: #007AFF;
    }

    .swiper-box {
		width:90%;
		margin-left: 5%;
		margin-top:15px;
		text-align: center;
        flex: 1;
        background-color: #fbfbfb;
		box-shadow: 0px 0px 15px #e9e9e9;
		/* height: 100vh; */
		height:40vh; 
    }
	.text{
        font-size: 40rpx;
		line-height: 60rpx;
		width:100%;
		display:inline;
	}
	.pron{
		margin-top: 10px;
        font-size: 35rpx;
		color: #555;
		line-height: 60rpx;
		width:100%;
		display:inline;
	}
	.img{
		height:240px;
		line-height: 0px;
	}



</style>
