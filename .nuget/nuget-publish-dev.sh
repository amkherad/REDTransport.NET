#!/bin/bash

#========================================================
#creates a new version for the framework.

year=`date +"%y"`
month=`date +"%m"`
day=`date +"%d"`

version=`cut -d ',' -f2 .version`
if [ "$version" == "" ]; then
    version=0
fi
newVersion=`expr $version + 1`
sed -i "s/$version\$/$newVersion/g" .version

version="1.$year.$month$day.$newVersion"

#========================================================



#========================================================
#runs the nuget-publish to compile and publish packages.

./nuget-publish.sh . "" "$version"
