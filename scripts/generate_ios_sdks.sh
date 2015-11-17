#!/bin/bash

# deps:
#
# perl
# ruby
# xcodeproj ([sudo] gem install xcodeproj)
# mono (brew version seems to miss PCL, use the pkg from the official site)
#


SDK_VERSION="0.12.0"


SCRIPT_PATH=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
SDK_BASE="${SCRIPT_PATH}/ios_sdks_out"
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
# Proccess Templates and create projects for each one
#
########################################################################

cat > ${SDK_OUT}/Podfile  <<HEREDOC

source 'https://github.com/Cocoapods/Specs.git'

# target :default
platform :ios, '8.0'
workspace 'all_services'

HEREDOC

function ouptutPodfileDefinitionForProject
{
	TargetName="$1"
	
	if [ -z ${__FIRST_PROJECT_IN_PODFILE+x} ]
	then 
		echo "xcodeproj '${TargetName}/${TargetName}'"
		__FIRST_PROJECT_IN_PODFILE=1
	fi
	
	echo "target :${TargetName} do"
	echo "xcodeproj '${TargetName}/${TargetName}'"
	echo "pod 'orc', '=0.11.0'"
	echo "end"
}


function processMetadata
{
	
	ConfigPath="$1"
	OutFilePath="$3"
	
	NamespaceOverride=""
	PrimaryNamespaceName=""
	EntityContainerName=""
	DeleteAndMoveTo=""
    AllowShortActions="true"
    
	source "${ConfigPath}"
	
	MetadataName="$EntityContainerName"
	
	cd "$DebugPath"	
	
	JsonPath=".config/TemplateWriterSettings.json"

    echo "{" > "${JsonPath}"
    echo "\"AvailableLanguages\": [ \"ObjC\" ]," >> "${JsonPath}"
    echo "\"TargetLanguage\": \"ObjC\"," >> "${JsonPath}"
    echo "\"Plugins\": [ ]," >> "${JsonPath}"
	echo "\"PrimaryNamespaceName\" : \"${PrimaryNamespaceName}\"," >> "${JsonPath}"
    echo "\"NamespacePrefix\": \"${NamespacePrefix}\"," >> "${JsonPath}"
    echo "\"InitializeCollections\": \"false\"," >> "${JsonPath}"
    echo "\"AllowShortActions\": \"${AllowShortActions}\"," >> "${JsonPath}"
    echo "\"NamespaceOverride\": \"${NamespaceOverride}\"" >> "${JsonPath}"
    echo "}" >> "${JsonPath}"

	rm -rf "$SDK_TMP_OUT"
	
	ouptutPodfileDefinitionForProject "${MetadataName}" >> ${SDK_OUT}/Podfile
	
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
    
    rm -rf "${SDK_TMP_OUT}"
    
    MetadataNamePlusNS="MS${MetadataName}"
    
    HeaderGuard=$(echo "${MetadataNamePlusNS}" | tr '[:lower:]' '[:upper:]')_H
    
    cat > "${SDK_OUT}/${DestFolderName}/${MetadataNamePlusNS}.h" <<HEREDOC


/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#ifndef ${HeaderGuard}
#define ${HeaderGuard}

#import "${MetadataNamePlusNS}Fetchers.h"
#import "${MetadataNamePlusNS}Models.h"

#endif
    
HEREDOC
    

    
	cd "${SDK_OUT}/${MetadataName}"
	ruby "${SCRIPT_PATH}/create_xcode_project.rb" "${MetadataName}" "${SDK_OUT}/${MetadataName}/${MetadataName}.xcodeproj" "`find . -regex '.*\.[hm]' -print0 | tr "\0" ";"`"
		
	
}

source do_process_metadata.sh

#
# Generate workspace
#
########################################################################
cd "${SDK_OUT}"

ruby -e "$(cat <<HEREDOC
require 'xcodeproj'

projects=ARGV[1].split ";"

projRefs=[]

projects.each do|p|
	projRefs.push(Xcodeproj::Workspace::FileReference.new(p[2..-1]))
end


workspace=Xcodeproj::Workspace.new(projRefs)

workspace.save_as(ARGV[0])


HEREDOC)" "${SDK_OUT}/all_services.xcworkspace" "`find . -name '*.xcodeproj' -print0 | tr "\0" ";"`"


#
# Search and Replaces
#
########################################################################

find "${SDK_OUT}" -name '*.h' -type f -exec sed -i '' 's/^@interface MSOneNoteApiNotesCollectionFetcher : MSOrcCollectionFetcher/@interface MSOneNoteApiNotesCollectionFetcher : MSOrcMultipartCollectionFetcher/g' {} \;

find "${SDK_OUT}" -name '*.h' -type f -exec sed -i '' 's/^@interface MSOneNoteApiPageCollectionFetcher : MSOrcCollectionFetcher/@interface MSOneNoteApiPageCollectionFetcher : MSOrcMultipartCollectionFetcher/g' {} \;


#
# Generate podspec
#
########################################################################
function processMetadata
{
	
	ConfigPath="$1"
	OutFilePath="$3"

    DeleteAndMoveTo=""
	EntityContainerName=""
    CocoaPodsName=""
    GithubRepoIOS=""
	
	source "${ConfigPath}"
	
    if [ -z "$DeleteAndMoveTo" ]
    then

	MetadataName="$EntityContainerName"	
    MetadataNamePlusNS="MS${MetadataName}"

    cd "${SDK_OUT}/${MetadataName}/"

    cat > "${CocoaPodsName}.podspec" <<HEREDOC


Pod::Spec.new do |s|
    s.name         = "${CocoaPodsName}"
    s.version      = "${SDK_VERSION}"
    s.summary      = "SUMMARY"
    s.description  = "DESCRIPTION"
    s.homepage     = "${GithubRepoIOS}"
    s.license      = "Apache License, Version 2.0"
    s.author             = { "v-migpe" => "v-migpe@microsoft.com" }
    s.social_media_url   = "http://twitter.com/OpenAtMicrosoft"

    s.platform     = :ios
    s.ios.deployment_target = "8.0"
    s.source       = { :git => "${GithubRepoIOS}.git",
                     :tag => "v#{s.version}"
                   }
    s.exclude_files = "**/Build/**/*"
    s.requires_arc = true
    s.source_files = "Fetchers/*.{h,m}","Model/*.{h,m}","${MetadataNamePlusNS}.h"

    s.dependency "orc"
    s.dependency 'ADALiOS', '=1.2.4'
    s.dependency 'LiveSDK', '=5.6.1'

end

HEREDOC
        
    fi

}

cd "${SCRIPT_PATH}"

source do_process_metadata.sh

echo "end" >> "${SDK_BASE}/Office365.podspec"
