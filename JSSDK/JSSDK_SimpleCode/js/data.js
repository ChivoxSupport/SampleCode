/*Menu*/
var tabBars = [{
    'name': 'Word',
    'id': 'word'
}, {
    'name': 'Sentence',
    'id': 'sent'
}, {
    'name': 'Paragraph',
    'id': 'pred'
}, {
    'name': 'Multiple Choice',
    'id': 'choc'
}, {
    'name': 'Situational Dialogue',
    'id': 'scne'
}, {
    'name': 'Speaking with pictures',
    'id': 'prtl'
}, {
    'name': 'Asr',
    'id': 'asrRec'
}]
/*Menu style*/
function setCurrentSlide(ele, index) {
    $(".swiper1 .swiper-slide").removeClass("selected");
    ele.addClass("selected");
}


/*Show or hide answer*/
var ifShow = false;
function showAnswer(index){
    ifShow?(ifShow=false, index==1?($("#dialogue").css("display","none")):($("#picture").css("display","none"))) : (ifShow=true, index==1?($("#dialogue").css("display","flex")):($("#picture").css("display","flex")));
}



var swiperHTML = "";
$.each(tabBars,function(index,data){
    swiperHTML+='<div class="swiper-slide swiper-no-swiping">'+data.name+"</div>";
});
$("#tabBars").html(swiperHTML);


var currentPage = 1;
/*初始化菜单*/
var swiper1 = new Swiper('.swiper1', {
    onInit: function(swiper){
        var n = swiper.activeIndex;
        setCurrentSlide($(".swiper1 .swiper-slide").eq(n), n);
    },
                    slidesPerView: 8,//slider容器能够同时显示的slides数量
                    paginationClickable: true,//true点击分页器的指示点分页器会控制Swiper切换。
                    spaceBetween: 0,//slide之间的距离（单位px）
                });
swiper1.slides.each(function(index, val) {
    var ele = $(this);
    ele.on("click", function() {
        setCurrentSlide(ele, index);
        swiper2.slideTo(index, 500, false);
        currentPage = index+1;
        ifShow = false;
        $("#dialogue").css("display","none");
        $("#picture").css("display","none");
        if(recording){
            sdk.stopRecord();
            recording = false;
            $("#initBtn").css("display","inherit");
            $("#luyinBtn").css("display","none");
        }
        $("#tableData").html(""); 
        audioUrl = "";
    });
});
var swiper2 = new Swiper('.swiper2', { 
            direction: 'horizontal',//Slides的滑动方向：水平(horizontal)或垂直(vertical)。
            loop: false,
            initialSlide: 0,
            autoHeight: true
        });







/*Content*/
var contentList = [{
    trans: 'n. 苹果',
    text: "apple",
    pron:"UK ['æp(ə)l]　US [ˈæpl]",
    duration: 4000,
    serverParams: {
        coreType:'en.word.pron',
        refText:"apple",
        rank: 100,
        attachAudioUrl: 1,
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
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
        userId: "chivoxJssdk"
    }
}];