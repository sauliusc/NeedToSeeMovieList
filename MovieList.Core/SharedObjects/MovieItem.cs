using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.SharedObjects
{
    public class MovieItem
    {
        #region Properties

        public DateTime CreateDate { get; set; }

        public string DescriptionUrl { get; set; }

        public DateTime? Duration { get; set; }

        public string FileUrl { get; set; }

        public Guid Id { get; set; }

        public int Priority { get; set; }

        public int Ratio { get; set; }

        public bool Seen { get; set; }

        public DateTime? SeenTime { get; set; }

        public MovieTypes Type { get; set; }

        public string Title { get; set; }

        #endregion Properties
    }
}
