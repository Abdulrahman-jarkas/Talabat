namespace Talabat.Vender.Endpoints.GetPaginatedProducts;

public class GetPaginatedResultRequest
{
	public int PageSize { get; set; }
	public int? LastId { get; set; }
}