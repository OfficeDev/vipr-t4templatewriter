#!/bin/sh

SCRIPT_PATH=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

function processMetadata
{	
	ConfigPath="$1"
	MetadataName="$2"
	OutFilePath="$3"
	
	echo "Processing ${MetadataName}..."
	
	Auth=''
	
	source "$ConfigPath"
	
	case "${Auth}" in
		office365 )
			curl "$MetadataLink" --anyauth -u "${Office365User}:${Office365Pass}" -o "$OutFilePath"
			;;
		copy )
			cp "${SCRIPT_PATH}/metadata_setups/${MetadataLink}" "$OutFilePath" 
			;;
		* )
			curl "$MetadataLink" -o "$OutFilePath" 
			;;
	esac
	
}

source do_process_metadata.sh


