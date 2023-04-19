using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VNPTBusinessservice;
using VNPTPortalservice;
using VNPTPublishservice;

namespace Infrastructure.Webservice.Webservice.VNPT
{
    public static class WebServiceHelper
    {
        public static PortalServiceSoapClient PortalService(string Url)
        {
            var endpoint = new EndpointAddress(new Uri($"{Url}/portalservice.asmx"));
            return new PortalServiceSoapClient(PortalServiceSoapClient.EndpointConfiguration.PortalServiceSoap, endpoint);
            
        } 
        public static PublishServiceSoapClient PublishService(string Url)
        {
            var endpoint = new EndpointAddress(new Uri($"{Url}/publishservice.asmx"));
            return new PublishServiceSoapClient(PublishServiceSoapClient.EndpointConfiguration.PublishServiceSoap, endpoint);
         
        } 
        public static BusinessServiceSoapClient BusinessService(string Url)
        {
            var endpoint = new EndpointAddress(new Uri($"{Url}/businessservice.asmx"));
            return new BusinessServiceSoapClient(BusinessServiceSoapClient.EndpointConfiguration.BusinessServiceSoap, endpoint);
        }
    }
}
