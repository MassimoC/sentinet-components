using System;
using Nevatech.Vsb.Repository.Processing;


namespace Canopus.Sentinet.PipelineComponents
{
    /// <summary>
    ///     Copy all the query parameters the context property bag
    /// </summary>
    public sealed class QueryParameters : IMessageProcessor
    {
        void IMessageProcessor.ImportConfiguration(string configuration) { }

        public MessagePipelineResult ProcessMessage(MessagePipelineContext context)
        {
            var qs = context.RoutingContext.RequestMatch.UriTemplateMatch.QueryParameters;

            foreach (var key in qs.AllKeys)
            {
                context.RoutingContext.Properties.Add(String.Format("ctxQS_{0}", key), context.RoutingContext.RequestMatch.UriTemplateMatch.QueryParameters[key]);
            }

            return MessagePipelineResult.Continue;
        }

    }

}
