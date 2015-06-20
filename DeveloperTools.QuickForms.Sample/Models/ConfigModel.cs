using System;

namespace DeveloperTools.QuickForms.Sample.Models
{
    public enum ConfigModelType { ConfigSystem, ConfigWorker, ConfigNotification, ConfigDisplay }

    public class ConfigModel
    {
        public ConfigModelType Type { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
        [EditorAttribute(EditorType = EditorFeatureType.MultiLine)]
        public String Description { get; set; }
        public bool GenerateConstants { get; set; }
    }
}
