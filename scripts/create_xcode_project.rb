#create_xcode_project.rb name path_to_project files_list
require 'xcodeproj'

proj = Xcodeproj::Project.new(ARGV[1])

files=ARGV[2].split ";"


target = proj.new_target(:static_library,ARGV[0],:ios,'7.0')


files.each do|f|
  subgroup=proj.main_group.find_subpath(File.dirname(f)[2..-1],true)
  newf=subgroup.new_reference(f)
  target.add_file_references([newf])
end

proj.save

