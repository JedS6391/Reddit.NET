namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
	public class Thing<TData>
	{
		public string Id { get; set; }
        public string Name { get; set; }    
        public string Kind { get; set; }    
        public TData Data { get; set; }
	}
}