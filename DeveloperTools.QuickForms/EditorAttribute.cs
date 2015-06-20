using System;

namespace DeveloperTools.QuickForms
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditorAttribute : Attribute
    {
        public String DisplayName { get; set; }

        public EditorFeatureType EditorType { get; set; }
    }
}
