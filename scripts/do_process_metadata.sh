
SCRIPT_PATH=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

ConfigsPath="${SCRIPT_PATH}/metadata_setups"
OutputPath="${SCRIPT_PATH}/../src/T4TemplateWriter/CSDL"
source ${ConfigsPath}/auth.settings

for f in $ConfigsPath/*.cfg
do
	MetadataName=$(basename "$f" .cfg)
	OutFilePath="${OutputPath}/${MetadataName}_metadata.xml"
	
    #processMetadata(path_to_config, metadata_name, outpath)
	processMetadata "$f" "$MetadataName" "$OutFilePath"
				
done
