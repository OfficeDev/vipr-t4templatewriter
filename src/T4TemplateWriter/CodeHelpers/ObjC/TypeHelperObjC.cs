// Copyright (c) Microsoft Open Technologies, Inc. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the source repository root for license information.﻿

using System;
using System.Collections.Generic;
using Vipr.Core.CodeModel;
using Vipr.T4TemplateWriter.Extensions;
using Vipr.T4TemplateWriter.Settings;
using Vipr.T4TemplateWriter.CodeHelpers;

namespace Vipr.T4TemplateWriter.CodeHelpers.ObjC
{
	public static class TypeHelperObjC
	{
        public static string Prefix = "";
        
        private static HashSet<string> primitiveTypes = new HashSet<string>()
        {
            "int","float","double","bool"
        };
        
        
		public static string GetTypeString(this OdcmType type)
        {
            
			if (type == null) {
				return "int";
			}
			switch (type.Name) {
			case "String":
				return "NSString";
			case "Int32":
				return "int";
			case "Int64":
				return "int";
            case "Single":
                return "float";
            case "Double":
                return "double";
			case "Guid":
				return "NSString";
			case "DateTimeOffset":
				return "NSDate";
			case "Binary":
				return "NSData";
			case "Boolean":
				return "bool";
			case "Stream":
				return "NSStream";
			default:
				return Prefix + type.Name;
			}
		}
        

        public static string GetTypeString(this OdcmProperty property, bool getRealType=false)
        {
            if (!getRealType && property.IsCollection)
				return  "NSMutableArray";
			else
                return property.Type.GetTypeString();
        }
        
        public static string GetTypeString(this OdcmParameter parameter, bool getRealType=false)
        {
            if (!getRealType && parameter.IsCollection)
				return  "NSMutableArray";
			else
                return parameter.Type.GetTypeString();
        }
        
        
        public static string GetRelatedEntityTypeString(this OdcmType type, string suffix)
        {
            string ts = type.GetTypeString();
            
            return ts + (suffix!="" && suffix!="Client" && ts.EndsWith("Collection")?"_":"")+ suffix;
        }
        
        //For allocation and passing of parameters
        public static string GetTypeReferenceString(this OdcmType type)
        {
            return type.GetTypeString() + (type.IsComplex()?" *":"");
        }
        
        public static string GetTypeReferenceString(this OdcmProperty property)
        {
            if (property.IsCollection)
				return  "NSMutableArray *";
			else
                return property.Type.GetTypeReferenceString();
        } 
        
        public static string GetSetterString(this OdcmProperty property)
        {
            return property.Name.ToObjCSetter();
        }

        public static string GetGetterString(this OdcmProperty property)
        {
            return property.Name.ToObjCGetter(property.IsBool());
        }
        
        public static string GetPropertyString(this OdcmProperty property)
        {
            return property.Name.ToObjCProperty();
        }
        
        public static bool IsComplex(this OdcmType type)
        {
            string t = GetTypeString(type);
            return !(primitiveTypes.Contains(t) || t == "Byte" || type is OdcmEnum);
        }

		public static bool IsComplex(this OdcmProperty property)
        {
            return property.Type.IsComplex();
		}
        
        public static bool IsFromOurNamespace(this OdcmType type)
        {
            return ConfigurationService.Settings.NamespacePrefix!="" && GetTypeString(type).StartsWith(ConfigurationService.Settings.NamespacePrefix);
        }
        
		public static bool IsSystem(this OdcmProperty property)
		{
            return property.Type.IsSystem();
		}

		public static bool IsSystem(this OdcmType type)
		{
			string t = GetTypeString(type);
			return (primitiveTypes.Contains(t) || t == "Byte" || t == "NSString" || t == "NSDate");
		}

		public static bool IsEnum(this OdcmProperty property)
		{
			return !property.IsCollection() && property.Type is OdcmEnum;
		}
        
        public static bool IsBool(this OdcmProperty property)
		{
			return GetTypeString(property.Type) == "bool";
		}

        public static bool IsInt(this OdcmProperty property)
		{
			return GetTypeString(property.Type) == "int";
		}
        
        public static bool IsFloat(this OdcmProperty property)
		{
			return GetTypeString(property.Type) == "float";
		}

        public static bool IsDouble(this OdcmProperty property)
		{
			return GetTypeString(property.Type) == "double";
		}
        
        public static string ToObjCInterface(this OdcmType e)
        {
            return e!=null?e.Name.ToObjCInterface():"";
        }
        
        public static string ToObjCInterface(this OdcmProperty prop)
        {
            return prop!=null?prop.Type.Name.ToObjCInterface():"";
        }
        
        public static string GetMethodString(this OdcmMethod method)
        {
            return method.Name.ToObjCMethod();  
        }
	}
}
