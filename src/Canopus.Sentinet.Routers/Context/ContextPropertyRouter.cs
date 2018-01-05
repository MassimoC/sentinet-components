using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Nevatech.Vsb.Repository;
using Nevatech.Vsb.Repository.Services;

namespace Canopus.Sentinet.Routers.Context
{
    public sealed class ContextPropertyRouter : IRouter
    {
        #region Constants

        private const string ConfigurationRootElementName = "Routes";
        private const string InvalidRouterConfiguration = "Router configuration is invalid. Outbound endpoint '{0}' does not exist.";

        #endregion

        #region Properties

        public Collection<RoutingRule> RoutingRules { get; private set; }

        #endregion

        #region Constructors

        public ContextPropertyRouter()
        {
            RoutingRules = new Collection<RoutingRule>();
        }

        #endregion

        #region IRouter Members

        /// <summary>
        ///     Exports router configuration.
        /// </summary>
        /// <returns>
        ///     String that defines router configuration.
        /// </returns>
        public string ExportConfiguration()
        {
            var result = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, NewLineOnAttributes = false };

            using (var writer = XmlWriter.Create(result, settings))
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                var serializer = new XmlSerializer(typeof(Collection<Route>), new XmlRootAttribute(ConfigurationRootElementName));
                serializer.Serialize(writer, RoutingRules, namespaces);
            }

            return result.ToString();
        }

        /// <summary>
        ///     Imports router configuration.
        /// </summary>
        /// <param name="configuration">
        ///     String that defines router configuration.
        /// </param>
        public void ImportConfiguration(string configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            using (var textReader = new StringReader(configuration))
            {
                var serializer = new XmlSerializer(typeof(Collection<RoutingRule>),
                    new XmlRootAttribute(ConfigurationRootElementName));

                RoutingRules = (Collection<RoutingRule>)serializer.Deserialize(textReader);
            }
        }

        /// <summary>
        ///     Returns collection of routes that will be used to populate
        ///     <see cref="MessageFilterTable&lt;ServiceEndpoint&gt;" /> of the routing service.
        /// </summary>
        /// <param name="backendEndpoints">
        ///     All available endpoints in the outbound endpoint group that this
        ///     router instance is bound to.
        /// </param>
        /// <returns>
        ///     Collection of routes that will be used to populate 
        ///     <see cref="MessageFilterTable&lt;ServiceEndpoint&gt;" /> of the routing service.
        /// </returns>
        public IEnumerable<Route> GetRoutes(IEnumerable<EndpointDefinition> backendEndpoints)
        {
            if (backendEndpoints == null) throw new ArgumentNullException("backendEndpoints");

            // Validate router configuration
            if (!Validate()) throw new ValidationException(ErrorMessage);

            var routes = new Collection<Route>();
            var routeEndpoints = new Collection<EndpointDefinition>();
            var priority = Byte.MaxValue;

            foreach (RoutingRule routingRule in RoutingRules)
            {
                // Collection can be reused as endpoints are copied in Route() constructor
                routeEndpoints.Clear();

                // collection of the backend endpoint per routingRule 
                foreach (string endpointUri in routingRule.Endpoints)
                {
                    // Find outbound endpoint by its AbsoluteURI
                    EndpointDefinition endpoint = backendEndpoints.FirstOrDefault(e => String.Equals(e.LogicalAddress.AbsoluteUri, endpointUri, StringComparison.OrdinalIgnoreCase));
                    if (endpoint == null) throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InvalidRouterConfiguration, endpointUri));
                    routeEndpoints.Add(endpoint);
                }

                // build a route for each routingRule
                var filter = new ContextPropertyMessageFilter
                {
                    PropertyName = routingRule.Name,
                    PropertyValue = routingRule.Value
                };
                // endpoint Fallback scenario
                routes.Add(new Route(filter, routeEndpoints, priority));
                priority--;
            }

            return routes;
        }

        #endregion


        #region IValidator Members

        public string ErrorMessage { get; private set; }

        /// <summary>
        ///     Evaluates the state of the object.
        /// </summary>
        /// <returns>
        ///     True if state is valid; otherwise, false.
        /// </returns>
        public bool Validate()
        {
            ErrorMessage = null;

            if (RoutingRules.Count == 0)
            {
                ErrorMessage = "RoutingRule has no routing destinations.";
                return false;
            }

            foreach (RoutingRule routingRule in RoutingRules)
            {
                if (string.IsNullOrEmpty(routingRule.Name))
                {
                    ErrorMessage = "The RoutingRule name is empty'.";
                    return false;
                }

                if (routingRule.Endpoints.Count == 0)
                {
                    ErrorMessage = "RoutingRule  has no endpoints specified.";
                    return false;
                }

                if (routingRule.Endpoints.Any(e => String.IsNullOrWhiteSpace(e)))
                {
                    ErrorMessage = "RoutingRule has invalid endpoint name.";
                    return false;
                }

                if (routingRule.Endpoints.Distinct().Count() != routingRule.Endpoints.Count)
                {
                    ErrorMessage = "Endpoints names in a destination must be unique.";
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
