using AutoMapper;
using MovieList.Core.SharedObjects;
using MovieList.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI
{
    public class GuiObjectMapper
    {
        public IMapper Mapper { get; set; }
        public GuiObjectMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MovieItem, MovieItemModel>().ReverseMap();
            });
            configuration.AssertConfigurationIsValid();
            Mapper = configuration.CreateMapper();
        }
    }
}
