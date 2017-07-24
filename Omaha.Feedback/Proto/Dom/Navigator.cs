using ProtoBuf;

namespace Omaha.Feedback.Proto.Dom
{
    [ProtoContract]
    public class Navigator
    {
        [ProtoMember(1, IsRequired=false)]
        // The value of 'navigator.appCodeName' property.
        public string AppCodeName { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // The value of 'navigator.appName' property.
        public string AppName { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // The value of 'navigator.appVersion' property.
        public string AppVersion { get; set; }

        [ProtoMember(4, IsRequired=false)]
        // The value of 'navigator.appMinorVersion' property.
        public string AppMinorVersion { get; set; }
        
        [ProtoMember(5, IsRequired=false)]
        // The value of 'navigator.cookieEnabled' property.
        public bool CookieEnabled { get; set; }
        
        [ProtoMember(6, IsRequired=false)]
        // The value of 'navigator.cpuClass' property.
        public string CpuClass { get; set; }
        
        [ProtoMember(7, IsRequired=false)]
        // The value of 'navigator.onLine' property.
        public bool OnLine { get; set; }
        
        [ProtoMember(8, IsRequired=false)]
        // The value of 'navigator.platform' property.
        public string Platform { get; set; }
        
        [ProtoMember(9, IsRequired=false)]
        // The value of 'navigator.browserLanguage' property.
        public string BrowserLanguage { get; set; }
        
        [ProtoMember(10, IsRequired=false)]
        // The value of 'navigator.systemLanguage' property.
        public string SystemLanguage { get; set; }
        
        [ProtoMember(11, IsRequired=false)]
        // The value of 'navigator.userAgent' property.
        public string UserAgent { get; set; }
        
        [ProtoMember(12, IsRequired=false)]
        // The return value of 'navigator.javaEnabled()' method.
        public bool JavaEnabled { get; set; }
        
        [ProtoMember(13, IsRequired=false)]
        // The return value of 'navigator.taintEnabled()' method.
        public bool TaintEnabled { get; set; }

        [ProtoMember(14, IsRequired=false)]
        // Plugin names specified by 'navigator.plugins' property.
        public string[] PluginNames { get; set; }
    }
}
