namespace REDTransport.NET.Server.AspNet
{
    public class RedTransportEndpointConfiguration
    {
        public string Route { get; set; }
        
        public string[] WhiteListSubRoutes { get; set; }
        public string[] BlackListSubRoutes { get; set; }
        
        public string MapToRoute { get; set; }


        internal bool IsMatched(string route)
        {
            return false;
        }
    }
}