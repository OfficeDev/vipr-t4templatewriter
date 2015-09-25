#!/bin/sh


function processMetadata
{
	
	MetadataPath="$1"
	BaseName="$2"
	OutFilePath="$3"
	
	CMakeLists="${MetadataPath}CMakeLists.txt"
	
	echo 'cmake_minimum_required (VERSION 2.8.11)' > $CMakeLists
	echo "project (${BaseName}_api)" >> $CMakeLists
	
}

source do_process_metadata.sh

