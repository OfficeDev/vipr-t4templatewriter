using System;
using System.Collections.Generic;
using System.Linq;
using Vipr.Core.CodeModel;

namespace Vipr.T4TemplateWriter.CodeHelpers
{
    public class Node
    {
        public Node Parent { get; set; }
        public OdcmProperty Property { get; set; }
        public IList<Node> ChildProperties { get; set; }


        public Node(Node parent, OdcmProperty mainProperty, IList<Node> childProperties)
        {
            this.Parent = parent;
            this.Property = mainProperty;
            this.ChildProperties = childProperties;

        }

        public Node(Node parent, OdcmProperty mainProperty)
        {
            this.Parent = parent;
            this.Property = mainProperty;
            this.ChildProperties = new List<Node>();

        }

        public void GenerateGraph()
        {
            if (this.ChildProperties != null && this.ChildProperties.Any())
            {
                foreach (Node c in this.ChildProperties)
                {
                    var clazz = c.Property.Type as OdcmClass;
                    if (clazz != null)
                    {
                        var childProp = clazz.Properties == null ? new List<OdcmProperty>() : clazz.Properties.WhereIsNavigation();

                        foreach (var odcmProperty in childProp)
                        {
                            var n1 = new Node(c, odcmProperty);
                            c.ChildProperties.Add(n1);
                        }
                        c.GenerateGraph();
                    }
                }
            }
        }
    }
}