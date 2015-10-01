using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vipr.Core.CodeModel;
using Vipr.T4TemplateWriter.Settings;
using Vipr.T4TemplateWriter.Extensions;

using Vipr.T4TemplateWriter.Output;
using Vipr.T4TemplateWriter.TemplateProcessor;

namespace Vipr.T4TemplateWriter.CodeHelpers.ObjC {
    public class CodeWriterObjC : CodeWriterBase {

        public CodeWriterObjC() : base(){}

        public CodeWriterObjC(OdcmModel model) : base(model)
        {
            TypeHelperObjC.Prefix = GetPrefix();
        }

        public string GetPrefix()
        {
            if (this.CurrentModel != null)
            {
                return ConfigurationService.Settings.NamespacePrefix + this.CurrentModel.EntityContainer.Name;
            } 
            else 
            {
                return ConfigurationService.Settings.NamespacePrefix;
            }
        }

        public override String WriteOpeningCommentLine() 
        {
            return "/*******************************************************************************\n";
        }

        public override String WriteClosingCommentLine() 
        {
            return "\n******************************************************************************/";
        }

        public override string WriteInlineCommentChar() 
        {
            return "// ";
        }
    
        public string NewStringFromIntegerVariable(string variableName)
        {
            return "[[NSString alloc] initWithFormat:@\"%d\", " + variableName + "]";
        }

    }
}
