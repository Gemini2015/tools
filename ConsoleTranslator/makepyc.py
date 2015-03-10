#!/usr/bin/python

import py_compile
import sys

if __name__ == "__main__":
    if len(sys.argv) <= 1:
        print "Usage: ~$ " + sys.argv[0] + " python_file"
        sys.exit()
    py_compile.compile(sys.argv[1])
    print "Done"

