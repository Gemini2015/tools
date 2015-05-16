#!/usr/bin/python
# Chris Cheng
# gemini2015@hotmail.com
# 2015.3.10

import urllib
import json
import sys


class Translator:
    name = ""

    def __init__(self):
        self.name = "None"
        pass

    def translate(self, word, dic=False, src="en", dst="zh"):
        pass

    def __get(self, word, dic, src, dst):
        pass

    def __parse(self, result, dic):
        pass


class BaiduTrans(Translator):
    API_TRANS_URL = "http://openapi.baidu.com/public/2.0/bmt/translate?%s"
    API_DICT_URL = "http://openapi.baidu.com/public/2.0/translate/dict/simple?%s"
    API_KEY = ""

    def __init__(self):
        Translator.__init__(self)
        self.name = "Baidu API"

    def translate(self, word, dic=False, src="en", dst="zh"):
        pre_result = ""
        if len(self.API_KEY) == 0:
            pre_result = "Warning: No API Key for " + self.name + "\n\n"

        result = self.__get(word, dic, src, dst)
        result = self.__parse(result, dic)
        return pre_result + result

    def __get(self, word, dic, src, dst):
        if dic:
            api_url = self.API_DICT_URL
        else:
            api_url = self.API_TRANS_URL
        data = {
            "client_id": self.API_KEY,
            "from": src,
            "to": dst,
            "q": word,
        }
        param = urllib.urlencode(data)
        result = urllib.urlopen(api_url % param).read()
        return result

    def __parse(self, result, dic):
        if dic:
            return self.__parsedict(result)
        else:
            return self.__parsetrans(result)

    def __parsedict(self, result):
        obj = json.JSONDecoder().decode(result)
        if "errno" not in obj:
            return "Error: No Result From " + self.name + "\n"

        code = int(obj["errno"])
        if code != 0:
            return "Error: Invalid Result From " + self.name + "\n"

        if len(obj["data"]) == 0:
            return "Error: Word Not Found \n"

        data = obj["data"]
        wordname = data["word_name"]
        ret = wordname + "\n"
        for symbol in data["symbols"]:
            # due to encoding problem, not print phonetic symbol
            if sys.platform != "win32":
                ret += "["
                if "ph_en" in symbol:
                    ret += symbol["ph_en"] + ", "
                if "ph_am" in symbol:
                    ret += symbol["ph_am"] + ", "
                if "ph_zh" in symbol:
                    ret += symbol["ph_zh"] + ", "
                ret += "] \n"

            parts = symbol["parts"]
            for part in parts:
                ret += part["part"] + "\t" + "; ".join(part["means"]) + "\n"
            ret += "\n"
        return ret

    def __parsetrans(self, result):
        obj = json.JSONDecoder().decode(result)
        if "error_code" in obj:
            ret = "Error: Invalid Result From " + self.name + "\n"
            ret += "Error Code: " + obj["error_code"] + "\n"
            ret += "Error:" + obj["error_msg"] + "\n"
            return ret
        ret = "From: " + obj["from"] + " to: " + obj["to"] + "\n"
        for trans in obj["trans_result"]:
            ret += trans["src"] + "\t" + trans["dst"] + "\n"
        return ret


class Shell:
    __translator = None

    def __init__(self):
        self.__translator = None
        pass

    def __showusage(self, cmd):
        print "Useage: ~$ %s [-d] [-f srclang] [-t destlang] word" % cmd
        print "Console translator based on Baidu Translate & Dict."

    def parse(self, argv):
        if len(argv) <= 1:
            return self.__showusage(argv[0])

        if len(argv) == 2:
            word = argv[1]
            self.__translator = BaiduTrans()
            result = self.__translator.translate(word)
            print result
            return

        # select translator
        self.__translator = BaiduTrans()
        if "-e" in argv:
            index = argv.index("-e")
            op = argv[index + 1]
            if op == "b":
                self.__translator = BaiduTrans()
            elif op == "y":
                self.__translator = BaiduTrans()

        # translate or dictionary
        dic = False
        if "-d" in argv:
            dic = True

        # src lang
        src = "en"
        if "-f" in argv:
            index = argv.index("-f")
            if index + 1 < len(argv):
                src = argv[index + 1]

        dst = "zh"
        if "-t" in argv:
            index = argv.index("-t")
            if index + 1 < len(argv):
                dst = argv[index + 1]

        word = argv[len(argv) - 1]
        result = self.__translator.translate(word=word, dic=dic, src=src, dst=dst)
        print( "\n" + result)


def main(argv):
    shell = Shell()
    shell.parse(argv)

if __name__ == "__main__":
    main(sys.argv)
