using AutoMapper;
using Retail.Application.Mappings;

namespace Retail.UnitTests
{
    public class TestBase
    {
        protected readonly IMapper _mapper;

        public TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }
}
