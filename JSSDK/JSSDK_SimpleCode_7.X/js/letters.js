function phoneticSymbolsAndLettersMap(jsonResultObj){
	            //提取当前单词的字母序列
	            var lettersOrigin = jsonResultObj.details[0].phoneme.letters;
	            //提取当前单词的音素序列
	            var pronsOrigin = jsonResultObj.details[0].phoneme.prons;
				//单词是英式还是美式
	            var accent = jsonResultObj.details[0].accent;

				var phoneList = jsonResultObj.details[0].phone;
				
	            var PhonemePronsFinal = "";
				var prons="";
				var letters="";
	            //针对tr、dr、dz、ts的处理
	            if (pronsOrigin.search("t_r") != -1) {
	                prons = pronsOrigin.replace(/t_r/g, "t r");
	                letters = lettersOrigin.replace(/t_r/g, "tr");
	                PhonemePronsFinal = pronsOrigin.replace(/t_r/g, "tr");
	            } else if (pronsOrigin.search("d_r") != -1) {
	                prons = pronsOrigin.replace(/d_r/g, "d r");
	                letters = lettersOrigin.replace(/d_r/g, "dr");
	                PhonemePronsFinal = pronsOrigin.replace(/d_r/g, "dr");
	            } else if (pronsOrigin.search("d_z") != -1) {
	                prons = pronsOrigin.replace(/d_z/g, "d z");
	                letters = lettersOrigin.replace(/d_s/g, "ds");
	                PhonemePronsFinal = pronsOrigin.replace(/d_z/g, "dz");
	            } else if (pronsOrigin.search("t_s") != -1) {
	                prons = pronsOrigin.replace(/t_s/g, "t s");
	                letters = lettersOrigin.replace(/t_s/g, "ts");
	                PhonemePronsFinal = pronsOrigin.replace(/t_s/g, "ts");
	            } else {
	                prons = pronsOrigin;
	                letters = lettersOrigin;
	                PhonemePronsFinal = pronsOrigin;
	            }
				pronsArray = prons.split("_");
				lettersArray = letters.split("_");
				
				var scoreIndex = 0;
				var myArray = [];
				for(let i=0;i<pronsArray.length;i++){
					var obj = new Object();
						if(pronsArray[i] == "#"){
							obj.prons = "#";//音素
							obj.letters = lettersArray[i];//字母
							obj.phone = "#";//音标
							obj.score = 0;//分数
						}else if(pronsArray[i].search(" ") != -1){
							//计算空格处多个字母平均值
							if(pronsArray[i].search("t r") != -1 || pronsArray[i].search("d r") != -1 || pronsArray[i].search("d z") != -1 || pronsArray[i].search("t s") != -1){
								obj.prons = pronsArray[i].replace(/\s+/g,"");
							}else{
								obj.prons = pronsArray[i];
							}
							//alert(obj.prons);
							obj.letters = lettersArray[i].replace(/\s+/g,"");
							
							//处理分数，计算平均值
							var specialArray = pronsArray[i].split(" ");
							var sum= 0;
							var str="";
							for(let s=0;s<specialArray.length;s++){
								sum+= phoneList[scoreIndex + s].score;
								//音标处理空格再映射
								if(accent == 2){
									str += phoneticConvertUS(specialArray[s]); //K.K.(US)
								}else{
									str += phoneticConvert(specialArray[s]); //IPA88(UK)
								}
							}
							obj.phone = str;
							obj.score = Math.round(sum/specialArray.length);
							scoreIndex += specialArray.length;
						}else{
							//正常映射取值
							obj.prons = pronsArray[i];
							obj.letters = lettersArray[i].replace(/\s+/g,"");
							if(accent == 2){
								obj.phone = phoneticConvertUS(obj.prons); //K.K.(US)
							}else{
								obj.phone = phoneticConvert(obj.prons); //IPA88(UK)
							}
							obj.score=phoneList[scoreIndex].score;
							scoreIndex++;
						}
						myArray.push(obj);
				}
				
				return myArray;			
}