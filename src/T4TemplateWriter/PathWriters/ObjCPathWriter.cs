// Copyright (c) Microsoft Open Technologies, Inc. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the source repository root for license information.﻿

using System;
using System.IO;
using Vipr.T4TemplateWriter.Settings;
using Vipr.T4TemplateWriter.TemplateProcessor;
using Vipr.Core.CodeModel;

namespace Vipr.T4TemplateWriter.Output
{
    class ObjCPathWriter : PathWriterBase
    {
        public override string WritePath(TemplateFileInfo template, String entityTypeName)
        {
            return WritePath(template.TemplateName,template.FileExtension,template.TemplateType,entityTypeName);
        }
        
        public string WritePath(String templateName, String fileExtension, TemplateType templateType, String entityTypeName)
        {
            String prefix = ConfigurationService.Settings.NamespacePrefix;
            String coreFileName = this.TransformFileName(templateName, fileExtension, templateType, entityTypeName);
            
            String containerName = entityTypeName == this.Model.EntityContainer.Name ? String.Empty : this.Model.EntityContainer.Name;

            return Path.Combine(
                templateType.ToString(), 
                String.Format("{0}{1}{2}",
                    prefix,
                    containerName,
                    coreFileName
                )
            );  
        }
        
        protected override String TransformFileName(TemplateFileInfo template, String entityTypeName)
        {
            return TransformFileName(template.TemplateName,template.FileExtension,template.TemplateType,entityTypeName);
        }
        
        protected String TransformFileName(String templateName, String fileExtension, TemplateType templateType, String entityTypeName)
        {
            string result;

            if (templateName.Contains("Entity") && (templateType == TemplateType.Fetchers)) {
                if(entityTypeName.EndsWith("Collection")) entityTypeName = entityTypeName + "_";
                result = templateName.Replace("Entity", entityTypeName);
            } else {
                result = String.Format("{0}.{1}", entityTypeName, fileExtension);
            }

            return result;  
        }
        
    }
}
