﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BeautifulRestApi.Filters
{
    public class ResultEnrichingFilter : IAsyncResultFilter
    {
        private readonly IResultEnricher[] _enrichers;

        public ResultEnrichingFilter(IEnumerable<IResultEnricher> enrichers)
        {
            _enrichers = enrichers.ToArray();
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var asObjectResult = context.Result as ObjectResult;
            if (asObjectResult == null)
            {
                return Task.CompletedTask;
            }

            foreach (var enricher in _enrichers)
            {
                if (enricher.CanEnrich(asObjectResult.Value))
                {
                    enricher.Enrich(context, asObjectResult.Value);
                }
            }

            return next();
        }
    }
}
