﻿using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.Queries.Orders {
  public class GetOrderQueryResult {
    public int OrderId { get; }
    public int CunsomerId { get; }
    public decimal Total { get; }
    public List<string> Products { get; }

    public GetOrderQueryResult(int orderId, int cunsomerId, decimal total, List<Product> products) {
      OrderId = orderId;
      CunsomerId = cunsomerId;
      Total = total;
      Products = products.Select(x => x.Details).ToList();
    }
  }
}
