using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Nevatech.Vsb.Repository.Processing;

namespace Canopus.Sentinet.PipelineComponents
{
    public sealed class MediaFormatter : IMessageProcessor
    {
        void IMessageProcessor.ImportConfiguration(string configuration) { }
        private const string downstreamError = "downstreamError";
        private string[] innerExceptions = {"stack trace", "inner exception", "System.ServiceModel"};

        public MessagePipelineResult ProcessMessage(MessagePipelineContext context)
        {         

            // basic example of soap formatter
            if (context.Message.IsFault)
            {
                if (!context.GetMessageBodyContent(true).ContainsAny(innerExceptions))
                    return MessagePipelineResult.Continue;

                Guid? transactionId = Nevatech.Vsb.Repository.Monitoring.MonitoringMessageProperty.Get(context.Message).Transaction.TransactionId;
                var messageDetails = String.Format("An error occurred while processing the message. Please contact the service owner. Reference ID : [{0}]", transactionId);
                context.Message = Message.CreateMessage(context.Message.Version, MessageFault.CreateFault(new FaultCode(downstreamError), messageDetails), "action");
            }

            return MessagePipelineResult.Continue;
        }
    }

    public static class MyExtensions
    {
        public static bool ContainsAny(this string value, params string[] search)
        {
            foreach (string item in search)
            {
                if (value.Contains(item))
                    return true;
            }

            return false;
        }
    }  


}
