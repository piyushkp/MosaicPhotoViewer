﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhotoCtmService.PhotoCtmDownload {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PhotoCtmDownload.utlSoap")]
    public interface utlSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/geturlandphotoidtable", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable geturlandphotoidtable(int eid, System.DateTime pdate, int pid);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface utlSoapChannel : PhotoCtmDownload.utlSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class utlSoapClient : System.ServiceModel.ClientBase<PhotoCtmDownload.utlSoap>, PhotoCtmDownload.utlSoap {
        
        public utlSoapClient() {
        }
        
        public utlSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public utlSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public utlSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public utlSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataTable geturlandphotoidtable(int eid, System.DateTime pdate, int pid) {
            return base.Channel.geturlandphotoidtable(eid, pdate, pid);
        }
    }
}
