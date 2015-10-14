// Copyright (c) Microsoft Open Technologies, Inc. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the source repository root for license information.﻿

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vipr.T4TemplateWriter.Extensions;
using Vipr.T4TemplateWriter.Settings;

namespace Vipr.T4TemplateWriter.Extensions
{
    public static class ObjCStringExtensions
    {
        public static string EntityContainerName="";
        private static string InvalidIdentifierEscapePrefix="_";
        private static HashSet<string> ObjCReservedOrProblematicKeywords=null;
        
        public static string ToObjCIdentifier(this string input)
        {
            if(ObjCReservedOrProblematicKeywords==null)
            {
                ObjCReservedOrProblematicKeywords=new HashSet<string>();
                ObjCReservedOrProblematicKeywords.Add("id");
                ObjCReservedOrProblematicKeywords.Add("YES");
                ObjCReservedOrProblematicKeywords.Add("NO");
                ObjCReservedOrProblematicKeywords.Add("true");
                ObjCReservedOrProblematicKeywords.Add("false");
                ObjCReservedOrProblematicKeywords.Add("NULL");
                ObjCReservedOrProblematicKeywords.Add("nil");
                ObjCReservedOrProblematicKeywords.Add("self");
                ObjCReservedOrProblematicKeywords.Add("description");
                ObjCReservedOrProblematicKeywords.Add("auto");
                ObjCReservedOrProblematicKeywords.Add("else");
                ObjCReservedOrProblematicKeywords.Add("long");
                ObjCReservedOrProblematicKeywords.Add("switch");
                ObjCReservedOrProblematicKeywords.Add("break");
                ObjCReservedOrProblematicKeywords.Add("enum");
                ObjCReservedOrProblematicKeywords.Add("register");
                ObjCReservedOrProblematicKeywords.Add("typedef");
                ObjCReservedOrProblematicKeywords.Add("case");
                ObjCReservedOrProblematicKeywords.Add("extern");
                ObjCReservedOrProblematicKeywords.Add("return");
                ObjCReservedOrProblematicKeywords.Add("union");
                ObjCReservedOrProblematicKeywords.Add("char");
                ObjCReservedOrProblematicKeywords.Add("float");
                ObjCReservedOrProblematicKeywords.Add("short");
                ObjCReservedOrProblematicKeywords.Add("unsigned");
                ObjCReservedOrProblematicKeywords.Add("const");
                ObjCReservedOrProblematicKeywords.Add("for");
                ObjCReservedOrProblematicKeywords.Add("signed");
                ObjCReservedOrProblematicKeywords.Add("void");
                ObjCReservedOrProblematicKeywords.Add("continue");
                ObjCReservedOrProblematicKeywords.Add("goto");
                ObjCReservedOrProblematicKeywords.Add("sizeof");
                ObjCReservedOrProblematicKeywords.Add("volatile");
                ObjCReservedOrProblematicKeywords.Add("default");
                ObjCReservedOrProblematicKeywords.Add("if");
                ObjCReservedOrProblematicKeywords.Add("static");
                ObjCReservedOrProblematicKeywords.Add("while");
                ObjCReservedOrProblematicKeywords.Add("do");
                ObjCReservedOrProblematicKeywords.Add("int");
                ObjCReservedOrProblematicKeywords.Add("struct");
                ObjCReservedOrProblematicKeywords.Add("_Packed");
                ObjCReservedOrProblematicKeywords.Add("double");
                ObjCReservedOrProblematicKeywords.Add("protocol");
                ObjCReservedOrProblematicKeywords.Add("interface");
                ObjCReservedOrProblematicKeywords.Add("implementation");
                ObjCReservedOrProblematicKeywords.Add("NSObject");
                ObjCReservedOrProblematicKeywords.Add("NSInteger");
                ObjCReservedOrProblematicKeywords.Add("NSNumber");
                ObjCReservedOrProblematicKeywords.Add("CGFloat");
                ObjCReservedOrProblematicKeywords.Add("property");
                ObjCReservedOrProblematicKeywords.Add("nonatomic");
                ObjCReservedOrProblematicKeywords.Add("retain");
                ObjCReservedOrProblematicKeywords.Add("weak");
                ObjCReservedOrProblematicKeywords.Add("unsafe_unretained");
                ObjCReservedOrProblematicKeywords.Add("readwrite");
                ObjCReservedOrProblematicKeywords.Add("readonly");
                ObjCReservedOrProblematicKeywords.Add("inline");
                ObjCReservedOrProblematicKeywords.Add("operations");
            }
            
            input=input.Trim();
       
            if(ObjCReservedOrProblematicKeywords.Contains(input) ||
              (input.StartsWith(InvalidIdentifierEscapePrefix) && 
              ObjCReservedOrProblematicKeywords.Contains(input.Substring(InvalidIdentifierEscapePrefix.Length)))
            )
            {
                return InvalidIdentifierEscapePrefix+input;
            }
            
			return input;
		}
        
        public static string ToObjCIdentifierPrefix(this string input, string prefix)
        {
            return input.ToCamelCasePrefix(prefix).ToObjCIdentifier();
        }
        
        public static string ToObjCMethodSignatureParameter(this string input, string methodName, int paramPos)
        {
            if(paramPos==0)
            {
                return "With" + input.ToUpperFirstChar();
            }
            else return input.ToObjCMethodParameter();
        }
    
        public static string ToObjCMethodParameter(this string input)
        {
            return input.ToLowerFirstChar().ToObjCIdentifier();
        }
    
        public static string ToObjCNamespacePrefixedIdentifier(this string input)
        {
            return (ConfigurationService.Settings.NamespacePrefix
            +EntityContainerName
            +input.ToUpperFirstChar()).ToObjCIdentifier();
        }
        
        public static string ToObjCEnum(this string input)
        {
            return input.ToObjCNamespacePrefixedIdentifier();
        }
        
        public static string ToObjCEnumEntry(this string input, string entityName)
        {
            return entityName.ToObjCEnum()+input.ToUpperFirstChar();
        }
        
        public static string ToObjCInterface(this string input)
        {
            return input.ToObjCNamespacePrefixedIdentifier();
        }
        
        public static string ToObjCProperty(this string input)
        {
            return input.ToCamelCasePrefix("").ToObjCIdentifier();
        }
        
        public static string ToObjCMethod(this string input)
        {
            return input.ToLowerFirstChar().ToObjCIdentifier();
        }
        
        public static string ToObjCSetter(this string input)
        {
            return input.ToCamelCasePrefix("set").ToObjCIdentifier();
        }
        
        public static string ToObjCGetter(this string input, bool isBool=false)
        {
            //fix names that violate semantic rules for methods that
            //create owned objects

            StringComparison strCmp=StringComparison.CurrentCultureIgnoreCase;

            if(input.StartsWith("alloc",strCmp) || input.StartsWith("new",strCmp)
            || input.StartsWith("copy",strCmp) || input.StartsWith("mutableCopy",strCmp))
            {
                return input.ToCamelCasePrefix("get").ToObjCIdentifier();
            }
            

            return input.ToCamelCasePrefix("").ToObjCIdentifier();
        }

    }
}
