using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.User.Model;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace IdpServer.Application.User.Command.Handler
{
    public class SearchLoginTypeRequestHandler : IRequestHandler<SearchLoginType, BaseSearchResponse<LoginTypeDto>>
    {
        private readonly ILoginTypeService _service;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public SearchLoginTypeRequestHandler(ILoginTypeService service, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _service = service;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<BaseSearchResponse<LoginTypeDto>> Handle(SearchLoginType request, CancellationToken cancellationToken)
        {
            Stopwatch watch = Stopwatch.StartNew();
            var key = "idp_login_types";
            var dto = _memoryCache.Get<BaseSearchResponse<LoginTypeDto>>(key);
            var enableConsent = Convert.ToBoolean(_configuration["EnableConsent"] ?? "false");

            if (dto == null)
            {
                dto = new BaseSearchResponse<LoginTypeDto>();
                var loginTypes = await _service.GetLoginTypesAsync();
                if (!enableConsent)
                {
                    loginTypes = loginTypes.Where(x => x.Code != LoginTypes.SAP_CDC).ToList();
                }

                dto.AddRangeData(loginTypes);

                _memoryCache.Set(key, dto, TimeSpan.FromHours(1));
            }

            dto.PageIndex = 0;
            dto.PageSize = dto.Data.Count();
            dto.TotalCount = dto.Data.Count();

            watch.Stop();
            dto.DurationInMilisecond = watch.ElapsedMilliseconds;

            return dto;
        }
    }
}
