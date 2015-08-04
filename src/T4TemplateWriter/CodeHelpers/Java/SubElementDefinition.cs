using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vipr.Core.CodeModel;

namespace Vipr.T4TemplateWriter.CodeHelpers.Java
{
    public class SubElementDefinition
    {
        public OdcmType PropertyType { get; set; }
        public OdcmType ParentType { get; set; }
        public String PropertyName { get; set; }

        public SubElementDefinition(String propertyName, OdcmType propertyType, OdcmType parentType)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            ParentType = parentType;
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as SubElementDefinition;
            if (toCompare != null)
            {
                return
                    toCompare.ParentType == ParentType && toCompare.PropertyName ==
                    PropertyName && toCompare.PropertyType == PropertyType;
            }
            
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return PropertyType.GetHashCode() ^ PropertyName.GetHashCode() ^ ParentType.GetHashCode();
        }
    }
}
