#!/usr/bin/python

# Chris Cheng
# gemini2015@hotmail.com
# 2015.3.10

import urllib
import json
import sys

trans_url = "http://openapi.baidu.com/public/2.0/bmt/translate?%s"
dict_url = "http://openapi.baidu.com/public/2.0/translate/dict/simple?%s"
API_Key = ""

def BaiduTrans(word):
    data = {
        "client_id": API_Key,
        "from": "en",
        "to": "zh",
        "q": word,
    }
    param = urllib.urlencode(data)
    result = urllib.urlopen(trans_url % param).read()
    return ParseTransResult(result)


def ParseTransResult(result):
    jsonobj = json.JSONDecoder().decode(result)
    if jsonobj.has_key("error_code"):
        print "Error:" + jsonobj["error_msg"]
        return
    tresult = jsonobj["trans_result"][0]
    print tresult["src"] + "\t" + tresult["dst"]


def BaiduDict(word):
    data = {
        "client_id": API_Key,
        "from": "en",
        "to": "zh",
        "q": word,
    }
    param = urllib.urlencode(data)
    result = urllib.urlopen(dict_url % param).read()
    return ParseDictResult(result)


def ParseDictResult(result):
    jsonobj = json.JSONDecoder().decode(result)
    code = int(jsonobj["errno"])
    if code != 0:
        print "Error"
        return 
    if len(jsonobj["data"]) == 0:
        print "Word Not Found"
        return
    data = jsonobj["data"]
    word = data["word_name"]
    symbol = data["symbols"][0]
    ph_en = symbol["ph_en"]
    ph_am = symbol["ph_am"]
    parts = symbol["parts"]
    # due to encoding problem, not print phonetic symbol
    if sys.platform == "win32":
        print word
    else:
        print word + "\t[" + ph_en + ", " + ph_am + "]"
    for part in parts:
        print part["part"] + "\t" + "; ".join(part["means"])

    
def ShowUsage(cmd):
    print "Useage: ~$ %s [-d] word" % cmd
    print "Console translator based on Baidu Translate & Dict."


def main(argv):
    if len(sys.argv) <= 1:
        return ShowUsage(sys.argv[0])
    if len(API_Key) == 0:
        print "API_Key is empty."
        return

    if len(sys.argv) == 2:
        return BaiduTrans(sys.argv[1])

    # dict
    for param in sys.argv[0]:
        if cmp("-d", param):
            return BaiduDict(sys.argv[2])


if __name__ == "__main__":
    main(sys.argv)
