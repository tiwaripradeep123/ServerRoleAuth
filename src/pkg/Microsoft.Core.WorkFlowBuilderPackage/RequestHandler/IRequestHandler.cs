using CoreFeed.Core.Com.Entity.App;
using System.Threading.Tasks;

  namespace CoreFeed.Business.Core
    {
        public interface IRequestHandler
        {
            Task<Response> Execute(Request request);
        }
    }
