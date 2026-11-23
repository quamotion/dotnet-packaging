#!/bin/bash
set -e

usage()
{
    echo "Usage: $0 -r [rid]"
}

while getopts "h?r:" opt; do
    case "$opt" in
    h|\?)
        usage
        exit 0
        ;;
    r)  rid=$OPTARG
        ;;
    esac
done

echo Packaging for $rid
format=x

if  [[ $rid == rhel* ]] || [[ $rid == centos* ]] || [[ $rid == ol* ]] || [[ $rid == fedora* ]] || [[ $rid == opensuse* ]] || [[ $rid == sles* ]] ;
then
    echo Packaging as RPM
    format=rpm
fi

if  [[ $rid == debian* ]] || [[ $rid == ubuntu* ]] || [[ $rid == linuxmint* ]] ;
then
    echo Packaging as deb
    format=deb
fi

if  [[ $rid == alpine* ]] ;
then
    echo Alpine Linux is not supported
    exit 0
fi

dotnet $format -c Release -r linux-x64 -f net10.0 --self-contained
cp bin/Release/net10.0/linux-x64/cliscd.1.0.0.linux-x64.$format $rid
docker build -t clisc:$rid $rid
docker run clisc:$rid

if [[ $format == deb ]]
then
    docker run clisc:$rid /usr/bin/apt-get remove -y cliscd
fi

if [[ $format == rpm ]]
then
    if [[ $rid == opensuse* ]]
    then
        docker run clisc:$rid /usr/bin/zypper -n rm cliscd
    else
        docker run clisc:$rid /usr/bin/rpm -e cliscd
    fi
fi