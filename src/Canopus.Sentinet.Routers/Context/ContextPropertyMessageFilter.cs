using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Nevatech.Vsb.Repository.Monitoring;


namespace Canopus.Sentinet.Routers.Context
{
    public sealed class ContextPropertyMessageFilter : MessageFilter
    {
        private const string Ctxpropertyobject = "Nevatech.Vsb.Repository.Monitoring.RoutingContextMessageProperty";
        #region Properties

        public String PropertyName { get; set; }
        public String PropertyValue { get; set; }

        #endregion

        #region Methods

        public override bool Match(Message message)
        {
            var keyList = message.Properties.Keys.ToList();
            if (!keyList.Contains(Ctxpropertyobject)) return false;

            var routingProps = (RoutingContextMessageProperty)message.Properties.Values.ToList()[keyList.IndexOf(Ctxpropertyobject)];
            var propKeys = routingProps.Properties.Keys.ToList();

            if (!propKeys.Contains(PropertyName)) return false;
            var vPropertyValue = routingProps.Properties.Values.ToList()[propKeys.IndexOf(PropertyName)];
            if (vPropertyValue != null) return (string)vPropertyValue == PropertyValue;
            return false;
        }


        public override bool Match(MessageBuffer buffer)
        {
            return false;
        }


        private bool Match()
        {
            return false;
        }


        #endregion
    }
}
