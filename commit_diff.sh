#!/bin/bash

if [ 2 -eq $# ]; then
	branch_a=$1
	branch_b=$2
elif [ 0 -eq $# ]; then
	branch_a=origin/master
	branch_b=origin/release
else
	echo arguments [branch_a] [branch_b]
	exit
fi

if [ 0 -eq `git branch -a | grep -wc $branch_a` ]; then
	echo branch $branch_a is not exist
	exit
fi
if [ 0 -eq `git branch -a | grep -wc $branch_b` ]; then
	echo branch $branch_b is not exist
	exit
fi

temp=mktemp
echo $branch_a >> $temp
git show-branch --sha1-name $branch_a $branch_b | grep -e '^\s*+\s*' | sort -k3 | uniq -c -f3 | grep -e '^[^[]\+1 +' | sed -e 's/^[^[]\+//g' >> $temp
echo $branch_b >> $temp
git show-branch --sha1-name $branch_b $branch_a | grep -e '^\s*+\s*' | sort -k3 | uniq -c -f3 | grep -e '^[^[]\+1 +' | sed -e 's/^[^[]\+//g' >> $temp

cat $temp
rm $temp

