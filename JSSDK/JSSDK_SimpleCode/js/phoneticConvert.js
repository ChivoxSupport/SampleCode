//IPA88(UK)
const phoneticMapUK = { 
  "ih": "ɪ",
  "ax": "ə",
  "oh": "ɒ",
  "uh": "ʊ",
  "ah": "ʌ",
  "eh": "e",
  "ae": "æ",
  "iy": "i:",
  "er": "ɜ:",
  "ao": "ɔ:",  
  "uw": "u:",
  "aa": "ɑ:",
  "ey": "eɪ",
  "ay": "aɪ",
  "oy": "ɔɪ",
  "aw": "aʊ",
  "ow": "әʊ",
  "ia": "ɪə",
  "ea": "ɛә",
  "ua": "ʊə",
  "tr": "tr",    //纠错内核会直接输出这个音素
  "dr": "dr",    //纠错内核会直接输出这个音素
  "dz": "dz",    //纠错内核会直接输出这个音素
  "ts": "ts",    //纠错内核会直接输出这个音素
  "p": "p",
  "k": "k",
  "m": "m",
  "s": "s",
  "f": "f",
  "sh": "ʃ", 
  "b": "b",
  "g": "g",
  "n": "n",
  "z": "z",
  "v": "v",
  "zh": "ʒ",  
  "t": "t",
  "l": "l",
  "ng": "ŋ", 
  "th": "θ",
  "w": "w",
  "ch": "tʃ",  
  "d": "d",
  "r": "r",
  "hh": "h",
  "dh": "ð",
  "y": "j",
  "jh": "dʒ"
};

//K.K.(US)
const phoneticMapUS = {
  "ih": "ɪ",
  "ax": "ə",
  "oh": "ɔ",
  "uh": "ʊ",
  "ah": "ʌ",
  "eh": "ɛ",
  "ae": "æ",
  "iy": "i",
  "er": "ɜ:",
  "ao": "ɔ",
  "uw": "u",
  "aa": "ɑ",
  "ey": "e",
  "ay": "aɪ",
  "oy": "ɔɪ",
  "aw": "aʊ",
  "ow": "oʊ",
  "ia": "ɪə",
  "p": "p",
  "k": "k",
  "m": "m",
  "s": "s",
  "f": "f",
  "sh": "ʃ",
  "ts": "ts",     //改成ts,纠错内核会直接输出这个音素
  "b": "b",
  "g": "g",
  "n": "n",
  "z": "z",
  "v": "v",
  "zh": "ʒ",
  "dz": "dz",      //改成dz,纠错内核会直接输出这个音素
  "t": "t",
  "l": "l",
  "ng": "ŋ",
  "th": "θ",
  "w": "w",
  "ch": "tʃ",
  "tr": "tr",      //改成tr,纠错内核会直接输出这个音素
  "d": "d",
  "r": "r",
  "hh": "h",
  "dh": "ð",
  "y": "j",
  "jh": "dʒ",
  "dr": "dr"       //改成dr,纠错内核会直接输出这个音素
};

function phoneticConvert(character){
  for (var key in phoneticMapUK) {
    if (phoneticMapUK.hasOwnProperty(character)) {
      console.log("phoneticConvert: ", character, "->", phoneticMapUK[character]);
      return phoneticMapUK[character];
    }
  }
  return character;
}

function phoneticConvertUS(character){
  for (var key in phoneticMapUS) {
    if (phoneticMapUS.hasOwnProperty(character)) {
      console.log("phoneticConvertUS: ", character, "->", phoneticMapUS[character]);
      return phoneticMapUS[character];
    }
  }
  return character;
}
