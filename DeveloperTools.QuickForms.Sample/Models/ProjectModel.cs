using System;

namespace DeveloperTools.QuickForms.Sample.Models
{
    public class ProjectModel
    {
        public String Name { get; set; }
        [EditorAttribute(EditorType = EditorFeatureType.OpenDirectory)]
        public String Destination { get; set; }
    }
}
