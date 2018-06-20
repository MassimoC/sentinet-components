using System.ServiceModel;
using Nevatech.Vsb.Repository.Processing;

namespace Canopus.Sentinet.PipelineComponents
{

    /// <summary>
    ///     Copy all the claims into the context property bag
    /// </summary>
    public sealed class ClaimsToContextPropertyBag : IMessageProcessor
    {
        void IMessageProcessor.ImportConfiguration(string configuration) { }

        public MessagePipelineResult ProcessMessage(MessagePipelineContext context)
        {

            var cp = OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties["ClaimsPrincipal"] as System.Security.Claims.ClaimsPrincipal;

            if (cp == null) return MessagePipelineResult.Continue;
            foreach (var claim in cp.Claims)
            {
                context.RoutingContext.Properties.Add(claim.Type, claim.Value);
            }

            return MessagePipelineResult.Continue;
        }

    }

}
