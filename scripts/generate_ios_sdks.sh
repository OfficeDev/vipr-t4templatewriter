#!/bin/sh

# deps:
#
# ruby
# [sudo] gem install xcodeproj
# mono (brew version seems to miss PCB, use the pkg from the official site)
#


SDK_VERSION="0.10.0"


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
platform :ios, '7.0'
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
	MetadataName="$2"
	OutFilePath="$3"
	
	NamespaceOverride=""
	PrimaryNamespaceName=""
	
	source "${ConfigPath}"
	
	cd "$DebugPath"	
	
	JsonPath=".config/TemplateWriterSettings.json"

    echo "{" > "${JsonPath}"
    echo "\"AvailableLanguages\": [ \"ObjC\" ]," >> "${JsonPath}"
    echo "\"TargetLanguage\": \"ObjC\"," >> "${JsonPath}"
    echo "\"Plugins\": [ ]," >> "${JsonPath}"
	echo "\"PrimaryNamespaceName\" : \"${PrimaryNamespaceName}\"," >> "${JsonPath}"
    echo "\"NamespacePrefix\": \"${NamespacePrefix}\"," >> "${JsonPath}"
    echo "\"InitializeCollections\": \"false\"," >> "${JsonPath}"
    echo "\"AllowShortActions\": \"false\"," >> "${JsonPath}"
    echo "\"NamespaceOverride\": \"${NamespaceOverride}\"" >> "${JsonPath}"
    echo "}" >> "${JsonPath}"

	rm -rf "$SDK_TMP_OUT"
	
	ouptutPodfileDefinitionForProject "${MetadataName}" >> ${SDK_OUT}/Podfile
	
	mono Vipr.exe "${OutFilePath}" --writer="Vipr.T4TemplateWriter" --outputPath="${SDK_TMP_OUT}" 
	
	mv -f "$SDK_TMP_OUT" "${SDK_OUT}/${MetadataName}"

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
# Generate podspec
#
########################################################################

cd "${SCRIPT_PATH}"

cat > "${SDK_BASE}/Office365.podspec" <<HEREDOC


Pod::Spec.new do |s|
  s.name         = "Office365"
  s.version      = "${SDK_VERSION}"
  s.summary      = "Client libraries for calling Office 365 service APIs from iOS apps."
  s.description  = <<-DESC
		   Client libraries for calling Office 365 service APIs from iOS apps.
		   These libraries are in preview.
                   DESC
  s.homepage     = "http://github.com/OfficeDev/Office-365-SDK-for-iOS"
  s.license      = "Apache License, Version 2.0"
  s.author             = { "joshgav" => "josh.gavant@microsoft.com" }
  s.social_media_url   = "http://twitter.com/OpenAtMicrosoft"

  s.platform     = :ios
  s.ios.deployment_target = "7.0"
  s.source       = { :git => "https://github.com/OfficeDev/Office-365-SDK-for-iOS.git",
		             :tag => "v#{s.version}"
		           }
  s.exclude_files = "**/Build/**/*"
  s.requires_arc = true
  s.dependency "orc"

  # --- Subspecs ------------------------------------------------------------------#

HEREDOC

function processMetadata
{
	
	ConfigPath="$1"
	MetadataName="$2"
	OutFilePath="$3"
	
	cat >> "${SDK_BASE}/Office365.podspec" <<HEREDOC

  s.subspec "${MetadataName}" do |subspec|
    subspec.source_files = "sdk/${MetadataName}/**/*.{h,m}"
    subspec.public_header_files = "sdk/${MetadataName}/**/*.h"
    subspec.header_dir = "${MetadataName}"
  end

HEREDOC



}

source do_process_metadata.sh

echo "end" >> "${SDK_BASE}/Office365.podspec"
