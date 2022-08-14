using CoreFeed.Core.Com.Entity.App;
using System.Collections.Generic;

namespace CoreFeed.Core.WorkFlowBuilderPackage.Funnel
{
    /// <summary>
    /// Default Data to be filled in Auth token<seealso cref="ClaimDataExtractor"/>
    /// <para>For customization override ClaimDataExtractor.ExtractData() and ClaimDataExtractorRegistration in your registration.</para>
    /// </summary>
    public interface IClaimDataExtractor
    {
        /// <summary>
        /// Pair to be filled if specified.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ExtractData(object extractedString);
    }

    public abstract class ClaimDataExtractor : IClaimDataExtractor
    {
        public virtual Dictionary<string, string> ExtractData(object extractedString)
        {
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// For a given request if user is authenticated, then configure additional authorizations at server for the request.
    /// You should implement class <see cref="ServerRoleAuthorization"/>
    /// <para>
    /// By default, it is not configured.
    /// Make sure to throw <see cref="System.UnauthorizedAccessException "/> when user is not authorized.
    /// </para>
    /// <para>
    /// E.g. 
    /// <code>
    /// throw new <see cref="System.UnauthorizedAccessException "/>("User is not authorized for the request.");
    /// </code>
    /// </para>
    /// </summary>
    public interface IServerRoleAuthorization
    {
        /// <summary>
        /// For a given request if user is authenticated, then configure additional authorizations at server for the request.
        /// <para>
        /// By default, it is not configured.
        /// Make sure to throw <see cref="System.UnauthorizedAccessException "/> when user is not authorized.
        /// </para>
        /// <para>
        /// E.g. 
        /// <code>
        /// throw new <see cref="System.UnauthorizedAccessException "/>("User is not authorized for the request.");
        /// </code>
        /// </para>
        /// </summary>
        void IsActionAuthorizedByServer(Request request);
    }

    /// <summary>
    /// Configure server role authorizations.
    /// </summary>
    public abstract class ServerRoleAuthorization : IServerRoleAuthorization
    {
        /// <summary>
        /// For a given request if user is authenticated, then configure additional authorizations at server for the request.
        /// <para>
        /// By default, it is not configured.
        /// Make sure to throw <see cref="System.UnauthorizedAccessException "/> when user is not authorized.
        /// </para>
        /// <para>
        /// E.g.
        /// <code>
        /// throw new <see cref="System.UnauthorizedAccessException "/>("User is not authorized for the request.");
        /// </code>
        /// </para>
        /// </summary>
        public virtual void IsActionAuthorizedByServer(Request request)
        {

        }
    }
}