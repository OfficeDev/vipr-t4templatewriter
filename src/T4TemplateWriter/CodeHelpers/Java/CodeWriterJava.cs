using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vipr.Core.CodeModel;

namespace Vipr.T4TemplateWriter.CodeHelpers.Java
{
    public class CodeWriterJava : CodeWriterBase
    {

        public CodeWriterJava() : base() { }
        public CodeWriterJava(OdcmModel model) : base(model) { }

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

        public List<SubElementDefinition> GetSubElementDefinitions(Node node)
        {
            var parent = node.Parent != null ? node.Parent.Property : null;
            IEnumerable<SubElementDefinition> list = new List<SubElementDefinition>();
            foreach (var childProperty in node.ChildProperties)
            {
                var subList = GetSubElementDefinitionsRecursively(parent, childProperty.Property, true);
                list = list.Union(subList);
            }

            return list.ToList();
        }

        private List<SubElementDefinition> GetSubElementDefinitionsRecursively(OdcmProperty parent, OdcmProperty child, bool isNavigation)
        {
            var list = new List<SubElementDefinition>();
            if (child != null && child.IsCollection && parent != null && !isNavigation)
            {
                var sd = new SubElementDefinition(child.Name, child.Type, parent.Type);
                if (!list.Contains(sd))
                {
                    list.Add(sd);
                }
            }

            var clazz = child.Type as OdcmClass;
            if (clazz != null)
            {
                if (clazz.Properties != null)
                {
                    var nav = clazz.NavigationProperties().ToList();
                    foreach (var p in clazz.Properties)
                    {
                        var subList = GetSubElementDefinitionsRecursively(child, p, nav.Contains(p));
                        list= list.Union(subList).ToList();
                    }
                }
            }

            return list;
        }
    }
}
