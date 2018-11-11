using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Producer.Repositories;

namespace Producer.Infrastructure
{
    public class TransactionFilter : ActionFilterAttribute
    {
        private readonly IDatabase _db;

        public TransactionFilter(IDatabase db)
        {
            _db = db;
        }
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                _db.BeginTransaction();
            }
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (_db.Transaction != null && context.Exception == null)
            {
                _db.Commit();
            }
        }
    }
}