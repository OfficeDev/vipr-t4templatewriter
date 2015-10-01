#!/bin/sh

# deps:
#
# mono (brew version seems to miss PCB, use the pkg from the official site)
#


SCRIPT_PATH=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
SDK_BASE="${SCRIPT_PATH}/android_sdks_out"
SDK_OUT="${SDK_BASE}/sdk"
SDK_TMP_OUT="${SDK_BASE}/tmp"
DebugPath="${SCRIPT_PATH}/../src/T4TemplateWriter/bin/debug"	

rm -rf "${SDK_OUT}"
mkdir -p "${SDK_OUT}"


#
# Build Vipr and the template writer
#
########################################################################

./mono_build.sh

#
# Download metadata
#
########################################################################

./download_metadata.sh

#
# Proccess Templates
#
########################################################################

function processMetadata
{
	
	ConfigPath="$1"
	OutFilePath="$3"
	
	NamespaceOverride=""
	PrimaryNamespaceName=""
	EntityContainerName=""
	DeleteAndMoveTo=""
    
	source "${ConfigPath}"
	
	MetadataName="$EntityContainerName"
	
	cd "$DebugPath"	
	
	JsonPath=".config/TemplateWriterSettings.json"

    echo "{" > "${JsonPath}"
    echo "\"AvailableLanguages\": [ \"Java\" ]," >> "${JsonPath}"
    echo "\"TargetLanguage\": \"Java\"," >> "${JsonPath}"
    echo "\"Plugins\": [ ]," >> "${JsonPath}"
	echo "\"PrimaryNamespaceName\" : \"${PrimaryNamespaceName}\"," >> "${JsonPath}"
    echo "\"NamespacePrefix\": \"${NamespacePrefix}\"," >> "${JsonPath}"
    echo "\"InitializeCollections\": \"false\"," >> "${JsonPath}"
    echo "\"AllowShortActions\": \"true\"," >> "${JsonPath}"
    echo "\"NamespaceOverride\": \"${NamespaceOverride}\"" >> "${JsonPath}"
    echo "}" >> "${JsonPath}"

	rm -rf "$SDK_TMP_OUT"
	
	mono Vipr.exe "${OutFilePath}" --writer="Vipr.T4TemplateWriter" --outputPath="${SDK_TMP_OUT}" 
	
    DestFolderName="${MetadataName}"
    
    if [ ! -z "$DeleteAndMoveTo" ]
    then
        DestFolderName="${DeleteAndMoveTo}"
    fi
    
    for F in $(find "$SDK_TMP_OUT" -type f); 
    do
        InsidePath=${F#$SDK_TMP_OUT}
        FileOutPath="${SDK_OUT}/${DestFolderName}${InsidePath}"
        
        mkdir -p $(dirname "$FileOutPath")
        
        if [ -e "$FileOutPath" ]
        then
            touch "${SDK_TMP_OUT}/__empty__"
            
            git merge-file -p --union "$F" "${SDK_TMP_OUT}/__empty__" "$FileOutPath" > "${SDK_TMP_OUT}/__cur_merge__"
            
            rm "$FileOutPath"
            mv "${SDK_TMP_OUT}/__cur_merge__" "$FileOutPath"
 
            
            rm "${SDK_TMP_OUT}/__empty__"
        else
            mv "$F" "$FileOutPath"
        fi
    
    done
    
    rm -rf "$SDK_TMP_OUT"
	
}

source do_process_metadata.sh
