#!/bin/bash
ps -ef |grep "WorldLinux.exe"|grep -v "grep"|awk '{print $2}'

if ( ps aux|grep "WorldLinux.exe"|grep -v "grep" > /dev/null)
then
        echo "Server is Already Running!"
else
        mono ./WorldLinux.exe
fi
