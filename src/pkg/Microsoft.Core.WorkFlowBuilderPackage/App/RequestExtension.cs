using Newtonsoft.Json;

namespace CoreFeed.Core.Com.Entity.App
{
    public static class RequestExtension
    {
        public static TRequestType ExtractRequest<TRequestType>(this Request request) where TRequestType : class
        {
            var response = JsonConvert.DeserializeObject<TRequestType>(request.RequestParam.ToString());

            if (typeof(TRequestType).IsSubclassOf(typeof(Request)))
            {
                var parentRequest = response as Request;
                (parentRequest).ExecutionContext = request.ExecutionContext;
                (parentRequest).PageCriteria = request.PageCriteria;
                (parentRequest).FilterByCriteria = request.FilterByCriteria;
                (parentRequest).RequestType = request.RequestType;
                (parentRequest).RequestSubType = request.RequestSubType;
                (parentRequest).DTO = request.DTO;
                (parentRequest).OrderByCriteria = request.OrderByCriteria;
            }

            return response;
        }
    }
}