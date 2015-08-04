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
        public List<Node> ChildProperties { get; set; }
        public List<OdcmProperty> Fields { get; set; }


        public Node(Node parent, OdcmProperty mainProperty, List<Node> childProperties)
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
            this.Fields = new List<OdcmProperty>();

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
                        if (clazz.Properties != null)
                        {
                            var childProp = clazz.Properties.WhereIsNavigation().ToList();
                            foreach (var odcmProperty in childProp)
                            {
                                var n1 = new Node(c, odcmProperty);
                                c.ChildProperties.Add(n1);
                            }

                            c.Fields.AddRange(clazz.Properties.Except(childProp));
                        }
                        
                        c.GenerateGraph();
                    }
                }
            }
        }
    }
}