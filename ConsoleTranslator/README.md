
# ConsoleTranslator

## 文件说明

`main.py`为源码，已经删除掉本人申请的API_Key，**不能直接运行**。你可以自己在百度开发者平台上申请一个。  
`main.pyc`使用我的API_Key编译的二进制码，**可以直接运行**。  
`makepyc.py`生成pyc的程序。

## 使用帮助

假设`main.pyc`位置为`~/util/main.pyc`。  

`$ python ~/util/main.pyc [-d] word`  

建议在`.bashrc`中加入`alias`。  

`alias tl='python ~/util/main.pyc'`
